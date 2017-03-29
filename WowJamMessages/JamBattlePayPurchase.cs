using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamBattlePayPurchase", Version=28333852)]
	public class JamBattlePayPurchase
	{
		[DataMember(Name="productID")]
		[FlexJamMember(Name="productID", Type=FlexJamType.UInt32)]
		public uint ProductID
		{
			get;
			set;
		}

		[DataMember(Name="purchaseID")]
		[FlexJamMember(Name="purchaseID", Type=FlexJamType.UInt64)]
		public ulong PurchaseID
		{
			get;
			set;
		}

		[DataMember(Name="resultCode")]
		[FlexJamMember(Name="resultCode", Type=FlexJamType.UInt32)]
		public uint ResultCode
		{
			get;
			set;
		}

		[DataMember(Name="status")]
		[FlexJamMember(Name="status", Type=FlexJamType.UInt32)]
		public uint Status
		{
			get;
			set;
		}

		[DataMember(Name="walletName")]
		[FlexJamMember(Name="walletName", Type=FlexJamType.String)]
		public string WalletName
		{
			get;
			set;
		}

		public JamBattlePayPurchase()
		{
		}
	}
}