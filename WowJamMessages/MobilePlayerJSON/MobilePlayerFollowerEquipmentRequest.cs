using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4800, Name="MobilePlayerFollowerEquipmentRequest", Version=38820897)]
	public class MobilePlayerFollowerEquipmentRequest
	{
		[DataMember(Name="garrFollowerTypeID")]
		[FlexJamMember(Name="garrFollowerTypeID", Type=FlexJamType.Int32)]
		public int GarrFollowerTypeID
		{
			get;
			set;
		}

		public MobilePlayerFollowerEquipmentRequest()
		{
		}
	}
}