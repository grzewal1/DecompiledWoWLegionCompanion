using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamWhoEntry", Version=28333852)]
	public class JamWhoEntry
	{
		[DataMember(Name="areaID")]
		[FlexJamMember(Name="areaID", Type=FlexJamType.Int32)]
		public int AreaID
		{
			get;
			set;
		}

		[DataMember(Name="guildGUID")]
		[FlexJamMember(Name="guildGUID", Type=FlexJamType.WowGuid)]
		public string GuildGUID
		{
			get;
			set;
		}

		[DataMember(Name="guildName")]
		[FlexJamMember(Name="guildName", Type=FlexJamType.String)]
		public string GuildName
		{
			get;
			set;
		}

		[DataMember(Name="guildVirtualRealmAddress")]
		[FlexJamMember(Name="guildVirtualRealmAddress", Type=FlexJamType.UInt32)]
		public uint GuildVirtualRealmAddress
		{
			get;
			set;
		}

		[DataMember(Name="isGM")]
		[FlexJamMember(Name="isGM", Type=FlexJamType.Bool)]
		public bool IsGM
		{
			get;
			set;
		}

		[DataMember(Name="playerData")]
		[FlexJamMember(Name="playerData", Type=FlexJamType.Struct)]
		public JamPlayerGuidLookupData PlayerData
		{
			get;
			set;
		}

		public JamWhoEntry()
		{
		}
	}
}