using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamSpawnRegionPlayerActivity", Version=28333852)]
	public class JamSpawnRegionPlayerActivity
	{
		[DataMember(Name="activeTime")]
		[FlexJamMember(Name="activeTime", Type=FlexJamType.UInt32)]
		public uint ActiveTime
		{
			get;
			set;
		}

		[DataMember(Name="idleTime")]
		[FlexJamMember(Name="idleTime", Type=FlexJamType.UInt32)]
		public uint IdleTime
		{
			get;
			set;
		}

		[DataMember(Name="player")]
		[FlexJamMember(Name="player", Type=FlexJamType.WowGuid)]
		public string Player
		{
			get;
			set;
		}

		public JamSpawnRegionPlayerActivity()
		{
		}
	}
}