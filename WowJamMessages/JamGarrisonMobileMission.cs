using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamGarrisonMobileMission", Version=28333852)]
	public class JamGarrisonMobileMission
	{
		[DataMember(Name="dbID")]
		[FlexJamMember(Name="dbID", Type=FlexJamType.UInt64)]
		public ulong DbID
		{
			get;
			set;
		}

		[DataMember(Name="encounter")]
		[FlexJamMember(ArrayDimensions=1, Name="encounter", Type=FlexJamType.Struct)]
		public JamGarrisonEncounter[] Encounter
		{
			get;
			set;
		}

		[DataMember(Name="missionDuration")]
		[FlexJamMember(Name="missionDuration", Type=FlexJamType.Int64)]
		public long MissionDuration
		{
			get;
			set;
		}

		[DataMember(Name="missionRecID")]
		[FlexJamMember(Name="missionRecID", Type=FlexJamType.Int32)]
		public int MissionRecID
		{
			get;
			set;
		}

		[DataMember(Name="missionState")]
		[FlexJamMember(Name="missionState", Type=FlexJamType.Int32)]
		public int MissionState
		{
			get;
			set;
		}

		[DataMember(Name="offerDuration")]
		[FlexJamMember(Name="offerDuration", Type=FlexJamType.Int64)]
		public long OfferDuration
		{
			get;
			set;
		}

		[DataMember(Name="offerTime")]
		[FlexJamMember(Name="offerTime", Type=FlexJamType.Int64)]
		public long OfferTime
		{
			get;
			set;
		}

		[DataMember(Name="overmaxReward")]
		[FlexJamMember(ArrayDimensions=1, Name="overmaxReward", Type=FlexJamType.Struct)]
		public JamGarrisonMissionReward[] OvermaxReward
		{
			get;
			set;
		}

		[DataMember(Name="reward")]
		[FlexJamMember(ArrayDimensions=1, Name="reward", Type=FlexJamType.Struct)]
		public JamGarrisonMissionReward[] Reward
		{
			get;
			set;
		}

		[DataMember(Name="startTime")]
		[FlexJamMember(Name="startTime", Type=FlexJamType.Int64)]
		public long StartTime
		{
			get;
			set;
		}

		[DataMember(Name="travelDuration")]
		[FlexJamMember(Name="travelDuration", Type=FlexJamType.Int64)]
		public long TravelDuration
		{
			get;
			set;
		}

		public JamGarrisonMobileMission()
		{
		}
	}
}