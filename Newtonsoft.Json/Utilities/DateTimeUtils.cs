using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Newtonsoft.Json.Utilities
{
	internal static class DateTimeUtils
	{
		public static string GetLocalOffset(this DateTime d)
		{
			TimeSpan utcOffset = TimeZoneInfo.Local.GetUtcOffset(d);
			string str = utcOffset.Hours.ToString("+00;-00", CultureInfo.InvariantCulture);
			int minutes = utcOffset.Minutes;
			return string.Concat(str, ":", minutes.ToString("00;00", CultureInfo.InvariantCulture));
		}

		public static XmlDateTimeSerializationMode ToSerializationMode(DateTimeKind kind)
		{
			switch (kind)
			{
				case DateTimeKind.Unspecified:
				{
					return XmlDateTimeSerializationMode.Unspecified;
				}
				case DateTimeKind.Utc:
				{
					return XmlDateTimeSerializationMode.Utc;
				}
				case DateTimeKind.Local:
				{
					return XmlDateTimeSerializationMode.Local;
				}
			}
			throw MiscellaneousUtils.CreateArgumentOutOfRangeException("kind", kind, "Unexpected DateTimeKind value.");
		}
	}
}