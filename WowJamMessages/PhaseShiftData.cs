using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="PhaseShiftData", Version=28333852)]
	public class PhaseShiftData
	{
		[DataMember(Name="personalGUID")]
		[FlexJamMember(Name="personalGUID", Type=FlexJamType.WowGuid)]
		public string PersonalGUID
		{
			get;
			set;
		}

		[DataMember(Name="phases")]
		[FlexJamMember(ArrayDimensions=1, Name="phases", Type=FlexJamType.Struct)]
		public PhaseShiftDataPhase[] Phases
		{
			get;
			set;
		}

		[DataMember(Name="phaseShiftFlags")]
		[FlexJamMember(Name="phaseShiftFlags", Type=FlexJamType.UInt32)]
		public uint PhaseShiftFlags
		{
			get;
			set;
		}

		public PhaseShiftData()
		{
		}
	}
}