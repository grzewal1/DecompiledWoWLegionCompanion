using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[FlexJamMessage(Id = 4856, Name = "MobileClientBountiesByWorldQuestUpdate", Version = 33577221u), DataContract]
	public class MobileClientBountiesByWorldQuestUpdate
	{
		[FlexJamMember(ArrayDimensions = 1, Name = "quest", Type = FlexJamType.Struct), DataMember(Name = "quest")]
		public MobileBountiesByWorldQuest[] Quest
		{
			get;
			set;
		}
	}
}
