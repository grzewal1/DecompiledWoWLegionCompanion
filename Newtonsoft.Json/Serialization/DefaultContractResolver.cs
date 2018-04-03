using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;

namespace Newtonsoft.Json.Serialization
{
	public class DefaultContractResolver : IContractResolver
	{
		private readonly static IContractResolver _instance;

		private readonly static IList<JsonConverter> BuiltInConverters;

		private static Dictionary<ResolverContractKey, JsonContract> _sharedContractCache;

		private readonly static object _typeContractCacheLock;

		private Dictionary<ResolverContractKey, JsonContract> _instanceContractCache;

		private readonly bool _sharedCache;

		public BindingFlags DefaultMembersSearchFlags
		{
			get;
			set;
		}

		public bool DynamicCodeGeneration
		{
			get
			{
				return JsonTypeReflector.DynamicCodeGeneration;
			}
		}

		internal static IContractResolver Instance
		{
			get
			{
				return DefaultContractResolver._instance;
			}
		}

		public bool SerializeCompilerGeneratedMembers
		{
			get;
			set;
		}

		static DefaultContractResolver()
		{
			DefaultContractResolver._instance = new DefaultContractResolver(true);
			DefaultContractResolver.BuiltInConverters = new List<JsonConverter>()
			{
				new KeyValuePairConverter(),
				new BsonObjectIdConverter()
			};
			DefaultContractResolver._typeContractCacheLock = new object();
		}

		public DefaultContractResolver() : this(false)
		{
		}

		public DefaultContractResolver(bool shareCache)
		{
			this.DefaultMembersSearchFlags = BindingFlags.Instance | BindingFlags.Public;
			this._sharedCache = shareCache;
		}

		internal static bool CanConvertToString(Type type)
		{
			TypeConverter converter = ConvertUtils.GetConverter(type);
			if (converter != null && !(converter is ComponentConverter) && !(converter is ReferenceConverter) && converter.GetType() != typeof(TypeConverter) && converter.CanConvertTo(typeof(string)))
			{
				return true;
			}
			if (type != typeof(Type) && !type.IsSubclassOf(typeof(Type)))
			{
				return false;
			}
			return true;
		}

		protected virtual JsonArrayContract CreateArrayContract(Type objectType)
		{
			JsonArrayContract jsonArrayContract = new JsonArrayContract(objectType);
			this.InitializeContract(jsonArrayContract);
			return jsonArrayContract;
		}

		protected virtual IList<JsonProperty> CreateConstructorParameters(ConstructorInfo constructor, JsonPropertyCollection memberProperties)
		{
			ParameterInfo[] parameters = constructor.GetParameters();
			JsonPropertyCollection jsonPropertyCollection = new JsonPropertyCollection(constructor.DeclaringType);
			ParameterInfo[] parameterInfoArray = parameters;
			for (int i = 0; i < (int)parameterInfoArray.Length; i++)
			{
				ParameterInfo parameterInfo = parameterInfoArray[i];
				JsonProperty closestMatchProperty = memberProperties.GetClosestMatchProperty(parameterInfo.Name);
				if (closestMatchProperty != null && closestMatchProperty.PropertyType != parameterInfo.ParameterType)
				{
					closestMatchProperty = null;
				}
				JsonProperty jsonProperty = this.CreatePropertyFromConstructorParameter(closestMatchProperty, parameterInfo);
				if (jsonProperty != null)
				{
					jsonPropertyCollection.AddProperty(jsonProperty);
				}
			}
			return jsonPropertyCollection;
		}

		protected virtual JsonContract CreateContract(Type objectType)
		{
			Type type = ReflectionUtils.EnsureNotNullableType(objectType);
			if (JsonConvert.IsJsonPrimitiveType(type))
			{
				return this.CreatePrimitiveContract(type);
			}
			if (JsonTypeReflector.GetJsonObjectAttribute(type) != null)
			{
				return this.CreateObjectContract(type);
			}
			if (JsonTypeReflector.GetJsonArrayAttribute(type) != null)
			{
				return this.CreateArrayContract(type);
			}
			if (type == typeof(JToken) || type.IsSubclassOf(typeof(JToken)))
			{
				return this.CreateLinqContract(type);
			}
			if (CollectionUtils.IsDictionaryType(type))
			{
				return this.CreateDictionaryContract(type);
			}
			if (typeof(IEnumerable).IsAssignableFrom(type))
			{
				return this.CreateArrayContract(type);
			}
			if (DefaultContractResolver.CanConvertToString(type))
			{
				return this.CreateStringContract(type);
			}
			if (typeof(ISerializable).IsAssignableFrom(type))
			{
				return this.CreateISerializableContract(type);
			}
			return this.CreateObjectContract(type);
		}

		protected virtual JsonDictionaryContract CreateDictionaryContract(Type objectType)
		{
			JsonDictionaryContract jsonDictionaryContract = new JsonDictionaryContract(objectType);
			this.InitializeContract(jsonDictionaryContract);
			DefaultContractResolver defaultContractResolver = this;
			jsonDictionaryContract.PropertyNameResolver = new Func<string, string>(defaultContractResolver.ResolvePropertyName);
			return jsonDictionaryContract;
		}

		protected virtual JsonISerializableContract CreateISerializableContract(Type objectType)
		{
			JsonISerializableContract jsonISerializableContract = new JsonISerializableContract(objectType);
			this.InitializeContract(jsonISerializableContract);
			ConstructorInfo constructor = objectType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(SerializationInfo), typeof(StreamingContext) }, null);
			if (constructor != null)
			{
				MethodCall<object, object> methodCall = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(constructor);
				jsonISerializableContract.ISerializableCreator = (object[] args) => methodCall(null, args);
			}
			return jsonISerializableContract;
		}

		protected virtual JsonLinqContract CreateLinqContract(Type objectType)
		{
			JsonLinqContract jsonLinqContract = new JsonLinqContract(objectType);
			this.InitializeContract(jsonLinqContract);
			return jsonLinqContract;
		}

		protected virtual IValueProvider CreateMemberValueProvider(MemberInfo member)
		{
			return new ReflectionValueProvider(member);
		}

		protected virtual JsonObjectContract CreateObjectContract(Type objectType)
		{
			JsonObjectContract jsonObjectContract = new JsonObjectContract(objectType);
			this.InitializeContract(jsonObjectContract);
			jsonObjectContract.MemberSerialization = JsonTypeReflector.GetObjectMemberSerialization(objectType);
			jsonObjectContract.Properties.AddRange<JsonProperty>(this.CreateProperties(jsonObjectContract.UnderlyingType, jsonObjectContract.MemberSerialization));
			if (((IEnumerable<ConstructorInfo>)objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)).Any<ConstructorInfo>((ConstructorInfo c) => c.IsDefined(typeof(JsonConstructorAttribute), true)))
			{
				ConstructorInfo attributeConstructor = this.GetAttributeConstructor(objectType);
				if (attributeConstructor != null)
				{
					jsonObjectContract.OverrideConstructor = attributeConstructor;
					jsonObjectContract.ConstructorParameters.AddRange<JsonProperty>(this.CreateConstructorParameters(attributeConstructor, jsonObjectContract.Properties));
				}
			}
			else if (jsonObjectContract.DefaultCreator == null || jsonObjectContract.DefaultCreatorNonPublic)
			{
				ConstructorInfo parametrizedConstructor = this.GetParametrizedConstructor(objectType);
				if (parametrizedConstructor != null)
				{
					jsonObjectContract.ParametrizedConstructor = parametrizedConstructor;
					jsonObjectContract.ConstructorParameters.AddRange<JsonProperty>(this.CreateConstructorParameters(parametrizedConstructor, jsonObjectContract.Properties));
				}
			}
			return jsonObjectContract;
		}

		protected virtual JsonPrimitiveContract CreatePrimitiveContract(Type objectType)
		{
			JsonPrimitiveContract jsonPrimitiveContract = new JsonPrimitiveContract(objectType);
			this.InitializeContract(jsonPrimitiveContract);
			return jsonPrimitiveContract;
		}

		protected virtual IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			List<MemberInfo> serializableMembers = this.GetSerializableMembers(type);
			if (serializableMembers == null)
			{
				throw new JsonSerializationException("Null collection of seralizable members returned.");
			}
			JsonPropertyCollection jsonPropertyCollection = new JsonPropertyCollection(type);
			foreach (MemberInfo serializableMember in serializableMembers)
			{
				JsonProperty jsonProperty = this.CreateProperty(serializableMember, memberSerialization);
				if (jsonProperty == null)
				{
					continue;
				}
				jsonPropertyCollection.AddProperty(jsonProperty);
			}
			return jsonPropertyCollection.OrderBy<JsonProperty, int>((JsonProperty p) => {
				int? order = p.Order;
				return (!order.HasValue ? -1 : order.Value);
			}).ToList<JsonProperty>();
		}

		protected virtual JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			bool flag;
			bool flag1;
			JsonProperty jsonProperty = new JsonProperty()
			{
				PropertyType = ReflectionUtils.GetMemberUnderlyingType(member),
				ValueProvider = this.CreateMemberValueProvider(member)
			};
			this.SetPropertySettingsFromAttributes(jsonProperty, member, member.Name, member.DeclaringType, memberSerialization, out flag, out flag1);
			jsonProperty.Readable = ReflectionUtils.CanReadMemberValue(member, flag);
			jsonProperty.Writable = ReflectionUtils.CanSetMemberValue(member, flag, flag1);
			jsonProperty.ShouldSerialize = this.CreateShouldSerializeTest(member);
			this.SetIsSpecifiedActions(jsonProperty, member, flag);
			return jsonProperty;
		}

		protected virtual JsonProperty CreatePropertyFromConstructorParameter(JsonProperty matchingMemberProperty, ParameterInfo parameterInfo)
		{
			bool flag;
			bool flag1;
			JsonProperty jsonProperty = new JsonProperty()
			{
				PropertyType = parameterInfo.ParameterType
			};
			this.SetPropertySettingsFromAttributes(jsonProperty, parameterInfo, parameterInfo.Name, parameterInfo.Member.DeclaringType, MemberSerialization.OptOut, out flag, out flag1);
			jsonProperty.Readable = false;
			jsonProperty.Writable = true;
			if (matchingMemberProperty != null)
			{
				jsonProperty.PropertyName = (jsonProperty.PropertyName == parameterInfo.Name ? matchingMemberProperty.PropertyName : jsonProperty.PropertyName);
				jsonProperty.Converter = jsonProperty.Converter ?? matchingMemberProperty.Converter;
				jsonProperty.MemberConverter = jsonProperty.MemberConverter ?? matchingMemberProperty.MemberConverter;
				jsonProperty.DefaultValue = jsonProperty.DefaultValue ?? matchingMemberProperty.DefaultValue;
				jsonProperty.Required = (jsonProperty.Required == Required.Default ? matchingMemberProperty.Required : jsonProperty.Required);
				JsonProperty jsonProperty1 = jsonProperty;
				bool? isReference = jsonProperty.IsReference;
				jsonProperty1.IsReference = (!isReference.HasValue ? matchingMemberProperty.IsReference : isReference);
				JsonProperty jsonProperty2 = jsonProperty;
				NullValueHandling? nullValueHandling = jsonProperty.NullValueHandling;
				jsonProperty2.NullValueHandling = (!nullValueHandling.HasValue ? matchingMemberProperty.NullValueHandling : nullValueHandling);
				JsonProperty jsonProperty3 = jsonProperty;
				DefaultValueHandling? defaultValueHandling = jsonProperty.DefaultValueHandling;
				jsonProperty3.DefaultValueHandling = (!defaultValueHandling.HasValue ? matchingMemberProperty.DefaultValueHandling : defaultValueHandling);
				JsonProperty jsonProperty4 = jsonProperty;
				ReferenceLoopHandling? referenceLoopHandling = jsonProperty.ReferenceLoopHandling;
				jsonProperty4.ReferenceLoopHandling = (!referenceLoopHandling.HasValue ? matchingMemberProperty.ReferenceLoopHandling : referenceLoopHandling);
				JsonProperty jsonProperty5 = jsonProperty;
				ObjectCreationHandling? objectCreationHandling = jsonProperty.ObjectCreationHandling;
				jsonProperty5.ObjectCreationHandling = (!objectCreationHandling.HasValue ? matchingMemberProperty.ObjectCreationHandling : objectCreationHandling);
				JsonProperty jsonProperty6 = jsonProperty;
				TypeNameHandling? typeNameHandling = jsonProperty.TypeNameHandling;
				jsonProperty6.TypeNameHandling = (!typeNameHandling.HasValue ? matchingMemberProperty.TypeNameHandling : typeNameHandling);
			}
			return jsonProperty;
		}

		private Predicate<object> CreateShouldSerializeTest(MemberInfo member)
		{
			MethodInfo method = member.DeclaringType.GetMethod(string.Concat("ShouldSerialize", member.Name), new Type[0]);
			if (method == null || method.ReturnType != typeof(bool))
			{
				return null;
			}
			MethodCall<object, object> methodCall = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method);
			return (object o) => (bool)methodCall(o, new object[0]);
		}

		protected virtual JsonStringContract CreateStringContract(Type objectType)
		{
			JsonStringContract jsonStringContract = new JsonStringContract(objectType);
			this.InitializeContract(jsonStringContract);
			return jsonStringContract;
		}

		private ConstructorInfo GetAttributeConstructor(Type objectType)
		{
			IList<ConstructorInfo> list = (
				from c in (IEnumerable<ConstructorInfo>)objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				where c.IsDefined(typeof(JsonConstructorAttribute), true)
				select c).ToList<ConstructorInfo>();
			if (list.Count > 1)
			{
				throw new Exception("Multiple constructors with the JsonConstructorAttribute.");
			}
			if (list.Count != 1)
			{
				return null;
			}
			return list[0];
		}

		private Dictionary<ResolverContractKey, JsonContract> GetCache()
		{
			if (this._sharedCache)
			{
				return DefaultContractResolver._sharedContractCache;
			}
			return this._instanceContractCache;
		}

		private void GetCallbackMethodsForType(Type type, out MethodInfo onSerializing, out MethodInfo onSerialized, out MethodInfo onDeserializing, out MethodInfo onDeserialized, out MethodInfo onError)
		{
			onSerializing = null;
			onSerialized = null;
			onDeserializing = null;
			onDeserialized = null;
			onError = null;
			MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < (int)methods.Length; i++)
			{
				MethodInfo methodInfo = methods[i];
				if (!methodInfo.ContainsGenericParameters)
				{
					Type type1 = null;
					ParameterInfo[] parameters = methodInfo.GetParameters();
					if (DefaultContractResolver.IsValidCallback(methodInfo, parameters, typeof(OnSerializingAttribute), onSerializing, ref type1))
					{
						onSerializing = methodInfo;
					}
					if (DefaultContractResolver.IsValidCallback(methodInfo, parameters, typeof(OnSerializedAttribute), onSerialized, ref type1))
					{
						onSerialized = methodInfo;
					}
					if (DefaultContractResolver.IsValidCallback(methodInfo, parameters, typeof(OnDeserializingAttribute), onDeserializing, ref type1))
					{
						onDeserializing = methodInfo;
					}
					if (DefaultContractResolver.IsValidCallback(methodInfo, parameters, typeof(OnDeserializedAttribute), onDeserialized, ref type1))
					{
						onDeserialized = methodInfo;
					}
					if (DefaultContractResolver.IsValidCallback(methodInfo, parameters, typeof(OnErrorAttribute), onError, ref type1))
					{
						onError = methodInfo;
					}
				}
			}
		}

		internal static string GetClrTypeFullName(Type type)
		{
			if (type.IsGenericTypeDefinition || !type.ContainsGenericParameters)
			{
				return type.FullName;
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[] { type.Namespace, type.Name });
		}

		private Func<object> GetDefaultCreator(Type createdType)
		{
			return JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(createdType);
		}

		private ConstructorInfo GetParametrizedConstructor(Type objectType)
		{
			IList<ConstructorInfo> constructors = objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
			if (constructors.Count != 1)
			{
				return null;
			}
			return constructors[0];
		}

		protected virtual List<MemberInfo> GetSerializableMembers(Type objectType)
		{
			Type type;
			DataContractAttribute dataContractAttribute = JsonTypeReflector.GetDataContractAttribute(objectType);
			List<MemberInfo> list = (
				from m in ReflectionUtils.GetFieldsAndProperties(objectType, this.DefaultMembersSearchFlags)
				where !ReflectionUtils.IsIndexedProperty(m)
				select m).ToList<MemberInfo>();
			List<MemberInfo> memberInfos = (
				from m in ReflectionUtils.GetFieldsAndProperties(objectType, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				where !ReflectionUtils.IsIndexedProperty(m)
				select m).ToList<MemberInfo>();
			List<MemberInfo> memberInfos1 = new List<MemberInfo>();
			foreach (MemberInfo memberInfo in memberInfos)
			{
				if (!this.SerializeCompilerGeneratedMembers && memberInfo.IsDefined(typeof(CompilerGeneratedAttribute), true))
				{
					continue;
				}
				if (list.Contains(memberInfo))
				{
					memberInfos1.Add(memberInfo);
				}
				else if (JsonTypeReflector.GetAttribute<JsonPropertyAttribute>((ICustomAttributeProvider)memberInfo) == null)
				{
					if (dataContractAttribute == null || JsonTypeReflector.GetAttribute<DataMemberAttribute>((ICustomAttributeProvider)memberInfo) == null)
					{
						continue;
					}
					memberInfos1.Add(memberInfo);
				}
				else
				{
					memberInfos1.Add(memberInfo);
				}
			}
			if (objectType.AssignableToTypeName("System.Data.Objects.DataClasses.EntityObject", out type))
			{
				memberInfos1 = memberInfos1.Where<MemberInfo>(new Func<MemberInfo, bool>(this.ShouldSerializeEntityMember)).ToList<MemberInfo>();
			}
			return memberInfos1;
		}

		private void InitializeContract(JsonContract contract)
		{
			JsonContainerAttribute jsonContainerAttribute = JsonTypeReflector.GetJsonContainerAttribute(contract.UnderlyingType);
			if (jsonContainerAttribute == null)
			{
				DataContractAttribute dataContractAttribute = JsonTypeReflector.GetDataContractAttribute(contract.UnderlyingType);
				if (dataContractAttribute != null && dataContractAttribute.IsReference)
				{
					contract.IsReference = new bool?(true);
				}
			}
			else
			{
				contract.IsReference = jsonContainerAttribute._isReference;
			}
			contract.Converter = this.ResolveContractConverter(contract.UnderlyingType);
			contract.InternalConverter = JsonSerializer.GetMatchingConverter(DefaultContractResolver.BuiltInConverters, contract.UnderlyingType);
			if (ReflectionUtils.HasDefaultConstructor(contract.CreatedType, true) || contract.CreatedType.IsValueType)
			{
				contract.DefaultCreator = this.GetDefaultCreator(contract.CreatedType);
				contract.DefaultCreatorNonPublic = (contract.CreatedType.IsValueType ? false : ReflectionUtils.GetDefaultConstructor(contract.CreatedType) == null);
			}
			this.ResolveCallbackMethods(contract, contract.UnderlyingType);
		}

		private static bool IsValidCallback(MethodInfo method, ParameterInfo[] parameters, Type attributeType, MethodInfo currentCallback, ref Type prevAttributeType)
		{
			if (!method.IsDefined(attributeType, false))
			{
				return false;
			}
			if (currentCallback != null)
			{
				throw new Exception("Invalid attribute. Both '{0}' and '{1}' in type '{2}' have '{3}'.".FormatWith(CultureInfo.InvariantCulture, new object[] { method, currentCallback, DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), attributeType }));
			}
			if (prevAttributeType != null)
			{
				throw new Exception("Invalid Callback. Method '{3}' in type '{2}' has both '{0}' and '{1}'.".FormatWith(CultureInfo.InvariantCulture, new object[] { prevAttributeType, attributeType, DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), method }));
			}
			if (method.IsVirtual)
			{
				throw new Exception("Virtual Method '{0}' of type '{1}' cannot be marked with '{2}' attribute.".FormatWith(CultureInfo.InvariantCulture, new object[] { method, DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), attributeType }));
			}
			if (method.ReturnType != typeof(void))
			{
				throw new Exception("Serialization Callback '{1}' in type '{0}' must return void.".FormatWith(CultureInfo.InvariantCulture, new object[] { DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), method }));
			}
			if (attributeType == typeof(OnErrorAttribute))
			{
				if (parameters == null || (int)parameters.Length != 2 || parameters[0].ParameterType != typeof(StreamingContext) || parameters[1].ParameterType != typeof(ErrorContext))
				{
					throw new Exception("Serialization Error Callback '{1}' in type '{0}' must have two parameters of type '{2}' and '{3}'.".FormatWith(CultureInfo.InvariantCulture, new object[] { DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), method, typeof(StreamingContext), typeof(ErrorContext) }));
				}
			}
			else if (parameters == null || (int)parameters.Length != 1 || parameters[0].ParameterType != typeof(StreamingContext))
			{
				throw new Exception("Serialization Callback '{1}' in type '{0}' must have a single parameter of type '{2}'.".FormatWith(CultureInfo.InvariantCulture, new object[] { DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), method, typeof(StreamingContext) }));
			}
			prevAttributeType = attributeType;
			return true;
		}

		private void ResolveCallbackMethods(JsonContract contract, Type t)
		{
			MethodInfo methodInfo;
			MethodInfo methodInfo1;
			MethodInfo methodInfo2;
			MethodInfo methodInfo3;
			MethodInfo methodInfo4;
			if (t.BaseType != null)
			{
				this.ResolveCallbackMethods(contract, t.BaseType);
			}
			this.GetCallbackMethodsForType(t, out methodInfo, out methodInfo1, out methodInfo2, out methodInfo3, out methodInfo4);
			if (methodInfo != null)
			{
				contract.OnSerializing = methodInfo;
			}
			if (methodInfo1 != null)
			{
				contract.OnSerialized = methodInfo1;
			}
			if (methodInfo2 != null)
			{
				contract.OnDeserializing = methodInfo2;
			}
			if (methodInfo3 != null)
			{
				contract.OnDeserialized = methodInfo3;
			}
			if (methodInfo4 != null)
			{
				contract.OnError = methodInfo4;
			}
		}

		public virtual JsonContract ResolveContract(Type type)
		{
			JsonContract jsonContract;
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			ResolverContractKey resolverContractKey = new ResolverContractKey(this.GetType(), type);
			Dictionary<ResolverContractKey, JsonContract> cache = this.GetCache();
			if (cache == null || !cache.TryGetValue(resolverContractKey, out jsonContract))
			{
				jsonContract = this.CreateContract(type);
				object obj = DefaultContractResolver._typeContractCacheLock;
				Monitor.Enter(obj);
				try
				{
					cache = this.GetCache();
					Dictionary<ResolverContractKey, JsonContract> resolverContractKeys = (cache == null ? new Dictionary<ResolverContractKey, JsonContract>() : new Dictionary<ResolverContractKey, JsonContract>(cache));
					resolverContractKeys[resolverContractKey] = jsonContract;
					this.UpdateCache(resolverContractKeys);
				}
				finally
				{
					Monitor.Exit(obj);
				}
			}
			return jsonContract;
		}

		protected virtual JsonConverter ResolveContractConverter(Type objectType)
		{
			return JsonTypeReflector.GetJsonConverter(objectType, objectType);
		}

		protected internal virtual string ResolvePropertyName(string propertyName)
		{
			return propertyName;
		}

		private void SetIsSpecifiedActions(JsonProperty property, MemberInfo member, bool allowNonPublicAccess)
		{
			MemberInfo field = member.DeclaringType.GetProperty(string.Concat(member.Name, "Specified"));
			if (field == null)
			{
				field = member.DeclaringType.GetField(string.Concat(member.Name, "Specified"));
			}
			if (field == null || ReflectionUtils.GetMemberUnderlyingType(field) != typeof(bool))
			{
				return;
			}
			Func<object, object> func = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(field);
			property.GetIsSpecified = (object o) => (bool)func(o);
			if (ReflectionUtils.CanSetMemberValue(field, allowNonPublicAccess, false))
			{
				property.SetIsSpecified = JsonTypeReflector.ReflectionDelegateFactory.CreateSet<object>(field);
			}
		}

		private void SetPropertySettingsFromAttributes(JsonProperty property, ICustomAttributeProvider attributeProvider, string name, Type declaringType, MemberSerialization memberSerialization, out bool allowNonPublicAccess, out bool hasExplicitAttribute)
		{
			DataMemberAttribute dataMemberAttribute;
			string propertyName;
			bool flag;
			NullValueHandling? nullable;
			DefaultValueHandling? nullable1;
			ReferenceLoopHandling? nullable2;
			ObjectCreationHandling? nullable3;
			TypeNameHandling? nullable4;
			bool? nullable5;
			int? nullable6;
			hasExplicitAttribute = false;
			if (JsonTypeReflector.GetDataContractAttribute(declaringType) == null || !(attributeProvider is MemberInfo))
			{
				dataMemberAttribute = null;
			}
			else
			{
				dataMemberAttribute = JsonTypeReflector.GetDataMemberAttribute((MemberInfo)attributeProvider);
			}
			JsonPropertyAttribute attribute = JsonTypeReflector.GetAttribute<JsonPropertyAttribute>(attributeProvider);
			if (attribute != null)
			{
				hasExplicitAttribute = true;
			}
			bool attribute1 = JsonTypeReflector.GetAttribute<JsonIgnoreAttribute>(attributeProvider) != null;
			if (attribute == null || attribute.PropertyName == null)
			{
				propertyName = (dataMemberAttribute == null || dataMemberAttribute.Name == null ? name : dataMemberAttribute.Name);
			}
			else
			{
				propertyName = attribute.PropertyName;
			}
			property.PropertyName = this.ResolvePropertyName(propertyName);
			property.UnderlyingName = name;
			if (attribute != null)
			{
				property.Required = attribute.Required;
				property.Order = attribute._order;
			}
			else if (dataMemberAttribute == null)
			{
				property.Required = Required.Default;
			}
			else
			{
				property.Required = (!dataMemberAttribute.IsRequired ? Required.Default : Required.AllowNull);
				JsonProperty jsonProperty = property;
				if (dataMemberAttribute.Order == -1)
				{
					nullable6 = null;
				}
				else
				{
					nullable6 = new int?(dataMemberAttribute.Order);
				}
				jsonProperty.Order = nullable6;
			}
			JsonProperty jsonProperty1 = property;
			if (attribute1)
			{
				flag = true;
			}
			else
			{
				flag = (memberSerialization != MemberSerialization.OptIn || attribute != null ? false : dataMemberAttribute == null);
			}
			jsonProperty1.Ignored = flag;
			property.Converter = JsonTypeReflector.GetJsonConverter(attributeProvider, property.PropertyType);
			property.MemberConverter = JsonTypeReflector.GetJsonConverter(attributeProvider, property.PropertyType);
			DefaultValueAttribute defaultValueAttribute = JsonTypeReflector.GetAttribute<DefaultValueAttribute>(attributeProvider);
			property.DefaultValue = (defaultValueAttribute == null ? null : defaultValueAttribute.Value);
			JsonProperty jsonProperty2 = property;
			if (attribute == null)
			{
				nullable = null;
			}
			else
			{
				nullable = attribute._nullValueHandling;
			}
			jsonProperty2.NullValueHandling = nullable;
			JsonProperty jsonProperty3 = property;
			if (attribute == null)
			{
				nullable1 = null;
			}
			else
			{
				nullable1 = attribute._defaultValueHandling;
			}
			jsonProperty3.DefaultValueHandling = nullable1;
			JsonProperty jsonProperty4 = property;
			if (attribute == null)
			{
				nullable2 = null;
			}
			else
			{
				nullable2 = attribute._referenceLoopHandling;
			}
			jsonProperty4.ReferenceLoopHandling = nullable2;
			JsonProperty jsonProperty5 = property;
			if (attribute == null)
			{
				nullable3 = null;
			}
			else
			{
				nullable3 = attribute._objectCreationHandling;
			}
			jsonProperty5.ObjectCreationHandling = nullable3;
			JsonProperty jsonProperty6 = property;
			if (attribute == null)
			{
				nullable4 = null;
			}
			else
			{
				nullable4 = attribute._typeNameHandling;
			}
			jsonProperty6.TypeNameHandling = nullable4;
			JsonProperty jsonProperty7 = property;
			if (attribute == null)
			{
				nullable5 = null;
			}
			else
			{
				nullable5 = attribute._isReference;
			}
			jsonProperty7.IsReference = nullable5;
			allowNonPublicAccess = false;
			if ((this.DefaultMembersSearchFlags & BindingFlags.NonPublic) == BindingFlags.NonPublic)
			{
				allowNonPublicAccess = true;
			}
			if (attribute != null)
			{
				allowNonPublicAccess = true;
			}
			if (dataMemberAttribute != null)
			{
				allowNonPublicAccess = true;
				hasExplicitAttribute = true;
			}
		}

		private bool ShouldSerializeEntityMember(MemberInfo memberInfo)
		{
			PropertyInfo propertyInfo = memberInfo as PropertyInfo;
			if (propertyInfo != null && propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition().FullName == "System.Data.Objects.DataClasses.EntityReference`1")
			{
				return false;
			}
			return true;
		}

		private void UpdateCache(Dictionary<ResolverContractKey, JsonContract> cache)
		{
			if (!this._sharedCache)
			{
				this._instanceContractCache = cache;
			}
			else
			{
				DefaultContractResolver._sharedContractCache = cache;
			}
		}
	}
}