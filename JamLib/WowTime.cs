using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace JamLib
{
	[DataContract]
	[FlexJamStruct(Name="WowTime")]
	public struct WowTime
	{
		[DataMember(Name="flags")]
		[FlexJamMember(Name="flags", Type=FlexJamType.Int32)]
		public int Flags
		{
			get;
			set;
		}

		[DataMember(Name="holidayOffset")]
		[FlexJamMember(Name="holidayOffset", Type=FlexJamType.Int32)]
		public int HolidayOffset
		{
			get;
			set;
		}

		[DataMember(Name="hour")]
		[FlexJamMember(Name="hour", Type=FlexJamType.Int32)]
		public int Hour
		{
			get;
			set;
		}

		[DataMember(Name="minute")]
		[FlexJamMember(Name="minute", Type=FlexJamType.Int32)]
		public int Minute
		{
			get;
			set;
		}

		[DataMember(Name="month")]
		[FlexJamMember(Name="month", Type=FlexJamType.Int32)]
		public int Month
		{
			get;
			set;
		}

		[DataMember(Name="monthDay")]
		[FlexJamMember(Name="monthDay", Type=FlexJamType.Int32)]
		public int MonthDay
		{
			get;
			set;
		}

		[DataMember(Name="weekday")]
		[FlexJamMember(Name="weekday", Type=FlexJamType.Int32)]
		public int WeekDay
		{
			get;
			set;
		}

		[DataMember(Name="year")]
		[FlexJamMember(Name="year", Type=FlexJamType.Int32)]
		public int Year
		{
			get;
			set;
		}
	}
}