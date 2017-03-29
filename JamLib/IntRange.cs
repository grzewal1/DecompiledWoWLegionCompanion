using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace JamLib
{
	[DataContract]
	[FlexJamStruct(Name="CiRange")]
	public struct IntRange
	{
		[DataMember(Name="h")]
		[FlexJamMember(Name="h", Type=FlexJamType.Int32)]
		public int High
		{
			get;
			set;
		}

		[DataMember(Name="l")]
		[FlexJamMember(Name="l", Type=FlexJamType.Int32)]
		public int Low
		{
			get;
			set;
		}
	}
}