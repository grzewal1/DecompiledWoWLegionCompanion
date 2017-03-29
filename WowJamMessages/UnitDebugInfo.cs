using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="UnitDebugInfo", Version=28333852)]
	public class UnitDebugInfo
	{
		[DataMember(Name="aiTriggerActionDebugInfo")]
		[FlexJamMember(ArrayDimensions=1, Name="aiTriggerActionDebugInfo", Type=FlexJamType.Struct)]
		public AITriggerActionDebugInfo[] AiTriggerActionDebugInfo
		{
			get;
			set;
		}

		[DataMember(Name="aiTriggerActionSetDebugInfo")]
		[FlexJamMember(ArrayDimensions=1, Name="aiTriggerActionSetDebugInfo", Type=FlexJamType.Struct)]
		public AITriggerActionSetDebugInfo[] AiTriggerActionSetDebugInfo
		{
			get;
			set;
		}

		[DataMember(Name="auraDebugInfo")]
		[FlexJamMember(ArrayDimensions=1, Name="auraDebugInfo", Type=FlexJamType.Struct)]
		public UnitAuraDebugInfo[] AuraDebugInfo
		{
			get;
			set;
		}

		[DataMember(Name="classID")]
		[FlexJamMember(Name="classID", Type=FlexJamType.Int32)]
		public int ClassID
		{
			get;
			set;
		}

		[DataMember(Name="creatureSpellDataID")]
		[FlexJamMember(Name="creatureSpellDataID", Type=FlexJamType.Int32)]
		public int CreatureSpellDataID
		{
			get;
			set;
		}

		[DataMember(Name="effectiveStatValues")]
		[FlexJamMember(ArrayDimensions=1, Name="effectiveStatValues", Type=FlexJamType.Int32)]
		public int[] EffectiveStatValues
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

		[DataMember(Name="percentRangedAttack")]
		[FlexJamMember(Name="percentRangedAttack", Type=FlexJamType.Int32)]
		public int PercentRangedAttack
		{
			get;
			set;
		}

		[DataMember(Name="percentSupportAction")]
		[FlexJamMember(Name="percentSupportAction", Type=FlexJamType.Int32)]
		public int PercentSupportAction
		{
			get;
			set;
		}

		[DataMember(Name="playerClassID")]
		[FlexJamMember(Name="playerClassID", Type=FlexJamType.Int32)]
		public int PlayerClassID
		{
			get;
			set;
		}

		[DataMember(Name="spawnEventDebugInfo")]
		[FlexJamMember(ArrayDimensions=1, Name="spawnEventDebugInfo", Type=FlexJamType.Struct)]
		public WowJamMessages.SpawnEventDebugInfo[] SpawnEventDebugInfo
		{
			get;
			set;
		}

		[DataMember(Name="spawnGroupID")]
		[FlexJamMember(Name="spawnGroupID", Type=FlexJamType.Int32)]
		public int SpawnGroupID
		{
			get;
			set;
		}

		[DataMember(Name="spawnGroupName")]
		[FlexJamMember(Name="spawnGroupName", Type=FlexJamType.String)]
		public string SpawnGroupName
		{
			get;
			set;
		}

		[DataMember(Name="spawnRegionID")]
		[FlexJamMember(Name="spawnRegionID", Type=FlexJamType.Int32)]
		public int SpawnRegionID
		{
			get;
			set;
		}

		[DataMember(Name="spawnRegionName")]
		[FlexJamMember(Name="spawnRegionName", Type=FlexJamType.String)]
		public string SpawnRegionName
		{
			get;
			set;
		}

		[DataMember(Name="spellDebugInfo")]
		[FlexJamMember(ArrayDimensions=1, Name="spellDebugInfo", Type=FlexJamType.Struct)]
		public CreatureSpellDebugInfo[] SpellDebugInfo
		{
			get;
			set;
		}

		[DataMember(Name="zoneFlags")]
		[FlexJamMember(ArrayDimensions=1, Name="zoneFlags", Type=FlexJamType.UInt32)]
		public uint[] ZoneFlags
		{
			get;
			set;
		}

		public UnitDebugInfo()
		{
			unsafe
			{
				this.ZoneFlags = new uint[3];
				this.EffectiveStatValues = new int[5];
				this.CreatureSpellDataID = 0;
				this.PercentSupportAction = 0;
				this.PercentRangedAttack = 0;
				this.SpawnGroupID = 0;
				this.SpawnGroupName = string.Empty;
				this.SpawnRegionID = 0;
				this.SpawnRegionName = string.Empty;
			}
		}
	}
}