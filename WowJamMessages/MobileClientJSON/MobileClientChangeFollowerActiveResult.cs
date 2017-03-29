using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using WowJamMessages;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4880, Name="MobileClientChangeFollowerActiveResult", Version=39869590)]
	public class MobileClientChangeFollowerActiveResult
	{
		[DataMember(Name="activationsRemaining")]
		[FlexJamMember(Name="activationsRemaining", Type=FlexJamType.Int32)]
		public int ActivationsRemaining
		{
			get;
			set;
		}

		[DataMember(Name="follower")]
		[FlexJamMember(Name="follower", Type=FlexJamType.Struct)]
		public JamGarrisonFollower Follower
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

		public MobileClientChangeFollowerActiveResult()
		{
		}
	}
}