using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4860, Name="MobileClientCreateShipmentResult", Version=39869590)]
	public class MobileClientCreateShipmentResult
	{
		[DataMember(Name="charShipmentID")]
		[FlexJamMember(Name="charShipmentID", Type=FlexJamType.Int32)]
		public int CharShipmentID
		{
			get;
			set;
		}

		[DataMember(Name="result")]
		[FlexJamMember(Name="result", Type=FlexJamType.Int32)]
		public int Result
		{
			get;
			set;
		}

		public MobileClientCreateShipmentResult()
		{
		}
	}
}