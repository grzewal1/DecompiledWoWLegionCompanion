using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamGarrisonFollower", Version=28333852)]
	public class JamGarrisonFollower
	{
		[DataMember(Name="abilityID")]
		[FlexJamMember(ArrayDimensions=1, Name="abilityID", Type=FlexJamType.Int32)]
		public int[] AbilityID
		{
			get;
			set;
		}

		[DataMember(Name="currentBuildingID")]
		[FlexJamMember(Name="currentBuildingID", Type=FlexJamType.Int32)]
		public int CurrentBuildingID
		{
			get;
			set;
		}

		[DataMember(Name="currentMissionID")]
		[FlexJamMember(Name="currentMissionID", Type=FlexJamType.Int32)]
		public int CurrentMissionID
		{
			get;
			set;
		}

		[DataMember(Name="customName")]
		[FlexJamMember(Name="customName", Type=FlexJamType.String)]
		public string CustomName
		{
			get;
			set;
		}

		[DataMember(Name="dbID")]
		[FlexJamMember(Name="dbID", Type=FlexJamType.UInt64)]
		public ulong DbID
		{
			get;
			set;
		}

		[DataMember(Name="durability")]
		[FlexJamMember(Name="durability", Type=FlexJamType.Int32)]
		public int Durability
		{
			get;
			set;
		}

		[DataMember(Name="flags")]
		[FlexJamMember(Name="flags", Type=FlexJamType.Int32)]
		public int Flags
		{
			get;
			set;
		}

		[DataMember(Name="followerLevel")]
		[FlexJamMember(Name="followerLevel", Type=FlexJamType.Int32)]
		public int FollowerLevel
		{
			get;
			set;
		}

		[DataMember(Name="garrFollowerID")]
		[FlexJamMember(Name="garrFollowerID", Type=FlexJamType.Int32)]
		public int GarrFollowerID
		{
			get;
			set;
		}

		[DataMember(Name="itemLevelArmor")]
		[FlexJamMember(Name="itemLevelArmor", Type=FlexJamType.Int32)]
		public int ItemLevelArmor
		{
			get;
			set;
		}

		[DataMember(Name="itemLevelWeapon")]
		[FlexJamMember(Name="itemLevelWeapon", Type=FlexJamType.Int32)]
		public int ItemLevelWeapon
		{
			get;
			set;
		}

		[DataMember(Name="quality")]
		[FlexJamMember(Name="quality", Type=FlexJamType.Int32)]
		public int Quality
		{
			get;
			set;
		}

		[DataMember(Name="xp")]
		[FlexJamMember(Name="xp", Type=FlexJamType.Int32)]
		public int Xp
		{
			get;
			set;
		}

		[DataMember(Name="zoneSupportSpellID")]
		[FlexJamMember(Name="zoneSupportSpellID", Type=FlexJamType.Int32)]
		public int ZoneSupportSpellID
		{
			get;
			set;
		}

		public JamGarrisonFollower()
		{
			this.CustomName = string.Empty;
		}
	}
}