using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Schema
{
	internal class JsonSchemaWriter
	{
		private readonly JsonWriter _writer;

		private readonly JsonSchemaResolver _resolver;

		public JsonSchemaWriter(JsonWriter writer, JsonSchemaResolver resolver)
		{
			ValidationUtils.ArgumentNotNull(writer, "writer");
			this._writer = writer;
			this._resolver = resolver;
		}

		private void ReferenceOrWriteSchema(JsonSchema schema)
		{
			if (schema.Id == null || this._resolver.GetSchema(schema.Id) == null)
			{
				this.WriteSchema(schema);
			}
			else
			{
				this._writer.WriteStartObject();
				this._writer.WritePropertyName("$ref");
				this._writer.WriteValue(schema.Id);
				this._writer.WriteEndObject();
			}
		}

		private void WriteItems(JsonSchema schema)
		{
			if (CollectionUtils.IsNullOrEmpty<JsonSchema>(schema.Items))
			{
				return;
			}
			this._writer.WritePropertyName("items");
			if (schema.Items.Count == 1)
			{
				this.ReferenceOrWriteSchema(schema.Items[0]);
				return;
			}
			this._writer.WriteStartArray();
			IEnumerator<JsonSchema> enumerator = schema.Items.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					this.ReferenceOrWriteSchema(enumerator.Current);
				}
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
			this._writer.WriteEndArray();
		}

		private void WritePropertyIfNotNull(JsonWriter writer, string propertyName, object value)
		{
			if (value != null)
			{
				writer.WritePropertyName(propertyName);
				writer.WriteValue(value);
			}
		}

		public void WriteSchema(JsonSchema schema)
		{
			ValidationUtils.ArgumentNotNull(schema, "schema");
			if (!this._resolver.LoadedSchemas.Contains(schema))
			{
				this._resolver.LoadedSchemas.Add(schema);
			}
			this._writer.WriteStartObject();
			this.WritePropertyIfNotNull(this._writer, "id", schema.Id);
			this.WritePropertyIfNotNull(this._writer, "title", schema.Title);
			this.WritePropertyIfNotNull(this._writer, "description", schema.Description);
			this.WritePropertyIfNotNull(this._writer, "required", schema.Required);
			this.WritePropertyIfNotNull(this._writer, "readonly", schema.ReadOnly);
			this.WritePropertyIfNotNull(this._writer, "hidden", schema.Hidden);
			this.WritePropertyIfNotNull(this._writer, "transient", schema.Transient);
			if (schema.Type.HasValue)
			{
				this.WriteType("type", this._writer, schema.Type.Value);
			}
			if (!schema.AllowAdditionalProperties)
			{
				this._writer.WritePropertyName("additionalProperties");
				this._writer.WriteValue(schema.AllowAdditionalProperties);
			}
			else if (schema.AdditionalProperties != null)
			{
				this._writer.WritePropertyName("additionalProperties");
				this.ReferenceOrWriteSchema(schema.AdditionalProperties);
			}
			this.WriteSchemaDictionaryIfNotNull(this._writer, "properties", schema.Properties);
			this.WriteSchemaDictionaryIfNotNull(this._writer, "patternProperties", schema.PatternProperties);
			this.WriteItems(schema);
			this.WritePropertyIfNotNull(this._writer, "minimum", schema.Minimum);
			this.WritePropertyIfNotNull(this._writer, "maximum", schema.Maximum);
			this.WritePropertyIfNotNull(this._writer, "exclusiveMinimum", schema.ExclusiveMinimum);
			this.WritePropertyIfNotNull(this._writer, "exclusiveMaximum", schema.ExclusiveMaximum);
			this.WritePropertyIfNotNull(this._writer, "minLength", schema.MinimumLength);
			this.WritePropertyIfNotNull(this._writer, "maxLength", schema.MaximumLength);
			this.WritePropertyIfNotNull(this._writer, "minItems", schema.MinimumItems);
			this.WritePropertyIfNotNull(this._writer, "maxItems", schema.MaximumItems);
			this.WritePropertyIfNotNull(this._writer, "divisibleBy", schema.DivisibleBy);
			this.WritePropertyIfNotNull(this._writer, "format", schema.Format);
			this.WritePropertyIfNotNull(this._writer, "pattern", schema.Pattern);
			if (schema.Enum != null)
			{
				this._writer.WritePropertyName("enum");
				this._writer.WriteStartArray();
				IEnumerator<JToken> enumerator = schema.Enum.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						enumerator.Current.WriteTo(this._writer, new JsonConverter[0]);
					}
				}
				finally
				{
					if (enumerator == null)
					{
					}
					enumerator.Dispose();
				}
				this._writer.WriteEndArray();
			}
			if (schema.Default != null)
			{
				this._writer.WritePropertyName("default");
				schema.Default.WriteTo(this._writer, new JsonConverter[0]);
			}
			if (schema.Options != null)
			{
				this._writer.WritePropertyName("options");
				this._writer.WriteStartArray();
				IEnumerator<KeyValuePair<JToken, string>> enumerator1 = schema.Options.GetEnumerator();
				try
				{
					while (enumerator1.MoveNext())
					{
						KeyValuePair<JToken, string> current = enumerator1.Current;
						this._writer.WriteStartObject();
						this._writer.WritePropertyName("value");
						current.Key.WriteTo(this._writer, new JsonConverter[0]);
						if (current.Value != null)
						{
							this._writer.WritePropertyName("label");
							this._writer.WriteValue(current.Value);
						}
						this._writer.WriteEndObject();
					}
				}
				finally
				{
					if (enumerator1 == null)
					{
					}
					enumerator1.Dispose();
				}
				this._writer.WriteEndArray();
			}
			if (schema.Disallow.HasValue)
			{
				this.WriteType("disallow", this._writer, schema.Disallow.Value);
			}
			if (schema.Extends != null)
			{
				this._writer.WritePropertyName("extends");
				this.ReferenceOrWriteSchema(schema.Extends);
			}
			this._writer.WriteEndObject();
		}

		private void WriteSchemaDictionaryIfNotNull(JsonWriter writer, string propertyName, IDictionary<string, JsonSchema> properties)
		{
			if (properties != null)
			{
				writer.WritePropertyName(propertyName);
				writer.WriteStartObject();
				IEnumerator<KeyValuePair<string, JsonSchema>> enumerator = properties.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, JsonSchema> current = enumerator.Current;
						writer.WritePropertyName(current.Key);
						this.ReferenceOrWriteSchema(current.Value);
					}
				}
				finally
				{
					if (enumerator == null)
					{
					}
					enumerator.Dispose();
				}
				writer.WriteEndObject();
			}
		}

		private void WriteType(string propertyName, JsonWriter writer, JsonSchemaType type)
		{
			IList<JsonSchemaType> jsonSchemaTypes;
			jsonSchemaTypes = (!Enum.IsDefined(typeof(JsonSchemaType), type) ? (
				from v in EnumUtils.GetFlagsValues<JsonSchemaType>(type)
				where v != JsonSchemaType.None
				select v).ToList<JsonSchemaType>() : new List<JsonSchemaType>()
			{
				type
			});
			if (jsonSchemaTypes.Count == 0)
			{
				return;
			}
			writer.WritePropertyName(propertyName);
			if (jsonSchemaTypes.Count == 1)
			{
				writer.WriteValue(JsonSchemaBuilder.MapType(jsonSchemaTypes[0]));
				return;
			}
			writer.WriteStartArray();
			IEnumerator<JsonSchemaType> enumerator = jsonSchemaTypes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					writer.WriteValue(JsonSchemaBuilder.MapType(enumerator.Current));
				}
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
			writer.WriteEndArray();
		}
	}
}