using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileJSON
{
	[DataContract]
	[FlexJamMessage(Id=4741, Name="MobileServerDevLogin", Version=28333852)]
	public class MobileServerDevLogin
	{
		[DataMember(Name="bnetAccount")]
		[FlexJamMember(Name="bnetAccount", Type=FlexJamType.WowGuid)]
		public string BnetAccount
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

		[DataMember(Name="locale")]
		[FlexJamMember(Name="locale", Type=FlexJamType.String)]
		public string Locale
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

		public MobileServerDevLogin()
		{
		}
	}
}