using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[FlexJamMessage(Id = 4781, Name = "MobilePlayerGarrisonDataRequest", Version = 33577221u), DataContract]
	public class MobilePlayerGarrisonDataRequest
	{
		[FlexJamMember(Name = "garrTypeID", Type = FlexJamType.Int32), DataMember(Name = "garrTypeID")]
		public int GarrTypeID
		{
			get;
			set;
		}
	}
}
