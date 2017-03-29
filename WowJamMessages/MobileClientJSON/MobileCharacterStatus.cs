using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	public enum MobileCharacterStatus
	{
		[EnumMember]
		MOBILE_CHAR_STATUS_OKAY,
		[EnumMember]
		MOBILE_CHAR_STATUS_NEED_QUEST,
		[EnumMember]
		MOBILE_CHAR_STATUS_LOCKED,
		[EnumMember]
		MOBILE_CHAR_STATUS_LOCKED_BILLING,
		[EnumMember]
		MOBILE_CHAR_STATUS_REVOKED_UPGRADE,
		[EnumMember]
		MOBILE_CHAR_STATUS_REVOKED_TRANSACTION,
		[EnumMember]
		MOBILE_CHAR_STATUS_RENAME
	}
}