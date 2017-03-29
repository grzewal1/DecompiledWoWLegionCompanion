using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.JSONRealmList
{
	[DataContract]
	[FlexJamMessage(Id=15033, Name="JSONRealmListServerIPAddresses", Version=28333852)]
	public class JSONRealmListServerIPAddresses
	{
		[DataMember(Name="families")]
		[FlexJamMember(ArrayDimensions=1, Name="families", Type=FlexJamType.Struct)]
		public JamJSONRealmListServerIPFamily[] Families
		{
			get;
			set;
		}

		public JSONRealmListServerIPAddresses()
		{
		}
	}
}