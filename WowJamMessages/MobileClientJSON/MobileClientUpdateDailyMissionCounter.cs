using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4844, Name="MobileClientUpdateDailyMissionCounter", Version=39869590)]
	public class MobileClientUpdateDailyMissionCounter
	{
		[DataMember(Name="count")]
		[FlexJamMember(Name="count", Type=FlexJamType.UInt16)]
		public ushort Count
		{
			get;
			set;
		}

		[DataMember(Name="garrTypeID")]
		[FlexJamMember(Name="garrTypeID", Type=FlexJamType.Int32)]
		public int GarrTypeID
		{
			get;
			set;
		}

		public MobileClientUpdateDailyMissionCounter()
		{
		}
	}
}