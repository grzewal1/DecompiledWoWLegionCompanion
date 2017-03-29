using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using WowJamMessages;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4852, Name="MobileClientExpediteMissionCheatResult", Version=39869590)]
	public class MobileClientExpediteMissionCheatResult
	{
		[DataMember(Name="mission")]
		[FlexJamMember(Name="mission", Type=FlexJamType.Struct)]
		public JamGarrisonMobileMission Mission
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

		[DataMember(Name="result")]
		[FlexJamMember(Name="result", Type=FlexJamType.Int32)]
		public int Result
		{
			get;
			set;
		}

		public MobileClientExpediteMissionCheatResult()
		{
		}
	}
}