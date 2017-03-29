using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamStruct(Name="MobileGuildMember", Version=39869590)]
	public class MobileGuildMember
	{
		[DataMember(Name="guid")]
		[FlexJamMember(Name="guid", Type=FlexJamType.WowGuid)]
		public string Guid
		{
			get;
			set;
		}

		[DataMember(Name="name")]
		[FlexJamMember(Name="name", Type=FlexJamType.String)]
		public string Name
		{
			get;
			set;
		}

		public MobileGuildMember()
		{
		}
	}
}