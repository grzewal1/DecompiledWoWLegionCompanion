using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4888, Name="MobileClientArtifactKnowledgeInfoResult", Version=39869590)]
	public class MobileClientArtifactKnowledgeInfoResult
	{
		[DataMember(Name="activeShipments")]
		[FlexJamMember(Name="activeShipments", Type=FlexJamType.Int32)]
		public int ActiveShipments
		{
			get;
			set;
		}

		[DataMember(Name="currentLevel")]
		[FlexJamMember(Name="currentLevel", Type=FlexJamType.Int32)]
		public int CurrentLevel
		{
			get;
			set;
		}

		[DataMember(Name="itemsInBags")]
		[FlexJamMember(Name="itemsInBags", Type=FlexJamType.Int32)]
		public int ItemsInBags
		{
			get;
			set;
		}

		[DataMember(Name="itemsInBank")]
		[FlexJamMember(Name="itemsInBank", Type=FlexJamType.Int32)]
		public int ItemsInBank
		{
			get;
			set;
		}

		[DataMember(Name="itemsInLoot")]
		[FlexJamMember(Name="itemsInLoot", Type=FlexJamType.Int32)]
		public int ItemsInLoot
		{
			get;
			set;
		}

		[DataMember(Name="itemsInMail")]
		[FlexJamMember(Name="itemsInMail", Type=FlexJamType.Int32)]
		public int ItemsInMail
		{
			get;
			set;
		}

		[DataMember(Name="maxLevel")]
		[FlexJamMember(Name="maxLevel", Type=FlexJamType.Int32)]
		public int MaxLevel
		{
			get;
			set;
		}

		public MobileClientArtifactKnowledgeInfoResult()
		{
		}
	}
}