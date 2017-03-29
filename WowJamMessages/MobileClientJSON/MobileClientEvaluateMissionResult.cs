using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4867, Name="MobileClientEvaluateMissionResult", Version=39869590)]
	public class MobileClientEvaluateMissionResult
	{
		[DataMember(Name="garrMissionID")]
		[FlexJamMember(Name="garrMissionID", Type=FlexJamType.Int32)]
		public int GarrMissionID
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

		[DataMember(Name="successChance")]
		[FlexJamMember(Name="successChance", Type=FlexJamType.Int32)]
		public int SuccessChance
		{
			get;
			set;
		}

		public MobileClientEvaluateMissionResult()
		{
		}
	}
}