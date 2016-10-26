using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[FlexJamMessage(Id = 4864, Name = "MobileClientFollowerArmamentsResult", Version = 33577221u), DataContract]
	public class MobileClientFollowerArmamentsResult
	{
		[FlexJamMember(ArrayDimensions = 1, Name = "armament", Type = FlexJamType.Struct), DataMember(Name = "armament")]
		public MobileFollowerArmament[] Armament
		{
			get;
			set;
		}
	}
}
