using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4804, Name="MobilePlayerWorldQuestBountiesRequest", Version=38820897)]
	public class MobilePlayerWorldQuestBountiesRequest
	{
		public MobilePlayerWorldQuestBountiesRequest()
		{
		}
	}
}