using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4853, Name="MobileClientAdvanceMissionSetResult", Version=39869590)]
	public class MobileClientAdvanceMissionSetResult
	{
		[DataMember(Name="missionSetID")]
		[FlexJamMember(Name="missionSetID", Type=FlexJamType.Int32)]
		public int MissionSetID
		{
			get;
			set;
		}

		[DataMember(Name="success")]
		[FlexJamMember(Name="success", Type=FlexJamType.Bool)]
		public bool Success
		{
			get;
			set;
		}

		public MobileClientAdvanceMissionSetResult()
		{
		}
	}
}