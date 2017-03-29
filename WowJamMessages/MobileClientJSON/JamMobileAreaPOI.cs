using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamStruct(Name="JamMobileAreaPOI", Version=39869590)]
	public class JamMobileAreaPOI
	{
		[DataMember(Name="areaPoiID")]
		[FlexJamMember(Name="areaPoiID", Type=FlexJamType.Int32)]
		public int AreaPoiID
		{
			get;
			set;
		}

		[DataMember(Name="description")]
		[FlexJamMember(Name="description", Type=FlexJamType.String)]
		public string Description
		{
			get;
			set;
		}

		[DataMember(Name="name")]
		[FlexJamMember(Name="name", Type=FlexJamType.String)]
		public string Name
		{
			get;
			set;
		}

		[DataMember(Name="timeRemaining")]
		[FlexJamMember(Name="timeRemaining", Type=FlexJamType.Int32)]
		public int TimeRemaining
		{
			get;
			set;
		}

		[DataMember(Name="x")]
		[FlexJamMember(Name="x", Type=FlexJamType.Float)]
		public float X
		{
			get;
			set;
		}

		[DataMember(Name="y")]
		[FlexJamMember(Name="y", Type=FlexJamType.Float)]
		public float Y
		{
			get;
			set;
		}

		public JamMobileAreaPOI()
		{
		}
	}
}