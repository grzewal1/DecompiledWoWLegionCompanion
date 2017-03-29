using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4882, Name="MobileClientArtifactInfoResult", Version=39869590)]
	public class MobileClientArtifactInfoResult
	{
		[DataMember(Name="knowledgeLevel")]
		[FlexJamMember(Name="knowledgeLevel", Type=FlexJamType.Int32)]
		public int KnowledgeLevel
		{
			get;
			set;
		}

		[DataMember(Name="xpMultiplier")]
		[FlexJamMember(Name="xpMultiplier", Type=FlexJamType.Float)]
		public float XpMultiplier
		{
			get;
			set;
		}

		public MobileClientArtifactInfoResult()
		{
		}
	}
}