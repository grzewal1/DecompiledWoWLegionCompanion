using System;

namespace bgs
{
	public class PartyMember : OnlinePlayer
	{
		public uint[] RoleIds = new uint[0];

		public PartyMember()
		{
		}

		public static uint GetLeaderRoleId(PartyType partyType)
		{
			PartyType partyType1 = partyType;
			if (partyType1 != PartyType.FRIENDLY_CHALLENGE)
			{
				if (partyType1 == PartyType.SPECTATOR_PARTY)
				{
					return Convert.ToUInt32(BnetParty.SpectatorPartyRoleSet.Leader);
				}
			}
			return Convert.ToUInt32(BnetParty.FriendlyGameRoleSet.Inviter);
		}

		public bool HasRole(Enum role)
		{
			return this.HasRole(Convert.ToUInt32(role));
		}

		public bool HasRole(uint roleId)
		{
			if (this.RoleIds == null)
			{
				return false;
			}
			for (int i = 0; i < (int)this.RoleIds.Length; i++)
			{
				if (this.RoleIds[i] == roleId)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsLeader(PartyType partyType)
		{
			return this.HasRole(PartyMember.GetLeaderRoleId(partyType));
		}
	}
}