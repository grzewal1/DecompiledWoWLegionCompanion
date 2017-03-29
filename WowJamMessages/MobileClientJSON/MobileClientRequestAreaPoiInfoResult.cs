using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4887, Name="MobileClientRequestAreaPoiInfoResult", Version=39869590)]
	public class MobileClientRequestAreaPoiInfoResult
	{
		[DataMember(Name="poiData")]
		[FlexJamMember(ArrayDimensions=1, Name="poiData", Type=FlexJamType.Struct)]
		public JamMobileAreaPOI[] PoiData
		{
			get;
			set;
		}

		public MobileClientRequestAreaPoiInfoResult()
		{
		}
	}
}