using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4806, Name="MobilePlayerChangeFollowerActive", Version=38820897)]
	public class MobilePlayerChangeFollowerActive
	{
		[DataMember(Name="garrFollowerID")]
		[FlexJamMember(Name="garrFollowerID", Type=FlexJamType.Int32)]
		public int GarrFollowerID
		{
			get;
			set;
		}

		[DataMember(Name="setInactive")]
		[FlexJamMember(Name="setInactive", Type=FlexJamType.Bool)]
		public bool SetInactive
		{
			get;
			set;
		}

		public MobilePlayerChangeFollowerActive()
		{
		}
	}
}