using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace bgs
{
	public class EnumUtils
	{
		private static Dictionary<Type, Dictionary<string, object>> s_enumCache;

		static EnumUtils()
		{
			EnumUtils.s_enumCache = new Dictionary<Type, Dictionary<string, object>>();
		}

		public EnumUtils()
		{
		}

		public static T GetEnum<T>(string str)
		{
			return EnumUtils.GetEnum<T>(str, StringComparison.Ordinal);
		}

		public static T GetEnum<T>(string str, StringComparison comparisonType)
		{
			T t;
			if (!EnumUtils.TryGetEnum<T>(str, comparisonType, out t))
			{
				throw new ArgumentException(string.Format("EnumUtils.GetEnum() - \"{0}\" has no matching value in enum {1}", str, typeof(T)));
			}
			return t;
		}

		public static string GetString<T>(T enumVal)
		{
			string str = enumVal.ToString();
			FieldInfo field = enumVal.GetType().GetField(str);
			DescriptionAttribute[] customAttributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
			if ((int)customAttributes.Length <= 0)
			{
				return str;
			}
			return customAttributes[0].Description;
		}

		public static int Length<T>()
		{
			return Enum.GetValues(typeof(T)).Length;
		}

		public static T Parse<T>(string str)
		{
			return (T)Enum.Parse(typeof(T), str);
		}

		public static T SafeParse<T>(string str)
		{
			T t;
			try
			{
				t = (T)Enum.Parse(typeof(T), str);
			}
			catch (Exception exception)
			{
				t = default(T);
			}
			return t;
		}

		public static bool TryCast<T>(object inVal, out T outVal)
		{
			bool flag;
			outVal = default(T);
			try
			{
				outVal = (T)inVal;
				flag = true;
			}
			catch (Exception exception)
			{
				flag = false;
			}
			return flag;
		}

		public static bool TryGetEnum<T>(string str, StringComparison comparisonType, out T result)
		{
			Dictionary<string, object> strs;
			object obj;
			bool flag;
			Type type = typeof(T);
			EnumUtils.s_enumCache.TryGetValue(type, out strs);
			if (strs != null && strs.TryGetValue(str, out obj))
			{
				result = (T)obj;
				return true;
			}
			IEnumerator enumerator = Enum.GetValues(type).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					T current = (T)enumerator.Current;
					bool flag1 = false;
					if (!EnumUtils.GetString<T>(current).Equals(str, comparisonType))
					{
						FieldInfo field = current.GetType().GetField(current.ToString());
						DescriptionAttribute[] customAttributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
						int num = 0;
						while (num < (int)customAttributes.Length)
						{
							if (!customAttributes[num].Description.Equals(str, comparisonType))
							{
								num++;
							}
							else
							{
								flag1 = true;
								break;
							}
						}
					}
					else
					{
						flag1 = true;
						result = current;
					}
					if (!flag1)
					{
						continue;
					}
					if (strs == null)
					{
						strs = new Dictionary<string, object>();
						EnumUtils.s_enumCache.Add(type, strs);
					}
					if (!strs.ContainsKey(str))
					{
						strs.Add(str, current);
					}
					result = current;
					flag = true;
					return flag;
				}
				result = default(T);
				return false;
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
			return flag;
		}

		public static bool TryGetEnum<T>(string str, out T outVal)
		{
			return EnumUtils.TryGetEnum<T>(str, StringComparison.Ordinal, out outVal);
		}
	}
}