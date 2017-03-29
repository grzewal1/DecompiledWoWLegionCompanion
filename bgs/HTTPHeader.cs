using System;
using System.Runtime.CompilerServices;

namespace bgs
{
	internal class HTTPHeader
	{
		public int ContentLength
		{
			get;
			set;
		}

		public int ContentStart
		{
			get;
			set;
		}

		public HTTPHeader()
		{
		}
	}
}