using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4791, Name="MobilePlayerRequestShipments", Version=38820897)]
	public class MobilePlayerRequestShipments
	{
		public MobilePlayerRequestShipments()
		{
		}
	}
}