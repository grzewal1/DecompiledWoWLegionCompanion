using Newtonsoft.Json.Utilities;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Serialization
{
	internal static class CachedAttributeGetter<T>
	where T : Attribute
	{
		private readonly static ThreadSafeStore<ICustomAttributeProvider, T> TypeAttributeCache;

		static CachedAttributeGetter()
		{
			CachedAttributeGetter<T>.TypeAttributeCache = new ThreadSafeStore<ICustomAttributeProvider, T>(new Func<ICustomAttributeProvider, T>(JsonTypeReflector.GetAttribute<T>));
		}

		public static T GetAttribute(ICustomAttributeProvider type)
		{
			return CachedAttributeGetter<T>.TypeAttributeCache.Get(type);
		}
	}
}