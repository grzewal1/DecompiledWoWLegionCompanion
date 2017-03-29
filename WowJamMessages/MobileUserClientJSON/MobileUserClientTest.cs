using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileUserClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4760, Name="MobileUserClientTest", Version=28333852)]
	public class MobileUserClientTest
	{
		public MobileUserClientTest()
		{
		}
	}
}