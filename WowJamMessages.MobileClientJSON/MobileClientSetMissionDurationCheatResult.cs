using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[FlexJamMessage(Id = 4858, Name = "MobileClientSetMissionDurationCheatResult", Version = 33577221u), DataContract]
	public class MobileClientSetMissionDurationCheatResult
	{
		[FlexJamMember(Name = "success", Type = FlexJamType.Bool), DataMember(Name = "success")]
		public bool Success
		{
			get;
			set;
		}
	}
}
