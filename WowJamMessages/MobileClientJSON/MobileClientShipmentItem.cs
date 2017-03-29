using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamStruct(Name="MobileClientShipmentItem", Version=39869590)]
	public class MobileClientShipmentItem
	{
		[DataMember(Name="context")]
		[FlexJamMember(Name="context", Type=FlexJamType.Int32)]
		public int Context
		{
			get;
			set;
		}

		[DataMember(Name="count")]
		[FlexJamMember(Name="count", Type=FlexJamType.Int32)]
		public int Count
		{
			get;
			set;
		}

		[DataMember(Name="iconFileDataID")]
		[FlexJamMember(Name="iconFileDataID", Type=FlexJamType.Int32)]
		public int IconFileDataID
		{
			get;
			set;
		}

		[DataMember(Name="itemID")]
		[FlexJamMember(Name="itemID", Type=FlexJamType.Int32)]
		public int ItemID
		{
			get;
			set;
		}

		[DataMember(Name="mailed")]
		[FlexJamMember(Name="mailed", Type=FlexJamType.Bool)]
		public bool Mailed
		{
			get;
			set;
		}

		public MobileClientShipmentItem()
		{
		}
	}
}