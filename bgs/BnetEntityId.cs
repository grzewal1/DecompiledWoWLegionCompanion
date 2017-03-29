using bgs.types;
using bnet.protocol;
using System;

namespace bgs
{
	public class BnetEntityId
	{
		protected ulong m_hi;

		protected ulong m_lo;

		public BnetEntityId()
		{
		}

		public BnetEntityId Clone()
		{
			return (BnetEntityId)this.MemberwiseClone();
		}

		public void CopyFrom(bgs.types.EntityId id)
		{
			this.m_hi = id.hi;
			this.m_lo = id.lo;
		}

		public void CopyFrom(BnetEntityId id)
		{
			this.m_hi = id.m_hi;
			this.m_lo = id.m_lo;
		}

		public static bgs.types.EntityId CreateEntityId(BnetEntityId src)
		{
			bgs.types.EntityId entityId = new bgs.types.EntityId()
			{
				hi = src.m_hi,
				lo = src.m_lo
			};
			return entityId;
		}

		public static bnet.protocol.EntityId CreateForProtocol(BnetEntityId src)
		{
			bnet.protocol.EntityId entityId = new bnet.protocol.EntityId();
			entityId.SetLow(src.GetLo());
			entityId.SetHigh(src.GetHi());
			return entityId;
		}

		public static BnetEntityId CreateFromEntityId(bgs.types.EntityId src)
		{
			BnetEntityId bnetEntityId = new BnetEntityId();
			bnetEntityId.CopyFrom(src);
			return bnetEntityId;
		}

		public static BnetEntityId CreateFromProtocol(bnet.protocol.EntityId src)
		{
			BnetEntityId bnetEntityId = new BnetEntityId();
			bnetEntityId.SetLo(src.Low);
			bnetEntityId.SetHi(src.High);
			return bnetEntityId;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			BnetEntityId bnetEntityId = obj as BnetEntityId;
			if (bnetEntityId == null)
			{
				return false;
			}
			return (this.m_hi != bnetEntityId.m_hi ? false : this.m_lo == bnetEntityId.m_lo);
		}

		public bool Equals(BnetEntityId other)
		{
			if (other == null)
			{
				return false;
			}
			return (this.m_hi != other.m_hi ? false : this.m_lo == other.m_lo);
		}

		public override int GetHashCode()
		{
			int hashCode = 17;
			hashCode = hashCode * 11 + this.m_hi.GetHashCode();
			hashCode = hashCode * 11 + this.m_lo.GetHashCode();
			return hashCode;
		}

		public ulong GetHi()
		{
			return this.m_hi;
		}

		public ulong GetLo()
		{
			return this.m_lo;
		}

		public bool IsEmpty()
		{
			return (this.m_hi | this.m_lo) == (long)0;
		}

		public bool IsValid()
		{
			return this.m_lo != (long)0;
		}

		public static bool operator ==(BnetEntityId a, BnetEntityId b)
		{
			if (object.ReferenceEquals(a, b))
			{
				return true;
			}
			if (a == null || b == null)
			{
				return false;
			}
			return (a.m_hi != b.m_hi ? false : a.m_lo == b.m_lo);
		}

		public static bool operator !=(BnetEntityId a, BnetEntityId b)
		{
			return !(a == b);
		}

		public void SetHi(ulong hi)
		{
			this.m_hi = hi;
		}

		public void SetLo(ulong lo)
		{
			this.m_lo = lo;
		}

		public override string ToString()
		{
			return string.Format("[hi={0} lo={1}]", this.m_hi, this.m_lo);
		}
	}
}