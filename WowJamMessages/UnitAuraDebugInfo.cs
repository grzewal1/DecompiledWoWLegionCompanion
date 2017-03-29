using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="UnitAuraDebugInfo", Version=28333852)]
	public class UnitAuraDebugInfo
	{
		[DataMember(Name="effectDebugInfo")]
		[FlexJamMember(ArrayDimensions=1, Name="effectDebugInfo", Type=FlexJamType.Struct)]
		public UnitAuraEffectDebugInfo[] EffectDebugInfo
		{
			get;
			set;
		}

		[DataMember(Name="enchantmentSlot")]
		[FlexJamMember(Name="enchantmentSlot", Type=FlexJamType.Int32)]
		public int EnchantmentSlot
		{
			get;
			set;
		}

		[DataMember(Name="fromEnchantment")]
		[FlexJamMember(Name="fromEnchantment", Type=FlexJamType.Bool)]
		public bool FromEnchantment
		{
			get;
			set;
		}

		[DataMember(Name="fromItem")]
		[FlexJamMember(Name="fromItem", Type=FlexJamType.Bool)]
		public bool FromItem
		{
			get;
			set;
		}

		[DataMember(Name="fromItemSet")]
		[FlexJamMember(Name="fromItemSet", Type=FlexJamType.Bool)]
		public bool FromItemSet
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

		[DataMember(Name="itemName")]
		[FlexJamMember(Name="itemName", Type=FlexJamType.String)]
		public string ItemName
		{
			get;
			set;
		}

		[DataMember(Name="name")]
		[FlexJamMember(Name="name", Type=FlexJamType.String)]
		public string Name
		{
			get;
			set;
		}

		[DataMember(Name="paused")]
		[FlexJamMember(Name="paused", Type=FlexJamType.Bool)]
		public bool Paused
		{
			get;
			set;
		}

		[DataMember(Name="serverOnly")]
		[FlexJamMember(Name="serverOnly", Type=FlexJamType.Bool)]
		public bool ServerOnly
		{
			get;
			set;
		}

		[DataMember(Name="spellID")]
		[FlexJamMember(Name="spellID", Type=FlexJamType.Int32)]
		public int SpellID
		{
			get;
			set;
		}

		public UnitAuraDebugInfo()
		{
		}
	}
}