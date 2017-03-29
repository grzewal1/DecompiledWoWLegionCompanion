using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamWhoRequestServerInfo", Version=28333852)]
	public class JamWhoRequestServerInfo
	{
		[DataMember(Name="factionGroup")]
		[FlexJamMember(Name="factionGroup", Type=FlexJamType.Int32)]
		public int FactionGroup
		{
			get;
			set;
		}

		[DataMember(Name="locale")]
		[FlexJamMember(Name="locale", Type=FlexJamType.Int32)]
		public int Locale
		{
			get;
			set;
		}

		[DataMember(Name="requesterVirtualRealmAddress")]
		[FlexJamMember(Name="requesterVirtualRealmAddress", Type=FlexJamType.UInt32)]
		public uint RequesterVirtualRealmAddress
		{
			get;
			set;
		}

		public JamWhoRequestServerInfo()
		{
		}
	}
}