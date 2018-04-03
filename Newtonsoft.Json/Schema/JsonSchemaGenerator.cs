using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Schema
{
	public class JsonSchemaGenerator
	{
		private IContractResolver _contractResolver;

		private JsonSchemaResolver _resolver;

		private IList<JsonSchemaGenerator.TypeSchema> _stack = new List<JsonSchemaGenerator.TypeSchema>();

		private JsonSchema _currentSchema;

		public IContractResolver ContractResolver
		{
			get
			{
				if (this._contractResolver == null)
				{
					return DefaultContractResolver.Instance;
				}
				return this._contractResolver;
			}
			set
			{
				this._contractResolver = value;
			}
		}

		private JsonSchema CurrentSchema
		{
			get
			{
				return this._currentSchema;
			}
		}

		public Newtonsoft.Json.Schema.UndefinedSchemaIdHandling UndefinedSchemaIdHandling
		{
			get;
			set;
		}

		public JsonSchemaGenerator()
		{
		}

		private JsonSchemaType AddNullType(JsonSchemaType type, Required valueRequired)
		{
			if (valueRequired == Required.Always)
			{
				return type;
			}
			return type | JsonSchemaType.Null;
		}

		public JsonSchema Generate(Type type)
		{
			return this.Generate(type, new JsonSchemaResolver(), false);
		}

		public JsonSchema Generate(Type type, JsonSchemaResolver resolver)
		{
			return this.Generate(type, resolver, false);
		}

		public JsonSchema Generate(Type type, bool rootSchemaNullable)
		{
			return this.Generate(type, new JsonSchemaResolver(), rootSchemaNullable);
		}

		public JsonSchema Generate(Type type, JsonSchemaResolver resolver, bool rootSchemaNullable)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			ValidationUtils.ArgumentNotNull(resolver, "resolver");
			this._resolver = resolver;
			return this.GenerateInternal(type, (rootSchemaNullable ? Required.Default : Required.Always), false);
		}

		private JsonSchema GenerateInternal(Type type, Required valueRequired, bool required)
		{
			JsonSchemaType? nullable;
			Type type1;
			Type type2;
			JsonSchemaType? nullable1;
			ValidationUtils.ArgumentNotNull(type, "type");
			string typeId = this.GetTypeId(type, false);
			string str = this.GetTypeId(type, true);
			if (!string.IsNullOrEmpty(typeId))
			{
				JsonSchema schema = this._resolver.GetSchema(typeId);
				if (schema != null)
				{
					if (valueRequired != Required.Always && !JsonSchemaGenerator.HasFlag(schema.Type, JsonSchemaType.Null))
					{
						JsonSchema jsonSchema = schema;
						JsonSchemaType? nullable2 = jsonSchema.Type;
						if (!nullable2.HasValue)
						{
							nullable = null;
							nullable1 = nullable;
						}
						else
						{
							nullable1 = new JsonSchemaType?(nullable2.GetValueOrDefault() | JsonSchemaType.Null);
						}
						jsonSchema.Type = nullable1;
					}
					if (required)
					{
						bool? nullable3 = schema.Required;
						if ((!nullable3.GetValueOrDefault() ? true : !nullable3.HasValue))
						{
							schema.Required = new bool?(true);
						}
					}
					return schema;
				}
			}
			if (this._stack.Any<JsonSchemaGenerator.TypeSchema>((JsonSchemaGenerator.TypeSchema tc) => tc.Type == type))
			{
				throw new Exception("Unresolved circular reference for type '{0}'. Explicitly define an Id for the type using a JsonObject/JsonArray attribute or automatically generate a type Id using the UndefinedSchemaIdHandling property.".FormatWith(CultureInfo.InvariantCulture, new object[] { type }));
			}
			JsonContract jsonContract = this.ContractResolver.ResolveContract(type);
			JsonConverter converter = jsonContract.Converter;
			JsonConverter jsonConverter = converter;
			if (converter == null)
			{
				JsonConverter internalConverter = jsonContract.InternalConverter;
				jsonConverter = internalConverter;
				if (internalConverter == null)
				{
					goto Label0;
				}
			}
			JsonSchema schema1 = jsonConverter.GetSchema();
			if (schema1 != null)
			{
				return schema1;
			}
		Label0:
			this.Push(new JsonSchemaGenerator.TypeSchema(type, new JsonSchema()));
			if (str != null)
			{
				this.CurrentSchema.Id = str;
			}
			if (required)
			{
				this.CurrentSchema.Required = new bool?(true);
			}
			this.CurrentSchema.Title = this.GetTitle(type);
			this.CurrentSchema.Description = this.GetDescription(type);
			if (jsonConverter != null)
			{
				this.CurrentSchema.Type = new JsonSchemaType?(JsonSchemaType.Any);
			}
			else if (jsonContract is JsonDictionaryContract)
			{
				this.CurrentSchema.Type = new JsonSchemaType?(this.AddNullType(JsonSchemaType.Object, valueRequired));
				ReflectionUtils.GetDictionaryKeyValueTypes(type, out type1, out type2);
				if (type1 != null && typeof(IConvertible).IsAssignableFrom(type1))
				{
					this.CurrentSchema.AdditionalProperties = this.GenerateInternal(type2, Required.Default, false);
				}
			}
			else if (jsonContract is JsonArrayContract)
			{
				this.CurrentSchema.Type = new JsonSchemaType?(this.AddNullType(JsonSchemaType.Array, valueRequired));
				this.CurrentSchema.Id = this.GetTypeId(type, false);
				JsonArrayAttribute jsonContainerAttribute = JsonTypeReflector.GetJsonContainerAttribute(type) as JsonArrayAttribute;
				bool flag = (jsonContainerAttribute == null ? true : jsonContainerAttribute.AllowNullItems);
				Type collectionItemType = ReflectionUtils.GetCollectionItemType(type);
				if (collectionItemType != null)
				{
					this.CurrentSchema.Items = new List<JsonSchema>()
					{
						this.GenerateInternal(collectionItemType, (flag ? Required.Default : Required.Always), false)
					};
				}
			}
			else if (jsonContract is JsonPrimitiveContract)
			{
				this.CurrentSchema.Type = new JsonSchemaType?(this.GetJsonSchemaType(type, valueRequired));
				nullable = this.CurrentSchema.Type;
				if ((nullable.GetValueOrDefault() != JsonSchemaType.Integer ? false : nullable.HasValue) && type.IsEnum && !type.IsDefined(typeof(FlagsAttribute), true))
				{
					this.CurrentSchema.Enum = new List<JToken>();
					this.CurrentSchema.Options = new Dictionary<JToken, string>();
					foreach (EnumValue<long> namesAndValue in EnumUtils.GetNamesAndValues<long>(type))
					{
						JToken jTokens = JToken.FromObject(namesAndValue.Value);
						this.CurrentSchema.Enum.Add(jTokens);
						this.CurrentSchema.Options.Add(jTokens, namesAndValue.Name);
					}
				}
			}
			else if (jsonContract is JsonObjectContract)
			{
				this.CurrentSchema.Type = new JsonSchemaType?(this.AddNullType(JsonSchemaType.Object, valueRequired));
				this.CurrentSchema.Id = this.GetTypeId(type, false);
				this.GenerateObjectSchema(type, (JsonObjectContract)jsonContract);
			}
			else if (jsonContract is JsonISerializableContract)
			{
				this.CurrentSchema.Type = new JsonSchemaType?(this.AddNullType(JsonSchemaType.Object, valueRequired));
				this.CurrentSchema.Id = this.GetTypeId(type, false);
				this.GenerateISerializableContract(type, (JsonISerializableContract)jsonContract);
			}
			else if (!(jsonContract is JsonStringContract))
			{
				if (!(jsonContract is JsonLinqContract))
				{
					throw new Exception("Unexpected contract type: {0}".FormatWith(CultureInfo.InvariantCulture, new object[] { jsonContract }));
				}
				this.CurrentSchema.Type = new JsonSchemaType?(JsonSchemaType.Any);
			}
			else
			{
				this.CurrentSchema.Type = new JsonSchemaType?((ReflectionUtils.IsNullable(jsonContract.UnderlyingType) ? this.AddNullType(JsonSchemaType.String, valueRequired) : JsonSchemaType.String));
			}
			return this.Pop().Schema;
		}

		private void GenerateISerializableContract(Type type, JsonISerializableContract contract)
		{
			this.CurrentSchema.AllowAdditionalProperties = true;
		}

		private void GenerateObjectSchema(Type type, JsonObjectContract contract)
		{
			this.CurrentSchema.Properties = new Dictionary<string, JsonSchema>();
			foreach (JsonProperty property in contract.Properties)
			{
				if (property.Ignored)
				{
					continue;
				}
				NullValueHandling? nullValueHandling = property.NullValueHandling;
				bool flag = ((nullValueHandling.GetValueOrDefault() != NullValueHandling.Ignore ? false : nullValueHandling.HasValue) || this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(), DefaultValueHandling.Ignore) || property.ShouldSerialize != null ? true : property.GetIsSpecified != null);
				JsonSchema jsonSchema = this.GenerateInternal(property.PropertyType, property.Required, !flag);
				if (property.DefaultValue != null)
				{
					jsonSchema.Default = JToken.FromObject(property.DefaultValue);
				}
				this.CurrentSchema.Properties.Add(property.PropertyName, jsonSchema);
			}
			if (type.IsSealed)
			{
				this.CurrentSchema.AllowAdditionalProperties = false;
			}
		}

		private string GetDescription(Type type)
		{
			JsonContainerAttribute jsonContainerAttribute = JsonTypeReflector.GetJsonContainerAttribute(type);
			if (jsonContainerAttribute != null && !string.IsNullOrEmpty(jsonContainerAttribute.Description))
			{
				return jsonContainerAttribute.Description;
			}
			DescriptionAttribute attribute = ReflectionUtils.GetAttribute<DescriptionAttribute>(type);
			if (attribute == null)
			{
				return null;
			}
			return attribute.Description;
		}

		private JsonSchemaType GetJsonSchemaType(Type type, Required valueRequired)
		{
			object[] objArray;
			CultureInfo invariantCulture;
			JsonSchemaType jsonSchemaType = JsonSchemaType.None;
			if (valueRequired != Required.Always && ReflectionUtils.IsNullable(type))
			{
				jsonSchemaType = JsonSchemaType.Null;
				if (ReflectionUtils.IsNullableType(type))
				{
					type = Nullable.GetUnderlyingType(type);
				}
			}
			TypeCode typeCode = Type.GetTypeCode(type);
			switch (typeCode)
			{
				case TypeCode.Empty:
				case TypeCode.Object:
				{
					return jsonSchemaType | JsonSchemaType.String;
				}
				case TypeCode.DBNull:
				{
					return jsonSchemaType | JsonSchemaType.Null;
				}
				case TypeCode.Boolean:
				{
					return jsonSchemaType | JsonSchemaType.Boolean;
				}
				case TypeCode.Char:
				{
					return jsonSchemaType | JsonSchemaType.String;
				}
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				{
					return jsonSchemaType | JsonSchemaType.Integer;
				}
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
				{
					return jsonSchemaType | JsonSchemaType.Float;
				}
				case TypeCode.DateTime:
				{
					return jsonSchemaType | JsonSchemaType.String;
				}
				case TypeCode.Object | TypeCode.DateTime:
				{
					invariantCulture = CultureInfo.InvariantCulture;
					objArray = new object[] { typeCode, type };
					throw new Exception("Unexpected type code '{0}' for type '{1}'.".FormatWith(invariantCulture, objArray));
				}
				case TypeCode.String:
				{
					return jsonSchemaType | JsonSchemaType.String;
				}
				default:
				{
					invariantCulture = CultureInfo.InvariantCulture;
					objArray = new object[] { typeCode, type };
					throw new Exception("Unexpected type code '{0}' for type '{1}'.".FormatWith(invariantCulture, objArray));
				}
			}
		}

		private string GetTitle(Type type)
		{
			JsonContainerAttribute jsonContainerAttribute = JsonTypeReflector.GetJsonContainerAttribute(type);
			if (jsonContainerAttribute == null || string.IsNullOrEmpty(jsonContainerAttribute.Title))
			{
				return null;
			}
			return jsonContainerAttribute.Title;
		}

		private string GetTypeId(Type type, bool explicitOnly)
		{
			JsonContainerAttribute jsonContainerAttribute = JsonTypeReflector.GetJsonContainerAttribute(type);
			if (jsonContainerAttribute != null && !string.IsNullOrEmpty(jsonContainerAttribute.Id))
			{
				return jsonContainerAttribute.Id;
			}
			if (explicitOnly)
			{
				return null;
			}
			Newtonsoft.Json.Schema.UndefinedSchemaIdHandling undefinedSchemaIdHandling = this.UndefinedSchemaIdHandling;
			if (undefinedSchemaIdHandling == Newtonsoft.Json.Schema.UndefinedSchemaIdHandling.UseTypeName)
			{
				return type.FullName;
			}
			if (undefinedSchemaIdHandling != Newtonsoft.Json.Schema.UndefinedSchemaIdHandling.UseAssemblyQualifiedName)
			{
				return null;
			}
			return type.AssemblyQualifiedName;
		}

		private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag)
		{
			return (value & flag) == flag;
		}

		internal static bool HasFlag(JsonSchemaType? value, JsonSchemaType flag)
		{
			JsonSchemaType? nullable;
			JsonSchemaType? nullable1;
			if (!value.HasValue)
			{
				return true;
			}
			if (!value.HasValue)
			{
				nullable = null;
				nullable1 = nullable;
			}
			else
			{
				nullable1 = new JsonSchemaType?(value.GetValueOrDefault() & flag);
			}
			nullable = nullable1;
			return (nullable.GetValueOrDefault() != flag ? false : nullable.HasValue);
		}

		private JsonSchemaGenerator.TypeSchema Pop()
		{
			JsonSchemaGenerator.TypeSchema item = this._stack[this._stack.Count - 1];
			this._stack.RemoveAt(this._stack.Count - 1);
			JsonSchemaGenerator.TypeSchema typeSchema = this._stack.LastOrDefault<JsonSchemaGenerator.TypeSchema>();
			if (typeSchema == null)
			{
				this._currentSchema = null;
			}
			else
			{
				this._currentSchema = typeSchema.Schema;
			}
			return item;
		}

		private void Push(JsonSchemaGenerator.TypeSchema typeSchema)
		{
			this._currentSchema = typeSchema.Schema;
			this._stack.Add(typeSchema);
			this._resolver.LoadedSchemas.Add(typeSchema.Schema);
		}

		private class TypeSchema
		{
			public JsonSchema Schema
			{
				get;
				private set;
			}

			public Type Type
			{
				get;
				private set;
			}

			public TypeSchema(Type type, JsonSchema schema)
			{
				ValidationUtils.ArgumentNotNull(type, "type");
				ValidationUtils.ArgumentNotNull(schema, "schema");
				this.Type = type;
				this.Schema = schema;
			}
		}
	}
}