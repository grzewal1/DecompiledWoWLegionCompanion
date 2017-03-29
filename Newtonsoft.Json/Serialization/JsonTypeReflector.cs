using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Newtonsoft.Json.Serialization
{
	internal static class JsonTypeReflector
	{
		public const string IdPropertyName = "$id";

		public const string RefPropertyName = "$ref";

		public const string TypePropertyName = "$type";

		public const string ValuePropertyName = "$value";

		public const string ArrayValuesPropertyName = "$values";

		public const string ShouldSerializePrefix = "ShouldSerialize";

		public const string SpecifiedPostfix = "Specified";

		private const string MetadataTypeAttributeTypeName = "System.ComponentModel.DataAnnotations.MetadataTypeAttribute, System.ComponentModel.DataAnnotations, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";

		private readonly static ThreadSafeStore<ICustomAttributeProvider, Type> JsonConverterTypeCache;

		private readonly static ThreadSafeStore<Type, Type> AssociatedMetadataTypesCache;

		private static Type _cachedMetadataTypeAttributeType;

		private static bool? _dynamicCodeGeneration;

		public static bool DynamicCodeGeneration
		{
			get
			{
				if (!JsonTypeReflector._dynamicCodeGeneration.HasValue)
				{
					JsonTypeReflector._dynamicCodeGeneration = new bool?(false);
				}
				return JsonTypeReflector._dynamicCodeGeneration.Value;
			}
		}

		public static Newtonsoft.Json.Utilities.ReflectionDelegateFactory ReflectionDelegateFactory
		{
			get
			{
				return LateBoundReflectionDelegateFactory.Instance;
			}
		}

		static JsonTypeReflector()
		{
			JsonTypeReflector.JsonConverterTypeCache = new ThreadSafeStore<ICustomAttributeProvider, Type>(new Func<ICustomAttributeProvider, Type>(JsonTypeReflector.GetJsonConverterTypeFromAttribute));
			JsonTypeReflector.AssociatedMetadataTypesCache = new ThreadSafeStore<Type, Type>(new Func<Type, Type>(JsonTypeReflector.GetAssociateMetadataTypeFromAttribute));
		}

		private static Type GetAssociatedMetadataType(Type type)
		{
			return JsonTypeReflector.AssociatedMetadataTypesCache.Get(type);
		}

		private static Type GetAssociateMetadataTypeFromAttribute(Type type)
		{
			Type metadataTypeAttributeType = JsonTypeReflector.GetMetadataTypeAttributeType();
			if (metadataTypeAttributeType == null)
			{
				return null;
			}
			object obj = type.GetCustomAttributes(metadataTypeAttributeType, true).SingleOrDefault<object>();
			if (obj == null)
			{
				return null;
			}
			return (new LateBoundMetadataTypeAttribute(obj)).MetadataClassType;
		}

		private static T GetAttribute<T>(Type type)
		where T : Attribute
		{
			T attribute;
			Type associatedMetadataType = JsonTypeReflector.GetAssociatedMetadataType(type);
			if (associatedMetadataType != null)
			{
				attribute = ReflectionUtils.GetAttribute<T>(associatedMetadataType, true);
				if (attribute != null)
				{
					return attribute;
				}
			}
			attribute = ReflectionUtils.GetAttribute<T>(type, true);
			if (attribute != null)
			{
				return attribute;
			}
			Type[] interfaces = type.GetInterfaces();
			for (int i = 0; i < (int)interfaces.Length; i++)
			{
				attribute = ReflectionUtils.GetAttribute<T>(interfaces[i], true);
				if (attribute != null)
				{
					return attribute;
				}
			}
			return (T)null;
		}

		private static T GetAttribute<T>(MemberInfo memberInfo)
		where T : Attribute
		{
			T attribute;
			Type associatedMetadataType = JsonTypeReflector.GetAssociatedMetadataType(memberInfo.DeclaringType);
			if (associatedMetadataType != null)
			{
				MemberInfo memberInfoFromType = ReflectionUtils.GetMemberInfoFromType(associatedMetadataType, memberInfo);
				if (memberInfoFromType != null)
				{
					attribute = ReflectionUtils.GetAttribute<T>(memberInfoFromType, true);
					if (attribute != null)
					{
						return attribute;
					}
				}
			}
			attribute = ReflectionUtils.GetAttribute<T>(memberInfo, true);
			if (attribute != null)
			{
				return attribute;
			}
			Type[] interfaces = memberInfo.DeclaringType.GetInterfaces();
			for (int i = 0; i < (int)interfaces.Length; i++)
			{
				MemberInfo memberInfoFromType1 = ReflectionUtils.GetMemberInfoFromType(interfaces[i], memberInfo);
				if (memberInfoFromType1 != null)
				{
					attribute = ReflectionUtils.GetAttribute<T>(memberInfoFromType1, true);
					if (attribute != null)
					{
						return attribute;
					}
				}
			}
			return (T)null;
		}

		public static T GetAttribute<T>(ICustomAttributeProvider attributeProvider)
		where T : Attribute
		{
			Type type = attributeProvider as Type;
			if (type != null)
			{
				return JsonTypeReflector.GetAttribute<T>(type);
			}
			MemberInfo memberInfo = attributeProvider as MemberInfo;
			if (memberInfo != null)
			{
				return JsonTypeReflector.GetAttribute<T>(memberInfo);
			}
			return ReflectionUtils.GetAttribute<T>(attributeProvider, true);
		}

		public static DataContractAttribute GetDataContractAttribute(Type type)
		{
			DataContractAttribute attribute = null;
			for (Type i = type; attribute == null && i != null; i = i.BaseType)
			{
				attribute = CachedAttributeGetter<DataContractAttribute>.GetAttribute(i);
			}
			return attribute;
		}

		public static DataMemberAttribute GetDataMemberAttribute(MemberInfo memberInfo)
		{
			if (memberInfo.MemberType == MemberTypes.Field)
			{
				return CachedAttributeGetter<DataMemberAttribute>.GetAttribute(memberInfo);
			}
			PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
			DataMemberAttribute attribute = CachedAttributeGetter<DataMemberAttribute>.GetAttribute(propertyInfo);
			if (attribute == null && propertyInfo.IsVirtual())
			{
				for (Type i = propertyInfo.DeclaringType; attribute == null && i != null; i = i.BaseType)
				{
					PropertyInfo memberInfoFromType = (PropertyInfo)ReflectionUtils.GetMemberInfoFromType(i, propertyInfo);
					if (memberInfoFromType != null && memberInfoFromType.IsVirtual())
					{
						attribute = CachedAttributeGetter<DataMemberAttribute>.GetAttribute(memberInfoFromType);
					}
				}
			}
			return attribute;
		}

		public static JsonArrayAttribute GetJsonArrayAttribute(Type type)
		{
			return JsonTypeReflector.GetJsonContainerAttribute(type) as JsonArrayAttribute;
		}

		public static JsonContainerAttribute GetJsonContainerAttribute(Type type)
		{
			return CachedAttributeGetter<JsonContainerAttribute>.GetAttribute(type);
		}

		public static JsonConverter GetJsonConverter(ICustomAttributeProvider attributeProvider, Type targetConvertedType)
		{
			Type jsonConverterType = JsonTypeReflector.GetJsonConverterType(attributeProvider);
			if (jsonConverterType == null)
			{
				return null;
			}
			JsonConverter jsonConverter = JsonConverterAttribute.CreateJsonConverterInstance(jsonConverterType);
			if (!jsonConverter.CanConvert(targetConvertedType))
			{
				throw new JsonSerializationException("JsonConverter {0} on {1} is not compatible with member type {2}.".FormatWith(CultureInfo.InvariantCulture, new object[] { jsonConverter.GetType().Name, attributeProvider, targetConvertedType.Name }));
			}
			return jsonConverter;
		}

		private static Type GetJsonConverterType(ICustomAttributeProvider attributeProvider)
		{
			return JsonTypeReflector.JsonConverterTypeCache.Get(attributeProvider);
		}

		private static Type GetJsonConverterTypeFromAttribute(ICustomAttributeProvider attributeProvider)
		{
			Type converterType;
			JsonConverterAttribute attribute = JsonTypeReflector.GetAttribute<JsonConverterAttribute>(attributeProvider);
			if (attribute == null)
			{
				converterType = null;
			}
			else
			{
				converterType = attribute.ConverterType;
			}
			return converterType;
		}

		public static JsonObjectAttribute GetJsonObjectAttribute(Type type)
		{
			return JsonTypeReflector.GetJsonContainerAttribute(type) as JsonObjectAttribute;
		}

		private static Type GetMetadataTypeAttributeType()
		{
			if (JsonTypeReflector._cachedMetadataTypeAttributeType == null)
			{
				Type type = Type.GetType("System.ComponentModel.DataAnnotations.MetadataTypeAttribute, System.ComponentModel.DataAnnotations, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
				if (type == null)
				{
					return null;
				}
				JsonTypeReflector._cachedMetadataTypeAttributeType = type;
			}
			return JsonTypeReflector._cachedMetadataTypeAttributeType;
		}

		public static MemberSerialization GetObjectMemberSerialization(Type objectType)
		{
			JsonObjectAttribute jsonObjectAttribute = JsonTypeReflector.GetJsonObjectAttribute(objectType);
			if (jsonObjectAttribute != null)
			{
				return jsonObjectAttribute.MemberSerialization;
			}
			if (JsonTypeReflector.GetDataContractAttribute(objectType) != null)
			{
				return MemberSerialization.OptIn;
			}
			return MemberSerialization.OptOut;
		}

		public static TypeConverter GetTypeConverter(Type type)
		{
			return TypeDescriptor.GetConverter(type);
		}
	}
}