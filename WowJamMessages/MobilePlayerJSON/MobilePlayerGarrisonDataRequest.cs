using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4780, Name="MobilePlayerGarrisonDataRequest", Version=38820897)]
	public class MobilePlayerGarrisonDataRequest
	{
		[DataMember(Name="garrTypeID")]
		[FlexJamMember(Name="garrTypeID", Type=FlexJamType.Int32)]
		public int GarrTypeID
		{
			get;
			set;
		}

		public MobilePlayerGarrisonDataRequest()
		{
		}
	}
}