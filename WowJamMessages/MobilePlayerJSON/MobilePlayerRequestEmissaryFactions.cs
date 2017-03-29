using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4789, Name="MobilePlayerRequestEmissaryFactions", Version=38820897)]
	public class MobilePlayerRequestEmissaryFactions
	{
		public MobilePlayerRequestEmissaryFactions()
		{
		}
	}
}