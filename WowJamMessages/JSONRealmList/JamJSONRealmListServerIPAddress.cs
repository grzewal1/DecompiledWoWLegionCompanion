using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.JSONRealmList
{
	[DataContract]
	[FlexJamStruct(Name="JamJSONRealmListServerIPAddress", Version=47212487)]
	public class JamJSONRealmListServerIPAddress
	{
		[DataMember(Name="ip")]
		[FlexJamMember(Name="ip", Type=FlexJamType.SockAddr)]
		public string Ip
		{
			get;
			set;
		}

		[DataMember(Name="port")]
		[FlexJamMember(Name="port", Type=FlexJamType.UInt16)]
		public ushort Port
		{
			get;
			set;
		}

		public JamJSONRealmListServerIPAddress()
		{
		}
	}
}