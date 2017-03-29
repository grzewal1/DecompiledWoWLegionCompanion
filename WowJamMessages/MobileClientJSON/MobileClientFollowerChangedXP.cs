using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using WowJamMessages;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4851, Name="MobileClientFollowerChangedXP", Version=39869590)]
	public class MobileClientFollowerChangedXP
	{
		[DataMember(Name="follower")]
		[FlexJamMember(Name="follower", Type=FlexJamType.Struct)]
		public JamGarrisonFollower Follower
		{
			get;
			set;
		}

		[DataMember(Name="oldFollower")]
		[FlexJamMember(Name="oldFollower", Type=FlexJamType.Struct)]
		public JamGarrisonFollower OldFollower
		{
			get;
			set;
		}

		[DataMember(Name="source")]
		[FlexJamMember(Name="source", Type=FlexJamType.Int32)]
		public int Source
		{
			get;
			set;
		}

		[DataMember(Name="xpChange")]
		[FlexJamMember(Name="xpChange", Type=FlexJamType.Int32)]
		public int XpChange
		{
			get;
			set;
		}

		public MobileClientFollowerChangedXP()
		{
		}
	}
}