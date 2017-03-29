using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileJSON
{
	[DataContract]
	[FlexJamMessage(Id=4743, Name="MobileServerConnect", Version=28333852)]
	public class MobileServerConnect
	{
		[DataMember(Name="build")]
		[FlexJamMember(Name="build", Type=FlexJamType.UInt16)]
		public ushort Build
		{
			get;
			set;
		}

		[DataMember(Name="buildType")]
		[FlexJamMember(Name="buildType", Type=FlexJamType.UInt32)]
		public uint BuildType
		{
			get;
			set;
		}

		[DataMember(Name="characterID")]
		[FlexJamMember(Name="characterID", Type=FlexJamType.WowGuid)]
		public string CharacterID
		{
			get;
			set;
		}

		[DataMember(Name="clientChallenge")]
		[FlexJamMember(ArrayDimensions=1, Name="clientChallenge", Type=FlexJamType.UInt8)]
		public byte[] ClientChallenge
		{
			get;
			set;
		}

		[DataMember(Name="joinTicket")]
		[FlexJamMember(ArrayDimensions=1, Name="joinTicket", Type=FlexJamType.UInt8)]
		public byte[] JoinTicket
		{
			get;
			set;
		}

		[DataMember(Name="proof")]
		[FlexJamMember(ArrayDimensions=1, Name="proof", Type=FlexJamType.UInt8)]
		public byte[] Proof
		{
			get;
			set;
		}

		[DataMember(Name="realmAddress")]
		[FlexJamMember(Name="realmAddress", Type=FlexJamType.UInt32)]
		public uint RealmAddress
		{
			get;
			set;
		}

		public MobileServerConnect()
		{
			unsafe
			{
				this.ClientChallenge = new byte[16];
				this.Proof = new byte[24];
			}
		}
	}
}