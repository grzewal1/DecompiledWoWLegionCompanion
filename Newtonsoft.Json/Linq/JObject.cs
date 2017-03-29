using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Linq
{
	public class JObject : JContainer, IEnumerable, IDictionary<string, JToken>, ICollection<KeyValuePair<string, JToken>>, ICustomTypeDescriptor, INotifyPropertyChanged, IEnumerable<KeyValuePair<string, JToken>>
	{
		private JObject.JPropertKeyedCollection _properties = new JObject.JPropertKeyedCollection(StringComparer.Ordinal);

		private PropertyChangedEventHandler PropertyChanged;

		protected override IList<JToken> ChildrenTokens
		{
			get
			{
				return this._properties;
			}
		}

		public override JToken this[object key]
		{
			get
			{
				ValidationUtils.ArgumentNotNull(key, "o");
				string str = key as string;
				if (str == null)
				{
					throw new ArgumentException("Accessed JObject values with invalid key value: {0}. Object property name expected.".FormatWith(CultureInfo.InvariantCulture, new object[] { MiscellaneousUtils.ToString(key) }));
				}
				return this[str];
			}
			set
			{
				ValidationUtils.ArgumentNotNull(key, "o");
				string str = key as string;
				if (str == null)
				{
					throw new ArgumentException("Set JObject values with invalid key value: {0}. Object property name expected.".FormatWith(CultureInfo.InvariantCulture, new object[] { MiscellaneousUtils.ToString(key) }));
				}
				this[str] = value;
			}
		}

		public JToken this[string propertyName]
		{
			get
			{
				JToken value;
				ValidationUtils.ArgumentNotNull(propertyName, "propertyName");
				JProperty jProperty = this.Property(propertyName);
				if (jProperty == null)
				{
					value = null;
				}
				else
				{
					value = jProperty.Value;
				}
				return value;
			}
			set
			{
				JProperty jProperty = this.Property(propertyName);
				if (jProperty == null)
				{
					this.Add(new JProperty(propertyName, value));
					this.OnPropertyChanged(propertyName);
				}
				else
				{
					jProperty.Value = value;
				}
			}
		}

		bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string,Newtonsoft.Json.Linq.JToken>>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		ICollection<string> System.Collections.Generic.IDictionary<string,Newtonsoft.Json.Linq.JToken>.Keys
		{
			get
			{
				return this._properties.Dictionary.Keys;
			}
		}

		ICollection<JToken> System.Collections.Generic.IDictionary<string,Newtonsoft.Json.Linq.JToken>.Values
		{
			get
			{
				return this._properties.Dictionary.Values;
			}
		}

		public override JTokenType Type
		{
			get
			{
				return JTokenType.Object;
			}
		}

		public JObject()
		{
		}

		public JObject(JObject other) : base(other)
		{
		}

		public JObject(params object[] content) : this((object)content)
		{
		}

		public JObject(object content)
		{
			this.Add(content);
		}

		public void Add(string propertyName, JToken value)
		{
			this.Add(new JProperty(propertyName, value));
		}

		internal override JToken CloneToken()
		{
			return new JObject(this);
		}

		internal override bool DeepEquals(JToken node)
		{
			JObject jObjects = node as JObject;
			return (jObjects == null ? false : base.ContentsEqual(jObjects));
		}

		public static new JObject FromObject(object o)
		{
			return JObject.FromObject(o, new JsonSerializer());
		}

		public static new JObject FromObject(object o, JsonSerializer jsonSerializer)
		{
			JToken jTokens = JToken.FromObjectInternal(o, jsonSerializer);
			if (jTokens != null && jTokens.Type != JTokenType.Object)
			{
				throw new ArgumentException("Object serialized to {0}. JObject instance expected.".FormatWith(CultureInfo.InvariantCulture, new object[] { jTokens.Type }));
			}
			return (JObject)jTokens;
		}

		internal override int GetDeepHashCode()
		{
			return base.ContentsHashCode();
		}

		[DebuggerHidden]
		public IEnumerator<KeyValuePair<string, JToken>> GetEnumerator()
		{
			JObject.<GetEnumerator>c__Iterator4 variable = null;
			return variable;
		}

		private static System.Type GetTokenPropertyType(JToken token)
		{
			if (!(token is JValue))
			{
				return token.GetType();
			}
			JValue jValue = (JValue)token;
			return (jValue.Value == null ? typeof(object) : jValue.Value.GetType());
		}

		internal override void InsertItem(int index, JToken item)
		{
			if (item != null && item.Type == JTokenType.Comment)
			{
				return;
			}
			base.InsertItem(index, item);
		}

		internal void InternalPropertyChanged(JProperty childProperty)
		{
			this.OnPropertyChanged(childProperty.Name);
		}

		internal void InternalPropertyChanging(JProperty childProperty)
		{
		}

		public static new JObject Load(JsonReader reader)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			if (reader.TokenType == JsonToken.None && !reader.Read())
			{
				throw new Exception("Error reading JObject from JsonReader.");
			}
			if (reader.TokenType != JsonToken.StartObject)
			{
				throw new Exception("Error reading JObject from JsonReader. Current JsonReader item is not an object: {0}".FormatWith(CultureInfo.InvariantCulture, new object[] { reader.TokenType }));
			}
			JObject jObjects = new JObject();
			jObjects.SetLineInfo(reader as IJsonLineInfo);
			jObjects.ReadTokenFrom(reader);
			return jObjects;
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public static new JObject Parse(string json)
		{
			return JObject.Load(new JsonTextReader(new StringReader(json)));
		}

		public IEnumerable<JProperty> Properties()
		{
			return this.ChildrenTokens.Cast<JProperty>();
		}

		public JProperty Property(string name)
		{
			JToken jTokens;
			if (this._properties.Dictionary == null)
			{
				return null;
			}
			if (name == null)
			{
				return null;
			}
			this._properties.Dictionary.TryGetValue(name, out jTokens);
			return (JProperty)jTokens;
		}

		public JEnumerable<JToken> PropertyValues()
		{
			return new JEnumerable<JToken>(
				from p in this.Properties()
				select p.Value);
		}

		public bool Remove(string propertyName)
		{
			JProperty jProperty = this.Property(propertyName);
			if (jProperty == null)
			{
				return false;
			}
			jProperty.Remove();
			return true;
		}

		void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string,Newtonsoft.Json.Linq.JToken>>.Add(KeyValuePair<string, JToken> item)
		{
			this.Add(new JProperty(item.Key, item.Value));
		}

		void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string,Newtonsoft.Json.Linq.JToken>>.Clear()
		{
			base.RemoveAll();
		}

		bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string,Newtonsoft.Json.Linq.JToken>>.Contains(KeyValuePair<string, JToken> item)
		{
			JProperty jProperty = this.Property(item.Key);
			if (jProperty == null)
			{
				return false;
			}
			return jProperty.Value == item.Value;
		}

		void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string,Newtonsoft.Json.Linq.JToken>>.CopyTo(KeyValuePair<string, JToken>[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex", "arrayIndex is less than 0.");
			}
			if (arrayIndex >= (int)array.Length)
			{
				throw new ArgumentException("arrayIndex is equal to or greater than the length of array.");
			}
			if (this.Count > (int)array.Length - arrayIndex)
			{
				throw new ArgumentException("The number of elements in the source JObject is greater than the available space from arrayIndex to the end of the destination array.");
			}
			int num = 0;
			IEnumerator<JToken> enumerator = this.ChildrenTokens.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					JProperty current = (JProperty)enumerator.Current;
					array[arrayIndex + num] = new KeyValuePair<string, JToken>(current.Name, current.Value);
					num++;
				}
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
		}

		bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string,Newtonsoft.Json.Linq.JToken>>.Remove(KeyValuePair<string, JToken> item)
		{
			if (!((ICollection<KeyValuePair<string, JToken>>)this).Contains(item))
			{
				return false;
			}
			((IDictionary<string, JToken>)this).Remove(item.Key);
			return true;
		}

		bool System.Collections.Generic.IDictionary<string,Newtonsoft.Json.Linq.JToken>.ContainsKey(string key)
		{
			if (this._properties.Dictionary == null)
			{
				return false;
			}
			return this._properties.Dictionary.ContainsKey(key);
		}

		AttributeCollection System.ComponentModel.ICustomTypeDescriptor.GetAttributes()
		{
			return AttributeCollection.Empty;
		}

		string System.ComponentModel.ICustomTypeDescriptor.GetClassName()
		{
			return null;
		}

		string System.ComponentModel.ICustomTypeDescriptor.GetComponentName()
		{
			return null;
		}

		TypeConverter System.ComponentModel.ICustomTypeDescriptor.GetConverter()
		{
			return new TypeConverter();
		}

		EventDescriptor System.ComponentModel.ICustomTypeDescriptor.GetDefaultEvent()
		{
			return null;
		}

		PropertyDescriptor System.ComponentModel.ICustomTypeDescriptor.GetDefaultProperty()
		{
			return null;
		}

		object System.ComponentModel.ICustomTypeDescriptor.GetEditor(System.Type editorBaseType)
		{
			return null;
		}

		EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return EventDescriptorCollection.Empty;
		}

		EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
		{
			return EventDescriptorCollection.Empty;
		}

		PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
		{
			return ((ICustomTypeDescriptor)this).GetProperties(null);
		}

		PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollections = new PropertyDescriptorCollection(null);
			IEnumerator<KeyValuePair<string, JToken>> enumerator = this.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, JToken> current = enumerator.Current;
					propertyDescriptorCollections.Add(new JPropertyDescriptor(current.Key, JObject.GetTokenPropertyType(current.Value)));
				}
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
			return propertyDescriptorCollections;
		}

		object System.ComponentModel.ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return null;
		}

		public bool TryGetValue(string propertyName, out JToken value)
		{
			JProperty jProperty = this.Property(propertyName);
			if (jProperty == null)
			{
				value = null;
				return false;
			}
			value = jProperty.Value;
			return true;
		}

		internal override void ValidateToken(JToken o, JToken existing)
		{
			ValidationUtils.ArgumentNotNull(o, "o");
			if (o.Type != JTokenType.Property)
			{
				throw new ArgumentException("Can not add {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, new object[] { o.GetType(), base.GetType() }));
			}
			JProperty jProperty = (JProperty)o;
			if (existing != null)
			{
				JProperty jProperty1 = (JProperty)existing;
				if (jProperty.Name == jProperty1.Name)
				{
					return;
				}
			}
			if (this._properties.Dictionary != null && this._properties.Dictionary.TryGetValue(jProperty.Name, out existing))
			{
				throw new ArgumentException("Can not add property {0} to {1}. Property with the same name already exists on object.".FormatWith(CultureInfo.InvariantCulture, new object[] { jProperty.Name, base.GetType() }));
			}
		}

		public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
		{
			writer.WriteStartObject();
			IEnumerator<JToken> enumerator = this.ChildrenTokens.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					((JProperty)enumerator.Current).WriteTo(writer, converters);
				}
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
			writer.WriteEndObject();
		}

		public event PropertyChangedEventHandler PropertyChanged
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.PropertyChanged += value;
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.PropertyChanged -= value;
			}
		}

		public class JPropertKeyedCollection : KeyedCollection<string, JToken>
		{
			public new IDictionary<string, JToken> Dictionary
			{
				get
				{
					return base.Dictionary;
				}
			}

			public JPropertKeyedCollection(IEqualityComparer<string> comparer) : base(comparer)
			{
			}

			protected override string GetKeyForItem(JToken item)
			{
				return ((JProperty)item).Name;
			}

			protected override void InsertItem(int index, JToken item)
			{
				if (this.Dictionary != null)
				{
					string keyForItem = this.GetKeyForItem(item);
					this.Dictionary[keyForItem] = item;
					base.Items.Insert(index, item);
				}
				else
				{
					base.InsertItem(index, item);
				}
			}
		}
	}
}