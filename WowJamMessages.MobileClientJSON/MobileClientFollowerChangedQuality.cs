using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[FlexJamMessage(Id = 4863, Name = "MobileClientFollowerChangedQuality", Version = 33577221u), DataContract]
	public class MobileClientFollowerChangedQuality
	{
		[FlexJamMember(Name = "oldFollower", Type = FlexJamType.Struct), DataMember(Name = "oldFollower")]
		public JamGarrisonFollower OldFollower
		{
			get;
			set;
		}

		[FlexJamMember(Name = "follower", Type = FlexJamType.Struct), DataMember(Name = "follower")]
		public JamGarrisonFollower Follower
		{
			get;
			set;
		}
	}
}
