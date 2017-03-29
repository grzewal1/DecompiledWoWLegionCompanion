using Newtonsoft.Json.Converters;
using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace JamLib
{
	public class StringEnumConverter : Newtonsoft.Json.Converters.StringEnumConverter
	{
		public StringEnumConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			Type type = (!objectType.IsGenericType || objectType.GetGenericTypeDefinition() != typeof(Nullable<>) ? objectType : Nullable.GetUnderlyingType(objectType));
			if (!type.IsEnum)
			{
				return false;
			}
			object[] customAttributes = type.GetCustomAttributes(typeof(DataContractAttribute), false);
			return (customAttributes == null ? false : (int)customAttributes.Length > 0);
		}
	}
}