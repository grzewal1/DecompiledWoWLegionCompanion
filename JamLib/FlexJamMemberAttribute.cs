using System;
using System.Runtime.CompilerServices;

namespace JamLib
{
	public class FlexJamMemberAttribute : Attribute
	{
		public int ArrayDimensions
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public bool Optional
		{
			get;
			set;
		}

		public FlexJamType Type
		{
			get;
			set;
		}

		public FlexJamMemberAttribute()
		{
		}
	}
}