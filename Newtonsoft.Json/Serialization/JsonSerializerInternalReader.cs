using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Newtonsoft.Json.Serialization
{
	internal class JsonSerializerInternalReader : JsonSerializerInternalBase
	{
		private JsonSerializerProxy _internalSerializer;

		private JsonFormatterConverter _formatterConverter;

		public JsonSerializerInternalReader(JsonSerializer serializer) : base(serializer)
		{
		}

		private void CheckedRead(JsonReader reader)
		{
			if (!reader.Read())
			{
				throw new JsonSerializationException("Unexpected end when deserializing object.");
			}
		}

		private object CreateAndPopulateDictionary(JsonReader reader, JsonDictionaryContract contract, string id)
		{
			if (contract.DefaultCreator == null || contract.DefaultCreatorNonPublic && base.Serializer.ConstructorHandling != ConstructorHandling.AllowNonPublicDefaultConstructor)
			{
				throw new JsonSerializationException("Unable to find a default constructor to use for type {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { contract.UnderlyingType }));
			}
			IWrappedDictionary wrappedDictionaries = contract.CreateWrapper(contract.DefaultCreator());
			this.PopulateDictionary(wrappedDictionaries, reader, contract, id);
			return wrappedDictionaries.UnderlyingDictionary;
		}

		private object CreateAndPopulateList(JsonReader reader, string reference, JsonArrayContract contract)
		{
			return CollectionUtils.CreateAndPopulateList(contract.CreatedType, (IList l, bool isTemporaryListReference) => {
				if (reference != null && isTemporaryListReference)
				{
					throw new JsonSerializationException("Cannot preserve reference to array or readonly list: {0}".FormatWith(CultureInfo.InvariantCulture, new object[] { contract.UnderlyingType }));
				}
				if (contract.OnSerializing != null && isTemporaryListReference)
				{
					throw new JsonSerializationException("Cannot call OnSerializing on an array or readonly list: {0}".FormatWith(CultureInfo.InvariantCulture, new object[] { contract.UnderlyingType }));
				}
				if (contract.OnError != null && isTemporaryListReference)
				{
					throw new JsonSerializationException("Cannot call OnError on an array or readonly list: {0}".FormatWith(CultureInfo.InvariantCulture, new object[] { contract.UnderlyingType }));
				}
				if (contract.IsMultidimensionalArray)
				{
					this.PopulateMultidimensionalArray(l, reader, reference, contract);
				}
				else
				{
					this.PopulateList(contract.CreateWrapper(l), reader, reference, contract);
				}
			});
		}

		private object CreateAndPopulateObject(JsonReader reader, JsonObjectContract contract, string id)
		{
			object defaultCreator = null;
			if (contract.UnderlyingType.IsInterface || contract.UnderlyingType.IsAbstract)
			{
				throw new JsonSerializationException("Could not create an instance of type {0}. Type is an interface or abstract class and cannot be instantated.".FormatWith(CultureInfo.InvariantCulture, new object[] { contract.UnderlyingType }));
			}
			if (contract.OverrideConstructor != null)
			{
				if ((int)contract.OverrideConstructor.GetParameters().Length > 0)
				{
					return this.CreateObjectFromNonDefaultConstructor(reader, contract, contract.OverrideConstructor, id);
				}
				defaultCreator = contract.OverrideConstructor.Invoke(null);
			}
			else if (contract.DefaultCreator != null && (!contract.DefaultCreatorNonPublic || base.Serializer.ConstructorHandling == ConstructorHandling.AllowNonPublicDefaultConstructor))
			{
				defaultCreator = contract.DefaultCreator();
			}
			else if (contract.ParametrizedConstructor != null)
			{
				return this.CreateObjectFromNonDefaultConstructor(reader, contract, contract.ParametrizedConstructor, id);
			}
			if (defaultCreator == null)
			{
				throw new JsonSerializationException("Unable to find a constructor to use for type {0}. A class should either have a default constructor, one constructor with arguments or a constructor marked with the JsonConstructor attribute.".FormatWith(CultureInfo.InvariantCulture, new object[] { contract.UnderlyingType }));
			}
			this.PopulateObject(defaultCreator, reader, contract, id);
			return defaultCreator;
		}

		private object CreateISerializable(JsonReader reader, JsonISerializableContract contract, string id)
		{
			Type underlyingType = contract.UnderlyingType;
			SerializationInfo serializationInfo = new SerializationInfo(contract.UnderlyingType, this.GetFormatterConverter());
			bool flag = false;
			do
			{
				JsonToken tokenType = reader.TokenType;
				if (tokenType == JsonToken.PropertyName)
				{
					string str = reader.Value.ToString();
					if (!reader.Read())
					{
						throw new JsonSerializationException("Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, new object[] { str }));
					}
					serializationInfo.AddValue(str, JToken.ReadFrom(reader));
				}
				else if (tokenType != JsonToken.Comment)
				{
					if (tokenType != JsonToken.EndObject)
					{
						throw new JsonSerializationException(string.Concat("Unexpected token when deserializing object: ", reader.TokenType));
					}
					flag = true;
				}
			}
			while (!flag && reader.Read());
			if (contract.ISerializableCreator == null)
			{
				throw new JsonSerializationException("ISerializable type '{0}' does not have a valid constructor. To correctly implement ISerializable a constructor that takes SerializationInfo and StreamingContext parameters should be present.".FormatWith(CultureInfo.InvariantCulture, new object[] { underlyingType }));
			}
			object serializableCreator = contract.ISerializableCreator(new object[] { serializationInfo, base.Serializer.Context });
			if (id != null)
			{
				base.Serializer.ReferenceResolver.AddReference(this, id, serializableCreator);
			}
			contract.InvokeOnDeserializing(serializableCreator, base.Serializer.Context);
			contract.InvokeOnDeserialized(serializableCreator, base.Serializer.Context);
			return serializableCreator;
		}

		private JToken CreateJObject(JsonReader reader)
		{
			JToken token;
			ValidationUtils.ArgumentNotNull(reader, "reader");
			using (JTokenWriter jTokenWriter = new JTokenWriter())
			{
				jTokenWriter.WriteStartObject();
				if (reader.TokenType != JsonToken.PropertyName)
				{
					jTokenWriter.WriteEndObject();
				}
				else
				{
					jTokenWriter.WriteToken(reader, reader.Depth - 1);
				}
				token = jTokenWriter.Token;
			}
			return token;
		}

		private JToken CreateJToken(JsonReader reader, JsonContract contract)
		{
			JToken token;
			ValidationUtils.ArgumentNotNull(reader, "reader");
			if (contract != null && contract.UnderlyingType == typeof(JRaw))
			{
				return JRaw.Create(reader);
			}
			using (JTokenWriter jTokenWriter = new JTokenWriter())
			{
				jTokenWriter.WriteToken(reader);
				token = jTokenWriter.Token;
			}
			return token;
		}

		private object CreateList(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, object existingValue, string reference)
		{
			object obj;
			if (!this.HasDefinedType(objectType))
			{
				obj = this.CreateJToken(reader, contract);
			}
			else
			{
				JsonArrayContract jsonArrayContract = this.EnsureArrayContract(objectType, contract);
				obj = (existingValue == null || objectType == typeof(BitArray) ? this.CreateAndPopulateList(reader, reference, jsonArrayContract) : this.PopulateList(jsonArrayContract.CreateWrapper(existingValue), reader, reference, jsonArrayContract));
			}
			return obj;
		}

		private object CreateObject(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, object existingValue)
		{
			bool flag;
			string str;
			string str1;
			Type type;
			string str2;
			TypeNameHandling? typeNameHandling;
			string str3;
			this.CheckedRead(reader);
			string str4 = null;
			if (reader.TokenType == JsonToken.PropertyName)
			{
				do
				{
					string str5 = reader.Value.ToString();
					if (string.Equals(str5, "$ref", StringComparison.Ordinal))
					{
						this.CheckedRead(reader);
						if (reader.TokenType != JsonToken.String && reader.TokenType != JsonToken.Null)
						{
							throw new JsonSerializationException("JSON reference {0} property must have a string or null value.".FormatWith(CultureInfo.InvariantCulture, new object[] { "$ref" }));
						}
						if (reader.Value == null)
						{
							str3 = null;
						}
						else
						{
							str3 = reader.Value.ToString();
						}
						string str6 = str3;
						this.CheckedRead(reader);
						if (str6 != null)
						{
							if (reader.TokenType == JsonToken.PropertyName)
							{
								throw new JsonSerializationException("Additional content found in JSON reference object. A JSON reference object should only have a {0} property.".FormatWith(CultureInfo.InvariantCulture, new object[] { "$ref" }));
							}
							return base.Serializer.ReferenceResolver.ResolveReference(this, str6);
						}
						flag = true;
					}
					else if (string.Equals(str5, "$type", StringComparison.Ordinal))
					{
						this.CheckedRead(reader);
						string str7 = reader.Value.ToString();
						this.CheckedRead(reader);
						if (member == null)
						{
							typeNameHandling = null;
						}
						else
						{
							typeNameHandling = member.TypeNameHandling;
						}
						TypeNameHandling? nullable = typeNameHandling;
						if ((!nullable.HasValue ? base.Serializer.TypeNameHandling : nullable.Value))
						{
							ReflectionUtils.SplitFullyQualifiedTypeName(str7, out str, out str1);
							try
							{
								type = base.Serializer.Binder.BindToType(str1, str);
							}
							catch (Exception exception1)
							{
								Exception exception = exception1;
								throw new JsonSerializationException("Error resolving type specified in JSON '{0}'.".FormatWith(CultureInfo.InvariantCulture, new object[] { str7 }), exception);
							}
							if (type == null)
							{
								throw new JsonSerializationException("Type specified in JSON '{0}' was not resolved.".FormatWith(CultureInfo.InvariantCulture, new object[] { str7 }));
							}
							if (objectType != null && !objectType.IsAssignableFrom(type))
							{
								throw new JsonSerializationException("Type specified in JSON '{0}' is not compatible with '{1}'.".FormatWith(CultureInfo.InvariantCulture, new object[] { type.AssemblyQualifiedName, objectType.AssemblyQualifiedName }));
							}
							objectType = type;
							contract = this.GetContractSafe(type);
						}
						flag = true;
					}
					else if (!string.Equals(str5, "$id", StringComparison.Ordinal))
					{
						if (string.Equals(str5, "$values", StringComparison.Ordinal))
						{
							this.CheckedRead(reader);
							object obj = this.CreateList(reader, objectType, contract, member, existingValue, str4);
							this.CheckedRead(reader);
							return obj;
						}
						flag = false;
					}
					else
					{
						this.CheckedRead(reader);
						if (reader.Value == null)
						{
							str2 = null;
						}
						else
						{
							str2 = reader.Value.ToString();
						}
						str4 = str2;
						this.CheckedRead(reader);
						flag = true;
					}
				}
				while (flag && reader.TokenType == JsonToken.PropertyName);
			}
			if (!this.HasDefinedType(objectType))
			{
				return this.CreateJObject(reader);
			}
			if (contract == null)
			{
				throw new JsonSerializationException("Could not resolve type '{0}' to a JsonContract.".FormatWith(CultureInfo.InvariantCulture, new object[] { objectType }));
			}
			JsonDictionaryContract jsonDictionaryContract = contract as JsonDictionaryContract;
			if (jsonDictionaryContract != null)
			{
				if (existingValue == null)
				{
					return this.CreateAndPopulateDictionary(reader, jsonDictionaryContract, str4);
				}
				return this.PopulateDictionary(jsonDictionaryContract.CreateWrapper(existingValue), reader, jsonDictionaryContract, str4);
			}
			JsonObjectContract jsonObjectContract = contract as JsonObjectContract;
			if (jsonObjectContract != null)
			{
				if (existingValue == null)
				{
					return this.CreateAndPopulateObject(reader, jsonObjectContract, str4);
				}
				return this.PopulateObject(existingValue, reader, jsonObjectContract, str4);
			}
			JsonPrimitiveContract jsonPrimitiveContract = contract as JsonPrimitiveContract;
			if (jsonPrimitiveContract != null && reader.TokenType == JsonToken.PropertyName && string.Equals(reader.Value.ToString(), "$value", StringComparison.Ordinal))
			{
				this.CheckedRead(reader);
				object obj1 = this.CreateValueInternal(reader, objectType, jsonPrimitiveContract, member, existingValue);
				this.CheckedRead(reader);
				return obj1;
			}
			JsonISerializableContract jsonISerializableContract = contract as JsonISerializableContract;
			if (jsonISerializableContract == null)
			{
				throw new JsonSerializationException("Cannot deserialize JSON object into type '{0}'.".FormatWith(CultureInfo.InvariantCulture, new object[] { objectType }));
			}
			return this.CreateISerializable(reader, jsonISerializableContract, str4);
		}

		private object CreateObjectFromNonDefaultConstructor(JsonReader reader, JsonObjectContract contract, ConstructorInfo constructorInfo, string id)
		{
			ValidationUtils.ArgumentNotNull(constructorInfo, "constructorInfo");
			IDictionary<JsonProperty, object> jsonProperties = this.ResolvePropertyAndConstructorValues(contract, reader, contract.UnderlyingType);
			ParameterInfo[] parameters = constructorInfo.GetParameters();
			Func<ParameterInfo, ParameterInfo> func = (ParameterInfo p) => p;
			IDictionary<ParameterInfo, object> dictionary = ((IEnumerable<ParameterInfo>)parameters).ToDictionary<ParameterInfo, ParameterInfo, object>(func, (ParameterInfo p) => null);
			IDictionary<JsonProperty, object> jsonProperties1 = new Dictionary<JsonProperty, object>();
			IEnumerator<KeyValuePair<JsonProperty, object>> enumerator = jsonProperties.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<JsonProperty, object> current = enumerator.Current;
					KeyValuePair<ParameterInfo, object> keyValuePair = dictionary.ForgivingCaseSensitiveFind<KeyValuePair<ParameterInfo, object>>((KeyValuePair<ParameterInfo, object> kv) => kv.Key.Name, current.Key.UnderlyingName);
					ParameterInfo key = keyValuePair.Key;
					if (key == null)
					{
						jsonProperties1.Add(current);
					}
					else
					{
						dictionary[key] = current.Value;
					}
				}
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
			object obj = constructorInfo.Invoke(dictionary.Values.ToArray<object>());
			if (id != null)
			{
				base.Serializer.ReferenceResolver.AddReference(this, id, obj);
			}
			contract.InvokeOnDeserializing(obj, base.Serializer.Context);
			IEnumerator<KeyValuePair<JsonProperty, object>> enumerator1 = jsonProperties1.GetEnumerator();
			try
			{
				while (enumerator1.MoveNext())
				{
					KeyValuePair<JsonProperty, object> current1 = enumerator1.Current;
					JsonProperty jsonProperty = current1.Key;
					object value = current1.Value;
					if (!this.ShouldSetPropertyValue(current1.Key, current1.Value))
					{
						if (jsonProperty.Writable || value == null)
						{
							continue;
						}
						JsonContract jsonContract = base.Serializer.ContractResolver.ResolveContract(jsonProperty.PropertyType);
						if (!(jsonContract is JsonArrayContract))
						{
							if (!(jsonContract is JsonDictionaryContract))
							{
								continue;
							}
							JsonDictionaryContract jsonDictionaryContract = jsonContract as JsonDictionaryContract;
							object value1 = jsonProperty.ValueProvider.GetValue(obj);
							if (value1 == null)
							{
								continue;
							}
							IWrappedDictionary wrappedDictionaries = jsonDictionaryContract.CreateWrapper(value1);
							IEnumerator enumerator2 = jsonDictionaryContract.CreateWrapper(value).GetEnumerator();
							try
							{
								while (enumerator2.MoveNext())
								{
									DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator2.Current;
									wrappedDictionaries.Add(dictionaryEntry.Key, dictionaryEntry.Value);
								}
							}
							finally
							{
								IDisposable disposable = enumerator2 as IDisposable;
								if (disposable == null)
								{
								}
								disposable.Dispose();
							}
						}
						else
						{
							JsonArrayContract jsonArrayContract = jsonContract as JsonArrayContract;
							object obj1 = jsonProperty.ValueProvider.GetValue(obj);
							if (obj1 != null)
							{
								IWrappedCollection wrappedCollections = jsonArrayContract.CreateWrapper(obj1);
								IEnumerator enumerator3 = jsonArrayContract.CreateWrapper(value).GetEnumerator();
								try
								{
									while (enumerator3.MoveNext())
									{
										wrappedCollections.Add(enumerator3.Current);
									}
								}
								finally
								{
									IDisposable disposable1 = enumerator3 as IDisposable;
									if (disposable1 == null)
									{
									}
									disposable1.Dispose();
								}
							}
						}
					}
					else
					{
						jsonProperty.ValueProvider.SetValue(obj, value);
					}
				}
			}
			finally
			{
				if (enumerator1 == null)
				{
				}
				enumerator1.Dispose();
			}
			contract.InvokeOnDeserialized(obj, base.Serializer.Context);
			return obj;
		}

		private object CreateValueInternal(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, object existingValue)
		{
			if (contract is JsonLinqContract)
			{
				return this.CreateJToken(reader, contract);
			}
			do
			{
				switch (reader.TokenType)
				{
					case JsonToken.StartObject:
					{
						return this.CreateObject(reader, objectType, contract, member, existingValue);
					}
					case JsonToken.StartArray:
					{
						return this.CreateList(reader, objectType, contract, member, existingValue, null);
					}
					case JsonToken.StartConstructor:
					case JsonToken.EndConstructor:
					{
						return reader.Value.ToString();
					}
					case JsonToken.PropertyName:
					case JsonToken.EndObject:
					case JsonToken.EndArray:
					{
						throw new JsonSerializationException(string.Concat("Unexpected token while deserializing object: ", reader.TokenType));
					}
					case JsonToken.Comment:
					{
						continue;
					}
					case JsonToken.Raw:
					{
						return new JRaw((string)reader.Value);
					}
					case JsonToken.Integer:
					case JsonToken.Float:
					case JsonToken.Boolean:
					case JsonToken.Date:
					case JsonToken.Bytes:
					{
						return this.EnsureType(reader.Value, CultureInfo.InvariantCulture, objectType);
					}
					case JsonToken.String:
					{
						if (string.IsNullOrEmpty((string)reader.Value) && objectType != null && ReflectionUtils.IsNullableType(objectType))
						{
							return null;
						}
						if (objectType == typeof(byte[]))
						{
							return Convert.FromBase64String((string)reader.Value);
						}
						return this.EnsureType(reader.Value, CultureInfo.InvariantCulture, objectType);
					}
					case JsonToken.Null:
					case JsonToken.Undefined:
					{
						if (objectType == typeof(DBNull))
						{
							return DBNull.Value;
						}
						return this.EnsureType(reader.Value, CultureInfo.InvariantCulture, objectType);
					}
					default:
					{
						throw new JsonSerializationException(string.Concat("Unexpected token while deserializing object: ", reader.TokenType));
					}
				}
			}
			while (reader.Read());
			throw new JsonSerializationException("Unexpected end when deserializing object.");
		}

		private object CreateValueNonProperty(JsonReader reader, Type objectType, JsonContract contract)
		{
			JsonConverter converter = this.GetConverter(contract, null);
			if (converter != null && converter.CanRead)
			{
				return converter.ReadJson(reader, objectType, null, this.GetInternalSerializer());
			}
			return this.CreateValueInternal(reader, objectType, contract, null, null);
		}

		private object CreateValueProperty(JsonReader reader, JsonProperty property, object target, bool gottenCurrentValue, object currentValue)
		{
			JsonContract contractSafe = this.GetContractSafe(property.PropertyType, currentValue);
			Type propertyType = property.PropertyType;
			JsonConverter converter = this.GetConverter(contractSafe, property.MemberConverter);
			if (converter == null || !converter.CanRead)
			{
				return this.CreateValueInternal(reader, propertyType, contractSafe, property, currentValue);
			}
			if (!gottenCurrentValue && target != null && property.Readable)
			{
				currentValue = property.ValueProvider.GetValue(target);
			}
			return converter.ReadJson(reader, propertyType, currentValue, this.GetInternalSerializer());
		}

		public object Deserialize(JsonReader reader, Type objectType)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (reader.TokenType == JsonToken.None && !this.ReadForType(reader, objectType, null))
			{
				return null;
			}
			return this.CreateValueNonProperty(reader, objectType, this.GetContractSafe(objectType));
		}

		private JsonArrayContract EnsureArrayContract(Type objectType, JsonContract contract)
		{
			if (contract == null)
			{
				throw new JsonSerializationException("Could not resolve type '{0}' to a JsonContract.".FormatWith(CultureInfo.InvariantCulture, new object[] { objectType }));
			}
			JsonArrayContract jsonArrayContract = contract as JsonArrayContract;
			if (jsonArrayContract == null)
			{
				throw new JsonSerializationException("Cannot deserialize JSON array into type '{0}'.".FormatWith(CultureInfo.InvariantCulture, new object[] { objectType }));
			}
			return jsonArrayContract;
		}

		private object EnsureType(object value, CultureInfo culture, Type targetType)
		{
			object obj;
			if (targetType == null)
			{
				return value;
			}
			if (ReflectionUtils.GetObjectType(value) == targetType)
			{
				return value;
			}
			try
			{
				obj = ConvertUtils.ConvertOrCast(value, culture, targetType);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				throw new JsonSerializationException("Error converting value {0} to type '{1}'.".FormatWith(CultureInfo.InvariantCulture, new object[] { this.FormatValueForPrint(value), targetType }), exception);
			}
			return obj;
		}

		private string FormatValueForPrint(object value)
		{
			if (value == null)
			{
				return "{null}";
			}
			if (!(value is string))
			{
				return value.ToString();
			}
			return string.Concat("\"", value, "\"");
		}

		private JsonContract GetContractSafe(Type type)
		{
			if (type == null)
			{
				return null;
			}
			return base.Serializer.ContractResolver.ResolveContract(type);
		}

		private JsonContract GetContractSafe(Type type, object value)
		{
			if (value == null)
			{
				return this.GetContractSafe(type);
			}
			return base.Serializer.ContractResolver.ResolveContract(value.GetType());
		}

		private JsonConverter GetConverter(JsonContract contract, JsonConverter memberConverter)
		{
			JsonConverter internalConverter = null;
			if (memberConverter != null)
			{
				internalConverter = memberConverter;
			}
			else if (contract != null)
			{
				if (contract.Converter == null)
				{
					JsonConverter matchingConverter = base.Serializer.GetMatchingConverter(contract.UnderlyingType);
					JsonConverter jsonConverter = matchingConverter;
					if (matchingConverter != null)
					{
						internalConverter = jsonConverter;
					}
					else if (contract.InternalConverter != null)
					{
						internalConverter = contract.InternalConverter;
					}
				}
				else
				{
					internalConverter = contract.Converter;
				}
			}
			return internalConverter;
		}

		private JsonFormatterConverter GetFormatterConverter()
		{
			if (this._formatterConverter == null)
			{
				this._formatterConverter = new JsonFormatterConverter(this.GetInternalSerializer());
			}
			return this._formatterConverter;
		}

		private JsonSerializerProxy GetInternalSerializer()
		{
			if (this._internalSerializer == null)
			{
				this._internalSerializer = new JsonSerializerProxy(this);
			}
			return this._internalSerializer;
		}

		private void HandleError(JsonReader reader, int initialDepth)
		{
			base.ClearErrorContext();
			reader.Skip();
			while (reader.Depth > initialDepth + 1)
			{
				reader.Read();
			}
		}

		private bool HasDefinedType(Type type)
		{
			return (type == null || type == typeof(object) ? false : !typeof(JToken).IsAssignableFrom(type));
		}

		private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag)
		{
			return (value & flag) == flag;
		}

		public void Populate(JsonReader reader, object target)
		{
			string str;
			ValidationUtils.ArgumentNotNull(target, "target");
			Type type = target.GetType();
			JsonContract jsonContract = base.Serializer.ContractResolver.ResolveContract(type);
			if (reader.TokenType == JsonToken.None)
			{
				reader.Read();
			}
			if (reader.TokenType != JsonToken.StartArray)
			{
				if (reader.TokenType != JsonToken.StartObject)
				{
					throw new JsonSerializationException("Unexpected initial token '{0}' when populating object. Expected JSON object or array.".FormatWith(CultureInfo.InvariantCulture, new object[] { reader.TokenType }));
				}
				this.CheckedRead(reader);
				string str1 = null;
				if (reader.TokenType == JsonToken.PropertyName && string.Equals(reader.Value.ToString(), "$id", StringComparison.Ordinal))
				{
					this.CheckedRead(reader);
					if (reader.Value == null)
					{
						str = null;
					}
					else
					{
						str = reader.Value.ToString();
					}
					str1 = str;
					this.CheckedRead(reader);
				}
				if (!(jsonContract is JsonDictionaryContract))
				{
					if (!(jsonContract is JsonObjectContract))
					{
						throw new JsonSerializationException("Cannot populate JSON object onto type '{0}'.".FormatWith(CultureInfo.InvariantCulture, new object[] { type }));
					}
					this.PopulateObject(target, reader, (JsonObjectContract)jsonContract, str1);
				}
				else
				{
					this.PopulateDictionary(CollectionUtils.CreateDictionaryWrapper(target), reader, (JsonDictionaryContract)jsonContract, str1);
				}
			}
			else
			{
				if (!(jsonContract is JsonArrayContract))
				{
					throw new JsonSerializationException("Cannot populate JSON array onto type '{0}'.".FormatWith(CultureInfo.InvariantCulture, new object[] { type }));
				}
				this.PopulateList(CollectionUtils.CreateCollectionWrapper(target), reader, null, (JsonArrayContract)jsonContract);
			}
		}

		private object PopulateDictionary(IWrappedDictionary dictionary, JsonReader reader, JsonDictionaryContract contract, string id)
		{
			if (id != null)
			{
				base.Serializer.ReferenceResolver.AddReference(this, id, dictionary.UnderlyingDictionary);
			}
			contract.InvokeOnDeserializing(dictionary.UnderlyingDictionary, base.Serializer.Context);
			int depth = reader.Depth;
			do
			{
				JsonToken tokenType = reader.TokenType;
				if (tokenType == JsonToken.PropertyName)
				{
					object value = reader.Value;
					try
					{
						try
						{
							value = this.EnsureType(value, CultureInfo.InvariantCulture, contract.DictionaryKeyType);
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							throw new JsonSerializationException("Could not convert string '{0}' to dictionary key type '{1}'. Create a TypeConverter to convert from the string to the key type object.".FormatWith(CultureInfo.InvariantCulture, new object[] { reader.Value, contract.DictionaryKeyType }), exception);
						}
						if (!this.ReadForType(reader, contract.DictionaryValueType, null))
						{
							throw new JsonSerializationException("Unexpected end when deserializing object.");
						}
						dictionary[value] = this.CreateValueNonProperty(reader, contract.DictionaryValueType, this.GetContractSafe(contract.DictionaryValueType));
					}
					catch (Exception exception2)
					{
						if (!base.IsErrorHandled(dictionary, contract, value, exception2))
						{
							throw;
						}
						else
						{
							this.HandleError(reader, depth);
						}
					}
				}
				else if (tokenType != JsonToken.Comment)
				{
					if (tokenType != JsonToken.EndObject)
					{
						throw new JsonSerializationException(string.Concat("Unexpected token when deserializing object: ", reader.TokenType));
					}
					contract.InvokeOnDeserialized(dictionary.UnderlyingDictionary, base.Serializer.Context);
					return dictionary.UnderlyingDictionary;
				}
			}
			while (reader.Read());
			throw new JsonSerializationException("Unexpected end when deserializing object.");
		}

		private object PopulateList(IWrappedCollection wrappedList, JsonReader reader, string reference, JsonArrayContract contract)
		{
			object underlyingCollection = wrappedList.UnderlyingCollection;
			if (wrappedList.IsFixedSize)
			{
				reader.Skip();
				return wrappedList.UnderlyingCollection;
			}
			if (reference != null)
			{
				base.Serializer.ReferenceResolver.AddReference(this, reference, underlyingCollection);
			}
			contract.InvokeOnDeserializing(underlyingCollection, base.Serializer.Context);
			int depth = reader.Depth;
			while (this.ReadForTypeArrayHack(reader, contract.CollectionItemType))
			{
				JsonToken tokenType = reader.TokenType;
				if (tokenType != JsonToken.Comment)
				{
					if (tokenType == JsonToken.EndArray)
					{
						contract.InvokeOnDeserialized(underlyingCollection, base.Serializer.Context);
						return wrappedList.UnderlyingCollection;
					}
					try
					{
						object obj = this.CreateValueNonProperty(reader, contract.CollectionItemType, this.GetContractSafe(contract.CollectionItemType));
						wrappedList.Add(obj);
					}
					catch (Exception exception)
					{
						if (!base.IsErrorHandled(underlyingCollection, contract, wrappedList.Count, exception))
						{
							throw;
						}
						else
						{
							this.HandleError(reader, depth);
						}
					}
				}
			}
			throw new JsonSerializationException("Unexpected end when deserializing array.");
		}

		private object PopulateMultidimensionalArray(IList list, JsonReader reader, string reference, JsonArrayContract contract)
		{
			JsonToken tokenType;
			int arrayRank = contract.UnderlyingType.GetArrayRank();
			if (reference != null)
			{
				base.Serializer.ReferenceResolver.AddReference(this, reference, list);
			}
			contract.InvokeOnDeserializing(list, base.Serializer.Context);
			Stack<IList> lists = new Stack<IList>();
			lists.Push(list);
			IList lists1 = list;
			bool flag = false;
			do
			{
				int depth = reader.Depth;
				if (lists.Count == arrayRank)
				{
					if (!this.ReadForTypeArrayHack(reader, contract.CollectionItemType))
					{
						break;
					}
					else
					{
						tokenType = reader.TokenType;
						if (tokenType != JsonToken.Comment)
						{
							if (tokenType == JsonToken.EndArray)
							{
								lists.Pop();
								lists1 = lists.Peek();
							}
							else
							{
								try
								{
									object obj = this.CreateValueNonProperty(reader, contract.CollectionItemType, this.GetContractSafe(contract.CollectionItemType));
									lists1.Add(obj);
								}
								catch (Exception exception)
								{
									if (!base.IsErrorHandled(list, contract, lists1.Count, exception))
									{
										throw;
									}
									else
									{
										this.HandleError(reader, depth);
									}
								}
							}
						}
					}
				}
				else if (!reader.Read())
				{
					break;
				}
				else
				{
					tokenType = reader.TokenType;
					switch (tokenType)
					{
						case JsonToken.StartArray:
						{
							IList objs = new List<object>();
							lists1.Add(objs);
							lists.Push(objs);
							lists1 = objs;
							break;
						}
						case JsonToken.Comment:
						{
							break;
						}
						default:
						{
							if (tokenType != JsonToken.EndArray)
							{
								throw new JsonSerializationException(string.Concat("Unexpected token when deserializing multidimensional array: ", reader.TokenType));
							}
							lists.Pop();
							if (lists.Count <= 0)
							{
								flag = true;
							}
							else
							{
								lists1 = lists.Peek();
							}
							break;
						}
					}
				}
			}
			while (!flag);
			if (!flag)
			{
				throw new JsonSerializationException(string.Concat("Unexpected end when deserializing array.", reader.TokenType));
			}
			contract.InvokeOnDeserialized(list, base.Serializer.Context);
			return list;
		}

		private object PopulateObject(object newObject, JsonReader reader, JsonObjectContract contract, string id)
		{
			contract.InvokeOnDeserializing(newObject, base.Serializer.Context);
			Dictionary<JsonProperty, JsonSerializerInternalReader.PropertyPresence> dictionary = contract.Properties.ToDictionary<JsonProperty, JsonProperty, JsonSerializerInternalReader.PropertyPresence>((JsonProperty m) => m, (JsonProperty m) => JsonSerializerInternalReader.PropertyPresence.None);
			if (id != null)
			{
				base.Serializer.ReferenceResolver.AddReference(this, id, newObject);
			}
			int depth = reader.Depth;
			do
			{
				JsonToken tokenType = reader.TokenType;
				if (tokenType == JsonToken.PropertyName)
				{
					string str = reader.Value.ToString();
					try
					{
						JsonProperty closestMatchProperty = contract.Properties.GetClosestMatchProperty(str);
						if (closestMatchProperty != null)
						{
							if (!this.ReadForType(reader, closestMatchProperty.PropertyType, closestMatchProperty.Converter))
							{
								throw new JsonSerializationException("Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, new object[] { str }));
							}
							this.SetPropertyPresence(reader, closestMatchProperty, dictionary);
							this.SetPropertyValue(closestMatchProperty, reader, newObject);
						}
						else
						{
							if (base.Serializer.MissingMemberHandling == MissingMemberHandling.Error)
							{
								throw new JsonSerializationException("Could not find member '{0}' on object of type '{1}'".FormatWith(CultureInfo.InvariantCulture, new object[] { str, contract.UnderlyingType.Name }));
							}
							reader.Skip();
							continue;
						}
					}
					catch (Exception exception)
					{
						if (!base.IsErrorHandled(newObject, contract, str, exception))
						{
							throw;
						}
						else
						{
							this.HandleError(reader, depth);
						}
					}
				}
				else if (tokenType != JsonToken.Comment)
				{
					if (tokenType != JsonToken.EndObject)
					{
						throw new JsonSerializationException(string.Concat("Unexpected token when deserializing object: ", reader.TokenType));
					}
					foreach (KeyValuePair<JsonProperty, JsonSerializerInternalReader.PropertyPresence> keyValuePair in dictionary)
					{
						JsonProperty key = keyValuePair.Key;
						JsonSerializerInternalReader.PropertyPresence value = keyValuePair.Value;
						if (value == JsonSerializerInternalReader.PropertyPresence.None)
						{
							if (key.Required == Required.AllowNull || key.Required == Required.Always)
							{
								throw new JsonSerializationException("Required property '{0}' not found in JSON.".FormatWith(CultureInfo.InvariantCulture, new object[] { key.PropertyName }));
							}
							if (this.HasFlag(key.DefaultValueHandling.GetValueOrDefault(base.Serializer.DefaultValueHandling), DefaultValueHandling.Populate) && key.Writable)
							{
								key.ValueProvider.SetValue(newObject, this.EnsureType(key.DefaultValue, CultureInfo.InvariantCulture, key.PropertyType));
							}
						}
						else if (value == JsonSerializerInternalReader.PropertyPresence.Null)
						{
							if (key.Required == Required.Always)
							{
								throw new JsonSerializationException("Required property '{0}' expects a value but got null.".FormatWith(CultureInfo.InvariantCulture, new object[] { key.PropertyName }));
							}
						}
					}
					contract.InvokeOnDeserialized(newObject, base.Serializer.Context);
					return newObject;
				}
			}
			while (reader.Read());
			throw new JsonSerializationException("Unexpected end when deserializing object.");
		}

		private bool ReadForType(JsonReader reader, Type t, JsonConverter propertyConverter)
		{
			if (this.GetConverter(this.GetContractSafe(t), propertyConverter) != null)
			{
				return reader.Read();
			}
			if (t == typeof(byte[]))
			{
				reader.ReadAsBytes();
				return true;
			}
			if (t == typeof(decimal) || t == typeof(decimal?))
			{
				reader.ReadAsDecimal();
				return true;
			}
			if (t == typeof(DateTimeOffset) || t == typeof(DateTimeOffset?))
			{
				reader.ReadAsDateTimeOffset();
				return true;
			}
			do
			{
				if (reader.Read())
				{
					continue;
				}
				return false;
			}
			while (reader.TokenType == JsonToken.Comment);
			return true;
		}

		private bool ReadForTypeArrayHack(JsonReader reader, Type t)
		{
			bool flag;
			try
			{
				flag = this.ReadForType(reader, t, null);
			}
			catch (JsonReaderException jsonReaderException)
			{
				if (reader.TokenType != JsonToken.EndArray)
				{
					throw;
				}
				else
				{
					flag = true;
				}
			}
			return flag;
		}

		private IDictionary<JsonProperty, object> ResolvePropertyAndConstructorValues(JsonObjectContract contract, JsonReader reader, Type objectType)
		{
			IDictionary<JsonProperty, object> jsonProperties = new Dictionary<JsonProperty, object>();
			bool flag = false;
			do
			{
				JsonToken tokenType = reader.TokenType;
				if (tokenType == JsonToken.PropertyName)
				{
					string str = reader.Value.ToString();
					JsonProperty closestMatchProperty = contract.ConstructorParameters.GetClosestMatchProperty(str) ?? contract.Properties.GetClosestMatchProperty(str);
					if (closestMatchProperty == null)
					{
						if (!reader.Read())
						{
							throw new JsonSerializationException("Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, new object[] { str }));
						}
						if (base.Serializer.MissingMemberHandling == MissingMemberHandling.Error)
						{
							throw new JsonSerializationException("Could not find member '{0}' on object of type '{1}'".FormatWith(CultureInfo.InvariantCulture, new object[] { str, objectType.Name }));
						}
						reader.Skip();
					}
					else
					{
						if (!this.ReadForType(reader, closestMatchProperty.PropertyType, closestMatchProperty.Converter))
						{
							throw new JsonSerializationException("Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, new object[] { str }));
						}
						if (closestMatchProperty.Ignored)
						{
							reader.Skip();
						}
						else
						{
							jsonProperties[closestMatchProperty] = this.CreateValueProperty(reader, closestMatchProperty, null, true, null);
						}
					}
				}
				else if (tokenType != JsonToken.Comment)
				{
					if (tokenType != JsonToken.EndObject)
					{
						throw new JsonSerializationException(string.Concat("Unexpected token when deserializing object: ", reader.TokenType));
					}
					flag = true;
				}
			}
			while (!flag && reader.Read());
			return jsonProperties;
		}

		private void SetPropertyPresence(JsonReader reader, JsonProperty property, Dictionary<JsonProperty, JsonSerializerInternalReader.PropertyPresence> requiredProperties)
		{
			if (property != null)
			{
				requiredProperties[property] = (reader.TokenType == JsonToken.Null || reader.TokenType == JsonToken.Undefined ? JsonSerializerInternalReader.PropertyPresence.Null : JsonSerializerInternalReader.PropertyPresence.Value);
			}
		}

		private void SetPropertyValue(JsonProperty property, JsonReader reader, object target)
		{
			if (property.Ignored)
			{
				reader.Skip();
				return;
			}
			object value = null;
			bool flag = false;
			bool flag1 = false;
			ObjectCreationHandling valueOrDefault = property.ObjectCreationHandling.GetValueOrDefault(base.Serializer.ObjectCreationHandling);
			if ((valueOrDefault == ObjectCreationHandling.Auto || valueOrDefault == ObjectCreationHandling.Reuse) && (reader.TokenType == JsonToken.StartArray || reader.TokenType == JsonToken.StartObject) && property.Readable)
			{
				value = property.ValueProvider.GetValue(target);
				flag1 = true;
				flag = (value == null || property.PropertyType.IsArray || ReflectionUtils.InheritsGenericDefinition(property.PropertyType, typeof(ReadOnlyCollection<>)) ? false : !property.PropertyType.IsValueType);
			}
			if (!property.Writable && !flag)
			{
				reader.Skip();
				return;
			}
			if (property.NullValueHandling.GetValueOrDefault(base.Serializer.NullValueHandling) == NullValueHandling.Ignore && reader.TokenType == JsonToken.Null)
			{
				reader.Skip();
				return;
			}
			if (this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(base.Serializer.DefaultValueHandling), DefaultValueHandling.Ignore) && JsonReader.IsPrimitiveToken(reader.TokenType) && MiscellaneousUtils.ValueEquals(reader.Value, property.DefaultValue))
			{
				reader.Skip();
				return;
			}
			object obj = this.CreateValueProperty(reader, property, target, flag1, (!flag ? null : value));
			if ((!flag || obj != value) && this.ShouldSetPropertyValue(property, obj))
			{
				property.ValueProvider.SetValue(target, obj);
				if (property.SetIsSpecified != null)
				{
					property.SetIsSpecified(target, true);
				}
			}
		}

		private bool ShouldSetPropertyValue(JsonProperty property, object value)
		{
			if (property.NullValueHandling.GetValueOrDefault(base.Serializer.NullValueHandling) == NullValueHandling.Ignore && value == null)
			{
				return false;
			}
			if (this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(base.Serializer.DefaultValueHandling), DefaultValueHandling.Ignore) && MiscellaneousUtils.ValueEquals(value, property.DefaultValue))
			{
				return false;
			}
			if (!property.Writable)
			{
				return false;
			}
			return true;
		}

		internal enum PropertyPresence
		{
			None,
			Null,
			Value
		}
	}
}