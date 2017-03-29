using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4869, Name="MobileClientSetShipmentDurationCheatResult", Version=39869590)]
	public class MobileClientSetShipmentDurationCheatResult
	{
		[DataMember(Name="success")]
		[FlexJamMember(Name="success", Type=FlexJamType.Bool)]
		public bool Success
		{
			get;
			set;
		}

		public MobileClientSetShipmentDurationCheatResult()
		{
		}
	}
}