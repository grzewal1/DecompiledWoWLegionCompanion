using bgs.types;
using bnet.protocol;
using bnet.protocol.account;
using bnet.protocol.game_utilities;
using System;
using System.Collections.Generic;

namespace bgs
{
	public interface IBattleNet
	{
		void AcceptFriendlyChallenge(ref bgs.types.EntityId partyId);

		void AcceptPartyInvite(ulong invitationId);

		void AnswerChallenge(ulong challengeID, string answer);

		void ApplicationWasPaused();

		void ApplicationWasUnpaused();

		void AppQuit();

		void AssignPartyRole(bgs.types.EntityId partyId, bgs.types.EntityId memberId, uint roleId);

		int BattleNetStatus();

		void CancelChallenge(ulong challengeID);

		void CancelFindGame();

		bool CheckWebAuth(out string url);

		void ClearBnetEvents();

		void ClearChallenges();

		void ClearErrors();

		void ClearFriendsUpdates();

		void ClearNotifications();

		void ClearPartyAttribute(bgs.types.EntityId partyId, string attributeKey);

		void ClearPartyListenerEvents();

		void ClearPartyUpdates();

		void ClearPresence();

		void ClearWhispers();

		ClientInterface Client();

		void CloseAurora();

		void CreateParty(string partyType, int privacyLevel, byte[] creatorBlob);

		long CurrentUTCTime();

		void DeclineFriendlyChallenge(ref bgs.types.EntityId partyId);

		void DeclinePartyInvite(ulong invitationId);

		void DissolveParty(bgs.types.EntityId partyId);

		string FilterProfanity(string unfiltered);

		void FindGame(byte[] requestGuid, int gameType, int missionId, long deckId, long aiDeckId, bool setScenarioIdAttr);

		string GetAccountCountry();

		constants.BnetRegion GetAccountRegion();

		void GetAllPartyAttributes(bgs.types.EntityId partyId, out string[] allKeys);

		void GetAllValuesForAttribute(string attributeKey, int context);

		void GetBnetEvents([Out] BattleNet.BnetEvent[] events);

		int GetBnetEventsSize();

		void GetChallenges([Out] ChallengeInfo[] challenges);

		int GetCountPartyMembers(bgs.types.EntityId partyId);

		constants.BnetRegion GetCurrentRegion();

		string GetEnvironment();

		void GetErrors([Out] BnetErrorInfo[] errors);

		int GetErrorsCount();

		void GetFriendsInfo(ref FriendsInfo info);

		void GetFriendsUpdates([Out] FriendsUpdate[] updates);

		List<bnet.protocol.EntityId> GetGameAccountList();

		void GetGameAccountState(GetGameAccountStateRequest request, RPCContextDelegate callback);

		string GetLaunchOption(string key, bool encrypted);

		BattleNetLogSource GetLogSource();

		int GetMaxPartyMembers(bgs.types.EntityId partyId);

		bgs.types.EntityId GetMyAccountId();

		bgs.types.EntityId GetMyGameAccountId();

		int GetNotificationCount();

		void GetNotifications([Out] BnetNotification[] notifications);

		void GetPartyAttributeBlob(bgs.types.EntityId partyId, string attributeKey, out byte[] value);

		bool GetPartyAttributeLong(bgs.types.EntityId partyId, string attributeKey, out long value);

		void GetPartyAttributeString(bgs.types.EntityId partyId, string attributeKey, out string value);

		void GetPartyInviteRequests(bgs.types.EntityId partyId, out InviteRequest[] requests);

		void GetPartyListenerEvents(out PartyListenerEvent[] events);

		void GetPartyMembers(bgs.types.EntityId partyId, out bgs.types.PartyMember[] members);

		int GetPartyPrivacy(bgs.types.EntityId partyId);

		void GetPartySentInvites(bgs.types.EntityId partyId, out PartyInvite[] invites);

		void GetPartyUpdates([Out] PartyEvent[] updates);

		void GetPartyUpdatesInfo(ref bgs.types.PartyInfo info);

		void GetPlayRestrictions(ref Lockouts restrictions, bool reload);

		int GetPort();

		void GetPresence([Out] PresenceUpdate[] updates);

		QueueEvent GetQueueEvent();

		void GetQueueInfo(ref QueueInfo queueInfo);

		double GetRealTimeSinceStartup();

		void GetReceivedPartyInvites(out PartyInvite[] invites);

		byte[] GetSessionKey();

		int GetShutdownMinutes();

		string GetUserEmailAddress();

		string GetVersion();

		void GetWhisperInfo(ref WhisperInfo info);

		void GetWhispers([Out] BnetWhisper[] whispers);

		void IgnoreInviteRequest(bgs.types.EntityId partyId, bgs.types.EntityId requestedTargetId);

		bool Init(bool fromEditor, string username, string targetServer, int port, SslParameters sslParams, ClientInterface ci);

		bool IsInitialized();

		bool IsVersionInt();

		void JoinParty(bgs.types.EntityId partyId, string partyType);

		void KickPartyMember(bgs.types.EntityId partyId, bgs.types.EntityId memberId);

		void LeaveParty(bgs.types.EntityId partyId);

		void ManageFriendInvite(int action, ulong inviteId);

		GamesAPI.GetAllValuesForAttributeResult NextGetAllValuesForAttributeResult();

		GamesAPI.UtilResponse NextUtilPacket();

		int NumChallenges();

		int PresenceSize();

		void ProcessAurora();

		void ProvideWebAuthToken(string token);

		void QueryAurora();

		void RemoveFriend(BnetAccountId account);

		void RequestCloseAurora();

		void RequestPartyInvite(bgs.types.EntityId partyId, bgs.types.EntityId whomToAskForApproval, bgs.types.EntityId whomToInvite, string szPartyType);

		void RequestPresenceFields(bool isGameAccountEntityId, [In] bgs.types.EntityId entityId, [In] PresenceFieldKey[] fieldList);

		void RescindFriendlyChallenge(ref bgs.types.EntityId partyId);

		void RevokePartyInvite(bgs.types.EntityId partyId, ulong invitationId);

		void SendClientRequest(ClientRequest request, RPCContextDelegate callback = null);

		void SendFriendInvite(string inviter, string invitee, bool byEmail);

		void SendFriendlyChallengeInvite(ref bgs.types.EntityId gameAccount, int scenarioId);

		void SendPartyChatMessage(bgs.types.EntityId partyId, string message);

		void SendPartyInvite(bgs.types.EntityId partyId, bgs.types.EntityId inviteeId, bool isReservation);

		void SendUtilPacket(int packetId, int systemId, byte[] bytes, int size, int subID, int context, ulong route);

		void SendWhisper(BnetGameAccountId gameAccount, string message);

		void SetMyFriendlyChallengeDeck(ref bgs.types.EntityId partyId, long deckID);

		void SetPartyAttributeBlob(bgs.types.EntityId partyId, string attributeKey, [In] byte[] value);

		void SetPartyAttributeLong(bgs.types.EntityId partyId, string attributeKey, [In] long value);

		void SetPartyAttributeString(bgs.types.EntityId partyId, string attributeKey, [In] string value);

		void SetPartyPrivacy(bgs.types.EntityId partyId, int privacyLevel);

		void SetPresenceBlob(uint field, byte[] val);

		void SetPresenceBool(uint field, bool val);

		void SetPresenceInt(uint field, long val);

		void SetPresenceString(uint field, string val);

		void SetRichPresence([In] RichPresenceUpdate[] updates);
	}
}