using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="PhaseShiftDataVersion1", Version=28333852)]
	public class PhaseShiftDataVersion1
	{
		[DataMember(Name="personalGUID")]
		[FlexJamMember(Name="personalGUID", Type=FlexJamType.WowGuid)]
		public string PersonalGUID
		{
			get;
			set;
		}

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

		public PhaseShiftDataVersion1()
		{
		}
	}
}