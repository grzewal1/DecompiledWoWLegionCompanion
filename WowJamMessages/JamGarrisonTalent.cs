using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamGarrisonTalent", Version=28333852)]
	public class JamGarrisonTalent
	{
		[DataMember(Name="flags")]
		[FlexJamMember(Name="flags", Type=FlexJamType.Int32)]
		public int Flags
		{
			get;
			set;
		}

		[DataMember(Name="garrTalentID")]
		[FlexJamMember(Name="garrTalentID", Type=FlexJamType.Int32)]
		public int GarrTalentID
		{
			get;
			set;
		}

		[DataMember(Name="startTime")]
		[FlexJamMember(Name="startTime", Type=FlexJamType.Int32)]
		public int StartTime
		{
			get;
			set;
		}

		public JamGarrisonTalent()
		{
		}
	}
}