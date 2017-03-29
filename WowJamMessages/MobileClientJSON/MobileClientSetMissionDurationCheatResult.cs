using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4868, Name="MobileClientSetMissionDurationCheatResult", Version=39869590)]
	public class MobileClientSetMissionDurationCheatResult
	{
		[DataMember(Name="success")]
		[FlexJamMember(Name="success", Type=FlexJamType.Bool)]
		public bool Success
		{
			get;
			set;
		}

		public MobileClientSetMissionDurationCheatResult()
		{
		}
	}
}