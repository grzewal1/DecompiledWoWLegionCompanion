using System;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	public enum TRANSFER_ABORT
	{
		[EnumMember]
		TRANSFER_ABORT_NONE,
		[EnumMember]
		TRANSFER_ABORT_ERROR,
		[EnumMember]
		TRANSFER_ABORT_MAX_PLAYERS,
		[EnumMember]
		TRANSFER_ABORT_NOT_FOUND,
		[EnumMember]
		TRANSFER_ABORT_TOO_MANY_INSTANCES,
		[EnumMember]
		TRANSFER_ABORT_LOGGING_OUT,
		[EnumMember]
		TRANSFER_ABORT_ZONE_IN_COMBAT,
		[EnumMember]
		TRANSFER_ABORT_INSUF_EXPAN_LVL,
		[EnumMember]
		TRANSFER_ABORT_DIFFICULTY,
		[EnumMember]
		TRANSFER_ABORT_UNIQUE_MESSAGE,
		[EnumMember]
		TRANSFER_ABORT_TOO_MANY_REALM_INSTANCES,
		[EnumMember]
		TRANSFER_ABORT_NEED_GROUP,
		[EnumMember]
		TRANSFER_ABORT_NEED_SERVER,
		[EnumMember]
		TRANSFER_ABORT_TIMEOUT,
		[EnumMember]
		TRANSFER_ABORT_BUSY,
		[EnumMember]
		TRANSFER_ABORT_REALM_ONLY,
		[EnumMember]
		TRANSFER_ABORT_MAP_NOT_ALLOWED,
		[EnumMember]
		TRANSFER_ABORT_MANY_REALM_INSTANCES,
		[EnumMember]
		TRANSFER_ABORT_LOCKED_TO_DIFFERENT_INSTANCE,
		[EnumMember]
		TRANSFER_ABORT_ALREADY_COMPLETED_ENCOUNTER,
		[EnumMember]
		TRANSFER_ABORT_PLAYER_CONDITION,
		[EnumMember]
		TRANSFER_ABORT_AREA_NOT_ZONED,
		[EnumMember]
		TRANSFER_ABORT_DIFFICULTY_NOT_FOUND,
		[EnumMember]
		TRANSFER_ABORT_DISCONNECTED,
		[EnumMember]
		TRANSFER_ABORT_XREALM_ZONE_DOWN,
		[EnumMember]
		TRANSFER_ABORT_SHUTTING_DOWN,
		[EnumMember]
		TRANSFER_ABORT_SOLO_PLAYER_SWITCH_DIFFICULTY,
		[EnumMember]
		TRANSFER_ABORT_WORLD_GONE,
		[EnumMember]
		TRANSFER_ABORT_SUSPEND_FAILED,
		[EnumMember]
		TRANSFER_ABORT_MULTIPASSENGER_SYNC_FAILED,
		[EnumMember]
		TRANSFER_ABORT_SAME_SERVER
	}
}