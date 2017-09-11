using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace GarbageFreeStringBuilder
{
	public static class StringBuilderExtensions
	{
		private readonly static char[] ms_digits;

		private readonly static uint ms_default_decimal_places;

		private readonly static char ms_default_pad_char;

		static StringBuilderExtensions()
		{
			StringBuilderExtensions.ms_digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
			StringBuilderExtensions.ms_default_decimal_places = 5;
			StringBuilderExtensions.ms_default_pad_char = '0';
		}

		public static StringBuilder Concat(this StringBuilder string_builder, uint uint_val, uint pad_amount, char pad_char, uint base_val)
		{
			unsafe
			{
				uint num = 0;
				uint uintVal = uint_val;
				do
				{
					uintVal /= base_val;
					num++;
				}
				while (uintVal > 0);
				string_builder.Append(pad_char, (int)Math.Max(pad_amount, num));
				int length = string_builder.Length;
				while (num > 0)
				{
					length--;
					string_builder[length] = StringBuilderExtensions.ms_digits[uint_val % base_val];
					uint_val /= base_val;
					num--;
				}
				return string_builder;
			}
		}

		public static StringBuilder Concat(this StringBuilder string_builder, uint uint_val)
		{
			string_builder.Concat(uint_val, 0, StringBuilderExtensions.ms_default_pad_char, 10);
			return string_builder;
		}

		public static StringBuilder Concat(this StringBuilder string_builder, uint uint_val, uint pad_amount)
		{
			string_builder.Concat(uint_val, pad_amount, StringBuilderExtensions.ms_default_pad_char, 10);
			return string_builder;
		}

		public static StringBuilder Concat(this StringBuilder string_builder, uint uint_val, uint pad_amount, char pad_char)
		{
			string_builder.Concat(uint_val, pad_amount, pad_char, 10);
			return string_builder;
		}

		public static StringBuilder Concat(this StringBuilder string_builder, int int_val, uint pad_amount, char pad_char, uint base_val)
		{
			if (int_val >= 0)
			{
				string_builder.Concat((uint)int_val, pad_amount, pad_char, base_val);
			}
			else
			{
				string_builder.Append('-');
				uint intVal = (uint)(-1 - int_val + 1);
				string_builder.Concat(intVal, pad_amount, pad_char, base_val);
			}
			return string_builder;
		}

		public static StringBuilder Concat(this StringBuilder string_builder, int int_val)
		{
			string_builder.Concat(int_val, 0, StringBuilderExtensions.ms_default_pad_char, 10);
			return string_builder;
		}

		public static StringBuilder Concat(this StringBuilder string_builder, int int_val, uint pad_amount)
		{
			string_builder.Concat(int_val, pad_amount, StringBuilderExtensions.ms_default_pad_char, 10);
			return string_builder;
		}

		public static StringBuilder Concat(this StringBuilder string_builder, int int_val, uint pad_amount, char pad_char)
		{
			string_builder.Concat(int_val, pad_amount, pad_char, 10);
			return string_builder;
		}

		public static StringBuilder Concat(this StringBuilder string_builder, float float_val, uint decimal_places, uint pad_amount, char pad_char)
		{
			int num;
			if (decimal_places != 0)
			{
				int floatVal = (int)float_val;
				string_builder.Concat(floatVal, pad_amount, pad_char, 10);
				string_builder.Append('.');
				float single = Math.Abs(float_val - (float)floatVal);
				do
				{
					single *= 10f;
					decimal_places--;
				}
				while (decimal_places > 0);
				single += 0.5f;
				string_builder.Concat((uint)single, 0, '0', 10);
			}
			else
			{
				num = (float_val < 0f ? (int)(float_val - 0.5f) : (int)(float_val + 0.5f));
				string_builder.Concat(num, pad_amount, pad_char, 10);
			}
			return string_builder;
		}

		public static StringBuilder Concat(this StringBuilder string_builder, float float_val)
		{
			string_builder.Concat(float_val, StringBuilderExtensions.ms_default_decimal_places, 0, StringBuilderExtensions.ms_default_pad_char);
			return string_builder;
		}

		public static StringBuilder Concat(this StringBuilder string_builder, float float_val, uint decimal_places)
		{
			string_builder.Concat(float_val, decimal_places, 0, StringBuilderExtensions.ms_default_pad_char);
			return string_builder;
		}

		public static StringBuilder Concat(this StringBuilder string_builder, float float_val, uint decimal_places, uint pad_amount)
		{
			string_builder.Concat(float_val, decimal_places, pad_amount, StringBuilderExtensions.ms_default_pad_char);
			return string_builder;
		}

		public static StringBuilder ConcatFormat<A>(this StringBuilder string_builder, string format_string, A arg1)
		where A : IConvertible
		{
			return string_builder.ConcatFormat<A, int, int, int>(format_string, arg1, 0, 0, 0);
		}

		public static StringBuilder ConcatFormat<A, B>(this StringBuilder string_builder, string format_string, A arg1, B arg2)
		where A : IConvertible
		where B : IConvertible
		{
			return string_builder.ConcatFormat<A, B, int, int>(format_string, arg1, arg2, 0, 0);
		}

		public static StringBuilder ConcatFormat<A, B, C>(this StringBuilder string_builder, string format_string, A arg1, B arg2, C arg3)
		where A : IConvertible
		where B : IConvertible
		where C : IConvertible
		{
			return string_builder.ConcatFormat<A, B, C, int>(format_string, arg1, arg2, arg3, 0);
		}

		public static StringBuilder ConcatFormat<A, B, C, D>(this StringBuilder string_builder, string format_string, A arg1, B arg2, C arg3, D arg4)
		where A : IConvertible
		where B : IConvertible
		where C : IConvertible
		where D : IConvertible
		{
			int num = 0;
			for (int i = 0; i < format_string.Length; i++)
			{
				if (format_string[i] == '{')
				{
					if (num < i)
					{
						string_builder.Append(format_string, num, i - num);
					}
					uint num1 = 10;
					uint formatString = 0;
					uint num2 = 5;
					i++;
					char chr = format_string[i];
					if (chr != '{')
					{
						i++;
						if (format_string[i] == ':')
						{
							i++;
							while (format_string[i] == '0')
							{
								i++;
								formatString++;
							}
							if (format_string[i] == 'X')
							{
								i++;
								num1 = 16;
								if (format_string[i] >= '0' && format_string[i] <= '9')
								{
									formatString = format_string[i] - 48;
									i++;
								}
							}
							else if (format_string[i] == '.')
							{
								i++;
								num2 = 0;
								while (format_string[i] == '0')
								{
									i++;
									num2++;
								}
							}
						}
						while (format_string[i] != '}')
						{
							i++;
						}
						switch (chr)
						{
							case '0':
							{
								string_builder.ConcatFormatValue<A>(arg1, formatString, num1, num2);
								break;
							}
							case '1':
							{
								string_builder.ConcatFormatValue<B>(arg2, formatString, num1, num2);
								break;
							}
							case '2':
							{
								string_builder.ConcatFormatValue<C>(arg3, formatString, num1, num2);
								break;
							}
							case '3':
							{
								string_builder.ConcatFormatValue<D>(arg4, formatString, num1, num2);
								break;
							}
						}
					}
					else
					{
						string_builder.Append('{');
						i++;
					}
					num = i + 1;
				}
			}
			if (num < format_string.Length)
			{
				string_builder.Append(format_string, num, format_string.Length - num);
			}
			return string_builder;
		}

		private static void ConcatFormatValue<T>(this StringBuilder string_builder, T arg, uint padding, uint base_value, uint decimal_places)
		where T : IConvertible
		{
			TypeCode typeCode = arg.GetTypeCode();
			switch (typeCode)
			{
				case TypeCode.Int32:
				{
					string_builder.Concat(arg.ToInt32(NumberFormatInfo.CurrentInfo), padding, '0', base_value);
					break;
				}
				case TypeCode.UInt32:
				{
					string_builder.Concat(arg.ToUInt32(NumberFormatInfo.CurrentInfo), padding, '0', base_value);
					break;
				}
				case TypeCode.Single:
				{
					string_builder.Concat(arg.ToSingle(NumberFormatInfo.CurrentInfo), decimal_places, padding, '0');
					break;
				}
				default:
				{
					if (typeCode == TypeCode.String)
					{
						string_builder.Append(Convert.ToString(arg));
						break;
					}
					else
					{
						break;
					}
				}
			}
		}
	}
}