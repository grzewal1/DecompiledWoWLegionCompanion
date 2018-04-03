using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Newtonsoft.Json.Serialization
{
	public class JsonPropertyCollection : KeyedCollection<string, JsonProperty>
	{
		private readonly Type _type;

		public JsonPropertyCollection(Type type)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			this._type = type;
		}

		public void AddProperty(JsonProperty property)
		{
			if (base.Contains(property.PropertyName))
			{
				if (property.Ignored)
				{
					return;
				}
				JsonProperty item = base[property.PropertyName];
				if (!item.Ignored)
				{
					throw new JsonSerializationException("A member with the name '{0}' already exists on '{1}'. Use the JsonPropertyAttribute to specify another name.".FormatWith(CultureInfo.InvariantCulture, new object[] { property.PropertyName, this._type }));
				}
				base.Remove(item);
			}
			base.Add(property);
		}

		public JsonProperty GetClosestMatchProperty(string propertyName)
		{
			JsonProperty property = this.GetProperty(propertyName, StringComparison.Ordinal) ?? this.GetProperty(propertyName, StringComparison.OrdinalIgnoreCase);
			return property;
		}

		protected override string GetKeyForItem(JsonProperty item)
		{
			return item.PropertyName;
		}

		public JsonProperty GetProperty(string propertyName, StringComparison comparisonType)
		{
			JsonProperty jsonProperty;
			using (IEnumerator<JsonProperty> enumerator = base.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					JsonProperty current = enumerator.Current;
					if (!string.Equals(propertyName, current.PropertyName, comparisonType))
					{
						continue;
					}
					jsonProperty = current;
					return jsonProperty;
				}
				return null;
			}
			return jsonProperty;
		}
	}
}