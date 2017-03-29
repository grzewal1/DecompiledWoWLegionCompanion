using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4790, Name="MobilePlayerCreateShipment", Version=38820897)]
	public class MobilePlayerCreateShipment
	{
		[DataMember(Name="charShipmentID")]
		[FlexJamMember(Name="charShipmentID", Type=FlexJamType.Int32)]
		public int CharShipmentID
		{
			get;
			set;
		}

		[DataMember(Name="numShipments")]
		[FlexJamMember(Name="numShipments", Type=FlexJamType.Int32)]
		public int NumShipments
		{
			get;
			set;
		}

		public MobilePlayerCreateShipment()
		{
		}
	}
}