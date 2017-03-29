using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.JSONRealmList
{
	[DataContract]
	[FlexJamMessage(Id=15031, Name="JSONRealmListUpdates", Version=28333852)]
	public class JSONRealmListUpdates
	{
		[DataMember(Name="updates")]
		[FlexJamMember(ArrayDimensions=1, Name="updates", Type=FlexJamType.Struct)]
		public JamJSONRealmListUpdatePart[] Updates
		{
			get;
			set;
		}

		public JSONRealmListUpdates()
		{
		}
	}
}