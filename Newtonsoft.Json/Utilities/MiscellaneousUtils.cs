using System;
using System.Globalization;

namespace Newtonsoft.Json.Utilities
{
	internal static class MiscellaneousUtils
	{
		public static int ByteArrayCompare(byte[] a1, byte[] a2)
		{
			int length = (int)a1.Length;
			int num = length.CompareTo((int)a2.Length);
			if (num != 0)
			{
				return num;
			}
			for (int i = 0; i < (int)a1.Length; i++)
			{
				int num1 = a1[i].CompareTo(a2[i]);
				if (num1 != 0)
				{
					return num1;
				}
			}
			return 0;
		}

		public static string BytesToHex(byte[] bytes)
		{
			return MiscellaneousUtils.BytesToHex(bytes, false);
		}

		public static string BytesToHex(byte[] bytes, bool removeDashes)
		{
			string str = BitConverter.ToString(bytes);
			if (removeDashes)
			{
				str = str.Replace("-", string.Empty);
			}
			return str;
		}

		public static ArgumentOutOfRangeException CreateArgumentOutOfRangeException(string paramName, object actualValue, string message)
		{
			string str = string.Concat(message, Environment.NewLine, "Actual value was {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { actualValue }));
			return new ArgumentOutOfRangeException(paramName, str);
		}

		public static string GetLocalName(string qualifiedName)
		{
			string str;
			string str1;
			MiscellaneousUtils.GetQualifiedNameParts(qualifiedName, out str, out str1);
			return str1;
		}

		public static string GetPrefix(string qualifiedName)
		{
			string str;
			string str1;
			MiscellaneousUtils.GetQualifiedNameParts(qualifiedName, out str, out str1);
			return str;
		}

		public static void GetQualifiedNameParts(string qualifiedName, out string prefix, out string localName)
		{
			int num = qualifiedName.IndexOf(':');
			if (num == -1 || num == 0 || qualifiedName.Length - 1 == num)
			{
				prefix = null;
				localName = qualifiedName;
			}
			else
			{
				prefix = qualifiedName.Substring(0, num);
				localName = qualifiedName.Substring(num + 1);
			}
		}

		public static byte[] HexToBytes(string hex)
		{
			string str = hex.Replace("-", string.Empty);
			byte[] numArray = new byte[str.Length / 2];
			int num = 4;
			int num1 = 0;
			string str1 = str;
			for (int i = 0; i < str1.Length; i++)
			{
				int num2 = (str1[i] - 48) % ' ';
				if (num2 > 9)
				{
					num2 -= 7;
				}
				ref byte numPointer = ref numArray[num1];
				numPointer = (byte)(numPointer | (byte)(num2 << (num & 31)));
				num ^= 4;
				if (num != 0)
				{
					num1++;
				}
			}
			return numArray;
		}

		public static string ToString(object value)
		{
			if (value == null)
			{
				return "{null}";
			}
			return (!(value is string) ? value.ToString() : string.Concat("\"", value.ToString(), "\""));
		}

		public static bool TryAction<T>(Creator<T> creator, out T output)
		{
			bool flag;
			ValidationUtils.ArgumentNotNull(creator, "creator");
			try
			{
				output = creator();
				flag = true;
			}
			catch
			{
				output = default(T);
				flag = false;
			}
			return flag;
		}

		public static bool ValueEquals(object objA, object objB)
		{
			if (objA == null && objB == null)
			{
				return true;
			}
			if (objA != null && objB == null)
			{
				return false;
			}
			if (objA == null && objB != null)
			{
				return false;
			}
			if (objA.GetType() == objB.GetType())
			{
				return objA.Equals(objB);
			}
			if (ConvertUtils.IsInteger(objA) && ConvertUtils.IsInteger(objB))
			{
				decimal num = Convert.ToDecimal(objA, CultureInfo.CurrentCulture);
				return num.Equals(Convert.ToDecimal(objB, CultureInfo.CurrentCulture));
			}
			if (!(objA is double) && !(objA is float) && !(objA is decimal) || !(objB is double) && !(objB is float) && !(objB is decimal))
			{
				return false;
			}
			return MathUtils.ApproxEquals(Convert.ToDouble(objA, CultureInfo.CurrentCulture), Convert.ToDouble(objB, CultureInfo.CurrentCulture));
		}
	}
}