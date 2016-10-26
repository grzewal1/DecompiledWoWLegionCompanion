using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[FlexJamMessage(Id = 4855, Name = "MobileClientWorldQuestUpdate", Version = 33577221u), DataContract]
	public class MobileClientWorldQuestUpdate
	{
		[FlexJamMember(ArrayDimensions = 1, Name = "quest", Type = FlexJamType.Struct), DataMember(Name = "quest")]
		public MobileWorldQuest[] Quest
		{
			get;
			set;
		}
	}
}
