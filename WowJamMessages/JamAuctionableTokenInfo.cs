using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamAuctionableTokenInfo", Version=28333852)]
	public class JamAuctionableTokenInfo
	{
		[DataMember(Name="expectedSecondsUntilSold")]
		[FlexJamMember(Name="expectedSecondsUntilSold", Type=FlexJamType.UInt32)]
		public uint ExpectedSecondsUntilSold
		{
			get;
			set;
		}

		[DataMember(Name="id")]
		[FlexJamMember(Name="id", Type=FlexJamType.UInt64)]
		public ulong Id
		{
			get;
			set;
		}

		[DataMember(Name="lastUpdate")]
		[FlexJamMember(Name="lastUpdate", Type=FlexJamType.Int32)]
		public int LastUpdate
		{
			get;
			set;
		}

		[DataMember(Name="price")]
		[FlexJamMember(Name="price", Type=FlexJamType.UInt64)]
		public ulong Price
		{
			get;
			set;
		}

		[DataMember(Name="status")]
		[FlexJamMember(Name="status", Type=FlexJamType.Int32)]
		public int Status
		{
			get;
			set;
		}

		public JamAuctionableTokenInfo()
		{
		}
	}
}