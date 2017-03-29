using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamGarrisonMissionReward", Version=28333852)]
	public class JamGarrisonMissionReward
	{
		[DataMember(Name="currencyQuantity")]
		[FlexJamMember(Name="currencyQuantity", Type=FlexJamType.UInt32)]
		public uint CurrencyQuantity
		{
			get;
			set;
		}

		[DataMember(Name="currencyType")]
		[FlexJamMember(Name="currencyType", Type=FlexJamType.Int32)]
		public int CurrencyType
		{
			get;
			set;
		}

		[DataMember(Name="followerXP")]
		[FlexJamMember(Name="followerXP", Type=FlexJamType.UInt32)]
		public uint FollowerXP
		{
			get;
			set;
		}

		[DataMember(Name="garrMssnBonusAbilityID")]
		[FlexJamMember(Name="garrMssnBonusAbilityID", Type=FlexJamType.UInt32)]
		public uint GarrMssnBonusAbilityID
		{
			get;
			set;
		}

		[DataMember(Name="itemFileDataID")]
		[FlexJamMember(Name="itemFileDataID", Type=FlexJamType.Int32)]
		public int ItemFileDataID
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

		[DataMember(Name="itemQuantity")]
		[FlexJamMember(Name="itemQuantity", Type=FlexJamType.UInt32)]
		public uint ItemQuantity
		{
			get;
			set;
		}

		public JamGarrisonMissionReward()
		{
			this.ItemFileDataID = 0;
		}
	}
}