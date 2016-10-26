using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[FlexJamMessage(Id = 4834, Name = "MobileClientUpdateDailyMissionCounter", Version = 33577221u), DataContract]
	public class MobileClientUpdateDailyMissionCounter
	{
		[FlexJamMember(Name = "garrTypeID", Type = FlexJamType.Int32), DataMember(Name = "garrTypeID")]
		public int GarrTypeID
		{
			get;
			set;
		}

		[FlexJamMember(Name = "count", Type = FlexJamType.UInt16), DataMember(Name = "count")]
		public ushort Count
		{
			get;
			set;
		}
	}
}
