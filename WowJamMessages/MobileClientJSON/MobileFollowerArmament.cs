using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamStruct(Name="MobileFollowerArmament", Version=39869590)]
	public class MobileFollowerArmament
	{
		[DataMember(Name="itemID")]
		[FlexJamMember(Name="itemID", Type=FlexJamType.Int32)]
		public int ItemID
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

		public MobileFollowerArmament()
		{
		}
	}
}