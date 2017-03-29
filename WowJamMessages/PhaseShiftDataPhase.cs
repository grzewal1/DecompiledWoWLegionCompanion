using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="PhaseShiftDataPhase", Version=28333852)]
	public class PhaseShiftDataPhase
	{
		[DataMember(Name="id")]
		[FlexJamMember(Name="id", Type=FlexJamType.UInt16)]
		public ushort Id
		{
			get;
			set;
		}

		[DataMember(Name="phaseFlags")]
		[FlexJamMember(Name="phaseFlags", Type=FlexJamType.UInt16)]
		public ushort PhaseFlags
		{
			get;
			set;
		}

		public PhaseShiftDataPhase()
		{
		}
	}
}