using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4810, Name="MobilePlayerGetArtifactInfo", Version=38820897)]
	public class MobilePlayerGetArtifactInfo
	{
		public MobilePlayerGetArtifactInfo()
		{
		}
	}
}