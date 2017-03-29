using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamCharacterShipment", Version=28333852)]
	public class JamCharacterShipment
	{
		[DataMember(Name="assignedFollowerDBID")]
		[FlexJamMember(Name="assignedFollowerDBID", Type=FlexJamType.UInt64)]
		public ulong AssignedFollowerDBID
		{
			get;
			set;
		}

		[DataMember(Name="buildingType")]
		[FlexJamMember(Name="buildingType", Type=FlexJamType.Int32)]
		public int BuildingType
		{
			get;
			set;
		}

		[DataMember(Name="creationTime")]
		[FlexJamMember(Name="creationTime", Type=FlexJamType.Int32)]
		public int CreationTime
		{
			get;
			set;
		}

		[DataMember(Name="shipmentDuration")]
		[FlexJamMember(Name="shipmentDuration", Type=FlexJamType.Int32)]
		public int ShipmentDuration
		{
			get;
			set;
		}

		[DataMember(Name="shipmentID")]
		[FlexJamMember(Name="shipmentID", Type=FlexJamType.UInt64)]
		public ulong ShipmentID
		{
			get;
			set;
		}

		[DataMember(Name="shipmentRecID")]
		[FlexJamMember(Name="shipmentRecID", Type=FlexJamType.Int32)]
		public int ShipmentRecID
		{
			get;
			set;
		}

		public JamCharacterShipment()
		{
		}
	}
}