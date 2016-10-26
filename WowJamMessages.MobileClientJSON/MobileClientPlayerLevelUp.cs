using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[FlexJamMessage(Id = 4873, Name = "MobileClientPlayerLevelUp", Version = 33577221u), DataContract]
	public class MobileClientPlayerLevelUp
	{
		[FlexJamMember(Name = "newLevel", Type = FlexJamType.Int32), DataMember(Name = "newLevel")]
		public int NewLevel
		{
			get;
			set;
		}
	}
}
