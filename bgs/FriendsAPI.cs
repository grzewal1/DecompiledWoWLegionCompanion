using bgs.RPCServices;
using bgs.types;
using bnet.protocol;
using bnet.protocol.friends;
using bnet.protocol.invitation;
using System;
using System.Collections.Generic;

namespace bgs
{
	public class FriendsAPI : BattleNetAPI
	{
		private ServiceDescriptor m_friendsService = new bgs.RPCServices.FriendsService();

		private ServiceDescriptor m_friendsNotifyService = new FriendsNotify();

		private FriendsAPI.FriendsAPIState m_state;

		private double m_subscribeStartTime;

		private float m_initializeTimeOut = 5f;

		private uint m_maxFriends;

		private uint m_maxReceivedInvitations;

		private uint m_maxSentInvitations;

		private uint m_friendsCount;

		private List<FriendsUpdate> m_updateList = new List<FriendsUpdate>();

		private Map<BnetEntityId, Map<ulong, bnet.protocol.EntityId>> m_friendEntityId = new Map<BnetEntityId, Map<ulong, bnet.protocol.EntityId>>();

		public ServiceDescriptor FriendsNotifyService
		{
			get
			{
				return this.m_friendsNotifyService;
			}
		}

		public ServiceDescriptor FriendsService
		{
			get
			{
				return this.m_friendsService;
			}
		}

		public float InitializeTimeOut
		{
			get
			{
				return this.m_initializeTimeOut;
			}
			set
			{
				this.m_initializeTimeOut = value;
			}
		}

		public bool IsInitialized
		{
			get
			{
				if (this.m_state == FriendsAPI.FriendsAPIState.INITIALIZED)
				{
					return true;
				}
				if (this.m_state == FriendsAPI.FriendsAPIState.FAILED_TO_INITIALIZE)
				{
					return true;
				}
				return false;
			}
		}

		public FriendsAPI(BattleNetCSharp battlenet) : base(battlenet, "Friends")
		{
		}

		private void AcceptInvitation(ulong inviteId)
		{
			GenericRequest genericRequest = new GenericRequest();
			genericRequest.SetInvitationId(inviteId);
			GenericRequest genericRequest1 = genericRequest;
			if (!genericRequest1.IsInitialized)
			{
				base.ApiLog.LogWarning("Battle.net Friends API C#: Failed to AcceptInvitation.");
				this.m_battleNet.EnqueueErrorInfo(BnetFeature.Friends, BnetFeatureEvent.Friends_OnAcceptInvitation, BattleNetErrors.ERROR_API_NOT_READY, 0);
				return;
			}
			this.m_rpcConnection.QueueRequest(this.m_friendsService.Id, 3, genericRequest1, new RPCContextDelegate(this.AcceptInvitationCallback), 0);
		}

		private void AcceptInvitationCallback(RPCContext context)
		{
			if (this.m_state != FriendsAPI.FriendsAPIState.INITIALIZED)
			{
				return;
			}
			BattleNetErrors status = (BattleNetErrors)context.Header.Status;
			if (status != BattleNetErrors.ERROR_OK)
			{
				base.ApiLog.LogWarning(string.Concat("Battle.net Friends API C#: Failed to AcceptInvitation. ", status));
				this.m_battleNet.EnqueueErrorInfo(BnetFeature.Friends, BnetFeatureEvent.Friends_OnAcceptInvitation, status, 0);
			}
		}

		private void AddFriendInternal(BnetEntityId entityId)
		{
			if (entityId == null)
			{
				return;
			}
			FriendsUpdate friendsUpdate = new FriendsUpdate()
			{
				action = 1,
				entity1 = entityId
			};
			this.m_updateList.Add(friendsUpdate);
			this.m_battleNet.Presence.PresenceSubscribe(BnetEntityId.CreateForProtocol(entityId));
			this.m_friendEntityId.Add(entityId, new Map<ulong, bnet.protocol.EntityId>());
			this.m_friendsCount = (uint)this.m_friendEntityId.Count;
		}

		public bool AddFriendsActiveGameAccount(BnetEntityId entityId, bnet.protocol.EntityId gameAccount, ulong index)
		{
			if (!this.IsFriend(entityId))
			{
				return false;
			}
			if (!this.m_friendEntityId[entityId].ContainsKey(index))
			{
				this.m_friendEntityId[entityId].Add(index, gameAccount);
				this.m_battleNet.Presence.PresenceSubscribe(gameAccount);
			}
			return true;
		}

		private void AddInvitationInternal(FriendsUpdate.Action action, Invitation invitation, int reason)
		{
			if (invitation == null)
			{
				return;
			}
			FriendsUpdate inviterName = new FriendsUpdate()
			{
				action = (int)action,
				long1 = invitation.Id,
				entity1 = this.GetBnetEntityIdFromIdentity(invitation.InviterIdentity)
			};
			if (invitation.HasInviterName)
			{
				inviterName.string1 = invitation.InviterName;
			}
			inviterName.entity2 = this.GetBnetEntityIdFromIdentity(invitation.InviteeIdentity);
			if (invitation.HasInviteeName)
			{
				inviterName.string2 = invitation.InviteeName;
			}
			if (invitation.HasInvitationMessage)
			{
				inviterName.string3 = invitation.InvitationMessage;
			}
			inviterName.bool1 = false;
			if (invitation.HasCreationTime)
			{
				inviterName.long2 = invitation.CreationTime;
			}
			if (invitation.HasExpirationTime)
			{
				inviterName.long3 = invitation.ExpirationTime;
			}
			this.m_updateList.Add(inviterName);
		}

		public void ClearFriendsUpdates()
		{
			this.m_updateList.Clear();
		}

		private void DeclineInvitation(ulong inviteId)
		{
			GenericRequest genericRequest = new GenericRequest();
			genericRequest.SetInvitationId(inviteId);
			GenericRequest genericRequest1 = genericRequest;
			if (!genericRequest1.IsInitialized)
			{
				base.ApiLog.LogWarning("Battle.net Friends API C#: Failed to DeclineInvitation.");
				this.m_battleNet.EnqueueErrorInfo(BnetFeature.Friends, BnetFeatureEvent.Friends_OnDeclineInvitation, BattleNetErrors.ERROR_API_NOT_READY, 0);
				return;
			}
			this.m_rpcConnection.QueueRequest(this.m_friendsService.Id, 5, genericRequest1, new RPCContextDelegate(this.DeclineInvitationCallback), 0);
		}

		private void DeclineInvitationCallback(RPCContext context)
		{
			if (this.m_state != FriendsAPI.FriendsAPIState.INITIALIZED)
			{
				return;
			}
			BattleNetErrors status = (BattleNetErrors)context.Header.Status;
			if (status != BattleNetErrors.ERROR_OK)
			{
				base.ApiLog.LogWarning(string.Concat("Battle.net Friends API C#: Failed to DeclineInvitation. ", status));
				this.m_battleNet.EnqueueErrorInfo(BnetFeature.Friends, BnetFeatureEvent.Friends_OnDeclineInvitation, status, 0);
			}
		}

		private BnetEntityId ExtractEntityIdFromFriendNotification(byte[] payload)
		{
			FriendNotification friendNotification = FriendNotification.ParseFrom(payload);
			if (!friendNotification.IsInitialized)
			{
				return null;
			}
			return BnetEntityId.CreateFromProtocol(friendNotification.Target.Id);
		}

		private Invitation ExtractInvitationFromInvitationNotification(byte[] payload)
		{
			InvitationNotification invitationNotification = InvitationNotification.ParseFrom(payload);
			if (!invitationNotification.IsInitialized)
			{
				return null;
			}
			return invitationNotification.Invitation;
		}

		private BnetEntityId GetBnetEntityIdFromIdentity(Identity identity)
		{
			BnetEntityId bnetEntityId = new BnetEntityId();
			if (identity.HasAccountId)
			{
				bnetEntityId.SetLo(identity.AccountId.Low);
				bnetEntityId.SetHi(identity.AccountId.High);
			}
			else if (!identity.HasGameAccountId)
			{
				bnetEntityId.SetLo((ulong)0);
				bnetEntityId.SetHi((ulong)0);
			}
			else
			{
				bnetEntityId.SetLo(identity.GameAccountId.Low);
				bnetEntityId.SetHi(identity.GameAccountId.High);
			}
			return bnetEntityId;
		}

		public bool GetFriendsActiveGameAccounts(BnetEntityId entityId, [Out] Map<ulong, bnet.protocol.EntityId> gameAccounts)
		{
			if (this.m_friendEntityId.TryGetValue(entityId, out gameAccounts))
			{
				return true;
			}
			return false;
		}

		public void GetFriendsInfo(ref FriendsInfo info)
		{
			if (this.m_state != FriendsAPI.FriendsAPIState.INITIALIZED)
			{
				return;
			}
			info.maxFriends = (int)this.m_maxFriends;
			info.maxRecvInvites = (int)this.m_maxReceivedInvitations;
			info.maxSentInvites = (int)this.m_maxSentInvitations;
			info.friendsSize = (int)this.m_friendsCount;
			info.updateSize = this.m_updateList.Count;
		}

		public void GetFriendsUpdates([Out] FriendsUpdate[] updates)
		{
			this.m_updateList.CopyTo(updates, 0);
		}

		public override void Initialize()
		{
			base.Initialize();
			if (this.m_state == FriendsAPI.FriendsAPIState.INITIALIZING)
			{
				return;
			}
			this.StartInitialize();
			this.Subscribe();
		}

		public override void InitRPCListeners(RPCConnection rpcConnection)
		{
			base.InitRPCListeners(rpcConnection);
			rpcConnection.RegisterServiceMethodListener(this.m_friendsNotifyService.Id, 1, new RPCContextDelegate(this.NotifyFriendAddedListenerCallback));
			rpcConnection.RegisterServiceMethodListener(this.m_friendsNotifyService.Id, 2, new RPCContextDelegate(this.NotifyFriendRemovedListenerCallback));
			rpcConnection.RegisterServiceMethodListener(this.m_friendsNotifyService.Id, 3, new RPCContextDelegate(this.NotifyReceivedInvitationAddedCallback));
			rpcConnection.RegisterServiceMethodListener(this.m_friendsNotifyService.Id, 4, new RPCContextDelegate(this.NotifyReceivedInvitationRemovedCallback));
		}

		public bool IsFriend(BnetEntityId entityId)
		{
			return this.m_friendEntityId.ContainsKey(entityId);
		}

		public void ManageFriendInvite(int action, ulong inviteId)
		{
			if (this.m_state != FriendsAPI.FriendsAPIState.INITIALIZED)
			{
				return;
			}
			FriendsAPI.InviteAction inviteAction = (FriendsAPI.InviteAction)action;
			if (inviteAction == FriendsAPI.InviteAction.INVITE_ACCEPT)
			{
				this.AcceptInvitation(inviteId);
			}
			else if (inviteAction == FriendsAPI.InviteAction.INVITE_DECLINE)
			{
				this.DeclineInvitation(inviteId);
			}
		}

		private void NotifyFriendAddedListenerCallback(RPCContext context)
		{
			if (this.m_state != FriendsAPI.FriendsAPIState.INITIALIZED)
			{
				return;
			}
			this.AddFriendInternal(this.ExtractEntityIdFromFriendNotification(context.Payload));
		}

		private void NotifyFriendRemovedListenerCallback(RPCContext context)
		{
			if (this.m_state != FriendsAPI.FriendsAPIState.INITIALIZED)
			{
				return;
			}
			this.RemoveFriendInternal(this.ExtractEntityIdFromFriendNotification(context.Payload));
		}

		private void NotifyReceivedInvitationAddedCallback(RPCContext context)
		{
			if (this.m_state != FriendsAPI.FriendsAPIState.INITIALIZED)
			{
				return;
			}
			Invitation invitation = this.ExtractInvitationFromInvitationNotification(context.Payload);
			this.AddInvitationInternal(FriendsUpdate.Action.FRIEND_INVITE, invitation, 0);
		}

		private void NotifyReceivedInvitationRemovedCallback(RPCContext context)
		{
			if (this.m_state != FriendsAPI.FriendsAPIState.INITIALIZED)
			{
				return;
			}
			Invitation invitation = this.ExtractInvitationFromInvitationNotification(context.Payload);
			this.AddInvitationInternal(FriendsUpdate.Action.FRIEND_INVITE_REMOVED, invitation, 0);
		}

		public override void OnDisconnected()
		{
			base.OnDisconnected();
		}

		public override void Process()
		{
			base.Process();
			if (this.m_state == FriendsAPI.FriendsAPIState.INITIALIZING && BattleNet.GetRealTimeSinceStartup() - this.m_subscribeStartTime >= (double)this.InitializeTimeOut)
			{
				this.m_state = FriendsAPI.FriendsAPIState.FAILED_TO_INITIALIZE;
				base.ApiLog.LogWarning("Battle.net Friends API C#: Initialize timed out.");
			}
		}

		private void ProcessSubscribeToFriendsResponse(SubscribeToFriendsResponse response)
		{
			if (response.HasMaxFriends)
			{
				this.m_maxFriends = response.MaxFriends;
			}
			if (response.HasMaxReceivedInvitations)
			{
				this.m_maxReceivedInvitations = response.MaxReceivedInvitations;
			}
			if (response.HasMaxSentInvitations)
			{
				this.m_maxSentInvitations = response.MaxSentInvitations;
			}
			for (int i = 0; i < response.FriendsCount; i++)
			{
				Friend item = response.Friends[i];
				BnetEntityId bnetEntityId = new BnetEntityId();
				bnetEntityId.SetLo(item.Id.Low);
				bnetEntityId.SetHi(item.Id.High);
				this.AddFriendInternal(bnetEntityId);
			}
			for (int j = 0; j < response.ReceivedInvitationsCount; j++)
			{
				this.AddInvitationInternal(FriendsUpdate.Action.FRIEND_INVITE, response.ReceivedInvitations[j], 0);
			}
			for (int k = 0; k < response.SentInvitationsCount; k++)
			{
				this.AddInvitationInternal(FriendsUpdate.Action.FRIEND_SENT_INVITE, response.SentInvitations[k], 0);
			}
		}

		public void RemoveFriend(BnetAccountId account)
		{
			if (this.m_state != FriendsAPI.FriendsAPIState.INITIALIZED)
			{
				return;
			}
			bnet.protocol.EntityId entityId = new bnet.protocol.EntityId();
			entityId.SetLow(account.GetLo());
			entityId.SetHigh(account.GetHi());
			GenericFriendRequest genericFriendRequest = new GenericFriendRequest();
			genericFriendRequest.SetTargetId(entityId);
			GenericFriendRequest genericFriendRequest1 = genericFriendRequest;
			if (!genericFriendRequest1.IsInitialized)
			{
				base.ApiLog.LogWarning("Battle.net Friends API C#: Failed to RemoveFriend.");
				this.m_battleNet.EnqueueErrorInfo(BnetFeature.Friends, BnetFeatureEvent.Friends_OnRemoveFriend, BattleNetErrors.ERROR_API_NOT_READY, 0);
				return;
			}
			this.m_rpcConnection.QueueRequest(this.m_friendsService.Id, 8, genericFriendRequest1, new RPCContextDelegate(this.RemoveFriendCallback), 0);
		}

		private void RemoveFriendCallback(RPCContext context)
		{
			if (this.m_state != FriendsAPI.FriendsAPIState.INITIALIZED)
			{
				return;
			}
			BattleNetErrors status = (BattleNetErrors)context.Header.Status;
			if (status != BattleNetErrors.ERROR_OK)
			{
				base.ApiLog.LogWarning(string.Concat("Battle.net Friends API C#: Failed to RemoveFriend. ", status));
				this.m_battleNet.EnqueueErrorInfo(BnetFeature.Friends, BnetFeatureEvent.Friends_OnRemoveFriend, status, 0);
			}
		}

		private void RemoveFriendInternal(BnetEntityId entityId)
		{
			if (entityId == null)
			{
				return;
			}
			FriendsUpdate friendsUpdate = new FriendsUpdate()
			{
				action = 2,
				entity1 = entityId
			};
			this.m_updateList.Add(friendsUpdate);
			this.m_battleNet.Presence.PresenceUnsubscribe(BnetEntityId.CreateForProtocol(entityId));
			if (this.m_friendEntityId.ContainsKey(entityId))
			{
				foreach (bnet.protocol.EntityId value in this.m_friendEntityId[entityId].Values)
				{
					this.m_battleNet.Presence.PresenceUnsubscribe(value);
				}
				this.m_friendEntityId.Remove(entityId);
			}
			this.m_friendsCount = (uint)this.m_friendEntityId.Count;
		}

		public void RemoveFriendsActiveGameAccount(BnetEntityId entityId, ulong index)
		{
			bnet.protocol.EntityId entityId1;
			if (this.IsFriend(entityId) && this.m_friendEntityId[entityId].TryGetValue(index, out entityId1))
			{
				this.m_battleNet.Presence.PresenceUnsubscribe(entityId1);
				this.m_friendEntityId[entityId].Remove(index);
			}
		}

		public void SendFriendInvite(string sender, string target, bool byEmail)
		{
			if (this.m_state != FriendsAPI.FriendsAPIState.INITIALIZED)
			{
				return;
			}
			SendInvitationRequest sendInvitationRequest = new SendInvitationRequest();
			bnet.protocol.EntityId entityId = new bnet.protocol.EntityId();
			entityId.SetLow((ulong)0);
			entityId.SetHigh((ulong)0);
			sendInvitationRequest.SetTargetId(entityId);
			InvitationParams invitationParam = new InvitationParams();
			FriendInvitationParams friendInvitationParam = new FriendInvitationParams();
			if (!byEmail)
			{
				friendInvitationParam.SetTargetBattleTag(target);
				friendInvitationParam.AddRole(1);
			}
			else
			{
				friendInvitationParam.SetTargetEmail(target);
				friendInvitationParam.AddRole(2);
			}
			invitationParam.SetFriendParams(friendInvitationParam);
			sendInvitationRequest.SetParams(invitationParam);
			SendInvitationRequest sendInvitationRequest1 = sendInvitationRequest;
			if (!sendInvitationRequest1.IsInitialized)
			{
				base.ApiLog.LogWarning("Battle.net Friends API C#: Failed to SendFriendInvite.");
				return;
			}
			this.m_rpcConnection.QueueRequest(this.m_friendsService.Id, 2, sendInvitationRequest1, new RPCContextDelegate(this.SendInvitationCallback), 0);
		}

		private void SendInvitationCallback(RPCContext context)
		{
			if (this.m_state != FriendsAPI.FriendsAPIState.INITIALIZED)
			{
				return;
			}
			BattleNetErrors status = (BattleNetErrors)context.Header.Status;
			if (status != BattleNetErrors.ERROR_OK)
			{
				base.ApiLog.LogWarning(string.Concat("Battle.net Friends API C#: Failed to SendInvitation. ", status));
			}
			this.m_battleNet.EnqueueErrorInfo(BnetFeature.Friends, BnetFeatureEvent.Friends_OnSendInvitation, status, 0);
		}

		private void StartInitialize()
		{
			this.m_subscribeStartTime = BattleNet.GetRealTimeSinceStartup();
			this.m_state = FriendsAPI.FriendsAPIState.INITIALIZING;
			this.m_maxFriends = 0;
			this.m_maxReceivedInvitations = 0;
			this.m_maxSentInvitations = 0;
			this.m_friendsCount = 0;
			this.m_updateList = new List<FriendsUpdate>();
			this.m_friendEntityId = new Map<BnetEntityId, Map<ulong, bnet.protocol.EntityId>>();
		}

		private void Subscribe()
		{
			SubscribeToFriendsRequest subscribeToFriendsRequest = new SubscribeToFriendsRequest();
			subscribeToFriendsRequest.SetObjectId((ulong)0);
			SubscribeToFriendsRequest subscribeToFriendsRequest1 = subscribeToFriendsRequest;
			if (!subscribeToFriendsRequest1.IsInitialized)
			{
				base.ApiLog.LogWarning("Battle.net Friends API C#: Failed to Subscribe.");
				return;
			}
			this.m_rpcConnection.QueueRequest(this.m_friendsService.Id, 1, subscribeToFriendsRequest1, new RPCContextDelegate(this.SubscribeToFriendsCallback), 0);
		}

		private void SubscribeToFriendsCallback(RPCContext context)
		{
			if (this.m_state != FriendsAPI.FriendsAPIState.INITIALIZING)
			{
				return;
			}
			if (context.Header.Status != 0)
			{
				this.m_state = FriendsAPI.FriendsAPIState.FAILED_TO_INITIALIZE;
				base.ApiLog.LogWarning("Battle.net Friends API C#: Failed to initialize.");
			}
			else
			{
				this.m_state = FriendsAPI.FriendsAPIState.INITIALIZED;
				base.ApiLog.LogDebug("Battle.net Friends API C#: Initialized.");
				this.ProcessSubscribeToFriendsResponse(SubscribeToFriendsResponse.ParseFrom(context.Payload));
			}
		}

		private enum FriendsAPIState
		{
			NOT_SET,
			INITIALIZING,
			INITIALIZED,
			FAILED_TO_INITIALIZE
		}

		public enum InviteAction
		{
			INVITE_ACCEPT = 1,
			INVITE_REVOKE = 2,
			INVITE_DECLINE = 3,
			INVITE_IGNORE = 4
		}
	}
}