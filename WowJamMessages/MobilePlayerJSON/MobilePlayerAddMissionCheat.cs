using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4785, Name="MobilePlayerAddMissionCheat", Version=38820897)]
	public class MobilePlayerAddMissionCheat
	{
		[DataMember(Name="garrMissionID")]
		[FlexJamMember(Name="garrMissionID", Type=FlexJamType.Int32)]
		public int GarrMissionID
		{
			get;
			set;
		}

		public MobilePlayerAddMissionCheat()
		{
		}
	}
}