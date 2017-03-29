using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using WowJamMessages;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4848, Name="MobileClientMissionAdded", Version=39869590)]
	public class MobileClientMissionAdded
	{
		[DataMember(Name="canStartMission")]
		[FlexJamMember(Name="canStartMission", Type=FlexJamType.Bool)]
		public bool CanStartMission
		{
			get;
			set;
		}

		[DataMember(Name="garrTypeID")]
		[FlexJamMember(Name="garrTypeID", Type=FlexJamType.Int32)]
		public int GarrTypeID
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

		[DataMember(Name="missionSource")]
		[FlexJamMember(Name="missionSource", Type=FlexJamType.UInt8)]
		public byte MissionSource
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

		public MobileClientMissionAdded()
		{
		}
	}
}