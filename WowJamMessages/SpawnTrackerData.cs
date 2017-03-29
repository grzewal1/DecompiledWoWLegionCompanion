using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="SpawnTrackerData", Version=28333852)]
	public class SpawnTrackerData
	{
		[DataMember(Name="questID")]
		[FlexJamMember(ArrayDimensions=1, Name="questID", Type=FlexJamType.Int32)]
		public int[] QuestID
		{
			get;
			set;
		}

		public SpawnTrackerData()
		{
		}
	}
}