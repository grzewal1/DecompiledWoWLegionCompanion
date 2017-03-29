using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4788, Name="MobilePlayerChat", Version=38820897)]
	public class MobilePlayerChat
	{
		[DataMember(Name="chatText")]
		[FlexJamMember(Name="chatText", Type=FlexJamType.String)]
		public string ChatText
		{
			get;
			set;
		}

		[DataMember(Name="slashCmd")]
		[FlexJamMember(Name="slashCmd", Type=FlexJamType.UInt8)]
		public byte SlashCmd
		{
			get;
			set;
		}

		[DataMember(Name="targetName")]
		[FlexJamMember(Name="targetName", Type=FlexJamType.String)]
		public string TargetName
		{
			get;
			set;
		}

		public MobilePlayerChat()
		{
			this.TargetName = string.Empty;
			this.ChatText = string.Empty;
		}
	}
}