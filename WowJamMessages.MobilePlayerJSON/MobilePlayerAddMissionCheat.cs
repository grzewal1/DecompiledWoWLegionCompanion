using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[FlexJamMessage(Id = 4786, Name = "MobilePlayerAddMissionCheat", Version = 33577221u), DataContract]
	public class MobilePlayerAddMissionCheat
	{
		[FlexJamMember(Name = "garrMissionID", Type = FlexJamType.Int32), DataMember(Name = "garrMissionID")]
		public int GarrMissionID
		{
			get;
			set;
		}
	}
}
