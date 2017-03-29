using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Schema
{
	public class JsonSchemaResolver
	{
		public IList<JsonSchema> LoadedSchemas
		{
			get;
			protected set;
		}

		public JsonSchemaResolver()
		{
			this.LoadedSchemas = new List<JsonSchema>();
		}

		public virtual JsonSchema GetSchema(string id)
		{
			JsonSchema jsonSchema = this.LoadedSchemas.SingleOrDefault<JsonSchema>((JsonSchema s) => s.Id == id);
			return jsonSchema;
		}
	}
}