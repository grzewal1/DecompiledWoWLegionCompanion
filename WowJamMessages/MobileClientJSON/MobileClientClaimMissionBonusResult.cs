using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using WowJamMessages;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4847, Name="MobileClientClaimMissionBonusResult", Version=39869590)]
	public class MobileClientClaimMissionBonusResult
	{
		[DataMember(Name="awardOvermax")]
		[FlexJamMember(Name="awardOvermax", Type=FlexJamType.Bool)]
		public bool AwardOvermax
		{
			get;
			set;
		}

		[DataMember(Name="garrMissionID")]
		[FlexJamMember(Name="garrMissionID", Type=FlexJamType.Int32)]
		public int GarrMissionID
		{
			get;
			set;
		}

		[DataMember(Name="mission")]
		[FlexJamMember(Name="mission", Type=FlexJamType.Struct)]
		public JamGarrisonMobileMission Mission
		{
			get;
			set;
		}

		[DataMember(Name="result")]
		[FlexJamMember(Name="result", Type=FlexJamType.Int32)]
		public int Result
		{
			get;
			set;
		}

		public MobileClientClaimMissionBonusResult()
		{
		}
	}
}