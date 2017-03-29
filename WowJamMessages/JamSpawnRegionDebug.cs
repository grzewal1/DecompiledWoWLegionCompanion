using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamSpawnRegionDebug", Version=28333852)]
	public class JamSpawnRegionDebug
	{
		[DataMember(Name="actual")]
		[FlexJamMember(Name="actual", Type=FlexJamType.Int32)]
		public int Actual
		{
			get;
			set;
		}

		[DataMember(Name="checkingThreshold")]
		[FlexJamMember(Name="checkingThreshold", Type=FlexJamType.Bool)]
		public bool CheckingThreshold
		{
			get;
			set;
		}

		[DataMember(Name="isFarmed")]
		[FlexJamMember(Name="isFarmed", Type=FlexJamType.Bool)]
		public bool IsFarmed
		{
			get;
			set;
		}

		[DataMember(Name="maxThreshold")]
		[FlexJamMember(Name="maxThreshold", Type=FlexJamType.Float)]
		public float MaxThreshold
		{
			get;
			set;
		}

		[DataMember(Name="minThreshold")]
		[FlexJamMember(Name="minThreshold", Type=FlexJamType.Float)]
		public float MinThreshold
		{
			get;
			set;
		}

		[DataMember(Name="numGroups")]
		[FlexJamMember(Name="numGroups", Type=FlexJamType.Int32)]
		public int NumGroups
		{
			get;
			set;
		}

		[DataMember(Name="numThresholdsHit")]
		[FlexJamMember(Name="numThresholdsHit", Type=FlexJamType.Int32)]
		public int NumThresholdsHit
		{
			get;
			set;
		}

		[DataMember(Name="pending")]
		[FlexJamMember(Name="pending", Type=FlexJamType.Int32)]
		public int Pending
		{
			get;
			set;
		}

		[DataMember(Name="players")]
		[FlexJamMember(ArrayDimensions=1, Name="players", Type=FlexJamType.Struct)]
		public JamSpawnRegionPlayerActivity[] Players
		{
			get;
			set;
		}

		[DataMember(Name="regionID")]
		[FlexJamMember(Name="regionID", Type=FlexJamType.Int32)]
		public int RegionID
		{
			get;
			set;
		}

		public JamSpawnRegionDebug()
		{
		}
	}
}