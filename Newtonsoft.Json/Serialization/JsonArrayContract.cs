using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Serialization
{
	public class JsonArrayContract : JsonContract
	{
		private readonly bool _isCollectionItemTypeNullableType;

		private readonly Type _genericCollectionDefinitionType;

		private Type _genericWrapperType;

		private MethodCall<object, object> _genericWrapperCreator;

		internal Type CollectionItemType
		{
			get;
			private set;
		}

		public bool IsMultidimensionalArray
		{
			get;
			private set;
		}

		public JsonArrayContract(Type underlyingType) : base(underlyingType)
		{
			if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(ICollection<>), out this._genericCollectionDefinitionType))
			{
				this.CollectionItemType = this._genericCollectionDefinitionType.GetGenericArguments()[0];
			}
			else if (!underlyingType.IsGenericType || underlyingType.GetGenericTypeDefinition() != typeof(IEnumerable<>))
			{
				this.CollectionItemType = ReflectionUtils.GetCollectionItemType(base.UnderlyingType);
			}
			else
			{
				this._genericCollectionDefinitionType = typeof(IEnumerable<>);
				this.CollectionItemType = underlyingType.GetGenericArguments()[0];
			}
			if (this.CollectionItemType != null)
			{
				this._isCollectionItemTypeNullableType = ReflectionUtils.IsNullableType(this.CollectionItemType);
			}
			if (this.IsTypeGenericCollectionInterface(base.UnderlyingType))
			{
				base.CreatedType = ReflectionUtils.MakeGenericType(typeof(List<>), new Type[] { this.CollectionItemType });
			}
			else if (typeof(HashSet<>).IsAssignableFrom(base.UnderlyingType))
			{
				base.CreatedType = ReflectionUtils.MakeGenericType(typeof(HashSet<>), new Type[] { this.CollectionItemType });
			}
			this.IsMultidimensionalArray = (!base.UnderlyingType.IsArray ? false : base.UnderlyingType.GetArrayRank() > 1);
		}

		internal IWrappedCollection CreateWrapper(object list)
		{
			if (list is IList && (this.CollectionItemType == null || !this._isCollectionItemTypeNullableType) || base.UnderlyingType.IsArray)
			{
				return new CollectionWrapper<object>((IList)list);
			}
			if (this._genericCollectionDefinitionType != null)
			{
				this.EnsureGenericWrapperCreator();
				return (IWrappedCollection)this._genericWrapperCreator(null, new object[] { list });
			}
			IList lists = ((IEnumerable)list).Cast<object>().ToList<object>();
			if (this.CollectionItemType != null)
			{
				Array arrays = Array.CreateInstance(this.CollectionItemType, lists.Count);
				for (int i = 0; i < lists.Count; i++)
				{
					arrays.SetValue(lists[i], i);
				}
				lists = arrays;
			}
			return new CollectionWrapper<object>(lists);
		}

		private void EnsureGenericWrapperCreator()
		{
			Type type;
			if (this._genericWrapperType == null)
			{
				this._genericWrapperType = ReflectionUtils.MakeGenericType(typeof(CollectionWrapper<>), new Type[] { this.CollectionItemType });
				type = (ReflectionUtils.InheritsGenericDefinition(this._genericCollectionDefinitionType, typeof(List<>)) || this._genericCollectionDefinitionType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ? ReflectionUtils.MakeGenericType(typeof(ICollection<>), new Type[] { this.CollectionItemType }) : this._genericCollectionDefinitionType);
				ConstructorInfo constructor = this._genericWrapperType.GetConstructor(new Type[] { type });
				this._genericWrapperCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(constructor);
			}
		}

		private bool IsTypeGenericCollectionInterface(Type type)
		{
			if (!type.IsGenericType)
			{
				return false;
			}
			Type genericTypeDefinition = type.GetGenericTypeDefinition();
			return (genericTypeDefinition == typeof(IList<>) || genericTypeDefinition == typeof(ICollection<>) ? true : genericTypeDefinition == typeof(IEnumerable<>));
		}
	}
}