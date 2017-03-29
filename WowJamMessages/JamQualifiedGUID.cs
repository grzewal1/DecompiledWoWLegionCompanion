using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamQualifiedGUID", Version=28333852)]
	public class JamQualifiedGUID
	{
		[DataMember(Name="guid")]
		[FlexJamMember(Name="guid", Type=FlexJamType.WowGuid)]
		public string Guid
		{
			get;
			set;
		}

		[DataMember(Name="virtualRealmAddress")]
		[FlexJamMember(Name="virtualRealmAddress", Type=FlexJamType.UInt32)]
		public uint VirtualRealmAddress
		{
			get;
			set;
		}

		public JamQualifiedGUID()
		{
			this.VirtualRealmAddress = 0;
			this.Guid = "0000000000000000";
		}
	}
}