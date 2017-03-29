using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="ScreenshotJFIFComment", Version=28333852)]
	public class ScreenshotJFIFComment
	{
		[DataMember(Name="classID")]
		[FlexJamMember(Name="classID", Type=FlexJamType.UInt32)]
		public uint ClassID
		{
			get;
			set;
		}

		[DataMember(Name="facing")]
		[FlexJamMember(Name="facing", Type=FlexJamType.Float)]
		public float Facing
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

		[DataMember(Name="isInGame")]
		[FlexJamMember(Name="isInGame", Type=FlexJamType.Bool)]
		public bool IsInGame
		{
			get;
			set;
		}

		[DataMember(Name="level")]
		[FlexJamMember(Name="level", Type=FlexJamType.Int32)]
		public int Level
		{
			get;
			set;
		}

		[DataMember(Name="mapID")]
		[FlexJamMember(Name="mapID", Type=FlexJamType.UInt32)]
		public uint MapID
		{
			get;
			set;
		}

		[DataMember(Name="mapName")]
		[FlexJamMember(Name="mapName", Type=FlexJamType.String)]
		public string MapName
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

		[DataMember(Name="position")]
		[FlexJamMember(Name="position", Type=FlexJamType.Struct)]
		public Vector3 Position
		{
			get;
			set;
		}

		[DataMember(Name="raceID")]
		[FlexJamMember(Name="raceID", Type=FlexJamType.UInt32)]
		public uint RaceID
		{
			get;
			set;
		}

		[DataMember(Name="realmName")]
		[FlexJamMember(Name="realmName", Type=FlexJamType.String)]
		public string RealmName
		{
			get;
			set;
		}

		[DataMember(Name="sex")]
		[FlexJamMember(Name="sex", Type=FlexJamType.UInt32)]
		public uint Sex
		{
			get;
			set;
		}

		[DataMember(Name="worldport")]
		[FlexJamMember(Name="worldport", Type=FlexJamType.String)]
		public string Worldport
		{
			get;
			set;
		}

		[DataMember(Name="zoneName")]
		[FlexJamMember(Name="zoneName", Type=FlexJamType.String)]
		public string ZoneName
		{
			get;
			set;
		}

		public ScreenshotJFIFComment()
		{
		}
	}
}