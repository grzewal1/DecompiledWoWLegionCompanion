using System;
using System.Runtime.CompilerServices;

namespace bgs
{
	public class ExternalChallenge
	{
		public ExternalChallenge Next
		{
			get;
			set;
		}

		public string PayLoadType
		{
			get;
			set;
		}

		public string URL
		{
			get;
			set;
		}

		public ExternalChallenge()
		{
		}
	}
}