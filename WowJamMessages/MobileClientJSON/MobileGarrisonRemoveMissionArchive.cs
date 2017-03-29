using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4849, Name="MobileGarrisonRemoveMissionArchive", Version=39869590)]
	public class MobileGarrisonRemoveMissionArchive
	{
		[DataMember(Name="garrTypeID")]
		[FlexJamMember(Name="garrTypeID", Type=FlexJamType.Int32)]
		public int GarrTypeID
		{
			get;
			set;
		}

		[DataMember(Name="missionID")]
		[FlexJamMember(Name="missionID", Type=FlexJamType.Int32)]
		public int MissionID
		{
			get;
			set;
		}

		[DataMember(Name="result")]
		[FlexJamMember(Name="result", Type=FlexJamType.Int32)]
		public int Result
		{
			get;
			set;
		}

		public MobileGarrisonRemoveMissionArchive()
		{
		}
	}
}