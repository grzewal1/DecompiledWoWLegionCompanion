using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4803, Name="MobilePlayerUseFollowerArmament", Version=38820897)]
	public class MobilePlayerUseFollowerArmament
	{
		[DataMember(Name="garrFollowerID")]
		[FlexJamMember(Name="garrFollowerID", Type=FlexJamType.Int32)]
		public int GarrFollowerID
		{
			get;
			set;
		}

		[DataMember(Name="garrFollowerTypeID")]
		[FlexJamMember(Name="garrFollowerTypeID", Type=FlexJamType.Int32)]
		public int GarrFollowerTypeID
		{
			get;
			set;
		}

		[DataMember(Name="itemID")]
		[FlexJamMember(Name="itemID", Type=FlexJamType.Int32)]
		public int ItemID
		{
			get;
			set;
		}

		public MobilePlayerUseFollowerArmament()
		{
		}
	}
}