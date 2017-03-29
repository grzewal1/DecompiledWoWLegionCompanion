using System;
using System.Runtime.CompilerServices;

namespace JamLib
{
	public class FlexJamEnumAttribute : Attribute
	{
		public bool BitField
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

		public FlexJamEnumAttribute()
		{
		}
	}
}