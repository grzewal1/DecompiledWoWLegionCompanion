using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamStruct(Name="MobileWorldQuestObjective", Version=39869590)]
	public class MobileWorldQuestObjective
	{
		[DataMember(Name="text")]
		[FlexJamMember(Name="text", Type=FlexJamType.String)]
		public string Text
		{
			get;
			set;
		}

		public MobileWorldQuestObjective()
		{
		}
	}
}