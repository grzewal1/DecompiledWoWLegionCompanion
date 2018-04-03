using System;
using System.Runtime.CompilerServices;

namespace bgs
{
	public struct Error
	{
		public uint Code
		{
			get
			{
				return (uint)this.EnumVal;
			}
		}

		public BattleNetErrors EnumVal
		{
			get;
			private set;
		}

		public string Name
		{
			get
			{
				return this.EnumVal.ToString();
			}
		}

		public Error(BattleNetErrors code)
		{
			this = new Error()
			{
				EnumVal = code
			};
		}

		public override bool Equals(object obj)
		{
			if (obj is BattleNetErrors)
			{
				return this.EnumVal == (BattleNetErrors)obj;
			}
			if (obj is Error)
			{
				return this.EnumVal == ((Error)obj).EnumVal;
			}
			if (obj is int)
			{
				return (int)this.EnumVal == (int)obj;
			}
			if (obj is uint)
			{
				return (uint)this.EnumVal == (uint)obj;
			}
			if (obj is long)
			{
				return (ulong)this.EnumVal == (long)obj;
			}
			if (!(obj is ulong))
			{
				return this.Equals(obj);
			}
			return (ulong)this.EnumVal == (ulong)obj;
		}

		public override int GetHashCode()
		{
			return this.Code.GetHashCode();
		}

		public static bool operator ==(Error a, BattleNetErrors b)
		{
			return a.EnumVal == b;
		}

		public static bool operator ==(Error a, uint b)
		{
			return (uint)a.EnumVal == b;
		}

		public static implicit operator Error(BattleNetErrors code)
		{
			return new Error(code);
		}

		public static implicit operator Error(uint code)
		{
			return new Error((BattleNetErrors)code);
		}

		public static bool operator !=(Error a, BattleNetErrors b)
		{
			return a.EnumVal != b;
		}

		public static bool operator !=(Error a, uint b)
		{
			return (uint)a.EnumVal != b;
		}

		public override string ToString()
		{
			return string.Format("{0} {1}", this.Code, this.Name);
		}
	}
}