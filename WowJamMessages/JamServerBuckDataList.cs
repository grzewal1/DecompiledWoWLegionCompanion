using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamServerBuckDataList", Version=28333852)]
	public class JamServerBuckDataList
	{
		[DataMember(Name="entries")]
		[FlexJamMember(ArrayDimensions=1, Name="entries", Type=FlexJamType.Struct)]
		public JamServerBuckDataEntry[] Entries
		{
			get;
			set;
		}

		[DataMember(Name="mpID")]
		[FlexJamMember(Name="mpID", Type=FlexJamType.UInt32)]
		public uint MpID
		{
			get;
			set;
		}

		public JamServerBuckDataList()
		{
		}
	}
}