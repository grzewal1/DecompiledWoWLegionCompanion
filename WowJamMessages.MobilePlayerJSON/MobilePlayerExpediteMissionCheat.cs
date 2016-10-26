using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[FlexJamMessage(Id = 4787, Name = "MobilePlayerExpediteMissionCheat", Version = 33577221u), DataContract]
	public class MobilePlayerExpediteMissionCheat
	{
		[FlexJamMember(Name = "garrMissionID", Type = FlexJamType.Int32), DataMember(Name = "garrMissionID")]
		public int GarrMissionID
		{
			get;
			set;
		}
	}
}
