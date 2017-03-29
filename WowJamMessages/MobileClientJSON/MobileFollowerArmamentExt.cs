using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamStruct(Name="MobileFollowerArmamentExt", Version=39869590)]
	public class MobileFollowerArmamentExt
	{
		[DataMember(Name="itemID")]
		[FlexJamMember(Name="itemID", Type=FlexJamType.Int32)]
		public int ItemID
		{
			get;
			set;
		}

		[DataMember(Name="maxItemLevel")]
		[FlexJamMember(Name="maxItemLevel", Type=FlexJamType.Int32)]
		public int MaxItemLevel
		{
			get;
			set;
		}

		[DataMember(Name="minItemLevel")]
		[FlexJamMember(Name="minItemLevel", Type=FlexJamType.Int32)]
		public int MinItemLevel
		{
			get;
			set;
		}

		[DataMember(Name="operation")]
		[FlexJamMember(Name="operation", Type=FlexJamType.Int32)]
		public int Operation
		{
			get;
			set;
		}

		[DataMember(Name="quantity")]
		[FlexJamMember(Name="quantity", Type=FlexJamType.Int32)]
		public int Quantity
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

		public MobileFollowerArmamentExt()
		{
		}
	}
}