using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4865, Name="MobileClientWorldQuestUpdate", Version=39869590)]
	public class MobileClientWorldQuestUpdate
	{
		[DataMember(Name="quest")]
		[FlexJamMember(ArrayDimensions=1, Name="quest", Type=FlexJamType.Struct)]
		public MobileWorldQuest[] Quest
		{
			get;
			set;
		}

		public MobileClientWorldQuestUpdate()
		{
		}
	}
}