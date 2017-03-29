using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamBattlePayShopEntry", Version=28333852)]
	public class JamBattlePayShopEntry
	{
		[DataMember(Name="bannerType")]
		[FlexJamMember(Name="bannerType", Type=FlexJamType.UInt8)]
		public byte BannerType
		{
			get;
			set;
		}

		[DataMember(Name="displayInfo")]
		[FlexJamMember(Optional=true, Name="displayInfo", Type=FlexJamType.Struct)]
		public JamBattlepayDisplayInfo[] DisplayInfo
		{
			get;
			set;
		}

		[DataMember(Name="entryID")]
		[FlexJamMember(Name="entryID", Type=FlexJamType.UInt32)]
		public uint EntryID
		{
			get;
			set;
		}

		[DataMember(Name="flags")]
		[FlexJamMember(Name="flags", Type=FlexJamType.UInt32)]
		public uint Flags
		{
			get;
			set;
		}

		[DataMember(Name="groupID")]
		[FlexJamMember(Name="groupID", Type=FlexJamType.UInt32)]
		public uint GroupID
		{
			get;
			set;
		}

		[DataMember(Name="ordering")]
		[FlexJamMember(Name="ordering", Type=FlexJamType.Int32)]
		public int Ordering
		{
			get;
			set;
		}

		[DataMember(Name="productID")]
		[FlexJamMember(Name="productID", Type=FlexJamType.UInt32)]
		public uint ProductID
		{
			get;
			set;
		}

		public JamBattlePayShopEntry()
		{
		}
	}
}