using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4781, Name="MobilePlayerGarrisonStartMission", Version=38820897)]
	public class MobilePlayerGarrisonStartMission
	{
		[DataMember(Name="followerDBIDs")]
		[FlexJamMember(ArrayDimensions=1, Name="followerDBIDs", Type=FlexJamType.UInt64)]
		public ulong[] FollowerDBIDs
		{
			get;
			set;
		}

		[DataMember(Name="garrMissionID")]
		[FlexJamMember(Name="garrMissionID", Type=FlexJamType.Int32)]
		public int GarrMissionID
		{
			get;
			set;
		}

		public MobilePlayerGarrisonStartMission()
		{
		}
	}
}