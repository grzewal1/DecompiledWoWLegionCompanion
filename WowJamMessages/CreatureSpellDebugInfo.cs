using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="CreatureSpellDebugInfo", Version=28333852)]
	public class CreatureSpellDebugInfo
	{
		[DataMember(Name="availability")]
		[FlexJamMember(Name="availability", Type=FlexJamType.Int32)]
		public int Availability
		{
			get;
			set;
		}

		[DataMember(Name="initialDelayMax")]
		[FlexJamMember(Name="initialDelayMax", Type=FlexJamType.Int32)]
		public int InitialDelayMax
		{
			get;
			set;
		}

		[DataMember(Name="initialDelayMin")]
		[FlexJamMember(Name="initialDelayMin", Type=FlexJamType.Int32)]
		public int InitialDelayMin
		{
			get;
			set;
		}

		[DataMember(Name="priority")]
		[FlexJamMember(Name="priority", Type=FlexJamType.Int32)]
		public int Priority
		{
			get;
			set;
		}

		[DataMember(Name="repeatFrequencyMax")]
		[FlexJamMember(Name="repeatFrequencyMax", Type=FlexJamType.Int32)]
		public int RepeatFrequencyMax
		{
			get;
			set;
		}

		[DataMember(Name="repeatFrequencyMin")]
		[FlexJamMember(Name="repeatFrequencyMin", Type=FlexJamType.Int32)]
		public int RepeatFrequencyMin
		{
			get;
			set;
		}

		[DataMember(Name="spellID")]
		[FlexJamMember(Name="spellID", Type=FlexJamType.Int32)]
		public int SpellID
		{
			get;
			set;
		}

		[DataMember(Name="spellName")]
		[FlexJamMember(Name="spellName", Type=FlexJamType.String)]
		public string SpellName
		{
			get;
			set;
		}

		public CreatureSpellDebugInfo()
		{
		}
	}
}