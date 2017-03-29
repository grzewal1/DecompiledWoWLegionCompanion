using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace JamLib
{
	[DataContract]
	[FlexJamStruct(Name="vector4")]
	public struct Vector4
	{
		[DataMember(Name="w")]
		[FlexJamMember(Name="w", Type=FlexJamType.Float)]
		public float W
		{
			get;
			set;
		}

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

		[DataMember(Name="z")]
		[FlexJamMember(Name="z", Type=FlexJamType.Float)]
		public float Z
		{
			get;
			set;
		}
	}
}