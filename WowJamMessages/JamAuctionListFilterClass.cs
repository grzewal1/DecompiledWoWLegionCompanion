using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamAuctionListFilterClass", Version=28333852)]
	public class JamAuctionListFilterClass
	{
		[DataMember(Name="itemClass")]
		[FlexJamMember(Name="itemClass", Type=FlexJamType.Int32)]
		public int ItemClass
		{
			get;
			set;
		}

		[DataMember(Name="subClasses")]
		[FlexJamMember(ArrayDimensions=1, Name="subClasses", Type=FlexJamType.Struct)]
		public JamAuctionListFilterSubClass[] SubClasses
		{
			get;
			set;
		}

		public JamAuctionListFilterClass()
		{
		}
	}
}