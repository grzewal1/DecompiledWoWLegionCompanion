using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamBattlePayProductGroup", Version=28333852)]
	public class JamBattlePayProductGroup
	{
		[DataMember(Name="displayType")]
		[FlexJamMember(Name="displayType", Type=FlexJamType.UInt8)]
		public byte DisplayType
		{
			get;
			set;
		}

		[DataMember(Name="groupID")]
		[FlexJamMember(Name="groupID", Type=FlexJamType.UInt32)]
		public uint GroupID
		{
			get;
			set;
		}

		[DataMember(Name="iconFileDataID")]
		[FlexJamMember(Name="iconFileDataID", Type=FlexJamType.Int32)]
		public int IconFileDataID
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

		[DataMember(Name="ordering")]
		[FlexJamMember(Name="ordering", Type=FlexJamType.Int32)]
		public int Ordering
		{
			get;
			set;
		}

		public JamBattlePayProductGroup()
		{
		}
	}
}