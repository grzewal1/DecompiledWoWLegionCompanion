using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4814, Name="MobilePlayerGarrisonCompleteAllTalentsCheat", Version=38820897)]
	public class MobilePlayerGarrisonCompleteAllTalentsCheat
	{
		public MobilePlayerGarrisonCompleteAllTalentsCheat()
		{
		}
	}
}