using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4884, Name="MobileClientPlayerLevelUp", Version=39869590)]
	public class MobileClientPlayerLevelUp
	{
		[DataMember(Name="newLevel")]
		[FlexJamMember(Name="newLevel", Type=FlexJamType.Int32)]
		public int NewLevel
		{
			get;
			set;
		}

		public MobileClientPlayerLevelUp()
		{
		}
	}
}