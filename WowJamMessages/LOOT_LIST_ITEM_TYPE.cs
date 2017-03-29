using System;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	public enum LOOT_LIST_ITEM_TYPE
	{
		[EnumMember]
		LOOT_LIST_ITEM,
		[EnumMember]
		LOOT_LIST_CURRENCY,
		[EnumMember]
		LOOT_LIST_TRACKING_QUEST
	}
}