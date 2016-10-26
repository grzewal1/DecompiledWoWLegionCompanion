using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[FlexJamMessage(Id = 4801, Name = "MobilePlayerFollowerEquipmentRequest", Version = 33577221u), DataContract]
	public class MobilePlayerFollowerEquipmentRequest
	{
		[FlexJamMember(Name = "garrFollowerTypeID", Type = FlexJamType.Int32), DataMember(Name = "garrFollowerTypeID")]
		public int GarrFollowerTypeID
		{
			get;
			set;
		}
	}
}
