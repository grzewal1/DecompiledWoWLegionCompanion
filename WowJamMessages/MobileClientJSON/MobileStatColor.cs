using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	public enum MobileStatColor
	{
		[EnumMember]
		MOBILE_STAT_COLOR_TRIVIAL,
		[EnumMember]
		MOBILE_STAT_COLOR_NORMAL,
		[EnumMember]
		MOBILE_STAT_COLOR_FRIENDLY,
		[EnumMember]
		MOBILE_STAT_COLOR_HOSTILE,
		[EnumMember]
		MOBILE_STAT_COLOR_INACTIVE,
		[EnumMember]
		MOBILE_STAT_COLOR_ERROR
	}
}