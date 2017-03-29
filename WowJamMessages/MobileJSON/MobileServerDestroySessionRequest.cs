using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileJSON
{
	[DataContract]
	public class MobileServerDestroySessionRequest
	{
		[DataMember(Name="lSessionID")]
		public ulong LSessionID
		{
			get;
			set;
		}

		public MobileServerDestroySessionRequest()
		{
		}
	}
}