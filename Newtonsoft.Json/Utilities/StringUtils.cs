using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Newtonsoft.Json.Utilities
{
	internal static class StringUtils
	{
		public const string CarriageReturnLineFeed = "\r\n";

		public const string Empty = "";

		public const char CarriageReturn = '\r';

		public const char LineFeed = '\n';

		public const char Tab = '\t';

		private static void ActionTextReaderLine(TextReader textReader, TextWriter textWriter, StringUtils.ActionLine lineAction)
		{
			bool flag = true;
			while (true)
			{
				string str = textReader.ReadLine();
				string str1 = str;
				if (str == null)
				{
					break;
				}
				if (flag)
				{
					flag = false;
				}
				else
				{
					textWriter.WriteLine();
				}
				lineAction(textWriter, str1);
			}
		}

		public static bool ContainsWhiteSpace(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			for (int i = 0; i < s.Length; i++)
			{
				if (char.IsWhiteSpace(s[i]))
				{
					return true;
				}
			}
			return false;
		}

		public static StringWriter CreateStringWriter(int capacity)
		{
			return new StringWriter(new StringBuilder(capacity), CultureInfo.InvariantCulture);
		}

		public static string EnsureEndsWith(string target, string value)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (target.Length >= value.Length)
			{
				if (string.Compare(target, target.Length - value.Length, value, 0, value.Length, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return target;
				}
				string str = target.TrimEnd(null);
				if (string.Compare(str, str.Length - value.Length, value, 0, value.Length, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return target;
				}
			}
			return string.Concat(target, value);
		}

		public static TSource ForgivingCaseSensitiveFind<TSource>(this IEnumerable<TSource> source, Func<TSource, string> valueSelector, string testValue)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (valueSelector == null)
			{
				throw new ArgumentNullException("valueSelector");
			}
			TSource[] array = (
				from  in source
				where string.Compare(valueSelector(s), testValue, StringComparison.OrdinalIgnoreCase) == 0
				select ).ToArray<TSource>();
			int length = (int)array.Length;
			if (length > 1)
			{
				IEnumerable<TSource> tSources = 
					from  in source
					where string.Compare(valueSelector(s), testValue, StringComparison.Ordinal) == 0
					select ;
				return tSources.SingleOrDefault<TSource>();
			}
			return (length != 1 ? default(TSource) : array[0]);
		}

		public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
		{
			ValidationUtils.ArgumentNotNull(format, "format");
			return string.Format(provider, format, args);
		}

		public static int? GetLength(string value)
		{
			if (value == null)
			{
				return null;
			}
			return new int?(value.Length);
		}

		public static void IfNotNullOrEmpty(string value, Action<string> action)
		{
			StringUtils.IfNotNullOrEmpty(value, action, null);
		}

		private static void IfNotNullOrEmpty(string value, Action<string> trueAction, Action<string> falseAction)
		{
			if (!string.IsNullOrEmpty(value))
			{
				if (trueAction != null)
				{
					trueAction(value);
				}
			}
			else if (falseAction != null)
			{
				falseAction(value);
			}
		}

		public static string Indent(string s, int indentation)
		{
			return StringUtils.Indent(s, indentation, ' ');
		}

		public static string Indent(string s, int indentation, char indentChar)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (indentation <= 0)
			{
				throw new ArgumentException("Must be greater than zero.", "indentation");
			}
			StringReader stringReader = new StringReader(s);
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			StringUtils.ActionTextReaderLine(stringReader, stringWriter, (TextWriter tw, string line) => {
				tw.Write(new string(indentChar, indentation));
				tw.Write(line);
			});
			return stringWriter.ToString();
		}

		public static bool IsNullOrEmptyOrWhiteSpace(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return true;
			}
			if (StringUtils.IsWhiteSpace(s))
			{
				return true;
			}
			return false;
		}

		public static bool IsWhiteSpace(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (s.Length == 0)
			{
				return false;
			}
			for (int i = 0; i < s.Length; i++)
			{
				if (!char.IsWhiteSpace(s[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static string NullEmptyString(string s)
		{
			string str;
			if (!string.IsNullOrEmpty(s))
			{
				str = s;
			}
			else
			{
				str = null;
			}
			return str;
		}

		public static string NumberLines(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			StringReader stringReader = new StringReader(s);
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			int num = 1;
			StringUtils.ActionTextReaderLine(stringReader, stringWriter, (TextWriter tw, string line) => {
				tw.Write(num.ToString(CultureInfo.InvariantCulture).PadLeft(4));
				tw.Write(". ");
				tw.Write(line);
				num++;
			});
			return stringWriter.ToString();
		}

		public static string ReplaceNewLines(string s, string replacement)
		{
			StringReader stringReader = new StringReader(s);
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			while (true)
			{
				string str = stringReader.ReadLine();
				string str1 = str;
				if (str == null)
				{
					break;
				}
				if (!flag)
				{
					stringBuilder.Append(replacement);
				}
				else
				{
					flag = false;
				}
				stringBuilder.Append(str1);
			}
			return stringBuilder.ToString();
		}

		public static string ToCamelCase(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return s;
			}
			if (!char.IsUpper(s[0]))
			{
				return s;
			}
			char lower = char.ToLower(s[0], CultureInfo.InvariantCulture);
			string str = lower.ToString(CultureInfo.InvariantCulture);
			if (s.Length > 1)
			{
				str = string.Concat(str, s.Substring(1));
			}
			return str;
		}

		public static string ToCharAsUnicode(char c)
		{
			char hex = MathUtils.IntToHex(c >> '\f' & 15);
			char chr = MathUtils.IntToHex(c >> '\b' & 15);
			char hex1 = MathUtils.IntToHex(c >> '\u0004' & 15);
			char chr1 = MathUtils.IntToHex(c & '\u000F');
			return new string(new char[] { '\\', 'u', hex, chr, hex1, chr1 });
		}

		public static string Truncate(string s, int maximumLength)
		{
			return StringUtils.Truncate(s, maximumLength, "...");
		}

		public static string Truncate(string s, int maximumLength, string suffix)
		{
			if (suffix == null)
			{
				throw new ArgumentNullException("suffix");
			}
			if (maximumLength <= 0)
			{
				throw new ArgumentException("Maximum length must be greater than zero.", "maximumLength");
			}
			int num = maximumLength - suffix.Length;
			if (num <= 0)
			{
				throw new ArgumentException("Length of suffix string is greater or equal to maximumLength");
			}
			if (s == null || s.Length <= maximumLength)
			{
				return s;
			}
			string str = s.Substring(0, num);
			return string.Concat(str.Trim(), suffix);
		}

		public static void WriteCharAsUnicode(TextWriter writer, char c)
		{
			ValidationUtils.ArgumentNotNull(writer, "writer");
			char hex = MathUtils.IntToHex(c >> '\f' & 15);
			char chr = MathUtils.IntToHex(c >> '\b' & 15);
			char hex1 = MathUtils.IntToHex(c >> '\u0004' & 15);
			char chr1 = MathUtils.IntToHex(c & '\u000F');
			writer.Write('\\');
			writer.Write('u');
			writer.Write(hex);
			writer.Write(chr);
			writer.Write(hex1);
			writer.Write(chr1);
		}

		private delegate void ActionLine(TextWriter textWriter, string line);
	}
}