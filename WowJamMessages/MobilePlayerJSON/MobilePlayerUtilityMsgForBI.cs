using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4808, Name="MobilePlayerUtilityMsgForBI", Version=38820897)]
	public class MobilePlayerUtilityMsgForBI
	{
		[DataMember(Name="data1")]
		[FlexJamMember(Name="data1", Type=FlexJamType.Int32)]
		public int Data1
		{
			get;
			set;
		}

		[DataMember(Name="data2")]
		[FlexJamMember(Name="data2", Type=FlexJamType.Int32)]
		public int Data2
		{
			get;
			set;
		}

		[DataMember(Name="data3")]
		[FlexJamMember(Name="data3", Type=FlexJamType.Int32)]
		public int Data3
		{
			get;
			set;
		}

		[DataMember(Name="data4")]
		[FlexJamMember(Name="data4", Type=FlexJamType.Int32)]
		public int Data4
		{
			get;
			set;
		}

		[DataMember(Name="msgType")]
		[FlexJamMember(Name="msgType", Type=FlexJamType.Int32)]
		public int MsgType
		{
			get;
			set;
		}

		public MobilePlayerUtilityMsgForBI()
		{
		}
	}
}