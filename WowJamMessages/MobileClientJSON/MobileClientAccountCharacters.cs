using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4858, Name="MobileClientAccountCharacters", Version=39869590)]
	public class MobileClientAccountCharacters
	{
		[DataMember(Name="characters")]
		[FlexJamMember(ArrayDimensions=1, Name="characters", Type=FlexJamType.Struct)]
		public MobilePlayerCharacter[] Characters
		{
			get;
			set;
		}

		public MobileClientAccountCharacters()
		{
		}
	}
}