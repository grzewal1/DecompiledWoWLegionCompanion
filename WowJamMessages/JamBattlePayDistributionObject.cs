using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamBattlePayDistributionObject", Version=28333852)]
	public class JamBattlePayDistributionObject
	{
		[DataMember(Name="deliverable")]
		[FlexJamMember(Optional=true, Name="deliverable", Type=FlexJamType.Struct)]
		public JamBattlePayDeliverable[] Deliverable
		{
			get;
			set;
		}

		[DataMember(Name="deliverableID")]
		[FlexJamMember(Name="deliverableID", Type=FlexJamType.UInt32)]
		public uint DeliverableID
		{
			get;
			set;
		}

		[DataMember(Name="distributionID")]
		[FlexJamMember(Name="distributionID", Type=FlexJamType.UInt64)]
		public ulong DistributionID
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

		[DataMember(Name="revoked")]
		[FlexJamMember(Name="revoked", Type=FlexJamType.Bool)]
		public bool Revoked
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

		[DataMember(Name="targetNativeRealm")]
		[FlexJamMember(Name="targetNativeRealm", Type=FlexJamType.UInt32)]
		public uint TargetNativeRealm
		{
			get;
			set;
		}

		[DataMember(Name="targetPlayer")]
		[FlexJamMember(Name="targetPlayer", Type=FlexJamType.WowGuid)]
		public string TargetPlayer
		{
			get;
			set;
		}

		[DataMember(Name="targetVirtualRealm")]
		[FlexJamMember(Name="targetVirtualRealm", Type=FlexJamType.UInt32)]
		public uint TargetVirtualRealm
		{
			get;
			set;
		}

		public JamBattlePayDistributionObject()
		{
		}
	}
}