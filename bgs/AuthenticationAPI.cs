using bgs.RPCServices;
using bgs.types;
using bnet.protocol;
using bnet.protocol.authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace bgs
{
	public class AuthenticationAPI : BattleNetAPI
	{
		private ServiceDescriptor m_authServerService = new bgs.RPCServices.AuthServerService();

		private ServiceDescriptor m_authClientService = new bgs.RPCServices.AuthClientService();

		private QueueInfo m_queueInfo;

		private List<bnet.protocol.EntityId> m_gameAccounts;

		private bnet.protocol.EntityId m_accountEntity;

		private bnet.protocol.EntityId m_gameAccount;

		private string m_battleTag;

		private byte[] m_sessionKey;

		private bool m_authenticationFailure;

		public bnet.protocol.EntityId AccountId
		{
			get
			{
				return this.m_accountEntity;
			}
		}

		public ServiceDescriptor AuthClientService
		{
			get
			{
				return this.m_authClientService;
			}
		}

		public ServiceDescriptor AuthServerService
		{
			get
			{
				return this.m_authServerService;
			}
		}

		public string BattleTag
		{
			get
			{
				return this.m_battleTag;
			}
		}

		public bnet.protocol.EntityId GameAccountId
		{
			get
			{
				return this.m_gameAccount;
			}
		}

		public byte[] SessionKey
		{
			get
			{
				return this.m_sessionKey;
			}
		}

		public AuthenticationAPI(BattleNetCSharp battlenet) : base(battlenet, "Authentication")
		{
		}

		public bool AuthenticationFailure()
		{
			return this.m_authenticationFailure;
		}

		public List<bnet.protocol.EntityId> GetGameAccountList()
		{
			return this.m_gameAccounts;
		}

		public void GetQueueInfo(ref QueueInfo queueInfo)
		{
			queueInfo.position = this.m_queueInfo.position;
			queueInfo.end = this.m_queueInfo.end;
			queueInfo.stdev = this.m_queueInfo.stdev;
			queueInfo.changed = this.m_queueInfo.changed;
			this.m_queueInfo.changed = false;
		}

		private void HandleGameAccountSelected(RPCContext context)
		{
			GameAccountSelectedRequest gameAccountSelectedRequest = GameAccountSelectedRequest.ParseFrom(context.Payload);
			base.ApiLog.LogDebug(string.Concat("HandleGameAccountSelected : ", gameAccountSelectedRequest.ToString()));
		}

		private void HandleLoadModuleRequest(RPCContext context)
		{
			base.ApiLog.LogWarning("RPC Called: LoadModule");
			this.m_authenticationFailure = true;
		}

		private void HandleLogonCompleteRequest(RPCContext context)
		{
			LogonResult logonResult = LogonResult.ParseFrom(context.Payload);
			BattleNetErrors errorCode = (BattleNetErrors)logonResult.ErrorCode;
			if (errorCode != BattleNetErrors.ERROR_OK)
			{
				this.m_battleNet.EnqueueErrorInfo(BnetFeature.Auth, BnetFeatureEvent.Auth_OnFinish, errorCode, 0);
				return;
			}
			this.m_accountEntity = logonResult.Account;
			this.m_battleNet.Presence.PresenceSubscribe(this.m_accountEntity);
			this.m_gameAccounts = new List<bnet.protocol.EntityId>();
			foreach (bnet.protocol.EntityId gameAccountList in logonResult.GameAccountList)
			{
				this.m_gameAccounts.Add(gameAccountList);
				this.m_battleNet.Presence.PresenceSubscribe(gameAccountList);
			}
			if (logonResult.HasBattleTag)
			{
				this.m_battleTag = logonResult.BattleTag;
			}
			if (this.m_gameAccounts.Count > 0)
			{
				this.m_gameAccount = logonResult.GameAccountList[0];
			}
			this.m_sessionKey = logonResult.SessionKey;
			this.m_battleNet.IssueSelectGameAccountRequest();
			this.m_battleNet.SetConnectedRegion(logonResult.ConnectedRegion);
			base.ApiLog.LogDebug("LogonComplete {0}", new object[] { logonResult });
			base.ApiLog.LogDebug("Region (connected): {0}", new object[] { logonResult.ConnectedRegion });
		}

		private void HandleLogonQueueEnd(RPCContext context)
		{
			base.ApiLog.LogDebug("HandleLogonQueueEnd : ");
			this.SaveQueuePosition(0, (long)0, (long)0, true);
		}

		private void HandleLogonQueueUpdate(RPCContext context)
		{
			LogonQueueUpdateRequest logonQueueUpdateRequest = LogonQueueUpdateRequest.ParseFrom(context.Payload);
			base.ApiLog.LogDebug(string.Concat("HandleLogonQueueUpdate : ", logonQueueUpdateRequest.ToString()));
			long estimatedTime = (long)((logonQueueUpdateRequest.EstimatedTime - this.m_battleNet.ServerTimeUTCAtConnectMicroseconds) / (long)1000000);
			this.SaveQueuePosition((int)logonQueueUpdateRequest.Position, estimatedTime, (long)logonQueueUpdateRequest.EtaDeviationInSec, false);
		}

		private void HandleLogonUpdateRequest(RPCContext context)
		{
			base.ApiLog.LogDebug("RPC Called: LogonUpdate");
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public override void InitRPCListeners(RPCConnection rpcConnection)
		{
			base.InitRPCListeners(rpcConnection);
			this.m_rpcConnection.RegisterServiceMethodListener(this.m_authClientService.Id, 5, new RPCContextDelegate(this.HandleLogonCompleteRequest));
			this.m_rpcConnection.RegisterServiceMethodListener(this.m_authClientService.Id, 10, new RPCContextDelegate(this.HandleLogonUpdateRequest));
			this.m_rpcConnection.RegisterServiceMethodListener(this.m_authClientService.Id, 1, new RPCContextDelegate(this.HandleLoadModuleRequest));
			this.m_rpcConnection.RegisterServiceMethodListener(this.m_authClientService.Id, 12, new RPCContextDelegate(this.HandleLogonQueueUpdate));
			this.m_rpcConnection.RegisterServiceMethodListener(this.m_authClientService.Id, 13, new RPCContextDelegate(this.HandleLogonQueueEnd));
			this.m_rpcConnection.RegisterServiceMethodListener(this.m_authClientService.Id, 14, new RPCContextDelegate(this.HandleGameAccountSelected));
		}

		public override void OnDisconnected()
		{
			base.OnDisconnected();
		}

		public void SaveQueuePosition(int position, long end, long stdev, bool ended)
		{
			bool flag;
			flag = (ended || position != this.m_queueInfo.position || end != this.m_queueInfo.end ? true : stdev != this.m_queueInfo.stdev);
			this.m_queueInfo.changed = flag;
			this.m_queueInfo.position = position;
			this.m_queueInfo.end = end;
			this.m_queueInfo.stdev = stdev;
		}

		public void VerifyWebCredentials(string token)
		{
			if (this.m_rpcConnection == null)
			{
				return;
			}
			VerifyWebCredentialsRequest verifyWebCredentialsRequest = new VerifyWebCredentialsRequest();
			verifyWebCredentialsRequest.SetWebCredentials(Encoding.UTF8.GetBytes(token));
			this.m_rpcConnection.BeginAuth();
			this.m_rpcConnection.QueueRequest(this.AuthClientService.Id, 7, verifyWebCredentialsRequest, null, 0);
		}
	}
}