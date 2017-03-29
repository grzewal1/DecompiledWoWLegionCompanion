using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using WowJamMessages;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4861, Name="MobileClientShipmentsUpdate", Version=39869590)]
	public class MobileClientShipmentsUpdate
	{
		[DataMember(Name="shipment")]
		[FlexJamMember(ArrayDimensions=1, Name="shipment", Type=FlexJamType.Struct)]
		public JamCharacterShipment[] Shipment
		{
			get;
			set;
		}

		public MobileClientShipmentsUpdate()
		{
		}
	}
}