using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Utilities
{
	internal static class CollectionUtils
	{
		public static bool AddDistinct<T>(this IList<T> list, T value)
		{
			return list.AddDistinct<T>(value, EqualityComparer<T>.Default);
		}

		public static bool AddDistinct<T>(this IList<T> list, T value, IEqualityComparer<T> comparer)
		{
			if (list.ContainsValue<T>(value, comparer))
			{
				return false;
			}
			list.Add(value);
			return true;
		}

		public static void AddRange<T>(this IList<T> initial, IEnumerable<T> collection)
		{
			if (initial == null)
			{
				throw new ArgumentNullException("initial");
			}
			if (collection == null)
			{
				return;
			}
			IEnumerator<T> enumerator = collection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					initial.Add(enumerator.Current);
				}
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
		}

		public static void AddRange(this IList initial, IEnumerable collection)
		{
			ValidationUtils.ArgumentNotNull(initial, "initial");
			(new ListWrapper<object>(initial)).AddRange<object>(collection.Cast<object>());
		}

		public static bool AddRangeDistinct<T>(this IList<T> list, IEnumerable<T> values)
		{
			return list.AddRangeDistinct<T>(values, EqualityComparer<T>.Default);
		}

		public static bool AddRangeDistinct<T>(this IList<T> list, IEnumerable<T> values, IEqualityComparer<T> comparer)
		{
			bool flag = true;
			IEnumerator<T> enumerator = values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (list.AddDistinct<T>(enumerator.Current, comparer))
					{
						continue;
					}
					flag = false;
				}
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
			return flag;
		}

		public static IEnumerable<T> CastValid<T>(this IEnumerable enumerable)
		{
			ValidationUtils.ArgumentNotNull(enumerable, "enumerable");
			return (
				from object  in enumerable
				where o is T
				select ).Cast<T>();
		}

		public static bool ContainsValue<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
		{
			bool flag;
			if (comparer == null)
			{
				comparer = EqualityComparer<TSource>.Default;
			}
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			IEnumerator<TSource> enumerator = source.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (!comparer.Equals(enumerator.Current, value))
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
			return flag;
		}

		private static void CopyFromJaggedToMultidimensionalArray(IList values, Array multidimensionalArray, int[] indices)
		{
			int length = (int)indices.Length;
			if (length == multidimensionalArray.Rank)
			{
				multidimensionalArray.SetValue(CollectionUtils.JaggedArrayGetValue(values, indices), indices);
				return;
			}
			int num = multidimensionalArray.GetLength(length);
			if (((IList)CollectionUtils.JaggedArrayGetValue(values, indices)).Count != num)
			{
				throw new Exception("Cannot deserialize non-cubical array as multidimensional array.");
			}
			int[] numArray = new int[length + 1];
			for (int i = 0; i < length; i++)
			{
				numArray[i] = indices[i];
			}
			for (int j = 0; j < multidimensionalArray.GetLength(length); j++)
			{
				numArray[length] = j;
				CollectionUtils.CopyFromJaggedToMultidimensionalArray(values, multidimensionalArray, numArray);
			}
		}

		public static object CreateAndPopulateList(Type listType, Action<IList, bool> populateList)
		{
			IList objs;
			Type type;
			ValidationUtils.ArgumentNotNull(listType, "listType");
			ValidationUtils.ArgumentNotNull(populateList, "populateList");
			bool flag = false;
			if (listType.IsArray)
			{
				objs = new List<object>();
				flag = true;
			}
			else if (ReflectionUtils.InheritsGenericDefinition(listType, typeof(ReadOnlyCollection<>), out type))
			{
				Type genericArguments = type.GetGenericArguments()[0];
				Type type1 = ReflectionUtils.MakeGenericType(typeof(IEnumerable<>), new Type[] { genericArguments });
				bool flag1 = false;
				ConstructorInfo[] constructors = listType.GetConstructors();
				int num = 0;
				while (num < (int)constructors.Length)
				{
					IList<ParameterInfo> parameters = constructors[num].GetParameters();
					if (parameters.Count != 1 || !type1.IsAssignableFrom(parameters[0].ParameterType))
					{
						num++;
					}
					else
					{
						flag1 = true;
						break;
					}
				}
				if (!flag1)
				{
					throw new Exception("Read-only type {0} does not have a public constructor that takes a type that implements {1}.".FormatWith(CultureInfo.InvariantCulture, new object[] { listType, type1 }));
				}
				objs = CollectionUtils.CreateGenericList(genericArguments);
				flag = true;
			}
			else if (typeof(IList).IsAssignableFrom(listType))
			{
				if (ReflectionUtils.IsInstantiatableType(listType))
				{
					objs = (IList)Activator.CreateInstance(listType);
				}
				else if (listType != typeof(IList))
				{
					objs = null;
				}
				else
				{
					objs = new List<object>();
				}
			}
			else if (ReflectionUtils.ImplementsGenericDefinition(listType, typeof(ICollection<>)))
			{
				if (!ReflectionUtils.IsInstantiatableType(listType))
				{
					objs = null;
				}
				else
				{
					objs = CollectionUtils.CreateCollectionWrapper(Activator.CreateInstance(listType));
				}
			}
			else if (listType != typeof(BitArray))
			{
				objs = null;
			}
			else
			{
				objs = new List<object>();
				flag = true;
			}
			if (objs == null)
			{
				throw new Exception("Cannot create and populate list type {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { listType }));
			}
			populateList(objs, flag);
			if (!flag)
			{
				if (objs is IWrappedCollection)
				{
					return ((IWrappedCollection)objs).UnderlyingCollection;
				}
			}
			else if (listType.IsArray)
			{
				objs = (listType.GetArrayRank() <= 1 ? CollectionUtils.ToArray(((List<object>)objs).ToArray(), ReflectionUtils.GetCollectionItemType(listType)) : CollectionUtils.ToMultidimensionalArray(objs, ReflectionUtils.GetCollectionItemType(listType), listType.GetArrayRank()));
			}
			else if (ReflectionUtils.InheritsGenericDefinition(listType, typeof(ReadOnlyCollection<>)))
			{
				objs = (IList)ReflectionUtils.CreateInstance(listType, new object[] { objs });
			}
			else if (listType == typeof(BitArray))
			{
				BitArray bitArrays = new BitArray(objs.Count);
				for (int i = 0; i < objs.Count; i++)
				{
					bitArrays[i] = (bool)objs[i];
				}
				return bitArrays;
			}
			return objs;
		}

		public static IWrappedCollection CreateCollectionWrapper(object list)
		{
			Type type;
			ValidationUtils.ArgumentNotNull(list, "list");
			if (!ReflectionUtils.ImplementsGenericDefinition(list.GetType(), typeof(ICollection<>), out type))
			{
				if (!(list is IList))
				{
					throw new Exception("Can not create ListWrapper for type {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { list.GetType() }));
				}
				return new CollectionWrapper<object>((IList)list);
			}
			Type collectionItemType = ReflectionUtils.GetCollectionItemType(type);
			Func<Type, IList<object>, object> func = (Type t, IList<object> a) => t.GetConstructor(new Type[] { type }).Invoke(new object[] { list });
			Type type1 = typeof(CollectionWrapper<>);
			Type[] typeArray = new Type[] { collectionItemType };
			object[] objArray = new object[] { list };
			return (IWrappedCollection)ReflectionUtils.CreateGeneric(type1, (IList<Type>)typeArray, func, objArray);
		}

		public static IWrappedDictionary CreateDictionaryWrapper(object dictionary)
		{
			Type type;
			ValidationUtils.ArgumentNotNull(dictionary, "dictionary");
			if (!ReflectionUtils.ImplementsGenericDefinition(dictionary.GetType(), typeof(IDictionary<,>), out type))
			{
				if (!(dictionary is IDictionary))
				{
					throw new Exception("Can not create DictionaryWrapper for type {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { dictionary.GetType() }));
				}
				return new DictionaryWrapper<object, object>((IDictionary)dictionary);
			}
			Type dictionaryKeyType = ReflectionUtils.GetDictionaryKeyType(type);
			Type dictionaryValueType = ReflectionUtils.GetDictionaryValueType(type);
			Func<Type, IList<object>, object> func = (Type t, IList<object> a) => t.GetConstructor(new Type[] { type }).Invoke(new object[] { dictionary });
			Type type1 = typeof(DictionaryWrapper<,>);
			Type[] typeArray = new Type[] { dictionaryKeyType, dictionaryValueType };
			object[] objArray = new object[] { dictionary };
			return (IWrappedDictionary)ReflectionUtils.CreateGeneric(type1, (IList<Type>)typeArray, func, objArray);
		}

		public static IDictionary CreateGenericDictionary(Type keyType, Type valueType)
		{
			ValidationUtils.ArgumentNotNull(keyType, "keyType");
			ValidationUtils.ArgumentNotNull(valueType, "valueType");
			return (IDictionary)ReflectionUtils.CreateGeneric(typeof(Dictionary<,>), keyType, new object[] { valueType });
		}

		public static IList CreateGenericList(Type listType)
		{
			ValidationUtils.ArgumentNotNull(listType, "listType");
			return (IList)ReflectionUtils.CreateGeneric(typeof(List<>), listType, new object[0]);
		}

		public static List<T> CreateList<T>(params T[] values)
		{
			return new List<T>(values);
		}

		public static List<T> CreateList<T>(ICollection collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			T[] tArray = new T[collection.Count];
			collection.CopyTo(tArray, 0);
			return new List<T>(tArray);
		}

		public static IWrappedList CreateListWrapper(object list)
		{
			Type type;
			ValidationUtils.ArgumentNotNull(list, "list");
			if (!ReflectionUtils.ImplementsGenericDefinition(list.GetType(), typeof(IList<>), out type))
			{
				if (!(list is IList))
				{
					throw new Exception("Can not create ListWrapper for type {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { list.GetType() }));
				}
				return new ListWrapper<object>((IList)list);
			}
			Type collectionItemType = ReflectionUtils.GetCollectionItemType(type);
			Func<Type, IList<object>, object> func = (Type t, IList<object> a) => t.GetConstructor(new Type[] { type }).Invoke(new object[] { list });
			Type type1 = typeof(ListWrapper<>);
			Type[] typeArray = new Type[] { collectionItemType };
			object[] objArray = new object[] { list };
			return (IWrappedList)ReflectionUtils.CreateGeneric(type1, (IList<Type>)typeArray, func, objArray);
		}

		public static List<T> Distinct<T>(List<T> collection)
		{
			List<T> ts = new List<T>();
			foreach (T t in collection)
			{
				if (ts.Contains(t))
				{
					continue;
				}
				ts.Add(t);
			}
			return ts;
		}

		public static List<List<T>> Flatten<T>(params IList<T>[] lists)
		{
			List<List<T>> lists1 = new List<List<T>>();
			Dictionary<int, T> nums = new Dictionary<int, T>();
			CollectionUtils.Recurse<T>(new List<IList<T>>(lists), 0, nums, lists1);
			return lists1;
		}

		private static IList<int> GetDimensions(IList values)
		{
			IList<int> nums = new List<int>();
			IList lists = values;
			while (true)
			{
				nums.Add(lists.Count);
				if (lists.Count != 0)
				{
					object item = lists[0];
					if (!(item is IList))
					{
						break;
					}
					else
					{
						lists = (IList)item;
					}
				}
				else
				{
					break;
				}
			}
			return nums;
		}

		public static T GetSingleItem<T>(IList<T> list)
		{
			return CollectionUtils.GetSingleItem<T>(list, false);
		}

		public static T GetSingleItem<T>(IList<T> list, bool returnDefaultIfEmpty)
		{
			if (list.Count == 1)
			{
				return list[0];
			}
			if (!returnDefaultIfEmpty || list.Count != 0)
			{
				throw new Exception("Expected single {0} in list but got {1}.".FormatWith(CultureInfo.InvariantCulture, new object[] { typeof(T), list.Count }));
			}
			return default(T);
		}

		public static Dictionary<K, List<V>> GroupBy<K, V>(ICollection<V> source, Func<V, K> keySelector)
		{
			List<V> vs;
			if (keySelector == null)
			{
				throw new ArgumentNullException("keySelector");
			}
			Dictionary<K, List<V>> ks = new Dictionary<K, List<V>>();
			IEnumerator<V> enumerator = source.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					V current = enumerator.Current;
					K k = keySelector(current);
					if (!ks.TryGetValue(k, out vs))
					{
						vs = new List<V>();
						ks.Add(k, vs);
					}
					vs.Add(current);
				}
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
			return ks;
		}

		public static int IndexOf<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
		{
			int num;
			int num1 = 0;
			IEnumerator<T> enumerator = collection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (!predicate(enumerator.Current))
					{
						num1++;
					}
					else
					{
						num = num1;
						return num;
					}
				}
				return -1;
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
			return num;
		}

		public static int IndexOf<TSource>(this IEnumerable<TSource> list, TSource value)
		where TSource : IEquatable<TSource>
		{
			return list.IndexOf<TSource>(value, EqualityComparer<TSource>.Default);
		}

		public static int IndexOf<TSource>(this IEnumerable<TSource> list, TSource value, IEqualityComparer<TSource> comparer)
		{
			int num;
			int num1 = 0;
			IEnumerator<TSource> enumerator = list.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (!comparer.Equals(enumerator.Current, value))
					{
						num1++;
					}
					else
					{
						num = num1;
						return num;
					}
				}
				return -1;
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
			return num;
		}

		public static bool IsCollectionType(Type type)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			if (type.IsArray)
			{
				return true;
			}
			if (typeof(ICollection).IsAssignableFrom(type))
			{
				return true;
			}
			if (ReflectionUtils.ImplementsGenericDefinition(type, typeof(ICollection<>)))
			{
				return true;
			}
			return false;
		}

		public static bool IsDictionaryType(Type type)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			if (typeof(IDictionary).IsAssignableFrom(type))
			{
				return true;
			}
			if (ReflectionUtils.ImplementsGenericDefinition(type, typeof(IDictionary<,>)))
			{
				return true;
			}
			return false;
		}

		public static bool IsListType(Type type)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			if (type.IsArray)
			{
				return true;
			}
			if (typeof(IList).IsAssignableFrom(type))
			{
				return true;
			}
			if (ReflectionUtils.ImplementsGenericDefinition(type, typeof(IList<>)))
			{
				return true;
			}
			return false;
		}

		public static bool IsNullOrEmpty(ICollection collection)
		{
			if (collection == null)
			{
				return true;
			}
			return collection.Count == 0;
		}

		public static bool IsNullOrEmpty<T>(ICollection<T> collection)
		{
			if (collection == null)
			{
				return true;
			}
			return collection.Count == 0;
		}

		public static bool IsNullOrEmptyOrDefault<T>(IList<T> list)
		{
			if (CollectionUtils.IsNullOrEmpty<T>(list))
			{
				return true;
			}
			return ReflectionUtils.ItemsUnitializedValue<T>(list);
		}

		private static object JaggedArrayGetValue(IList values, int[] indices)
		{
			IList item = values;
			for (int i = 0; i < (int)indices.Length; i++)
			{
				int num = indices[i];
				if (i == (int)indices.Length - 1)
				{
					return item[num];
				}
				item = (IList)item[num];
			}
			return item;
		}

		public static bool ListEquals<T>(IList<T> a, IList<T> b)
		{
			if (a == null || b == null)
			{
				return (a != null ? false : b == null);
			}
			if (a.Count != b.Count)
			{
				return false;
			}
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			for (int i = 0; i < a.Count; i++)
			{
				if (!@default.Equals(a[i], b[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static IList<T> Minus<T>(IList<T> list, IList<T> minus)
		{
			ValidationUtils.ArgumentNotNull(list, "list");
			List<T> ts = new List<T>(list.Count);
			IEnumerator<T> enumerator = list.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					T current = enumerator.Current;
					if (minus != null && minus.Contains(current))
					{
						continue;
					}
					ts.Add(current);
				}
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
			return ts;
		}

		private static void Recurse<T>(IList<IList<T>> global, int current, Dictionary<int, T> currentSet, List<List<T>> flattenedResult)
		{
			IList<T> item = global[current];
			for (int i = 0; i < item.Count; i++)
			{
				currentSet[current] = item[i];
				if (current != global.Count - 1)
				{
					CollectionUtils.Recurse<T>(global, current + 1, currentSet, flattenedResult);
				}
				else
				{
					List<T> ts = new List<T>();
					for (int j = 0; j < currentSet.Count; j++)
					{
						ts.Add(currentSet[j]);
					}
					flattenedResult.Add(ts);
				}
			}
		}

		public static IList<T> Slice<T>(IList<T> list, int? start, int? end)
		{
			return CollectionUtils.Slice<T>(list, start, end, null);
		}

		public static IList<T> Slice<T>(IList<T> list, int? start, int? end, int? step)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if ((step.GetValueOrDefault() != 0 ? false : step.HasValue))
			{
				throw new ArgumentException("Step cannot be zero.", "step");
			}
			List<T> ts = new List<T>();
			if (list.Count == 0)
			{
				return ts;
			}
			int num = (!step.HasValue ? 1 : step.Value);
			int num1 = (!start.HasValue ? 0 : start.Value);
			int num2 = (!end.HasValue ? list.Count : end.Value);
			num1 = (num1 >= 0 ? num1 : list.Count + num1);
			num2 = (num2 >= 0 ? num2 : list.Count + num2);
			num1 = Math.Max(num1, 0);
			num2 = Math.Min(num2, list.Count - 1);
			for (int i = num1; i < num2; i = i + num)
			{
				ts.Add(list[i]);
			}
			return ts;
		}

		public static Array ToArray(Array initial, Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			Array arrays = Array.CreateInstance(type, initial.Length);
			Array.Copy(initial, 0, arrays, 0, initial.Length);
			return arrays;
		}

		public static Array ToMultidimensionalArray(IList values, Type type, int rank)
		{
			IList<int> dimensions = CollectionUtils.GetDimensions(values);
			while (dimensions.Count < rank)
			{
				dimensions.Add(0);
			}
			Array arrays = Array.CreateInstance(type, dimensions.ToArray<int>());
			CollectionUtils.CopyFromJaggedToMultidimensionalArray(values, arrays, new int[0]);
			return arrays;
		}

		public static bool TryGetSingleItem<T>(IList<T> list, out T value)
		{
			return CollectionUtils.TryGetSingleItem<T>(list, false, out value);
		}

		public static bool TryGetSingleItem<T>(IList<T> list, bool returnDefaultIfEmpty, out T value)
		{
			return MiscellaneousUtils.TryAction<T>(() => CollectionUtils.GetSingleItem<T>(list, returnDefaultIfEmpty), out value);
		}
	}
}