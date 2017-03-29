using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4862, Name="MobileClientShipmentTypes", Version=39869590)]
	public class MobileClientShipmentTypes
	{
		[DataMember(Name="shipment")]
		[FlexJamMember(ArrayDimensions=1, Name="shipment", Type=FlexJamType.Struct)]
		public MobileClientShipmentType[] Shipment
		{
			get;
			set;
		}

		public MobileClientShipmentTypes()
		{
		}
	}
}