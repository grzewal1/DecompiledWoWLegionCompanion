using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4817, Name="MobilePlayerConsumeArtifactKnowledgeItem", Version=38820897)]
	public class MobilePlayerConsumeArtifactKnowledgeItem
	{
		public MobilePlayerConsumeArtifactKnowledgeItem()
		{
		}
	}
}