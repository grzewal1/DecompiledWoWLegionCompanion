using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4802, Name="MobilePlayerFollowerArmamentsRequest", Version=38820897)]
	public class MobilePlayerFollowerArmamentsRequest
	{
		[DataMember(Name="garrFollowerTypeID")]
		[FlexJamMember(Name="garrFollowerTypeID", Type=FlexJamType.Int32)]
		public int GarrFollowerTypeID
		{
			get;
			set;
		}

		public MobilePlayerFollowerArmamentsRequest()
		{
		}
	}
}