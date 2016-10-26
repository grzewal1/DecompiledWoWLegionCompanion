using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[FlexJamStruct(Name = "MobileClientShipmentItem", Version = 33577221u), DataContract]
	public class MobileClientShipmentItem
	{
		[FlexJamMember(Name = "context", Type = FlexJamType.Int32), DataMember(Name = "context")]
		public int Context
		{
			get;
			set;
		}

		[FlexJamMember(Name = "iconFileDataID", Type = FlexJamType.Int32), DataMember(Name = "iconFileDataID")]
		public int IconFileDataID
		{
			get;
			set;
		}

		[FlexJamMember(Name = "mailed", Type = FlexJamType.Bool), DataMember(Name = "mailed")]
		public bool Mailed
		{
			get;
			set;
		}

		[FlexJamMember(Name = "itemID", Type = FlexJamType.Int32), DataMember(Name = "itemID")]
		public int ItemID
		{
			get;
			set;
		}

		[FlexJamMember(Name = "count", Type = FlexJamType.Int32), DataMember(Name = "count")]
		public int Count
		{
			get;
			set;
		}
	}
}
