using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4856, Name="MobileClientGuildMemberLoggedIn", Version=39869590)]
	public class MobileClientGuildMemberLoggedIn
	{
		[DataMember(Name="member")]
		[FlexJamMember(Name="member", Type=FlexJamType.Struct)]
		public MobileGuildMember Member
		{
			get;
			set;
		}

		public MobileClientGuildMemberLoggedIn()
		{
		}
	}
}