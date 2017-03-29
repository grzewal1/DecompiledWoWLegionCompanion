using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Serialization
{
	public class JsonObjectContract : JsonContract
	{
		public JsonPropertyCollection ConstructorParameters
		{
			get;
			private set;
		}

		public Newtonsoft.Json.MemberSerialization MemberSerialization
		{
			get;
			set;
		}

		public ConstructorInfo OverrideConstructor
		{
			get;
			set;
		}

		public ConstructorInfo ParametrizedConstructor
		{
			get;
			set;
		}

		public JsonPropertyCollection Properties
		{
			get;
			private set;
		}

		public JsonObjectContract(Type underlyingType) : base(underlyingType)
		{
			this.Properties = new JsonPropertyCollection(base.UnderlyingType);
			this.ConstructorParameters = new JsonPropertyCollection(base.UnderlyingType);
		}
	}
}