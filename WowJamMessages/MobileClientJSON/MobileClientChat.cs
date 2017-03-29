using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4854, Name="MobileClientChat", Version=39869590)]
	public class MobileClientChat
	{
		[DataMember(Name="channel")]
		[FlexJamMember(Name="channel", Type=FlexJamType.String)]
		public string Channel
		{
			get;
			set;
		}

		[DataMember(Name="chatFlags")]
		[FlexJamMember(Name="chatFlags", Type=FlexJamType.UInt16)]
		public ushort ChatFlags
		{
			get;
			set;
		}

		[DataMember(Name="chatText")]
		[FlexJamMember(Name="chatText", Type=FlexJamType.String)]
		public string ChatText
		{
			get;
			set;
		}

		[DataMember(Name="prefix")]
		[FlexJamMember(Name="prefix", Type=FlexJamType.String)]
		public string Prefix
		{
			get;
			set;
		}

		[DataMember(Name="senderGUID")]
		[FlexJamMember(Name="senderGUID", Type=FlexJamType.WowGuid)]
		public string SenderGUID
		{
			get;
			set;
		}

		[DataMember(Name="senderName")]
		[FlexJamMember(Name="senderName", Type=FlexJamType.String)]
		public string SenderName
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

		public MobileClientChat()
		{
			this.SenderGUID = "0000000000000000";
			this.SenderName = string.Empty;
			this.Prefix = string.Empty;
			this.Channel = string.Empty;
			this.ChatText = string.Empty;
			this.ChatFlags = 0;
		}
	}
}