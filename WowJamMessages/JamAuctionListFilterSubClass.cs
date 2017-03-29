using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamAuctionListFilterSubClass", Version=28333852)]
	public class JamAuctionListFilterSubClass
	{
		[DataMember(Name="invTypeMask")]
		[FlexJamMember(Name="invTypeMask", Type=FlexJamType.UInt32)]
		public uint InvTypeMask
		{
			get;
			set;
		}

		[DataMember(Name="itemSubclass")]
		[FlexJamMember(Name="itemSubclass", Type=FlexJamType.Int32)]
		public int ItemSubclass
		{
			get;
			set;
		}

		public JamAuctionListFilterSubClass()
		{
		}
	}
}