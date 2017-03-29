using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="SystemDebugInfo", Version=28333852)]
	public class SystemDebugInfo
	{
		[DataMember(Name="attributeDescriptions")]
		[FlexJamMember(ArrayDimensions=1, Name="attributeDescriptions", Type=FlexJamType.Struct)]
		public DebugAttributeDescription[] AttributeDescriptions
		{
			get;
			set;
		}

		[DataMember(Name="attributes")]
		[FlexJamMember(ArrayDimensions=1, Name="attributes", Type=FlexJamType.Struct)]
		public DebugAttribute[] Attributes
		{
			get;
			set;
		}

		[DataMember(Name="name")]
		[FlexJamMember(Name="name", Type=FlexJamType.String)]
		public string Name
		{
			get;
			set;
		}

		[DataMember(Name="requestParameter")]
		[FlexJamMember(Name="requestParameter", Type=FlexJamType.String)]
		public string RequestParameter
		{
			get;
			set;
		}

		[DataMember(Name="updateTime")]
		[FlexJamMember(Name="updateTime", Type=FlexJamType.Int32)]
		public int UpdateTime
		{
			get;
			set;
		}

		public SystemDebugInfo()
		{
			this.RequestParameter = string.Empty;
		}
	}
}