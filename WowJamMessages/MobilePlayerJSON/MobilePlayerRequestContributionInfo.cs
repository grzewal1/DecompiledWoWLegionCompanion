using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4811, Name="MobilePlayerRequestContributionInfo", Version=38820897)]
	public class MobilePlayerRequestContributionInfo
	{
		public MobilePlayerRequestContributionInfo()
		{
		}
	}
}