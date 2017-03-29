using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamWhoRequest", Version=28333852)]
	public class JamWhoRequest
	{
		[DataMember(Name="classFilter")]
		[FlexJamMember(Name="classFilter", Type=FlexJamType.Int32)]
		public int ClassFilter
		{
			get;
			set;
		}

		[DataMember(Name="exactName")]
		[FlexJamMember(Name="exactName", Type=FlexJamType.Bool)]
		public bool ExactName
		{
			get;
			set;
		}

		[DataMember(Name="guild")]
		[FlexJamMember(Name="guild", Type=FlexJamType.String)]
		public string Guild
		{
			get;
			set;
		}

		[DataMember(Name="guildVirtualRealmName")]
		[FlexJamMember(Name="guildVirtualRealmName", Type=FlexJamType.String)]
		public string GuildVirtualRealmName
		{
			get;
			set;
		}

		[DataMember(Name="maxLevel")]
		[FlexJamMember(Name="maxLevel", Type=FlexJamType.Int32)]
		public int MaxLevel
		{
			get;
			set;
		}

		[DataMember(Name="minLevel")]
		[FlexJamMember(Name="minLevel", Type=FlexJamType.Int32)]
		public int MinLevel
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

		[DataMember(Name="raceFilter")]
		[FlexJamMember(Name="raceFilter", Type=FlexJamType.Int32)]
		public int RaceFilter
		{
			get;
			set;
		}

		[DataMember(Name="serverInfo")]
		[FlexJamMember(Optional=true, Name="serverInfo", Type=FlexJamType.Struct)]
		public JamWhoRequestServerInfo[] ServerInfo
		{
			get;
			set;
		}

		[DataMember(Name="showArenaPlayers")]
		[FlexJamMember(Name="showArenaPlayers", Type=FlexJamType.Bool)]
		public bool ShowArenaPlayers
		{
			get;
			set;
		}

		[DataMember(Name="showEnemies")]
		[FlexJamMember(Name="showEnemies", Type=FlexJamType.Bool)]
		public bool ShowEnemies
		{
			get;
			set;
		}

		[DataMember(Name="virtualRealmName")]
		[FlexJamMember(Name="virtualRealmName", Type=FlexJamType.String)]
		public string VirtualRealmName
		{
			get;
			set;
		}

		[DataMember(Name="words")]
		[FlexJamMember(ArrayDimensions=1, Name="words", Type=FlexJamType.Struct)]
		public JamWhoWord[] Words
		{
			get;
			set;
		}

		public JamWhoRequest()
		{
		}
	}
}