using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4795, Name="MobilePlayerEvaluateMission", Version=38820897)]
	public class MobilePlayerEvaluateMission
	{
		[DataMember(Name="garrFollowerID")]
		[FlexJamMember(ArrayDimensions=1, Name="garrFollowerID", Type=FlexJamType.Int32)]
		public int[] GarrFollowerID
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

		public MobilePlayerEvaluateMission()
		{
		}
	}
}