using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[FlexJamMessage(Id = 4865, Name = "MobileClientUseFollowerArmamentResult", Version = 33577221u), DataContract]
	public class MobileClientUseFollowerArmamentResult
	{
		[FlexJamMember(Name = "result", Type = FlexJamType.Int32), DataMember(Name = "result")]
		public int Result
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

		[FlexJamMember(Name = "oldFollower", Type = FlexJamType.Struct), DataMember(Name = "oldFollower")]
		public JamGarrisonFollower OldFollower
		{
			get;
			set;
		}
	}
}
