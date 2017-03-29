using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

public static class GeneralUtils
{
	public const float DEVELOPMENT_BUILD_TEXT_WIDTH = 115f;

	public static bool AreArraysEqual<T>(T[] arr1, T[] arr2)
	{
		if (arr1 == arr2)
		{
			return true;
		}
		if (arr1 == null)
		{
			return false;
		}
		if (arr2 == null)
		{
			return false;
		}
		if ((int)arr1.Length != (int)arr2.Length)
		{
			return false;
		}
		for (int i = 0; i < (int)arr1.Length; i++)
		{
			if (!arr1[i].Equals(arr2[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static bool AreBytesEqual(byte[] bytes1, byte[] bytes2)
	{
		return GeneralUtils.AreArraysEqual<byte>(bytes1, bytes2);
	}

	public static bool CallbackIsValid(Delegate callback)
	{
		bool flag = true;
		if (callback == null)
		{
			flag = false;
		}
		else if (!callback.Method.IsStatic)
		{
			flag = GeneralUtils.IsObjectAlive(callback.Target);
			if (!flag)
			{
				Console.WriteLine(string.Format("Target for callback {0} is null.", callback.Method.Name));
			}
		}
		return flag;
	}

	private static object CloneClass(object obj, Type objType)
	{
		object obj1 = GeneralUtils.CreateNewType(objType);
		FieldInfo[] fields = objType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		for (int i = 0; i < (int)fields.Length; i++)
		{
			FieldInfo fieldInfo = fields[i];
			fieldInfo.SetValue(obj1, GeneralUtils.CloneValue(fieldInfo.GetValue(obj), fieldInfo.FieldType));
		}
		return obj1;
	}

	private static object CloneValue(object src, Type type)
	{
		if (src != null && type != typeof(string) && type.IsClass)
		{
			if (!type.IsGenericType)
			{
				return GeneralUtils.CloneClass(src, type);
			}
			if (src is IDictionary)
			{
				IDictionary dictionaries = src as IDictionary;
				IDictionary dictionaries1 = GeneralUtils.CreateNewType(type) as IDictionary;
				Type genericArguments = type.GetGenericArguments()[0];
				Type genericArguments1 = type.GetGenericArguments()[1];
				IDictionaryEnumerator enumerator = dictionaries.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry current = (DictionaryEntry)enumerator.Current;
						dictionaries1.Add(GeneralUtils.CloneValue(current.Key, genericArguments), GeneralUtils.CloneValue(current.Value, genericArguments1));
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable == null)
					{
					}
					disposable.Dispose();
				}
				return dictionaries1;
			}
			if (src is IList)
			{
				IList lists = src as IList;
				IList lists1 = GeneralUtils.CreateNewType(type) as IList;
				Type type1 = type.GetGenericArguments()[0];
				IEnumerator enumerator1 = lists.GetEnumerator();
				try
				{
					while (enumerator1.MoveNext())
					{
						object obj = enumerator1.Current;
						lists1.Add(GeneralUtils.CloneValue(obj, type1));
					}
				}
				finally
				{
					IDisposable disposable1 = enumerator1 as IDisposable;
					if (disposable1 == null)
					{
					}
					disposable1.Dispose();
				}
				return lists1;
			}
		}
		return src;
	}

	public static T[] Combine<T>(T[] arr1, T[] arr2)
	{
		T[] tArray = new T[(int)arr1.Length + (int)arr2.Length];
		Array.Copy(arr1, 0, tArray, 0, (int)arr1.Length);
		Array.Copy(arr1, 0, tArray, (int)arr1.Length, (int)arr2.Length);
		return tArray;
	}

	private static object CreateNewType(Type type)
	{
		object obj = Activator.CreateInstance(type);
		if (obj == null)
		{
			throw new SystemException(string.Format("Unable to instantiate type {0} with default constructor.", type.Name));
		}
		return obj;
	}

	public static T DeepClone<T>(T obj)
	{
		return (T)GeneralUtils.CloneValue(obj, obj.GetType());
	}

	public static void DeepReset<T>(T obj)
	{
		Type type = typeof(T);
		T t = Activator.CreateInstance<T>();
		if (t == null)
		{
			throw new SystemException(string.Format("Unable to instantiate type {0} with default constructor.", type.Name));
		}
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		for (int i = 0; i < (int)fields.Length; i++)
		{
			FieldInfo fieldInfo = fields[i];
			fieldInfo.SetValue(obj, fieldInfo.GetValue(t));
		}
	}

	public static bool ForceBool(string strVal)
	{
		string str = strVal.ToLowerInvariant().Trim();
		if (!(str == "on") && !(str == "1") && !(str == "true"))
		{
			return false;
		}
		return true;
	}

	public static float ForceFloat(string str)
	{
		float single = 0f;
		GeneralUtils.TryParseFloat(str, out single);
		return single;
	}

	public static int ForceInt(string str)
	{
		int num = 0;
		GeneralUtils.TryParseInt(str, out num);
		return num;
	}

	public static long ForceLong(string str)
	{
		long num = (long)0;
		GeneralUtils.TryParseLong(str, out num);
		return num;
	}

	public static bool IsEditorPlaying()
	{
		return false;
	}

	public static bool IsObjectAlive(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		return true;
	}

	public static bool IsOverriddenMethod(MethodInfo childMethod, MethodInfo ancestorMethod)
	{
		if (childMethod == null)
		{
			return false;
		}
		if (ancestorMethod == null)
		{
			return false;
		}
		if (childMethod.Equals(ancestorMethod))
		{
			return false;
		}
		MethodInfo baseDefinition = childMethod.GetBaseDefinition();
		while (!baseDefinition.Equals(childMethod) && !baseDefinition.Equals(ancestorMethod))
		{
			MethodInfo methodInfo = baseDefinition;
			baseDefinition = baseDefinition.GetBaseDefinition();
			if (!baseDefinition.Equals(methodInfo))
			{
				continue;
			}
			return false;
		}
		return baseDefinition.Equals(ancestorMethod);
	}

	public static void ListMove<T>(IList<T> list, int srcIndex, int dstIndex)
	{
		if (srcIndex == dstIndex)
		{
			return;
		}
		T item = list[srcIndex];
		list.RemoveAt(srcIndex);
		if (dstIndex > srcIndex)
		{
			dstIndex--;
		}
		list.Insert(dstIndex, item);
	}

	public static void ListSwap<T>(IList<T> list, int indexA, int indexB)
	{
		T item = list[indexA];
		list[indexA] = list[indexB];
		list[indexB] = item;
	}

	public static void Swap<T>(ref T a, ref T b)
	{
		T t = a;
		a = b;
		b = t;
	}

	public static bool TryParseBool(string strVal, out bool boolVal)
	{
		string str = strVal.ToLowerInvariant().Trim();
		if (str == "off" || str == "0" || str == "false")
		{
			boolVal = false;
			return true;
		}
		if (!(str == "on") && !(str == "1") && !(str == "true"))
		{
			boolVal = false;
			return false;
		}
		boolVal = true;
		return true;
	}

	public static bool TryParseFloat(string str, out float val)
	{
		return float.TryParse(str, NumberStyles.Any, null, out val);
	}

	public static bool TryParseInt(string str, out int val)
	{
		return int.TryParse(str, NumberStyles.Any, null, out val);
	}

	public static bool TryParseLong(string str, out long val)
	{
		return long.TryParse(str, NumberStyles.Any, null, out val);
	}

	public static int UnsignedMod(int x, int y)
	{
		int num = x % y;
		if (num < 0)
		{
			num = num + y;
		}
		return num;
	}
}