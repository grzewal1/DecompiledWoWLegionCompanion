using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using UnityEngine;

namespace Newtonsoft.Json.Serialization
{
	internal class JsonSerializerInternalWriter : JsonSerializerInternalBase
	{
		private JsonSerializerProxy _internalSerializer;

		private List<object> _serializeStack;

		private List<object> SerializeStack
		{
			get
			{
				if (this._serializeStack == null)
				{
					this._serializeStack = new List<object>();
				}
				return this._serializeStack;
			}
		}

		public JsonSerializerInternalWriter(JsonSerializer serializer) : base(serializer)
		{
		}

		private bool CheckForCircularReference(object value, ReferenceLoopHandling? referenceLoopHandling, JsonContract contract)
		{
			if (value == null || contract is JsonPrimitiveContract)
			{
				return true;
			}
			if (this.SerializeStack.IndexOf(value) == -1)
			{
				return true;
			}
			switch ((value is Vector2 || value is Vector3 || value is Vector4 || value is Color || value is Color32 ? ReferenceLoopHandling.Ignore : referenceLoopHandling.GetValueOrDefault(base.Serializer.ReferenceLoopHandling)))
			{
				case ReferenceLoopHandling.Error:
				{
					throw new JsonSerializationException("Self referencing loop detected for type '{0}'.".FormatWith(CultureInfo.InvariantCulture, new object[] { value.GetType() }));
				}
				case ReferenceLoopHandling.Ignore:
				{
					return false;
				}
				case ReferenceLoopHandling.Serialize:
				{
					return true;
				}
			}
			throw new InvalidOperationException("Unexpected ReferenceLoopHandling value: '{0}'".FormatWith(CultureInfo.InvariantCulture, new object[] { base.Serializer.ReferenceLoopHandling }));
		}

		private JsonContract GetContractSafe(object value)
		{
			if (value == null)
			{
				return null;
			}
			return base.Serializer.ContractResolver.ResolveContract(value.GetType());
		}

		private JsonSerializerProxy GetInternalSerializer()
		{
			if (this._internalSerializer == null)
			{
				this._internalSerializer = new JsonSerializerProxy(this);
			}
			return this._internalSerializer;
		}

		private string GetPropertyName(DictionaryEntry entry)
		{
			string str;
			if (entry.Key is IConvertible)
			{
				return Convert.ToString(entry.Key, CultureInfo.InvariantCulture);
			}
			if (JsonSerializerInternalWriter.TryConvertToString(entry.Key, entry.Key.GetType(), out str))
			{
				return str;
			}
			return entry.Key.ToString();
		}

		private string GetReference(JsonWriter writer, object value)
		{
			string reference;
			try
			{
				reference = base.Serializer.ReferenceResolver.GetReference(this, value);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				throw new JsonSerializationException("Error writing object reference for '{0}'.".FormatWith(CultureInfo.InvariantCulture, new object[] { value.GetType() }), exception);
			}
			return reference;
		}

		private void HandleError(JsonWriter writer, int initialDepth)
		{
			base.ClearErrorContext();
			while (writer.Top > initialDepth)
			{
				writer.WriteEnd();
			}
		}

		private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag)
		{
			return (value & flag) == flag;
		}

		private bool HasFlag(PreserveReferencesHandling value, PreserveReferencesHandling flag)
		{
			return (value & flag) == flag;
		}

		private bool HasFlag(TypeNameHandling value, TypeNameHandling flag)
		{
			return (value & flag) == flag;
		}

		private bool IsSpecified(JsonProperty property, object target)
		{
			if (property.GetIsSpecified == null)
			{
				return true;
			}
			return property.GetIsSpecified(target);
		}

		public void Serialize(JsonWriter jsonWriter, object value)
		{
			if (jsonWriter == null)
			{
				throw new ArgumentNullException("jsonWriter");
			}
			this.SerializeValue(jsonWriter, value, this.GetContractSafe(value), null, null);
		}

		private void SerializeConvertable(JsonWriter writer, JsonConverter converter, object value, JsonContract contract)
		{
			if (!this.ShouldWriteReference(value, null, contract))
			{
				if (!this.CheckForCircularReference(value, null, contract))
				{
					return;
				}
				this.SerializeStack.Add(value);
				converter.WriteJson(writer, value, this.GetInternalSerializer());
				this.SerializeStack.RemoveAt(this.SerializeStack.Count - 1);
			}
			else
			{
				this.WriteReference(writer, value);
			}
		}

		private void SerializeDictionary(JsonWriter writer, IWrappedDictionary values, JsonDictionaryContract contract, JsonProperty member, JsonContract collectionValueContract)
		{
			contract.InvokeOnSerializing(values.UnderlyingDictionary, base.Serializer.Context);
			this.SerializeStack.Add(values.UnderlyingDictionary);
			writer.WriteStartObject();
			bool? isReference = contract.IsReference;
			if ((!isReference.HasValue ? this.HasFlag(base.Serializer.PreserveReferencesHandling, PreserveReferencesHandling.Objects) : isReference.Value))
			{
				writer.WritePropertyName("$id");
				writer.WriteValue(base.Serializer.ReferenceResolver.GetReference(this, values.UnderlyingDictionary));
			}
			if (this.ShouldWriteType(TypeNameHandling.Objects, contract, member, collectionValueContract))
			{
				this.WriteTypeProperty(writer, values.UnderlyingDictionary.GetType());
			}
			JsonContract jsonContract = base.Serializer.ContractResolver.ResolveContract(contract.DictionaryValueType ?? typeof(object));
			int top = writer.Top;
			IDictionaryEnumerator enumerator = values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DictionaryEntry current = (DictionaryEntry)enumerator.Current;
					string propertyName = this.GetPropertyName(current);
					propertyName = (contract.PropertyNameResolver == null ? propertyName : contract.PropertyNameResolver(propertyName));
					try
					{
						object value = current.Value;
						JsonContract contractSafe = this.GetContractSafe(value);
						if (this.ShouldWriteReference(value, null, contractSafe))
						{
							writer.WritePropertyName(propertyName);
							this.WriteReference(writer, value);
						}
						else if (this.CheckForCircularReference(value, null, contract))
						{
							writer.WritePropertyName(propertyName);
							this.SerializeValue(writer, value, contractSafe, null, jsonContract);
						}
						else
						{
							continue;
						}
					}
					catch (Exception exception)
					{
						if (!base.IsErrorHandled(values.UnderlyingDictionary, contract, propertyName, exception))
						{
							throw;
						}
						else
						{
							this.HandleError(writer, top);
						}
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				IDisposable disposable1 = disposable;
				if (disposable != null)
				{
					disposable1.Dispose();
				}
			}
			writer.WriteEndObject();
			this.SerializeStack.RemoveAt(this.SerializeStack.Count - 1);
			contract.InvokeOnSerialized(values.UnderlyingDictionary, base.Serializer.Context);
		}

		[SecuritySafeCritical]
		private void SerializeISerializable(JsonWriter writer, ISerializable value, JsonISerializableContract contract)
		{
			contract.InvokeOnSerializing(value, base.Serializer.Context);
			this.SerializeStack.Add(value);
			writer.WriteStartObject();
			SerializationInfo serializationInfo = new SerializationInfo(contract.UnderlyingType, new FormatterConverter());
			value.GetObjectData(serializationInfo, base.Serializer.Context);
			SerializationInfoEnumerator enumerator = serializationInfo.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SerializationEntry current = enumerator.Current;
				writer.WritePropertyName(current.Name);
				this.SerializeValue(writer, current.Value, this.GetContractSafe(current.Value), null, null);
			}
			writer.WriteEndObject();
			this.SerializeStack.RemoveAt(this.SerializeStack.Count - 1);
			contract.InvokeOnSerialized(value, base.Serializer.Context);
		}

		private void SerializeList(JsonWriter writer, IWrappedCollection values, JsonArrayContract contract, JsonProperty member, JsonContract collectionValueContract)
		{
			contract.InvokeOnSerializing(values.UnderlyingCollection, base.Serializer.Context);
			this.SerializeStack.Add(values.UnderlyingCollection);
			bool? isReference = contract.IsReference;
			bool flag = (!isReference.HasValue ? this.HasFlag(base.Serializer.PreserveReferencesHandling, PreserveReferencesHandling.Arrays) : isReference.Value);
			bool flag1 = this.ShouldWriteType(TypeNameHandling.Arrays, contract, member, collectionValueContract);
			if (flag || flag1)
			{
				writer.WriteStartObject();
				if (flag)
				{
					writer.WritePropertyName("$id");
					writer.WriteValue(base.Serializer.ReferenceResolver.GetReference(this, values.UnderlyingCollection));
				}
				if (flag1)
				{
					this.WriteTypeProperty(writer, values.UnderlyingCollection.GetType());
				}
				writer.WritePropertyName("$values");
			}
			JsonContract jsonContract = base.Serializer.ContractResolver.ResolveContract(contract.CollectionItemType ?? typeof(object));
			writer.WriteStartArray();
			int top = writer.Top;
			int num = 0;
			IEnumerator enumerator = values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					try
					{
						try
						{
							JsonContract contractSafe = this.GetContractSafe(current);
							if (this.ShouldWriteReference(current, null, contractSafe))
							{
								this.WriteReference(writer, current);
							}
							else if (this.CheckForCircularReference(current, null, contract))
							{
								this.SerializeValue(writer, current, contractSafe, null, jsonContract);
							}
						}
						catch (Exception exception)
						{
							if (!base.IsErrorHandled(values.UnderlyingCollection, contract, num, exception))
							{
								throw;
							}
							else
							{
								this.HandleError(writer, top);
							}
						}
					}
					finally
					{
						num++;
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				IDisposable disposable1 = disposable;
				if (disposable != null)
				{
					disposable1.Dispose();
				}
			}
			writer.WriteEndArray();
			if (flag || flag1)
			{
				writer.WriteEndObject();
			}
			this.SerializeStack.RemoveAt(this.SerializeStack.Count - 1);
			contract.InvokeOnSerialized(values.UnderlyingCollection, base.Serializer.Context);
		}

		private void SerializeMultidimensionalArray(JsonWriter writer, Array values, JsonArrayContract contract, JsonProperty member, JsonContract collectionContract)
		{
			contract.InvokeOnSerializing(values, base.Serializer.Context);
			this._serializeStack.Add(values);
			bool flag = this.WriteStartArray(writer, values, contract, member, collectionContract);
			this.SerializeMultidimensionalArray(writer, values, contract, member, writer.Top, new int[0]);
			if (flag)
			{
				writer.WriteEndObject();
			}
			this._serializeStack.RemoveAt(this._serializeStack.Count - 1);
			contract.InvokeOnSerialized(values, base.Serializer.Context);
		}

		private void SerializeMultidimensionalArray(JsonWriter writer, Array values, JsonArrayContract contract, JsonProperty member, int initialDepth, int[] indices)
		{
			int length = (int)indices.Length;
			int[] numArray = new int[length + 1];
			for (int i = 0; i < length; i++)
			{
				numArray[i] = indices[i];
			}
			writer.WriteStartArray();
			for (int j = 0; j < values.GetLength(length); j++)
			{
				numArray[length] = j;
				if ((int)numArray.Length != values.Rank)
				{
					this.SerializeMultidimensionalArray(writer, values, contract, member, initialDepth + 1, numArray);
				}
				else
				{
					object value = values.GetValue(numArray);
					try
					{
						JsonContract contractSafe = this.GetContractSafe(value);
						if (this.ShouldWriteReference(value, member, contractSafe))
						{
							this.WriteReference(writer, value);
						}
						else if (this.CheckForCircularReference(value, null, contractSafe))
						{
							this.SerializeValue(writer, value, contractSafe, member, contract);
						}
					}
					catch (Exception exception)
					{
						if (!base.IsErrorHandled(values, contract, j, exception))
						{
							throw;
						}
						else
						{
							this.HandleError(writer, initialDepth + 1);
						}
					}
				}
			}
			writer.WriteEndArray();
		}

		private void SerializeObject(JsonWriter writer, object value, JsonObjectContract contract, JsonProperty member, JsonContract collectionValueContract)
		{
			contract.InvokeOnSerializing(value, base.Serializer.Context);
			this.SerializeStack.Add(value);
			writer.WriteStartObject();
			bool? isReference = contract.IsReference;
			if ((!isReference.HasValue ? this.HasFlag(base.Serializer.PreserveReferencesHandling, PreserveReferencesHandling.Objects) : isReference.Value))
			{
				writer.WritePropertyName("$id");
				writer.WriteValue(base.Serializer.ReferenceResolver.GetReference(this, value));
			}
			if (this.ShouldWriteType(TypeNameHandling.Objects, contract, member, collectionValueContract))
			{
				this.WriteTypeProperty(writer, contract.UnderlyingType);
			}
			int top = writer.Top;
			foreach (JsonProperty property in contract.Properties)
			{
				try
				{
					if (!property.Ignored && property.Readable && this.ShouldSerialize(property, value) && this.IsSpecified(property, value))
					{
						object obj = property.ValueProvider.GetValue(value);
						this.WriteMemberInfoProperty(writer, obj, property, this.GetContractSafe(obj));
					}
				}
				catch (Exception exception)
				{
					if (!base.IsErrorHandled(value, contract, property.PropertyName, exception))
					{
						throw;
					}
					else
					{
						this.HandleError(writer, top);
					}
				}
			}
			writer.WriteEndObject();
			this.SerializeStack.RemoveAt(this.SerializeStack.Count - 1);
			contract.InvokeOnSerialized(value, base.Serializer.Context);
		}

		private void SerializePrimitive(JsonWriter writer, object value, JsonPrimitiveContract contract, JsonProperty member, JsonContract collectionValueContract)
		{
			if (contract.UnderlyingType != typeof(byte[]) || !this.ShouldWriteType(TypeNameHandling.Objects, contract, member, collectionValueContract))
			{
				writer.WriteValue(value);
				return;
			}
			writer.WriteStartObject();
			this.WriteTypeProperty(writer, contract.CreatedType);
			writer.WritePropertyName("$value");
			writer.WriteValue(value);
			writer.WriteEndObject();
		}

		private void SerializeString(JsonWriter writer, object value, JsonStringContract contract)
		{
			string str;
			contract.InvokeOnSerializing(value, base.Serializer.Context);
			JsonSerializerInternalWriter.TryConvertToString(value, contract.UnderlyingType, out str);
			writer.WriteValue(str);
			contract.InvokeOnSerialized(value, base.Serializer.Context);
		}

		private void SerializeValue(JsonWriter writer, object value, JsonContract valueContract, JsonProperty member, JsonContract collectionValueContract)
		{
			JsonConverter converter;
			JsonConverter[] array;
			if (member == null)
			{
				converter = null;
			}
			else
			{
				converter = member.Converter;
			}
			JsonConverter jsonConverter = converter;
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			if (jsonConverter == null)
			{
				JsonConverter converter1 = valueContract.Converter;
				jsonConverter = converter1;
				if (converter1 == null)
				{
					JsonConverter matchingConverter = base.Serializer.GetMatchingConverter(valueContract.UnderlyingType);
					jsonConverter = matchingConverter;
					if (matchingConverter == null)
					{
						JsonConverter internalConverter = valueContract.InternalConverter;
						jsonConverter = internalConverter;
						if (internalConverter == null)
						{
							goto Label0;
						}
					}
				}
			}
			if (jsonConverter.CanWrite)
			{
				this.SerializeConvertable(writer, jsonConverter, value, valueContract);
				return;
			}
			if (valueContract is JsonPrimitiveContract)
			{
				this.SerializePrimitive(writer, value, (JsonPrimitiveContract)valueContract, member, collectionValueContract);
			}
			else if (valueContract is JsonStringContract)
			{
				this.SerializeString(writer, value, (JsonStringContract)valueContract);
			}
			else if (valueContract is JsonObjectContract)
			{
				this.SerializeObject(writer, value, (JsonObjectContract)valueContract, member, collectionValueContract);
			}
			else if (valueContract is JsonDictionaryContract)
			{
				JsonDictionaryContract jsonDictionaryContract = (JsonDictionaryContract)valueContract;
				this.SerializeDictionary(writer, jsonDictionaryContract.CreateWrapper(value), jsonDictionaryContract, member, collectionValueContract);
			}
			else if (valueContract is JsonArrayContract)
			{
				JsonArrayContract jsonArrayContract = (JsonArrayContract)valueContract;
				if (jsonArrayContract.IsMultidimensionalArray)
				{
					this.SerializeMultidimensionalArray(writer, (Array)value, jsonArrayContract, member, collectionValueContract);
				}
				else
				{
					this.SerializeList(writer, jsonArrayContract.CreateWrapper(value), jsonArrayContract, member, collectionValueContract);
				}
			}
			else if (valueContract is JsonLinqContract)
			{
				JToken jTokens = (JToken)value;
				JsonWriter jsonWriter = writer;
				if (base.Serializer.Converters == null)
				{
					array = null;
				}
				else
				{
					array = base.Serializer.Converters.ToArray<JsonConverter>();
				}
				jTokens.WriteTo(jsonWriter, array);
			}
			else if (valueContract is JsonISerializableContract)
			{
				this.SerializeISerializable(writer, (ISerializable)value, (JsonISerializableContract)valueContract);
			}
		}

		private bool ShouldSerialize(JsonProperty property, object target)
		{
			if (property.ShouldSerialize == null)
			{
				return true;
			}
			return property.ShouldSerialize(target);
		}

		private bool ShouldWriteReference(object value, JsonProperty property, JsonContract contract)
		{
			if (value == null)
			{
				return false;
			}
			if (contract is JsonPrimitiveContract)
			{
				return false;
			}
			bool? isReference = null;
			if (property != null)
			{
				isReference = property.IsReference;
			}
			if (!isReference.HasValue)
			{
				isReference = contract.IsReference;
			}
			if (!isReference.HasValue)
			{
				isReference = (!(contract is JsonArrayContract) ? new bool?(this.HasFlag(base.Serializer.PreserveReferencesHandling, PreserveReferencesHandling.Objects)) : new bool?(this.HasFlag(base.Serializer.PreserveReferencesHandling, PreserveReferencesHandling.Arrays)));
			}
			if (!isReference.Value)
			{
				return false;
			}
			return base.Serializer.ReferenceResolver.IsReferenced(this, value);
		}

		private bool ShouldWriteType(TypeNameHandling typeNameHandlingFlag, JsonContract contract, JsonProperty member, JsonContract collectionValueContract)
		{
			TypeNameHandling? nullable;
			TypeNameHandling? typeNameHandling;
			if (member == null)
			{
				nullable = null;
				typeNameHandling = nullable;
			}
			else
			{
				typeNameHandling = member.TypeNameHandling;
			}
			nullable = typeNameHandling;
			if (this.HasFlag((!nullable.HasValue ? base.Serializer.TypeNameHandling : nullable.Value), typeNameHandlingFlag))
			{
				return true;
			}
			if (member != null)
			{
				TypeNameHandling? typeNameHandling1 = member.TypeNameHandling;
				if ((int)((!typeNameHandling1.HasValue ? base.Serializer.TypeNameHandling : typeNameHandling1.Value)) == 4 && contract.UnderlyingType != member.PropertyType)
				{
					JsonContract jsonContract = base.Serializer.ContractResolver.ResolveContract(member.PropertyType);
					if (contract.UnderlyingType != jsonContract.CreatedType)
					{
						return true;
					}
				}
			}
			else if (collectionValueContract != null && base.Serializer.TypeNameHandling == TypeNameHandling.Auto && contract.UnderlyingType != collectionValueContract.UnderlyingType)
			{
				return true;
			}
			return false;
		}

		internal static bool TryConvertToString(object value, Type type, out string s)
		{
			TypeConverter converter = ConvertUtils.GetConverter(type);
			if (converter != null && !(converter is ComponentConverter) && converter.GetType() != typeof(TypeConverter) && converter.CanConvertTo(typeof(string)))
			{
				s = converter.ConvertToInvariantString(value);
				return true;
			}
			if (!(value is Type))
			{
				s = null;
				return false;
			}
			s = ((Type)value).AssemblyQualifiedName;
			return true;
		}

		private void WriteMemberInfoProperty(JsonWriter writer, object memberValue, JsonProperty property, JsonContract contract)
		{
			string propertyName = property.PropertyName;
			object defaultValue = property.DefaultValue;
			if (property.NullValueHandling.GetValueOrDefault(base.Serializer.NullValueHandling) == NullValueHandling.Ignore && memberValue == null)
			{
				return;
			}
			if (this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(base.Serializer.DefaultValueHandling), DefaultValueHandling.Ignore) && MiscellaneousUtils.ValueEquals(memberValue, defaultValue))
			{
				return;
			}
			if (this.ShouldWriteReference(memberValue, property, contract))
			{
				writer.WritePropertyName(propertyName);
				this.WriteReference(writer, memberValue);
				return;
			}
			if (!this.CheckForCircularReference(memberValue, property.ReferenceLoopHandling, contract))
			{
				return;
			}
			if (memberValue == null && property.Required == Required.Always)
			{
				throw new JsonSerializationException("Cannot write a null value for property '{0}'. Property requires a value.".FormatWith(CultureInfo.InvariantCulture, new object[] { property.PropertyName }));
			}
			writer.WritePropertyName(propertyName);
			this.SerializeValue(writer, memberValue, contract, property, null);
		}

		private void WriteReference(JsonWriter writer, object value)
		{
			writer.WriteStartObject();
			writer.WritePropertyName("$ref");
			writer.WriteValue(base.Serializer.ReferenceResolver.GetReference(this, value));
			writer.WriteEndObject();
		}

		private bool WriteStartArray(JsonWriter writer, object values, JsonArrayContract contract, JsonProperty member, JsonContract containerContract)
		{
			bool? isReference = contract.IsReference;
			bool flag = (!isReference.HasValue ? this.HasFlag(base.Serializer.PreserveReferencesHandling, PreserveReferencesHandling.Arrays) : isReference.Value);
			bool flag1 = this.ShouldWriteType(TypeNameHandling.Arrays, contract, member, containerContract);
			bool flag2 = (flag ? true : flag1);
			if (flag2)
			{
				writer.WriteStartObject();
				if (flag)
				{
					writer.WritePropertyName("$id");
					writer.WriteValue(this.GetReference(writer, values));
				}
				if (flag1)
				{
					this.WriteTypeProperty(writer, values.GetType());
				}
				writer.WritePropertyName("$values");
			}
			return flag2;
		}

		private void WriteTypeProperty(JsonWriter writer, Type type)
		{
			writer.WritePropertyName("$type");
			writer.WriteValue(ReflectionUtils.GetTypeName(type, base.Serializer.TypeNameAssemblyFormat, base.Serializer.Binder));
		}
	}
}