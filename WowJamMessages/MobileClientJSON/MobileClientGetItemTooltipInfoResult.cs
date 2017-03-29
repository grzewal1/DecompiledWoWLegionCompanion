using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4881, Name="MobileClientGetItemTooltipInfoResult", Version=39869590)]
	public class MobileClientGetItemTooltipInfoResult
	{
		[DataMember(Name="itemContext")]
		[FlexJamMember(Name="itemContext", Type=FlexJamType.Int32)]
		public int ItemContext
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

		[DataMember(Name="stats")]
		[FlexJamMember(Name="stats", Type=FlexJamType.Struct)]
		public MobileItemStats Stats
		{
			get;
			set;
		}

		public MobileClientGetItemTooltipInfoResult()
		{
		}
	}
}