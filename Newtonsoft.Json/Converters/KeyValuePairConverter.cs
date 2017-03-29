using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Newtonsoft.Json.Converters
{
	public class KeyValuePairConverter : JsonConverter
	{
		public KeyValuePairConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			if (!objectType.IsValueType || !objectType.IsGenericType)
			{
				return false;
			}
			return objectType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			IList<Type> genericArguments = objectType.GetGenericArguments();
			Type item = genericArguments[0];
			Type type = genericArguments[1];
			reader.Read();
			reader.Read();
			object obj = serializer.Deserialize(reader, item);
			reader.Read();
			reader.Read();
			object obj1 = serializer.Deserialize(reader, type);
			reader.Read();
			return ReflectionUtils.CreateInstance(objectType, new object[] { obj, obj1 });
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			Type type = value.GetType();
			PropertyInfo property = type.GetProperty("Key");
			PropertyInfo propertyInfo = type.GetProperty("Value");
			writer.WriteStartObject();
			writer.WritePropertyName("Key");
			serializer.Serialize(writer, ReflectionUtils.GetMemberValue(property, value));
			writer.WritePropertyName("Value");
			serializer.Serialize(writer, ReflectionUtils.GetMemberValue(propertyInfo, value));
			writer.WriteEndObject();
		}
	}
}