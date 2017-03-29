using bgs.RPCServices;
using bnet.protocol;
using bnet.protocol.attribute;
using bnet.protocol.channel;
using bnet.protocol.channel_invitation;
using bnet.protocol.invitation;
using bnet.protocol.presence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace bgs
{
	public class ChannelAPI : BattleNetAPI
	{
		private static ulong s_nextObjectId;

		private Map<ulong, ChannelAPI.ChannelReferenceObject> m_activeChannels = new Map<ulong, ChannelAPI.ChannelReferenceObject>();

		private Map<EntityId, ulong> m_channelEntityObjectMap = new Map<EntityId, ulong>();

		private Map<ChannelAPI.InvitationServiceType, List<ChannelAPI.ReceivedInvite>> m_receivedInvitations = new Map<ChannelAPI.InvitationServiceType, List<ChannelAPI.ReceivedInvite>>();

		private Map<EntityId, List<Suggestion>> m_receivedInviteRequests;

		private ServiceDescriptor m_channelService = new bgs.RPCServices.ChannelService();

		private ServiceDescriptor m_channelSubscriberService = new bgs.RPCServices.ChannelSubscriberService();

		private ServiceDescriptor m_channelOwnerService = new bgs.RPCServices.ChannelOwnerService();

		private static ServiceDescriptor m_channelInvitationService;

		private static ServiceDescriptor m_channelInvitationNotifyService;

		public ServiceDescriptor ChannelInvitationNotifyService
		{
			get
			{
				return ChannelAPI.m_channelInvitationNotifyService;
			}
		}

		public ServiceDescriptor ChannelInvitationService
		{
			get
			{
				return ChannelAPI.m_channelInvitationService;
			}
		}

		public ServiceDescriptor ChannelOwnerService
		{
			get
			{
				return this.m_channelOwnerService;
			}
		}

		public ServiceDescriptor ChannelService
		{
			get
			{
				return this.m_channelService;
			}
		}

		public ServiceDescriptor ChannelSubscriberService
		{
			get
			{
				return this.m_channelSubscriberService;
			}
		}

		static ChannelAPI()
		{
			ChannelAPI.s_nextObjectId = (ulong)0;
			ChannelAPI.m_channelInvitationService = new bgs.RPCServices.ChannelInvitationService();
			ChannelAPI.m_channelInvitationNotifyService = new bgs.RPCServices.ChannelInvitationNotifyService();
		}

		public ChannelAPI(BattleNetCSharp battlenet) : base(battlenet, "Channel")
		{
		}

		public void AcceptInvitation(ulong invitationId, EntityId channelId, ChannelAPI.ChannelType channelType, RPCContextDelegate callback = null)
		{
			AcceptInvitationRequest acceptInvitationRequest = new AcceptInvitationRequest();
			acceptInvitationRequest.SetInvitationId(invitationId);
			acceptInvitationRequest.SetObjectId(ChannelAPI.GetNextObjectId());
			ChannelAPI.ChannelData channelDatum = new ChannelAPI.ChannelData(this, channelId, (ulong)0, channelType);
			channelDatum.SetSubscriberObjectId(acceptInvitationRequest.ObjectId);
			this.m_rpcConnection.QueueRequest(ChannelAPI.m_channelInvitationService.Id, 4, acceptInvitationRequest, (RPCContext ctx) => channelDatum.AcceptInvitationCallback(ctx, callback), 0);
		}

		public void AddActiveChannel(ulong objectId, ChannelAPI.ChannelReferenceObject channelRefObject)
		{
			this.m_activeChannels.Add(objectId, channelRefObject);
			this.m_channelEntityObjectMap[channelRefObject.m_channelData.m_channelId] = objectId;
		}

		public void DeclineInvitation(ulong invitationId, EntityId channelId, RPCContextDelegate callback)
		{
			GenericRequest genericRequest = new GenericRequest();
			genericRequest.SetInvitationId(invitationId);
			this.m_rpcConnection.QueueRequest(ChannelAPI.m_channelInvitationService.Id, 5, genericRequest, callback, 0);
		}

		public ChannelAPI.ReceivedInvite[] GetAllReceivedInvites()
		{
			return this.m_receivedInvitations.Values.SelectMany<List<ChannelAPI.ReceivedInvite>, ChannelAPI.ReceivedInvite>((List<ChannelAPI.ReceivedInvite> l) => l).ToArray<ChannelAPI.ReceivedInvite>();
		}

		public ChannelAPI.ChannelReferenceObject GetChannelReferenceObject(EntityId entityId)
		{
			ulong num;
			ChannelAPI.ChannelReferenceObject channelReferenceObject;
			if (this.m_channelEntityObjectMap.TryGetValue(entityId, out num) && this.m_activeChannels.TryGetValue(num, out channelReferenceObject))
			{
				return channelReferenceObject;
			}
			return null;
		}

		public ChannelAPI.ChannelReferenceObject GetChannelReferenceObject(ulong objectId)
		{
			ChannelAPI.ChannelReferenceObject channelReferenceObject;
			if (this.m_activeChannels.TryGetValue(objectId, out channelReferenceObject))
			{
				return channelReferenceObject;
			}
			return null;
		}

		public Suggestion[] GetInviteRequests(EntityId channelId)
		{
			List<Suggestion> suggestions;
			Suggestion[] array = null;
			if (this.m_receivedInviteRequests != null && this.m_receivedInviteRequests.TryGetValue(channelId, out suggestions))
			{
				array = suggestions.ToArray();
			}
			if (array == null)
			{
				array = new Suggestion[0];
			}
			return array;
		}

		public static ulong GetNextObjectId()
		{
			UInt64 sNextObjectId = ChannelAPI.s_nextObjectId + (long)1;
			ChannelAPI.s_nextObjectId = sNextObjectId;
			return sNextObjectId;
		}

		public ChannelAPI.ReceivedInvite GetReceivedInvite(ChannelAPI.InvitationServiceType serviceType, ulong invitationId)
		{
			ChannelAPI.ReceivedInvite[] receivedInvites = this.GetReceivedInvites(serviceType);
			for (int i = 0; i < (int)receivedInvites.Length; i++)
			{
				ChannelAPI.ReceivedInvite receivedInvite = receivedInvites[i];
				if (receivedInvite.Invitation.Id == invitationId)
				{
					return receivedInvite;
				}
			}
			return null;
		}

		public ChannelAPI.ReceivedInvite[] GetReceivedInvites(ChannelAPI.InvitationServiceType serviceType)
		{
			List<ChannelAPI.ReceivedInvite> receivedInvites;
			this.m_receivedInvitations.TryGetValue(serviceType, out receivedInvites);
			return (receivedInvites != null ? receivedInvites.ToArray() : new ChannelAPI.ReceivedInvite[0]);
		}

		private void HandleChannelInvitation_NotifyHasRoomForInvitation(RPCContext context)
		{
			base.ApiLog.LogDebug("HandleChannelInvitation_NotifyHasRoomForInvitation");
		}

		private void HandleChannelInvitation_NotifyReceivedInvitationAdded(RPCContext context)
		{
			List<ChannelAPI.ReceivedInvite> receivedInvites;
			base.ApiLog.LogDebug("HandleChannelInvitation_NotifyReceivedInvitationAdded");
			InvitationAddedNotification invitationAddedNotification = InvitationAddedNotification.ParseFrom(context.Payload);
			if (invitationAddedNotification.Invitation.HasChannelInvitation)
			{
				ChannelInvitation channelInvitation = invitationAddedNotification.Invitation.ChannelInvitation;
				ChannelAPI.InvitationServiceType serviceType = (ChannelAPI.InvitationServiceType)channelInvitation.ServiceType;
				if (!this.m_receivedInvitations.TryGetValue(serviceType, out receivedInvites))
				{
					receivedInvites = new List<ChannelAPI.ReceivedInvite>();
					this.m_receivedInvitations[serviceType] = receivedInvites;
				}
				receivedInvites.Add(new ChannelAPI.ReceivedInvite(channelInvitation, invitationAddedNotification.Invitation));
				if (serviceType == ChannelAPI.InvitationServiceType.INVITATION_SERVICE_TYPE_PARTY)
				{
					this.m_battleNet.Party.ReceivedInvitationAdded(invitationAddedNotification, channelInvitation);
				}
			}
		}

		private void HandleChannelInvitation_NotifyReceivedInvitationRemoved(RPCContext context)
		{
			List<ChannelAPI.ReceivedInvite> receivedInvites;
			base.ApiLog.LogDebug("HandleChannelInvitation_NotifyReceivedInvitationRemoved");
			InvitationRemovedNotification invitationRemovedNotification = InvitationRemovedNotification.ParseFrom(context.Payload);
			if (invitationRemovedNotification.Invitation.HasChannelInvitation)
			{
				ChannelInvitation channelInvitation = invitationRemovedNotification.Invitation.ChannelInvitation;
				ChannelAPI.InvitationServiceType serviceType = (ChannelAPI.InvitationServiceType)channelInvitation.ServiceType;
				ulong id = invitationRemovedNotification.Invitation.Id;
				string empty = string.Empty;
				if (serviceType == ChannelAPI.InvitationServiceType.INVITATION_SERVICE_TYPE_PARTY)
				{
					empty = this.m_battleNet.Party.GetReceivedInvitationPartyType(id);
				}
				if (this.m_receivedInvitations.TryGetValue(serviceType, out receivedInvites))
				{
					int num = 0;
					while (num < receivedInvites.Count)
					{
						if (receivedInvites[num].Invitation.Id != id)
						{
							num++;
						}
						else
						{
							receivedInvites.RemoveAt(num);
							break;
						}
					}
					if (receivedInvites.Count == 0)
					{
						this.m_receivedInvitations.Remove(serviceType);
					}
				}
				if (serviceType == ChannelAPI.InvitationServiceType.INVITATION_SERVICE_TYPE_PARTY)
				{
					this.m_battleNet.Party.ReceivedInvitationRemoved(empty, invitationRemovedNotification, channelInvitation);
				}
			}
		}

		private void HandleChannelInvitation_NotifyReceivedSuggestionAdded(RPCContext context)
		{
			List<Suggestion> suggestions;
			EntityId channelId;
			SuggestionAddedNotification suggestionAddedNotification = SuggestionAddedNotification.ParseFrom(context.Payload);
			if (!suggestionAddedNotification.Suggestion.HasChannelId)
			{
				channelId = null;
			}
			else
			{
				channelId = suggestionAddedNotification.Suggestion.ChannelId;
			}
			EntityId entityId = channelId;
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.GetChannelReferenceObject(entityId);
			if (channelReferenceObject == null)
			{
				base.ApiLog.LogError(string.Concat("HandleChannelInvitation_NotifyReceivedSuggestionAdded had unexpected traffic for channelId: ", entityId));
				return;
			}
			base.ApiLog.LogDebug(string.Concat("HandleChannelInvitation_NotifyReceivedSuggestionAdded: ", suggestionAddedNotification));
			if (this.m_receivedInviteRequests == null)
			{
				this.m_receivedInviteRequests = new Map<EntityId, List<Suggestion>>();
			}
			if (!this.m_receivedInviteRequests.TryGetValue(entityId, out suggestions))
			{
				suggestions = new List<Suggestion>();
				this.m_receivedInviteRequests[entityId] = suggestions;
			}
			if (suggestions.IndexOf(suggestionAddedNotification.Suggestion) < 0)
			{
				suggestions.Add(suggestionAddedNotification.Suggestion);
			}
			if (channelReferenceObject.m_channelData.m_channelType == ChannelAPI.ChannelType.PARTY_CHANNEL)
			{
				this.m_battleNet.Party.ReceivedInviteRequestDelta(entityId, suggestionAddedNotification.Suggestion, null);
			}
		}

		private void HandleChannelSubscriber_NotifyAdd(RPCContext context)
		{
			AddNotification addNotification = AddNotification.ParseFrom(context.Payload);
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.GetChannelReferenceObject(context.Header.ObjectId);
			if (channelReferenceObject == null)
			{
				base.ApiLog.LogError(string.Concat("HandleChannelSubscriber_NotifyAdd had unexpected traffic for objectId : ", context.Header.ObjectId));
				return;
			}
			base.ApiLog.LogDebug(string.Concat("HandleChannelSubscriber_NotifyAdd: ", addNotification));
			ChannelAPI.ChannelType mChannelType = channelReferenceObject.m_channelData.m_channelType;
			switch (mChannelType)
			{
				case ChannelAPI.ChannelType.PRESENCE_CHANNEL:
				{
					if (addNotification.ChannelState.HasPresence)
					{
						bnet.protocol.presence.ChannelState presence = addNotification.ChannelState.Presence;
						this.m_battleNet.Presence.HandlePresenceUpdates(presence, channelReferenceObject);
					}
					break;
				}
				case ChannelAPI.ChannelType.CHAT_CHANNEL:
				case ChannelAPI.ChannelType.PARTY_CHANNEL:
				case ChannelAPI.ChannelType.GAME_CHANNEL:
				{
					ChannelAPI.ChannelData mChannelData = (ChannelAPI.ChannelData)channelReferenceObject.m_channelData;
					if (mChannelData != null)
					{
						mChannelData.m_channelState = addNotification.ChannelState;
						foreach (Member memberList in addNotification.MemberList)
						{
							EntityId gameAccountId = memberList.Identity.GameAccountId;
							mChannelData.m_members.Add(gameAccountId, memberList);
							if (this.m_battleNet.GameAccountId.Equals(gameAccountId))
							{
								continue;
							}
							this.m_battleNet.Presence.PresenceSubscribe(memberList.Identity.GameAccountId);
						}
					}
					break;
				}
				default:
				{
					goto case ChannelAPI.ChannelType.GAME_CHANNEL;
				}
			}
			if (mChannelType == ChannelAPI.ChannelType.PARTY_CHANNEL)
			{
				this.m_battleNet.Party.PartyJoined(channelReferenceObject, addNotification);
			}
		}

		private void HandleChannelSubscriber_NotifyJoin(RPCContext context)
		{
			JoinNotification joinNotification = JoinNotification.ParseFrom(context.Payload);
			base.ApiLog.LogDebug(string.Concat("HandleChannelSubscriber_NotifyJoin: ", joinNotification));
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.GetChannelReferenceObject(context.Header.ObjectId);
			if (channelReferenceObject == null)
			{
				base.ApiLog.LogError(string.Concat("HandleChannelSubscriber_NotifyJoin had unexpected traffic for objectId : ", context.Header.ObjectId));
				return;
			}
			ChannelAPI.ChannelType mChannelType = channelReferenceObject.m_channelData.m_channelType;
			switch (mChannelType)
			{
				case ChannelAPI.ChannelType.PRESENCE_CHANNEL:
				{
					break;
				}
				case ChannelAPI.ChannelType.CHAT_CHANNEL:
				case ChannelAPI.ChannelType.PARTY_CHANNEL:
				case ChannelAPI.ChannelType.GAME_CHANNEL:
				{
					ChannelAPI.ChannelData mChannelData = (ChannelAPI.ChannelData)channelReferenceObject.m_channelData;
					if (mChannelData != null)
					{
						EntityId gameAccountId = joinNotification.Member.Identity.GameAccountId;
						mChannelData.m_members.Add(gameAccountId, joinNotification.Member);
						if (!this.m_battleNet.GameAccountId.Equals(gameAccountId))
						{
							this.m_battleNet.Presence.PresenceSubscribe(joinNotification.Member.Identity.GameAccountId);
						}
					}
					break;
				}
				default:
				{
					goto case ChannelAPI.ChannelType.GAME_CHANNEL;
				}
			}
			if (mChannelType == ChannelAPI.ChannelType.PARTY_CHANNEL)
			{
				this.m_battleNet.Party.PartyMemberJoined(channelReferenceObject, joinNotification);
			}
		}

		private void HandleChannelSubscriber_NotifyLeave(RPCContext context)
		{
			LeaveNotification leaveNotification = LeaveNotification.ParseFrom(context.Payload);
			base.ApiLog.LogDebug(string.Concat("HandleChannelSubscriber_NotifyLeave: ", leaveNotification));
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.GetChannelReferenceObject(context.Header.ObjectId);
			if (channelReferenceObject == null)
			{
				base.ApiLog.LogError(string.Concat("HandleChannelSubscriber_NotifyLeave had unexpected traffic for objectId : ", context.Header.ObjectId));
				return;
			}
			switch (channelReferenceObject.m_channelData.m_channelType)
			{
				case ChannelAPI.ChannelType.CHAT_CHANNEL:
				case ChannelAPI.ChannelType.GAME_CHANNEL:
				{
					ChannelAPI.ChannelData mChannelData = (ChannelAPI.ChannelData)channelReferenceObject.m_channelData;
					if (mChannelData != null)
					{
						mChannelData.m_members.Remove(leaveNotification.MemberId);
						if (!this.m_battleNet.GameAccountId.Equals(leaveNotification.MemberId))
						{
							this.m_battleNet.Presence.PresenceUnsubscribe(leaveNotification.MemberId);
						}
					}
					break;
				}
				case ChannelAPI.ChannelType.PARTY_CHANNEL:
				{
					this.m_battleNet.Party.PartyMemberLeft(channelReferenceObject, leaveNotification);
					goto case ChannelAPI.ChannelType.GAME_CHANNEL;
				}
			}
		}

		private void HandleChannelSubscriber_NotifyRemove(RPCContext context)
		{
			RemoveNotification removeNotification = RemoveNotification.ParseFrom(context.Payload);
			base.ApiLog.LogDebug(string.Concat("HandleChannelSubscriber_NotifyRemove: ", removeNotification));
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.GetChannelReferenceObject(context.Header.ObjectId);
			if (channelReferenceObject == null)
			{
				base.ApiLog.LogError(string.Concat("HandleChannelSubscriber_NotifyRemove had unexpected traffic for objectId : ", context.Header.ObjectId));
				return;
			}
			switch (channelReferenceObject.m_channelData.m_channelType)
			{
				case ChannelAPI.ChannelType.CHAT_CHANNEL:
				{
					ChannelAPI.ChannelData mChannelData = (ChannelAPI.ChannelData)channelReferenceObject.m_channelData;
					if (mChannelData != null)
					{
						foreach (Member value in mChannelData.m_members.Values)
						{
							if (this.m_battleNet.GameAccountId.Equals(value.Identity.GameAccountId))
							{
								continue;
							}
							this.m_battleNet.Presence.PresenceUnsubscribe(value.Identity.GameAccountId);
						}
					}
					break;
				}
				case ChannelAPI.ChannelType.PARTY_CHANNEL:
				{
					this.m_battleNet.Party.PartyLeft(channelReferenceObject, removeNotification);
					goto case ChannelAPI.ChannelType.CHAT_CHANNEL;
				}
				case ChannelAPI.ChannelType.GAME_CHANNEL:
				{
					this.m_battleNet.Games.GameLeft(channelReferenceObject, removeNotification);
					goto case ChannelAPI.ChannelType.CHAT_CHANNEL;
				}
			}
			this.RemoveActiveChannel(context.Header.ObjectId);
		}

		private void HandleChannelSubscriber_NotifySendMessage(RPCContext context)
		{
			SendMessageNotification sendMessageNotification = SendMessageNotification.ParseFrom(context.Payload);
			base.ApiLog.LogDebug(string.Concat("HandleChannelSubscriber_NotifySendMessage: ", sendMessageNotification));
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.GetChannelReferenceObject(context.Header.ObjectId);
			if (channelReferenceObject == null)
			{
				base.ApiLog.LogError(string.Concat("HandleChannelSubscriber_NotifySendMessage had unexpected traffic for objectId : ", context.Header.ObjectId));
				return;
			}
			if (channelReferenceObject.m_channelData.m_channelType == ChannelAPI.ChannelType.PARTY_CHANNEL)
			{
				this.m_battleNet.Party.PartyMessageReceived(channelReferenceObject, sendMessageNotification);
			}
		}

		private void HandleChannelSubscriber_NotifyUpdateChannelState(RPCContext context)
		{
			UpdateChannelStateNotification updateChannelStateNotification = UpdateChannelStateNotification.ParseFrom(context.Payload);
			base.ApiLog.LogDebug(string.Concat("HandleChannelSubscriber_NotifyUpdateChannelState: ", updateChannelStateNotification));
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.GetChannelReferenceObject(context.Header.ObjectId);
			if (channelReferenceObject == null)
			{
				base.ApiLog.LogError(string.Concat("HandleChannelSubscriber_NotifyUpdateChannelState had unexpected traffic for objectId : ", context.Header.ObjectId));
				return;
			}
			ChannelAPI.ChannelType mChannelType = channelReferenceObject.m_channelData.m_channelType;
			switch (mChannelType)
			{
				case ChannelAPI.ChannelType.PRESENCE_CHANNEL:
				{
					if (updateChannelStateNotification.StateChange.HasPresence)
					{
						bnet.protocol.presence.ChannelState presence = updateChannelStateNotification.StateChange.Presence;
						this.m_battleNet.Presence.HandlePresenceUpdates(presence, channelReferenceObject);
					}
					break;
				}
				case ChannelAPI.ChannelType.CHAT_CHANNEL:
				case ChannelAPI.ChannelType.GAME_CHANNEL:
				{
					ChannelAPI.ChannelData mChannelData = (ChannelAPI.ChannelData)channelReferenceObject.m_channelData;
					if (mChannelData != null)
					{
						bool flag = mChannelType == ChannelAPI.ChannelType.PARTY_CHANNEL;
						bool flag1 = false;
						Map<string, bnet.protocol.attribute.Variant> strs = null;
						bnet.protocol.channel.ChannelState mChannelState = mChannelData.m_channelState;
						bnet.protocol.channel.ChannelState stateChange = updateChannelStateNotification.StateChange;
						if (stateChange.HasMaxMembers)
						{
							mChannelState.MaxMembers = stateChange.MaxMembers;
						}
						if (stateChange.HasMinMembers)
						{
							mChannelState.MinMembers = stateChange.MinMembers;
						}
						if (stateChange.HasMaxInvitations)
						{
							mChannelState.MaxInvitations = stateChange.MaxInvitations;
						}
						if (stateChange.HasPrivacyLevel && mChannelState.PrivacyLevel != stateChange.PrivacyLevel)
						{
							mChannelState.PrivacyLevel = stateChange.PrivacyLevel;
							flag1 = true;
						}
						if (stateChange.HasName)
						{
							mChannelState.Name = stateChange.Name;
						}
						if (stateChange.HasDelegateName)
						{
							mChannelState.DelegateName = stateChange.DelegateName;
						}
						if (stateChange.HasChannelType)
						{
							if (!flag)
							{
								mChannelState.ChannelType = stateChange.ChannelType;
							}
							if (flag && stateChange.ChannelType != PartyAPI.PARTY_TYPE_DEFAULT)
							{
								mChannelState.ChannelType = stateChange.ChannelType;
								int num = -1;
								int num1 = 0;
								while (num1 < mChannelState.AttributeList.Count)
								{
									if (mChannelState.AttributeList[num1].Name != "WTCG.Party.Type")
									{
										num1++;
									}
									else
									{
										num = num1;
										break;
									}
								}
								bnet.protocol.attribute.Attribute attribute = ProtocolHelper.CreateAttribute("WTCG.Party.Type", mChannelState.ChannelType);
								if (num < 0)
								{
									mChannelState.AttributeList.Add(attribute);
								}
								else
								{
									mChannelState.AttributeList[num] = attribute;
								}
							}
						}
						if (stateChange.HasProgram)
						{
							mChannelState.Program = stateChange.Program;
						}
						if (stateChange.HasAllowOfflineMembers)
						{
							mChannelState.AllowOfflineMembers = stateChange.AllowOfflineMembers;
						}
						if (stateChange.HasSubscribeToPresence)
						{
							mChannelState.SubscribeToPresence = stateChange.SubscribeToPresence;
						}
						if (stateChange.AttributeCount > 0 && strs == null)
						{
							strs = new Map<string, bnet.protocol.attribute.Variant>();
						}
						for (int i = 0; i < stateChange.AttributeCount; i++)
						{
							bnet.protocol.attribute.Attribute item = stateChange.AttributeList[i];
							int num2 = -1;
							int num3 = 0;
							while (num3 < mChannelState.AttributeList.Count)
							{
								if (mChannelState.AttributeList[num3].Name != item.Name)
								{
									num3++;
								}
								else
								{
									num2 = num3;
									break;
								}
							}
							if (item.Value.IsNone())
							{
								if (num2 >= 0)
								{
									mChannelState.AttributeList.RemoveAt(num2);
								}
							}
							else if (num2 < 0)
							{
								mChannelState.AddAttribute(item);
							}
							else
							{
								mChannelState.Attribute[num2] = item;
							}
							strs.Add(item.Name, item.Value);
						}
						if (!stateChange.HasReason)
						{
							mChannelState.Invitation.AddRange(stateChange.InvitationList);
						}
						else
						{
							IList<Invitation> invitationList = stateChange.InvitationList;
							IList<Invitation> invitations = mChannelState.InvitationList;
							for (int j = 0; j < invitationList.Count; j++)
							{
								Invitation invitation = invitationList[j];
								int num4 = 0;
								while (num4 < invitations.Count)
								{
									if (invitations[num4].Id != invitation.Id)
									{
										num4++;
									}
									else
									{
										mChannelState.InvitationList.RemoveAt(num4);
										break;
									}
								}
							}
						}
						mChannelData.m_channelState = mChannelState;
						if (flag)
						{
							if (flag1)
							{
								this.m_battleNet.Party.PartyPrivacyChanged(mChannelData.m_channelId, mChannelState.PrivacyLevel);
							}
							if (stateChange.InvitationList.Count > 0)
							{
								uint? nullable = null;
								if (stateChange.HasReason)
								{
									nullable = new uint?(stateChange.Reason);
								}
								foreach (Invitation invitationList1 in stateChange.InvitationList)
								{
									this.m_battleNet.Party.PartyInvitationDelta(mChannelData.m_channelId, invitationList1, nullable);
								}
							}
							if (strs != null)
							{
								foreach (KeyValuePair<string, bnet.protocol.attribute.Variant> str in strs)
								{
									this.m_battleNet.Party.PartyAttributeChanged(mChannelData.m_channelId, str.Key, str.Value);
								}
							}
						}
					}
					break;
				}
				case ChannelAPI.ChannelType.PARTY_CHANNEL:
				{
					this.m_battleNet.Party.PreprocessPartyChannelUpdated(channelReferenceObject, updateChannelStateNotification);
					goto case ChannelAPI.ChannelType.GAME_CHANNEL;
				}
			}
		}

		private void HandleChannelSubscriber_NotifyUpdateMemberState(RPCContext context)
		{
			Member member;
			UpdateMemberStateNotification updateMemberStateNotification = UpdateMemberStateNotification.ParseFrom(context.Payload);
			base.ApiLog.LogDebug(string.Concat("HandleChannelSubscriber_NotifyUpdateMemberState: ", updateMemberStateNotification));
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.GetChannelReferenceObject(context.Header.ObjectId);
			if (channelReferenceObject == null)
			{
				base.ApiLog.LogError(string.Concat("HandleChannelSubscriber_NotifyUpdateMemberState had unexpected traffic for objectId : ", context.Header.ObjectId));
				return;
			}
			ChannelAPI.ChannelType mChannelType = channelReferenceObject.m_channelData.m_channelType;
			ChannelAPI.ChannelData mChannelData = (ChannelAPI.ChannelData)channelReferenceObject.m_channelData;
			EntityId mChannelId = mChannelData.m_channelId;
			List<EntityId> entityIds = null;
			for (int i = 0; i < updateMemberStateNotification.StateChangeList.Count; i++)
			{
				Member item = updateMemberStateNotification.StateChangeList[i];
				if (item.Identity.HasGameAccountId)
				{
					EntityId gameAccountId = item.Identity.GameAccountId;
					Map<string, bnet.protocol.attribute.Variant> strs = null;
					Member member1 = null;
					if (mChannelData.m_members.TryGetValue(gameAccountId, out member))
					{
						Member member2 = member;
						MemberState state = member.State;
						if (item.State.AttributeCount <= 0)
						{
							if (item.State.HasPrivileges)
							{
								state.Privileges = item.State.Privileges;
							}
							if (member.State.RoleCount != item.State.RoleCount || !member.State.RoleList.All<uint>((uint roleId) => item.State.RoleList.Contains(roleId)) || !item.State.RoleList.All<uint>((uint roleId) => member.State.RoleList.Contains(roleId)))
							{
								if (entityIds == null)
								{
									entityIds = new List<EntityId>();
								}
								entityIds.Add(gameAccountId);
								state.ClearRole();
								state.Role.AddRange(item.State.RoleList);
							}
							if (item.State.HasInfo)
							{
								if (!state.HasInfo)
								{
									state.SetInfo(item.State.Info);
								}
								else if (item.State.Info.HasBattleTag)
								{
									state.Info.SetBattleTag(item.State.Info.BattleTag);
								}
							}
						}
						else
						{
							if (strs == null)
							{
								strs = new Map<string, bnet.protocol.attribute.Variant>();
							}
							for (int j = 0; j < item.State.AttributeCount; j++)
							{
								bnet.protocol.attribute.Attribute attribute = item.State.AttributeList[j];
								int num = -1;
								int num1 = 0;
								while (num1 < state.AttributeList.Count)
								{
									if (state.AttributeList[num1].Name != attribute.Name)
									{
										num1++;
									}
									else
									{
										num = num1;
										break;
									}
								}
								if (attribute.Value.IsNone())
								{
									if (num >= 0)
									{
										state.AttributeList.RemoveAt(num);
									}
								}
								else if (num < 0)
								{
									state.AddAttribute(attribute);
								}
								else
								{
									state.Attribute[num] = attribute;
								}
								strs.Add(attribute.Name, attribute.Value);
							}
						}
						member2.SetState(state);
						member1 = member2;
					}
					else
					{
						member1 = item;
					}
					if (member1 != null)
					{
						mChannelData.m_members[gameAccountId] = member1;
					}
					strs == null;
				}
				else
				{
					base.ApiLog.LogError("HandleChannelSubscriber_NotifyUpdateMemberState no identity/gameAccount in Member list at index={0} channelId={1}-{2}", new object[] { i, mChannelId.High, mChannelId.Low });
				}
			}
			if (entityIds != null && mChannelType == ChannelAPI.ChannelType.PARTY_CHANNEL)
			{
				this.m_battleNet.Party.MemberRolesChanged(channelReferenceObject, entityIds);
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			this.SubscribeToInvitationService();
		}

		public override void InitRPCListeners(RPCConnection rpcConnection)
		{
			base.InitRPCListeners(rpcConnection);
			this.m_rpcConnection.RegisterServiceMethodListener(this.m_channelSubscriberService.Id, 1, new RPCContextDelegate(this.HandleChannelSubscriber_NotifyAdd));
			this.m_rpcConnection.RegisterServiceMethodListener(this.m_channelSubscriberService.Id, 2, new RPCContextDelegate(this.HandleChannelSubscriber_NotifyJoin));
			this.m_rpcConnection.RegisterServiceMethodListener(this.m_channelSubscriberService.Id, 3, new RPCContextDelegate(this.HandleChannelSubscriber_NotifyRemove));
			this.m_rpcConnection.RegisterServiceMethodListener(this.m_channelSubscriberService.Id, 4, new RPCContextDelegate(this.HandleChannelSubscriber_NotifyLeave));
			this.m_rpcConnection.RegisterServiceMethodListener(this.m_channelSubscriberService.Id, 5, new RPCContextDelegate(this.HandleChannelSubscriber_NotifySendMessage));
			this.m_rpcConnection.RegisterServiceMethodListener(this.m_channelSubscriberService.Id, 6, new RPCContextDelegate(this.HandleChannelSubscriber_NotifyUpdateChannelState));
			this.m_rpcConnection.RegisterServiceMethodListener(this.m_channelSubscriberService.Id, 7, new RPCContextDelegate(this.HandleChannelSubscriber_NotifyUpdateMemberState));
			this.m_rpcConnection.RegisterServiceMethodListener(ChannelAPI.m_channelInvitationNotifyService.Id, 1, new RPCContextDelegate(this.HandleChannelInvitation_NotifyReceivedInvitationAdded));
			this.m_rpcConnection.RegisterServiceMethodListener(ChannelAPI.m_channelInvitationNotifyService.Id, 2, new RPCContextDelegate(this.HandleChannelInvitation_NotifyReceivedInvitationRemoved));
			this.m_rpcConnection.RegisterServiceMethodListener(ChannelAPI.m_channelInvitationNotifyService.Id, 3, new RPCContextDelegate(this.HandleChannelInvitation_NotifyReceivedSuggestionAdded));
			this.m_rpcConnection.RegisterServiceMethodListener(ChannelAPI.m_channelInvitationNotifyService.Id, 4, new RPCContextDelegate(this.HandleChannelInvitation_NotifyHasRoomForInvitation));
		}

		public void JoinChannel(EntityId channelId, ChannelAPI.ChannelType channelType)
		{
			JoinChannelRequest joinChannelRequest = new JoinChannelRequest();
			joinChannelRequest.SetChannelId(channelId);
			joinChannelRequest.SetObjectId(ChannelAPI.GetNextObjectId());
			ChannelAPI.ChannelData channelDatum = new ChannelAPI.ChannelData(this, channelId, (ulong)0, channelType);
			channelDatum.SetSubscriberObjectId(joinChannelRequest.ObjectId);
			this.m_rpcConnection.QueueRequest(this.m_channelOwnerService.Id, 3, joinChannelRequest, new RPCContextDelegate(channelDatum.JoinChannelCallback), (uint)channelType);
		}

		public override void OnDisconnected()
		{
			base.OnDisconnected();
			this.m_activeChannels.Clear();
			this.m_channelEntityObjectMap.Clear();
		}

		public void RemoveActiveChannel(ulong objectId)
		{
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.GetChannelReferenceObject(objectId);
			if (channelReferenceObject != null)
			{
				this.m_channelEntityObjectMap.Remove(channelReferenceObject.m_channelData.m_channelId);
				this.m_activeChannels.Remove(objectId);
			}
		}

		public void RemoveInviteRequestsFor(EntityId channelId, EntityId suggesteeId, uint removeReason)
		{
			List<Suggestion> suggestions;
			if (this.m_receivedInviteRequests == null || suggesteeId == null)
			{
				return;
			}
			ChannelAPI.ChannelReferenceObject channelReferenceObject = this.GetChannelReferenceObject(channelId);
			bool flag = (channelReferenceObject == null ? false : channelReferenceObject.m_channelData.m_channelType == ChannelAPI.ChannelType.PARTY_CHANNEL);
			if (this.m_receivedInviteRequests.TryGetValue(channelId, out suggestions))
			{
				for (int i = 0; i < suggestions.Count; i++)
				{
					Suggestion item = suggestions[i];
					if (suggesteeId.Equals(item.SuggesterId))
					{
						suggestions.RemoveAt(i);
						i--;
						if (flag)
						{
							this.m_battleNet.Party.ReceivedInviteRequestDelta(channelId, item, new uint?(removeReason));
						}
					}
				}
				if (suggestions.Count == 0)
				{
					this.m_receivedInviteRequests.Remove(channelId);
					if (this.m_receivedInviteRequests.Count == 0)
					{
						this.m_receivedInviteRequests = null;
					}
				}
			}
		}

		public void RevokeInvitation(ulong invitationId, EntityId channelId, RPCContextDelegate callback)
		{
			RevokeInvitationRequest revokeInvitationRequest = new RevokeInvitationRequest();
			revokeInvitationRequest.SetInvitationId(invitationId);
			revokeInvitationRequest.SetChannelId(channelId);
			this.m_rpcConnection.QueueRequest(ChannelAPI.m_channelInvitationService.Id, 6, revokeInvitationRequest, callback, 0);
		}

		public void SendInvitation(EntityId channelId, EntityId entityId, ChannelAPI.InvitationServiceType serviceType, RPCContextDelegate callback)
		{
			SendInvitationRequest sendInvitationRequest = new SendInvitationRequest();
			sendInvitationRequest.SetTargetId(entityId);
			InvitationParams invitationParam = new InvitationParams();
			ChannelInvitationParams channelInvitationParam = new ChannelInvitationParams();
			channelInvitationParam.SetChannelId(channelId);
			channelInvitationParam.SetServiceType((uint)serviceType);
			invitationParam.SetChannelParams(channelInvitationParam);
			sendInvitationRequest.SetParams(invitationParam);
			this.m_rpcConnection.QueueRequest(ChannelAPI.m_channelInvitationService.Id, 3, sendInvitationRequest, callback, 0);
		}

		private void SubscribeToInvitationService()
		{
			bnet.protocol.channel_invitation.SubscribeRequest subscribeRequest = new bnet.protocol.channel_invitation.SubscribeRequest();
			subscribeRequest.SetObjectId((ulong)0);
			this.m_rpcConnection.QueueRequest(ChannelAPI.m_channelInvitationService.Id, 1, subscribeRequest, new RPCContextDelegate(this.SubscribeToInvitationServiceCallback), 0);
		}

		private void SubscribeToInvitationServiceCallback(RPCContext context)
		{
			base.CheckRPCCallback("SubscribeToInvitationServiceCallback", context);
		}

		public void SuggestInvitation(EntityId partyId, EntityId whomToAskForApproval, EntityId whomToInvite, RPCContextDelegate callback)
		{
			SuggestInvitationRequest suggestInvitationRequest = new SuggestInvitationRequest();
			suggestInvitationRequest.SetChannelId(partyId);
			suggestInvitationRequest.SetApprovalId(whomToAskForApproval);
			suggestInvitationRequest.SetTargetId(whomToInvite);
			this.m_rpcConnection.QueueRequest(ChannelAPI.m_channelInvitationService.Id, 7, suggestInvitationRequest, callback, 0);
		}

		public void UpdateChannelAttributes(ChannelAPI.ChannelData channelData, List<bnet.protocol.attribute.Attribute> attributeList, RPCContextDelegate callback)
		{
			UpdateChannelStateRequest updateChannelStateRequest = new UpdateChannelStateRequest();
			bnet.protocol.channel.ChannelState channelState = new bnet.protocol.channel.ChannelState();
			foreach (bnet.protocol.attribute.Attribute attribute in attributeList)
			{
				channelState.AddAttribute(attribute);
			}
			updateChannelStateRequest.SetStateChange(channelState);
			this.m_rpcConnection.QueueRequest(this.m_channelService.Id, 4, updateChannelStateRequest, callback, (uint)channelData.m_objectId);
		}

		public class BaseChannelData
		{
			public EntityId m_channelId;

			public ChannelAPI.ChannelType m_channelType;

			public ulong m_objectId;

			public ulong m_subscriberObjectId;

			public BaseChannelData(EntityId entityId, ulong objectId, ChannelAPI.ChannelType channelType)
			{
				this.m_channelId = entityId;
				this.m_channelType = channelType;
				this.m_objectId = objectId;
			}

			public void SetChannelId(EntityId channelId)
			{
				this.m_channelId = channelId;
			}

			public void SetObjectId(ulong objectId)
			{
				this.m_objectId = objectId;
			}

			public void SetSubscriberObjectId(ulong objectId)
			{
				this.m_subscriberObjectId = objectId;
			}
		}

		public class ChannelData : ChannelAPI.BaseChannelData
		{
			public bnet.protocol.channel.ChannelState m_channelState;

			public Map<EntityId, Member> m_members;

			private ChannelAPI m_channelAPI;

			public ChannelData(ChannelAPI channelAPI, EntityId entityId, ulong objectId, ChannelAPI.ChannelType channelType) : base(entityId, objectId, channelType)
			{
				this.m_channelState = new bnet.protocol.channel.ChannelState();
				this.m_members = new Map<EntityId, Member>();
				this.m_channelAPI = channelAPI;
			}

			public void AcceptInvitationCallback(RPCContext context, RPCContextDelegate callback)
			{
				BattleNetErrors status = (BattleNetErrors)context.Header.Status;
				if (status != BattleNetErrors.ERROR_OK)
				{
					this.m_channelAPI.ApiLog.LogError(string.Concat("AcceptInvitationCallback: ", status.ToString()));
					return;
				}
				base.SetObjectId(AcceptInvitationResponse.ParseFrom(context.Payload).ObjectId);
				this.m_channelAPI.AddActiveChannel(this.m_subscriberObjectId, new ChannelAPI.ChannelReferenceObject(this));
				this.m_channelAPI.ApiLog.LogDebug(string.Concat("AcceptInvitationCallback, status=", status.ToString()));
				if (callback != null)
				{
					callback(context);
				}
			}

			public void JoinChannelCallback(RPCContext context)
			{
				BattleNetErrors status = (BattleNetErrors)context.Header.Status;
				if (status != BattleNetErrors.ERROR_OK)
				{
					this.m_channelAPI.ApiLog.LogError(string.Concat("JoinChannelCallback: ", status.ToString()));
					return;
				}
				base.SetObjectId(JoinChannelResponse.ParseFrom(context.Payload).ObjectId);
				this.m_channelAPI.AddActiveChannel(this.m_subscriberObjectId, new ChannelAPI.ChannelReferenceObject(this));
				this.m_channelAPI.ApiLog.LogDebug(string.Concat("JoinChannelCallback, status=", status.ToString()));
			}
		}

		public class ChannelReferenceObject
		{
			public ChannelAPI.BaseChannelData m_channelData;

			public ChannelReferenceObject(EntityId entityId, ChannelAPI.ChannelType channelType)
			{
				this.m_channelData = new ChannelAPI.BaseChannelData(entityId, (ulong)0, channelType);
			}

			public ChannelReferenceObject(ChannelAPI.BaseChannelData channelData)
			{
				this.m_channelData = channelData;
			}
		}

		public enum ChannelType
		{
			PRESENCE_CHANNEL,
			CHAT_CHANNEL,
			PARTY_CHANNEL,
			GAME_CHANNEL
		}

		public enum InvitationServiceType
		{
			INVITATION_SERVICE_TYPE_NONE,
			INVITATION_SERVICE_TYPE_PARTY,
			INVITATION_SERVICE_TYPE_CHAT,
			INVITATION_SERVICE_TYPE_GAMES
		}

		public class ReceivedInvite
		{
			public ChannelInvitation ChannelInvitation;

			public Invitation Invitation;

			public IList<bnet.protocol.attribute.Attribute> Attributes
			{
				get
				{
					return this.State.AttributeList;
				}
			}

			public EntityId ChannelId
			{
				get
				{
					return this.ChannelInvitation.ChannelDescription.ChannelId;
				}
			}

			public string ChannelType
			{
				get
				{
					return this.State.ChannelType;
				}
			}

			public bnet.protocol.channel.ChannelState State
			{
				get
				{
					return this.ChannelInvitation.ChannelDescription.State;
				}
			}

			public ReceivedInvite(ChannelInvitation c, Invitation i)
			{
				this.ChannelInvitation = c;
				this.Invitation = i;
			}
		}
	}
}