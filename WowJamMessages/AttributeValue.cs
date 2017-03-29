using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="AttributeValue", Version=28333852)]
	public class AttributeValue
	{
		[DataMember(Name="floatValue")]
		[FlexJamMember(Name="floatValue", Type=FlexJamType.Float)]
		public float FloatValue
		{
			get;
			set;
		}

		[DataMember(Name="guidValue")]
		[FlexJamMember(Name="guidValue", Type=FlexJamType.WowGuid)]
		public string GuidValue
		{
			get;
			set;
		}

		[DataMember(Name="intValue")]
		[FlexJamMember(Name="intValue", Type=FlexJamType.Int32)]
		public int IntValue
		{
			get;
			set;
		}

		[DataMember(Name="stringValue")]
		[FlexJamMember(Name="stringValue", Type=FlexJamType.String)]
		public string StringValue
		{
			get;
			set;
		}

		[DataMember(Name="type")]
		[FlexJamMember(Name="type", Type=FlexJamType.Enum)]
		public AttributeValueType Type
		{
			get;
			set;
		}

		[DataMember(Name="vector3Value")]
		[FlexJamMember(Name="vector3Value", Type=FlexJamType.Struct)]
		public Vector3 Vector3Value
		{
			get;
			set;
		}

		public AttributeValue()
		{
			this.IntValue = 0;
			this.FloatValue = 0f;
			this.StringValue = string.Empty;
			this.GuidValue = "0000000000000000";
		}
	}
}