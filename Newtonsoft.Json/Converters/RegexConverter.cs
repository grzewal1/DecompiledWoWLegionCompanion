using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Newtonsoft.Json.Converters
{
	public class RegexConverter : JsonConverter
	{
		public RegexConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Regex);
		}

		private bool HasFlag(RegexOptions options, RegexOptions flag)
		{
			return (options & flag) == flag;
		}

		private object ReadBson(BsonReader reader)
		{
			string value = (string)reader.Value;
			int num = value.LastIndexOf("/");
			string str = value.Substring(1, num - 1);
			string str1 = value.Substring(num + 1);
			RegexOptions regexOption = RegexOptions.None;
			string str2 = str1;
			for (int i = 0; i < str2.Length; i++)
			{
				char chr = str2[i];
				if (chr == 'i')
				{
					regexOption = regexOption | RegexOptions.IgnoreCase;
				}
				else if (chr == 'm')
				{
					regexOption = regexOption | RegexOptions.Multiline;
				}
				else if (chr == 's')
				{
					regexOption = regexOption | RegexOptions.Singleline;
				}
				else if (chr == 'x')
				{
					regexOption = regexOption | RegexOptions.ExplicitCapture;
				}
			}
			return new Regex(str, regexOption);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			BsonReader bsonReader = reader as BsonReader;
			if (bsonReader != null)
			{
				return this.ReadBson(bsonReader);
			}
			return this.ReadJson(reader);
		}

		private Regex ReadJson(JsonReader reader)
		{
			reader.Read();
			reader.Read();
			string value = (string)reader.Value;
			reader.Read();
			reader.Read();
			int num = Convert.ToInt32(reader.Value, CultureInfo.InvariantCulture);
			reader.Read();
			return new Regex(value, (RegexOptions)num);
		}

		private void WriteBson(BsonWriter writer, Regex regex)
		{
			string str = null;
			if (this.HasFlag(regex.Options, RegexOptions.IgnoreCase))
			{
				str = string.Concat(str, "i");
			}
			if (this.HasFlag(regex.Options, RegexOptions.Multiline))
			{
				str = string.Concat(str, "m");
			}
			if (this.HasFlag(regex.Options, RegexOptions.Singleline))
			{
				str = string.Concat(str, "s");
			}
			str = string.Concat(str, "u");
			if (this.HasFlag(regex.Options, RegexOptions.ExplicitCapture))
			{
				str = string.Concat(str, "x");
			}
			writer.WriteRegex(regex.ToString(), str);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			Regex regex = (Regex)value;
			BsonWriter bsonWriter = writer as BsonWriter;
			if (bsonWriter == null)
			{
				this.WriteJson(writer, regex);
			}
			else
			{
				this.WriteBson(bsonWriter, regex);
			}
		}

		private void WriteJson(JsonWriter writer, Regex regex)
		{
			writer.WriteStartObject();
			writer.WritePropertyName("Pattern");
			writer.WriteValue(regex.ToString());
			writer.WritePropertyName("Options");
			writer.WriteValue(regex.Options);
			writer.WriteEndObject();
		}
	}
}