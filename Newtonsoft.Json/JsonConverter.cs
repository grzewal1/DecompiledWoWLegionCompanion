using Newtonsoft.Json.Schema;
using System;

namespace Newtonsoft.Json
{
	public abstract class JsonConverter
	{
		public virtual bool CanRead
		{
			get
			{
				return true;
			}
		}

		public virtual bool CanWrite
		{
			get
			{
				return true;
			}
		}

		protected JsonConverter()
		{
		}

		public abstract bool CanConvert(Type objectType);

		public virtual JsonSchema GetSchema()
		{
			return null;
		}

		public abstract object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer);

		public abstract void WriteJson(JsonWriter writer, object value, JsonSerializer serializer);
	}
}