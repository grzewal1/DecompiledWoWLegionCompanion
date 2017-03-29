using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileJSON
{
	[DataContract]
	[FlexJamMessage(Id=4742, Name="MobileServerSetCharacter", Version=28333852)]
	public class MobileServerSetCharacter
	{
		[DataMember(Name="characterGUID")]
		[FlexJamMember(Name="characterGUID", Type=FlexJamType.WowGuid)]
		public string CharacterGUID
		{
			get;
			set;
		}

		public MobileServerSetCharacter()
		{
		}
	}
}