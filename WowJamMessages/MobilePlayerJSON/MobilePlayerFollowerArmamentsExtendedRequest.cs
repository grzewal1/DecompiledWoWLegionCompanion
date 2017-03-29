using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4819, Name="MobilePlayerFollowerArmamentsExtendedRequest", Version=38820897)]
	public class MobilePlayerFollowerArmamentsExtendedRequest
	{
		[DataMember(Name="garrFollowerTypeID")]
		[FlexJamMember(Name="garrFollowerTypeID", Type=FlexJamType.Int32)]
		public int GarrFollowerTypeID
		{
			get;
			set;
		}

		public MobilePlayerFollowerArmamentsExtendedRequest()
		{
		}
	}
}