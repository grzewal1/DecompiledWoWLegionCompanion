using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileJSON
{
	[DataContract]
	public class MobileServerCreateSessionResponse
	{
		[DataMember(Name="lSessionID")]
		public ulong LSessionID
		{
			get;
			set;
		}

		[DataMember(Name="referral")]
		public string Referral
		{
			get;
			set;
		}

		[DataMember(Name="result")]
		public int Result
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

		public MobileServerCreateSessionResponse()
		{
		}
	}
}