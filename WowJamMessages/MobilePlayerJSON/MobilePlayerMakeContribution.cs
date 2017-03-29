using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4812, Name="MobilePlayerMakeContribution", Version=38820897)]
	public class MobilePlayerMakeContribution
	{
		[DataMember(Name="contributionID")]
		[FlexJamMember(Name="contributionID", Type=FlexJamType.Int32)]
		public int ContributionID
		{
			get;
			set;
		}

		public MobilePlayerMakeContribution()
		{
		}
	}
}