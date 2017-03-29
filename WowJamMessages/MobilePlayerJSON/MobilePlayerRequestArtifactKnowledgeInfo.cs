using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4816, Name="MobilePlayerRequestArtifactKnowledgeInfo", Version=38820897)]
	public class MobilePlayerRequestArtifactKnowledgeInfo
	{
		public MobilePlayerRequestArtifactKnowledgeInfo()
		{
		}
	}
}