using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.JSONRealmList
{
	[DataContract]
	[FlexJamStruct(Name="JamJSONRealmListLoginHistoryEntry", Version=47212487)]
	public class JamJSONRealmListLoginHistoryEntry
	{
		[DataMember(Name="characterGUID")]
		[FlexJamMember(Name="characterGUID", Type=FlexJamType.WowGuid)]
		public string CharacterGUID
		{
			get;
			set;
		}

		[DataMember(Name="lastPlayedTime")]
		[FlexJamMember(Name="lastPlayedTime", Type=FlexJamType.Int32)]
		public int LastPlayedTime
		{
			get;
			set;
		}

		[DataMember(Name="virtualRealmAddress")]
		[FlexJamMember(Name="virtualRealmAddress", Type=FlexJamType.UInt32)]
		public uint VirtualRealmAddress
		{
			get;
			set;
		}

		public JamJSONRealmListLoginHistoryEntry()
		{
		}
	}
}