using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="DebugAttributeDescription", Version=28333852)]
	public class DebugAttributeDescription
	{
		[DataMember(Name="descriptionData")]
		[FlexJamMember(Name="descriptionData", Type=FlexJamType.String)]
		public string DescriptionData
		{
			get;
			set;
		}

		[DataMember(Name="flags")]
		[FlexJamMember(Name="flags", Type=FlexJamType.Int32)]
		public int Flags
		{
			get;
			set;
		}

		[DataMember(Name="key")]
		[FlexJamMember(Name="key", Type=FlexJamType.String)]
		public string Key
		{
			get;
			set;
		}

		[DataMember(Name="type")]
		[FlexJamMember(Name="type", Type=FlexJamType.Int32)]
		public int Type
		{
			get;
			set;
		}

		public DebugAttributeDescription()
		{
		}
	}
}