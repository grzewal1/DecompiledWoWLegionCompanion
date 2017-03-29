using bnet.protocol;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using WowJamMessages.JSONRealmList;

namespace WowJamMessages
{
	[DataContract]
	public class RecentCharacter
	{
		[DataMember(Name="entry")]
		public JamJSONCharacterEntry Entry
		{
			get;
			set;
		}

		[DataMember(Name="gameAccount")]
		public EntityId GameAccount
		{
			get;
			set;
		}

		[DataMember(Name="subRegion")]
		public string SubRegion
		{
			get;
			set;
		}

		[DataMember(Name="unixTime")]
		public int UnixTime
		{
			get;
			set;
		}

		[DataMember(Name="version")]
		public int Version
		{
			get;
			set;
		}

		[DataMember(Name="webToken")]
		public string WebToken
		{
			get;
			set;
		}

		public RecentCharacter()
		{
		}
	}
}