using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple=false)]
	public sealed class JsonPropertyAttribute : Attribute
	{
		internal Newtonsoft.Json.NullValueHandling? _nullValueHandling;

		internal Newtonsoft.Json.DefaultValueHandling? _defaultValueHandling;

		internal Newtonsoft.Json.ReferenceLoopHandling? _referenceLoopHandling;

		internal Newtonsoft.Json.ObjectCreationHandling? _objectCreationHandling;

		internal Newtonsoft.Json.TypeNameHandling? _typeNameHandling;

		internal bool? _isReference;

		internal int? _order;

		public Newtonsoft.Json.DefaultValueHandling DefaultValueHandling
		{
			get
			{
				Newtonsoft.Json.DefaultValueHandling? nullable = this._defaultValueHandling;
				return (!nullable.HasValue ? Newtonsoft.Json.DefaultValueHandling.Include : nullable.Value);
			}
			set
			{
				this._defaultValueHandling = new Newtonsoft.Json.DefaultValueHandling?(value);
			}
		}

		public bool IsReference
		{
			get
			{
				bool? nullable = this._isReference;
				return (!nullable.HasValue ? false : nullable.Value);
			}
			set
			{
				this._isReference = new bool?(value);
			}
		}

		public Newtonsoft.Json.NullValueHandling NullValueHandling
		{
			get
			{
				Newtonsoft.Json.NullValueHandling? nullable = this._nullValueHandling;
				return (!nullable.HasValue ? Newtonsoft.Json.NullValueHandling.Include : nullable.Value);
			}
			set
			{
				this._nullValueHandling = new Newtonsoft.Json.NullValueHandling?(value);
			}
		}

		public Newtonsoft.Json.ObjectCreationHandling ObjectCreationHandling
		{
			get
			{
				Newtonsoft.Json.ObjectCreationHandling? nullable = this._objectCreationHandling;
				return (!nullable.HasValue ? Newtonsoft.Json.ObjectCreationHandling.Auto : nullable.Value);
			}
			set
			{
				this._objectCreationHandling = new Newtonsoft.Json.ObjectCreationHandling?(value);
			}
		}

		public int Order
		{
			get
			{
				int? nullable = this._order;
				return (!nullable.HasValue ? 0 : nullable.Value);
			}
			set
			{
				this._order = new int?(value);
			}
		}

		public string PropertyName
		{
			get;
			set;
		}

		public Newtonsoft.Json.ReferenceLoopHandling ReferenceLoopHandling
		{
			get
			{
				Newtonsoft.Json.ReferenceLoopHandling? nullable = this._referenceLoopHandling;
				return (!nullable.HasValue ? Newtonsoft.Json.ReferenceLoopHandling.Error : nullable.Value);
			}
			set
			{
				this._referenceLoopHandling = new Newtonsoft.Json.ReferenceLoopHandling?(value);
			}
		}

		public Newtonsoft.Json.Required Required
		{
			get;
			set;
		}

		public Newtonsoft.Json.TypeNameHandling TypeNameHandling
		{
			get
			{
				Newtonsoft.Json.TypeNameHandling? nullable = this._typeNameHandling;
				return (!nullable.HasValue ? Newtonsoft.Json.TypeNameHandling.None : nullable.Value);
			}
			set
			{
				this._typeNameHandling = new Newtonsoft.Json.TypeNameHandling?(value);
			}
		}

		public JsonPropertyAttribute()
		{
		}

		public JsonPropertyAttribute(string propertyName)
		{
			this.PropertyName = propertyName;
		}
	}
}