using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace JsonDotNet.Extras.CustomConverters
{
	public class Matrix4x4Converter : JsonConverter
	{
		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		public Matrix4x4Converter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Matrix4x4);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			JToken jTokens = JToken.FromObject(value);
			if (jTokens.Type == JTokenType.Object)
			{
				JObject jObjects = (JObject)jTokens;
				IList<string> list = (
					from p in jObjects.Properties()
					where (p.Name == "inverse" ? false : p.Name != "transpose")
					select p.Name).ToList<string>();
				jObjects.AddFirst(new JProperty("Keys", new JArray(list)));
				jObjects.WriteTo(writer, new JsonConverter[0]);
			}
			else
			{
				jTokens.WriteTo(writer, new JsonConverter[0]);
			}
		}
	}
}