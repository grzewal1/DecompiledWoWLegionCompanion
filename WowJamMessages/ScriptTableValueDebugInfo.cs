using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="ScriptTableValueDebugInfo", Version=28333852)]
	public class ScriptTableValueDebugInfo
	{
		[DataMember(Name="keyName")]
		[FlexJamMember(Name="keyName", Type=FlexJamType.String)]
		public string KeyName
		{
			get;
			set;
		}

		[DataMember(Name="valueName")]
		[FlexJamMember(Name="valueName", Type=FlexJamType.String)]
		public string ValueName
		{
			get;
			set;
		}

		public ScriptTableValueDebugInfo()
		{
		}
	}
}