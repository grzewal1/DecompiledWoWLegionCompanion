using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4877, Name="MobileClientWorldQuestBountiesResult", Version=39869590)]
	public class MobileClientWorldQuestBountiesResult
	{
		[DataMember(Name="bounty")]
		[FlexJamMember(ArrayDimensions=1, Name="bounty", Type=FlexJamType.Struct)]
		public MobileWorldQuestBounty[] Bounty
		{
			get;
			set;
		}

		[DataMember(Name="lockedQuestID")]
		[FlexJamMember(Name="lockedQuestID", Type=FlexJamType.Int32)]
		public int LockedQuestID
		{
			get;
			set;
		}

		[DataMember(Name="visible")]
		[FlexJamMember(Name="visible", Type=FlexJamType.Bool)]
		public bool Visible
		{
			get;
			set;
		}

		public MobileClientWorldQuestBountiesResult()
		{
		}
	}
}