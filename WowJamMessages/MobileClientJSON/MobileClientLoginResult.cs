using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4840, Name="MobileClientLoginResult", Version=39869590)]
	public class MobileClientLoginResult
	{
		[DataMember(Name="success")]
		[FlexJamMember(Name="success", Type=FlexJamType.Bool)]
		public bool Success
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

		public MobileClientLoginResult()
		{
			this.Version = 0;
		}
	}
}