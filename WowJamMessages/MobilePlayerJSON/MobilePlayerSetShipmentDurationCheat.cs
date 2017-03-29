using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4797, Name="MobilePlayerSetShipmentDurationCheat", Version=38820897)]
	public class MobilePlayerSetShipmentDurationCheat
	{
		[DataMember(Name="seconds")]
		[FlexJamMember(Name="seconds", Type=FlexJamType.Int32)]
		public int Seconds
		{
			get;
			set;
		}

		public MobilePlayerSetShipmentDurationCheat()
		{
		}
	}
}