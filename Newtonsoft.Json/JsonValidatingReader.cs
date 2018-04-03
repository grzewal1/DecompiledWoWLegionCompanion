using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace Newtonsoft.Json
{
	public class JsonValidatingReader : JsonReader, IJsonLineInfo
	{
		private readonly JsonReader _reader;

		private readonly Stack<JsonValidatingReader.SchemaScope> _stack;

		private JsonSchema _schema;

		private JsonSchemaModel _model;

		private JsonValidatingReader.SchemaScope _currentScope;

		private IEnumerable<JsonSchemaModel> CurrentMemberSchemas
		{
			get
			{
				JsonSchemaModel jsonSchemaModel;
				if (this._currentScope == null)
				{
					return new List<JsonSchemaModel>(new JsonSchemaModel[] { this._model });
				}
				if (this._currentScope.Schemas == null || this._currentScope.Schemas.Count == 0)
				{
					return Enumerable.Empty<JsonSchemaModel>();
				}
				switch (this._currentScope.TokenType)
				{
					case JTokenType.None:
					{
						return this._currentScope.Schemas;
					}
					case JTokenType.Object:
					{
						if (this._currentScope.CurrentPropertyName == null)
						{
							throw new Exception("CurrentPropertyName has not been set on scope.");
						}
						IList<JsonSchemaModel> jsonSchemaModels = new List<JsonSchemaModel>();
						foreach (JsonSchemaModel currentSchema in this.CurrentSchemas)
						{
							if (currentSchema.Properties != null && currentSchema.Properties.TryGetValue(this._currentScope.CurrentPropertyName, out jsonSchemaModel))
							{
								jsonSchemaModels.Add(jsonSchemaModel);
							}
							if (currentSchema.PatternProperties != null)
							{
								foreach (KeyValuePair<string, JsonSchemaModel> patternProperty in currentSchema.PatternProperties)
								{
									if (!Regex.IsMatch(this._currentScope.CurrentPropertyName, patternProperty.Key))
									{
										continue;
									}
									jsonSchemaModels.Add(patternProperty.Value);
								}
							}
							if (jsonSchemaModels.Count != 0 || !currentSchema.AllowAdditionalProperties || currentSchema.AdditionalProperties == null)
							{
								continue;
							}
							jsonSchemaModels.Add(currentSchema.AdditionalProperties);
						}
						return jsonSchemaModels;
					}
					case JTokenType.Array:
					{
						IList<JsonSchemaModel> jsonSchemaModels1 = new List<JsonSchemaModel>();
						foreach (JsonSchemaModel currentSchema1 in this.CurrentSchemas)
						{
							if (!CollectionUtils.IsNullOrEmpty<JsonSchemaModel>(currentSchema1.Items))
							{
								if (currentSchema1.Items.Count == 1)
								{
									jsonSchemaModels1.Add(currentSchema1.Items[0]);
								}
								if (currentSchema1.Items.Count > this._currentScope.ArrayItemCount - 1)
								{
									jsonSchemaModels1.Add(currentSchema1.Items[this._currentScope.ArrayItemCount - 1]);
								}
							}
							if (!currentSchema1.AllowAdditionalProperties || currentSchema1.AdditionalProperties == null)
							{
								continue;
							}
							jsonSchemaModels1.Add(currentSchema1.AdditionalProperties);
						}
						return jsonSchemaModels1;
					}
					case JTokenType.Constructor:
					{
						return Enumerable.Empty<JsonSchemaModel>();
					}
				}
				throw new ArgumentOutOfRangeException("TokenType", "Unexpected token type: {0}".FormatWith(CultureInfo.InvariantCulture, new object[] { this._currentScope.TokenType }));
			}
		}

		private IEnumerable<JsonSchemaModel> CurrentSchemas
		{
			get
			{
				return this._currentScope.Schemas;
			}
		}

		public override int Depth
		{
			get
			{
				return this._reader.Depth;
			}
		}

		int Newtonsoft.Json.IJsonLineInfo.LineNumber
		{
			get
			{
				IJsonLineInfo jsonLineInfo = this._reader as IJsonLineInfo;
				return (jsonLineInfo == null ? 0 : jsonLineInfo.LineNumber);
			}
		}

		int Newtonsoft.Json.IJsonLineInfo.LinePosition
		{
			get
			{
				IJsonLineInfo jsonLineInfo = this._reader as IJsonLineInfo;
				return (jsonLineInfo == null ? 0 : jsonLineInfo.LinePosition);
			}
		}

		public override char QuoteChar
		{
			get
			{
				return this._reader.QuoteChar;
			}
			protected internal set
			{
			}
		}

		public JsonReader Reader
		{
			get
			{
				return this._reader;
			}
		}

		public JsonSchema Schema
		{
			get
			{
				return this._schema;
			}
			set
			{
				if (this.TokenType != JsonToken.None)
				{
					throw new Exception("Cannot change schema while validating JSON.");
				}
				this._schema = value;
				this._model = null;
			}
		}

		public override JsonToken TokenType
		{
			get
			{
				return this._reader.TokenType;
			}
		}

		public override object Value
		{
			get
			{
				return this._reader.Value;
			}
		}

		public override Type ValueType
		{
			get
			{
				return this._reader.ValueType;
			}
		}

		public JsonValidatingReader(JsonReader reader)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			this._reader = reader;
			this._stack = new Stack<JsonValidatingReader.SchemaScope>();
		}

		private JsonSchemaType? GetCurrentNodeSchemaType()
		{
			JsonToken tokenType = this._reader.TokenType;
			switch (tokenType)
			{
				case JsonToken.Integer:
				{
					return new JsonSchemaType?(JsonSchemaType.Integer);
				}
				case JsonToken.Float:
				{
					return new JsonSchemaType?(JsonSchemaType.Float);
				}
				case JsonToken.String:
				{
					return new JsonSchemaType?(JsonSchemaType.String);
				}
				case JsonToken.Boolean:
				{
					return new JsonSchemaType?(JsonSchemaType.Boolean);
				}
				case JsonToken.Null:
				{
					return new JsonSchemaType?(JsonSchemaType.Null);
				}
				default:
				{
					if (tokenType == JsonToken.StartObject)
					{
						break;
					}
					else
					{
						if (tokenType == JsonToken.StartArray)
						{
							return new JsonSchemaType?(JsonSchemaType.Array);
						}
						return null;
					}
				}
			}
			return new JsonSchemaType?(JsonSchemaType.Object);
		}

		private bool IsPropertyDefinied(JsonSchemaModel schema, string propertyName)
		{
			bool flag;
			if (schema.Properties != null && schema.Properties.ContainsKey(propertyName))
			{
				return true;
			}
			if (schema.PatternProperties != null)
			{
				using (IEnumerator<string> enumerator = schema.PatternProperties.Keys.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!Regex.IsMatch(propertyName, enumerator.Current))
						{
							continue;
						}
						flag = true;
						return flag;
					}
					return false;
				}
				return flag;
			}
			return false;
		}

		private static bool IsZero(double value)
		{
			return Math.Abs(value) < 10 * 2.22044604925031E-16;
		}

		bool Newtonsoft.Json.IJsonLineInfo.HasLineInfo()
		{
			IJsonLineInfo jsonLineInfo = this._reader as IJsonLineInfo;
			return (jsonLineInfo == null ? false : jsonLineInfo.HasLineInfo());
		}

		private void OnValidationEvent(JsonSchemaException exception)
		{
			Newtonsoft.Json.Schema.ValidationEventHandler validationEventHandler = this.ValidationEventHandler;
			if (validationEventHandler == null)
			{
				throw exception;
			}
			validationEventHandler(this, new ValidationEventArgs(exception));
		}

		private JsonValidatingReader.SchemaScope Pop()
		{
			JsonValidatingReader.SchemaScope schemaScope;
			JsonValidatingReader.SchemaScope schemaScope1 = this._stack.Pop();
			if (this._stack.Count == 0)
			{
				schemaScope = null;
			}
			else
			{
				schemaScope = this._stack.Peek();
			}
			this._currentScope = schemaScope;
			return schemaScope1;
		}

		private void ProcessValue()
		{
			if (this._currentScope != null && this._currentScope.TokenType == JTokenType.Array)
			{
				JsonValidatingReader.SchemaScope arrayItemCount = this._currentScope;
				arrayItemCount.ArrayItemCount = arrayItemCount.ArrayItemCount + 1;
				foreach (JsonSchemaModel currentSchema in this.CurrentSchemas)
				{
					if (currentSchema == null || currentSchema.Items == null || currentSchema.Items.Count <= 1 || this._currentScope.ArrayItemCount < currentSchema.Items.Count)
					{
						continue;
					}
					this.RaiseError("Index {0} has not been defined and the schema does not allow additional items.".FormatWith(CultureInfo.InvariantCulture, new object[] { this._currentScope.ArrayItemCount }), currentSchema);
				}
			}
		}

		private void Push(JsonValidatingReader.SchemaScope scope)
		{
			this._stack.Push(scope);
			this._currentScope = scope;
		}

		private void RaiseError(string message, JsonSchemaModel schema)
		{
			IJsonLineInfo jsonLineInfo = this;
			string str = (!jsonLineInfo.HasLineInfo() ? message : string.Concat(message, " Line {0}, position {1}.".FormatWith(CultureInfo.InvariantCulture, new object[] { jsonLineInfo.LineNumber, jsonLineInfo.LinePosition })));
			this.OnValidationEvent(new JsonSchemaException(str, null, jsonLineInfo.LineNumber, jsonLineInfo.LinePosition));
		}

		public override bool Read()
		{
			if (!this._reader.Read())
			{
				return false;
			}
			if (this._reader.TokenType == JsonToken.Comment)
			{
				return true;
			}
			this.ValidateCurrentToken();
			return true;
		}

		public override byte[] ReadAsBytes()
		{
			byte[] numArray = this._reader.ReadAsBytes();
			this.ValidateCurrentToken();
			return numArray;
		}

		public override DateTimeOffset? ReadAsDateTimeOffset()
		{
			DateTimeOffset? nullable = this._reader.ReadAsDateTimeOffset();
			this.ValidateCurrentToken();
			return nullable;
		}

		public override decimal? ReadAsDecimal()
		{
			decimal? nullable = this._reader.ReadAsDecimal();
			this.ValidateCurrentToken();
			return nullable;
		}

		private bool TestType(JsonSchemaModel currentSchema, JsonSchemaType currentType)
		{
			if (JsonSchemaGenerator.HasFlag(new JsonSchemaType?(currentSchema.Type), currentType))
			{
				return true;
			}
			this.RaiseError("Invalid type. Expected {0} but got {1}.".FormatWith(CultureInfo.InvariantCulture, new object[] { currentSchema.Type, currentType }), currentSchema);
			return false;
		}

		private bool ValidateArray(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return true;
			}
			return this.TestType(schema, JsonSchemaType.Array);
		}

		private void ValidateBoolean(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return;
			}
			if (!this.TestType(schema, JsonSchemaType.Boolean))
			{
				return;
			}
			this.ValidateInEnumAndNotDisallowed(schema);
		}

		private void ValidateCurrentToken()
		{
			if (this._model == null)
			{
				this._model = (new JsonSchemaModelBuilder()).Build(this._schema);
			}
			switch (this._reader.TokenType)
			{
				case JsonToken.StartObject:
				{
					this.ProcessValue();
					IList<JsonSchemaModel> list = this.CurrentMemberSchemas.Where<JsonSchemaModel>(new Func<JsonSchemaModel, bool>(this.ValidateObject)).ToList<JsonSchemaModel>();
					this.Push(new JsonValidatingReader.SchemaScope(JTokenType.Object, list));
					break;
				}
				case JsonToken.StartArray:
				{
					this.ProcessValue();
					IList<JsonSchemaModel> jsonSchemaModels = this.CurrentMemberSchemas.Where<JsonSchemaModel>(new Func<JsonSchemaModel, bool>(this.ValidateArray)).ToList<JsonSchemaModel>();
					this.Push(new JsonValidatingReader.SchemaScope(JTokenType.Array, jsonSchemaModels));
					break;
				}
				case JsonToken.StartConstructor:
				{
					this.Push(new JsonValidatingReader.SchemaScope(JTokenType.Constructor, null));
					break;
				}
				case JsonToken.PropertyName:
				{
					foreach (JsonSchemaModel currentSchema in this.CurrentSchemas)
					{
						this.ValidatePropertyName(currentSchema);
					}
					break;
				}
				case JsonToken.Comment:
				{
					throw new ArgumentOutOfRangeException();
				}
				case JsonToken.Raw:
				{
					break;
				}
				case JsonToken.Integer:
				{
					this.ProcessValue();
					foreach (JsonSchemaModel currentMemberSchema in this.CurrentMemberSchemas)
					{
						this.ValidateInteger(currentMemberSchema);
					}
					break;
				}
				case JsonToken.Float:
				{
					this.ProcessValue();
					foreach (JsonSchemaModel jsonSchemaModel in this.CurrentMemberSchemas)
					{
						this.ValidateFloat(jsonSchemaModel);
					}
					break;
				}
				case JsonToken.String:
				{
					this.ProcessValue();
					foreach (JsonSchemaModel currentMemberSchema1 in this.CurrentMemberSchemas)
					{
						this.ValidateString(currentMemberSchema1);
					}
					break;
				}
				case JsonToken.Boolean:
				{
					this.ProcessValue();
					foreach (JsonSchemaModel jsonSchemaModel1 in this.CurrentMemberSchemas)
					{
						this.ValidateBoolean(jsonSchemaModel1);
					}
					break;
				}
				case JsonToken.Null:
				{
					this.ProcessValue();
					foreach (JsonSchemaModel currentMemberSchema2 in this.CurrentMemberSchemas)
					{
						this.ValidateNull(currentMemberSchema2);
					}
					break;
				}
				case JsonToken.Undefined:
				{
					break;
				}
				case JsonToken.EndObject:
				{
					foreach (JsonSchemaModel currentSchema1 in this.CurrentSchemas)
					{
						this.ValidateEndObject(currentSchema1);
					}
					this.Pop();
					break;
				}
				case JsonToken.EndArray:
				{
					foreach (JsonSchemaModel currentSchema2 in this.CurrentSchemas)
					{
						this.ValidateEndArray(currentSchema2);
					}
					this.Pop();
					break;
				}
				case JsonToken.EndConstructor:
				{
					this.Pop();
					break;
				}
				case JsonToken.Date:
				{
					break;
				}
				default:
				{
					throw new ArgumentOutOfRangeException();
				}
			}
		}

		private void ValidateEndArray(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return;
			}
			int arrayItemCount = this._currentScope.ArrayItemCount;
			if (schema.MaximumItems.HasValue)
			{
				int? maximumItems = schema.MaximumItems;
				if ((!maximumItems.HasValue ? false : arrayItemCount > maximumItems.GetValueOrDefault()))
				{
					this.RaiseError("Array item count {0} exceeds maximum count of {1}.".FormatWith(CultureInfo.InvariantCulture, new object[] { arrayItemCount, schema.MaximumItems }), schema);
				}
			}
			if (schema.MinimumItems.HasValue)
			{
				int? minimumItems = schema.MinimumItems;
				if ((!minimumItems.HasValue ? false : arrayItemCount < minimumItems.GetValueOrDefault()))
				{
					this.RaiseError("Array item count {0} is less than minimum count of {1}.".FormatWith(CultureInfo.InvariantCulture, new object[] { arrayItemCount, schema.MinimumItems }), schema);
				}
			}
		}

		private void ValidateEndObject(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return;
			}
			Dictionary<string, bool> requiredProperties = this._currentScope.RequiredProperties;
			if (requiredProperties != null)
			{
				List<string> list = (
					from kv in requiredProperties
					where !kv.Value
					select kv.Key).ToList<string>();
				if (list.Count > 0)
				{
					this.RaiseError("Required properties are missing from object: {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { string.Join(", ", list.ToArray()) }), schema);
				}
			}
		}

		private void ValidateFloat(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return;
			}
			if (!this.TestType(schema, JsonSchemaType.Float))
			{
				return;
			}
			this.ValidateInEnumAndNotDisallowed(schema);
			double num = Convert.ToDouble(this._reader.Value, CultureInfo.InvariantCulture);
			if (schema.Maximum.HasValue)
			{
				double? maximum = schema.Maximum;
				if ((!maximum.HasValue ? false : num > maximum.GetValueOrDefault()))
				{
					this.RaiseError("Float {0} exceeds maximum value of {1}.".FormatWith(CultureInfo.InvariantCulture, new object[] { JsonConvert.ToString(num), schema.Maximum }), schema);
				}
				if (schema.ExclusiveMaximum)
				{
					double? nullable = schema.Maximum;
					if ((num != nullable.GetValueOrDefault() ? false : nullable.HasValue))
					{
						this.RaiseError("Float {0} equals maximum value of {1} and exclusive maximum is true.".FormatWith(CultureInfo.InvariantCulture, new object[] { JsonConvert.ToString(num), schema.Maximum }), schema);
					}
				}
			}
			if (schema.Minimum.HasValue)
			{
				double? minimum = schema.Minimum;
				if ((!minimum.HasValue ? false : num < minimum.GetValueOrDefault()))
				{
					this.RaiseError("Float {0} is less than minimum value of {1}.".FormatWith(CultureInfo.InvariantCulture, new object[] { JsonConvert.ToString(num), schema.Minimum }), schema);
				}
				if (schema.ExclusiveMinimum)
				{
					double? minimum1 = schema.Minimum;
					if ((num != minimum1.GetValueOrDefault() ? false : minimum1.HasValue))
					{
						this.RaiseError("Float {0} equals minimum value of {1} and exclusive minimum is true.".FormatWith(CultureInfo.InvariantCulture, new object[] { JsonConvert.ToString(num), schema.Minimum }), schema);
					}
				}
			}
			if (schema.DivisibleBy.HasValue && !JsonValidatingReader.IsZero(num % schema.DivisibleBy.Value))
			{
				this.RaiseError("Float {0} is not evenly divisible by {1}.".FormatWith(CultureInfo.InvariantCulture, new object[] { JsonConvert.ToString(num), schema.DivisibleBy }), schema);
			}
		}

		private void ValidateInEnumAndNotDisallowed(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return;
			}
			JToken jValue = new JValue(this._reader.Value);
			if (schema.Enum != null)
			{
				StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
				jValue.WriteTo(new JsonTextWriter(stringWriter), new JsonConverter[0]);
				if (!schema.Enum.ContainsValue<JToken>(jValue, new JTokenEqualityComparer()))
				{
					this.RaiseError("Value {0} is not defined in enum.".FormatWith(CultureInfo.InvariantCulture, new object[] { stringWriter.ToString() }), schema);
				}
			}
			JsonSchemaType? currentNodeSchemaType = this.GetCurrentNodeSchemaType();
			if (currentNodeSchemaType.HasValue && JsonSchemaGenerator.HasFlag(new JsonSchemaType?(schema.Disallow), currentNodeSchemaType.Value))
			{
				this.RaiseError("Type {0} is disallowed.".FormatWith(CultureInfo.InvariantCulture, new object[] { currentNodeSchemaType }), schema);
			}
		}

		private void ValidateInteger(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return;
			}
			if (!this.TestType(schema, JsonSchemaType.Integer))
			{
				return;
			}
			this.ValidateInEnumAndNotDisallowed(schema);
			long num = Convert.ToInt64(this._reader.Value, CultureInfo.InvariantCulture);
			if (schema.Maximum.HasValue)
			{
				double? maximum = schema.Maximum;
				if ((!maximum.HasValue ? false : (double)num > maximum.GetValueOrDefault()))
				{
					this.RaiseError("Integer {0} exceeds maximum value of {1}.".FormatWith(CultureInfo.InvariantCulture, new object[] { num, schema.Maximum }), schema);
				}
				if (schema.ExclusiveMaximum)
				{
					double num1 = (double)num;
					double? nullable = schema.Maximum;
					if ((num1 != nullable.GetValueOrDefault() ? false : nullable.HasValue))
					{
						this.RaiseError("Integer {0} equals maximum value of {1} and exclusive maximum is true.".FormatWith(CultureInfo.InvariantCulture, new object[] { num, schema.Maximum }), schema);
					}
				}
			}
			if (schema.Minimum.HasValue)
			{
				double? minimum = schema.Minimum;
				if ((!minimum.HasValue ? false : (double)num < minimum.GetValueOrDefault()))
				{
					this.RaiseError("Integer {0} is less than minimum value of {1}.".FormatWith(CultureInfo.InvariantCulture, new object[] { num, schema.Minimum }), schema);
				}
				if (schema.ExclusiveMinimum)
				{
					double num2 = (double)num;
					double? minimum1 = schema.Minimum;
					if ((num2 != minimum1.GetValueOrDefault() ? false : minimum1.HasValue))
					{
						this.RaiseError("Integer {0} equals minimum value of {1} and exclusive minimum is true.".FormatWith(CultureInfo.InvariantCulture, new object[] { num, schema.Minimum }), schema);
					}
				}
			}
			if (schema.DivisibleBy.HasValue && !JsonValidatingReader.IsZero((double)num % schema.DivisibleBy.Value))
			{
				this.RaiseError("Integer {0} is not evenly divisible by {1}.".FormatWith(CultureInfo.InvariantCulture, new object[] { JsonConvert.ToString(num), schema.DivisibleBy }), schema);
			}
		}

		private void ValidateNull(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return;
			}
			if (!this.TestType(schema, JsonSchemaType.Null))
			{
				return;
			}
			this.ValidateInEnumAndNotDisallowed(schema);
		}

		private bool ValidateObject(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return true;
			}
			return this.TestType(schema, JsonSchemaType.Object);
		}

		private void ValidatePropertyName(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return;
			}
			string str = Convert.ToString(this._reader.Value, CultureInfo.InvariantCulture);
			if (this._currentScope.RequiredProperties.ContainsKey(str))
			{
				this._currentScope.RequiredProperties[str] = true;
			}
			if (!schema.AllowAdditionalProperties && !this.IsPropertyDefinied(schema, str))
			{
				this.RaiseError("Property '{0}' has not been defined and the schema does not allow additional properties.".FormatWith(CultureInfo.InvariantCulture, new object[] { str }), schema);
			}
			this._currentScope.CurrentPropertyName = str;
		}

		private void ValidateString(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return;
			}
			if (!this.TestType(schema, JsonSchemaType.String))
			{
				return;
			}
			this.ValidateInEnumAndNotDisallowed(schema);
			string str = this._reader.Value.ToString();
			if (schema.MaximumLength.HasValue)
			{
				int? maximumLength = schema.MaximumLength;
				if ((!maximumLength.HasValue ? false : str.Length > maximumLength.GetValueOrDefault()))
				{
					this.RaiseError("String '{0}' exceeds maximum length of {1}.".FormatWith(CultureInfo.InvariantCulture, new object[] { str, schema.MaximumLength }), schema);
				}
			}
			if (schema.MinimumLength.HasValue)
			{
				int? minimumLength = schema.MinimumLength;
				if ((!minimumLength.HasValue ? false : str.Length < minimumLength.GetValueOrDefault()))
				{
					this.RaiseError("String '{0}' is less than minimum length of {1}.".FormatWith(CultureInfo.InvariantCulture, new object[] { str, schema.MinimumLength }), schema);
				}
			}
			if (schema.Patterns != null)
			{
				foreach (string pattern in schema.Patterns)
				{
					if (Regex.IsMatch(str, pattern))
					{
						continue;
					}
					this.RaiseError("String '{0}' does not match regex pattern '{1}'.".FormatWith(CultureInfo.InvariantCulture, new object[] { str, pattern }), schema);
				}
			}
		}

		public event Newtonsoft.Json.Schema.ValidationEventHandler ValidationEventHandler
		{
			add
			{
				Newtonsoft.Json.Schema.ValidationEventHandler validationEventHandler;
				Newtonsoft.Json.Schema.ValidationEventHandler validationEventHandler1 = this.ValidationEventHandler;
				do
				{
					validationEventHandler = validationEventHandler1;
					validationEventHandler1 = Interlocked.CompareExchange<Newtonsoft.Json.Schema.ValidationEventHandler>(ref this.ValidationEventHandler, (Newtonsoft.Json.Schema.ValidationEventHandler)Delegate.Combine(validationEventHandler, value), validationEventHandler1);
				}
				while ((object)validationEventHandler1 != (object)validationEventHandler);
			}
			remove
			{
				Newtonsoft.Json.Schema.ValidationEventHandler validationEventHandler;
				Newtonsoft.Json.Schema.ValidationEventHandler validationEventHandler1 = this.ValidationEventHandler;
				do
				{
					validationEventHandler = validationEventHandler1;
					validationEventHandler1 = Interlocked.CompareExchange<Newtonsoft.Json.Schema.ValidationEventHandler>(ref this.ValidationEventHandler, (Newtonsoft.Json.Schema.ValidationEventHandler)Delegate.Remove(validationEventHandler, value), validationEventHandler1);
				}
				while ((object)validationEventHandler1 != (object)validationEventHandler);
			}
		}

		private class SchemaScope
		{
			private readonly JTokenType _tokenType;

			private readonly IList<JsonSchemaModel> _schemas;

			private readonly Dictionary<string, bool> _requiredProperties;

			public int ArrayItemCount
			{
				get;
				set;
			}

			public string CurrentPropertyName
			{
				get;
				set;
			}

			public Dictionary<string, bool> RequiredProperties
			{
				get
				{
					return this._requiredProperties;
				}
			}

			public IList<JsonSchemaModel> Schemas
			{
				get
				{
					return this._schemas;
				}
			}

			public JTokenType TokenType
			{
				get
				{
					return this._tokenType;
				}
			}

			public SchemaScope(JTokenType tokenType, IList<JsonSchemaModel> schemas)
			{
				this._tokenType = tokenType;
				this._schemas = schemas;
				this._requiredProperties = schemas.SelectMany<JsonSchemaModel, string>(new Func<JsonSchemaModel, IEnumerable<string>>(this.GetRequiredProperties)).Distinct<string>().ToDictionary<string, string, bool>((string p) => p, (string p) => false);
			}

			private IEnumerable<string> GetRequiredProperties(JsonSchemaModel schema)
			{
				if (schema == null || schema.Properties == null)
				{
					return Enumerable.Empty<string>();
				}
				return 
					from p in schema.Properties
					where p.Value.Required
					select p.Key;
			}
		}
	}
}