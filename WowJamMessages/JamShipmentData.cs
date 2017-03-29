using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamShipmentData", Version=28333852)]
	public class JamShipmentData
	{
		[DataMember(Name="pendingShipment")]
		[FlexJamMember(ArrayDimensions=1, Name="pendingShipment", Type=FlexJamType.Int32)]
		public int[] PendingShipment
		{
			get;
			set;
		}

		[DataMember(Name="resetPending")]
		[FlexJamMember(Name="resetPending", Type=FlexJamType.Bool)]
		public bool ResetPending
		{
			get;
			set;
		}

		[DataMember(Name="shipment")]
		[FlexJamMember(ArrayDimensions=1, Name="shipment", Type=FlexJamType.Struct)]
		public JamCharacterShipment[] Shipment
		{
			get;
			set;
		}

		public JamShipmentData()
		{
			this.ResetPending = false;
		}
	}
}