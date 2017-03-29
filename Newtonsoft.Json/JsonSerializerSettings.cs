using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace Newtonsoft.Json
{
	public class JsonSerializerSettings
	{
		internal const Newtonsoft.Json.ReferenceLoopHandling DefaultReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Error;

		internal const Newtonsoft.Json.MissingMemberHandling DefaultMissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;

		internal const Newtonsoft.Json.NullValueHandling DefaultNullValueHandling = Newtonsoft.Json.NullValueHandling.Include;

		internal const Newtonsoft.Json.DefaultValueHandling DefaultDefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Include;

		internal const Newtonsoft.Json.ObjectCreationHandling DefaultObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Auto;

		internal const Newtonsoft.Json.PreserveReferencesHandling DefaultPreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;

		internal const Newtonsoft.Json.ConstructorHandling DefaultConstructorHandling = Newtonsoft.Json.ConstructorHandling.Default;

		internal const Newtonsoft.Json.TypeNameHandling DefaultTypeNameHandling = Newtonsoft.Json.TypeNameHandling.None;

		internal const FormatterAssemblyStyle DefaultTypeNameAssemblyFormat = FormatterAssemblyStyle.Simple;

		internal readonly static StreamingContext DefaultContext;

		public SerializationBinder Binder
		{
			get;
			set;
		}

		public Newtonsoft.Json.ConstructorHandling ConstructorHandling
		{
			get;
			set;
		}

		public StreamingContext Context
		{
			get;
			set;
		}

		public IContractResolver ContractResolver
		{
			get;
			set;
		}

		public IList<JsonConverter> Converters
		{
			get;
			set;
		}

		public Newtonsoft.Json.DefaultValueHandling DefaultValueHandling
		{
			get;
			set;
		}

		public EventHandler<ErrorEventArgs> Error
		{
			get;
			set;
		}

		public Newtonsoft.Json.MissingMemberHandling MissingMemberHandling
		{
			get;
			set;
		}

		public Newtonsoft.Json.NullValueHandling NullValueHandling
		{
			get;
			set;
		}

		public Newtonsoft.Json.ObjectCreationHandling ObjectCreationHandling
		{
			get;
			set;
		}

		public Newtonsoft.Json.PreserveReferencesHandling PreserveReferencesHandling
		{
			get;
			set;
		}

		public Newtonsoft.Json.ReferenceLoopHandling ReferenceLoopHandling
		{
			get;
			set;
		}

		public IReferenceResolver ReferenceResolver
		{
			get;
			set;
		}

		public FormatterAssemblyStyle TypeNameAssemblyFormat
		{
			get;
			set;
		}

		public Newtonsoft.Json.TypeNameHandling TypeNameHandling
		{
			get;
			set;
		}

		static JsonSerializerSettings()
		{
			JsonSerializerSettings.DefaultContext = new StreamingContext();
		}

		public JsonSerializerSettings()
		{
			this.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Error;
			this.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
			this.ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Auto;
			this.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
			this.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Include;
			this.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
			this.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None;
			this.TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple;
			this.Context = JsonSerializerSettings.DefaultContext;
			this.Converters = new List<JsonConverter>();
		}
	}
}