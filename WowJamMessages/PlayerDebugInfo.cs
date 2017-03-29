using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="PlayerDebugInfo", Version=28333852)]
	public class PlayerDebugInfo
	{
		[DataMember(Name="combatRatings")]
		[FlexJamMember(ArrayDimensions=1, Name="combatRatings", Type=FlexJamType.Int32)]
		public int[] CombatRatings
		{
			get;
			set;
		}

		public PlayerDebugInfo()
		{
			this.CombatRatings = new int[32];
		}
	}
}