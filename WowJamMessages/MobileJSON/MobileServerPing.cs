using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileJSON
{
	[DataContract]
	public class MobileServerPing
	{
		[DataMember(Name="reply")]
		public bool Reply
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

		public MobileServerPing()
		{
		}
	}
}