using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4855, Name="MobileClientGuildMembersOnline", Version=39869590)]
	public class MobileClientGuildMembersOnline
	{
		[DataMember(Name="members")]
		[FlexJamMember(ArrayDimensions=1, Name="members", Type=FlexJamType.Struct)]
		public MobileGuildMember[] Members
		{
			get;
			set;
		}

		public MobileClientGuildMembersOnline()
		{
		}
	}
}