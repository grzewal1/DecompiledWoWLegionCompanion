using System;

namespace Newtonsoft.Json.Utilities
{
	internal class MathUtils
	{
		public MathUtils()
		{
		}

		public static bool ApproxEquals(double d1, double d2)
		{
			return Math.Abs(d1 - d2) < Math.Abs(d1) * 1E-06;
		}

		public static int GetDecimalPlaces(double value)
		{
			int i;
			int num = 10;
			double num1 = Math.Pow(0.1, (double)num);
			if (value == 0)
			{
				return 0;
			}
			for (i = 0; value - Math.Floor(value) > num1 && i < num; i++)
			{
				value *= 10;
			}
			return i;
		}

		public static int HexToInt(char h)
		{
			if (h >= '0' && h <= '9')
			{
				return h - 48;
			}
			if (h >= 'a' && h <= 'f')
			{
				return h - 97 + 10;
			}
			if (h < 'A' || h > 'F')
			{
				return -1;
			}
			return h - 65 + 10;
		}

		public static int IntLength(int i)
		{
			if (i < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (i == 0)
			{
				return 1;
			}
			return (int)Math.Floor(Math.Log10((double)i)) + 1;
		}

		public static char IntToHex(int n)
		{
			if (n <= 9)
			{
				return (char)(n + 48);
			}
			return (char)(n - 10 + 97);
		}

		public static int? Max(int? val1, int? val2)
		{
			if (!val1.HasValue)
			{
				return val2;
			}
			if (!val2.HasValue)
			{
				return val1;
			}
			return new int?(Math.Max(val1.Value, val2.Value));
		}

		public static double? Max(double? val1, double? val2)
		{
			if (!val1.HasValue)
			{
				return val2;
			}
			if (!val2.HasValue)
			{
				return val1;
			}
			return new double?(Math.Max(val1.Value, val2.Value));
		}

		public static int? Min(int? val1, int? val2)
		{
			if (!val1.HasValue)
			{
				return val2;
			}
			if (!val2.HasValue)
			{
				return val1;
			}
			return new int?(Math.Min(val1.Value, val2.Value));
		}

		public static double? Min(double? val1, double? val2)
		{
			if (!val1.HasValue)
			{
				return val2;
			}
			if (!val2.HasValue)
			{
				return val1;
			}
			return new double?(Math.Min(val1.Value, val2.Value));
		}
	}
}