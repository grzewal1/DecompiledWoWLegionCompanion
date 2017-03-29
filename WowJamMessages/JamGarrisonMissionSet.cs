using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamGarrisonMissionSet", Version=28333852)]
	public class JamGarrisonMissionSet
	{
		[DataMember(Name="garrMissionSetID")]
		[FlexJamMember(Name="garrMissionSetID", Type=FlexJamType.Int32)]
		public int GarrMissionSetID
		{
			get;
			set;
		}

		[DataMember(Name="lastUpdateTime")]
		[FlexJamMember(Name="lastUpdateTime", Type=FlexJamType.Int32)]
		public int LastUpdateTime
		{
			get;
			set;
		}

		public JamGarrisonMissionSet()
		{
		}
	}
}