using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4845, Name="MobileClientStartMissionResult", Version=39869590)]
	public class MobileClientStartMissionResult
	{
		[DataMember(Name="garrMissionID")]
		[FlexJamMember(Name="garrMissionID", Type=FlexJamType.Int32)]
		public int GarrMissionID
		{
			get;
			set;
		}

		[DataMember(Name="newDailyMissionCounter")]
		[FlexJamMember(Name="newDailyMissionCounter", Type=FlexJamType.UInt16)]
		public ushort NewDailyMissionCounter
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

		public MobileClientStartMissionResult()
		{
		}
	}
}