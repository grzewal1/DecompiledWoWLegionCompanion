using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamStruct(Name="MobileClientShipmentType", Version=39869590)]
	public class MobileClientShipmentType
	{
		[DataMember(Name="canOrder")]
		[FlexJamMember(Name="canOrder", Type=FlexJamType.Bool)]
		public bool CanOrder
		{
			get;
			set;
		}

		[DataMember(Name="canPickup")]
		[FlexJamMember(Name="canPickup", Type=FlexJamType.Bool)]
		public bool CanPickup
		{
			get;
			set;
		}

		[DataMember(Name="charShipmentID")]
		[FlexJamMember(Name="charShipmentID", Type=FlexJamType.Int32)]
		public int CharShipmentID
		{
			get;
			set;
		}

		[DataMember(Name="currencyCost")]
		[FlexJamMember(Name="currencyCost", Type=FlexJamType.Int32)]
		public int CurrencyCost
		{
			get;
			set;
		}

		[DataMember(Name="currencyTypeID")]
		[FlexJamMember(Name="currencyTypeID", Type=FlexJamType.Int32)]
		public int CurrencyTypeID
		{
			get;
			set;
		}

		public MobileClientShipmentType()
		{
		}
	}
}