using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace JamLib
{
	[DataContract]
	[FlexJamStruct(Name="JamServerSpec")]
	public struct JamServerSpec : IComparable<JamServerSpec>
	{
		[DataMember(Name="realm")]
		[FlexJamMember(Name="realm", Type=FlexJamType.UInt32)]
		public uint RealmAddress
		{
			get;
			set;
		}

		[DataMember(Name="server")]
		[FlexJamMember(Name="server", Type=FlexJamType.UInt32)]
		public uint ServerID
		{
			get;
			set;
		}

		[DataMember(Name="type")]
		[FlexJamMember(Name="type", Type=FlexJamType.Int32)]
		public JAM_DESTINATION ServerType
		{
			get;
			set;
		}

		public int CompareTo(JamServerSpec other)
		{
			if (this.RealmAddress < other.RealmAddress)
			{
				return -1;
			}
			if (this.RealmAddress > other.RealmAddress)
			{
				return 1;
			}
			if (this.ServerType < other.ServerType)
			{
				return -1;
			}
			if (this.ServerType > other.ServerType)
			{
				return 1;
			}
			if (this.ServerID < other.ServerID)
			{
				return -1;
			}
			if (this.ServerID > other.ServerID)
			{
				return 1;
			}
			return 0;
		}

		public override bool Equals(object obj)
		{
			return (!(obj is JamServerSpec) ? false : (JamServerSpec)obj == this);
		}

		public override int GetHashCode()
		{
			return (int)(this.RealmAddress ^ (uint)this.ServerType ^ this.ServerID);
		}

		public bool IsRoutable()
		{
			return ((ulong)this.ServerType >= (long)WowConstants.NUM_JAM_DESTINATION_TYPES ? false : this.RealmAddress != 0);
		}

		public bool IsValid()
		{
			return ((ulong)this.ServerType >= (long)WowConstants.NUM_JAM_DESTINATION_TYPES || this.ServerID == 0 ? false : this.RealmAddress != 0);
		}

		public static bool operator ==(JamServerSpec a, JamServerSpec b)
		{
			return (a.RealmAddress != b.RealmAddress || a.ServerType != b.ServerType ? false : a.ServerID == b.ServerID);
		}

		public static bool operator >(JamServerSpec a, JamServerSpec b)
		{
			return a.CompareTo(b) > 0;
		}

		public static bool operator >=(JamServerSpec a, JamServerSpec b)
		{
			return a.CompareTo(b) >= 0;
		}

		public static bool operator !=(JamServerSpec a, JamServerSpec b)
		{
			return !(a == b);
		}

		public static bool operator <(JamServerSpec a, JamServerSpec b)
		{
			return a.CompareTo(b) < 0;
		}

		public static bool operator <=(JamServerSpec a, JamServerSpec b)
		{
			return a.CompareTo(b) <= 0;
		}

		public override string ToString()
		{
			return string.Format("{0:x}:{1}:{2}", this.RealmAddress, (int)this.ServerType, this.ServerID);
		}
	}
}