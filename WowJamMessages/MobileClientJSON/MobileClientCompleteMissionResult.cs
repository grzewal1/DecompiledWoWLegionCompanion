using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using WowJamMessages;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4846, Name="MobileClientCompleteMissionResult", Version=39869590)]
	public class MobileClientCompleteMissionResult
	{
		[DataMember(Name="bonusRollSucceeded")]
		[FlexJamMember(Name="bonusRollSucceeded", Type=FlexJamType.Bool)]
		public bool BonusRollSucceeded
		{
			get;
			set;
		}

		[DataMember(Name="followerInfo")]
		[FlexJamMember(ArrayDimensions=1, Name="followerInfo", Type=FlexJamType.Struct)]
		public JamGarrisonMissionFollowerInfo[] FollowerInfo
		{
			get;
			set;
		}

		[DataMember(Name="garrMissionID")]
		[FlexJamMember(Name="garrMissionID", Type=FlexJamType.Int32)]
		public int GarrMissionID
		{
			get;
			set;
		}

		[DataMember(Name="mission")]
		[FlexJamMember(Name="mission", Type=FlexJamType.Struct)]
		public JamGarrisonMobileMission Mission
		{
			get;
			set;
		}

		[DataMember(Name="missionSuccessChance")]
		[FlexJamMember(Name="missionSuccessChance", Type=FlexJamType.UInt8)]
		public byte MissionSuccessChance
		{
			get;
			set;
		}

		[DataMember(Name="result")]
		[FlexJamMember(Name="result", Type=FlexJamType.Int32)]
		public int Result
		{
			get;
			set;
		}

		public MobileClientCompleteMissionResult()
		{
		}
	}
}