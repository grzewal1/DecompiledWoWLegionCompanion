using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamGarrisonMissionBonusAbility", Version=28333852)]
	public class JamGarrisonMissionBonusAbility
	{
		[DataMember(Name="garrMssnBonusAbilityID")]
		[FlexJamMember(Name="garrMssnBonusAbilityID", Type=FlexJamType.Int32)]
		public int GarrMssnBonusAbilityID
		{
			get;
			set;
		}

		[DataMember(Name="startTime")]
		[FlexJamMember(Name="startTime", Type=FlexJamType.Int32)]
		public int StartTime
		{
			get;
			set;
		}

		public JamGarrisonMissionBonusAbility()
		{
		}
	}
}