using System;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	public enum BATTLEPETRESULT
	{
		[EnumMember]
		BATTLEPETRESULT_OK,
		[EnumMember]
		BATTLEPETRESULT_BAD_PARAM,
		[EnumMember]
		BATTLEPETRESULT_DUPLICATE_CONVERTED_PET,
		[EnumMember]
		BATTLEPETRESULT_CANT_HAVE_MORE_PETS_OF_THAT_TYPE,
		[EnumMember]
		BATTLEPETRESULT_CANT_HAVE_MORE_PETS,
		[EnumMember]
		BATTLEPETRESULT_CANT_INVALID_CHARACTER_GUID,
		[EnumMember]
		BATTLEPETRESULT_UNCAPTURABLE,
		[EnumMember]
		BATTLEPETRESULT_TOO_HIGH_LEVEL_TO_UNCAGE
	}
}