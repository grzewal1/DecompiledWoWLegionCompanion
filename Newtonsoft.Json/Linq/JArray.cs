using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Newtonsoft.Json.Linq
{
	public class JArray : JContainer, IList<JToken>, ICollection<JToken>, IEnumerable<JToken>, IEnumerable
	{
		private IList<JToken> _values = new List<JToken>();

		protected override IList<JToken> ChildrenTokens
		{
			get
			{
				return this._values;
			}
		}

		public override JToken this[object key]
		{
			get
			{
				ValidationUtils.ArgumentNotNull(key, "o");
				if (!(key is int))
				{
					throw new ArgumentException("Accessed JArray values with invalid key value: {0}. Array position index expected.".FormatWith(CultureInfo.InvariantCulture, new object[] { MiscellaneousUtils.ToString(key) }));
				}
				return this.GetItem((int)key);
			}
			set
			{
				ValidationUtils.ArgumentNotNull(key, "o");
				if (!(key is int))
				{
					throw new ArgumentException("Set JArray values with invalid key value: {0}. Array position index expected.".FormatWith(CultureInfo.InvariantCulture, new object[] { MiscellaneousUtils.ToString(key) }));
				}
				this.SetItem((int)key, value);
			}
		}

		public JToken this[int index]
		{
			get
			{
				return this.GetItem(index);
			}
			set
			{
				this.SetItem(index, value);
			}
		}

		bool System.Collections.Generic.ICollection<Newtonsoft.Json.Linq.JToken>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public override JTokenType Type
		{
			get
			{
				return JTokenType.Array;
			}
		}

		public JArray()
		{
		}

		public JArray(JArray other) : base(other)
		{
		}

		public JArray(params object[] content) : this((object)content)
		{
		}

		public JArray(object content)
		{
			this.Add(content);
		}

		public void Add(JToken item)
		{
			this.Add(item);
		}

		public void Clear()
		{
			this.ClearItems();
		}

		internal override JToken CloneToken()
		{
			return new JArray(this);
		}

		public bool Contains(JToken item)
		{
			return this.ContainsItem(item);
		}

		internal override bool DeepEquals(JToken node)
		{
			JArray jArrays = node as JArray;
			return (jArrays == null ? false : base.ContentsEqual(jArrays));
		}

		public static new JArray FromObject(object o)
		{
			return JArray.FromObject(o, new JsonSerializer());
		}

		public static new JArray FromObject(object o, JsonSerializer jsonSerializer)
		{
			JToken jTokens = JToken.FromObjectInternal(o, jsonSerializer);
			if (jTokens.Type != JTokenType.Array)
			{
				throw new ArgumentException("Object serialized to {0}. JArray instance expected.".FormatWith(CultureInfo.InvariantCulture, new object[] { jTokens.Type }));
			}
			return (JArray)jTokens;
		}

		internal override int GetDeepHashCode()
		{
			return base.ContentsHashCode();
		}

		public int IndexOf(JToken item)
		{
			return base.IndexOfItem(item);
		}

		public void Insert(int index, JToken item)
		{
			this.InsertItem(index, item);
		}

		public static new JArray Load(JsonReader reader)
		{
			if (reader.TokenType == JsonToken.None && !reader.Read())
			{
				throw new Exception("Error reading JArray from JsonReader.");
			}
			if (reader.TokenType != JsonToken.StartArray)
			{
				throw new Exception("Error reading JArray from JsonReader. Current JsonReader item is not an array: {0}".FormatWith(CultureInfo.InvariantCulture, new object[] { reader.TokenType }));
			}
			JArray jArrays = new JArray();
			jArrays.SetLineInfo(reader as IJsonLineInfo);
			jArrays.ReadTokenFrom(reader);
			return jArrays;
		}

		public static new JArray Parse(string json)
		{
			return JArray.Load(new JsonTextReader(new StringReader(json)));
		}

		public bool Remove(JToken item)
		{
			return this.RemoveItem(item);
		}

		public void RemoveAt(int index)
		{
			this.RemoveItemAt(index);
		}

		void System.Collections.Generic.ICollection<Newtonsoft.Json.Linq.JToken>.CopyTo(JToken[] array, int arrayIndex)
		{
			this.CopyItemsTo(array, arrayIndex);
		}

		public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
		{
			writer.WriteStartArray();
			foreach (JToken childrenToken in this.ChildrenTokens)
			{
				childrenToken.WriteTo(writer, converters);
			}
			writer.WriteEndArray();
		}
	}
}