using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4794, Name="MobilePlayerRequestWorldQuests", Version=38820897)]
	public class MobilePlayerRequestWorldQuests
	{
		public MobilePlayerRequestWorldQuests()
		{
		}
	}
}