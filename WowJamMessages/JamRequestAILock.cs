using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamRequestAILock", Version=28333852)]
	public class JamRequestAILock
	{
		[DataMember(Name="lockReason")]
		[FlexJamMember(Name="lockReason", Type=FlexJamType.UInt32)]
		public uint LockReason
		{
			get;
			set;
		}

		[DataMember(Name="lockResourceGUID")]
		[FlexJamMember(Name="lockResourceGUID", Type=FlexJamType.WowGuid)]
		public string LockResourceGUID
		{
			get;
			set;
		}

		[DataMember(Name="ticketGUID")]
		[FlexJamMember(Name="ticketGUID", Type=FlexJamType.WowGuid)]
		public string TicketGUID
		{
			get;
			set;
		}

		public JamRequestAILock()
		{
		}
	}
}