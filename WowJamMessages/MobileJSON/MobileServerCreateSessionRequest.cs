using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileJSON
{
	[DataContract]
	public class MobileServerCreateSessionRequest
	{
		[DataMember(Name="bnetAccountID")]
		public ulong BnetAccountID
		{
			get;
			set;
		}

		[DataMember(Name="rSessionID")]
		public string RSessionID
		{
			get;
			set;
		}

		[DataMember(Name="token")]
		public ulong Token
		{
			get;
			set;
		}

		public MobileServerCreateSessionRequest()
		{
		}
	}
}