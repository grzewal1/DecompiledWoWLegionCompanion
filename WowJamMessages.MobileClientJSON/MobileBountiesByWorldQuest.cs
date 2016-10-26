using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[FlexJamStruct(Name = "MobileBountiesByWorldQuest", Version = 33577221u), DataContract]
	public class MobileBountiesByWorldQuest
	{
		[FlexJamMember(ArrayDimensions = 1, Name = "bountyQuestID", Type = FlexJamType.Int32), DataMember(Name = "bountyQuestID")]
		public int[] BountyQuestID
		{
			get;
			set;
		}

		[FlexJamMember(Name = "questID", Type = FlexJamType.Int32), DataMember(Name = "questID")]
		public int QuestID
		{
			get;
			set;
		}
	}
}
