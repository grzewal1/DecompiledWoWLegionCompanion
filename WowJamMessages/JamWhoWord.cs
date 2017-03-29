using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamWhoWord", Version=28333852)]
	public class JamWhoWord
	{
		[DataMember(Name="word")]
		[FlexJamMember(Name="word", Type=FlexJamType.String)]
		public string Word
		{
			get;
			set;
		}

		public JamWhoWord()
		{
		}
	}
}