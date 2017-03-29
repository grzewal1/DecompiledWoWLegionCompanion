using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace Newtonsoft.Json
{
	public class JsonSerializer
	{
		private Newtonsoft.Json.TypeNameHandling _typeNameHandling;

		private FormatterAssemblyStyle _typeNameAssemblyFormat;

		private Newtonsoft.Json.PreserveReferencesHandling _preserveReferencesHandling;

		private Newtonsoft.Json.ReferenceLoopHandling _referenceLoopHandling;

		private Newtonsoft.Json.MissingMemberHandling _missingMemberHandling;

		private Newtonsoft.Json.ObjectCreationHandling _objectCreationHandling;

		private Newtonsoft.Json.NullValueHandling _nullValueHandling;

		private Newtonsoft.Json.DefaultValueHandling _defaultValueHandling;

		private Newtonsoft.Json.ConstructorHandling _constructorHandling;

		private JsonConverterCollection _converters;

		private IContractResolver _contractResolver;

		private IReferenceResolver _referenceResolver;

		private SerializationBinder _binder;

		private StreamingContext _context;

		private EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> Error;

		public virtual SerializationBinder Binder
		{
			get
			{
				return this._binder;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", "Serialization binder cannot be null.");
				}
				this._binder = value;
			}
		}

		public virtual Newtonsoft.Json.ConstructorHandling ConstructorHandling
		{
			get
			{
				return this._constructorHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.ConstructorHandling.Default || value > Newtonsoft.Json.ConstructorHandling.AllowNonPublicDefaultConstructor)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._constructorHandling = value;
			}
		}

		public virtual StreamingContext Context
		{
			get
			{
				return this._context;
			}
			set
			{
				this._context = value;
			}
		}

		public virtual IContractResolver ContractResolver
		{
			get
			{
				if (this._contractResolver == null)
				{
					this._contractResolver = DefaultContractResolver.Instance;
				}
				return this._contractResolver;
			}
			set
			{
				this._contractResolver = value;
			}
		}

		public virtual JsonConverterCollection Converters
		{
			get
			{
				if (this._converters == null)
				{
					this._converters = new JsonConverterCollection();
				}
				return this._converters;
			}
		}

		public virtual Newtonsoft.Json.DefaultValueHandling DefaultValueHandling
		{
			get
			{
				return this._defaultValueHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.DefaultValueHandling.Include || value > Newtonsoft.Json.DefaultValueHandling.IgnoreAndPopulate)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._defaultValueHandling = value;
			}
		}

		public virtual Newtonsoft.Json.MissingMemberHandling MissingMemberHandling
		{
			get
			{
				return this._missingMemberHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.MissingMemberHandling.Ignore || value > Newtonsoft.Json.MissingMemberHandling.Error)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._missingMemberHandling = value;
			}
		}

		public virtual Newtonsoft.Json.NullValueHandling NullValueHandling
		{
			get
			{
				return this._nullValueHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.NullValueHandling.Include || value > Newtonsoft.Json.NullValueHandling.Ignore)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._nullValueHandling = value;
			}
		}

		public virtual Newtonsoft.Json.ObjectCreationHandling ObjectCreationHandling
		{
			get
			{
				return this._objectCreationHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.ObjectCreationHandling.Auto || value > Newtonsoft.Json.ObjectCreationHandling.Replace)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._objectCreationHandling = value;
			}
		}

		public virtual Newtonsoft.Json.PreserveReferencesHandling PreserveReferencesHandling
		{
			get
			{
				return this._preserveReferencesHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.PreserveReferencesHandling.None || value > Newtonsoft.Json.PreserveReferencesHandling.All)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._preserveReferencesHandling = value;
			}
		}

		public virtual Newtonsoft.Json.ReferenceLoopHandling ReferenceLoopHandling
		{
			get
			{
				return this._referenceLoopHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.ReferenceLoopHandling.Error || value > Newtonsoft.Json.ReferenceLoopHandling.Serialize)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._referenceLoopHandling = value;
			}
		}

		public virtual IReferenceResolver ReferenceResolver
		{
			get
			{
				if (this._referenceResolver == null)
				{
					this._referenceResolver = new DefaultReferenceResolver();
				}
				return this._referenceResolver;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", "Reference resolver cannot be null.");
				}
				this._referenceResolver = value;
			}
		}

		public virtual FormatterAssemblyStyle TypeNameAssemblyFormat
		{
			get
			{
				return this._typeNameAssemblyFormat;
			}
			set
			{
				if (value < FormatterAssemblyStyle.Simple || value > FormatterAssemblyStyle.Full)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._typeNameAssemblyFormat = value;
			}
		}

		public virtual Newtonsoft.Json.TypeNameHandling TypeNameHandling
		{
			get
			{
				return this._typeNameHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.TypeNameHandling.None || value > Newtonsoft.Json.TypeNameHandling.Auto)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._typeNameHandling = value;
			}
		}

		public JsonSerializer()
		{
			this._referenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Error;
			this._missingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
			this._nullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
			this._defaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Include;
			this._objectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Auto;
			this._preserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
			this._constructorHandling = Newtonsoft.Json.ConstructorHandling.Default;
			this._typeNameHandling = Newtonsoft.Json.TypeNameHandling.None;
			this._context = JsonSerializerSettings.DefaultContext;
			this._binder = DefaultSerializationBinder.Instance;
		}

		public static JsonSerializer Create(JsonSerializerSettings settings)
		{
			JsonSerializer jsonSerializer = new JsonSerializer();
			if (settings != null)
			{
				if (!CollectionUtils.IsNullOrEmpty<JsonConverter>(settings.Converters))
				{
					jsonSerializer.Converters.AddRange<JsonConverter>(settings.Converters);
				}
				jsonSerializer.TypeNameHandling = settings.TypeNameHandling;
				jsonSerializer.TypeNameAssemblyFormat = settings.TypeNameAssemblyFormat;
				jsonSerializer.PreserveReferencesHandling = settings.PreserveReferencesHandling;
				jsonSerializer.ReferenceLoopHandling = settings.ReferenceLoopHandling;
				jsonSerializer.MissingMemberHandling = settings.MissingMemberHandling;
				jsonSerializer.ObjectCreationHandling = settings.ObjectCreationHandling;
				jsonSerializer.NullValueHandling = settings.NullValueHandling;
				jsonSerializer.DefaultValueHandling = settings.DefaultValueHandling;
				jsonSerializer.ConstructorHandling = settings.ConstructorHandling;
				jsonSerializer.Context = settings.Context;
				if (settings.Error != null)
				{
					jsonSerializer.Error += settings.get_Error();
				}
				if (settings.ContractResolver != null)
				{
					jsonSerializer.ContractResolver = settings.ContractResolver;
				}
				if (settings.ReferenceResolver != null)
				{
					jsonSerializer.ReferenceResolver = settings.ReferenceResolver;
				}
				if (settings.Binder != null)
				{
					jsonSerializer.Binder = settings.Binder;
				}
			}
			return jsonSerializer;
		}

		public object Deserialize(JsonReader reader)
		{
			return this.Deserialize(reader, null);
		}

		public object Deserialize(TextReader reader, Type objectType)
		{
			return this.Deserialize(new JsonTextReader(reader), objectType);
		}

		public T Deserialize<T>(JsonReader reader)
		{
			return (T)this.Deserialize(reader, typeof(T));
		}

		public object Deserialize(JsonReader reader, Type objectType)
		{
			return this.DeserializeInternal(reader, objectType);
		}

		internal virtual object DeserializeInternal(JsonReader reader, Type objectType)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			return (new JsonSerializerInternalReader(this)).Deserialize(reader, objectType);
		}

		internal JsonConverter GetMatchingConverter(Type type)
		{
			return JsonSerializer.GetMatchingConverter(this._converters, type);
		}

		internal static JsonConverter GetMatchingConverter(IList<JsonConverter> converters, Type objectType)
		{
			ValidationUtils.ArgumentNotNull(objectType, "objectType");
			if (converters != null)
			{
				for (int i = 0; i < converters.Count; i++)
				{
					JsonConverter item = converters[i];
					if (item.CanConvert(objectType))
					{
						return item;
					}
				}
			}
			return null;
		}

		internal void OnError(Newtonsoft.Json.Serialization.ErrorEventArgs e)
		{
			EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> error = this.Error;
			if (error != null)
			{
				error(this, e);
			}
		}

		public void Populate(TextReader reader, object target)
		{
			this.Populate(new JsonTextReader(reader), target);
		}

		public void Populate(JsonReader reader, object target)
		{
			this.PopulateInternal(reader, target);
		}

		internal virtual void PopulateInternal(JsonReader reader, object target)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			ValidationUtils.ArgumentNotNull(target, "target");
			(new JsonSerializerInternalReader(this)).Populate(reader, target);
		}

		public void Serialize(TextWriter textWriter, object value)
		{
			this.Serialize(new JsonTextWriter(textWriter), value);
		}

		public void Serialize(JsonWriter jsonWriter, object value)
		{
			this.SerializeInternal(jsonWriter, value);
		}

		internal virtual void SerializeInternal(JsonWriter jsonWriter, object value)
		{
			ValidationUtils.ArgumentNotNull(jsonWriter, "jsonWriter");
			(new JsonSerializerInternalWriter(this)).Serialize(jsonWriter, value);
		}

		public virtual event EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> Error
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.Error += value;
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.Error -= value;
			}
		}
	}
}