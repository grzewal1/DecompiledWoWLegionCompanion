using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4885, Name="MobileClientRequestContributionInfoResult", Version=39869590)]
	public class MobileClientRequestContributionInfoResult
	{
		[DataMember(Name="contribution")]
		[FlexJamMember(ArrayDimensions=1, Name="contribution", Type=FlexJamType.Struct)]
		public MobileContribution[] Contribution
		{
			get;
			set;
		}

		[DataMember(Name="hasAccess")]
		[FlexJamMember(Name="hasAccess", Type=FlexJamType.Bool)]
		public bool HasAccess
		{
			get;
			set;
		}

		[DataMember(Name="legionfallWarResources")]
		[FlexJamMember(Name="legionfallWarResources", Type=FlexJamType.Int32)]
		public int LegionfallWarResources
		{
			get;
			set;
		}

		public MobileClientRequestContributionInfoResult()
		{
		}
	}
}