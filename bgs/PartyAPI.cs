using bgs.types;
using bnet.protocol;
using bnet.protocol.attribute;
using bnet.protocol.channel;
using bnet.protocol.channel_invitation;
using bnet.protocol.invitation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace bgs
{
	public class PartyAPI : BattleNetAPI
	{
		public static string PARTY_TYPE_DEFAULT;

		private Map<bnet.protocol.EntityId, PartyAPI.PartyData> m_activeParties = new Map<bnet.protocol.EntityId, PartyAPI.PartyData>();

		private List<PartyEvent> m_partyEvents = new List<PartyEvent>();

		private List<PartyListenerEvent> m_partyListenerEvents = new List<PartyListenerEvent>();

		static PartyAPI()
		{
			PartyAPI.PARTY_TYPE_DEFAULT = "default";
		}

		public PartyAPI(BattleNetCSharp battlenet) : base(battlenet, "Party")
		{
		}

		public void AcceptFriendlyChallenge(bnet.protocol.EntityId partyId)
		{
			bnet.protocol.EntityId entityId = new bnet.protocol.EntityId();
			PartyAPI.PartyData partyData = this.GetPartyData(partyId);
			if (partyData != null)
			{
				entityId = partyData.m_friendGameAccount;
			}
			this.PushPartyEvent(partyId, "dll", "ok", entityId);
			this.FriendlyChallenge_PushStateChange(partyId, "deck", false);
		}

		public void AcceptPartyInvite(ulong invitationId)
		{
			ChannelAPI.ReceivedInvite receivedInvite = this.m_battleNet.Channel.GetReceivedInvite(ChannelAPI.InvitationServiceType.INVITATION_SERVICE_TYPE_PARTY, invitationId);
			if (receivedInvite == null)
			{
				return;
			}
			string receivedInvitationPartyType = this.GetReceivedInvitationPartyType(invitationId);
			this.m_battleNet.Channel.AcceptInvitation(invitationId, receivedInvite.ChannelId, ChannelAPI.ChannelType.PARTY_CHANNEL, (RPCContext ctx) => this.GenericPartyRequestCallback(ctx, string.Concat("AcceptPartyInvite inviteId=", invitationId), BnetFeatureEvent.Party_AcceptInvite_Callback, null, receivedInvitationPartyType));
		}

		public void AddActiveChannel(ulong objectId, ChannelAPI.ChannelReferenceObject channelRefObject, PartyAPI.PartyData partyData)
		{
			bnet.protocol.EntityId mChannelId = channelRefObject.m_channelData.m_channelId;
			this.m_battleNet.Channel.AddActiveChannel(objectId, channelRefObject);
			this.m_activeParties.Add(mChannelId, partyData);
		}

		public void AssignPartyRole(bnet.protocol.EntityId partyId, bnet.protocol.EntityId memberId, uint roleId)
		{
			string partyType = this.GetPartyType(partyId);
			if (this.GetPartyData(partyId) == null)
			{
				this.GenericPartyRequestCallback_Internal(BattleNetErrors.ERROR_INVALID_ARGS, "AssignPartyRole no PartyData", BnetFeatureEvent.Party_AssignRole_Callback, partyId, partyType);
				return;
			}
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.m_battleNet.Channel.GetChannelReferenceObject(partyId);
			if (channelReferenceObject == null)
			{
				this.GenericPartyRequestCallback_Internal(BattleNetErrors.ERROR_INVALID_ARGS, "AssignPartyRole no channelRefObject", BnetFeatureEvent.Party_AssignRole_Callback, partyId, partyType);
				return;
			}
			UpdateMemberStateRequest updateMemberStateRequest = new UpdateMemberStateRequest();
			Member member = new Member();
			Identity identity = new Identity();
			MemberState memberState = new MemberState();
			memberState.AddRole(roleId);
			identity.SetGameAccountId(memberId);
			member.SetIdentity(identity);
			member.SetState(memberState);
			updateMemberStateRequest.AddStateChange(member);
			this.m_rpcConnection.QueueRequest(this.m_battleNet.Channel.ChannelService.Id, 5, updateMemberStateRequest, (RPCContext ctx) => this.GenericPartyRequestCallback(ctx, string.Concat(new object[] { "AssignPartyRole memberId=", memberId.Low, " roleId=", roleId }), BnetFeatureEvent.Party_AssignRole_Callback, partyId, partyType), (uint)channelReferenceObject.m_channelData.m_objectId);
		}

		public void ClearPartyAttribute(bnet.protocol.EntityId partyId, string attributeKey)
		{
			bnet.protocol.attribute.Attribute attribute = new bnet.protocol.attribute.Attribute();
			bnet.protocol.attribute.Variant variant = new bnet.protocol.attribute.Variant();
			attribute.SetName(attributeKey);
			attribute.SetValue(variant);
			this.SetPartyAttribute_Internal(string.Concat("ClearPartyAttribute key=", attributeKey), BnetFeatureEvent.Party_ClearAttribute_Callback, partyId, attribute);
		}

		public void ClearPartyListenerEvents()
		{
			this.m_partyListenerEvents.Clear();
		}

		public void ClearPartyUpdates()
		{
			this.m_partyEvents.Clear();
		}

		public void CreateParty(string szPartyType, int privacyLevel, byte[] creatorBlob)
		{
			PartyAPI.PartyData partyDatum = new PartyAPI.PartyData(this.m_battleNet);
			CreateChannelRequest createChannelRequest = new CreateChannelRequest();
			createChannelRequest.SetObjectId(ChannelAPI.GetNextObjectId());
			partyDatum.SetSubscriberObjectId(createChannelRequest.ObjectId);
			ChannelState channelState = new ChannelState();
			channelState.SetChannelType(szPartyType);
			channelState.SetPrivacyLevel((ChannelState.Types.PrivacyLevel)privacyLevel);
			channelState.AddAttribute(ProtocolHelper.CreateAttribute("WTCG.Party.Type", szPartyType));
			channelState.AddAttribute(ProtocolHelper.CreateAttribute("WTCG.Party.Creator", creatorBlob));
			createChannelRequest.SetChannelState(channelState);
			this.m_rpcConnection.QueueRequest(this.m_battleNet.Channel.ChannelOwnerService.Id, 2, createChannelRequest, (RPCContext ctx) => partyDatum.CreateChannelCallback(ctx, szPartyType), 2);
		}

		public void DeclineFriendlyChallenge(bnet.protocol.EntityId partyId, string action)
		{
			PartyAPI.PartyData partyData = this.GetPartyData(partyId);
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.m_battleNet.Channel.GetChannelReferenceObject(partyId);
			if (partyData == null || channelReferenceObject == null)
			{
				this.PushPartyEvent(partyId, "parm", "party", new bnet.protocol.EntityId());
				return;
			}
			this.PushPartyEvent(partyId, "dll", action, partyData.m_friendGameAccount);
			this.m_rpcConnection.QueueRequest(this.m_battleNet.Channel.ChannelService.Id, 6, new DissolveRequest(), new RPCContextDelegate(partyData.DeclineInvite_DissolvePartyInviteCallback), (uint)channelReferenceObject.m_channelData.m_objectId);
		}

		public void DeclinePartyInvite(ulong invitationId)
		{
			ChannelAPI.ReceivedInvite receivedInvite = this.m_battleNet.Channel.GetReceivedInvite(ChannelAPI.InvitationServiceType.INVITATION_SERVICE_TYPE_PARTY, invitationId);
			if (receivedInvite == null)
			{
				return;
			}
			string receivedInvitationPartyType = this.GetReceivedInvitationPartyType(invitationId);
			this.m_battleNet.Channel.DeclineInvitation(invitationId, receivedInvite.ChannelId, (RPCContext ctx) => this.GenericPartyRequestCallback(ctx, string.Concat("DeclinePartyInvite inviteId=", invitationId), BnetFeatureEvent.Party_DeclineInvite_Callback, null, receivedInvitationPartyType));
		}

		public void DissolveParty(bnet.protocol.EntityId partyId)
		{
			string partyType = this.GetPartyType(partyId);
			if (this.GetPartyData(partyId) == null)
			{
				this.GenericPartyRequestCallback_Internal(BattleNetErrors.ERROR_INVALID_ARGS, "DissolveParty no PartyData", BnetFeatureEvent.Party_Dissolve_Callback, partyId, partyType);
				return;
			}
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.m_battleNet.Channel.GetChannelReferenceObject(partyId);
			if (channelReferenceObject == null)
			{
				this.GenericPartyRequestCallback_Internal(BattleNetErrors.ERROR_INVALID_ARGS, "DissolveParty no channelRefObject", BnetFeatureEvent.Party_Dissolve_Callback, partyId, partyType);
				return;
			}
			this.m_rpcConnection.QueueRequest(this.m_battleNet.Channel.ChannelService.Id, 6, new DissolveRequest(), (RPCContext ctx) => this.GenericPartyRequestCallback(ctx, "DissolveParty", BnetFeatureEvent.Party_Dissolve_Callback, partyId, partyType), (uint)channelReferenceObject.m_channelData.m_objectId);
		}

		private void FriendlyChallenge_PushStateChange(bnet.protocol.EntityId partyId, string state, bool onlyIfMaker = false)
		{
			PartyAPI.PartyData partyData = this.GetPartyData(partyId);
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.m_battleNet.Channel.GetChannelReferenceObject(partyId);
			if (partyData == null || channelReferenceObject == null)
			{
				return;
			}
			if (onlyIfMaker && !partyData.m_maker)
			{
				return;
			}
			List<bnet.protocol.attribute.Attribute> attributes = new List<bnet.protocol.attribute.Attribute>()
			{
				ProtocolHelper.CreateAttribute((!partyData.m_maker ? "s2" : "s1"), state)
			};
			this.m_battleNet.Channel.UpdateChannelAttributes((ChannelAPI.ChannelData)channelReferenceObject.m_channelData, attributes, null);
		}

		private void GenericPartyRequestCallback(RPCContext context, string message, BnetFeatureEvent featureEvent, bnet.protocol.EntityId partyId, string szPartyType)
		{
			BattleNetErrors status;
			if (context == null || context.Header == null)
			{
				status = BattleNetErrors.ERROR_RPC_MALFORMED_RESPONSE;
			}
			else
			{
				status = (BattleNetErrors)context.Header.Status;
			}
			this.GenericPartyRequestCallback_Internal(status, message, featureEvent, partyId, szPartyType);
		}

		private void GenericPartyRequestCallback_Internal(BattleNetErrors error, string message, BnetFeatureEvent featureEvent, bnet.protocol.EntityId partyId, string szPartyType)
		{
			this.m_battleNet.Party.PushPartyErrorEvent(PartyListenerEventType.OPERATION_CALLBACK, message, error, featureEvent, partyId, szPartyType);
			if (error == BattleNetErrors.ERROR_OK)
			{
				if (partyId == null)
				{
					message = string.Format("PartyRequest {0} status={1} type={2}", message, error.ToString(), szPartyType);
				}
				else
				{
					message = string.Format("PartyRequest {0} status={1} partyId={2} type={3}", new object[] { message, error.ToString(), partyId.Low, szPartyType });
				}
				this.m_battleNet.Party.ApiLog.LogDebug(message);
				return;
			}
			if (partyId == null)
			{
				message = string.Format("PartyRequestError: {0} {1} {2} type={3}", new object[] { (int)error, error.ToString(), message, szPartyType });
			}
			else
			{
				message = string.Format("PartyRequestError: {0} {1} {2} partyId={3} type={4}", new object[] { (int)error, error.ToString(), message, partyId.Low, szPartyType });
			}
			this.m_battleNet.Party.ApiLog.LogError(message);
		}

		public void GetAllPartyAttributes(bnet.protocol.EntityId partyId, out string[] allKeys)
		{
			ChannelState channelState;
			ChannelAPI.ReceivedInvite receivedInvite = this.m_battleNet.Channel.GetAllReceivedInvites().FirstOrDefault<ChannelAPI.ReceivedInvite>((ChannelAPI.ReceivedInvite i) => (i.ChannelId == null || i.ChannelId.High != partyId.High ? false : i.ChannelId.Low == partyId.Low));
			channelState = (receivedInvite == null || receivedInvite.State == null ? this.GetPartyState(partyId) : receivedInvite.State);
			if (channelState != null)
			{
				allKeys = new string[channelState.AttributeList.Count];
				for (int num = 0; num < channelState.AttributeList.Count; num++)
				{
					bnet.protocol.attribute.Attribute item = channelState.AttributeList[num];
					allKeys[num] = item.Name;
				}
			}
			else
			{
				allKeys = new string[0];
			}
		}

		public int GetCountPartyMembers(bnet.protocol.EntityId partyId)
		{
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.m_battleNet.Channel.GetChannelReferenceObject(partyId);
			if (channelReferenceObject != null)
			{
				ChannelAPI.ChannelData mChannelData = channelReferenceObject.m_channelData as ChannelAPI.ChannelData;
				if (mChannelData != null && mChannelData.m_members != null)
				{
					return mChannelData.m_members.Count;
				}
			}
			return 0;
		}

		public int GetMaxPartyMembers(bnet.protocol.EntityId partyId)
		{
			ChannelState partyState = this.GetPartyState(partyId);
			if (partyState == null || !partyState.HasMaxMembers)
			{
				return 0;
			}
			return (int)partyState.MaxMembers;
		}

		public void GetPartyAttributeBlob(bnet.protocol.EntityId partyId, string attributeKey, out byte[] value)
		{
			value = null;
			bnet.protocol.attribute.Attribute receivedInvitationAttribute = this.GetReceivedInvitationAttribute(partyId, attributeKey);
			if (receivedInvitationAttribute != null && receivedInvitationAttribute.Value.HasBlobValue)
			{
				value = receivedInvitationAttribute.Value.BlobValue;
				return;
			}
			ChannelState partyState = this.GetPartyState(partyId);
			if (partyState != null)
			{
				int num = 0;
				while (num < partyState.AttributeList.Count)
				{
					receivedInvitationAttribute = partyState.AttributeList[num];
					if (!(receivedInvitationAttribute.Name == attributeKey) || !receivedInvitationAttribute.Value.HasBlobValue)
					{
						num++;
					}
					else
					{
						value = receivedInvitationAttribute.Value.BlobValue;
						break;
					}
				}
			}
		}

		public bool GetPartyAttributeLong(bnet.protocol.EntityId partyId, string attributeKey, out long value)
		{
			value = (long)0;
			bnet.protocol.attribute.Attribute receivedInvitationAttribute = this.GetReceivedInvitationAttribute(partyId, attributeKey);
			if (receivedInvitationAttribute != null && receivedInvitationAttribute.Value.HasIntValue)
			{
				value = receivedInvitationAttribute.Value.IntValue;
				return true;
			}
			ChannelState partyState = this.GetPartyState(partyId);
			if (partyState != null)
			{
				for (int i = 0; i < partyState.AttributeList.Count; i++)
				{
					receivedInvitationAttribute = partyState.AttributeList[i];
					if (receivedInvitationAttribute.Name == attributeKey && receivedInvitationAttribute.Value.HasIntValue)
					{
						value = receivedInvitationAttribute.Value.IntValue;
						return true;
					}
				}
			}
			return false;
		}

		public void GetPartyAttributeString(bnet.protocol.EntityId partyId, string attributeKey, out string value)
		{
			value = null;
			bnet.protocol.attribute.Attribute receivedInvitationAttribute = this.GetReceivedInvitationAttribute(partyId, attributeKey);
			if (receivedInvitationAttribute != null && receivedInvitationAttribute.Value.HasStringValue)
			{
				value = receivedInvitationAttribute.Value.StringValue;
				return;
			}
			ChannelState partyState = this.GetPartyState(partyId);
			if (partyState != null)
			{
				int num = 0;
				while (num < partyState.AttributeList.Count)
				{
					receivedInvitationAttribute = partyState.AttributeList[num];
					if (!(receivedInvitationAttribute.Name == attributeKey) || !receivedInvitationAttribute.Value.HasStringValue)
					{
						num++;
					}
					else
					{
						value = receivedInvitationAttribute.Value.StringValue;
						break;
					}
				}
			}
		}

		private PartyAPI.PartyData GetPartyData(bnet.protocol.EntityId partyId)
		{
			PartyAPI.PartyData partyDatum;
			if (this.m_activeParties.TryGetValue(partyId, out partyDatum))
			{
				return partyDatum;
			}
			return null;
		}

		public void GetPartyInviteRequests(bnet.protocol.EntityId partyId, out InviteRequest[] requests)
		{
			Suggestion[] inviteRequests = this.m_battleNet.Channel.GetInviteRequests(partyId);
			requests = new InviteRequest[(int)inviteRequests.Length];
			for (int i = 0; i < (int)requests.Length; i++)
			{
				Suggestion suggestion = inviteRequests[i];
				InviteRequest inviteRequest = new InviteRequest()
				{
					TargetName = suggestion.SuggesteeName,
					TargetId = BnetGameAccountId.CreateFromProtocol(suggestion.SuggesteeId),
					RequesterName = suggestion.SuggesterName,
					RequesterId = BnetGameAccountId.CreateFromProtocol(suggestion.SuggesterId)
				};
				requests[i] = inviteRequest;
			}
		}

		public void GetPartyListenerEvents(out PartyListenerEvent[] updates)
		{
			updates = new PartyListenerEvent[this.m_partyListenerEvents.Count];
			this.m_partyListenerEvents.CopyTo(updates);
		}

		public void GetPartyMembers(bnet.protocol.EntityId partyId, out bgs.types.PartyMember[] members)
		{
			members = null;
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.m_battleNet.Channel.GetChannelReferenceObject(partyId);
			if (channelReferenceObject != null)
			{
				ChannelAPI.ChannelData mChannelData = channelReferenceObject.m_channelData as ChannelAPI.ChannelData;
				if (mChannelData != null && mChannelData.m_members != null)
				{
					members = new bgs.types.PartyMember[mChannelData.m_members.Count];
					int num = 0;
					foreach (KeyValuePair<bnet.protocol.EntityId, Member> mMember in mChannelData.m_members)
					{
						bnet.protocol.EntityId key = mMember.Key;
						Member value = mMember.Value;
						bgs.types.PartyMember item = new bgs.types.PartyMember()
						{
							memberGameAccountId = new bgs.types.EntityId(key)
						};
						if (value.State.RoleCount > 0)
						{
							item.firstMemberRole = value.State.RoleList[0];
						}
						members[num] = item;
						num++;
					}
				}
			}
			if (members == null)
			{
				members = new bgs.types.PartyMember[0];
			}
		}

		public int GetPartyPrivacy(bnet.protocol.EntityId partyId)
		{
			int privacyLevel = 4;
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.m_battleNet.Channel.GetChannelReferenceObject(partyId);
			if (channelReferenceObject == null)
			{
				return privacyLevel;
			}
			ChannelAPI.ChannelData mChannelData = channelReferenceObject.m_channelData as ChannelAPI.ChannelData;
			if (mChannelData != null && mChannelData.m_channelState != null && mChannelData.m_channelState.HasPrivacyLevel)
			{
				privacyLevel = (int)mChannelData.m_channelState.PrivacyLevel;
			}
			return privacyLevel;
		}

		public void GetPartySentInvites(bnet.protocol.EntityId partyId, out PartyInvite[] invites)
		{
			invites = null;
			ChannelState partyState = this.GetPartyState(partyId);
			if (partyState != null)
			{
				invites = new PartyInvite[partyState.InvitationCount];
				PartyType partyTypeFromString = BnetParty.GetPartyTypeFromString(this.GetPartyType(partyId));
				for (int i = 0; i < (int)invites.Length; i++)
				{
					Invitation item = partyState.InvitationList[i];
					PartyInvite partyInvite = new PartyInvite()
					{
						InviteId = item.Id,
						PartyId = PartyId.FromProtocol(partyId),
						PartyType = partyTypeFromString,
						InviterName = item.InviterName,
						InviterId = BnetGameAccountId.CreateFromProtocol(item.InviterIdentity.GameAccountId),
						InviteeId = BnetGameAccountId.CreateFromProtocol(item.InviteeIdentity.GameAccountId)
					};
					invites[i] = partyInvite;
				}
			}
			if (invites == null)
			{
				invites = new PartyInvite[0];
			}
		}

		private ChannelState GetPartyState(bnet.protocol.EntityId partyId)
		{
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.m_battleNet.Channel.GetChannelReferenceObject(partyId);
			if (channelReferenceObject == null)
			{
				return null;
			}
			return (channelReferenceObject.m_channelData as ChannelAPI.ChannelData).m_channelState;
		}

		public string GetPartyType(bnet.protocol.EntityId partyId)
		{
			string str;
			string empty = string.Empty;
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.m_battleNet.Channel.GetChannelReferenceObject(partyId);
			if (channelReferenceObject == null || !(channelReferenceObject.m_channelData is ChannelAPI.ChannelData))
			{
				return empty;
			}
			ChannelState mChannelState = ((ChannelAPI.ChannelData)channelReferenceObject.m_channelData).m_channelState;
			if (mChannelState == null)
			{
				return empty;
			}
			empty = mChannelState.ChannelType;
			if (empty == PartyAPI.PARTY_TYPE_DEFAULT)
			{
				this.GetPartyAttributeString(partyId, "WTCG.Party.Type", out str);
				if (str != null)
				{
					empty = str;
				}
			}
			return empty;
		}

		public int GetPartyUpdateCount()
		{
			return this.m_partyEvents.Count;
		}

		public void GetPartyUpdates([Out] PartyEvent[] updates)
		{
			this.m_partyEvents.CopyTo(updates);
		}

		public bnet.protocol.attribute.Attribute GetReceivedInvitationAttribute(bnet.protocol.EntityId partyId, string attributeKey)
		{
			ChannelAPI.ReceivedInvite[] allReceivedInvites = this.m_battleNet.Channel.GetAllReceivedInvites();
			ChannelAPI.ReceivedInvite receivedInvite = allReceivedInvites.FirstOrDefault<ChannelAPI.ReceivedInvite>((ChannelAPI.ReceivedInvite i) => (i.ChannelId == null || i.ChannelId.High != partyId.High ? false : i.ChannelId.Low == partyId.Low));
			if (receivedInvite != null && receivedInvite.State != null)
			{
				ChannelState state = receivedInvite.State;
				for (int num = 0; num < state.AttributeList.Count; num++)
				{
					bnet.protocol.attribute.Attribute item = state.AttributeList[num];
					if (item.Name == attributeKey)
					{
						return item;
					}
				}
			}
			return null;
		}

		public string GetReceivedInvitationPartyType(ulong invitationId)
		{
			string empty = string.Empty;
			ChannelAPI.ReceivedInvite receivedInvite = this.m_battleNet.Channel.GetReceivedInvite(ChannelAPI.InvitationServiceType.INVITATION_SERVICE_TYPE_PARTY, invitationId);
			if (receivedInvite != null && receivedInvite.State != null)
			{
				ChannelState state = receivedInvite.State;
				int num = 0;
				while (num < state.AttributeList.Count)
				{
					bnet.protocol.attribute.Attribute item = state.AttributeList[num];
					if (!(item.Name == "WTCG.Party.Type") || !item.Value.HasStringValue)
					{
						num++;
					}
					else
					{
						empty = item.Value.StringValue;
						break;
					}
				}
			}
			return empty;
		}

		public void GetReceivedPartyInvites(out PartyInvite[] invites)
		{
			ChannelAPI.ReceivedInvite[] receivedInvites = this.m_battleNet.Channel.GetReceivedInvites(ChannelAPI.InvitationServiceType.INVITATION_SERVICE_TYPE_PARTY);
			invites = new PartyInvite[(int)receivedInvites.Length];
			for (int i = 0; i < (int)invites.Length; i++)
			{
				ChannelAPI.ReceivedInvite receivedInvite = receivedInvites[i];
				Invitation invitation = receivedInvite.Invitation;
				string receivedInvitationPartyType = this.GetReceivedInvitationPartyType(invitation.Id);
				PartyType partyTypeFromString = BnetParty.GetPartyTypeFromString(receivedInvitationPartyType);
				PartyInvite partyInvite = new PartyInvite()
				{
					InviteId = invitation.Id,
					PartyId = PartyId.FromProtocol(receivedInvite.ChannelId),
					PartyType = partyTypeFromString,
					InviterName = invitation.InviterName,
					InviterId = BnetGameAccountId.CreateFromProtocol(invitation.InviterIdentity.GameAccountId),
					InviteeId = BnetGameAccountId.CreateFromProtocol(invitation.InviteeIdentity.GameAccountId)
				};
				invites[i] = partyInvite;
			}
		}

		public void IgnoreInviteRequest(bnet.protocol.EntityId partyId, bnet.protocol.EntityId requestedTargetId)
		{
			this.m_battleNet.Channel.RemoveInviteRequestsFor(partyId, requestedTargetId, 1);
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public override void InitRPCListeners(RPCConnection rpcConnection)
		{
			base.InitRPCListeners(rpcConnection);
		}

		public void JoinParty(bnet.protocol.EntityId partyId, string szPartyType)
		{
			PartyAPI.PartyData partyData = this.GetPartyData(partyId);
			if (partyData != null)
			{
				this.PushPartyError(partyId, BattleNetErrors.ERROR_PARTY_ALREADY_IN_PARTY, BnetFeatureEvent.Party_Join_Callback);
				return;
			}
			partyData = new PartyAPI.PartyData(this.m_battleNet, partyId, null);
			JoinChannelRequest joinChannelRequest = new JoinChannelRequest();
			joinChannelRequest.SetChannelId(partyId);
			joinChannelRequest.SetObjectId(ChannelAPI.GetNextObjectId());
			partyData.SetSubscriberObjectId(joinChannelRequest.ObjectId);
			partyData.m_partyId = partyId;
			this.m_rpcConnection.QueueRequest(this.m_battleNet.Channel.ChannelOwnerService.Id, 3, joinChannelRequest, (RPCContext ctx) => partyData.JoinChannelCallback(ctx, partyId, szPartyType), 2);
		}

		public void KickPartyMember(bnet.protocol.EntityId partyId, bnet.protocol.EntityId memberId)
		{
			string partyType = this.GetPartyType(partyId);
			if (this.GetPartyData(partyId) == null)
			{
				this.GenericPartyRequestCallback_Internal(BattleNetErrors.ERROR_INVALID_ARGS, "KickPartyMember no PartyData", BnetFeatureEvent.Party_KickMember_Callback, partyId, partyType);
				return;
			}
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.m_battleNet.Channel.GetChannelReferenceObject(partyId);
			if (channelReferenceObject == null)
			{
				this.GenericPartyRequestCallback_Internal(BattleNetErrors.ERROR_INVALID_ARGS, "KickPartyMember no channelRefObject", BnetFeatureEvent.Party_KickMember_Callback, partyId, partyType);
				return;
			}
			RemoveMemberRequest removeMemberRequest = new RemoveMemberRequest()
			{
				MemberId = memberId
			};
			this.m_rpcConnection.QueueRequest(this.m_battleNet.Channel.ChannelService.Id, 2, removeMemberRequest, (RPCContext ctx) => this.GenericPartyRequestCallback(ctx, string.Concat("KickPartyMember memberId=", memberId.Low), BnetFeatureEvent.Party_KickMember_Callback, partyId, partyType), (uint)channelReferenceObject.m_channelData.m_objectId);
		}

		public void LeaveParty(bnet.protocol.EntityId partyId)
		{
			string partyType = this.GetPartyType(partyId);
			if (this.GetPartyData(partyId) == null)
			{
				this.GenericPartyRequestCallback_Internal(BattleNetErrors.ERROR_INVALID_ARGS, "LeaveParty no PartyData", BnetFeatureEvent.Party_Leave_Callback, partyId, partyType);
				return;
			}
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.m_battleNet.Channel.GetChannelReferenceObject(partyId);
			if (channelReferenceObject == null)
			{
				this.GenericPartyRequestCallback_Internal(BattleNetErrors.ERROR_INVALID_ARGS, "LeaveParty no channelRefObject", BnetFeatureEvent.Party_Leave_Callback, partyId, partyType);
				return;
			}
			RemoveMemberRequest removeMemberRequest = new RemoveMemberRequest()
			{
				MemberId = this.m_battleNet.GetMyGameAccountId().ToProtocol()
			};
			this.m_rpcConnection.QueueRequest(this.m_battleNet.Channel.ChannelService.Id, 2, removeMemberRequest, (RPCContext ctx) => this.GenericPartyRequestCallback(ctx, "LeaveParty", BnetFeatureEvent.Party_Leave_Callback, partyId, partyType), (uint)channelReferenceObject.m_channelData.m_objectId);
		}

		public void MemberRolesChanged(ChannelAPI.ChannelReferenceObject channelRefObject, IEnumerable<bnet.protocol.EntityId> membersWithRoleChanges)
		{
			bnet.protocol.EntityId mChannelId = channelRefObject.m_channelData.m_channelId;
			string partyType = this.GetPartyType(mChannelId);
			PartyListenerEvent partyListenerEvent = new PartyListenerEvent()
			{
				Type = PartyListenerEventType.MEMBER_ROLE_CHANGED,
				PartyId = PartyId.FromProtocol(mChannelId),
				StringData = partyType
			};
			IEnumerator<bnet.protocol.EntityId> enumerator = membersWithRoleChanges.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					partyListenerEvent.SubjectMemberId = BnetGameAccountId.CreateFromProtocol(enumerator.Current);
					this.m_partyListenerEvents.Add(partyListenerEvent);
				}
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
		}

		public override void OnDisconnected()
		{
			base.OnDisconnected();
			this.m_activeParties.Clear();
			this.m_partyEvents.Clear();
			this.m_partyListenerEvents.Clear();
		}

		public void PartyAttributeChanged(bnet.protocol.EntityId channelId, string attributeKey, bnet.protocol.attribute.Variant attributeValue)
		{
			PartyListenerEvent intValue = new PartyListenerEvent()
			{
				Type = PartyListenerEventType.PARTY_ATTRIBUTE_CHANGED,
				PartyId = PartyId.FromProtocol(channelId),
				StringData = attributeKey
			};
			if (attributeValue.IsNone())
			{
				intValue.UintData = 0;
			}
			else if (attributeValue.HasIntValue)
			{
				intValue.UintData = 1;
				intValue.UlongData = (ulong)attributeValue.IntValue;
			}
			else if (attributeValue.HasStringValue)
			{
				intValue.UintData = 2;
				intValue.StringData2 = attributeValue.StringValue;
			}
			else if (!attributeValue.HasBlobValue)
			{
				intValue.UintData = 0;
			}
			else
			{
				intValue.UintData = 3;
				intValue.BlobData = attributeValue.BlobValue;
			}
			this.m_partyListenerEvents.Add(intValue);
		}

		public void PartyInvitationDelta(bnet.protocol.EntityId partyId, Invitation invite, uint? removeReason)
		{
			string partyType = this.GetPartyType(partyId);
			PartyListenerEvent uintData = new PartyListenerEvent()
			{
				Type = (!removeReason.HasValue ? PartyListenerEventType.PARTY_INVITE_SENT : PartyListenerEventType.PARTY_INVITE_REMOVED),
				PartyId = PartyId.FromProtocol(partyId),
				UlongData = invite.Id,
				SubjectMemberId = BnetGameAccountId.CreateFromProtocol(invite.InviterIdentity.GameAccountId),
				TargetMemberId = BnetGameAccountId.CreateFromProtocol(invite.InviteeIdentity.GameAccountId),
				StringData = partyType,
				StringData2 = invite.InviterName
			};
			if (!removeReason.HasValue)
			{
				uintData.UintData = 0;
				if (invite.HasChannelInvitation)
				{
					ChannelInvitation channelInvitation = invite.ChannelInvitation;
					if (channelInvitation.HasReserved && channelInvitation.Reserved)
					{
						uintData.UintData = uintData.UintData | 1;
					}
					if (channelInvitation.HasRejoin && channelInvitation.Rejoin)
					{
						uintData.UintData = uintData.UintData | 1;
					}
				}
			}
			else
			{
				uintData.UintData = removeReason.Value;
			}
			this.m_partyListenerEvents.Add(uintData);
		}

		public void PartyJoined(ChannelAPI.ChannelReferenceObject channelRefObject, AddNotification notification)
		{
			bnet.protocol.EntityId mChannelId = ((ChannelAPI.ChannelData)channelRefObject.m_channelData).m_channelId;
			string partyType = this.GetPartyType(mChannelId);
			PartyAPI.PartyData partyData = this.GetPartyData(mChannelId);
			bool flag = false;
			if ((partyData != null ? true : false))
			{
				flag = (partyData == null ? false : partyData.m_friendGameAccount != null);
			}
			else if (BnetParty.GetPartyTypeFromString(partyType) != PartyType.FRIENDLY_CHALLENGE)
			{
				PartyAPI.PartyData partyDatum = new PartyAPI.PartyData(this.m_battleNet, mChannelId, null);
				this.m_activeParties.Add(mChannelId, partyDatum);
			}
			else
			{
				flag = true;
				foreach (Member memberList in notification.MemberList)
				{
					bnet.protocol.EntityId gameAccountId = memberList.Identity.GameAccountId;
					if (this.m_battleNet.GameAccountId.Equals(gameAccountId))
					{
						continue;
					}
					PartyAPI.PartyData partyDatum1 = new PartyAPI.PartyData(this.m_battleNet, mChannelId, gameAccountId);
					this.m_activeParties.Add(mChannelId, partyDatum1);
					partyDatum1.FriendlyChallenge_HandleChannelAttributeUpdate(notification.ChannelState.AttributeList);
					break;
				}
			}
			if (flag)
			{
				this.FriendlyChallenge_PushStateChange(mChannelId, "wait", false);
			}
			PartyListenerEvent partyListenerEvent = new PartyListenerEvent()
			{
				Type = PartyListenerEventType.JOINED_PARTY,
				PartyId = PartyId.FromProtocol(mChannelId),
				StringData = partyType
			};
			this.m_partyListenerEvents.Add(partyListenerEvent);
		}

		public void PartyLeft(ChannelAPI.ChannelReferenceObject channelRefObject, RemoveNotification notification)
		{
			uint reason;
			ChannelAPI.ChannelData mChannelData = (ChannelAPI.ChannelData)channelRefObject.m_channelData;
			bnet.protocol.EntityId mChannelId = channelRefObject.m_channelData.m_channelId;
			string partyType = this.GetPartyType(mChannelId);
			PartyAPI.PartyData partyData = this.GetPartyData(mChannelData.m_channelId);
			if (partyData != null)
			{
				if (partyData.m_friendGameAccount != null)
				{
					string str = "NO_SUPPLIED_REASON";
					if (notification.HasReason)
					{
						str = notification.Reason.ToString();
					}
					this.PushPartyEvent(partyData.m_partyId, "left", str, partyData.m_friendGameAccount);
				}
				this.m_activeParties.Remove(partyData.m_partyId);
			}
			PartyListenerEvent partyListenerEvent = new PartyListenerEvent()
			{
				Type = PartyListenerEventType.LEFT_PARTY,
				PartyId = PartyId.FromProtocol(mChannelId),
				StringData = partyType
			};
			if (!notification.HasReason)
			{
				reason = 0;
			}
			else
			{
				reason = notification.Reason;
			}
			partyListenerEvent.UintData = reason;
			this.m_partyListenerEvents.Add(partyListenerEvent);
		}

		public void PartyMemberJoined(ChannelAPI.ChannelReferenceObject channelRefObject, JoinNotification notification)
		{
			bnet.protocol.EntityId mChannelId = channelRefObject.m_channelData.m_channelId;
			PartyListenerEvent partyListenerEvent = new PartyListenerEvent()
			{
				Type = PartyListenerEventType.MEMBER_JOINED,
				PartyId = PartyId.FromProtocol(mChannelId),
				SubjectMemberId = BnetGameAccountId.CreateFromProtocol(notification.Member.Identity.GameAccountId)
			};
			this.m_partyListenerEvents.Add(partyListenerEvent);
		}

		public void PartyMemberLeft(ChannelAPI.ChannelReferenceObject channelRefObject, LeaveNotification notification)
		{
			uint reason;
			bnet.protocol.EntityId mChannelId = channelRefObject.m_channelData.m_channelId;
			PartyAPI.PartyData partyData = this.GetPartyData(mChannelId);
			if (partyData != null && partyData.m_friendGameAccount != null)
			{
				this.m_rpcConnection.QueueRequest(this.m_battleNet.Channel.ChannelService.Id, 6, new DissolveRequest(), new RPCContextDelegate(partyData.PartyMemberLeft_DissolvePartyInviteCallback), (uint)channelRefObject.m_channelData.m_objectId);
			}
			PartyListenerEvent partyListenerEvent = new PartyListenerEvent()
			{
				Type = PartyListenerEventType.MEMBER_LEFT,
				PartyId = PartyId.FromProtocol(mChannelId),
				SubjectMemberId = BnetGameAccountId.CreateFromProtocol(notification.MemberId)
			};
			if (!notification.HasReason)
			{
				reason = 0;
			}
			else
			{
				reason = notification.Reason;
			}
			partyListenerEvent.UintData = reason;
			this.m_partyListenerEvents.Add(partyListenerEvent);
		}

		public void PartyMessageReceived(ChannelAPI.ChannelReferenceObject channelRefObject, SendMessageNotification notification)
		{
			bnet.protocol.EntityId mChannelId = channelRefObject.m_channelData.m_channelId;
			string stringValue = null;
			int num = 0;
			while (num < notification.Message.AttributeCount)
			{
				bnet.protocol.attribute.Attribute item = notification.Message.AttributeList[num];
				if (!attribute.TEXT_ATTRIBUTE.Equals(item.Name) || !item.Value.HasStringValue)
				{
					num++;
				}
				else
				{
					stringValue = item.Value.StringValue;
					break;
				}
			}
			if (string.IsNullOrEmpty(stringValue))
			{
				return;
			}
			PartyListenerEvent partyListenerEvent = new PartyListenerEvent()
			{
				Type = PartyListenerEventType.CHAT_MESSAGE_RECEIVED,
				PartyId = PartyId.FromProtocol(mChannelId),
				SubjectMemberId = BnetGameAccountId.CreateFromProtocol(notification.AgentId),
				StringData = stringValue
			};
			this.m_partyListenerEvents.Add(partyListenerEvent);
		}

		public void PartyPrivacyChanged(bnet.protocol.EntityId channelId, ChannelState.Types.PrivacyLevel newPrivacyLevel)
		{
			string partyType = this.GetPartyType(channelId);
			PartyListenerEvent partyListenerEvent = new PartyListenerEvent()
			{
				Type = PartyListenerEventType.PRIVACY_CHANGED,
				PartyId = PartyId.FromProtocol(channelId),
				UintData = (uint)newPrivacyLevel,
				StringData = partyType
			};
			this.m_partyListenerEvents.Add(partyListenerEvent);
		}

		public void PreprocessPartyChannelUpdated(ChannelAPI.ChannelReferenceObject channelRefObject, UpdateChannelStateNotification notification)
		{
			PartyAPI.PartyData partyData = this.GetPartyData(channelRefObject.m_channelData.m_channelId);
			if (partyData != null && partyData.m_friendGameAccount != null)
			{
				partyData.FriendlyChallenge_HandleChannelAttributeUpdate(notification.StateChange.AttributeList);
			}
		}

		private void PushPartyError(bnet.protocol.EntityId partyId, BattleNetErrors errorCode, BnetFeatureEvent featureEvent)
		{
			PartyEvent high = new PartyEvent();
			if (partyId != null)
			{
				high.partyId.hi = partyId.High;
				high.partyId.lo = partyId.Low;
			}
			high.errorInfo = new BnetErrorInfo(BnetFeature.Party, featureEvent, errorCode);
			this.m_partyEvents.Add(high);
		}

		private void PushPartyErrorEvent(PartyListenerEventType evtType, string szDebugContext, Error error, BnetFeatureEvent featureEvent, bnet.protocol.EntityId partyId = null, string szStringData = null)
		{
			PartyListenerEvent partyListenerEvent = new PartyListenerEvent()
			{
				Type = evtType,
				PartyId = (partyId != null ? PartyId.FromProtocol(partyId) : new PartyId()),
				UintData = error.Code,
				UlongData = 17179869184L | (ulong)featureEvent,
				StringData = (szDebugContext != null ? szDebugContext : string.Empty),
				StringData2 = (szStringData != null ? szStringData : string.Empty)
			};
			this.m_partyListenerEvents.Add(partyListenerEvent);
		}

		private void PushPartyEvent(bnet.protocol.EntityId partyId, string type, string data, bnet.protocol.EntityId friendGameAccount)
		{
			PartyEvent high = new PartyEvent();
			high.partyId.hi = partyId.High;
			high.partyId.lo = partyId.Low;
			high.eventName = type;
			high.eventData = data;
			high.otherMemberId.hi = friendGameAccount.High;
			high.otherMemberId.lo = friendGameAccount.Low;
			this.m_partyEvents.Add(high);
		}

		public void ReceivedInvitationAdded(InvitationAddedNotification notification, ChannelInvitation channelInvitation)
		{
			string receivedInvitationPartyType = this.GetReceivedInvitationPartyType(notification.Invitation.Id);
			if (BnetParty.GetPartyTypeFromString(receivedInvitationPartyType) == PartyType.FRIENDLY_CHALLENGE)
			{
				this.m_battleNet.Channel.AcceptInvitation(notification.Invitation.Id, channelInvitation.ChannelDescription.ChannelId, ChannelAPI.ChannelType.PARTY_CHANNEL, null);
			}
			PartyListenerEvent uintData = new PartyListenerEvent()
			{
				Type = PartyListenerEventType.RECEIVED_INVITE_ADDED,
				PartyId = PartyId.FromProtocol(channelInvitation.ChannelDescription.ChannelId),
				UlongData = notification.Invitation.Id,
				SubjectMemberId = BnetGameAccountId.CreateFromProtocol(notification.Invitation.InviterIdentity.GameAccountId),
				TargetMemberId = BnetGameAccountId.CreateFromProtocol(notification.Invitation.InviteeIdentity.GameAccountId),
				StringData = receivedInvitationPartyType,
				StringData2 = notification.Invitation.InviterName,
				UintData = 0
			};
			if (channelInvitation.HasReserved && channelInvitation.Reserved)
			{
				uintData.UintData = uintData.UintData | 1;
			}
			if (channelInvitation.HasRejoin && channelInvitation.Rejoin)
			{
				uintData.UintData = uintData.UintData | 1;
			}
			this.m_partyListenerEvents.Add(uintData);
		}

		public void ReceivedInvitationRemoved(string szPartyType, InvitationRemovedNotification notification, ChannelInvitation channelInvitation)
		{
			PartyListenerEvent reason = new PartyListenerEvent()
			{
				Type = PartyListenerEventType.RECEIVED_INVITE_REMOVED,
				PartyId = PartyId.FromProtocol(channelInvitation.ChannelDescription.ChannelId),
				UlongData = notification.Invitation.Id,
				SubjectMemberId = BnetGameAccountId.CreateFromProtocol(notification.Invitation.InviterIdentity.GameAccountId),
				TargetMemberId = BnetGameAccountId.CreateFromProtocol(notification.Invitation.InviteeIdentity.GameAccountId),
				StringData = szPartyType,
				StringData2 = notification.Invitation.InviterName
			};
			if (notification.HasReason)
			{
				reason.UintData = notification.Reason;
			}
			this.m_partyListenerEvents.Add(reason);
		}

		public void ReceivedInviteRequestDelta(bnet.protocol.EntityId partyId, Suggestion suggestion, uint? removeReason)
		{
			PartyListenerEvent value = new PartyListenerEvent()
			{
				Type = (!removeReason.HasValue ? PartyListenerEventType.INVITE_REQUEST_ADDED : PartyListenerEventType.INVITE_REQUEST_REMOVED),
				PartyId = PartyId.FromProtocol(partyId),
				SubjectMemberId = BnetGameAccountId.CreateFromProtocol(suggestion.SuggesterId),
				TargetMemberId = BnetGameAccountId.CreateFromProtocol(suggestion.SuggesteeId),
				StringData = suggestion.SuggesterName,
				StringData2 = suggestion.SuggesteeName
			};
			if (removeReason.HasValue)
			{
				value.UintData = removeReason.Value;
			}
			this.m_partyListenerEvents.Add(value);
		}

		public void RequestPartyInvite(bnet.protocol.EntityId partyId, bnet.protocol.EntityId whomToAskForApproval, bnet.protocol.EntityId whomToInvite, string szPartyType)
		{
			this.m_battleNet.Channel.SuggestInvitation(partyId, whomToAskForApproval, whomToInvite, (RPCContext ctx) => this.GenericPartyRequestCallback(ctx, string.Concat(new object[] { "RequestPartyInvite whomToInvite=", whomToInvite, " whomToAskForApproval=", whomToAskForApproval }), BnetFeatureEvent.Party_RequestPartyInvite_Callback, partyId, szPartyType));
		}

		public void RevokePartyInvite(bnet.protocol.EntityId partyId, ulong invitationId)
		{
			string partyType = this.GetPartyType(partyId);
			this.m_battleNet.Channel.RevokeInvitation(invitationId, partyId, (RPCContext ctx) => this.GenericPartyRequestCallback(ctx, string.Concat("RevokePartyInvite inviteId=", invitationId), BnetFeatureEvent.Party_RevokeInvite_Callback, partyId, partyType));
		}

		public void SendFriendlyChallengeInvite(bnet.protocol.EntityId friendEntityId, int scenarioId)
		{
			PartyAPI.PartyData partyDatum = new PartyAPI.PartyData(this.m_battleNet, friendEntityId)
			{
				m_scenarioId = scenarioId
			};
			CreateChannelRequest createChannelRequest = new CreateChannelRequest();
			createChannelRequest.SetObjectId(ChannelAPI.GetNextObjectId());
			partyDatum.SetSubscriberObjectId(createChannelRequest.ObjectId);
			ChannelState channelState = new ChannelState();
			channelState.SetName("FriendlyGame");
			channelState.SetPrivacyLevel(ChannelState.Types.PrivacyLevel.PRIVACY_LEVEL_OPEN);
			channelState.AddAttribute(ProtocolHelper.CreateAttribute("WTCG.Party.Type", "FriendlyGame"));
			channelState.AddAttribute(ProtocolHelper.CreateAttribute("WTCG.Game.ScenarioId", (long)scenarioId));
			createChannelRequest.SetChannelState(channelState);
			this.m_rpcConnection.QueueRequest(this.m_battleNet.Channel.ChannelOwnerService.Id, 2, createChannelRequest, (RPCContext ctx) => partyDatum.CreateChannelCallback(ctx, "FriendlyGame"), 2);
		}

		public void SendPartyChatMessage(bnet.protocol.EntityId partyId, string message)
		{
			string partyType = this.GetPartyType(partyId);
			if (this.GetPartyData(partyId) == null)
			{
				this.GenericPartyRequestCallback_Internal(BattleNetErrors.ERROR_INVALID_ARGS, "SendPartyChatMessage no PartyData", BnetFeatureEvent.Party_SendChatMessage_Callback, partyId, partyType);
				return;
			}
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.m_battleNet.Channel.GetChannelReferenceObject(partyId);
			if (channelReferenceObject == null)
			{
				this.GenericPartyRequestCallback_Internal(BattleNetErrors.ERROR_INVALID_ARGS, "SendPartyChatMessage no channelRefObject", BnetFeatureEvent.Party_SendChatMessage_Callback, partyId, partyType);
				return;
			}
			SendMessageRequest sendMessageRequest = new SendMessageRequest();
			Message message1 = new Message();
			message1.AddAttribute(ProtocolHelper.CreateAttribute(attribute.TEXT_ATTRIBUTE, message));
			sendMessageRequest.SetMessage(message1);
			this.m_rpcConnection.QueueRequest(this.m_battleNet.Channel.ChannelService.Id, 3, sendMessageRequest, (RPCContext ctx) => this.GenericPartyRequestCallback(ctx, "SendPartyChatMessage", BnetFeatureEvent.Party_SendChatMessage_Callback, partyId, partyType), (uint)channelReferenceObject.m_channelData.m_objectId);
		}

		public void SendPartyInvite(bnet.protocol.EntityId partyId, bnet.protocol.EntityId inviteeId, bool isReservation)
		{
			if (this.GetPartyData(partyId) == null)
			{
				return;
			}
			string partyType = this.GetPartyType(partyId);
			this.m_battleNet.Channel.SendInvitation(partyId, inviteeId, ChannelAPI.InvitationServiceType.INVITATION_SERVICE_TYPE_PARTY, (RPCContext ctx) => this.SendPartyInviteCallback(ctx, partyId, inviteeId, partyType));
		}

		public void SendPartyInviteCallback(RPCContext context, bnet.protocol.EntityId partyId, bnet.protocol.EntityId inviteeId, string szPartyType)
		{
			string str = string.Concat("SendPartyInvite inviteeId=", inviteeId.Low);
			BattleNetErrors status = (BattleNetErrors)context.Header.Status;
			this.m_battleNet.Party.PushPartyErrorEvent(PartyListenerEventType.OPERATION_CALLBACK, str, status, BnetFeatureEvent.Party_SendInvite_Callback, partyId, szPartyType);
			if (status == BattleNetErrors.ERROR_OK)
			{
				str = (partyId == null ? string.Format("PartyRequest {0} status={1} type={2}", str, status.ToString(), szPartyType) : string.Format("PartyRequest {0} status={1} partyId={2} type={3}", new object[] { str, status.ToString(), partyId.Low, szPartyType }));
				this.m_battleNet.Party.ApiLog.LogDebug(str);
				this.m_battleNet.Channel.RemoveInviteRequestsFor(partyId, inviteeId, 0);
				return;
			}
			str = (partyId == null ? string.Format("PartyRequestError: {0} {1} {2} type={3}", new object[] { (int)status, status.ToString(), str, szPartyType }) : string.Format("PartyRequestError: {0} {1} {2} partyId={3} type={4}", new object[] { (int)status, status.ToString(), str, partyId.Low, szPartyType }));
			this.m_battleNet.Party.ApiLog.LogError(str);
		}

		private void SetPartyAttribute_Internal(string debugMessage, BnetFeatureEvent featureEvent, bnet.protocol.EntityId partyId, bnet.protocol.attribute.Attribute attr)
		{
			ChannelState channelState = new ChannelState();
			channelState.AddAttribute(attr);
			this.UpdatePartyState_Internal(debugMessage, featureEvent, partyId, channelState);
		}

		public void SetPartyAttributeBlob(bnet.protocol.EntityId partyId, string attributeKey, byte[] value)
		{
			this.SetPartyAttribute_Internal(string.Concat("SetPartyAttributeBlob key=", attributeKey, " val=", (value != null ? string.Concat((int)value.Length, " bytes") : "null")), BnetFeatureEvent.Party_SetAttribute_Callback, partyId, ProtocolHelper.CreateAttribute(attributeKey, value));
		}

		public void SetPartyAttributeLong(bnet.protocol.EntityId partyId, string attributeKey, long value)
		{
			this.SetPartyAttribute_Internal(string.Concat(new object[] { "SetPartyAttributeLong key=", attributeKey, " val=", value }), BnetFeatureEvent.Party_SetAttribute_Callback, partyId, ProtocolHelper.CreateAttribute(attributeKey, value));
		}

		public void SetPartyAttributeString(bnet.protocol.EntityId partyId, string attributeKey, string value)
		{
			this.SetPartyAttribute_Internal(string.Concat("SetPartyAttributeString key=", attributeKey, " val=", value), BnetFeatureEvent.Party_SetAttribute_Callback, partyId, ProtocolHelper.CreateAttribute(attributeKey, value));
		}

		public void SetPartyDeck(bnet.protocol.EntityId partyId, long deckId)
		{
			PartyAPI.PartyData partyData = this.GetPartyData(partyId);
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.m_battleNet.Channel.GetChannelReferenceObject(partyId);
			if (partyData == null || channelReferenceObject == null)
			{
				this.PushPartyEvent(partyId, "parm", "party", new bnet.protocol.EntityId());
				return;
			}
			this.PushPartyEvent(partyId, "dll", "deck", partyData.m_friendGameAccount);
			if (deckId == 0)
			{
				this.FriendlyChallenge_PushStateChange(partyId, "deck", false);
			}
			else
			{
				this.FriendlyChallenge_PushStateChange(partyId, "game", false);
			}
			List<bnet.protocol.attribute.Attribute> attributes = new List<bnet.protocol.attribute.Attribute>()
			{
				ProtocolHelper.CreateAttribute((!partyData.m_maker ? "d2" : "d1"), deckId)
			};
			this.m_battleNet.Channel.UpdateChannelAttributes((ChannelAPI.ChannelData)channelReferenceObject.m_channelData, attributes, null);
			if (!partyData.m_maker)
			{
				return;
			}
			partyData.m_makerDeck = deckId;
			if (deckId != 0)
			{
				partyData.StartFriendlyChallengeGameIfReady();
			}
		}

		public void SetPartyPrivacy(bnet.protocol.EntityId partyId, int privacyLevel)
		{
			ChannelState channelState = new ChannelState()
			{
				PrivacyLevel = (ChannelState.Types.PrivacyLevel)privacyLevel
			};
			this.UpdatePartyState_Internal(string.Concat("SetPartyPrivacy privacy=", privacyLevel), BnetFeatureEvent.Party_SetPrivacy_Callback, partyId, channelState);
		}

		private void UpdatePartyState_Internal(string debugMessage, BnetFeatureEvent featureEvent, bnet.protocol.EntityId partyId, ChannelState state)
		{
			string partyType = this.GetPartyType(partyId);
			if (this.GetPartyData(partyId) == null)
			{
				this.GenericPartyRequestCallback_Internal(BattleNetErrors.ERROR_INVALID_ARGS, string.Format("{0} no PartyData", debugMessage), featureEvent, partyId, partyType);
				return;
			}
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.m_battleNet.Channel.GetChannelReferenceObject(partyId);
			if (channelReferenceObject == null)
			{
				this.GenericPartyRequestCallback_Internal(BattleNetErrors.ERROR_INVALID_ARGS, string.Format("{0} no channelRefObject", debugMessage), featureEvent, partyId, partyType);
				return;
			}
			UpdateChannelStateRequest updateChannelStateRequest = new UpdateChannelStateRequest()
			{
				StateChange = state
			};
			this.m_rpcConnection.QueueRequest(this.m_battleNet.Channel.ChannelService.Id, 4, updateChannelStateRequest, (RPCContext ctx) => this.GenericPartyRequestCallback(ctx, debugMessage, featureEvent, partyId, partyType), (uint)channelReferenceObject.m_channelData.m_objectId);
		}

		public struct PartyCreateOptions
		{
			public string m_name;

			public ChannelState.Types.PrivacyLevel m_privacyLevel;
		}

		public class PartyData
		{
			public bnet.protocol.EntityId m_partyId;

			public bnet.protocol.EntityId m_friendGameAccount;

			public int m_scenarioId;

			public long m_makerDeck;

			public long m_inviteeDeck;

			public bool m_maker;

			public ulong m_subscriberObjectId;

			private BattleNetCSharp m_battleNet;

			public PartyData(BattleNetCSharp battlenet)
			{
				this.m_battleNet = battlenet;
			}

			public PartyData(BattleNetCSharp battlenet, bnet.protocol.EntityId friendGameAccount)
			{
				this.m_friendGameAccount = friendGameAccount;
				this.m_maker = true;
				this.m_battleNet = battlenet;
			}

			public PartyData(BattleNetCSharp battlenet, bnet.protocol.EntityId partyId, bnet.protocol.EntityId friendGameAccount)
			{
				this.m_partyId = partyId;
				this.m_friendGameAccount = friendGameAccount;
				this.m_battleNet = battlenet;
			}

			public void CreateChannelCallback(RPCContext context, string szPartyType)
			{
				BattleNetErrors status = (BattleNetErrors)context.Header.Status;
				this.m_battleNet.Party.PushPartyErrorEvent(PartyListenerEventType.OPERATION_CALLBACK, "CreateParty", status, BnetFeatureEvent.Party_Create_Callback, null, szPartyType);
				if (status != BattleNetErrors.ERROR_OK)
				{
					this.m_battleNet.Party.ApiLog.LogError(string.Concat("CreateChannelCallback: code=", new Error(status)));
					this.m_battleNet.Party.PushPartyError(new bnet.protocol.EntityId(), status, BnetFeatureEvent.Party_Create_Callback);
					return;
				}
				CreateChannelResponse createChannelResponse = CreateChannelResponse.ParseFrom(context.Payload);
				this.m_partyId = createChannelResponse.ChannelId;
				ChannelAPI.ChannelData channelDatum = new ChannelAPI.ChannelData(this.m_battleNet.Channel, createChannelResponse.ChannelId, createChannelResponse.ObjectId, ChannelAPI.ChannelType.PARTY_CHANNEL);
				channelDatum.SetSubscriberObjectId(this.m_subscriberObjectId);
				this.m_battleNet.Party.AddActiveChannel(this.m_subscriberObjectId, new ChannelAPI.ChannelReferenceObject(channelDatum), this);
				if (this.m_friendGameAccount != null)
				{
					this.m_battleNet.Channel.SendInvitation(createChannelResponse.ChannelId, this.m_friendGameAccount, ChannelAPI.InvitationServiceType.INVITATION_SERVICE_TYPE_PARTY, new RPCContextDelegate(this.CreateParty_SendInvitationCallback));
				}
				this.m_battleNet.Party.ApiLog.LogDebug(string.Concat("CreateChannelCallback code=", new Error(status)));
			}

			private void CreateParty_SendInvitationCallback(RPCContext context)
			{
				BattleNetErrors status = (BattleNetErrors)context.Header.Status;
				if (status != BattleNetErrors.ERROR_OK)
				{
					this.m_battleNet.Party.ApiLog.LogError(string.Concat("SendInvitationCallback: ", status.ToString()));
					this.m_battleNet.Party.PushPartyError(this.m_partyId, status, BnetFeatureEvent.Party_SendInvite_Callback);
					return;
				}
				SendInvitationResponse sendInvitationResponse = SendInvitationResponse.ParseFrom(context.Payload);
				if (sendInvitationResponse.Invitation.HasChannelInvitation)
				{
					if (sendInvitationResponse.Invitation.ChannelInvitation.ServiceType == 1)
					{
						this.m_battleNet.Party.PushPartyEvent(this.m_partyId, "cb", "inv", this.m_friendGameAccount);
					}
				}
				this.m_battleNet.Party.ApiLog.LogDebug(string.Concat("SendInvitationCallback, status=", status.ToString()));
			}

			public void DeclineInvite_DissolvePartyInviteCallback(RPCContext context)
			{
				BattleNetErrors status = (BattleNetErrors)context.Header.Status;
				if (status != BattleNetErrors.ERROR_OK)
				{
					this.m_battleNet.Party.ApiLog.LogError(string.Concat("DisolvePartyInviteCallback: ", status.ToString()));
					this.m_battleNet.Party.PushPartyError(this.m_partyId, status, BnetFeatureEvent.Party_Dissolve_Callback);
					return;
				}
				if (this.m_friendGameAccount != null)
				{
					this.m_battleNet.Party.PushPartyEvent(this.m_partyId, "cb", "drop", this.m_friendGameAccount);
				}
				this.m_battleNet.Party.ApiLog.LogDebug(string.Concat("DisolvePartyInviteCallback, status=", status.ToString()));
			}

			public void FriendlyChallenge_HandleChannelAttributeUpdate(IList<bnet.protocol.attribute.Attribute> attributeList)
			{
				IEnumerator<bnet.protocol.attribute.Attribute> enumerator = attributeList.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						bnet.protocol.attribute.Attribute current = enumerator.Current;
						if (current.Value.HasIntValue)
						{
							if (current.Name == "d2")
							{
								this.m_inviteeDeck = current.Value.IntValue;
								this.StartFriendlyChallengeGameIfReady();
							}
						}
						else if (!current.Value.HasStringValue)
						{
							this.m_battleNet.Party.ApiLog.LogError(string.Concat("Party : unknown value type, key: ", current.Name));
						}
						else
						{
							this.m_battleNet.Party.PushPartyEvent(this.m_partyId, current.Name, current.Value.StringValue, this.m_friendGameAccount);
						}
					}
				}
				finally
				{
					if (enumerator == null)
					{
					}
					enumerator.Dispose();
				}
			}

			public void JoinChannelCallback(RPCContext context, bnet.protocol.EntityId partyId, string szPartyType)
			{
				BattleNetErrors status = (BattleNetErrors)context.Header.Status;
				this.m_battleNet.Party.PushPartyErrorEvent(PartyListenerEventType.OPERATION_CALLBACK, "JoinParty", status, BnetFeatureEvent.Party_Join_Callback, partyId, szPartyType);
				if (status != BattleNetErrors.ERROR_OK)
				{
					this.m_battleNet.Party.ApiLog.LogError(string.Concat("JoinChannelCallback: code=", new Error(status)));
					this.m_battleNet.Party.PushPartyError(new bnet.protocol.EntityId(), status, BnetFeatureEvent.Party_Join_Callback);
					return;
				}
				JoinChannelResponse joinChannelResponse = JoinChannelResponse.ParseFrom(context.Payload);
				ChannelAPI.ChannelData channelDatum = new ChannelAPI.ChannelData(this.m_battleNet.Channel, this.m_partyId, joinChannelResponse.ObjectId, ChannelAPI.ChannelType.PARTY_CHANNEL);
				channelDatum.SetSubscriberObjectId(this.m_subscriberObjectId);
				this.m_battleNet.Party.AddActiveChannel(this.m_subscriberObjectId, new ChannelAPI.ChannelReferenceObject(channelDatum), this);
				this.m_battleNet.Party.ApiLog.LogDebug(string.Concat("JoinChannelCallback cide=", new Error(status)));
			}

			public void PartyMemberLeft_DissolvePartyInviteCallback(RPCContext context)
			{
				BattleNetErrors status = (BattleNetErrors)context.Header.Status;
				if (status == BattleNetErrors.ERROR_OK)
				{
					this.m_battleNet.Party.ApiLog.LogDebug(string.Concat("PartyMemberLeft_DissolvePartyInviteCallback, status=", status.ToString()));
					return;
				}
				this.m_battleNet.Party.ApiLog.LogError(string.Concat("PartyMemberLeft_DissolvePartyInviteCallback: ", status.ToString()));
				this.m_battleNet.Party.PushPartyError(this.m_partyId, status, BnetFeatureEvent.Party_Dissolve_Callback);
			}

			public void SetSubscriberObjectId(ulong objectId)
			{
				this.m_subscriberObjectId = objectId;
			}

			public void StartFriendlyChallengeGameIfReady()
			{
				ChannelAPI.ChannelReferenceObject channelReferenceObject = this.m_battleNet.Channel.GetChannelReferenceObject(this.m_partyId);
				if (channelReferenceObject == null)
				{
					return;
				}
				if (!this.m_maker)
				{
					return;
				}
				if (this.m_makerDeck == 0)
				{
					return;
				}
				if (this.m_inviteeDeck == 0)
				{
					return;
				}
				List<bnet.protocol.attribute.Attribute> attributes = new List<bnet.protocol.attribute.Attribute>()
				{
					ProtocolHelper.CreateAttribute("s1", "goto"),
					ProtocolHelper.CreateAttribute("s2", "goto")
				};
				this.m_battleNet.Channel.UpdateChannelAttributes((ChannelAPI.ChannelData)channelReferenceObject.m_channelData, attributes, null);
				this.m_battleNet.Games.CreateFriendlyChallengeGame(this.m_makerDeck, this.m_inviteeDeck, this.m_friendGameAccount, this.m_scenarioId);
				this.m_makerDeck = (long)0;
				this.m_inviteeDeck = (long)0;
			}
		}
	}
}