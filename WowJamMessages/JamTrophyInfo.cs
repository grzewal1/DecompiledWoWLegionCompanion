using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamTrophyInfo", Version=28333852)]
	public class JamTrophyInfo
	{
		[DataMember(Name="canUseData")]
		[FlexJamMember(Name="canUseData", Type=FlexJamType.Int32)]
		public int CanUseData
		{
			get;
			set;
		}

		[DataMember(Name="canUseReason")]
		[FlexJamMember(Name="canUseReason", Type=FlexJamType.Int32)]
		public int CanUseReason
		{
			get;
			set;
		}

		[DataMember(Name="trophyID")]
		[FlexJamMember(Name="trophyID", Type=FlexJamType.Int32)]
		public int TrophyID
		{
			get;
			set;
		}

		public JamTrophyInfo()
		{
		}
	}
}