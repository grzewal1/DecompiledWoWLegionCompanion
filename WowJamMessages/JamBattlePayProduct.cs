using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamBattlePayProduct", Version=28333852)]
	public class JamBattlePayProduct
	{
		[DataMember(Name="currentPriceFixedPoint")]
		[FlexJamMember(Name="currentPriceFixedPoint", Type=FlexJamType.UInt64)]
		public ulong CurrentPriceFixedPoint
		{
			get;
			set;
		}

		[DataMember(Name="deliverables")]
		[FlexJamMember(ArrayDimensions=1, Name="deliverables", Type=FlexJamType.UInt32)]
		public uint[] Deliverables
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

		[DataMember(Name="flags")]
		[FlexJamMember(Name="flags", Type=FlexJamType.UInt32)]
		public uint Flags
		{
			get;
			set;
		}

		[DataMember(Name="normalPriceFixedPoint")]
		[FlexJamMember(Name="normalPriceFixedPoint", Type=FlexJamType.UInt64)]
		public ulong NormalPriceFixedPoint
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

		[DataMember(Name="type")]
		[FlexJamMember(Name="type", Type=FlexJamType.UInt8)]
		public byte Type
		{
			get;
			set;
		}

		public JamBattlePayProduct()
		{
		}
	}
}