using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamStruct(Name="MobileItemStats", Version=39869590)]
	public class MobileItemStats
	{
		[DataMember(Name="bonusStat")]
		[FlexJamMember(ArrayDimensions=1, Name="bonusStat", Type=FlexJamType.Struct)]
		public MobileItemBonusStat[] BonusStat
		{
			get;
			set;
		}

		[DataMember(Name="dpr")]
		[FlexJamMember(Name="dpr", Type=FlexJamType.Float)]
		public float Dpr
		{
			get;
			set;
		}

		[DataMember(Name="effectiveArmor")]
		[FlexJamMember(Name="effectiveArmor", Type=FlexJamType.Int32)]
		public int EffectiveArmor
		{
			get;
			set;
		}

		[DataMember(Name="itemDelay")]
		[FlexJamMember(Name="itemDelay", Type=FlexJamType.Int32)]
		public int ItemDelay
		{
			get;
			set;
		}

		[DataMember(Name="itemLevel")]
		[FlexJamMember(Name="itemLevel", Type=FlexJamType.Int32)]
		public int ItemLevel
		{
			get;
			set;
		}

		[DataMember(Name="maxDamage")]
		[FlexJamMember(Name="maxDamage", Type=FlexJamType.Int32)]
		public int MaxDamage
		{
			get;
			set;
		}

		[DataMember(Name="minDamage")]
		[FlexJamMember(Name="minDamage", Type=FlexJamType.Int32)]
		public int MinDamage
		{
			get;
			set;
		}

		[DataMember(Name="quality")]
		[FlexJamMember(Name="quality", Type=FlexJamType.Int32)]
		public int Quality
		{
			get;
			set;
		}

		[DataMember(Name="requiredLevel")]
		[FlexJamMember(Name="requiredLevel", Type=FlexJamType.Int32)]
		public int RequiredLevel
		{
			get;
			set;
		}

		[DataMember(Name="weaponSpeed")]
		[FlexJamMember(Name="weaponSpeed", Type=FlexJamType.Float)]
		public float WeaponSpeed
		{
			get;
			set;
		}

		public MobileItemStats()
		{
		}
	}
}