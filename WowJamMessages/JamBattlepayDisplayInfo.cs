using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamBattlepayDisplayInfo", Version=28333852)]
	public class JamBattlepayDisplayInfo
	{
		[DataMember(Name="creatureDisplayInfoID")]
		[FlexJamMember(Optional=true, Name="creatureDisplayInfoID", Type=FlexJamType.UInt32)]
		public uint[] CreatureDisplayInfoID
		{
			get;
			set;
		}

		[DataMember(Name="fileDataID")]
		[FlexJamMember(Optional=true, Name="fileDataID", Type=FlexJamType.UInt32)]
		public uint[] FileDataID
		{
			get;
			set;
		}

		[DataMember(Name="flags")]
		[FlexJamMember(Optional=true, Name="flags", Type=FlexJamType.UInt32)]
		public uint[] Flags
		{
			get;
			set;
		}

		[DataMember(Name="name1")]
		[FlexJamMember(Name="name1", Type=FlexJamType.String)]
		public string Name1
		{
			get;
			set;
		}

		[DataMember(Name="name2")]
		[FlexJamMember(Name="name2", Type=FlexJamType.String)]
		public string Name2
		{
			get;
			set;
		}

		[DataMember(Name="name3")]
		[FlexJamMember(Name="name3", Type=FlexJamType.String)]
		public string Name3
		{
			get;
			set;
		}

		[DataMember(Name="overrideBackground")]
		[FlexJamMember(Optional=true, Name="overrideBackground", Type=FlexJamType.UInt32)]
		public uint[] OverrideBackground
		{
			get;
			set;
		}

		[DataMember(Name="overrideTextColor")]
		[FlexJamMember(Optional=true, Name="overrideTextColor", Type=FlexJamType.UInt32)]
		public uint[] OverrideTextColor
		{
			get;
			set;
		}

		[DataMember(Name="overrideTexture")]
		[FlexJamMember(Optional=true, Name="overrideTexture", Type=FlexJamType.UInt32)]
		public uint[] OverrideTexture
		{
			get;
			set;
		}

		public JamBattlepayDisplayInfo()
		{
		}
	}
}