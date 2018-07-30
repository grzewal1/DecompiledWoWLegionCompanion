using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WoWCompanionApp
{
	public class CommunityMember
	{
		private ulong m_clubId;

		private ClubMemberInfo m_clubMember;

		private const string OWNER_STRING_KEY = "COMMUNITY_MEMBER_ROLE_NAME_OWNER";

		private const string LEADER_STRING_KEY = "COMMUNITY_MEMBER_ROLE_NAME_LEADER";

		private const string MODERATOR_STRING_KEY = "COMMUNITY_MEMBER_ROLE_NAME_MODERATOR";

		private const string MEMBER_STRING_KEY = "COMMUNITY_MEMBER_ROLE_NAME_MEMBER";

		public uint Class
		{
			get
			{
				return this.m_clubMember.classID.Value;
			}
		}

		public ulong ClubID
		{
			get
			{
				return this.m_clubId;
			}
		}

		public uint MemberID
		{
			get
			{
				return this.m_clubMember.memberId;
			}
		}

		public string Name
		{
			get
			{
				return this.m_clubMember.name;
			}
		}

		public ClubMemberPresence Presence
		{
			get
			{
				return this.m_clubMember.presence;
			}
		}

		public ClubRoleIdentifier Role
		{
			get
			{
				return this.m_clubMember.role.Value;
			}
		}

		public CommunityMember(ulong clubId, ClubMemberInfo clubMember)
		{
			this.m_clubId = clubId;
			this.m_clubMember = clubMember;
		}

		public void AssignRole(ClubRoleIdentifier newRole)
		{
			Club.AssignMemberRole(this.m_clubId, this.m_clubMember.memberId, newRole);
		}

		public string ConvertRoleToString()
		{
			if (this.m_clubMember.role.HasValue)
			{
				switch (this.m_clubMember.role.Value)
				{
					case ClubRoleIdentifier.Owner:
					{
						return StaticDB.GetString("COMMUNITY_MEMBER_ROLE_NAME_OWNER", "[PH] Owner");
					}
					case ClubRoleIdentifier.Leader:
					{
						return StaticDB.GetString("COMMUNITY_MEMBER_ROLE_NAME_LEADER", "[PH] Leader");
					}
					case ClubRoleIdentifier.Moderator:
					{
						return StaticDB.GetString("COMMUNITY_MEMBER_ROLE_NAME_MODERATOR", "[PH] Moderator");
					}
					case ClubRoleIdentifier.Member:
					{
						return StaticDB.GetString("COMMUNITY_MEMBER_ROLE_NAME_MEMBER", "[PH] Member");
					}
				}
			}
			return string.Empty;
		}

		public List<ClubRoleIdentifier> GetAssignableRoles()
		{
			return Club.GetAssignableRoles(this.m_clubId, this.m_clubMember.memberId);
		}

		public void HandleMemberUpdatedEvent(Club.ClubMemberUpdatedEvent memberUpdate)
		{
			ClubMemberInfo? memberInfo = Club.GetMemberInfo(this.m_clubId, this.MemberID);
			if (memberInfo.HasValue)
			{
				this.m_clubMember = memberInfo.Value;
			}
		}

		public void HandlePresenceUpdateEvent(Club.ClubMemberPresenceUpdatedEvent presenceUpdate)
		{
			this.m_clubMember.presence = presenceUpdate.Presence;
		}

		public void HandleRoleUpdateEvent(Club.ClubMemberRoleUpdatedEvent roleUpdate)
		{
			ClubMemberInfo? memberInfo = Club.GetMemberInfo(this.m_clubId, this.MemberID);
			if (memberInfo.HasValue)
			{
				this.m_clubMember = memberInfo.Value;
			}
		}

		public bool IsKickable()
		{
			ClubPrivilegeInfo clubPrivileges = Club.GetClubPrivileges(this.m_clubId);
			return clubPrivileges.kickableRoleIds.Exists((int id) => id == (int)this.Role);
		}

		public void KickMember()
		{
			Club.KickMember(this.m_clubId, this.m_clubMember.memberId);
		}
	}
}