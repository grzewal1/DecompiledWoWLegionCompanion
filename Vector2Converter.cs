using Newtonsoft.Json;
using System;
using UnityEngine;

public class Vector2Converter : JsonConverter
{
	public override bool CanRead
	{
		get
		{
			return false;
		}
	}

	public Vector2Converter()
	{
	}

	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Vector2);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		Vector2 vector2 = (Vector2)value;
		writer.WriteStartObject();
		writer.WritePropertyName("x");
		writer.WriteValue(vector2.x);
		writer.WritePropertyName("y");
		writer.WriteValue(vector2.y);
		writer.WriteEndObject();
	}
}