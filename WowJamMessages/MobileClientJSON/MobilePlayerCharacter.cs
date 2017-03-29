using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamStruct(Name="MobilePlayerCharacter", Version=39869590)]
	public class MobilePlayerCharacter
	{
		[DataMember(Name="charClass")]
		[FlexJamMember(Name="charClass", Type=FlexJamType.UInt8)]
		public byte CharClass
		{
			get;
			set;
		}

		[DataMember(Name="charLevel")]
		[FlexJamMember(Name="charLevel", Type=FlexJamType.UInt8)]
		public byte CharLevel
		{
			get;
			set;
		}

		[DataMember(Name="charRace")]
		[FlexJamMember(Name="charRace", Type=FlexJamType.UInt8)]
		public byte CharRace
		{
			get;
			set;
		}

		[DataMember(Name="guid")]
		[FlexJamMember(Name="guid", Type=FlexJamType.WowGuid)]
		public string Guid
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

		[DataMember(Name="status")]
		[FlexJamMember(Name="status", Type=FlexJamType.Int32)]
		public int Status
		{
			get;
			set;
		}

		public MobilePlayerCharacter()
		{
		}
	}
}