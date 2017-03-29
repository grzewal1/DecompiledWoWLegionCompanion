using System;
using System.Runtime.CompilerServices;

namespace JamLib
{
	public class FlexJamMessageAttribute : Attribute
	{
		public int Id
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public uint Version
		{
			get;
			set;
		}

		public FlexJamMessageAttribute()
		{
		}
	}
}