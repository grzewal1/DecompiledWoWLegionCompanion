using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4793, Name="MobilePlayerCompleteShipment", Version=38820897)]
	public class MobilePlayerCompleteShipment
	{
		[DataMember(Name="shipmentID")]
		[FlexJamMember(Name="shipmentID", Type=FlexJamType.UInt64)]
		public ulong ShipmentID
		{
			get;
			set;
		}

		public MobilePlayerCompleteShipment()
		{
		}
	}
}