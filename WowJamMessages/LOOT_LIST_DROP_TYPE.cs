using System;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	public enum LOOT_LIST_DROP_TYPE
	{
		[EnumMember]
		LOOT_LIST_DROP_NORMAL,
		[EnumMember]
		LOOT_LIST_DROP_MULTI,
		[EnumMember]
		LOOT_LIST_DROP_PERSONAL,
		[EnumMember]
		LOOT_LIST_DROP_PERSONAL_PUSH,
		[EnumMember]
		LOOT_LIST_DROP_PERSONAL_PUSH_FORCE_CLAIM
	}
}