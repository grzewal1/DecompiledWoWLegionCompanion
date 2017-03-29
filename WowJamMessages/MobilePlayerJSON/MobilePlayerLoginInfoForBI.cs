using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4807, Name="MobilePlayerLoginInfoForBI", Version=38820897)]
	public class MobilePlayerLoginInfoForBI
	{
		public MobilePlayerLoginInfoForBI()
		{
		}
	}
}