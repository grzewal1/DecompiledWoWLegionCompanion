using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4878, Name="MobileClientWorldQuestInactiveBountiesResult", Version=39869590)]
	public class MobileClientWorldQuestInactiveBountiesResult
	{
		[DataMember(Name="bounty")]
		[FlexJamMember(ArrayDimensions=1, Name="bounty", Type=FlexJamType.Struct)]
		public MobileWorldQuestBounty[] Bounty
		{
			get;
			set;
		}

		public MobileClientWorldQuestInactiveBountiesResult()
		{
		}
	}
}