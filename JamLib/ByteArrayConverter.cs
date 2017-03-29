using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace JamLib
{
	public class ByteArrayConverter : JsonConverter
	{
		public ByteArrayConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(byte[]);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.StartArray)
			{
				throw new Exception(string.Format("Unexpected token parsing binary. Expected StartArray, got {0}.", reader.TokenType));
			}
			List<byte> nums = new List<byte>();
			while (reader.Read())
			{
				JsonToken tokenType = reader.TokenType;
				switch (tokenType)
				{
					case JsonToken.Comment:
					{
						continue;
					}
					case JsonToken.Integer:
					{
						nums.Add(Convert.ToByte(reader.Value));
						continue;
					}
					case JsonToken.String:
					{
						nums.Add(Convert.ToByte(reader.Value));
						continue;
					}
					default:
					{
						if (tokenType == JsonToken.EndArray)
						{
							break;
						}
						else
						{
							throw new Exception(string.Format("Unexpected token when reading bytes: {0}", reader.TokenType));
						}
					}
				}
				return nums.ToArray();
			}
			throw new Exception("Unexpected end when reading bytes.");
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			byte[] numArray = (byte[])value;
			writer.WriteStartArray();
			for (int i = 0; i < (int)numArray.Length; i++)
			{
				writer.WriteValue(numArray[i]);
			}
			writer.WriteEndArray();
		}
	}
}