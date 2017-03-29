using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamStruct(Name="MobileBountiesByWorldQuest", Version=39869590)]
	public class MobileBountiesByWorldQuest
	{
		[DataMember(Name="bountyQuestID")]
		[FlexJamMember(ArrayDimensions=1, Name="bountyQuestID", Type=FlexJamType.Int32)]
		public int[] BountyQuestID
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

		public MobileBountiesByWorldQuest()
		{
		}
	}
}