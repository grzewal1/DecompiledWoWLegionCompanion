using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="UnitAuraEffectDebugInfo", Version=28333852)]
	public class UnitAuraEffectDebugInfo
	{
		[DataMember(Name="active")]
		[FlexJamMember(Name="active", Type=FlexJamType.Bool)]
		public bool Active
		{
			get;
			set;
		}

		[DataMember(Name="amount")]
		[FlexJamMember(Name="amount", Type=FlexJamType.Float)]
		public float Amount
		{
			get;
			set;
		}

		[DataMember(Name="effectIndex")]
		[FlexJamMember(Name="effectIndex", Type=FlexJamType.Int32)]
		public int EffectIndex
		{
			get;
			set;
		}

		public UnitAuraEffectDebugInfo()
		{
		}
	}
}