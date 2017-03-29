using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4809, Name="MobilePlayerGetItemTooltipInfo", Version=38820897)]
	public class MobilePlayerGetItemTooltipInfo
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

		public MobilePlayerGetItemTooltipInfo()
		{
		}
	}
}