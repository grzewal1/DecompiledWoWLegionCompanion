using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4782, Name="MobilePlayerGarrisonCompleteMission", Version=38820897)]
	public class MobilePlayerGarrisonCompleteMission
	{
		[DataMember(Name="garrMissionID")]
		[FlexJamMember(Name="garrMissionID", Type=FlexJamType.Int32)]
		public int GarrMissionID
		{
			get;
			set;
		}

		public MobilePlayerGarrisonCompleteMission()
		{
		}
	}
}