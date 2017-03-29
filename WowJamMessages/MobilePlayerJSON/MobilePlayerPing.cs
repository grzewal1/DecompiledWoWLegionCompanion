using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4784, Name="MobilePlayerPing", Version=38820897)]
	public class MobilePlayerPing
	{
		public MobilePlayerPing()
		{
		}
	}
}