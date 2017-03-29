using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamGarrisonMission", Version=28333852)]
	public class JamGarrisonMission
	{
		[DataMember(Name="dbID")]
		[FlexJamMember(Name="dbID", Type=FlexJamType.UInt64)]
		public ulong DbID
		{
			get;
			set;
		}

		[DataMember(Name="flags")]
		[FlexJamMember(Name="flags", Type=FlexJamType.UInt32)]
		public uint Flags
		{
			get;
			set;
		}

		[DataMember(Name="missionDuration")]
		[FlexJamMember(Name="missionDuration", Type=FlexJamType.Int32)]
		public int MissionDuration
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
		[FlexJamMember(Name="offerDuration", Type=FlexJamType.Int32)]
		public int OfferDuration
		{
			get;
			set;
		}

		[DataMember(Name="offerTime")]
		[FlexJamMember(Name="offerTime", Type=FlexJamType.Int32)]
		public int OfferTime
		{
			get;
			set;
		}

		[DataMember(Name="startTime")]
		[FlexJamMember(Name="startTime", Type=FlexJamType.Int32)]
		public int StartTime
		{
			get;
			set;
		}

		[DataMember(Name="successChance")]
		[FlexJamMember(Name="successChance", Type=FlexJamType.Int32)]
		public int SuccessChance
		{
			get;
			set;
		}

		[DataMember(Name="travelDuration")]
		[FlexJamMember(Name="travelDuration", Type=FlexJamType.Int32)]
		public int TravelDuration
		{
			get;
			set;
		}

		public JamGarrisonMission()
		{
			this.Flags = 0;
		}
	}
}