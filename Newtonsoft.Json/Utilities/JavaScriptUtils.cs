using System;
using System.IO;

namespace Newtonsoft.Json.Utilities
{
	internal static class JavaScriptUtils
	{
		public static string ToEscapedJavaScriptString(string value)
		{
			return JavaScriptUtils.ToEscapedJavaScriptString(value, '\"', true);
		}

		public static string ToEscapedJavaScriptString(string value, char delimiter, bool appendDelimiters)
		{
			string str;
			int? length = StringUtils.GetLength(value);
			using (StringWriter stringWriter = StringUtils.CreateStringWriter((!length.HasValue ? 16 : length.Value)))
			{
				JavaScriptUtils.WriteEscapedJavaScriptString(stringWriter, value, delimiter, appendDelimiters);
				str = stringWriter.ToString();
			}
			return str;
		}

		public static void WriteEscapedJavaScriptString(TextWriter writer, string value, char delimiter, bool appendDelimiters)
		{
			string str;
			string str1;
			string str2;
			string charAsUnicode;
			if (appendDelimiters)
			{
				writer.Write(delimiter);
			}
			if (value != null)
			{
				int num = 0;
				int num1 = 0;
				char[] charArray = null;
				for (int i = 0; i < value.Length; i++)
				{
					char chr = value[i];
					switch (chr)
					{
						case '\b':
						{
							str = "\\b";
							break;
						}
						case '\t':
						{
							str = "\\t";
							break;
						}
						case '\n':
						{
							str = "\\n";
							break;
						}
						case '\f':
						{
							str = "\\f";
							break;
						}
						case '\r':
						{
							str = "\\r";
							break;
						}
						default:
						{
							if (chr == '\u2028')
							{
								str = "\\u2028";
								break;
							}
							else if (chr == '\u2029')
							{
								str = "\\u2029";
								break;
							}
							else if (chr == '\"')
							{
								if (delimiter != '\"')
								{
									str1 = null;
								}
								else
								{
									str1 = "\\\"";
								}
								str = str1;
								break;
							}
							else if (chr == '\'')
							{
								if (delimiter != '\'')
								{
									str2 = null;
								}
								else
								{
									str2 = "\\'";
								}
								str = str2;
								break;
							}
							else if (chr == '\\')
							{
								str = "\\\\";
								break;
							}
							else if (chr == '\u0085')
							{
								str = "\\u0085";
								break;
							}
							else
							{
								if (chr > '\u001F')
								{
									charAsUnicode = null;
								}
								else
								{
									charAsUnicode = StringUtils.ToCharAsUnicode(chr);
								}
								str = charAsUnicode;
								break;
							}
						}
					}
					if (str == null)
					{
						num1++;
					}
					else
					{
						if (charArray == null)
						{
							charArray = value.ToCharArray();
						}
						if (num1 > 0)
						{
							writer.Write(charArray, num, num1);
							num1 = 0;
						}
						writer.Write(str);
						num = i + 1;
					}
				}
				if (num1 > 0)
				{
					if (num != 0)
					{
						writer.Write(charArray, num, num1);
					}
					else
					{
						writer.Write(value);
					}
				}
			}
			if (appendDelimiters)
			{
				writer.Write(delimiter);
			}
		}
	}
}