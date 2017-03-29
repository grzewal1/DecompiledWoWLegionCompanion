using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4863, Name="MobileClientCompleteShipmentResult", Version=39869590)]
	public class MobileClientCompleteShipmentResult
	{
		[DataMember(Name="result")]
		[FlexJamMember(Name="result", Type=FlexJamType.Int32)]
		public int Result
		{
			get;
			set;
		}

		[DataMember(Name="shipmentID")]
		[FlexJamMember(Name="shipmentID", Type=FlexJamType.UInt64)]
		public ulong ShipmentID
		{
			get;
			set;
		}

		public MobileClientCompleteShipmentResult()
		{
		}
	}
}