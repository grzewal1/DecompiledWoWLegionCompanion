using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamPlayerGuidLookupData", Version=28333852)]
	public class JamPlayerGuidLookupData
	{
		[DataMember(Name="bnetAccount")]
		[FlexJamMember(Name="bnetAccount", Type=FlexJamType.WowGuid)]
		public string BnetAccount
		{
			get;
			set;
		}

		[DataMember(Name="classID")]
		[FlexJamMember(Name="classID", Type=FlexJamType.UInt8)]
		public byte ClassID
		{
			get;
			set;
		}

		[DataMember(Name="declinedNames")]
		[FlexJamMember(ArrayDimensions=1, Name="declinedNames", Type=FlexJamType.String)]
		public string[] DeclinedNames
		{
			get;
			set;
		}

		[DataMember(Name="guidActual")]
		[FlexJamMember(Name="guidActual", Type=FlexJamType.WowGuid)]
		public string GuidActual
		{
			get;
			set;
		}

		[DataMember(Name="isDeleted")]
		[FlexJamMember(Name="isDeleted", Type=FlexJamType.Bool)]
		public bool IsDeleted
		{
			get;
			set;
		}

		[DataMember(Name="level")]
		[FlexJamMember(Name="level", Type=FlexJamType.UInt8)]
		public byte Level
		{
			get;
			set;
		}

		[DataMember(Name="name")]
		[FlexJamMember(Name="name", Type=FlexJamType.String)]
		public string Name
		{
			get;
			set;
		}

		[DataMember(Name="race")]
		[FlexJamMember(Name="race", Type=FlexJamType.UInt8)]
		public byte Race
		{
			get;
			set;
		}

		[DataMember(Name="sex")]
		[FlexJamMember(Name="sex", Type=FlexJamType.UInt8)]
		public byte Sex
		{
			get;
			set;
		}

		[DataMember(Name="virtualRealmAddress")]
		[FlexJamMember(Name="virtualRealmAddress", Type=FlexJamType.UInt32)]
		public uint VirtualRealmAddress
		{
			get;
			set;
		}

		[DataMember(Name="wowAccount")]
		[FlexJamMember(Name="wowAccount", Type=FlexJamType.WowGuid)]
		public string WowAccount
		{
			get;
			set;
		}

		public JamPlayerGuidLookupData()
		{
			this.DeclinedNames = new string[5];
		}
	}
}