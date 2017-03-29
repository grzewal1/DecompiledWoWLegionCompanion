using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.JSONRealmList
{
	[DataContract]
	[FlexJamMessage(Id=15036, Name="JSONRealmCharacterList", Version=28333852)]
	public class JSONRealmCharacterList
	{
		[DataMember(Name="characterList")]
		[FlexJamMember(ArrayDimensions=1, Name="characterList", Type=FlexJamType.Struct)]
		public JamJSONCharacterEntry[] CharacterList
		{
			get;
			set;
		}

		public JSONRealmCharacterList()
		{
		}
	}
}