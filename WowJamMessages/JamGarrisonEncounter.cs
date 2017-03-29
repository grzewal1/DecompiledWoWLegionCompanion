using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamGarrisonEncounter", Version=28333852)]
	public class JamGarrisonEncounter
	{
		[DataMember(Name="encounterID")]
		[FlexJamMember(Name="encounterID", Type=FlexJamType.Int32)]
		public int EncounterID
		{
			get;
			set;
		}

		[DataMember(Name="mechanicID")]
		[FlexJamMember(ArrayDimensions=1, Name="mechanicID", Type=FlexJamType.Int32)]
		public int[] MechanicID
		{
			get;
			set;
		}

		public JamGarrisonEncounter()
		{
		}
	}
}