using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4886, Name="MobileClientMakeContributionResult", Version=39869590)]
	public class MobileClientMakeContributionResult
	{
		[DataMember(Name="contributionID")]
		[FlexJamMember(Name="contributionID", Type=FlexJamType.Int32)]
		public int ContributionID
		{
			get;
			set;
		}

		[DataMember(Name="result")]
		[FlexJamMember(Name="result", Type=FlexJamType.Bool)]
		public bool Result
		{
			get;
			set;
		}

		public MobileClientMakeContributionResult()
		{
		}
	}
}