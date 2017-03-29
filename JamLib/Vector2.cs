using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace JamLib
{
	[DataContract]
	[FlexJamStruct(Name="vector2")]
	public struct Vector2
	{
		[DataMember(Name="x")]
		[FlexJamMember(Name="x", Type=FlexJamType.Float)]
		public float X
		{
			get;
			set;
		}

		[DataMember(Name="y")]
		[FlexJamMember(Name="y", Type=FlexJamType.Float)]
		public float Y
		{
			get;
			set;
		}
	}
}