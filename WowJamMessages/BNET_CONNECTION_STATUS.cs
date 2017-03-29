using System;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	public enum BNET_CONNECTION_STATUS
	{
		[EnumMember]
		BNET_CONNECTION_STATUS_NONE,
		[EnumMember]
		BNET_CONNECTION_STATUS_OK,
		[EnumMember]
		BNET_CONNECTION_STATUS_DISCONNECTED
	}
}