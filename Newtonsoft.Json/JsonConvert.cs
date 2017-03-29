using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Newtonsoft.Json
{
	public static class JsonConvert
	{
		public readonly static string True;

		public readonly static string False;

		public readonly static string Null;

		public readonly static string Undefined;

		public readonly static string PositiveInfinity;

		public readonly static string NegativeInfinity;

		public readonly static string NaN;

		internal readonly static long InitialJavaScriptDateTicks;

		static JsonConvert()
		{
			JsonConvert.True = "true";
			JsonConvert.False = "false";
			JsonConvert.Null = "null";
			JsonConvert.Undefined = "undefined";
			JsonConvert.PositiveInfinity = "Infinity";
			JsonConvert.NegativeInfinity = "-Infinity";
			JsonConvert.NaN = "NaN";
			JsonConvert.InitialJavaScriptDateTicks = 621355968000000000L;
		}

		internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime, TimeSpan offset)
		{
			return JsonConvert.UniversialTicksToJavaScriptTicks(JsonConvert.ToUniversalTicks(dateTime, offset));
		}

		internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime)
		{
			return JsonConvert.ConvertDateTimeToJavaScriptTicks(dateTime, true);
		}

		internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime, bool convertToUtc)
		{
			return JsonConvert.UniversialTicksToJavaScriptTicks((!convertToUtc ? dateTime.Ticks : JsonConvert.ToUniversalTicks(dateTime)));
		}

		internal static DateTime ConvertJavaScriptTicksToDateTime(long javaScriptTicks)
		{
			DateTime dateTime = new DateTime(javaScriptTicks * (long)10000 + JsonConvert.InitialJavaScriptDateTicks, DateTimeKind.Utc);
			return dateTime;
		}

		public static T DeserializeAnonymousType<T>(string value, T anonymousTypeObject)
		{
			return JsonConvert.DeserializeObject<T>(value);
		}

		public static object DeserializeObject(string value)
		{
			return JsonConvert.DeserializeObject(value, null, (JsonSerializerSettings)null);
		}

		public static object DeserializeObject(string value, JsonSerializerSettings settings)
		{
			return JsonConvert.DeserializeObject(value, null, settings);
		}

		public static object DeserializeObject(string value, Type type)
		{
			return JsonConvert.DeserializeObject(value, type, (JsonSerializerSettings)null);
		}

		public static T DeserializeObject<T>(string value)
		{
			return JsonConvert.DeserializeObject<T>(value, (JsonSerializerSettings)null);
		}

		public static T DeserializeObject<T>(string value, params JsonConverter[] converters)
		{
			return (T)JsonConvert.DeserializeObject(value, typeof(T), converters);
		}

		public static T DeserializeObject<T>(string value, JsonSerializerSettings settings)
		{
			return (T)JsonConvert.DeserializeObject(value, typeof(T), settings);
		}

		public static object DeserializeObject(string value, Type type, params JsonConverter[] converters)
		{
			JsonSerializerSettings jsonSerializerSetting;
			if (converters == null || (int)converters.Length <= 0)
			{
				jsonSerializerSetting = null;
			}
			else
			{
				jsonSerializerSetting = new JsonSerializerSettings()
				{
					Converters = converters
				};
			}
			return JsonConvert.DeserializeObject(value, type, jsonSerializerSetting);
		}

		public static object DeserializeObject(string value, Type type, JsonSerializerSettings settings)
		{
			object obj;
			StringReader stringReader = new StringReader(value);
			JsonSerializer jsonSerializer = JsonSerializer.Create(settings);
			using (JsonReader jsonTextReader = new JsonTextReader(stringReader))
			{
				obj = jsonSerializer.Deserialize(jsonTextReader, type);
				if (jsonTextReader.Read() && jsonTextReader.TokenType != JsonToken.Comment)
				{
					throw new JsonSerializationException("Additional text found in JSON string after finishing deserializing object.");
				}
			}
			return obj;
		}

		public static XmlDocument DeserializeXmlNode(string value)
		{
			return JsonConvert.DeserializeXmlNode(value, null);
		}

		public static XmlDocument DeserializeXmlNode(string value, string deserializeRootElementName)
		{
			return JsonConvert.DeserializeXmlNode(value, deserializeRootElementName, false);
		}

		public static XmlDocument DeserializeXmlNode(string value, string deserializeRootElementName, bool writeArrayAttribute)
		{
			XmlNodeConverter xmlNodeConverter = new XmlNodeConverter()
			{
				DeserializeRootElementName = deserializeRootElementName,
				WriteArrayAttribute = writeArrayAttribute
			};
			XmlNodeConverter xmlNodeConverter1 = xmlNodeConverter;
			return (XmlDocument)JsonConvert.DeserializeObject(value, typeof(XmlDocument), new JsonConverter[] { xmlNodeConverter1 });
		}

		private static string EnsureDecimalPlace(double value, string text)
		{
			if (double.IsNaN(value) || double.IsInfinity(value) || text.IndexOf('.') != -1 || text.IndexOf('E') != -1)
			{
				return text;
			}
			return string.Concat(text, ".0");
		}

		private static string EnsureDecimalPlace(string text)
		{
			if (text.IndexOf('.') != -1)
			{
				return text;
			}
			return string.Concat(text, ".0");
		}

		private static TimeSpan GetUtcOffset(DateTime dateTime)
		{
			return TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);
		}

		internal static bool IsJsonPrimitive(object value)
		{
			if (value == null)
			{
				return true;
			}
			IConvertible convertible = value as IConvertible;
			if (convertible != null)
			{
				return JsonConvert.IsJsonPrimitiveTypeCode(convertible.GetTypeCode());
			}
			if (value is DateTimeOffset)
			{
				return true;
			}
			if (value is byte[])
			{
				return true;
			}
			if (value is Uri)
			{
				return true;
			}
			if (value is TimeSpan)
			{
				return true;
			}
			if (value is Guid)
			{
				return true;
			}
			return false;
		}

		internal static bool IsJsonPrimitiveType(Type type)
		{
			if (ReflectionUtils.IsNullableType(type))
			{
				type = Nullable.GetUnderlyingType(type);
			}
			if (type == typeof(DateTimeOffset))
			{
				return true;
			}
			if (type == typeof(byte[]))
			{
				return true;
			}
			if (type == typeof(Uri))
			{
				return true;
			}
			if (type == typeof(TimeSpan))
			{
				return true;
			}
			if (type == typeof(Guid))
			{
				return true;
			}
			return JsonConvert.IsJsonPrimitiveTypeCode(Type.GetTypeCode(type));
		}

		private static bool IsJsonPrimitiveTypeCode(TypeCode typeCode)
		{
			switch (typeCode)
			{
				case TypeCode.DBNull:
				case TypeCode.Boolean:
				case TypeCode.Char:
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
				case TypeCode.DateTime:
				case TypeCode.String:
				{
					return true;
				}
				case TypeCode.Object | TypeCode.DateTime:
				{
					return false;
				}
				default:
				{
					return false;
				}
			}
		}

		public static void PopulateObject(string value, object target)
		{
			JsonConvert.PopulateObject(value, target, null);
		}

		public static void PopulateObject(string value, object target, JsonSerializerSettings settings)
		{
			StringReader stringReader = new StringReader(value);
			JsonSerializer jsonSerializer = JsonSerializer.Create(settings);
			using (JsonReader jsonTextReader = new JsonTextReader(stringReader))
			{
				jsonSerializer.Populate(jsonTextReader, target);
				if (jsonTextReader.Read() && jsonTextReader.TokenType != JsonToken.Comment)
				{
					throw new JsonSerializationException("Additional text found in JSON string after finishing deserializing object.");
				}
			}
		}

		public static string SerializeObject(object value)
		{
			return JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.None, (JsonSerializerSettings)null);
		}

		public static string SerializeObject(object value, Newtonsoft.Json.Formatting formatting)
		{
			return JsonConvert.SerializeObject(value, formatting, (JsonSerializerSettings)null);
		}

		public static string SerializeObject(object value, params JsonConverter[] converters)
		{
			return JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.None, converters);
		}

		public static string SerializeObject(object value, Newtonsoft.Json.Formatting formatting, params JsonConverter[] converters)
		{
			JsonSerializerSettings jsonSerializerSetting;
			if (converters == null || (int)converters.Length <= 0)
			{
				jsonSerializerSetting = null;
			}
			else
			{
				jsonSerializerSetting = new JsonSerializerSettings()
				{
					Converters = converters
				};
			}
			return JsonConvert.SerializeObject(value, formatting, jsonSerializerSetting);
		}

		public static string SerializeObject(object value, Newtonsoft.Json.Formatting formatting, JsonSerializerSettings settings)
		{
			JsonSerializer jsonSerializer = JsonSerializer.Create(settings);
			StringWriter stringWriter = new StringWriter(new StringBuilder(128), CultureInfo.InvariantCulture);
			using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
			{
				jsonTextWriter.Formatting = formatting;
				jsonSerializer.Serialize(jsonTextWriter, value);
			}
			return stringWriter.ToString();
		}

		public static string SerializeXmlNode(XmlNode node)
		{
			return JsonConvert.SerializeXmlNode(node, Newtonsoft.Json.Formatting.None);
		}

		public static string SerializeXmlNode(XmlNode node, Newtonsoft.Json.Formatting formatting)
		{
			XmlNodeConverter xmlNodeConverter = new XmlNodeConverter();
			return JsonConvert.SerializeObject(node, formatting, new JsonConverter[] { xmlNodeConverter });
		}

		public static string SerializeXmlNode(XmlNode node, Newtonsoft.Json.Formatting formatting, bool omitRootObject)
		{
			XmlNodeConverter xmlNodeConverter = new XmlNodeConverter()
			{
				OmitRootObject = omitRootObject
			};
			return JsonConvert.SerializeObject(node, formatting, new JsonConverter[] { xmlNodeConverter });
		}

		public static string ToString(DateTime value)
		{
			string str;
			using (StringWriter stringWriter = StringUtils.CreateStringWriter(64))
			{
				JsonConvert.WriteDateTimeString(stringWriter, value, JsonConvert.GetUtcOffset(value), value.Kind);
				str = stringWriter.ToString();
			}
			return str;
		}

		public static string ToString(DateTimeOffset value)
		{
			string str;
			using (StringWriter stringWriter = StringUtils.CreateStringWriter(64))
			{
				JsonConvert.WriteDateTimeString(stringWriter, value.UtcDateTime, value.Offset, DateTimeKind.Local);
				str = stringWriter.ToString();
			}
			return str;
		}

		public static string ToString(bool value)
		{
			return (!value ? JsonConvert.False : JsonConvert.True);
		}

		public static string ToString(char value)
		{
			return JsonConvert.ToString(char.ToString(value));
		}

		public static string ToString(Enum value)
		{
			return value.ToString("D");
		}

		public static string ToString(int value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		public static string ToString(short value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		public static string ToString(ushort value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		public static string ToString(uint value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		public static string ToString(long value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		public static string ToString(ulong value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		public static string ToString(float value)
		{
			return JsonConvert.EnsureDecimalPlace((double)value, value.ToString("R", CultureInfo.InvariantCulture));
		}

		public static string ToString(double value)
		{
			return JsonConvert.EnsureDecimalPlace(value, value.ToString("R", CultureInfo.InvariantCulture));
		}

		public static string ToString(byte value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		public static string ToString(sbyte value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		public static string ToString(decimal value)
		{
			return JsonConvert.EnsureDecimalPlace(value.ToString(null, CultureInfo.InvariantCulture));
		}

		public static string ToString(Guid value)
		{
			return string.Concat('\"', value.ToString("D", CultureInfo.InvariantCulture), '\"');
		}

		public static string ToString(TimeSpan value)
		{
			return string.Concat('\"', value.ToString(), '\"');
		}

		public static string ToString(Uri value)
		{
			return string.Concat('\"', value.ToString(), '\"');
		}

		public static string ToString(string value)
		{
			return JsonConvert.ToString(value, '\"');
		}

		public static string ToString(string value, char delimter)
		{
			return JavaScriptUtils.ToEscapedJavaScriptString(value, delimter, true);
		}

		public static string ToString(object value)
		{
			if (value == null)
			{
				return JsonConvert.Null;
			}
			IConvertible convertible = value as IConvertible;
			if (convertible == null)
			{
				if (value is DateTimeOffset)
				{
					return JsonConvert.ToString((DateTimeOffset)value);
				}
				if (value is Guid)
				{
					return JsonConvert.ToString((Guid)value);
				}
				if (value is Uri)
				{
					return JsonConvert.ToString((Uri)value);
				}
				if (value is TimeSpan)
				{
					return JsonConvert.ToString((TimeSpan)value);
				}
			}
			else
			{
				switch (convertible.GetTypeCode())
				{
					case TypeCode.DBNull:
					{
						return JsonConvert.Null;
					}
					case TypeCode.Boolean:
					{
						return JsonConvert.ToString(convertible.ToBoolean(CultureInfo.InvariantCulture));
					}
					case TypeCode.Char:
					{
						return JsonConvert.ToString(convertible.ToChar(CultureInfo.InvariantCulture));
					}
					case TypeCode.SByte:
					{
						return JsonConvert.ToString(convertible.ToSByte(CultureInfo.InvariantCulture));
					}
					case TypeCode.Byte:
					{
						return JsonConvert.ToString(convertible.ToByte(CultureInfo.InvariantCulture));
					}
					case TypeCode.Int16:
					{
						return JsonConvert.ToString(convertible.ToInt16(CultureInfo.InvariantCulture));
					}
					case TypeCode.UInt16:
					{
						return JsonConvert.ToString(convertible.ToUInt16(CultureInfo.InvariantCulture));
					}
					case TypeCode.Int32:
					{
						return JsonConvert.ToString(convertible.ToInt32(CultureInfo.InvariantCulture));
					}
					case TypeCode.UInt32:
					{
						return JsonConvert.ToString(convertible.ToUInt32(CultureInfo.InvariantCulture));
					}
					case TypeCode.Int64:
					{
						return JsonConvert.ToString(convertible.ToInt64(CultureInfo.InvariantCulture));
					}
					case TypeCode.UInt64:
					{
						return JsonConvert.ToString(convertible.ToUInt64(CultureInfo.InvariantCulture));
					}
					case TypeCode.Single:
					{
						return JsonConvert.ToString(convertible.ToSingle(CultureInfo.InvariantCulture));
					}
					case TypeCode.Double:
					{
						return JsonConvert.ToString(convertible.ToDouble(CultureInfo.InvariantCulture));
					}
					case TypeCode.Decimal:
					{
						return JsonConvert.ToString(convertible.ToDecimal(CultureInfo.InvariantCulture));
					}
					case TypeCode.DateTime:
					{
						return JsonConvert.ToString(convertible.ToDateTime(CultureInfo.InvariantCulture));
					}
					case TypeCode.Object | TypeCode.DateTime:
					{
						break;
					}
					case TypeCode.String:
					{
						return JsonConvert.ToString(convertible.ToString(CultureInfo.InvariantCulture));
					}
					default:
					{
						goto case TypeCode.Object | TypeCode.DateTime;
					}
				}
			}
			throw new ArgumentException("Unsupported type: {0}. Use the JsonSerializer class to get the object's JSON representation.".FormatWith(CultureInfo.InvariantCulture, new object[] { value.GetType() }));
		}

		private static long ToUniversalTicks(DateTime dateTime)
		{
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				return dateTime.Ticks;
			}
			return JsonConvert.ToUniversalTicks(dateTime, JsonConvert.GetUtcOffset(dateTime));
		}

		private static long ToUniversalTicks(DateTime dateTime, TimeSpan offset)
		{
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				return dateTime.Ticks;
			}
			long ticks = dateTime.Ticks - offset.Ticks;
			if (ticks > 3155378975999999999L)
			{
				return 3155378975999999999L;
			}
			if (ticks >= (long)0)
			{
				return ticks;
			}
			return (long)0;
		}

		private static long UniversialTicksToJavaScriptTicks(long universialTicks)
		{
			return (universialTicks - JsonConvert.InitialJavaScriptDateTicks) / (long)10000;
		}

		internal static void WriteDateTimeString(TextWriter writer, DateTime value)
		{
			JsonConvert.WriteDateTimeString(writer, value, JsonConvert.GetUtcOffset(value), value.Kind);
		}

		internal static void WriteDateTimeString(TextWriter writer, DateTime value, TimeSpan offset, DateTimeKind kind)
		{
			long javaScriptTicks = JsonConvert.ConvertDateTimeToJavaScriptTicks(value, offset);
			writer.Write("\"\\/Date(");
			writer.Write(javaScriptTicks);
			switch (kind)
			{
				case DateTimeKind.Unspecified:
				case DateTimeKind.Local:
				{
					writer.Write((offset.Ticks < (long)0 ? "-" : "+"));
					int num = Math.Abs(offset.Hours);
					if (num < 10)
					{
						writer.Write(0);
					}
					writer.Write(num);
					int num1 = Math.Abs(offset.Minutes);
					if (num1 < 10)
					{
						writer.Write(0);
					}
					writer.Write(num1);
					writer.Write(")\\/\"");
					return;
				}
				case DateTimeKind.Utc:
				{
					writer.Write(")\\/\"");
					return;
				}
				default:
				{
					writer.Write(")\\/\"");
					return;
				}
			}
		}
	}
}