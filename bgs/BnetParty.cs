using bgs.types;
using bnet.protocol.attribute;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace bgs
{
	public static class BnetParty
	{
		public const string ATTRIBUTE_PARTY_TYPE = "WTCG.Party.Type";

		public const string ATTRIBUTE_PARTY_CREATOR = "WTCG.Party.Creator";

		public const string ATTRIBUTE_SCENARIO_ID = "WTCG.Game.ScenarioId";

		public const string ATTRIBUTE_FRIENDLY_DECLINE_REASON = "WTCG.Friendly.DeclineReason";

		public const string ATTRIBUTE_PARTY_SERVER_INFO = "WTCG.Party.ServerInfo";

		private static Map<BnetFeatureEvent, HashSet<BattleNetErrors>> s_ignorableErrorCodes;

		private static Map<PartyId, PartyType> s_joinedParties;

		private static Map<PartyType, BnetParty.CreateSuccessCallback> s_pendingPartyCreates;

		private static Map<string, List<BnetParty.PartyAttributeChangedHandler>> s_attributeChangedSubscribers;

		static BnetParty()
		{
			Map<BnetFeatureEvent, HashSet<BattleNetErrors>> bnetFeatureEvents = new Map<BnetFeatureEvent, HashSet<BattleNetErrors>>();
			HashSet<BattleNetErrors> battleNetErrors = new HashSet<BattleNetErrors>();
			battleNetErrors.Add(BattleNetErrors.ERROR_CHANNEL_NO_SUCH_MEMBER);
			bnetFeatureEvents.Add(BnetFeatureEvent.Party_KickMember_Callback, battleNetErrors);
			battleNetErrors = new HashSet<BattleNetErrors>();
			battleNetErrors.Add(BattleNetErrors.ERROR_CHANNEL_NOT_MEMBER);
			battleNetErrors.Add(BattleNetErrors.ERROR_CHANNEL_NO_CHANNEL);
			bnetFeatureEvents.Add(BnetFeatureEvent.Party_Leave_Callback, battleNetErrors);
			BnetParty.s_ignorableErrorCodes = bnetFeatureEvents;
			BnetParty.s_joinedParties = new Map<PartyId, PartyType>();
			BnetParty.s_pendingPartyCreates = null;
			BnetParty.s_attributeChangedSubscribers = null;
		}

		public static void AcceptInviteRequest(PartyId partyId, BnetGameAccountId requestedTargetId)
		{
			BnetParty.SendInvite(partyId, requestedTargetId);
		}

		public static void AcceptReceivedInvite(ulong inviteId)
		{
			BattleNet.AcceptPartyInvite(inviteId);
		}

		public static void ClearPartyAttribute(PartyId partyId, string attributeKey)
		{
			BattleNet.ClearPartyAttribute(partyId.ToEntityId(), attributeKey);
		}

		public static int CountMembers(PartyId partyId)
		{
			if (partyId == null)
			{
				return 0;
			}
			return BattleNet.GetCountPartyMembers(partyId.ToEntityId());
		}

		public static void CreateParty(PartyType partyType, PrivacyLevel privacyLevel, byte[] creatorBlob, BnetParty.CreateSuccessCallback successCallback)
		{
			string str = EnumUtils.GetString<PartyType>(partyType);
			if (BnetParty.s_pendingPartyCreates != null && BnetParty.s_pendingPartyCreates.ContainsKey(partyType))
			{
				BnetParty.RaisePartyError(true, str, BnetFeatureEvent.Party_Create_Callback, "CreateParty: Already creating party of type {0}", new object[] { partyType });
				return;
			}
			if (BnetParty.s_pendingPartyCreates == null)
			{
				BnetParty.s_pendingPartyCreates = new Map<PartyType, BnetParty.CreateSuccessCallback>();
			}
			BnetParty.s_pendingPartyCreates[partyType] = successCallback;
			BattleNet.CreateParty(str, (int)privacyLevel, creatorBlob);
		}

		public static void DeclineReceivedInvite(ulong inviteId)
		{
			BattleNet.DeclinePartyInvite(inviteId);
		}

		public static void DissolveParty(PartyId partyId)
		{
			if (!BnetParty.IsInParty(partyId))
			{
				return;
			}
			BattleNet.DissolveParty(partyId.ToEntityId());
		}

		public static KeyValuePair<string, object>[] GetAllPartyAttributes(PartyId partyId)
		{
			string[] strArrays;
			if (partyId == null)
			{
				return new KeyValuePair<string, object>[0];
			}
			BattleNet.GetAllPartyAttributes(partyId.ToEntityId(), out strArrays);
			KeyValuePair<string, object>[] keyValuePair = new KeyValuePair<string, object>[(int)strArrays.Length];
			for (int i = 0; i < (int)keyValuePair.Length; i++)
			{
				string str = strArrays[i];
				object obj = null;
				long? partyAttributeLong = BnetParty.GetPartyAttributeLong(partyId, str);
				if (partyAttributeLong.HasValue)
				{
					obj = partyAttributeLong;
				}
				string partyAttributeString = BnetParty.GetPartyAttributeString(partyId, str);
				if (partyAttributeString != null)
				{
					obj = partyAttributeString;
				}
				byte[] partyAttributeBlob = BnetParty.GetPartyAttributeBlob(partyId, str);
				if (partyAttributeBlob != null)
				{
					obj = partyAttributeBlob;
				}
				keyValuePair[i] = new KeyValuePair<string, object>(str, obj);
			}
			return keyValuePair;
		}

		public static InviteRequest[] GetInviteRequests(PartyId partyId)
		{
			InviteRequest[] inviteRequestArray;
			if (partyId == null)
			{
				return new InviteRequest[0];
			}
			BattleNet.GetPartyInviteRequests(partyId.ToEntityId(), out inviteRequestArray);
			return inviteRequestArray;
		}

		public static bgs.PartyInfo[] GetJoinedParties()
		{
			return (
				from kv in BnetParty.s_joinedParties
				select new bgs.PartyInfo(kv.Key, kv.Value)).ToArray<bgs.PartyInfo>();
		}

		public static bgs.PartyInfo GetJoinedParty(PartyId partyId)
		{
			if (partyId == null)
			{
				return null;
			}
			PartyType partyType = PartyType.DEFAULT;
			if (!BnetParty.s_joinedParties.TryGetValue(partyId, out partyType))
			{
				return null;
			}
			return new bgs.PartyInfo(partyId, partyType);
		}

		public static PartyId[] GetJoinedPartyIds()
		{
			return BnetParty.s_joinedParties.Keys.ToArray<PartyId>();
		}

		public static bgs.PartyMember GetLeader(PartyId partyId)
		{
			if (partyId == null)
			{
				return null;
			}
			bgs.PartyMember[] members = BnetParty.GetMembers(partyId);
			PartyType partyType = BnetParty.GetPartyType(partyId);
			for (int i = 0; i < (int)members.Length; i++)
			{
				bgs.PartyMember partyMember = members[i];
				if (partyMember.IsLeader(partyType))
				{
					return partyMember;
				}
			}
			return null;
		}

		public static bgs.PartyMember GetMember(PartyId partyId, BnetGameAccountId memberId)
		{
			bgs.PartyMember[] members = BnetParty.GetMembers(partyId);
			for (int i = 0; i < (int)members.Length; i++)
			{
				bgs.PartyMember partyMember = members[i];
				if (partyMember.GameAccountId == memberId)
				{
					return partyMember;
				}
			}
			return null;
		}

		public static bgs.PartyMember[] GetMembers(PartyId partyId)
		{
			bgs.types.PartyMember[] partyMemberArray;
			if (partyId == null)
			{
				return new bgs.PartyMember[0];
			}
			BattleNet.GetPartyMembers(partyId.ToEntityId(), out partyMemberArray);
			bgs.PartyMember[] partyMemberArray1 = new bgs.PartyMember[(int)partyMemberArray.Length];
			for (int i = 0; i < (int)partyMemberArray1.Length; i++)
			{
				bgs.types.PartyMember partyMember = partyMemberArray[i];
				bgs.PartyMember partyMember1 = new bgs.PartyMember()
				{
					GameAccountId = BnetGameAccountId.CreateFromEntityId(partyMember.memberGameAccountId),
					RoleIds = new uint[] { partyMember.firstMemberRole }
				};
				partyMemberArray1[i] = partyMember1;
			}
			return partyMemberArray1;
		}

		public static bgs.PartyMember GetMyselfMember(PartyId partyId)
		{
			if (partyId == null)
			{
				return null;
			}
			BnetGameAccountId bnetGameAccountId = BnetGameAccountId.CreateFromEntityId(BattleNet.GetMyGameAccountId());
			if (bnetGameAccountId == null)
			{
				return null;
			}
			return BnetParty.GetMember(partyId, bnetGameAccountId);
		}

		public static byte[] GetPartyAttributeBlob(PartyId partyId, string attributeKey)
		{
			byte[] numArray;
			if (partyId == null)
			{
				return null;
			}
			BattleNet.GetPartyAttributeBlob(partyId.ToEntityId(), attributeKey, out numArray);
			return numArray;
		}

		public static long? GetPartyAttributeLong(PartyId partyId, string attributeKey)
		{
			long num;
			if (partyId == null)
			{
				return null;
			}
			if (BattleNet.GetPartyAttributeLong(partyId.ToEntityId(), attributeKey, out num))
			{
				return new long?(num);
			}
			return null;
		}

		public static string GetPartyAttributeString(PartyId partyId, string attributeKey)
		{
			string str;
			if (partyId == null)
			{
				return null;
			}
			BattleNet.GetPartyAttributeString(partyId.ToEntityId(), attributeKey, out str);
			return str;
		}

		public static bnet.protocol.attribute.Variant GetPartyAttributeVariant(PartyId partyId, string attributeKey)
		{
			long num;
			string str;
			byte[] numArray;
			bnet.protocol.attribute.Variant variant = new bnet.protocol.attribute.Variant();
			EntityId entityId = partyId.ToEntityId();
			if (!BattleNet.GetPartyAttributeLong(entityId, attributeKey, out num))
			{
				BattleNet.GetPartyAttributeString(entityId, attributeKey, out str);
				if (str == null)
				{
					BattleNet.GetPartyAttributeBlob(entityId, attributeKey, out numArray);
					if (numArray != null)
					{
						variant.BlobValue = numArray;
					}
				}
				else
				{
					variant.StringValue = str;
				}
			}
			else
			{
				variant.IntValue = num;
			}
			return variant;
		}

		public static PartyType GetPartyType(PartyId partyId)
		{
			PartyType partyType = PartyType.DEFAULT;
			if (partyId == null)
			{
				return partyType;
			}
			BnetParty.s_joinedParties.TryGetValue(partyId, out partyType);
			return partyType;
		}

		public static PartyType GetPartyTypeFromString(string partyType)
		{
			PartyType partyType1 = PartyType.DEFAULT;
			if (partyType != null)
			{
				EnumUtils.TryGetEnum<PartyType>(partyType, out partyType1);
			}
			return partyType1;
		}

		public static PrivacyLevel GetPrivacyLevel(PartyId partyId)
		{
			if (partyId == null)
			{
				return PrivacyLevel.CLOSED;
			}
			return (PrivacyLevel)BattleNet.GetPartyPrivacy(partyId.ToEntityId());
		}

		public static PartyInvite GetReceivedInvite(ulong inviteId)
		{
			PartyInvite[] receivedInvites = BnetParty.GetReceivedInvites();
			return receivedInvites.FirstOrDefault<PartyInvite>((PartyInvite i) => i.InviteId == inviteId);
		}

		public static PartyInvite GetReceivedInviteFrom(BnetGameAccountId inviterId, PartyType partyType)
		{
			PartyInvite[] receivedInvites = BnetParty.GetReceivedInvites();
			return receivedInvites.FirstOrDefault<PartyInvite>((PartyInvite i) => (i.InviterId != inviterId ? false : i.PartyType == partyType));
		}

		public static PartyInvite[] GetReceivedInvites()
		{
			PartyInvite[] partyInviteArray;
			BattleNet.GetReceivedPartyInvites(out partyInviteArray);
			return partyInviteArray;
		}

		public static PartyInvite GetSentInvite(PartyId partyId, ulong inviteId)
		{
			if (partyId == null)
			{
				return null;
			}
			PartyInvite[] sentInvites = BnetParty.GetSentInvites(partyId);
			return sentInvites.FirstOrDefault<PartyInvite>((PartyInvite i) => i.InviteId == inviteId);
		}

		public static PartyInvite[] GetSentInvites(PartyId partyId)
		{
			PartyInvite[] partyInviteArray;
			if (partyId == null)
			{
				return new PartyInvite[0];
			}
			BattleNet.GetPartySentInvites(partyId.ToEntityId(), out partyInviteArray);
			return partyInviteArray;
		}

		public static void IgnoreInviteRequest(PartyId partyId, BnetGameAccountId requestedTargetId)
		{
			BattleNet.IgnoreInviteRequest(partyId.ToEntityId(), BnetEntityId.CreateEntityId(requestedTargetId));
		}

		private static bool IsIgnorableError(BnetFeatureEvent feature, BattleNetErrors code)
		{
			HashSet<BattleNetErrors> battleNetErrors;
			if (!BnetParty.s_ignorableErrorCodes.TryGetValue(feature, out battleNetErrors))
			{
				return false;
			}
			return battleNetErrors.Contains(code);
		}

		public static bool IsInParty(PartyId partyId)
		{
			if (partyId == null)
			{
				return false;
			}
			return BnetParty.s_joinedParties.ContainsKey(partyId);
		}

		public static bool IsLeader(PartyId partyId)
		{
			if (partyId == null)
			{
				return false;
			}
			bgs.PartyMember myselfMember = BnetParty.GetMyselfMember(partyId);
			if (myselfMember != null && myselfMember.IsLeader(BnetParty.GetPartyType(partyId)))
			{
				return true;
			}
			return false;
		}

		public static bool IsMember(PartyId partyId, BnetGameAccountId memberId)
		{
			if (partyId == null)
			{
				return false;
			}
			return BnetParty.GetMember(partyId, memberId) != null;
		}

		public static bool IsPartyFull(PartyId partyId, bool includeInvites = true)
		{
			if (partyId == null)
			{
				return false;
			}
			int num = BnetParty.CountMembers(partyId);
			int num1 = (!includeInvites ? 0 : (int)BnetParty.GetSentInvites(partyId).Length);
			int maxPartyMembers = BattleNet.GetMaxPartyMembers(partyId.ToEntityId());
			return num + num1 >= maxPartyMembers;
		}

		public static void JoinParty(PartyId partyId, PartyType partyType)
		{
			BattleNet.JoinParty(partyId.ToEntityId(), EnumUtils.GetString<PartyType>(partyType));
		}

		public static void KickMember(PartyId partyId, BnetGameAccountId memberId)
		{
			if (!BnetParty.IsInParty(partyId))
			{
				return;
			}
			BattleNet.KickPartyMember(partyId.ToEntityId(), BnetEntityId.CreateEntityId(memberId));
		}

		public static void Leave(PartyId partyId)
		{
			if (!BnetParty.IsInParty(partyId))
			{
				return;
			}
			BattleNet.LeaveParty(partyId.ToEntityId());
		}

		public static void Process()
		{
			PartyListenerEvent[] partyListenerEventArray;
			List<BnetParty.PartyAttributeChangedHandler> partyAttributeChangedHandlers;
			BattleNet.GetPartyListenerEvents(out partyListenerEventArray);
			BattleNet.ClearPartyListenerEvents();
			for (int i = 0; i < (int)partyListenerEventArray.Length; i++)
			{
				PartyListenerEvent partyListenerEvent = partyListenerEventArray[i];
				PartyId partyId = partyListenerEvent.PartyId;
				switch (partyListenerEvent.Type)
				{
					case PartyListenerEventType.ERROR_RAISED:
					case PartyListenerEventType.OPERATION_CALLBACK:
					{
						PartyError partyError = partyListenerEvent.ToPartyError();
						if (partyError.ErrorCode != 0)
						{
							if (BnetParty.IsIgnorableError(partyError.FeatureEvent, partyError.ErrorCode.EnumVal))
							{
								partyError.ErrorCode = 0;
								if (partyError.FeatureEvent == BnetFeatureEvent.Party_Leave_Callback)
								{
									if (!BnetParty.s_joinedParties.ContainsKey(partyId))
									{
										BnetParty.s_joinedParties[partyId] = PartyType.SPECTATOR_PARTY;
									}
									goto case PartyListenerEventType.LEFT_PARTY;
								}
							}
							if (partyError.IsOperationCallback && partyError.FeatureEvent == BnetFeatureEvent.Party_Create_Callback)
							{
								PartyType partyType = partyError.PartyType;
								if (BnetParty.s_pendingPartyCreates.ContainsKey(partyType))
								{
									BnetParty.s_pendingPartyCreates.Remove(partyType);
								}
							}
						}
						if (partyError.ErrorCode != 0)
						{
							BnetParty.RaisePartyError(partyError);
						}
						break;
					}
					case PartyListenerEventType.JOINED_PARTY:
					{
						string stringData = partyListenerEvent.StringData;
						PartyType partyTypeFromString = BnetParty.GetPartyTypeFromString(stringData);
						BnetParty.s_joinedParties[partyId] = partyTypeFromString;
						if (BnetParty.s_pendingPartyCreates != null)
						{
							BnetParty.CreateSuccessCallback item = null;
							if (BnetParty.s_pendingPartyCreates.ContainsKey(partyTypeFromString))
							{
								item = BnetParty.s_pendingPartyCreates[partyTypeFromString];
								BnetParty.s_pendingPartyCreates.Remove(partyTypeFromString);
							}
							else if (stringData == "default" && BnetParty.s_pendingPartyCreates.Count == 0)
							{
								item = BnetParty.s_pendingPartyCreates.First<KeyValuePair<PartyType, BnetParty.CreateSuccessCallback>>().Value;
								BnetParty.s_pendingPartyCreates.Clear();
							}
							if (item != null)
							{
								item(partyTypeFromString, partyId);
							}
						}
						if (BnetParty.OnJoined != null)
						{
							BnetParty.OnJoined(0, new bgs.PartyInfo(partyId, partyTypeFromString), null);
						}
						break;
					}
					case PartyListenerEventType.LEFT_PARTY:
					{
						if (BnetParty.s_joinedParties.ContainsKey(partyId))
						{
							PartyType item1 = BnetParty.s_joinedParties[partyId];
							BnetParty.s_joinedParties.Remove(partyId);
							if (BnetParty.OnJoined != null)
							{
								BnetParty.OnJoined(1, new bgs.PartyInfo(partyId, item1), new LeaveReason?((LeaveReason)partyListenerEvent.UintData));
							}
						}
						break;
					}
					case PartyListenerEventType.PRIVACY_CHANGED:
					{
						if (BnetParty.OnPrivacyLevelChanged != null)
						{
							BnetParty.OnPrivacyLevelChanged(BnetParty.GetJoinedParty(partyId), partyListenerEvent.UintData);
						}
						break;
					}
					case PartyListenerEventType.MEMBER_JOINED:
					case PartyListenerEventType.MEMBER_LEFT:
					{
						if (BnetParty.OnMemberEvent != null)
						{
							OnlineEventType onlineEventType = (partyListenerEvent.Type != PartyListenerEventType.MEMBER_JOINED ? OnlineEventType.REMOVED : OnlineEventType.ADDED);
							LeaveReason? nullable = null;
							if (partyListenerEvent.Type == PartyListenerEventType.MEMBER_LEFT)
							{
								nullable = new LeaveReason?((LeaveReason)partyListenerEvent.UintData);
							}
							BnetParty.OnMemberEvent(onlineEventType, BnetParty.GetJoinedParty(partyId), partyListenerEvent.SubjectMemberId, false, nullable);
						}
						break;
					}
					case PartyListenerEventType.MEMBER_ROLE_CHANGED:
					{
						if (BnetParty.OnMemberEvent != null)
						{
							LeaveReason? nullable1 = null;
							BnetParty.OnMemberEvent(2, BnetParty.GetJoinedParty(partyId), partyListenerEvent.SubjectMemberId, true, nullable1);
						}
						break;
					}
					case PartyListenerEventType.RECEIVED_INVITE_ADDED:
					case PartyListenerEventType.RECEIVED_INVITE_REMOVED:
					{
						if (BnetParty.OnReceivedInvite != null)
						{
							OnlineEventType onlineEventType1 = (partyListenerEvent.Type != PartyListenerEventType.RECEIVED_INVITE_ADDED ? OnlineEventType.REMOVED : OnlineEventType.ADDED);
							PartyType partyType1 = PartyType.DEFAULT;
							if (partyListenerEvent.StringData != null)
							{
								EnumUtils.TryGetEnum<PartyType>(partyListenerEvent.StringData, out partyType1);
							}
							bgs.PartyInfo partyInfo = new bgs.PartyInfo(partyId, partyType1);
							InviteRemoveReason? nullable2 = null;
							if (partyListenerEvent.Type == PartyListenerEventType.RECEIVED_INVITE_REMOVED)
							{
								nullable2 = new InviteRemoveReason?((InviteRemoveReason)partyListenerEvent.UintData);
							}
							BnetParty.OnReceivedInvite(onlineEventType1, partyInfo, partyListenerEvent.UlongData, nullable2);
						}
						break;
					}
					case PartyListenerEventType.PARTY_INVITE_SENT:
					case PartyListenerEventType.PARTY_INVITE_REMOVED:
					{
						if (BnetParty.OnSentInvite != null)
						{
							bool subjectMemberId = partyListenerEvent.SubjectMemberId == BnetGameAccountId.CreateFromEntityId(BattleNet.GetMyGameAccountId());
							OnlineEventType onlineEventType2 = (partyListenerEvent.Type != PartyListenerEventType.PARTY_INVITE_SENT ? OnlineEventType.REMOVED : OnlineEventType.ADDED);
							PartyType partyType2 = PartyType.DEFAULT;
							if (partyListenerEvent.StringData != null)
							{
								EnumUtils.TryGetEnum<PartyType>(partyListenerEvent.StringData, out partyType2);
							}
							bgs.PartyInfo partyInfo1 = new bgs.PartyInfo(partyId, partyType2);
							InviteRemoveReason? nullable3 = null;
							if (partyListenerEvent.Type == PartyListenerEventType.PARTY_INVITE_REMOVED)
							{
								nullable3 = new InviteRemoveReason?((InviteRemoveReason)partyListenerEvent.UintData);
							}
							BnetParty.OnSentInvite(onlineEventType2, partyInfo1, partyListenerEvent.UlongData, subjectMemberId, nullable3);
						}
						break;
					}
					case PartyListenerEventType.INVITE_REQUEST_ADDED:
					case PartyListenerEventType.INVITE_REQUEST_REMOVED:
					{
						if (BnetParty.OnSentInvite != null)
						{
							OnlineEventType onlineEventType3 = (partyListenerEvent.Type != PartyListenerEventType.INVITE_REQUEST_ADDED ? OnlineEventType.REMOVED : OnlineEventType.ADDED);
							bgs.PartyInfo joinedParty = BnetParty.GetJoinedParty(partyId);
							InviteRequestRemovedReason? nullable4 = null;
							if (partyListenerEvent.Type == PartyListenerEventType.INVITE_REQUEST_REMOVED)
							{
								nullable4 = new InviteRequestRemovedReason?((InviteRequestRemovedReason)partyListenerEvent.UintData);
							}
							InviteRequest inviteRequest = new InviteRequest()
							{
								TargetId = partyListenerEvent.TargetMemberId,
								TargetName = partyListenerEvent.StringData2,
								RequesterId = partyListenerEvent.SubjectMemberId,
								RequesterName = partyListenerEvent.StringData
							};
							BnetParty.OnReceivedInviteRequest(onlineEventType3, joinedParty, inviteRequest, nullable4);
						}
						break;
					}
					case PartyListenerEventType.CHAT_MESSAGE_RECEIVED:
					{
						if (BnetParty.OnChatMessage != null)
						{
							BnetParty.OnChatMessage(BnetParty.GetJoinedParty(partyId), partyListenerEvent.SubjectMemberId, partyListenerEvent.StringData);
						}
						break;
					}
					case PartyListenerEventType.PARTY_ATTRIBUTE_CHANGED:
					{
						bgs.PartyInfo joinedParty1 = BnetParty.GetJoinedParty(partyId);
						string str = partyListenerEvent.StringData;
						if (str == "WTCG.Party.Type")
						{
							PartyType partyTypeFromString1 = BnetParty.GetPartyTypeFromString(BnetParty.GetPartyAttributeString(partyId, "WTCG.Party.Type"));
							if (partyTypeFromString1 != PartyType.DEFAULT)
							{
								BnetParty.s_joinedParties[partyId] = partyTypeFromString1;
							}
						}
						bnet.protocol.attribute.Variant variant = null;
						switch (partyListenerEvent.UintData)
						{
							case 0:
							{
								break;
							}
							case 1:
							{
								variant = new bnet.protocol.attribute.Variant()
								{
									IntValue = (long)partyListenerEvent.UlongData
								};
								break;
							}
							case 2:
							{
								variant = new bnet.protocol.attribute.Variant()
								{
									StringValue = partyListenerEvent.StringData2
								};
								break;
							}
							case 3:
							{
								variant = new bnet.protocol.attribute.Variant()
								{
									BlobValue = partyListenerEvent.BlobData
								};
								break;
							}
							default:
							{
								goto case 0;
							}
						}
						if (BnetParty.OnPartyAttributeChanged != null)
						{
							BnetParty.OnPartyAttributeChanged(joinedParty1, str, variant);
						}
						if (BnetParty.s_attributeChangedSubscribers != null && BnetParty.s_attributeChangedSubscribers.TryGetValue(str, out partyAttributeChangedHandlers))
						{
							BnetParty.PartyAttributeChangedHandler[] array = partyAttributeChangedHandlers.ToArray();
							for (int j = 0; j < (int)array.Length; j++)
							{
								array[j](joinedParty1, str, variant);
							}
						}
						break;
					}
				}
			}
		}

		private static void RaisePartyError(bool isOperationCallback, string szPartyType, BnetFeatureEvent featureEvent, string errorMessageFormat, params object[] args)
		{
			string str = string.Format(errorMessageFormat, args);
			PartyError partyError = new PartyError()
			{
				IsOperationCallback = isOperationCallback,
				DebugContext = str,
				ErrorCode = 0,
				Feature = BnetFeature.Party,
				FeatureEvent = featureEvent,
				szPartyType = szPartyType
			};
			BnetParty.RaisePartyError(partyError);
		}

		private static void RaisePartyError(bool isOperationCallback, string debugContext, BattleNetErrors errorCode, BnetFeature feature, BnetFeatureEvent featureEvent, PartyId partyId, string szPartyType, string stringData, string errorMessageFormat, params object[] args)
		{
			if (BnetParty.OnError == null)
			{
				return;
			}
			PartyError partyError = new PartyError()
			{
				IsOperationCallback = isOperationCallback,
				DebugContext = debugContext,
				ErrorCode = errorCode,
				Feature = feature,
				FeatureEvent = featureEvent,
				PartyId = partyId,
				szPartyType = szPartyType,
				StringData = stringData
			};
			BnetParty.RaisePartyError(partyError);
		}

		private static void RaisePartyError(PartyError error)
		{
			string str = string.Format("BnetParty: event={0} feature={1} code={2} partyId={3} type={4} strData={5}", new object[] { error.FeatureEvent.ToString(), (int)error.FeatureEvent, error.ErrorCode, error.PartyId, error.szPartyType, error.StringData });
			Log.Party.Print((error.ErrorCode != 0 ? LogLevel.Error : LogLevel.Info), str);
			if (BnetParty.OnError != null)
			{
				BnetParty.OnError(error);
			}
		}

		public static void RegisterAttributeChangedHandler(string attributeKey, BnetParty.PartyAttributeChangedHandler handler)
		{
			List<BnetParty.PartyAttributeChangedHandler> partyAttributeChangedHandlers;
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			if (BnetParty.s_attributeChangedSubscribers == null)
			{
				BnetParty.s_attributeChangedSubscribers = new Map<string, List<BnetParty.PartyAttributeChangedHandler>>();
			}
			if (!BnetParty.s_attributeChangedSubscribers.TryGetValue(attributeKey, out partyAttributeChangedHandlers))
			{
				partyAttributeChangedHandlers = new List<BnetParty.PartyAttributeChangedHandler>();
				BnetParty.s_attributeChangedSubscribers[attributeKey] = partyAttributeChangedHandlers;
			}
			if (!partyAttributeChangedHandlers.Contains(handler))
			{
				partyAttributeChangedHandlers.Add(handler);
			}
		}

		public static void RemoveFromAllEventHandlers(object targetObject)
		{
			Type type;
			if (targetObject != null)
			{
				type = targetObject.GetType();
			}
			else
			{
				type = null;
			}
			Type type1 = type;
			if (BnetParty.OnError != null)
			{
				IEnumerator enumerator = (BnetParty.OnError.GetInvocationList().Clone() as Array).GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Delegate current = (Delegate)enumerator.Current;
						if (current.Target != targetObject && (current.Target != null || current.Method.DeclaringType != type1))
						{
							continue;
						}
						BnetParty.OnError = (BnetParty.PartyErrorHandler)Delegate.Remove(BnetParty.OnError, (BnetParty.PartyErrorHandler)current);
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable == null)
					{
					}
					disposable.Dispose();
				}
			}
			if (BnetParty.OnJoined != null)
			{
				IEnumerator enumerator1 = (BnetParty.OnJoined.GetInvocationList().Clone() as Array).GetEnumerator();
				try
				{
					while (enumerator1.MoveNext())
					{
						Delegate @delegate = (Delegate)enumerator1.Current;
						if (@delegate.Target != targetObject && (@delegate.Target != null || @delegate.Method.DeclaringType != type1))
						{
							continue;
						}
						BnetParty.OnJoined = (BnetParty.JoinedHandler)Delegate.Remove(BnetParty.OnJoined, (BnetParty.JoinedHandler)@delegate);
					}
				}
				finally
				{
					IDisposable disposable1 = enumerator1 as IDisposable;
					if (disposable1 == null)
					{
					}
					disposable1.Dispose();
				}
			}
			if (BnetParty.OnPrivacyLevelChanged != null)
			{
				IEnumerator enumerator2 = (BnetParty.OnPrivacyLevelChanged.GetInvocationList().Clone() as Array).GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						Delegate current1 = (Delegate)enumerator2.Current;
						if (current1.Target != targetObject && (current1.Target != null || current1.Method.DeclaringType != type1))
						{
							continue;
						}
						BnetParty.OnPrivacyLevelChanged = (BnetParty.PrivacyLevelChangedHandler)Delegate.Remove(BnetParty.OnPrivacyLevelChanged, (BnetParty.PrivacyLevelChangedHandler)current1);
					}
				}
				finally
				{
					IDisposable disposable2 = enumerator2 as IDisposable;
					if (disposable2 == null)
					{
					}
					disposable2.Dispose();
				}
			}
			if (BnetParty.OnMemberEvent != null)
			{
				IEnumerator enumerator3 = (BnetParty.OnMemberEvent.GetInvocationList().Clone() as Array).GetEnumerator();
				try
				{
					while (enumerator3.MoveNext())
					{
						Delegate delegate1 = (Delegate)enumerator3.Current;
						if (delegate1.Target != targetObject && (delegate1.Target != null || delegate1.Method.DeclaringType != type1))
						{
							continue;
						}
						BnetParty.OnMemberEvent = (BnetParty.MemberEventHandler)Delegate.Remove(BnetParty.OnMemberEvent, (BnetParty.MemberEventHandler)delegate1);
					}
				}
				finally
				{
					IDisposable disposable3 = enumerator3 as IDisposable;
					if (disposable3 == null)
					{
					}
					disposable3.Dispose();
				}
			}
			if (BnetParty.OnReceivedInvite != null)
			{
				IEnumerator enumerator4 = (BnetParty.OnReceivedInvite.GetInvocationList().Clone() as Array).GetEnumerator();
				try
				{
					while (enumerator4.MoveNext())
					{
						Delegate current2 = (Delegate)enumerator4.Current;
						if (current2.Target != targetObject && (current2.Target != null || current2.Method.DeclaringType != type1))
						{
							continue;
						}
						BnetParty.OnReceivedInvite = (BnetParty.ReceivedInviteHandler)Delegate.Remove(BnetParty.OnReceivedInvite, (BnetParty.ReceivedInviteHandler)current2);
					}
				}
				finally
				{
					IDisposable disposable4 = enumerator4 as IDisposable;
					if (disposable4 == null)
					{
					}
					disposable4.Dispose();
				}
			}
			if (BnetParty.OnSentInvite != null)
			{
				IEnumerator enumerator5 = (BnetParty.OnSentInvite.GetInvocationList().Clone() as Array).GetEnumerator();
				try
				{
					while (enumerator5.MoveNext())
					{
						Delegate delegate2 = (Delegate)enumerator5.Current;
						if (delegate2.Target != targetObject && (delegate2.Target != null || delegate2.Method.DeclaringType != type1))
						{
							continue;
						}
						BnetParty.OnSentInvite = (BnetParty.SentInviteHandler)Delegate.Remove(BnetParty.OnSentInvite, (BnetParty.SentInviteHandler)delegate2);
					}
				}
				finally
				{
					IDisposable disposable5 = enumerator5 as IDisposable;
					if (disposable5 == null)
					{
					}
					disposable5.Dispose();
				}
			}
			if (BnetParty.OnReceivedInviteRequest != null)
			{
				IEnumerator enumerator6 = (BnetParty.OnReceivedInviteRequest.GetInvocationList().Clone() as Array).GetEnumerator();
				try
				{
					while (enumerator6.MoveNext())
					{
						Delegate current3 = (Delegate)enumerator6.Current;
						if (current3.Target != targetObject && (current3.Target != null || current3.Method.DeclaringType != type1))
						{
							continue;
						}
						BnetParty.OnReceivedInviteRequest = (BnetParty.ReceivedInviteRequestHandler)Delegate.Remove(BnetParty.OnReceivedInviteRequest, (BnetParty.ReceivedInviteRequestHandler)current3);
					}
				}
				finally
				{
					IDisposable disposable6 = enumerator6 as IDisposable;
					if (disposable6 == null)
					{
					}
					disposable6.Dispose();
				}
			}
			if (BnetParty.OnChatMessage != null)
			{
				IEnumerator enumerator7 = (BnetParty.OnChatMessage.GetInvocationList().Clone() as Array).GetEnumerator();
				try
				{
					while (enumerator7.MoveNext())
					{
						Delegate delegate3 = (Delegate)enumerator7.Current;
						if (delegate3.Target != targetObject && (delegate3.Target != null || delegate3.Method.DeclaringType != type1))
						{
							continue;
						}
						BnetParty.OnChatMessage = (BnetParty.ChatMessageHandler)Delegate.Remove(BnetParty.OnChatMessage, (BnetParty.ChatMessageHandler)delegate3);
					}
				}
				finally
				{
					IDisposable disposable7 = enumerator7 as IDisposable;
					if (disposable7 == null)
					{
					}
					disposable7.Dispose();
				}
			}
			if (BnetParty.OnPartyAttributeChanged != null)
			{
				IEnumerator enumerator8 = (BnetParty.OnPartyAttributeChanged.GetInvocationList().Clone() as Array).GetEnumerator();
				try
				{
					while (enumerator8.MoveNext())
					{
						Delegate current4 = (Delegate)enumerator8.Current;
						if (current4.Target != targetObject && (current4.Target != null || current4.Method.DeclaringType != type1))
						{
							continue;
						}
						BnetParty.OnPartyAttributeChanged = (BnetParty.PartyAttributeChangedHandler)Delegate.Remove(BnetParty.OnPartyAttributeChanged, (BnetParty.PartyAttributeChangedHandler)current4);
					}
				}
				finally
				{
					IDisposable disposable8 = enumerator8 as IDisposable;
					if (disposable8 == null)
					{
					}
					disposable8.Dispose();
				}
			}
			if (BnetParty.s_attributeChangedSubscribers != null)
			{
				foreach (KeyValuePair<string, List<BnetParty.PartyAttributeChangedHandler>> sAttributeChangedSubscriber in BnetParty.s_attributeChangedSubscribers)
				{
					var array = sAttributeChangedSubscriber.Value.Select((BnetParty.PartyAttributeChangedHandler h, int idx) => new { Handler = h, Index = idx }).ToArray();
					for (int i = 0; i < (int)array.Length; i++)
					{
						var variable = array[i];
						if (variable.Handler.Target == targetObject || variable.Handler.Method.DeclaringType == type1)
						{
							sAttributeChangedSubscriber.Value.RemoveAt(variable.Index);
						}
					}
				}
			}
		}

		public static void RequestInvite(PartyId partyId, BnetGameAccountId whomToAskForApproval, BnetGameAccountId whomToInvite, PartyType partyType)
		{
			if (!BnetParty.IsLeader(partyId))
			{
				EntityId entityId = partyId.ToEntityId();
				EntityId entityId1 = BnetEntityId.CreateEntityId(whomToAskForApproval);
				EntityId entityId2 = BnetEntityId.CreateEntityId(whomToInvite);
				BattleNet.RequestPartyInvite(entityId, entityId1, entityId2, EnumUtils.GetString<PartyType>(partyType));
				return;
			}
			PartyError partyError = new PartyError()
			{
				IsOperationCallback = true,
				DebugContext = "RequestInvite",
				ErrorCode = 20,
				Feature = BnetFeature.Party,
				FeatureEvent = BnetFeatureEvent.Party_RequestPartyInvite_Callback,
				PartyId = partyId,
				szPartyType = EnumUtils.GetString<PartyType>(partyType),
				StringData = "leaders cannot RequestInvite - use SendInvite instead."
			};
			BnetParty.OnError(partyError);
		}

		public static void RevokeSentInvite(PartyId partyId, ulong inviteId)
		{
			if (!BnetParty.IsInParty(partyId))
			{
				return;
			}
			BattleNet.RevokePartyInvite(partyId.ToEntityId(), inviteId);
		}

		public static void SendChatMessage(PartyId partyId, string chatMessage)
		{
			if (!BnetParty.IsInParty(partyId))
			{
				return;
			}
			BattleNet.SendPartyChatMessage(partyId.ToEntityId(), chatMessage);
		}

		public static void SendInvite(PartyId toWhichPartyId, BnetGameAccountId recipientId)
		{
			if (!BnetParty.IsInParty(toWhichPartyId))
			{
				return;
			}
			EntityId entityId = toWhichPartyId.ToEntityId();
			BattleNet.SendPartyInvite(entityId, BnetEntityId.CreateEntityId(recipientId), false);
		}

		public static void SetLeader(PartyId partyId, BnetGameAccountId memberId)
		{
			if (!BnetParty.IsInParty(partyId))
			{
				return;
			}
			EntityId entityId = partyId.ToEntityId();
			EntityId entityId1 = BnetEntityId.CreateEntityId(memberId);
			PartyType partyType = BnetParty.GetPartyType(partyId);
			BattleNet.AssignPartyRole(entityId, entityId1, bgs.PartyMember.GetLeaderRoleId(partyType));
		}

		public static void SetPartyAttributeBlob(PartyId partyId, string attributeKey, byte[] value)
		{
			BattleNet.SetPartyAttributeBlob(partyId.ToEntityId(), attributeKey, value);
		}

		public static void SetPartyAttributeLong(PartyId partyId, string attributeKey, long value)
		{
			BattleNet.SetPartyAttributeLong(partyId.ToEntityId(), attributeKey, value);
		}

		public static void SetPartyAttributeString(PartyId partyId, string attributeKey, string value)
		{
			BattleNet.SetPartyAttributeString(partyId.ToEntityId(), attributeKey, value);
		}

		public static void SetPrivacy(PartyId partyId, PrivacyLevel privacyLevel)
		{
			if (!BnetParty.IsInParty(partyId))
			{
				return;
			}
			BattleNet.SetPartyPrivacy(partyId.ToEntityId(), (int)privacyLevel);
		}

		public static bool UnregisterAttributeChangedHandler(string attributeKey, BnetParty.PartyAttributeChangedHandler handler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			if (BnetParty.s_attributeChangedSubscribers == null)
			{
				return false;
			}
			List<BnetParty.PartyAttributeChangedHandler> partyAttributeChangedHandlers = null;
			if (!BnetParty.s_attributeChangedSubscribers.TryGetValue(attributeKey, out partyAttributeChangedHandlers))
			{
				return false;
			}
			return partyAttributeChangedHandlers.Remove(handler);
		}

		public static event BnetParty.ChatMessageHandler OnChatMessage;

		public static event BnetParty.PartyErrorHandler OnError;

		public static event BnetParty.JoinedHandler OnJoined;

		public static event BnetParty.MemberEventHandler OnMemberEvent;

		public static event BnetParty.PartyAttributeChangedHandler OnPartyAttributeChanged;

		public static event BnetParty.PrivacyLevelChangedHandler OnPrivacyLevelChanged;

		public static event BnetParty.ReceivedInviteHandler OnReceivedInvite;

		public static event BnetParty.ReceivedInviteRequestHandler OnReceivedInviteRequest;

		public static event BnetParty.SentInviteHandler OnSentInvite;

		public delegate void ChatMessageHandler(bgs.PartyInfo party, BnetGameAccountId speakerId, string chatMessage);

		public delegate void CreateSuccessCallback(PartyType type, PartyId newlyCreatedPartyId);

		public enum FriendlyGameRoleSet
		{
			Inviter = 1,
			Invitee = 2
		}

		public delegate void JoinedHandler(OnlineEventType evt, bgs.PartyInfo party, LeaveReason? reason);

		public delegate void MemberEventHandler(OnlineEventType evt, bgs.PartyInfo party, BnetGameAccountId memberId, bool isRolesUpdate, LeaveReason? reason);

		public delegate void PartyAttributeChangedHandler(bgs.PartyInfo party, string attributeKey, bnet.protocol.attribute.Variant attributeValue);

		public delegate void PartyErrorHandler(PartyError error);

		public delegate void PrivacyLevelChangedHandler(bgs.PartyInfo party, PrivacyLevel newPrivacyLevel);

		public delegate void ReceivedInviteHandler(OnlineEventType evt, bgs.PartyInfo party, ulong inviteId, InviteRemoveReason? reason);

		public delegate void ReceivedInviteRequestHandler(OnlineEventType evt, bgs.PartyInfo party, InviteRequest request, InviteRequestRemovedReason? reason);

		public delegate void SentInviteHandler(OnlineEventType evt, bgs.PartyInfo party, ulong inviteId, bool senderIsMyself, InviteRemoveReason? reason);

		public enum SpectatorPartyRoleSet
		{
			Member = 1,
			Leader = 2
		}
	}
}