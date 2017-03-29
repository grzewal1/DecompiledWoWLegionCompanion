using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Schema
{
	internal class JsonSchemaBuilder
	{
		private JsonReader _reader;

		private readonly IList<JsonSchema> _stack;

		private readonly JsonSchemaResolver _resolver;

		private JsonSchema _currentSchema;

		private JsonSchema CurrentSchema
		{
			get
			{
				return this._currentSchema;
			}
		}

		public JsonSchemaBuilder(JsonSchemaResolver resolver)
		{
			this._stack = new List<JsonSchema>();
			this._resolver = resolver;
		}

		private JsonSchema BuildSchema()
		{
			if (this._reader.TokenType != JsonToken.StartObject)
			{
				throw new Exception("Expected StartObject while parsing schema object, got {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { this._reader.TokenType }));
			}
			this._reader.Read();
			if (this._reader.TokenType == JsonToken.EndObject)
			{
				this.Push(new JsonSchema());
				return this.Pop();
			}
			string str = Convert.ToString(this._reader.Value, CultureInfo.InvariantCulture);
			this._reader.Read();
			if (str != "$ref")
			{
				this.Push(new JsonSchema());
				this.ProcessSchemaProperty(str);
				while (this._reader.Read() && this._reader.TokenType != JsonToken.EndObject)
				{
					str = Convert.ToString(this._reader.Value, CultureInfo.InvariantCulture);
					this._reader.Read();
					this.ProcessSchemaProperty(str);
				}
				return this.Pop();
			}
			string value = (string)this._reader.Value;
			while (this._reader.Read() && this._reader.TokenType != JsonToken.EndObject)
			{
				if (this._reader.TokenType != JsonToken.StartObject)
				{
					continue;
				}
				throw new Exception("Found StartObject within the schema reference with the Id '{0}'".FormatWith(CultureInfo.InvariantCulture, new object[] { value }));
			}
			JsonSchema schema = this._resolver.GetSchema(value);
			if (schema == null)
			{
				throw new Exception("Could not resolve schema reference for Id '{0}'.".FormatWith(CultureInfo.InvariantCulture, new object[] { value }));
			}
			return schema;
		}

		internal static JsonSchemaType MapType(string type)
		{
			JsonSchemaType jsonSchemaType;
			if (!JsonSchemaConstants.JsonSchemaTypeMapping.TryGetValue(type, out jsonSchemaType))
			{
				throw new Exception("Invalid JSON schema type: {0}".FormatWith(CultureInfo.InvariantCulture, new object[] { type }));
			}
			return jsonSchemaType;
		}

		internal static string MapType(JsonSchemaType type)
		{
			KeyValuePair<string, JsonSchemaType> keyValuePair = JsonSchemaConstants.JsonSchemaTypeMapping.Single<KeyValuePair<string, JsonSchemaType>>((KeyValuePair<string, JsonSchemaType> kv) => kv.Value == type);
			return keyValuePair.Key;
		}

		internal JsonSchema Parse(JsonReader reader)
		{
			this._reader = reader;
			if (reader.TokenType == JsonToken.None)
			{
				this._reader.Read();
			}
			return this.BuildSchema();
		}

		private JsonSchema Pop()
		{
			JsonSchema jsonSchema = this._currentSchema;
			this._stack.RemoveAt(this._stack.Count - 1);
			this._currentSchema = this._stack.LastOrDefault<JsonSchema>();
			return jsonSchema;
		}

		private void ProcessAdditionalProperties()
		{
			if (this._reader.TokenType != JsonToken.Boolean)
			{
				this.CurrentSchema.AdditionalProperties = this.BuildSchema();
			}
			else
			{
				this.CurrentSchema.AllowAdditionalProperties = (bool)this._reader.Value;
			}
		}

		private void ProcessDefault()
		{
			this.CurrentSchema.Default = JToken.ReadFrom(this._reader);
		}

		private void ProcessEnum()
		{
			if (this._reader.TokenType != JsonToken.StartArray)
			{
				throw new Exception("Expected StartArray token while parsing enum values, got {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { this._reader.TokenType }));
			}
			this.CurrentSchema.Enum = new List<JToken>();
			while (this._reader.Read() && this._reader.TokenType != JsonToken.EndArray)
			{
				JToken jTokens = JToken.ReadFrom(this._reader);
				this.CurrentSchema.Enum.Add(jTokens);
			}
		}

		private void ProcessExtends()
		{
			this.CurrentSchema.Extends = this.BuildSchema();
		}

		private void ProcessIdentity()
		{
			this.CurrentSchema.Identity = new List<string>();
			JsonToken tokenType = this._reader.TokenType;
			if (tokenType == JsonToken.StartArray)
			{
				while (this._reader.Read() && this._reader.TokenType != JsonToken.EndArray)
				{
					if (this._reader.TokenType != JsonToken.String)
					{
						throw new Exception("Exception JSON property name string token, got {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { this._reader.TokenType }));
					}
					this.CurrentSchema.Identity.Add(this._reader.Value.ToString());
				}
			}
			else
			{
				if (tokenType != JsonToken.String)
				{
					throw new Exception("Expected array or JSON property name string token, got {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { this._reader.TokenType }));
				}
				this.CurrentSchema.Identity.Add(this._reader.Value.ToString());
			}
		}

		private void ProcessItems()
		{
			this.CurrentSchema.Items = new List<JsonSchema>();
			JsonToken tokenType = this._reader.TokenType;
			if (tokenType == JsonToken.StartObject)
			{
				this.CurrentSchema.Items.Add(this.BuildSchema());
			}
			else
			{
				if (tokenType != JsonToken.StartArray)
				{
					throw new Exception("Expected array or JSON schema object token, got {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { this._reader.TokenType }));
				}
				while (this._reader.Read() && this._reader.TokenType != JsonToken.EndArray)
				{
					this.CurrentSchema.Items.Add(this.BuildSchema());
				}
			}
		}

		private void ProcessOptions()
		{
			string str;
			int num;
			this.CurrentSchema.Options = new Dictionary<JToken, string>(new JTokenEqualityComparer());
			if (this._reader.TokenType != JsonToken.StartArray)
			{
				throw new Exception("Expected array token, got {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { this._reader.TokenType }));
			}
		Label0:
			while (this._reader.Read() && this._reader.TokenType != JsonToken.EndArray)
			{
				if (this._reader.TokenType != JsonToken.StartObject)
				{
					throw new Exception("Expect object token, got {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { this._reader.TokenType }));
				}
				string value = null;
				JToken jTokens = null;
				while (true)
				{
					if (!this._reader.Read() || this._reader.TokenType == JsonToken.EndObject)
					{
						if (jTokens == null)
						{
							throw new Exception("No value specified for JSON schema option.");
						}
						if (this.CurrentSchema.Options.ContainsKey(jTokens))
						{
							throw new Exception("Duplicate value in JSON schema option collection: {0}".FormatWith(CultureInfo.InvariantCulture, new object[] { jTokens }));
						}
						this.CurrentSchema.Options.Add(jTokens, value);
						goto Label0;
					}
					else
					{
						str = Convert.ToString(this._reader.Value, CultureInfo.InvariantCulture);
						this._reader.Read();
						string str1 = str;
						if (str1 == null)
						{
							break;
						}
						if (JsonSchemaBuilder.<>f__switch$map3 == null)
						{
							Dictionary<string, int> strs = new Dictionary<string, int>(2)
							{
								{ "value", 0 },
								{ "label", 1 }
							};
							JsonSchemaBuilder.<>f__switch$map3 = strs;
						}
						if (!JsonSchemaBuilder.<>f__switch$map3.TryGetValue(str1, out num))
						{
							break;
						}
						if (num == 0)
						{
							jTokens = JToken.ReadFrom(this._reader);
						}
						else if (num == 1)
						{
							value = (string)this._reader.Value;
						}
						else
						{
							break;
						}
					}
				}
				throw new Exception("Unexpected property in JSON schema option: {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { str }));
			}
		}

		private void ProcessPatternProperties()
		{
			Dictionary<string, JsonSchema> strs = new Dictionary<string, JsonSchema>();
			if (this._reader.TokenType != JsonToken.StartObject)
			{
				throw new Exception("Expected start object token.");
			}
			while (this._reader.Read() && this._reader.TokenType != JsonToken.EndObject)
			{
				string str = Convert.ToString(this._reader.Value, CultureInfo.InvariantCulture);
				this._reader.Read();
				if (strs.ContainsKey(str))
				{
					throw new Exception("Property {0} has already been defined in schema.".FormatWith(CultureInfo.InvariantCulture, new object[] { str }));
				}
				strs.Add(str, this.BuildSchema());
			}
			this.CurrentSchema.PatternProperties = strs;
		}

		private void ProcessProperties()
		{
			IDictionary<string, JsonSchema> strs = new Dictionary<string, JsonSchema>();
			if (this._reader.TokenType != JsonToken.StartObject)
			{
				throw new Exception("Expected StartObject token while parsing schema properties, got {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { this._reader.TokenType }));
			}
			while (this._reader.Read() && this._reader.TokenType != JsonToken.EndObject)
			{
				string str = Convert.ToString(this._reader.Value, CultureInfo.InvariantCulture);
				this._reader.Read();
				if (strs.ContainsKey(str))
				{
					throw new Exception("Property {0} has already been defined in schema.".FormatWith(CultureInfo.InvariantCulture, new object[] { str }));
				}
				strs.Add(str, this.BuildSchema());
			}
			this.CurrentSchema.Properties = strs;
		}

		private void ProcessSchemaProperty(string propertyName)
		{
			int num;
			string str = propertyName;
			if (str != null)
			{
				if (JsonSchemaBuilder.<>f__switch$map2 == null)
				{
					Dictionary<string, int> strs = new Dictionary<string, int>(29)
					{
						{ "type", 0 },
						{ "id", 1 },
						{ "title", 2 },
						{ "description", 3 },
						{ "properties", 4 },
						{ "items", 5 },
						{ "additionalProperties", 6 },
						{ "patternProperties", 7 },
						{ "required", 8 },
						{ "requires", 9 },
						{ "identity", 10 },
						{ "minimum", 11 },
						{ "maximum", 12 },
						{ "exclusiveMinimum", 13 },
						{ "exclusiveMaximum", 14 },
						{ "maxLength", 15 },
						{ "minLength", 16 },
						{ "maxItems", 17 },
						{ "minItems", 18 },
						{ "divisibleBy", 19 },
						{ "disallow", 20 },
						{ "default", 21 },
						{ "hidden", 22 },
						{ "readonly", 23 },
						{ "format", 24 },
						{ "pattern", 25 },
						{ "options", 26 },
						{ "enum", 27 },
						{ "extends", 28 }
					};
					JsonSchemaBuilder.<>f__switch$map2 = strs;
				}
				if (JsonSchemaBuilder.<>f__switch$map2.TryGetValue(str, out num))
				{
					switch (num)
					{
						case 0:
						{
							this.CurrentSchema.Type = this.ProcessType();
							break;
						}
						case 1:
						{
							this.CurrentSchema.Id = (string)this._reader.Value;
							break;
						}
						case 2:
						{
							this.CurrentSchema.Title = (string)this._reader.Value;
							break;
						}
						case 3:
						{
							this.CurrentSchema.Description = (string)this._reader.Value;
							break;
						}
						case 4:
						{
							this.ProcessProperties();
							break;
						}
						case 5:
						{
							this.ProcessItems();
							break;
						}
						case 6:
						{
							this.ProcessAdditionalProperties();
							break;
						}
						case 7:
						{
							this.ProcessPatternProperties();
							break;
						}
						case 8:
						{
							this.CurrentSchema.Required = new bool?((bool)this._reader.Value);
							break;
						}
						case 9:
						{
							this.CurrentSchema.Requires = (string)this._reader.Value;
							break;
						}
						case 10:
						{
							this.ProcessIdentity();
							break;
						}
						case 11:
						{
							this.CurrentSchema.Minimum = new double?(Convert.ToDouble(this._reader.Value, CultureInfo.InvariantCulture));
							break;
						}
						case 12:
						{
							this.CurrentSchema.Maximum = new double?(Convert.ToDouble(this._reader.Value, CultureInfo.InvariantCulture));
							break;
						}
						case 13:
						{
							this.CurrentSchema.ExclusiveMinimum = new bool?((bool)this._reader.Value);
							break;
						}
						case 14:
						{
							this.CurrentSchema.ExclusiveMaximum = new bool?((bool)this._reader.Value);
							break;
						}
						case 15:
						{
							this.CurrentSchema.MaximumLength = new int?(Convert.ToInt32(this._reader.Value, CultureInfo.InvariantCulture));
							break;
						}
						case 16:
						{
							this.CurrentSchema.MinimumLength = new int?(Convert.ToInt32(this._reader.Value, CultureInfo.InvariantCulture));
							break;
						}
						case 17:
						{
							this.CurrentSchema.MaximumItems = new int?(Convert.ToInt32(this._reader.Value, CultureInfo.InvariantCulture));
							break;
						}
						case 18:
						{
							this.CurrentSchema.MinimumItems = new int?(Convert.ToInt32(this._reader.Value, CultureInfo.InvariantCulture));
							break;
						}
						case 19:
						{
							this.CurrentSchema.DivisibleBy = new double?(Convert.ToDouble(this._reader.Value, CultureInfo.InvariantCulture));
							break;
						}
						case 20:
						{
							this.CurrentSchema.Disallow = this.ProcessType();
							break;
						}
						case 21:
						{
							this.ProcessDefault();
							break;
						}
						case 22:
						{
							this.CurrentSchema.Hidden = new bool?((bool)this._reader.Value);
							break;
						}
						case 23:
						{
							this.CurrentSchema.ReadOnly = new bool?((bool)this._reader.Value);
							break;
						}
						case 24:
						{
							this.CurrentSchema.Format = (string)this._reader.Value;
							break;
						}
						case 25:
						{
							this.CurrentSchema.Pattern = (string)this._reader.Value;
							break;
						}
						case 26:
						{
							this.ProcessOptions();
							break;
						}
						case 27:
						{
							this.ProcessEnum();
							break;
						}
						case 28:
						{
							this.ProcessExtends();
							break;
						}
						default:
						{
							this._reader.Skip();
							return;
						}
					}
				}
				else
				{
					this._reader.Skip();
					return;
				}
			}
			else
			{
				this._reader.Skip();
				return;
			}
		}

		private JsonSchemaType? ProcessType()
		{
			JsonSchemaType? nullable;
			JsonToken tokenType = this._reader.TokenType;
			if (tokenType != JsonToken.StartArray)
			{
				if (tokenType != JsonToken.String)
				{
					throw new Exception("Expected array or JSON schema type string token, got {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { this._reader.TokenType }));
				}
				return new JsonSchemaType?(JsonSchemaBuilder.MapType(this._reader.Value.ToString()));
			}
			JsonSchemaType? nullable1 = new JsonSchemaType?(JsonSchemaType.None);
			while (this._reader.Read() && this._reader.TokenType != JsonToken.EndArray)
			{
				if (this._reader.TokenType != JsonToken.String)
				{
					throw new Exception("Exception JSON schema type string token, got {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { this._reader.TokenType }));
				}
				if (!nullable1.HasValue)
				{
					nullable = null;
				}
				else
				{
					nullable = new JsonSchemaType?(nullable1.GetValueOrDefault() | JsonSchemaBuilder.MapType(this._reader.Value.ToString()));
				}
				nullable1 = nullable;
			}
			return nullable1;
		}

		private void Push(JsonSchema value)
		{
			this._currentSchema = value;
			this._stack.Add(value);
			this._resolver.LoadedSchemas.Add(value);
		}
	}
}