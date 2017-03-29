using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="PhaseShiftDataVersion0", Version=28333852)]
	public class PhaseShiftDataVersion0
	{
		[DataMember(Name="phaseID")]
		[FlexJamMember(ArrayDimensions=1, Name="phaseID", Type=FlexJamType.UInt16)]
		public ushort[] PhaseID
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

		public PhaseShiftDataVersion0()
		{
		}
	}
}