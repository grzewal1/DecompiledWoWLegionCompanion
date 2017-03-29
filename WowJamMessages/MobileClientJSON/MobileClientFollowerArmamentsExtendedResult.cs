using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4875, Name="MobileClientFollowerArmamentsExtendedResult", Version=39869590)]
	public class MobileClientFollowerArmamentsExtendedResult
	{
		[DataMember(Name="armament")]
		[FlexJamMember(ArrayDimensions=1, Name="armament", Type=FlexJamType.Struct)]
		public MobileFollowerArmamentExt[] Armament
		{
			get;
			set;
		}

		public MobileClientFollowerArmamentsExtendedResult()
		{
		}
	}
}