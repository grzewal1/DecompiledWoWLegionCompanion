using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4866, Name="MobileClientBountiesByWorldQuestUpdate", Version=39869590)]
	public class MobileClientBountiesByWorldQuestUpdate
	{
		[DataMember(Name="quest")]
		[FlexJamMember(ArrayDimensions=1, Name="quest", Type=FlexJamType.Struct)]
		public MobileBountiesByWorldQuest[] Quest
		{
			get;
			set;
		}

		public MobileClientBountiesByWorldQuestUpdate()
		{
		}
	}
}