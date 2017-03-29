using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace Newtonsoft.Json.Serialization
{
	public class DefaultSerializationBinder : SerializationBinder
	{
		internal readonly static DefaultSerializationBinder Instance;

		private readonly ThreadSafeStore<DefaultSerializationBinder.TypeNameKey, Type> _typeCache = new ThreadSafeStore<DefaultSerializationBinder.TypeNameKey, Type>(new Func<DefaultSerializationBinder.TypeNameKey, Type>(DefaultSerializationBinder.GetTypeFromTypeNameKey));

		static DefaultSerializationBinder()
		{
			DefaultSerializationBinder.Instance = new DefaultSerializationBinder();
		}

		public DefaultSerializationBinder()
		{
		}

		public override Type BindToType(string assemblyName, string typeName)
		{
			return this._typeCache.Get(new DefaultSerializationBinder.TypeNameKey(assemblyName, typeName));
		}

		private static Type GetTypeFromTypeNameKey(DefaultSerializationBinder.TypeNameKey typeNameKey)
		{
			string assemblyName = typeNameKey.AssemblyName;
			string typeName = typeNameKey.TypeName;
			if (assemblyName == null)
			{
				return Type.GetType(typeName);
			}
			Assembly assembly = Assembly.Load(assemblyName);
			if (assembly == null)
			{
				throw new JsonSerializationException("Could not load assembly '{0}'.".FormatWith(CultureInfo.InvariantCulture, new object[] { assemblyName }));
			}
			Type type = assembly.GetType(typeName);
			if (type == null)
			{
				throw new JsonSerializationException("Could not find type '{0}' in assembly '{1}'.".FormatWith(CultureInfo.InvariantCulture, new object[] { typeName, assembly.FullName }));
			}
			return type;
		}

		internal struct TypeNameKey
		{
			internal readonly string AssemblyName;

			internal readonly string TypeName;

			public TypeNameKey(string assemblyName, string typeName)
			{
				this.AssemblyName = assemblyName;
				this.TypeName = typeName;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is DefaultSerializationBinder.TypeNameKey))
				{
					return false;
				}
				return this.Equals((DefaultSerializationBinder.TypeNameKey)obj);
			}

			public bool Equals(DefaultSerializationBinder.TypeNameKey other)
			{
				return (this.AssemblyName != other.AssemblyName ? false : this.TypeName == other.TypeName);
			}

			public override int GetHashCode()
			{
				return (this.AssemblyName == null ? 0 : this.AssemblyName.GetHashCode()) ^ (this.TypeName == null ? 0 : this.TypeName.GetHashCode());
			}
		}
	}
}