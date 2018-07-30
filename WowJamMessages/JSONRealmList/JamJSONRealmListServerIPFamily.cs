using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.JSONRealmList
{
	[DataContract]
	[FlexJamStruct(Name="JamJSONRealmListServerIPFamily", Version=47212487)]
	public class JamJSONRealmListServerIPFamily
	{
		[DataMember(Name="addresses")]
		[FlexJamMember(ArrayDimensions=1, Name="addresses", Type=FlexJamType.Struct)]
		public JamJSONRealmListServerIPAddress[] Addresses
		{
			get;
			set;
		}

		[DataMember(Name="family")]
		[FlexJamMember(Name="family", Type=FlexJamType.Int8)]
		public sbyte Family
		{
			get;
			set;
		}

		public JamJSONRealmListServerIPFamily()
		{
		}
	}
}