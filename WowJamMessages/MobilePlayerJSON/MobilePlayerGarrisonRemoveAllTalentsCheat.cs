using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4815, Name="MobilePlayerGarrisonRemoveAllTalentsCheat", Version=38820897)]
	public class MobilePlayerGarrisonRemoveAllTalentsCheat
	{
		public MobilePlayerGarrisonRemoveAllTalentsCheat()
		{
		}
	}
}