using bgs.RPCServices;
using bgs.types;
using bnet.protocol;
using bnet.protocol.account;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace bgs
{
	public class AccountAPI : BattleNetAPI
	{
		private ServiceDescriptor m_accountService = new bgs.RPCServices.AccountService();

		private ServiceDescriptor m_accountNotify = new AccountNotify();

		private uint m_preferredRegion = -1;

		private string m_accountCountry;

		private List<AccountLicense> m_licenses = new List<AccountLicense>();

		public ServiceDescriptor AccountNotifyService
		{
			get
			{
				return this.m_accountNotify;
			}
		}

		public ServiceDescriptor AccountService
		{
			get
			{
				return this.m_accountService;
			}
		}

		public int GameSessionRunningCount
		{
			get;
			set;
		}

		public bool HasLicenses
		{
			get
			{
				if (this.m_licenses == null)
				{
					return false;
				}
				return this.m_licenses.Count > 0;
			}
		}

		public AccountAPI.GameSessionInfo LastGameSessionInfo
		{
			get;
			set;
		}

		public AccountAPI(BattleNetCSharp battlenet) : base(battlenet, "Account")
		{
		}

		public bool CheckLicense(uint licenseId)
		{
			bool flag;
			if (this.m_licenses == null || this.m_licenses.Count == 0)
			{
				return false;
			}
			List<AccountLicense>.Enumerator enumerator = this.m_licenses.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Id != licenseId)
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				((IDisposable)(object)enumerator).Dispose();
			}
			return flag;
		}

		public string GetAccountCountry()
		{
			return this.m_accountCountry;
		}

		private void GetAccountLevelInfo(bnet.protocol.EntityId accountId)
		{
			GetAccountStateRequest getAccountStateRequest = new GetAccountStateRequest();
			getAccountStateRequest.SetEntityId(accountId);
			AccountFieldOptions accountFieldOption = new AccountFieldOptions();
			accountFieldOption.SetFieldAccountLevelInfo(true);
			getAccountStateRequest.SetOptions(accountFieldOption);
			this.m_rpcConnection.QueueRequest(this.m_accountService.Id, 30, getAccountStateRequest, new RPCContextDelegate(this.GetAccountStateCallback), 0);
		}

		private void GetAccountStateCallback(RPCContext context)
		{
			if (context == null || context.Payload == null)
			{
				base.ApiLog.LogWarning("GetAccountLevelInfo invalid context!");
				return;
			}
			BattleNetErrors status = (BattleNetErrors)context.Header.Status;
			if (status != BattleNetErrors.ERROR_OK)
			{
				base.ApiLog.LogError("GetAccountLevelInfo failed with error={0}", new object[] { status.ToString() });
				return;
			}
			GetAccountStateResponse getAccountStateResponse = GetAccountStateResponse.ParseFrom(context.Payload);
			if (getAccountStateResponse == null || !getAccountStateResponse.IsInitialized)
			{
				base.ApiLog.LogWarning("GetAccountStateCallback unable to parse response!");
				return;
			}
			if (!getAccountStateResponse.HasState || !getAccountStateResponse.State.HasAccountLevelInfo)
			{
				base.ApiLog.LogWarning("GetAccountStateCallback response has no data!");
				return;
			}
			GetAccountStateRequest request = (GetAccountStateRequest)context.Request;
			if (request != null && request.EntityId == this.m_battleNet.AccountId)
			{
				AccountLevelInfo accountLevelInfo = getAccountStateResponse.State.AccountLevelInfo;
				this.m_preferredRegion = accountLevelInfo.PreferredRegion;
				this.m_accountCountry = accountLevelInfo.Country;
				base.ApiLog.LogDebug("Region (preferred): {0}", new object[] { this.m_preferredRegion });
				base.ApiLog.LogDebug("Country (account): {0}", new object[] { this.m_accountCountry });
				if (accountLevelInfo.LicensesList.Count <= 0)
				{
					base.ApiLog.LogWarning("No licenses found!");
				}
				else
				{
					this.m_licenses.Clear();
					base.ApiLog.LogDebug("Found {0} licenses.", new object[] { accountLevelInfo.LicensesList.Count });
					for (int i = 0; i < accountLevelInfo.LicensesList.Count; i++)
					{
						AccountLicense item = accountLevelInfo.LicensesList[i];
						this.m_licenses.Add(item);
						base.ApiLog.LogDebug("Adding license id={0}", new object[] { item.Id });
					}
				}
			}
			base.ApiLog.LogDebug(string.Concat("GetAccountLevelInfo, status=", status.ToString()));
		}

		public void GetGameAccountState(GetGameAccountStateRequest request, RPCContextDelegate callback)
		{
			this.m_rpcConnection.QueueRequest(this.m_accountService.Id, 31, request, callback, 0);
		}

		public void GetPlayRestrictions(ref Lockouts restrictions, bool reload)
		{
			if (!reload)
			{
				if (this.LastGameSessionInfo != null)
				{
					restrictions.loaded = true;
					restrictions.sessionStartTime = this.LastGameSessionInfo.SessionStartTime;
					return;
				}
				if (this.GameSessionRunningCount > 0)
				{
					return;
				}
			}
			this.LastGameSessionInfo = null;
			AccountAPI gameSessionRunningCount = this;
			gameSessionRunningCount.GameSessionRunningCount = gameSessionRunningCount.GameSessionRunningCount + 1;
			GetGameSessionInfoRequest getGameSessionInfoRequest = new GetGameSessionInfoRequest();
			getGameSessionInfoRequest.SetEntityId(this.m_battleNet.GameAccountId);
			AccountAPI.GetGameSessionInfoRequestContext getGameSessionInfoRequestContext = new AccountAPI.GetGameSessionInfoRequestContext(this);
			this.m_rpcConnection.QueueRequest(this.m_accountService.Id, 34, getGameSessionInfoRequest, new RPCContextDelegate(getGameSessionInfoRequestContext.GetGameSessionInfoRequestContextCallback), 0);
		}

		public uint GetPreferredRegion()
		{
			return this.m_preferredRegion;
		}

		private void HandleAccountNotify_AccountStateUpdated(RPCContext context)
		{
			if (context == null || context.Payload == null)
			{
				base.ApiLog.LogWarning("HandleAccountNotify_AccountStateUpdated invalid context!");
				return;
			}
			AccountStateNotification accountStateNotification = AccountStateNotification.ParseFrom(context.Payload);
			if (accountStateNotification == null || !accountStateNotification.IsInitialized)
			{
				base.ApiLog.LogWarning("HandleAccountNotify_AccountStateUpdated unable to parse response!");
				return;
			}
			if (!accountStateNotification.HasState)
			{
				base.ApiLog.LogDebug("HandleAccountNotify_AccountStateUpdated HasState=false, data={0}", new object[] { accountStateNotification });
				return;
			}
			AccountState state = accountStateNotification.State;
			if (!state.HasAccountLevelInfo)
			{
				base.ApiLog.LogDebug("HandleAccountNotify_AccountStateUpdated HasAccountLevelInfo=false, data={0}", new object[] { accountStateNotification });
				return;
			}
			if (!state.AccountLevelInfo.HasPreferredRegion)
			{
				base.ApiLog.LogDebug("HandleAccountNotify_AccountStateUpdated HasPreferredRegion=false, data={0}", new object[] { accountStateNotification });
				return;
			}
			base.ApiLog.LogDebug("HandleAccountNotify_AccountStateUpdated, data={0}", new object[] { accountStateNotification });
		}

		private void HandleAccountNotify_GameAccountStateUpdated(RPCContext context)
		{
			GameAccountStateNotification gameAccountStateNotification = GameAccountStateNotification.ParseFrom(context.Payload);
			base.ApiLog.LogDebug(string.Concat("HandleAccountNotify_GameAccountStateUpdated, data=", gameAccountStateNotification));
		}

		private void HandleAccountNotify_GameAccountsUpdated(RPCContext context)
		{
			GameAccountNotification gameAccountNotification = GameAccountNotification.ParseFrom(context.Payload);
			base.ApiLog.LogDebug(string.Concat("HandleAccountNotify_GameAccountsUpdated, data=", gameAccountNotification));
		}

		private void HandleAccountNotify_GameSessionUpdated(RPCContext context)
		{
			GameAccountSessionNotification gameAccountSessionNotification = GameAccountSessionNotification.ParseFrom(context.Payload);
			base.ApiLog.LogDebug(string.Concat("HandleAccountNotify_GameSessionUpdated, data=", gameAccountSessionNotification));
		}

		public override void Initialize()
		{
			base.ApiLog.LogDebug("Account API initializing");
			base.Initialize();
			this.GetAccountLevelInfo(this.m_battleNet.AccountId);
		}

		public override void InitRPCListeners(RPCConnection rpcConnection)
		{
			base.InitRPCListeners(rpcConnection);
			this.m_rpcConnection.RegisterServiceMethodListener(this.m_accountNotify.Id, 1, new RPCContextDelegate(this.HandleAccountNotify_AccountStateUpdated));
			this.m_rpcConnection.RegisterServiceMethodListener(this.m_accountNotify.Id, 2, new RPCContextDelegate(this.HandleAccountNotify_GameAccountStateUpdated));
			this.m_rpcConnection.RegisterServiceMethodListener(this.m_accountNotify.Id, 3, new RPCContextDelegate(this.HandleAccountNotify_GameAccountsUpdated));
			this.m_rpcConnection.RegisterServiceMethodListener(this.m_accountNotify.Id, 4, new RPCContextDelegate(this.HandleAccountNotify_GameSessionUpdated));
		}

		public override void OnDisconnected()
		{
			base.OnDisconnected();
		}

		private void SubscribeToAccountService()
		{
			SubscriptionUpdateRequest subscriptionUpdateRequest = new SubscriptionUpdateRequest();
			SubscriberReference subscriberReference = new SubscriberReference();
			subscriberReference.SetEntityId(this.m_battleNet.AccountId);
			subscriberReference.SetObjectId((ulong)0);
			AccountFieldOptions accountFieldOption = new AccountFieldOptions();
			accountFieldOption.SetAllFields(true);
			subscriberReference.SetAccountOptions(accountFieldOption);
			subscriptionUpdateRequest.AddRef(subscriberReference);
			subscriberReference = new SubscriberReference();
			subscriberReference.SetEntityId(this.m_battleNet.GameAccountId);
			subscriberReference.SetObjectId((ulong)0);
			(new GameAccountFieldOptions()).SetAllFields(true);
			subscriptionUpdateRequest.AddRef(subscriberReference);
			this.m_rpcConnection.QueueRequest(this.m_accountService.Id, 25, subscriptionUpdateRequest, new RPCContextDelegate(this.SubscribeToAccountServiceCallback), 0);
		}

		private void SubscribeToAccountServiceCallback(RPCContext context)
		{
			BattleNetErrors status = (BattleNetErrors)context.Header.Status;
			if (status != BattleNetErrors.ERROR_OK)
			{
				base.ApiLog.LogError(string.Concat("SubscribeToAccountServiceCallback: ", status.ToString()));
				return;
			}
			base.ApiLog.LogDebug(string.Concat("SubscribeToAccountServiceCallback, status=", status.ToString()));
		}

		public class GameSessionInfo
		{
			public ulong SessionStartTime;

			public GameSessionInfo()
			{
			}
		}

		private class GetGameSessionInfoRequestContext
		{
			private AccountAPI m_parent;

			public GetGameSessionInfoRequestContext(AccountAPI parent)
			{
				this.m_parent = parent;
			}

			public void GetGameSessionInfoRequestContextCallback(RPCContext context)
			{
				AccountAPI mParent = this.m_parent;
				mParent.GameSessionRunningCount = mParent.GameSessionRunningCount - 1;
				if (context == null || context.Payload == null)
				{
					this.m_parent.ApiLog.LogWarning("GetPlayRestrictions invalid context!");
					return;
				}
				BattleNetErrors status = (BattleNetErrors)context.Header.Status;
				if (status != BattleNetErrors.ERROR_OK)
				{
					this.m_parent.ApiLog.LogError("GetPlayRestrictions failed with error={0}", new object[] { status.ToString() });
					return;
				}
				GetGameSessionInfoResponse getGameSessionInfoResponse = GetGameSessionInfoResponse.ParseFrom(context.Payload);
				if (getGameSessionInfoResponse == null || !getGameSessionInfoResponse.IsInitialized)
				{
					this.m_parent.ApiLog.LogWarning("GetPlayRestrictions unable to parse response!");
					return;
				}
				if (!getGameSessionInfoResponse.HasSessionInfo)
				{
					this.m_parent.ApiLog.LogWarning("GetPlayRestrictions response has no data!");
					return;
				}
				this.m_parent.LastGameSessionInfo = new AccountAPI.GameSessionInfo();
				if (!getGameSessionInfoResponse.SessionInfo.HasStartTime)
				{
					this.m_parent.ApiLog.LogWarning("GetPlayRestrictions response has no HasStartTime!");
				}
				else
				{
					this.m_parent.LastGameSessionInfo.SessionStartTime = (ulong)getGameSessionInfoResponse.SessionInfo.StartTime;
				}
			}
		}
	}
}