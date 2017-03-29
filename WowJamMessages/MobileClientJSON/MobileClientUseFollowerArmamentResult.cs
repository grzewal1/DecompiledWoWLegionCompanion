using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using WowJamMessages;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4876, Name="MobileClientUseFollowerArmamentResult", Version=39869590)]
	public class MobileClientUseFollowerArmamentResult
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

		[DataMember(Name="result")]
		[FlexJamMember(Name="result", Type=FlexJamType.Int32)]
		public int Result
		{
			get;
			set;
		}

		public MobileClientUseFollowerArmamentResult()
		{
		}
	}
}