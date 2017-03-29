using Newtonsoft.Json;
using System;

namespace JamLib
{
	public class JamEmbeddedMessageConverter : JsonConverter
	{
		public JamEmbeddedMessageConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(JamEmbeddedMessage);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.String)
			{
				throw new JsonSerializationException(string.Format("Unexpected token parsing binary. Expected String or StartArray, got {0}.", reader.TokenType));
			}
			string str = reader.Value.ToString();
			JamEmbeddedMessage jamEmbeddedMessage = (JamEmbeddedMessage)existingValue ?? new JamEmbeddedMessage();
			jamEmbeddedMessage.Message = MessageFactory.Deserialize(str);
			return jamEmbeddedMessage;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(MessageFactory.Serialize(((JamEmbeddedMessage)value).Message));
		}
	}
}