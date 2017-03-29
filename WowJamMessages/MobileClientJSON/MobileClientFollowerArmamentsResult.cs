using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4874, Name="MobileClientFollowerArmamentsResult", Version=39869590)]
	public class MobileClientFollowerArmamentsResult
	{
		[DataMember(Name="armament")]
		[FlexJamMember(ArrayDimensions=1, Name="armament", Type=FlexJamType.Struct)]
		public MobileFollowerArmament[] Armament
		{
			get;
			set;
		}

		public MobileClientFollowerArmamentsResult()
		{
		}
	}
}