using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[FlexJamMessage(Id = 4859, Name = "MobileClientSetShipmentDurationCheatResult", Version = 33577221u), DataContract]
	public class MobileClientSetShipmentDurationCheatResult
	{
		[FlexJamMember(Name = "success", Type = FlexJamType.Bool), DataMember(Name = "success")]
		public bool Success
		{
			get;
			set;
		}
	}
}
