using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="DebugAttribute", Version=28333852)]
	public class DebugAttribute
	{
		[DataMember(Name="key")]
		[FlexJamMember(Name="key", Type=FlexJamType.String)]
		public string Key
		{
			get;
			set;
		}

		[DataMember(Name="param")]
		[FlexJamMember(Name="param", Type=FlexJamType.Int32)]
		public int Param
		{
			get;
			set;
		}

		[DataMember(Name="value")]
		[FlexJamMember(Name="value", Type=FlexJamType.Struct)]
		public AttributeValue Value
		{
			get;
			set;
		}

		public DebugAttribute()
		{
		}
	}
}