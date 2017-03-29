using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamStruct(Name="MobileWorldQuestBounty", Version=39869590)]
	public class MobileWorldQuestBounty
	{
		[DataMember(Name="currency")]
		[FlexJamMember(ArrayDimensions=1, Name="currency", Type=FlexJamType.Struct)]
		public MobileWorldQuestReward[] Currency
		{
			get;
			set;
		}

		[DataMember(Name="endTime")]
		[FlexJamMember(Name="endTime", Type=FlexJamType.Int32)]
		public int EndTime
		{
			get;
			set;
		}

		[DataMember(Name="experience")]
		[FlexJamMember(Name="experience", Type=FlexJamType.Int32)]
		public int Experience
		{
			get;
			set;
		}

		[DataMember(Name="faction")]
		[FlexJamMember(ArrayDimensions=1, Name="faction", Type=FlexJamType.Struct)]
		public MobileWorldQuestReward[] Faction
		{
			get;
			set;
		}

		[DataMember(Name="iconFileDataID")]
		[FlexJamMember(Name="iconFileDataID", Type=FlexJamType.Int32)]
		public int IconFileDataID
		{
			get;
			set;
		}

		[DataMember(Name="item")]
		[FlexJamMember(ArrayDimensions=1, Name="item", Type=FlexJamType.Struct)]
		public MobileWorldQuestReward[] Item
		{
			get;
			set;
		}

		[DataMember(Name="money")]
		[FlexJamMember(Name="money", Type=FlexJamType.Int32)]
		public int Money
		{
			get;
			set;
		}

		[DataMember(Name="numCompleted")]
		[FlexJamMember(Name="numCompleted", Type=FlexJamType.Int32)]
		public int NumCompleted
		{
			get;
			set;
		}

		[DataMember(Name="numNeeded")]
		[FlexJamMember(Name="numNeeded", Type=FlexJamType.Int32)]
		public int NumNeeded
		{
			get;
			set;
		}

		[DataMember(Name="questID")]
		[FlexJamMember(Name="questID", Type=FlexJamType.Int32)]
		public int QuestID
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

		public MobileWorldQuestBounty()
		{
		}
	}
}