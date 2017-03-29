using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4796, Name="MobilePlayerSetMissionDurationCheat", Version=38820897)]
	public class MobilePlayerSetMissionDurationCheat
	{
		[DataMember(Name="seconds")]
		[FlexJamMember(Name="seconds", Type=FlexJamType.Int32)]
		public int Seconds
		{
			get;
			set;
		}

		public MobilePlayerSetMissionDurationCheat()
		{
		}
	}
}