using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using WowJamMessages;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4843, Name="MobileClientGarrisonDataRequestResult", Version=39869590)]
	public class MobileClientGarrisonDataRequestResult
	{
		[DataMember(Name="characterClassID")]
		[FlexJamMember(Name="characterClassID", Type=FlexJamType.Int32)]
		public int CharacterClassID
		{
			get;
			set;
		}

		[DataMember(Name="characterLevel")]
		[FlexJamMember(Name="characterLevel", Type=FlexJamType.Int32)]
		public int CharacterLevel
		{
			get;
			set;
		}

		[DataMember(Name="characterName")]
		[FlexJamMember(Name="characterName", Type=FlexJamType.String)]
		public string CharacterName
		{
			get;
			set;
		}

		[DataMember(Name="dailyMissionCount")]
		[FlexJamMember(Name="dailyMissionCount", Type=FlexJamType.Int32)]
		public int DailyMissionCount
		{
			get;
			set;
		}

		[DataMember(Name="follower")]
		[FlexJamMember(ArrayDimensions=1, Name="follower", Type=FlexJamType.Struct)]
		public JamGarrisonFollower[] Follower
		{
			get;
			set;
		}

		[DataMember(Name="garrTypeID")]
		[FlexJamMember(Name="garrTypeID", Type=FlexJamType.Int32)]
		public int GarrTypeID
		{
			get;
			set;
		}

		[DataMember(Name="goldCurrency")]
		[FlexJamMember(Name="goldCurrency", Type=FlexJamType.Int32)]
		public int GoldCurrency
		{
			get;
			set;
		}

		[DataMember(Name="mission")]
		[FlexJamMember(ArrayDimensions=1, Name="mission", Type=FlexJamType.Struct)]
		public JamGarrisonMobileMission[] Mission
		{
			get;
			set;
		}

		[DataMember(Name="oilCurrency")]
		[FlexJamMember(Name="oilCurrency", Type=FlexJamType.Int32)]
		public int OilCurrency
		{
			get;
			set;
		}

		[DataMember(Name="orderhallResourcesCurrency")]
		[FlexJamMember(Name="orderhallResourcesCurrency", Type=FlexJamType.Int32)]
		public int OrderhallResourcesCurrency
		{
			get;
			set;
		}

		[DataMember(Name="pvpFaction")]
		[FlexJamMember(Name="pvpFaction", Type=FlexJamType.Int32)]
		public int PvpFaction
		{
			get;
			set;
		}

		[DataMember(Name="resourcesCurrency")]
		[FlexJamMember(Name="resourcesCurrency", Type=FlexJamType.Int32)]
		public int ResourcesCurrency
		{
			get;
			set;
		}

		[DataMember(Name="serverTime")]
		[FlexJamMember(Name="serverTime", Type=FlexJamType.Int64)]
		public long ServerTime
		{
			get;
			set;
		}

		[DataMember(Name="talent")]
		[FlexJamMember(ArrayDimensions=1, Name="talent", Type=FlexJamType.Struct)]
		public JamGarrisonTalent[] Talent
		{
			get;
			set;
		}

		public MobileClientGarrisonDataRequestResult()
		{
		}
	}
}