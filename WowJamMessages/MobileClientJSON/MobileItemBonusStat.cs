using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamStruct(Name="MobileItemBonusStat", Version=39869590)]
	public class MobileItemBonusStat
	{
		[DataMember(Name="bonusAmount")]
		[FlexJamMember(Name="bonusAmount", Type=FlexJamType.Int32)]
		public int BonusAmount
		{
			get;
			set;
		}

		[DataMember(Name="color")]
		[FlexJamMember(Name="color", Type=FlexJamType.Enum)]
		public MobileStatColor Color
		{
			get;
			set;
		}

		[DataMember(Name="statID")]
		[FlexJamMember(Name="statID", Type=FlexJamType.Int32)]
		public int StatID
		{
			get;
			set;
		}

		public MobileItemBonusStat()
		{
		}
	}
}