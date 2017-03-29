using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamShortVec3", Version=28333852)]
	public class JamShortVec3
	{
		[DataMember(Name="x")]
		[FlexJamMember(Name="x", Type=FlexJamType.Int16)]
		public short X
		{
			get;
			set;
		}

		[DataMember(Name="y")]
		[FlexJamMember(Name="y", Type=FlexJamType.Int16)]
		public short Y
		{
			get;
			set;
		}

		[DataMember(Name="z")]
		[FlexJamMember(Name="z", Type=FlexJamType.Int16)]
		public short Z
		{
			get;
			set;
		}

		public JamShortVec3()
		{
		}
	}
}