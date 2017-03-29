using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamPartyMemberPetState", Version=28333852)]
	public class JamPartyMemberPetState
	{
		[DataMember(Name="auras")]
		[FlexJamMember(ArrayDimensions=1, Name="auras", Type=FlexJamType.Struct)]
		public JamPartyMemberAuraState[] Auras
		{
			get;
			set;
		}

		[DataMember(Name="displayID")]
		[FlexJamMember(Name="displayID", Type=FlexJamType.Int32)]
		public int DisplayID
		{
			get;
			set;
		}

		[DataMember(Name="guid")]
		[FlexJamMember(Name="guid", Type=FlexJamType.WowGuid)]
		public string Guid
		{
			get;
			set;
		}

		[DataMember(Name="health")]
		[FlexJamMember(Name="health", Type=FlexJamType.Int32)]
		public int Health
		{
			get;
			set;
		}

		[DataMember(Name="maxHealth")]
		[FlexJamMember(Name="maxHealth", Type=FlexJamType.Int32)]
		public int MaxHealth
		{
			get;
			set;
		}

		[DataMember(Name="name")]
		[FlexJamMember(Name="name", Type=FlexJamType.String)]
		public string Name
		{
			get;
			set;
		}

		public JamPartyMemberPetState()
		{
			this.Guid = "0000000000000000";
		}
	}
}