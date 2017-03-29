using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamPartyMemberState", Version=28333852)]
	public class JamPartyMemberState
	{
		[DataMember(Name="areaID")]
		[FlexJamMember(Name="areaID", Type=FlexJamType.UInt16)]
		public ushort AreaID
		{
			get;
			set;
		}

		[DataMember(Name="auras")]
		[FlexJamMember(ArrayDimensions=1, Name="auras", Type=FlexJamType.Struct)]
		public JamPartyMemberAuraState[] Auras
		{
			get;
			set;
		}

		[DataMember(Name="displayPower")]
		[FlexJamMember(Name="displayPower", Type=FlexJamType.UInt8)]
		public byte DisplayPower
		{
			get;
			set;
		}

		[DataMember(Name="flags")]
		[FlexJamMember(Name="flags", Type=FlexJamType.UInt16)]
		public ushort Flags
		{
			get;
			set;
		}

		[DataMember(Name="health")]
		[FlexJamMember(Name="health", Type=FlexJamType.Int32)]
		public int Health
		{
			get;
			set;
		}

		[DataMember(Name="level")]
		[FlexJamMember(Name="level", Type=FlexJamType.UInt16)]
		public ushort Level
		{
			get;
			set;
		}

		[DataMember(Name="loc")]
		[FlexJamMember(Name="loc", Type=FlexJamType.Struct)]
		public JamShortVec3 Loc
		{
			get;
			set;
		}

		[DataMember(Name="maxHealth")]
		[FlexJamMember(Name="maxHealth", Type=FlexJamType.Int32)]
		public int MaxHealth
		{
			get;
			set;
		}

		[DataMember(Name="maxPower")]
		[FlexJamMember(Name="maxPower", Type=FlexJamType.UInt16)]
		public ushort MaxPower
		{
			get;
			set;
		}

		[DataMember(Name="overrideDisplayPower")]
		[FlexJamMember(Name="overrideDisplayPower", Type=FlexJamType.UInt16)]
		public ushort OverrideDisplayPower
		{
			get;
			set;
		}

		[DataMember(Name="partyType")]
		[FlexJamMember(ArrayDimensions=1, Name="partyType", Type=FlexJamType.UInt8)]
		public byte[] PartyType
		{
			get;
			set;
		}

		[DataMember(Name="pet")]
		[FlexJamMember(Optional=true, Name="pet", Type=FlexJamType.Struct)]
		public JamPartyMemberPetState[] Pet
		{
			get;
			set;
		}

		[DataMember(Name="phase")]
		[FlexJamMember(Name="phase", Type=FlexJamType.Struct)]
		public PhaseShiftData Phase
		{
			get;
			set;
		}

		[DataMember(Name="power")]
		[FlexJamMember(Name="power", Type=FlexJamType.UInt16)]
		public ushort Power
		{
			get;
			set;
		}

		[DataMember(Name="spec")]
		[FlexJamMember(Name="spec", Type=FlexJamType.UInt16)]
		public ushort Spec
		{
			get;
			set;
		}

		[DataMember(Name="vehicleSeatRecID")]
		[FlexJamMember(Name="vehicleSeatRecID", Type=FlexJamType.Int32)]
		public int VehicleSeatRecID
		{
			get;
			set;
		}

		[DataMember(Name="wmoDoodadPlacementID")]
		[FlexJamMember(Name="wmoDoodadPlacementID", Type=FlexJamType.UInt32)]
		public uint WmoDoodadPlacementID
		{
			get;
			set;
		}

		[DataMember(Name="wmoGroupID")]
		[FlexJamMember(Name="wmoGroupID", Type=FlexJamType.UInt16)]
		public ushort WmoGroupID
		{
			get;
			set;
		}

		public JamPartyMemberState()
		{
			this.PartyType = new byte[2];
		}
	}
}