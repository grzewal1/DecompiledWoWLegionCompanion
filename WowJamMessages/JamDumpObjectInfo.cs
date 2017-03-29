using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamDumpObjectInfo", Version=28333852)]
	public class JamDumpObjectInfo
	{
		[DataMember(Name="displayID")]
		[FlexJamMember(Name="displayID", Type=FlexJamType.UInt32)]
		public uint DisplayID
		{
			get;
			set;
		}

		[DataMember(Name="granted")]
		[FlexJamMember(Name="granted", Type=FlexJamType.Bool)]
		public bool Granted
		{
			get;
			set;
		}

		[DataMember(Name="guid")]
		[FlexJamMember(Name="guid", Type=FlexJamType.WowGuid)]
		public string Guid
		{
			get;
			set;
		}

		[DataMember(Name="position")]
		[FlexJamMember(Name="position", Type=FlexJamType.Struct)]
		public Vector3 Position
		{
			get;
			set;
		}

		[DataMember(Name="visibleRange")]
		[FlexJamMember(Name="visibleRange", Type=FlexJamType.Float)]
		public float VisibleRange
		{
			get;
			set;
		}

		public JamDumpObjectInfo()
		{
			this.Granted = true;
		}
	}
}