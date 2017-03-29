using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamStruct(Name="MobileQuestItem", Version=39869590)]
	public class MobileQuestItem
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

		public MobileQuestItem()
		{
		}
	}
}