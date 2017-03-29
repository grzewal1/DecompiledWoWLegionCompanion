using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.JSONRealmList
{
	[DataContract]
	[FlexJamMessage(Id=15035, Name="JSONRealmListTicketClientInformation", Version=28333852)]
	public class JSONRealmListTicketClientInformation
	{
		[DataMember(Name="info")]
		[FlexJamMember(Name="info", Type=FlexJamType.Struct)]
		public JamJSONRealmListTicketClientInformation Info
		{
			get;
			set;
		}

		public JSONRealmListTicketClientInformation()
		{
		}
	}
}