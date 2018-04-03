using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;

namespace Newtonsoft.Json.Converters
{
	public class StringEnumConverter : JsonConverter
	{
		private readonly Dictionary<Type, BidirectionalDictionary<string, string>> _enumMemberNamesPerType = new Dictionary<Type, BidirectionalDictionary<string, string>>();

		public bool CamelCaseText
		{
			get;
			set;
		}

		public StringEnumConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			return ((!ReflectionUtils.IsNullableType(objectType) ? objectType : Nullable.GetUnderlyingType(objectType))).IsEnum;
		}

		private BidirectionalDictionary<string, string> GetEnumNameMap(Type t)
		{
			BidirectionalDictionary<string, string> bidirectionalDictionary;
			BidirectionalDictionary<string, string> bidirectionalDictionary1;
			string str;
			if (!this._enumMemberNamesPerType.TryGetValue(t, out bidirectionalDictionary))
			{
				object obj = this._enumMemberNamesPerType;
				Monitor.Enter(obj);
				try
				{
					if (!this._enumMemberNamesPerType.TryGetValue(t, out bidirectionalDictionary))
					{
						bidirectionalDictionary = new BidirectionalDictionary<string, string>(StringComparer.OrdinalIgnoreCase, StringComparer.OrdinalIgnoreCase);
						FieldInfo[] fields = t.GetFields();
						for (int i = 0; i < (int)fields.Length; i++)
						{
							FieldInfo fieldInfo = fields[i];
							string name = fieldInfo.Name;
							string str1 = (
								from EnumMemberAttribute a in fieldInfo.GetCustomAttributes(typeof(EnumMemberAttribute), true)
								select a.Value).SingleOrDefault<string>() ?? fieldInfo.Name;
							if (bidirectionalDictionary.TryGetBySecond(str1, out str))
							{
								throw new Exception("Enum name '{0}' already exists on enum '{1}'.".FormatWith(CultureInfo.InvariantCulture, new object[] { str1, t.Name }));
							}
							bidirectionalDictionary.Add(name, str1);
						}
						this._enumMemberNamesPerType[t] = bidirectionalDictionary;
						return bidirectionalDictionary;
					}
					else
					{
						bidirectionalDictionary1 = bidirectionalDictionary;
					}
				}
				finally
				{
					Monitor.Exit(obj);
				}
				return bidirectionalDictionary1;
			}
			return bidirectionalDictionary;
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			string str;
			Type type = (!ReflectionUtils.IsNullableType(objectType) ? objectType : Nullable.GetUnderlyingType(objectType));
			if (reader.TokenType == JsonToken.Null)
			{
				if (!ReflectionUtils.IsNullableType(objectType))
				{
					throw new Exception("Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { objectType }));
				}
				return null;
			}
			if (reader.TokenType != JsonToken.String)
			{
				if (reader.TokenType != JsonToken.Integer)
				{
					throw new Exception("Unexpected token when parsing enum. Expected String or Integer, got {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { reader.TokenType }));
				}
				return ConvertUtils.ConvertOrCast(reader.Value, CultureInfo.InvariantCulture, type);
			}
			BidirectionalDictionary<string, string> enumNameMap = this.GetEnumNameMap(type);
			enumNameMap.TryGetBySecond(reader.Value.ToString(), out str);
			str = str ?? reader.Value.ToString();
			return Enum.Parse(type, str, true);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			string camelCase;
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			Enum @enum = (Enum)value;
			string str = @enum.ToString("G");
			if (char.IsNumber(str[0]) || str[0] == '-')
			{
				writer.WriteValue(value);
			}
			else
			{
				BidirectionalDictionary<string, string> enumNameMap = this.GetEnumNameMap(@enum.GetType());
				enumNameMap.TryGetByFirst(str, out camelCase);
				camelCase = camelCase ?? str;
				if (this.CamelCaseText)
				{
					camelCase = StringUtils.ToCamelCase(camelCase);
				}
				writer.WriteValue(camelCase);
			}
		}
	}
}