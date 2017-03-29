using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;

namespace Newtonsoft.Json.Converters
{
	public class BsonObjectIdConverter : JsonConverter
	{
		public BsonObjectIdConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(BsonObjectId);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.Bytes)
			{
				throw new JsonSerializationException("Expected Bytes but got {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { reader.TokenType }));
			}
			return new BsonObjectId((byte[])reader.Value);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			BsonObjectId bsonObjectId = (BsonObjectId)value;
			BsonWriter bsonWriter = writer as BsonWriter;
			if (bsonWriter == null)
			{
				writer.WriteValue(bsonObjectId.Value);
			}
			else
			{
				bsonWriter.WriteObjectId(bsonObjectId.Value);
			}
		}
	}
}