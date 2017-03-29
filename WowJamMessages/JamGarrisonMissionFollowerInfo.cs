using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamGarrisonMissionFollowerInfo", Version=28333852)]
	public class JamGarrisonMissionFollowerInfo
	{
		[DataMember(Name="followerDBID")]
		[FlexJamMember(Name="followerDBID", Type=FlexJamType.UInt64)]
		public ulong FollowerDBID
		{
			get;
			set;
		}

		[DataMember(Name="missionCompleteState")]
		[FlexJamMember(Name="missionCompleteState", Type=FlexJamType.UInt32)]
		public uint MissionCompleteState
		{
			get;
			set;
		}

		public JamGarrisonMissionFollowerInfo()
		{
		}
	}
}