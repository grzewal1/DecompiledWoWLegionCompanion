using System;
using System.Runtime.CompilerServices;

namespace JamLib
{
	public class FlexJamStructAttribute : Attribute
	{
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

		public FlexJamStructAttribute()
		{
		}
	}
}