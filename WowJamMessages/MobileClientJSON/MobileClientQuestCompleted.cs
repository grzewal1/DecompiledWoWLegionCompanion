using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4890, Name="MobileClientQuestCompleted", Version=39869590)]
	public class MobileClientQuestCompleted
	{
		[DataMember(Name="item")]
		[FlexJamMember(ArrayDimensions=1, Name="item", Type=FlexJamType.Struct)]
		public MobileQuestItem[] Item
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

		public MobileClientQuestCompleted()
		{
		}
	}
}