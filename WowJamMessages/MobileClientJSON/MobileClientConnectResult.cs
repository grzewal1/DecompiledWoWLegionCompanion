using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4842, Name="MobileClientConnectResult", Version=39869590)]
	public class MobileClientConnectResult
	{
		[DataMember(Name="result")]
		[FlexJamMember(Name="result", Type=FlexJamType.Enum)]
		public MOBILE_CONNECT_RESULT Result
		{
			get;
			set;
		}

		[DataMember(Name="version")]
		[FlexJamMember(Name="version", Type=FlexJamType.Int32)]
		public int Version
		{
			get;
			set;
		}

		public MobileClientConnectResult()
		{
			this.Version = 0;
		}
	}
}