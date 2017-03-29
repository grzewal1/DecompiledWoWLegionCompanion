using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4818, Name="MobilePlayerRequestMaxFollowers", Version=38820897)]
	public class MobilePlayerRequestMaxFollowers
	{
		public MobilePlayerRequestMaxFollowers()
		{
		}
	}
}