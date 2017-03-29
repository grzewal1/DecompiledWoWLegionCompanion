using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamWhoResponse", Version=28333852)]
	public class JamWhoResponse
	{
		[DataMember(Name="entries")]
		[FlexJamMember(ArrayDimensions=1, Name="entries", Type=FlexJamType.Struct)]
		public JamWhoEntry[] Entries
		{
			get;
			set;
		}

		public JamWhoResponse()
		{
		}
	}
}