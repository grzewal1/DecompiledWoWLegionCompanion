using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4792, Name="MobilePlayerRequestShipmentTypes", Version=38820897)]
	public class MobilePlayerRequestShipmentTypes
	{
		public MobilePlayerRequestShipmentTypes()
		{
		}
	}
}