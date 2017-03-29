using bgs.types;
using bnet.protocol;
using System;
using System.Runtime.CompilerServices;

namespace bgs
{
	public class PartyId
	{
		public readonly static PartyId Empty;

		public ulong Hi
		{
			get;
			private set;
		}

		public bool IsEmpty
		{
			get
			{
				return (this.Hi != 0 ? false : this.Lo == (long)0);
			}
		}

		public ulong Lo
		{
			get;
			private set;
		}

		static PartyId()
		{
			PartyId.Empty = new PartyId((ulong)0, (ulong)0);
		}

		public PartyId()
		{
			ulong num = (ulong)0;
			this.Lo = num;
			this.Hi = num;
		}

		public PartyId(ulong highBits, ulong lowBits)
		{
			this.Set(highBits, lowBits);
		}

		public PartyId(bgs.types.EntityId partyEntityId)
		{
			this.Set(partyEntityId.hi, partyEntityId.lo);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is PartyId))
			{
				return this.Equals(obj);
			}
			return this == (PartyId)obj;
		}

		public static PartyId FromBnetEntityId(BnetEntityId entityId)
		{
			return new PartyId(entityId.GetHi(), entityId.GetLo());
		}

		public static PartyId FromEntityId(bgs.types.EntityId entityId)
		{
			return new PartyId(entityId);
		}

		public static PartyId FromProtocol(bnet.protocol.EntityId protoEntityId)
		{
			return new PartyId(protoEntityId.High, protoEntityId.Low);
		}

		public override int GetHashCode()
		{
			return this.Hi.GetHashCode() ^ this.Lo.GetHashCode();
		}

		public static bool operator ==(PartyId a, PartyId b)
		{
			if (a == null)
			{
				return b == null;
			}
			if (b == null)
			{
				return false;
			}
			return (a.Hi != b.Hi ? false : a.Lo == b.Lo);
		}

		public static implicit operator PartyId(BnetEntityId entityId)
		{
			if (entityId == null)
			{
				return null;
			}
			return new PartyId(entityId.GetHi(), entityId.GetLo());
		}

		public static bool operator !=(PartyId a, PartyId b)
		{
			return !(a == b);
		}

		public void Set(ulong highBits, ulong lowBits)
		{
			this.Hi = highBits;
			this.Lo = lowBits;
		}

		public bgs.types.EntityId ToEntityId()
		{
			bgs.types.EntityId entityId = new bgs.types.EntityId()
			{
				hi = this.Hi,
				lo = this.Lo
			};
			return entityId;
		}

		public override string ToString()
		{
			return string.Format("{0}-{1}", this.Hi, this.Lo);
		}
	}
}