using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamGlobalAura", Version=28333852)]
	public class JamGlobalAura
	{
		[DataMember(Name="playerConditionID")]
		[FlexJamMember(Name="playerConditionID", Type=FlexJamType.Int32)]
		public int PlayerConditionID
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

		public JamGlobalAura()
		{
		}
	}
}