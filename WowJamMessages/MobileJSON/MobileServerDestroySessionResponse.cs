using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileJSON
{
	[DataContract]
	public class MobileServerDestroySessionResponse
	{
		[DataMember(Name="rSessionID")]
		public string RSessionID
		{
			get;
			set;
		}

		public MobileServerDestroySessionResponse()
		{
		}
	}
}