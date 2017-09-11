using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace bgs
{
	public class TimeUtils
	{
		public const int SEC_PER_MINUTE = 60;

		public const int SEC_PER_HOUR = 3600;

		public const int SEC_PER_DAY = 86400;

		public const int SEC_PER_WEEK = 604800;

		public const int MS_PER_SEC = 1000;

		public const int MS_PER_MINUTE = 60000;

		public const int MS_PER_HOUR = 3600000;

		public const string DEFAULT_TIME_UNITS_STR = "sec";

		public readonly static DateTime EPOCH_TIME;

		public readonly static TimeUtils.ElapsedStringSet SPLASHSCREEN_DATETIME_STRINGSET;

		static TimeUtils()
		{
			TimeUtils.EPOCH_TIME = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			TimeUtils.ElapsedStringSet elapsedStringSet = new TimeUtils.ElapsedStringSet()
			{
				m_seconds = "GLOBAL_DATETIME_SPLASHSCREEN_SECONDS",
				m_minutes = "GLOBAL_DATETIME_SPLASHSCREEN_MINUTES",
				m_hours = "GLOBAL_DATETIME_SPLASHSCREEN_HOURS",
				m_yesterday = "GLOBAL_DATETIME_SPLASHSCREEN_DAY",
				m_days = "GLOBAL_DATETIME_SPLASHSCREEN_DAYS",
				m_weeks = "GLOBAL_DATETIME_SPLASHSCREEN_WEEKS",
				m_monthAgo = "GLOBAL_DATETIME_SPLASHSCREEN_MONTH"
			};
			TimeUtils.SPLASHSCREEN_DATETIME_STRINGSET = elapsedStringSet;
		}

		public TimeUtils()
		{
		}

		private static void AppendDevTimeUnitsString(string formatString, int msPerUnit, StringBuilder builder, ref long ms, ref int unitCount)
		{
			long num = ms / (long)msPerUnit;
			if (num > (long)0)
			{
				if (unitCount > 0)
				{
					builder.Append(' ');
				}
				builder.AppendFormat(formatString, num);
				unitCount++;
			}
			ms = ms - num * (long)msPerUnit;
		}

		public static long BinaryStamp()
		{
			return DateTime.UtcNow.ToBinary();
		}

		public static DateTime ConvertEpochMicrosecToDateTime(ulong microsec)
		{
			DateTime ePOCHTIME = TimeUtils.EPOCH_TIME;
			return ePOCHTIME.AddMilliseconds((double)((float)microsec) / 1000);
		}

		public static float ForceDevSecFromElapsedTimeString(string timeStr)
		{
			float single;
			TimeUtils.TryParseDevSecFromElapsedTimeString(timeStr, out single);
			return single;
		}

		public static string GetDevElapsedTimeString(TimeSpan span)
		{
			return TimeUtils.GetDevElapsedTimeString((long)span.TotalMilliseconds);
		}

		public static string GetDevElapsedTimeString(long ms)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			if (ms >= (long)3600000)
			{
				TimeUtils.AppendDevTimeUnitsString("{0}h", 3600000, stringBuilder, ref ms, ref num);
			}
			if (ms >= (long)60000)
			{
				TimeUtils.AppendDevTimeUnitsString("{0}m", 60000, stringBuilder, ref ms, ref num);
			}
			if (ms >= (long)1000)
			{
				TimeUtils.AppendDevTimeUnitsString("{0}s", 1000, stringBuilder, ref ms, ref num);
			}
			if (num <= 1)
			{
				TimeUtils.AppendDevTimeUnitsString("{0}ms", 1, stringBuilder, ref ms, ref num);
			}
			return stringBuilder.ToString();
		}

		public static void GetElapsedTime(int seconds, out TimeUtils.ElapsedTimeType timeType, out int time)
		{
			time = 0;
			if (seconds < 60)
			{
				timeType = TimeUtils.ElapsedTimeType.SECONDS;
				time = seconds;
				return;
			}
			if (seconds < 3600)
			{
				timeType = TimeUtils.ElapsedTimeType.MINUTES;
				time = seconds / 60;
				return;
			}
			int num = seconds / 86400;
			if (num == 0)
			{
				timeType = TimeUtils.ElapsedTimeType.HOURS;
				time = seconds / 3600;
				return;
			}
			if (num == 1)
			{
				timeType = TimeUtils.ElapsedTimeType.YESTERDAY;
				return;
			}
			int num1 = seconds / 604800;
			if (num1 == 0)
			{
				timeType = TimeUtils.ElapsedTimeType.DAYS;
				time = num;
				return;
			}
			if (num1 >= 4)
			{
				timeType = TimeUtils.ElapsedTimeType.MONTH_AGO;
				return;
			}
			timeType = TimeUtils.ElapsedTimeType.WEEKS;
			time = num1;
		}

		public static TimeSpan GetElapsedTimeSinceEpoch(DateTime? endDateTime = null)
		{
			return (!endDateTime.HasValue ? DateTime.UtcNow : endDateTime.Value) - TimeUtils.EPOCH_TIME;
		}

		private static string ParseTimeUnitsStr(string unitsStr)
		{
			int num;
			if (unitsStr == null)
			{
				return "sec";
			}
			unitsStr = unitsStr.ToLowerInvariant();
			string str = unitsStr;
			if (str != null)
			{
				if (TimeUtils.<>f__switch$map10 == null)
				{
					Dictionary<string, int> strs = new Dictionary<string, int>(13)
					{
						{ "s", 0 },
						{ "sec", 0 },
						{ "secs", 0 },
						{ "second", 0 },
						{ "seconds", 0 },
						{ "m", 1 },
						{ "min", 1 },
						{ "mins", 1 },
						{ "minute", 1 },
						{ "minutes", 1 },
						{ "h", 2 },
						{ "hour", 2 },
						{ "hours", 2 }
					};
					TimeUtils.<>f__switch$map10 = strs;
				}
				if (TimeUtils.<>f__switch$map10.TryGetValue(str, out num))
				{
					switch (num)
					{
						case 0:
						{
							return "sec";
						}
						case 1:
						{
							return "min";
						}
						case 2:
						{
							return "hour";
						}
					}
				}
			}
			return "sec";
		}

		public static bool TryParseDevSecFromElapsedTimeString(string timeStr, out float sec)
		{
			sec = 0f;
			MatchCollection matchCollections = Regex.Matches(timeStr, "(?<number>(?:[0-9]+,)*[0-9]+)\\s*(?<units>[a-zA-Z]+)");
			if (matchCollections.Count == 0)
			{
				return false;
			}
			Match item = matchCollections[0];
			if (!item.Groups[0].Success)
			{
				return false;
			}
			Group group = item.Groups["number"];
			Group item1 = item.Groups["units"];
			if (!group.Success || !item1.Success)
			{
				return false;
			}
			string value = group.Value;
			string str = item1.Value;
			if (!float.TryParse(value, out sec))
			{
				return false;
			}
			str = TimeUtils.ParseTimeUnitsStr(str);
			if (str == "min")
			{
				sec *= 60f;
			}
			else if (str == "hour")
			{
				sec *= 3600f;
			}
			return true;
		}

		public class ElapsedStringSet
		{
			public string m_seconds;

			public string m_minutes;

			public string m_hours;

			public string m_yesterday;

			public string m_days;

			public string m_weeks;

			public string m_monthAgo;

			public ElapsedStringSet()
			{
			}
		}

		public enum ElapsedTimeType
		{
			SECONDS,
			MINUTES,
			HOURS,
			YESTERDAY,
			DAYS,
			WEEKS,
			MONTH_AGO
		}
	}
}