using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4787, Name="MobilePlayerGarrisonAdvanceMissionSet", Version=38820897)]
	public class MobilePlayerGarrisonAdvanceMissionSet
	{
		[DataMember(Name="garrTypeID")]
		[FlexJamMember(Name="garrTypeID", Type=FlexJamType.Int32)]
		public int GarrTypeID
		{
			get;
			set;
		}

		[DataMember(Name="missionSetID")]
		[FlexJamMember(Name="missionSetID", Type=FlexJamType.Int32)]
		public int MissionSetID
		{
			get;
			set;
		}

		public MobilePlayerGarrisonAdvanceMissionSet()
		{
		}
	}
}