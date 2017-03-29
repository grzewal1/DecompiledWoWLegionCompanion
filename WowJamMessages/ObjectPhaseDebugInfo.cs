using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="ObjectPhaseDebugInfo", Version=28333852)]
	public class ObjectPhaseDebugInfo
	{
		[DataMember(Name="phaseID")]
		[FlexJamMember(Name="phaseID", Type=FlexJamType.Int32)]
		public int PhaseID
		{
			get;
			set;
		}

		[DataMember(Name="phaseName")]
		[FlexJamMember(Name="phaseName", Type=FlexJamType.String)]
		public string PhaseName
		{
			get;
			set;
		}

		public ObjectPhaseDebugInfo()
		{
		}
	}
}