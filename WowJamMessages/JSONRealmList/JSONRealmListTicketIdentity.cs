using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.JSONRealmList
{
	[DataContract]
	[FlexJamMessage(Id=15034, Name="JSONRealmListTicketIdentity", Version=28333852)]
	public class JSONRealmListTicketIdentity
	{
		[DataMember(Name="gameAccountID")]
		[FlexJamMember(Name="gameAccountID", Type=FlexJamType.UInt64)]
		public ulong GameAccountID
		{
			get;
			set;
		}

		[DataMember(Name="gameAccountRegion")]
		[FlexJamMember(Name="gameAccountRegion", Type=FlexJamType.UInt8)]
		public byte GameAccountRegion
		{
			get;
			set;
		}

		public JSONRealmListTicketIdentity()
		{
		}
	}
}