using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4805, Name="MobilePlayerFollowerActivationDataRequest", Version=38820897)]
	public class MobilePlayerFollowerActivationDataRequest
	{
		[DataMember(Name="garrTypeID")]
		[FlexJamMember(Name="garrTypeID", Type=FlexJamType.Int32)]
		public int GarrTypeID
		{
			get;
			set;
		}

		public MobilePlayerFollowerActivationDataRequest()
		{
		}
	}
}