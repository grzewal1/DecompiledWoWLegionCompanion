using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.JSONRealmList
{
	[DataContract]
	[FlexJamStruct(Name="JamJSONCharacterEntry", Version=28333852)]
	public class JamJSONCharacterEntry
	{
		[DataMember(Name="classID")]
		[FlexJamMember(Name="classID", Type=FlexJamType.UInt8)]
		public byte ClassID
		{
			get;
			set;
		}

		[DataMember(Name="experienceLevel")]
		[FlexJamMember(Name="experienceLevel", Type=FlexJamType.UInt8)]
		public byte ExperienceLevel
		{
			get;
			set;
		}

		[DataMember(Name="hasMobileAccess")]
		[FlexJamMember(Name="hasMobileAccess", Type=FlexJamType.Bool)]
		public bool HasMobileAccess
		{
			get;
			set;
		}

		[DataMember(Name="lastActiveTime")]
		[FlexJamMember(Name="lastActiveTime", Type=FlexJamType.Int32)]
		public int LastActiveTime
		{
			get;
			set;
		}

		[DataMember(Name="name")]
		[FlexJamMember(Name="name", Type=FlexJamType.String)]
		public string Name
		{
			get;
			set;
		}

		[DataMember(Name="playerGuid")]
		[FlexJamMember(Name="playerGuid", Type=FlexJamType.WowGuid)]
		public string PlayerGuid
		{
			get;
			set;
		}

		[DataMember(Name="raceID")]
		[FlexJamMember(Name="raceID", Type=FlexJamType.UInt8)]
		public byte RaceID
		{
			get;
			set;
		}

		[DataMember(Name="sexID")]
		[FlexJamMember(Name="sexID", Type=FlexJamType.UInt8)]
		public byte SexID
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

		public JamJSONCharacterEntry()
		{
		}
	}
}