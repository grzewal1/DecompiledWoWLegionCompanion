using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4857, Name="MobileClientGuildMemberLoggedOut", Version=39869590)]
	public class MobileClientGuildMemberLoggedOut
	{
		[DataMember(Name="member")]
		[FlexJamMember(Name="member", Type=FlexJamType.Struct)]
		public MobileGuildMember Member
		{
			get;
			set;
		}

		public MobileClientGuildMemberLoggedOut()
		{
		}
	}
}