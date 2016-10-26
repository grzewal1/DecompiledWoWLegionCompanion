using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[FlexJamMessage(Id = 4794, Name = "MobilePlayerCompleteShipment", Version = 33577221u), DataContract]
	public class MobilePlayerCompleteShipment
	{
		[FlexJamMember(Name = "shipmentID", Type = FlexJamType.UInt64), DataMember(Name = "shipmentID")]
		public ulong ShipmentID
		{
			get;
			set;
		}
	}
}
