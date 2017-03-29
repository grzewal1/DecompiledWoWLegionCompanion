using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;

namespace Newtonsoft.Json.Converters
{
	public class JavaScriptDateTimeConverter : DateTimeConverterBase
	{
		public JavaScriptDateTimeConverter()
		{
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			Type type = (!ReflectionUtils.IsNullableType(objectType) ? objectType : Nullable.GetUnderlyingType(objectType));
			if (reader.TokenType == JsonToken.Null)
			{
				if (!ReflectionUtils.IsNullableType(objectType))
				{
					throw new Exception("Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { objectType }));
				}
				return null;
			}
			if (reader.TokenType != JsonToken.StartConstructor || string.Compare(reader.Value.ToString(), "Date", StringComparison.Ordinal) != 0)
			{
				throw new Exception("Unexpected token or value when parsing date. Token: {0}, Value: {1}".FormatWith(CultureInfo.InvariantCulture, new object[] { reader.TokenType, reader.Value }));
			}
			reader.Read();
			if (reader.TokenType != JsonToken.Integer)
			{
				throw new Exception("Unexpected token parsing date. Expected Integer, got {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { reader.TokenType }));
			}
			DateTime dateTime = JsonConvert.ConvertJavaScriptTicksToDateTime((long)reader.Value);
			reader.Read();
			if (reader.TokenType != JsonToken.EndConstructor)
			{
				throw new Exception("Unexpected token parsing date. Expected EndConstructor, got {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { reader.TokenType }));
			}
			if (type != typeof(DateTimeOffset))
			{
				return dateTime;
			}
			return new DateTimeOffset(dateTime);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			long javaScriptTicks;
			if (!(value is DateTime))
			{
				if (!(value is DateTimeOffset))
				{
					throw new Exception("Expected date object value.");
				}
				DateTimeOffset universalTime = ((DateTimeOffset)value).ToUniversalTime();
				javaScriptTicks = JsonConvert.ConvertDateTimeToJavaScriptTicks(universalTime.UtcDateTime);
			}
			else
			{
				javaScriptTicks = JsonConvert.ConvertDateTimeToJavaScriptTicks(((DateTime)value).ToUniversalTime());
			}
			writer.WriteStartConstructor("Date");
			writer.WriteValue(javaScriptTicks);
			writer.WriteEndConstructor();
		}
	}
}