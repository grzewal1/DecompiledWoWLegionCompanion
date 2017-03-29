using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileJSON
{
	[DataContract]
	[FlexJamMessage(Id=4740, Name="MobileServerLogin", Version=28333852)]
	public class MobileServerLogin
	{
		[DataMember(Name="joinTicket")]
		[FlexJamMember(ArrayDimensions=1, Name="joinTicket", Type=FlexJamType.UInt8)]
		public byte[] JoinTicket
		{
			get;
			set;
		}

		[DataMember(Name="locale")]
		[FlexJamMember(Name="locale", Type=FlexJamType.String)]
		public string Locale
		{
			get;
			set;
		}

		public MobileServerLogin()
		{
		}
	}
}