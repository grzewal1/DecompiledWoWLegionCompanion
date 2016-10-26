using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[FlexJamMessage(Id = 4867, Name = "MobileClientWorldQuestInactiveBountiesResult", Version = 33577221u), DataContract]
	public class MobileClientWorldQuestInactiveBountiesResult
	{
		[FlexJamMember(ArrayDimensions = 1, Name = "bounty", Type = FlexJamType.Struct), DataMember(Name = "bounty")]
		public MobileWorldQuestBounty[] Bounty
		{
			get;
			set;
		}
	}
}
