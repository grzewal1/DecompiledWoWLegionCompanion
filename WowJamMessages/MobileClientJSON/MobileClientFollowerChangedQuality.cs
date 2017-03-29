using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using WowJamMessages;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4873, Name="MobileClientFollowerChangedQuality", Version=39869590)]
	public class MobileClientFollowerChangedQuality
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

		public MobileClientFollowerChangedQuality()
		{
		}
	}
}