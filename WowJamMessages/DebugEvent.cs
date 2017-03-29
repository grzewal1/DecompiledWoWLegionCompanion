using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="DebugEvent", Version=28333852)]
	public class DebugEvent
	{
		[DataMember(Name="eventName")]
		[FlexJamMember(Name="eventName", Type=FlexJamType.String)]
		public string EventName
		{
			get;
			set;
		}

		[DataMember(Name="eventNameHash")]
		[FlexJamMember(Name="eventNameHash", Type=FlexJamType.Int32)]
		public int EventNameHash
		{
			get;
			set;
		}

		[DataMember(Name="eventTime")]
		[FlexJamMember(Name="eventTime", Type=FlexJamType.Int32)]
		public int EventTime
		{
			get;
			set;
		}

		[DataMember(Name="guid")]
		[FlexJamMember(ArrayDimensions=1, Name="guid", Type=FlexJamType.WowGuid)]
		public string[] Guid
		{
			get;
			set;
		}

		[DataMember(Name="messageText")]
		[FlexJamMember(Name="messageText", Type=FlexJamType.String)]
		public string MessageText
		{
			get;
			set;
		}

		[DataMember(Name="messageTextHash")]
		[FlexJamMember(Name="messageTextHash", Type=FlexJamType.Int32)]
		public int MessageTextHash
		{
			get;
			set;
		}

		[DataMember(Name="systemName")]
		[FlexJamMember(Name="systemName", Type=FlexJamType.String)]
		public string SystemName
		{
			get;
			set;
		}

		[DataMember(Name="systemNameHash")]
		[FlexJamMember(Name="systemNameHash", Type=FlexJamType.Int32)]
		public int SystemNameHash
		{
			get;
			set;
		}

		public DebugEvent()
		{
		}
	}
}