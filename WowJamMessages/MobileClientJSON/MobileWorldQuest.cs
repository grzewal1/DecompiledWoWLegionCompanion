using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamStruct(Name="MobileWorldQuest", Version=39869590)]
	public class MobileWorldQuest
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

		[DataMember(Name="objective")]
		[FlexJamMember(ArrayDimensions=1, Name="objective", Type=FlexJamType.Struct)]
		public MobileWorldQuestObjective[] Objective
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

		[DataMember(Name="questInfoID")]
		[FlexJamMember(Name="questInfoID", Type=FlexJamType.Int32)]
		public int QuestInfoID
		{
			get;
			set;
		}

		[DataMember(Name="questTitle")]
		[FlexJamMember(Name="questTitle", Type=FlexJamType.String)]
		public string QuestTitle
		{
			get;
			set;
		}

		[DataMember(Name="startLocationMapID")]
		[FlexJamMember(Name="startLocationMapID", Type=FlexJamType.Int32)]
		public int StartLocationMapID
		{
			get;
			set;
		}

		[DataMember(Name="startLocationX")]
		[FlexJamMember(Name="startLocationX", Type=FlexJamType.Int32)]
		public int StartLocationX
		{
			get;
			set;
		}

		[DataMember(Name="startLocationY")]
		[FlexJamMember(Name="startLocationY", Type=FlexJamType.Int32)]
		public int StartLocationY
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

		[DataMember(Name="worldMapAreaID")]
		[FlexJamMember(Name="worldMapAreaID", Type=FlexJamType.Int32)]
		public int WorldMapAreaID
		{
			get;
			set;
		}

		public MobileWorldQuest()
		{
		}
	}
}