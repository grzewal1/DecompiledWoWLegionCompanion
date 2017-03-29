using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="SpawnEventDebugInfo", Version=28333852)]
	public class SpawnEventDebugInfo
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

		[DataMember(Name="entryNum")]
		[FlexJamMember(Name="entryNum", Type=FlexJamType.Int32)]
		public int EntryNum
		{
			get;
			set;
		}

		[DataMember(Name="eventID")]
		[FlexJamMember(Name="eventID", Type=FlexJamType.Int32)]
		public int EventID
		{
			get;
			set;
		}

		[DataMember(Name="eventName")]
		[FlexJamMember(Name="eventName", Type=FlexJamType.String)]
		public string EventName
		{
			get;
			set;
		}

		[DataMember(Name="eventPercent")]
		[FlexJamMember(Name="eventPercent", Type=FlexJamType.Int32)]
		public int EventPercent
		{
			get;
			set;
		}

		public SpawnEventDebugInfo()
		{
			this.EventName = string.Empty;
			this.AiGroupActionSetName = string.Empty;
		}
	}
}