using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamGarrisonFollowerCategoryInfo", Version=28333852)]
	public class JamGarrisonFollowerCategoryInfo
	{
		[DataMember(Name="classSpec")]
		[FlexJamMember(Name="classSpec", Type=FlexJamType.Int32)]
		public int ClassSpec
		{
			get;
			set;
		}

		[DataMember(Name="classSpecPlayerCondID")]
		[FlexJamMember(Name="classSpecPlayerCondID", Type=FlexJamType.Int32)]
		public int ClassSpecPlayerCondID
		{
			get;
			set;
		}

		public JamGarrisonFollowerCategoryInfo()
		{
		}
	}
}