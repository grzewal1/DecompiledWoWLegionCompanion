using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamGarrisonTrophy", Version=28333852)]
	public class JamGarrisonTrophy
	{
		[DataMember(Name="trophyID")]
		[FlexJamMember(Name="trophyID", Type=FlexJamType.Int32)]
		public int TrophyID
		{
			get;
			set;
		}

		[DataMember(Name="trophyInstanceID")]
		[FlexJamMember(Name="trophyInstanceID", Type=FlexJamType.Int32)]
		public int TrophyInstanceID
		{
			get;
			set;
		}

		public JamGarrisonTrophy()
		{
		}
	}
}