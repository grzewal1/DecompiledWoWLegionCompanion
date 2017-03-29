using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4864, Name="MobileClientShipmentPushResult", Version=39869590)]
	public class MobileClientShipmentPushResult
	{
		[DataMember(Name="charShipmentID")]
		[FlexJamMember(Name="charShipmentID", Type=FlexJamType.Int32)]
		public int CharShipmentID
		{
			get;
			set;
		}

		[DataMember(Name="items")]
		[FlexJamMember(ArrayDimensions=1, Name="items", Type=FlexJamType.Struct)]
		public MobileClientShipmentItem[] Items
		{
			get;
			set;
		}

		public MobileClientShipmentPushResult()
		{
		}
	}
}