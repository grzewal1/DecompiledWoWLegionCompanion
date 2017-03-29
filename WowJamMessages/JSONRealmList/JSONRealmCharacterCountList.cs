using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.JSONRealmList
{
	[DataContract]
	[FlexJamMessage(Id=15030, Name="JSONRealmCharacterCountList", Version=28333852)]
	public class JSONRealmCharacterCountList
	{
		[DataMember(Name="counts")]
		[FlexJamMember(ArrayDimensions=1, Name="counts", Type=FlexJamType.Struct)]
		public JamJSONRealmCharacterCount[] Counts
		{
			get;
			set;
		}

		public JSONRealmCharacterCountList()
		{
		}
	}
}