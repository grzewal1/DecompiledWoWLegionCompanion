using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[FlexJamMessage(Id = 4783, Name = "MobilePlayerGarrisonCompleteMission", Version = 33577221u), DataContract]
	public class MobilePlayerGarrisonCompleteMission
	{
		[FlexJamMember(Name = "garrMissionID", Type = FlexJamType.Int32), DataMember(Name = "garrMissionID")]
		public int GarrMissionID
		{
			get;
			set;
		}
	}
}
