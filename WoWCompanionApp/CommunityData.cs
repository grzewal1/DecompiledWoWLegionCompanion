using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace WoWCompanionApp
{
	public class CommunityData
	{
		private static CommunityData m_instance;

		private static Dictionary<ulong, Community> m_communityDictionary;

		private static List<CommunityPendingInvite> m_pendingInvites;

		private static Community m_guildCommunity;

		public static CommunityData Instance
		{
			get
			{
				if (CommunityData.m_instance == null)
				{
					CommunityData.m_instance = new CommunityData();
				}
				return CommunityData.m_instance;
			}
		}

		static CommunityData()
		{
			CommunityData.m_instance = null;
			CommunityData.m_communityDictionary = new Dictionary<ulong, Community>();
			CommunityData.m_pendingInvites = new List<CommunityPendingInvite>();
			CommunityData.m_guildCommunity = null;
		}

		private CommunityData()
		{
			Club.OnClubAdded += new Club.ClubAddedHandler(this.OnClubAdded);
			Club.OnClubRemoved += new Club.ClubRemovedHandler(this.OnClubRemoved);
			Club.OnClubUpdated += new Club.ClubUpdatedHandler(this.OnClubUpdated);
			Club.OnClubInvitationAddedForSelf += new Club.ClubInvitationAddedForSelfHandler(this.OnInviteAdded);
			Club.OnClubInvitationRemovedForSelf += new Club.ClubInvitationRemovedForSelfHandler(this.OnInviteRemoved);
			Club.OnClubMemberAdded += new Club.ClubMemberAddedHandler(this.OnMemberAdded);
			Club.OnClubMemberRemoved += new Club.ClubMemberRemovedHandler(this.OnMemberRemoved);
			Club.OnClubMemberUpdated += new Club.ClubMemberUpdatedHandler(this.OnMemberUpdated);
			Club.OnClubMemberRoleUpdated += new Club.ClubMemberRoleUpdatedHandler(this.OnMemberRoleUpdated);
			Club.OnClubMemberPresenceUpdated += new Club.ClubMemberPresenceUpdatedHandler(this.OnMemberPresenceUpdated);
			Club.OnClubStreamAdded += new Club.ClubStreamAddedHandler(this.OnStreamAdded);
			Club.OnClubStreamRemoved += new Club.ClubStreamRemovedHandler(this.OnStreamRemoved);
		}

		private void AddCommunity(ClubInfo community)
		{
			if (!CommunityData.m_communityDictionary.ContainsKey(community.clubId))
			{
				Community community1 = new Community(community);
				CommunityData.m_communityDictionary.Add(community1.ClubId, community1);
				community1.RefreshStreams();
			}
		}

		public void ClearData()
		{
			CommunityData.m_communityDictionary.Clear();
			CommunityData.m_pendingInvites.Clear();
		}

		private void FireChannelRefreshCallback(ulong clubID)
		{
			if (CommunityData.OnChannelRefresh != null)
			{
				CommunityData.OnChannelRefresh(clubID);
			}
		}

		private void FireCommunityRefreshCallback()
		{
			if (CommunityData.OnCommunityRefresh != null)
			{
				CommunityData.OnCommunityRefresh();
			}
		}

		private void FireInviteRefreshCallback()
		{
			if (CommunityData.OnInviteRefresh != null)
			{
				CommunityData.OnInviteRefresh();
			}
		}

		private void FireRosterRefreshCallback(ulong clubID)
		{
			if (CommunityData.OnRosterRefresh != null)
			{
				CommunityData.OnRosterRefresh(clubID);
			}
		}

		public void ForEachCommunity(Action<Community> action)
		{
			foreach (Community value in CommunityData.m_communityDictionary.Values)
			{
				if (value.IsGuild())
				{
					continue;
				}
				action(value);
			}
		}

		public void ForGuild(Action<Community> action)
		{
			if (CommunityData.m_guildCommunity != null)
			{
				action(CommunityData.m_guildCommunity);
			}
		}

		public ReadOnlyCollection<CommunityPendingInvite> GetPendingInvites()
		{
			return CommunityData.m_pendingInvites.AsReadOnly();
		}

		public void HandleMessageAddedEvent(Club.ClubMessageAddedEvent messageEvent)
		{
			if (CommunityData.m_communityDictionary.ContainsKey(messageEvent.ClubID))
			{
				CommunityData.m_communityDictionary[messageEvent.ClubID].HandleMessageAddedEvent(messageEvent);
			}
		}

		public bool HasCommunities()
		{
			return CommunityData.m_communityDictionary.Count > (CommunityData.m_guildCommunity != null ? 1 : 0);
		}

		public bool HasGuild()
		{
			return CommunityData.m_guildCommunity != null;
		}

		public bool HasUnreadCommunityMessages(Community ignoreCommunity = null)
		{
			bool flag;
			Dictionary<ulong, Community>.ValueCollection.Enumerator enumerator = CommunityData.m_communityDictionary.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Community current = enumerator.Current;
					if (current == ignoreCommunity || !current.HasUnreadMessages(null))
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
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		private void OnClubAdded(Club.ClubAddedEvent newClubEvent)
		{
			ClubInfo? clubInfo = Club.GetClubInfo(newClubEvent.ClubID);
			if (clubInfo.HasValue)
			{
				this.AddCommunity(clubInfo.Value);
				this.FireCommunityRefreshCallback();
			}
		}

		private void OnClubRemoved(Club.ClubRemovedEvent removeClubEvent)
		{
			CommunityData.m_communityDictionary.Remove(removeClubEvent.ClubID);
			this.FireCommunityRefreshCallback();
		}

		private void OnClubUpdated(Club.ClubUpdatedEvent updateClubEvent)
		{
			if (CommunityData.m_communityDictionary.ContainsKey(updateClubEvent.ClubID))
			{
				CommunityData.m_communityDictionary[updateClubEvent.ClubID].HandleClubUpdatedEvent(updateClubEvent);
			}
		}

		private void OnInviteAdded(Club.ClubInvitationAddedForSelfEvent newInviteEvent)
		{
			CommunityData.m_pendingInvites.Add(new CommunityPendingInvite(newInviteEvent.Invitation));
			this.FireInviteRefreshCallback();
		}

		private void OnInviteRemoved(Club.ClubInvitationRemovedForSelfEvent inviteRemovedEvent)
		{
			this.RefreshInvitations();
			this.FireInviteRefreshCallback();
		}

		private void OnMemberAdded(Club.ClubMemberAddedEvent newMemberEvent)
		{
			if (CommunityData.m_communityDictionary.ContainsKey(newMemberEvent.ClubID))
			{
				CommunityData.m_communityDictionary[newMemberEvent.ClubID].HandleMemberAddedEvent(newMemberEvent);
				this.FireRosterRefreshCallback(newMemberEvent.ClubID);
			}
		}

		private void OnMemberPresenceUpdated(Club.ClubMemberPresenceUpdatedEvent updatePresenceEvent)
		{
			if (CommunityData.m_communityDictionary.ContainsKey(updatePresenceEvent.ClubID))
			{
				CommunityData.m_communityDictionary[updatePresenceEvent.ClubID].HandleMemberPresenceUpdatedEvent(updatePresenceEvent);
				this.FireRosterRefreshCallback(updatePresenceEvent.ClubID);
			}
		}

		private void OnMemberRemoved(Club.ClubMemberRemovedEvent removeMemberEvent)
		{
			if (CommunityData.m_communityDictionary.ContainsKey(removeMemberEvent.ClubID))
			{
				CommunityData.m_communityDictionary[removeMemberEvent.ClubID].HandleMemberRemovedEvent(removeMemberEvent);
				this.FireRosterRefreshCallback(removeMemberEvent.ClubID);
			}
		}

		private void OnMemberRoleUpdated(Club.ClubMemberRoleUpdatedEvent updateRoleEvent)
		{
			if (CommunityData.m_communityDictionary.ContainsKey(updateRoleEvent.ClubID))
			{
				CommunityData.m_communityDictionary[updateRoleEvent.ClubID].HandleMemberRoleUpdatedEvent(updateRoleEvent);
				this.FireRosterRefreshCallback(updateRoleEvent.ClubID);
			}
		}

		private void OnMemberUpdated(Club.ClubMemberUpdatedEvent updateMemberEvent)
		{
			if (CommunityData.m_communityDictionary.ContainsKey(updateMemberEvent.ClubID))
			{
				CommunityData.m_communityDictionary[updateMemberEvent.ClubID].HandleMemberUpdatedEvent(updateMemberEvent);
				this.FireRosterRefreshCallback(updateMemberEvent.ClubID);
			}
		}

		private void OnStreamAdded(Club.ClubStreamAddedEvent newStreamEvent)
		{
			if (CommunityData.m_communityDictionary.ContainsKey(newStreamEvent.ClubID))
			{
				CommunityData.m_communityDictionary[newStreamEvent.ClubID].HandleStreamAddedEvent(newStreamEvent);
				this.FireChannelRefreshCallback(newStreamEvent.ClubID);
			}
		}

		private void OnStreamRemoved(Club.ClubStreamRemovedEvent removeStreamEvent)
		{
			if (CommunityData.m_communityDictionary.ContainsKey(removeStreamEvent.ClubID))
			{
				CommunityData.m_communityDictionary[removeStreamEvent.ClubID].HandleStreamRemovedEvent(removeStreamEvent);
				this.FireChannelRefreshCallback(removeStreamEvent.ClubID);
			}
		}

		public void RefreshCommunities()
		{
			foreach (ClubInfo subscribedClub in Club.GetSubscribedClubs())
			{
				CommunityData.Instance.AddCommunity(subscribedClub);
				if (subscribedClub.clubType != ClubType.Guild)
				{
					continue;
				}
				CommunityData.m_guildCommunity = CommunityData.m_communityDictionary[subscribedClub.clubId];
			}
		}

		public void RefreshInvitations()
		{
			CommunityData.m_pendingInvites.Clear();
			foreach (ClubSelfInvitationInfo invitationsForSelf in Club.GetInvitationsForSelf())
			{
				CommunityData.m_pendingInvites.Add(new CommunityPendingInvite(invitationsForSelf));
			}
		}

		public void Shutdown()
		{
			if (CommunityData.m_instance != null)
			{
				Club.OnClubAdded -= new Club.ClubAddedHandler(this.OnClubAdded);
				Club.OnClubRemoved -= new Club.ClubRemovedHandler(this.OnClubRemoved);
				Club.OnClubUpdated -= new Club.ClubUpdatedHandler(this.OnClubUpdated);
				Club.OnClubInvitationAddedForSelf -= new Club.ClubInvitationAddedForSelfHandler(this.OnInviteAdded);
				Club.OnClubInvitationRemovedForSelf -= new Club.ClubInvitationRemovedForSelfHandler(this.OnInviteRemoved);
				Club.OnClubMemberAdded -= new Club.ClubMemberAddedHandler(this.OnMemberAdded);
				Club.OnClubMemberRemoved -= new Club.ClubMemberRemovedHandler(this.OnMemberRemoved);
				Club.OnClubMemberUpdated -= new Club.ClubMemberUpdatedHandler(this.OnMemberUpdated);
				Club.OnClubMemberRoleUpdated -= new Club.ClubMemberRoleUpdatedHandler(this.OnMemberRoleUpdated);
				Club.OnClubMemberPresenceUpdated -= new Club.ClubMemberPresenceUpdatedHandler(this.OnMemberPresenceUpdated);
				Club.OnClubStreamAdded -= new Club.ClubStreamAddedHandler(this.OnStreamAdded);
				Club.OnClubStreamRemoved -= new Club.ClubStreamRemovedHandler(this.OnStreamRemoved);
				CommunityData.m_communityDictionary.Clear();
				CommunityData.m_pendingInvites.Clear();
				CommunityData.m_instance = null;
			}
		}

		public static event CommunityData.CommunityRefreshHandler OnChannelRefresh
		{
			add
			{
				CommunityData.CommunityRefreshHandler communityRefreshHandler;
				CommunityData.CommunityRefreshHandler onChannelRefresh = CommunityData.OnChannelRefresh;
				do
				{
					communityRefreshHandler = onChannelRefresh;
					onChannelRefresh = Interlocked.CompareExchange<CommunityData.CommunityRefreshHandler>(ref CommunityData.OnChannelRefresh, (CommunityData.CommunityRefreshHandler)Delegate.Combine(communityRefreshHandler, value), onChannelRefresh);
				}
				while ((object)onChannelRefresh != (object)communityRefreshHandler);
			}
			remove
			{
				CommunityData.CommunityRefreshHandler communityRefreshHandler;
				CommunityData.CommunityRefreshHandler onChannelRefresh = CommunityData.OnChannelRefresh;
				do
				{
					communityRefreshHandler = onChannelRefresh;
					onChannelRefresh = Interlocked.CompareExchange<CommunityData.CommunityRefreshHandler>(ref CommunityData.OnChannelRefresh, (CommunityData.CommunityRefreshHandler)Delegate.Remove(communityRefreshHandler, value), onChannelRefresh);
				}
				while ((object)onChannelRefresh != (object)communityRefreshHandler);
			}
		}

		public static event CommunityData.RefreshHandler OnCommunityRefresh
		{
			add
			{
				CommunityData.RefreshHandler refreshHandler;
				CommunityData.RefreshHandler onCommunityRefresh = CommunityData.OnCommunityRefresh;
				do
				{
					refreshHandler = onCommunityRefresh;
					onCommunityRefresh = Interlocked.CompareExchange<CommunityData.RefreshHandler>(ref CommunityData.OnCommunityRefresh, (CommunityData.RefreshHandler)Delegate.Combine(refreshHandler, value), onCommunityRefresh);
				}
				while ((object)onCommunityRefresh != (object)refreshHandler);
			}
			remove
			{
				CommunityData.RefreshHandler refreshHandler;
				CommunityData.RefreshHandler onCommunityRefresh = CommunityData.OnCommunityRefresh;
				do
				{
					refreshHandler = onCommunityRefresh;
					onCommunityRefresh = Interlocked.CompareExchange<CommunityData.RefreshHandler>(ref CommunityData.OnCommunityRefresh, (CommunityData.RefreshHandler)Delegate.Remove(refreshHandler, value), onCommunityRefresh);
				}
				while ((object)onCommunityRefresh != (object)refreshHandler);
			}
		}

		public static event CommunityData.RefreshHandler OnInviteRefresh
		{
			add
			{
				CommunityData.RefreshHandler refreshHandler;
				CommunityData.RefreshHandler onInviteRefresh = CommunityData.OnInviteRefresh;
				do
				{
					refreshHandler = onInviteRefresh;
					onInviteRefresh = Interlocked.CompareExchange<CommunityData.RefreshHandler>(ref CommunityData.OnInviteRefresh, (CommunityData.RefreshHandler)Delegate.Combine(refreshHandler, value), onInviteRefresh);
				}
				while ((object)onInviteRefresh != (object)refreshHandler);
			}
			remove
			{
				CommunityData.RefreshHandler refreshHandler;
				CommunityData.RefreshHandler onInviteRefresh = CommunityData.OnInviteRefresh;
				do
				{
					refreshHandler = onInviteRefresh;
					onInviteRefresh = Interlocked.CompareExchange<CommunityData.RefreshHandler>(ref CommunityData.OnInviteRefresh, (CommunityData.RefreshHandler)Delegate.Remove(refreshHandler, value), onInviteRefresh);
				}
				while ((object)onInviteRefresh != (object)refreshHandler);
			}
		}

		public static event CommunityData.CommunityRefreshHandler OnRosterRefresh
		{
			add
			{
				CommunityData.CommunityRefreshHandler communityRefreshHandler;
				CommunityData.CommunityRefreshHandler onRosterRefresh = CommunityData.OnRosterRefresh;
				do
				{
					communityRefreshHandler = onRosterRefresh;
					onRosterRefresh = Interlocked.CompareExchange<CommunityData.CommunityRefreshHandler>(ref CommunityData.OnRosterRefresh, (CommunityData.CommunityRefreshHandler)Delegate.Combine(communityRefreshHandler, value), onRosterRefresh);
				}
				while ((object)onRosterRefresh != (object)communityRefreshHandler);
			}
			remove
			{
				CommunityData.CommunityRefreshHandler communityRefreshHandler;
				CommunityData.CommunityRefreshHandler onRosterRefresh = CommunityData.OnRosterRefresh;
				do
				{
					communityRefreshHandler = onRosterRefresh;
					onRosterRefresh = Interlocked.CompareExchange<CommunityData.CommunityRefreshHandler>(ref CommunityData.OnRosterRefresh, (CommunityData.CommunityRefreshHandler)Delegate.Remove(communityRefreshHandler, value), onRosterRefresh);
				}
				while ((object)onRosterRefresh != (object)communityRefreshHandler);
			}
		}

		public delegate void CommunityRefreshHandler(ulong communityId);

		public delegate void RefreshHandler();
	}
}