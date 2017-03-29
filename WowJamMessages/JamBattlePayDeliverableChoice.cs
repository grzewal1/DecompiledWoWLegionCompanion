using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamBattlePayDeliverableChoice", Version=28333852)]
	public class JamBattlePayDeliverableChoice
	{
		[DataMember(Name="alreadyOwns")]
		[FlexJamMember(Name="alreadyOwns", Type=FlexJamType.Bool)]
		public bool AlreadyOwns
		{
			get;
			set;
		}

		[DataMember(Name="battlePetCreatureID")]
		[FlexJamMember(Name="battlePetCreatureID", Type=FlexJamType.UInt32)]
		public uint BattlePetCreatureID
		{
			get;
			set;
		}

		[DataMember(Name="displayInfo")]
		[FlexJamMember(Optional=true, Name="displayInfo", Type=FlexJamType.Struct)]
		public JamBattlepayDisplayInfo[] DisplayInfo
		{
			get;
			set;
		}

		[DataMember(Name="ID")]
		[FlexJamMember(Name="ID", Type=FlexJamType.UInt32)]
		public uint ID
		{
			get;
			set;
		}

		[DataMember(Name="itemID")]
		[FlexJamMember(Name="itemID", Type=FlexJamType.UInt32)]
		public uint ItemID
		{
			get;
			set;
		}

		[DataMember(Name="mountSpellID")]
		[FlexJamMember(Name="mountSpellID", Type=FlexJamType.UInt32)]
		public uint MountSpellID
		{
			get;
			set;
		}

		[DataMember(Name="petResult")]
		[FlexJamMember(Optional=true, Name="petResult", Type=FlexJamType.Enum)]
		public BATTLEPETRESULT[] PetResult
		{
			get;
			set;
		}

		[DataMember(Name="quantity")]
		[FlexJamMember(Name="quantity", Type=FlexJamType.UInt32)]
		public uint Quantity
		{
			get;
			set;
		}

		[DataMember(Name="type")]
		[FlexJamMember(Name="type", Type=FlexJamType.UInt8)]
		public byte Type
		{
			get;
			set;
		}

		public JamBattlePayDeliverableChoice()
		{
		}
	}
}