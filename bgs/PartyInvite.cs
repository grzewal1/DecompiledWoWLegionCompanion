using System;

namespace bgs
{
	public class PartyInvite
	{
		public ulong InviteId;

		public bgs.PartyId PartyId;

		public bgs.PartyType PartyType;

		public string InviterName;

		public BnetGameAccountId InviterId;

		public BnetGameAccountId InviteeId;

		private PartyInvite.Flags InviteFlags;

		public bool IsRejoin
		{
			get
			{
				return (this.InviteFlags & PartyInvite.Flags.RESERVATION) == PartyInvite.Flags.RESERVATION;
			}
		}

		public bool IsReservation
		{
			get
			{
				return (this.InviteFlags & PartyInvite.Flags.RESERVATION) == PartyInvite.Flags.RESERVATION;
			}
		}

		public PartyInvite()
		{
		}

		public PartyInvite(ulong inviteId, bgs.PartyId partyId, bgs.PartyType type)
		{
			this.InviteId = inviteId;
			this.PartyId = partyId;
			this.PartyType = type;
		}

		public uint GetFlags()
		{
			return (uint)this.InviteFlags;
		}

		public void SetFlags(uint flagsValue)
		{
			this.InviteFlags = (PartyInvite.Flags)flagsValue;
		}

		[Flags]
		public enum Flags
		{
			REJOIN = 1,
			RESERVATION = 1
		}
	}
}