using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="AITriggerActionSetDebugInfo", Version=28333852)]
	public class AITriggerActionSetDebugInfo
	{
		[DataMember(Name="aiTriggerActionSetID")]
		[FlexJamMember(Name="aiTriggerActionSetID", Type=FlexJamType.Int32)]
		public int AiTriggerActionSetID
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

		public AITriggerActionSetDebugInfo()
		{
		}
	}
}