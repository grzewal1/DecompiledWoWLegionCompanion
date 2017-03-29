using System;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	public enum AttributeValueType
	{
		[EnumMember]
		AVT_INT,
		[EnumMember]
		AVT_FLOAT,
		[EnumMember]
		AVT_STRING,
		[EnumMember]
		AVT_GUID,
		[EnumMember]
		AVT_VECTOR3
	}
}