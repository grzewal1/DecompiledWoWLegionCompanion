using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4889, Name="MobileClientRequestMaxFollowersResult", Version=39869590)]
	public class MobileClientRequestMaxFollowersResult
	{
		[DataMember(Name="maxFollowers")]
		[FlexJamMember(Name="maxFollowers", Type=FlexJamType.Int32)]
		public int MaxFollowers
		{
			get;
			set;
		}

		public MobileClientRequestMaxFollowersResult()
		{
		}
	}
}