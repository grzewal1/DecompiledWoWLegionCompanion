using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4872, Name="MobileClientFollowerEquipmentResult", Version=39869590)]
	public class MobileClientFollowerEquipmentResult
	{
		[DataMember(Name="equipment")]
		[FlexJamMember(ArrayDimensions=1, Name="equipment", Type=FlexJamType.Struct)]
		public MobileFollowerEquipment[] Equipment
		{
			get;
			set;
		}

		public MobileClientFollowerEquipmentResult()
		{
		}
	}
}