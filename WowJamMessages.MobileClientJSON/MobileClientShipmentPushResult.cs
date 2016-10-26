using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[FlexJamMessage(Id = 4854, Name = "MobileClientShipmentPushResult", Version = 33577221u), DataContract]
	public class MobileClientShipmentPushResult
	{
		[FlexJamMember(Name = "charShipmentID", Type = FlexJamType.Int32), DataMember(Name = "charShipmentID")]
		public int CharShipmentID
		{
			get;
			set;
		}

		[FlexJamMember(ArrayDimensions = 1, Name = "items", Type = FlexJamType.Struct), DataMember(Name = "items")]
		public MobileClientShipmentItem[] Items
		{
			get;
			set;
		}
	}
}
