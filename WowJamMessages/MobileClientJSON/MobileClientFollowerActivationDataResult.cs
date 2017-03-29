using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4879, Name="MobileClientFollowerActivationDataResult", Version=39869590)]
	public class MobileClientFollowerActivationDataResult
	{
		[DataMember(Name="activationsRemaining")]
		[FlexJamMember(Name="activationsRemaining", Type=FlexJamType.Int32)]
		public int ActivationsRemaining
		{
			get;
			set;
		}

		[DataMember(Name="goldCost")]
		[FlexJamMember(Name="goldCost", Type=FlexJamType.Int32)]
		public int GoldCost
		{
			get;
			set;
		}

		public MobileClientFollowerActivationDataResult()
		{
		}
	}
}