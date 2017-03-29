using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileCharacterJSON
{
	[DataContract]
	[FlexJamMessage(Id=4820, Name="MobileCharacterTest", Version=28333852)]
	public class MobileCharacterTest
	{
		public MobileCharacterTest()
		{
		}
	}
}