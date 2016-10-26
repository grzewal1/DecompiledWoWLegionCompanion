using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[FlexJamMessage(Id = 4869, Name = "MobileClientChangeFollowerActiveResult", Version = 33577221u), DataContract]
	public class MobileClientChangeFollowerActiveResult
	{
		[FlexJamMember(Name = "result", Type = FlexJamType.Int32), DataMember(Name = "result")]
		public int Result
		{
			get;
			set;
		}

		[FlexJamMember(Name = "activationsRemaining", Type = FlexJamType.Int32), DataMember(Name = "activationsRemaining")]
		public int ActivationsRemaining
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
