using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.JSONRealmList
{
	[DataContract]
	[FlexJamStruct(Name="JamJSONRealmCharacterCount", Version=47212487)]
	public class JamJSONRealmCharacterCount
	{
		[DataMember(Name="count")]
		[FlexJamMember(Name="count", Type=FlexJamType.UInt8)]
		public byte Count
		{
			get;
			set;
		}

		[DataMember(Name="wowRealmAddress")]
		[FlexJamMember(Name="wowRealmAddress", Type=FlexJamType.UInt32)]
		public uint WowRealmAddress
		{
			get;
			set;
		}

		public JamJSONRealmCharacterCount()
		{
		}
	}
}