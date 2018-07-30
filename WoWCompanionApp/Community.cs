using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WoWCompanionApp
{
	public class Community
	{
		private ClubInfo m_clubInfo;

		private Dictionary<ulong, CommunityStream> m_streamList = new Dictionary<ulong, CommunityStream>();

		private List<CommunityMember> m_memberList = new List<CommunityMember>();

		private ClubPrivilegeInfo m_clubPrivilegeInfo;

		public ulong ClubId
		{
			get
			{
				return this.m_clubInfo.clubId;
			}
		}

		public string Description
		{
			get
			{
				return this.m_clubInfo.description;
			}
		}

		public string Name
		{
			get
			{
				return this.m_clubInfo.name;
			}
		}

		public Community(ClubInfo clubInfo)
		{
			this.m_clubInfo = clubInfo;
		}

		private void AddStream(ClubStreamInfo info)
		{
			if (!this.m_streamList.ContainsKey(info.streamId))
			{
				CommunityStream communityStream = new CommunityStream(this.ClubId, info);
				this.m_streamList.Add(communityStream.StreamId, communityStream);
			}
		}

		public void CreateStream(string name, string subject, bool modsOnly)
		{
			Club.CreateStream(this.ClubId, name, subject, modsOnly);
		}

		public void EditCommunity(string name, string description, uint? avatarId)
		{
			Club.EditClub(this.ClubId, name, this.m_clubInfo.shortName, description, avatarId, string.Empty);
		}

		public ReadOnlyCollection<CommunityStream> GetAllStreams()
		{
			List<CommunityStream> communityStreams = new List<CommunityStream>();
			foreach (CommunityStream value in this.m_streamList.Values)
			{
				communityStreams.Add(value);
			}
			return communityStreams.AsReadOnly();
		}

		public CommunityStream GetDefaultStream(ulong streamId)
		{
			if (this.m_streamList.ContainsKey(streamId))
			{
				return this.m_streamList[streamId];
			}
			if (this.m_streamList.Count <= 0)
			{
				return null;
			}
			return this.m_streamList.First<KeyValuePair<ulong, CommunityStream>>().Value;
		}

		public ReadOnlyCollection<CommunityMember> GetMemberList()
		{
			return this.m_memberList.AsReadOnly();
		}

		public void GetPrivileges()
		{
			Club.GetClubPrivileges(this.ClubId);
		}

		public void HandleClubUpdatedEvent(Club.ClubUpdatedEvent updateEvent)
		{
			ClubInfo? clubInfo = Club.GetClubInfo(updateEvent.ClubID);
			if (clubInfo.HasValue)
			{
				this.m_clubInfo = clubInfo.Value;
			}
		}

		public void HandleMemberAddedEvent(Club.ClubMemberAddedEvent addedEvent)
		{
			ClubMemberInfo? memberInfo = Club.GetMemberInfo(this.ClubId, addedEvent.MemberID);
			if (memberInfo.HasValue)
			{
				this.m_memberList.Add(new CommunityMember(this.ClubId, memberInfo.Value));
			}
		}

		public void HandleMemberPresenceUpdatedEvent(Club.ClubMemberPresenceUpdatedEvent updatePresenceEvent)
		{
			CommunityMember communityMember = this.m_memberList.Find((CommunityMember member) => member.MemberID == updatePresenceEvent.MemberID);
			communityMember.HandlePresenceUpdateEvent(updatePresenceEvent);
		}

		public void HandleMemberRemovedEvent(Club.ClubMemberRemovedEvent removeMemberEvent)
		{
			this.m_memberList.RemoveAll((CommunityMember member) => member.MemberID == removeMemberEvent.MemberID);
		}

		public void HandleMemberRoleUpdatedEvent(Club.ClubMemberRoleUpdatedEvent updateRoleEvent)
		{
			CommunityMember communityMember = this.m_memberList.Find((CommunityMember member) => member.MemberID == updateRoleEvent.MemberID);
			communityMember.HandleRoleUpdateEvent(updateRoleEvent);
		}

		public void HandleMemberUpdatedEvent(Club.ClubMemberUpdatedEvent updateMemberEvent)
		{
			CommunityMember communityMember = this.m_memberList.Find((CommunityMember member) => member.MemberID == updateMemberEvent.MemberID);
			if (communityMember != null)
			{
				communityMember.HandleMemberUpdatedEvent(updateMemberEvent);
			}
		}

		public void HandleMessageAddedEvent(Club.ClubMessageAddedEvent messageEvent)
		{
			if (this.m_streamList.ContainsKey(messageEvent.StreamID))
			{
				this.m_streamList[messageEvent.StreamID].HandleMessageAddedEvent(messageEvent);
			}
		}

		public void HandleStreamAddedEvent(Club.ClubStreamAddedEvent streamAddedEvent)
		{
			ClubStreamInfo? streamInfo = Club.GetStreamInfo(streamAddedEvent.ClubID, streamAddedEvent.StreamID);
			if (streamInfo.HasValue)
			{
				this.AddStream(streamInfo.Value);
			}
		}

		public void HandleStreamRemovedEvent(Club.ClubStreamRemovedEvent streamRemovedEvent)
		{
			if (this.m_streamList.ContainsKey(streamRemovedEvent.StreamID))
			{
				this.m_streamList.Remove(streamRemovedEvent.StreamID);
			}
		}

		public bool HasUnreadMessages(CommunityStream ignoreStream = null)
		{
			bool flag;
			Dictionary<ulong, CommunityStream>.ValueCollection.Enumerator enumerator = this.m_streamList.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					CommunityStream current = enumerator.Current;
					if (current == ignoreStream || !current.HasUnreadMessages())
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

		public void LeaveClub()
		{
			ClubRoleIdentifier? value = Club.GetMemberInfoForSelf(this.ClubId).Value.role;
			if ((value.GetValueOrDefault() != ClubRoleIdentifier.Owner ? true : !value.HasValue) || this.m_memberList.Count >= 2)
			{
				Club.LeaveClub(this.ClubId);
			}
			else
			{
				Club.DestroyClub(this.ClubId);
			}
		}

		public void MarkAllAsRead()
		{
			foreach (CommunityStream value in this.m_streamList.Values)
			{
				value.ClearNotifications();
			}
		}

		public void PopulateCommunityInfo()
		{
			this.RefreshStreams();
			this.RefreshMemberList();
			this.GetPrivileges();
		}

		public void RefreshMemberList()
		{
			this.m_memberList.Clear();
			foreach (uint clubMember in Club.GetClubMembers(this.ClubId, null))
			{
				ClubMemberInfo? memberInfo = Club.GetMemberInfo(this.ClubId, clubMember);
				if (!memberInfo.HasValue)
				{
					continue;
				}
				this.m_memberList.Add(new CommunityMember(this.ClubId, memberInfo.Value));
			}
		}

		public void RefreshStreams()
		{
			foreach (ClubStreamInfo stream in Club.GetStreams(this.ClubId))
			{
				this.AddStream(stream);
			}
			foreach (ClubStreamNotificationSetting clubStreamNotificationSetting in Club.GetClubStreamNotificationSettings(this.ClubId))
			{
				this.m_streamList[clubStreamNotificationSetting.streamId].Filter = clubStreamNotificationSetting;
			}
		}
	}
}