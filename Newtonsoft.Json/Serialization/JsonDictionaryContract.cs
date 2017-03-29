using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Serialization
{
	public class JsonDictionaryContract : JsonContract
	{
		private readonly bool _isDictionaryValueTypeNullableType;

		private readonly Type _genericCollectionDefinitionType;

		private Type _genericWrapperType;

		private MethodCall<object, object> _genericWrapperCreator;

		internal Type DictionaryKeyType
		{
			get;
			private set;
		}

		internal Type DictionaryValueType
		{
			get;
			private set;
		}

		public Func<string, string> PropertyNameResolver
		{
			get;
			set;
		}

		public JsonDictionaryContract(Type underlyingType) : base(underlyingType)
		{
			Type genericArguments;
			Type type;
			if (!ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(IDictionary<,>), out this._genericCollectionDefinitionType))
			{
				ReflectionUtils.GetDictionaryKeyValueTypes(base.UnderlyingType, out genericArguments, out type);
			}
			else
			{
				genericArguments = this._genericCollectionDefinitionType.GetGenericArguments()[0];
				type = this._genericCollectionDefinitionType.GetGenericArguments()[1];
			}
			this.DictionaryKeyType = genericArguments;
			this.DictionaryValueType = type;
			if (this.DictionaryValueType != null)
			{
				this._isDictionaryValueTypeNullableType = ReflectionUtils.IsNullableType(this.DictionaryValueType);
			}
			if (this.IsTypeGenericDictionaryInterface(base.UnderlyingType))
			{
				base.CreatedType = ReflectionUtils.MakeGenericType(typeof(Dictionary<,>), new Type[] { genericArguments, type });
			}
			else if (base.UnderlyingType == typeof(IDictionary))
			{
				base.CreatedType = typeof(Dictionary<object, object>);
			}
		}

		internal IWrappedDictionary CreateWrapper(object dictionary)
		{
			if (dictionary is IDictionary && (this.DictionaryValueType == null || !this._isDictionaryValueTypeNullableType))
			{
				return new DictionaryWrapper<object, object>((IDictionary)dictionary);
			}
			if (this._genericWrapperType == null)
			{
				this._genericWrapperType = ReflectionUtils.MakeGenericType(typeof(DictionaryWrapper<,>), new Type[] { this.DictionaryKeyType, this.DictionaryValueType });
				ConstructorInfo constructor = this._genericWrapperType.GetConstructor(new Type[] { this._genericCollectionDefinitionType });
				this._genericWrapperCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(constructor);
			}
			return (IWrappedDictionary)this._genericWrapperCreator(null, new object[] { dictionary });
		}

		private bool IsTypeGenericDictionaryInterface(Type type)
		{
			if (!type.IsGenericType)
			{
				return false;
			}
			return type.GetGenericTypeDefinition() == typeof(IDictionary<,>);
		}
	}
}