using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="AITriggerActionDebugInfo", Version=28333852)]
	public class AITriggerActionDebugInfo
	{
		[DataMember(Name="aiGroupActionSetID")]
		[FlexJamMember(Name="aiGroupActionSetID", Type=FlexJamType.Int32)]
		public int AiGroupActionSetID
		{
			get;
			set;
		}

		[DataMember(Name="aiGroupActionSetName")]
		[FlexJamMember(Name="aiGroupActionSetName", Type=FlexJamType.String)]
		public string AiGroupActionSetName
		{
			get;
			set;
		}

		[DataMember(Name="note")]
		[FlexJamMember(Name="note", Type=FlexJamType.String)]
		public string Note
		{
			get;
			set;
		}

		[DataMember(Name="param")]
		[FlexJamMember(ArrayDimensions=1, Name="param", Type=FlexJamType.Int32)]
		public int[] Param
		{
			get;
			set;
		}

		[DataMember(Name="repeatCount")]
		[FlexJamMember(Name="repeatCount", Type=FlexJamType.Int32)]
		public int RepeatCount
		{
			get;
			set;
		}

		[DataMember(Name="triggerData")]
		[FlexJamMember(Name="triggerData", Type=FlexJamType.Int32)]
		public int TriggerData
		{
			get;
			set;
		}

		[DataMember(Name="triggerDescription")]
		[FlexJamMember(Name="triggerDescription", Type=FlexJamType.String)]
		public string TriggerDescription
		{
			get;
			set;
		}

		[DataMember(Name="triggerTime")]
		[FlexJamMember(Name="triggerTime", Type=FlexJamType.UInt32)]
		public uint TriggerTime
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

		[DataMember(Name="typeName")]
		[FlexJamMember(Name="typeName", Type=FlexJamType.String)]
		public string TypeName
		{
			get;
			set;
		}

		public AITriggerActionDebugInfo()
		{
			unsafe
			{
				this.TypeName = string.Empty;
				this.Param = new int[2];
				this.TriggerDescription = string.Empty;
				this.Note = string.Empty;
				this.AiGroupActionSetName = string.Empty;
			}
		}
	}
}