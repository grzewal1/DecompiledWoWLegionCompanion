using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4850, Name="MobileClientPong", Version=39869590)]
	public class MobileClientPong
	{
		public MobileClientPong()
		{
		}
	}
}