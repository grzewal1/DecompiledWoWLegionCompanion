using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;

namespace Newtonsoft.Json
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Parameter, AllowMultiple=false)]
	public sealed class JsonConverterAttribute : Attribute
	{
		private readonly Type _converterType;

		public Type ConverterType
		{
			get
			{
				return this._converterType;
			}
		}

		public JsonConverterAttribute(Type converterType)
		{
			if (converterType == null)
			{
				throw new ArgumentNullException("converterType");
			}
			this._converterType = converterType;
		}

		internal static JsonConverter CreateJsonConverterInstance(Type converterType)
		{
			JsonConverter jsonConverter;
			try
			{
				jsonConverter = (JsonConverter)Activator.CreateInstance(converterType);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				throw new Exception("Error creating {0}".FormatWith(CultureInfo.InvariantCulture, new object[] { converterType }), exception);
			}
			return jsonConverter;
		}
	}
}