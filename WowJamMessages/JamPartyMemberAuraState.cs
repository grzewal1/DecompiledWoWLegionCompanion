using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamPartyMemberAuraState", Version=28333852)]
	public class JamPartyMemberAuraState
	{
		[DataMember(Name="activeFlags")]
		[FlexJamMember(Name="activeFlags", Type=FlexJamType.UInt32)]
		public uint ActiveFlags
		{
			get;
			set;
		}

		[DataMember(Name="aura")]
		[FlexJamMember(Name="aura", Type=FlexJamType.Int32)]
		public int Aura
		{
			get;
			set;
		}

		[DataMember(Name="flags")]
		[FlexJamMember(Name="flags", Type=FlexJamType.UInt8)]
		public byte Flags
		{
			get;
			set;
		}

		[DataMember(Name="points")]
		[FlexJamMember(ArrayDimensions=1, Name="points", Type=FlexJamType.Float)]
		public float[] Points
		{
			get;
			set;
		}

		public JamPartyMemberAuraState()
		{
		}
	}
}