using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Newtonsoft.Json.Linq
{
	public abstract class JContainer : JToken, IEnumerable, IEnumerable<JToken>, ICollection<JToken>, IList<JToken>, IList, ICollection
	{
		private object _syncRoot;

		private bool _busy;

		protected abstract IList<JToken> ChildrenTokens
		{
			get;
		}

		public int Count
		{
			get
			{
				return this.ChildrenTokens.Count;
			}
		}

		public override JToken First
		{
			get
			{
				return this.ChildrenTokens.FirstOrDefault<JToken>();
			}
		}

		public override bool HasValues
		{
			get
			{
				return this.ChildrenTokens.Count > 0;
			}
		}

		public override JToken Last
		{
			get
			{
				return this.ChildrenTokens.LastOrDefault<JToken>();
			}
		}

		bool System.Collections.Generic.ICollection<Newtonsoft.Json.Linq.JToken>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		JToken System.Collections.Generic.IList<Newtonsoft.Json.Linq.JToken>.this[int index]
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

		bool System.Collections.ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object System.Collections.ICollection.SyncRoot
		{
			get
			{
				if (this._syncRoot == null)
				{
					Interlocked.CompareExchange(ref this._syncRoot, new object(), null);
				}
				return this._syncRoot;
			}
		}

		bool System.Collections.IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool System.Collections.IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		object System.Collections.IList.this[int index]
		{
			get
			{
				return this.GetItem(index);
			}
			set
			{
				this.SetItem(index, this.EnsureValue(value));
			}
		}

		internal JContainer()
		{
		}

		internal JContainer(JContainer other)
		{
			ValidationUtils.ArgumentNotNull(other, "c");
			IEnumerator<JToken> enumerator = ((IEnumerable<JToken>)other).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					this.Add(enumerator.Current);
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

		public virtual void Add(object content)
		{
			this.AddInternal(this.ChildrenTokens.Count, content);
		}

		public void AddFirst(object content)
		{
			this.AddInternal(0, content);
		}

		internal void AddInternal(int index, object content)
		{
			if (!this.IsMultiContent(content))
			{
				this.InsertItem(index, this.CreateFromContent(content));
			}
			else
			{
				IEnumerable enumerable = (IEnumerable)content;
				int num = index;
				IEnumerator enumerator = enumerable.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						this.AddInternal(num, enumerator.Current);
						num++;
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable == null)
					{
					}
					disposable.Dispose();
				}
			}
		}

		internal void CheckReentrancy()
		{
			if (this._busy)
			{
				throw new InvalidOperationException("Cannot change {0} during a collection change event.".FormatWith(CultureInfo.InvariantCulture, new object[] { base.GetType() }));
			}
		}

		public override JEnumerable<JToken> Children()
		{
			return new JEnumerable<JToken>(this.ChildrenTokens);
		}

		internal virtual void ClearItems()
		{
			this.CheckReentrancy();
			IEnumerator<JToken> enumerator = this.ChildrenTokens.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					JToken current = enumerator.Current;
					current.Parent = null;
					current.Previous = null;
					current.Next = null;
				}
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
			this.ChildrenTokens.Clear();
		}

		internal virtual bool ContainsItem(JToken item)
		{
			return this.IndexOfItem(item) != -1;
		}

		internal bool ContentsEqual(JContainer container)
		{
			JToken next;
			JToken jTokens;
			JToken first = this.First;
			JToken first1 = container.First;
			if (first == first1)
			{
				return true;
			}
			while (first != null || first1 != null)
			{
				if (first == null || first1 == null || !first.DeepEquals(first1))
				{
					return false;
				}
				if (first == this.Last)
				{
					next = null;
				}
				else
				{
					next = first.Next;
				}
				first = next;
				if (first1 == container.Last)
				{
					jTokens = null;
				}
				else
				{
					jTokens = first1.Next;
				}
				first1 = jTokens;
			}
			return true;
		}

		internal int ContentsHashCode()
		{
			int deepHashCode = 0;
			IEnumerator<JToken> enumerator = this.ChildrenTokens.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					deepHashCode = deepHashCode ^ enumerator.Current.GetDeepHashCode();
				}
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
			return deepHashCode;
		}

		internal virtual void CopyItemsTo(Array array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex", "arrayIndex is less than 0.");
			}
			if (arrayIndex >= array.Length)
			{
				throw new ArgumentException("arrayIndex is equal to or greater than the length of array.");
			}
			if (this.Count > array.Length - arrayIndex)
			{
				throw new ArgumentException("The number of elements in the source JObject is greater than the available space from arrayIndex to the end of the destination array.");
			}
			int num = 0;
			IEnumerator<JToken> enumerator = this.ChildrenTokens.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					array.SetValue(enumerator.Current, arrayIndex + num);
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

		internal JToken CreateFromContent(object content)
		{
			if (content is JToken)
			{
				return (JToken)content;
			}
			return new JValue(content);
		}

		public JsonWriter CreateWriter()
		{
			return new JTokenWriter(this);
		}

		[DebuggerHidden]
		public IEnumerable<JToken> Descendants()
		{
			JContainer.<Descendants>c__Iterator3 variable = null;
			return variable;
		}

		internal JToken EnsureParentToken(JToken item)
		{
			if (item == null)
			{
				return new JValue(null);
			}
			if (item.Parent == null)
			{
				JContainer parent = this;
				while (parent.Parent != null)
				{
					parent = parent.Parent;
				}
				if (item == parent)
				{
					item = item.CloneToken();
				}
			}
			else
			{
				item = item.CloneToken();
			}
			return item;
		}

		private JToken EnsureValue(object value)
		{
			if (value == null)
			{
				return null;
			}
			if (!(value is JToken))
			{
				throw new ArgumentException("Argument is not a JToken.");
			}
			return (JToken)value;
		}

		internal virtual JToken GetItem(int index)
		{
			return this.ChildrenTokens[index];
		}

		internal int IndexOfItem(JToken item)
		{
			return this.ChildrenTokens.IndexOf<JToken>(item, JContainer.JTokenReferenceEqualityComparer.Instance);
		}

		internal virtual void InsertItem(int index, JToken item)
		{
			JToken jTokens;
			JToken jTokens1;
			if (index > this.ChildrenTokens.Count)
			{
				throw new ArgumentOutOfRangeException("index", "Index must be within the bounds of the List.");
			}
			this.CheckReentrancy();
			item = this.EnsureParentToken(item);
			if (index != 0)
			{
				jTokens = this.ChildrenTokens[index - 1];
			}
			else
			{
				jTokens = null;
			}
			JToken jTokens2 = jTokens;
			if (index != this.ChildrenTokens.Count)
			{
				jTokens1 = this.ChildrenTokens[index];
			}
			else
			{
				jTokens1 = null;
			}
			JToken jTokens3 = jTokens1;
			this.ValidateToken(item, null);
			item.Parent = this;
			item.Previous = jTokens2;
			if (jTokens2 != null)
			{
				jTokens2.Next = item;
			}
			item.Next = jTokens3;
			if (jTokens3 != null)
			{
				jTokens3.Previous = item;
			}
			this.ChildrenTokens.Insert(index, item);
		}

		internal bool IsMultiContent(object content)
		{
			return (!(content is IEnumerable) || content is string || content is JToken ? false : !(content is byte[]));
		}

		internal static bool IsTokenUnchanged(JToken currentValue, JToken newValue)
		{
			JValue jValue = currentValue as JValue;
			if (jValue == null)
			{
				return false;
			}
			if (jValue.Type == JTokenType.Null && newValue == null)
			{
				return true;
			}
			return jValue.Equals(newValue);
		}

		internal void ReadContentFrom(JsonReader r)
		{
			JValue jValue;
			object[] tokenType;
			CultureInfo invariantCulture;
			ValidationUtils.ArgumentNotNull(r, "r");
			IJsonLineInfo jsonLineInfo = r as IJsonLineInfo;
			JContainer parent = this;
			do
			{
				if (parent is JProperty && ((JProperty)parent).Value != null)
				{
					if (parent == this)
					{
						return;
					}
					parent = parent.Parent;
				}
				switch (r.TokenType)
				{
					case JsonToken.None:
					{
						continue;
					}
					case JsonToken.StartObject:
					{
						JObject jObjects = new JObject();
						jObjects.SetLineInfo(jsonLineInfo);
						parent.Add(jObjects);
						parent = jObjects;
						continue;
					}
					case JsonToken.StartArray:
					{
						JArray jArrays = new JArray();
						jArrays.SetLineInfo(jsonLineInfo);
						parent.Add(jArrays);
						parent = jArrays;
						continue;
					}
					case JsonToken.StartConstructor:
					{
						JConstructor jConstructor = new JConstructor(r.Value.ToString());
						jConstructor.SetLineInfo(jConstructor);
						parent.Add(jConstructor);
						parent = jConstructor;
						continue;
					}
					case JsonToken.PropertyName:
					{
						string str = r.Value.ToString();
						JProperty jProperty = new JProperty(str);
						jProperty.SetLineInfo(jsonLineInfo);
						JProperty jProperty1 = ((JObject)parent).Property(str);
						if (jProperty1 != null)
						{
							jProperty1.Replace(jProperty);
						}
						else
						{
							parent.Add(jProperty);
						}
						parent = jProperty;
						continue;
					}
					case JsonToken.Comment:
					{
						jValue = JValue.CreateComment(r.Value.ToString());
						jValue.SetLineInfo(jsonLineInfo);
						parent.Add(jValue);
						continue;
					}
					case JsonToken.Raw:
					{
						invariantCulture = CultureInfo.InvariantCulture;
						tokenType = new object[] { r.TokenType };
						throw new InvalidOperationException("The JsonReader should not be on a token of type {0}.".FormatWith(invariantCulture, tokenType));
					}
					case JsonToken.Integer:
					case JsonToken.Float:
					case JsonToken.String:
					case JsonToken.Boolean:
					case JsonToken.Date:
					case JsonToken.Bytes:
					{
						jValue = new JValue(r.Value);
						jValue.SetLineInfo(jsonLineInfo);
						parent.Add(jValue);
						continue;
					}
					case JsonToken.Null:
					{
						jValue = new JValue(null, JTokenType.Null);
						jValue.SetLineInfo(jsonLineInfo);
						parent.Add(jValue);
						continue;
					}
					case JsonToken.Undefined:
					{
						jValue = new JValue(null, JTokenType.Undefined);
						jValue.SetLineInfo(jsonLineInfo);
						parent.Add(jValue);
						continue;
					}
					case JsonToken.EndObject:
					{
						if (parent == this)
						{
							return;
						}
						parent = parent.Parent;
						continue;
					}
					case JsonToken.EndArray:
					{
						if (parent == this)
						{
							return;
						}
						parent = parent.Parent;
						continue;
					}
					case JsonToken.EndConstructor:
					{
						if (parent == this)
						{
							return;
						}
						parent = parent.Parent;
						continue;
					}
					default:
					{
						invariantCulture = CultureInfo.InvariantCulture;
						tokenType = new object[] { r.TokenType };
						throw new InvalidOperationException("The JsonReader should not be on a token of type {0}.".FormatWith(invariantCulture, tokenType));
					}
				}
			}
			while (r.Read());
		}

		internal void ReadTokenFrom(JsonReader r)
		{
			int depth = r.Depth;
			if (!r.Read())
			{
				throw new Exception("Error reading {0} from JsonReader.".FormatWith(CultureInfo.InvariantCulture, new object[] { base.GetType().Name }));
			}
			this.ReadContentFrom(r);
			if (r.Depth > depth)
			{
				throw new Exception("Unexpected end of content while loading {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { base.GetType().Name }));
			}
		}

		public void RemoveAll()
		{
			this.ClearItems();
		}

		internal virtual bool RemoveItem(JToken item)
		{
			int num = this.IndexOfItem(item);
			if (num < 0)
			{
				return false;
			}
			this.RemoveItemAt(num);
			return true;
		}

		internal virtual void RemoveItemAt(int index)
		{
			JToken item;
			JToken jTokens;
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "Index is less than 0.");
			}
			if (index >= this.ChildrenTokens.Count)
			{
				throw new ArgumentOutOfRangeException("index", "Index is equal to or greater than Count.");
			}
			this.CheckReentrancy();
			JToken item1 = this.ChildrenTokens[index];
			if (index != 0)
			{
				item = this.ChildrenTokens[index - 1];
			}
			else
			{
				item = null;
			}
			JToken jTokens1 = item;
			if (index != this.ChildrenTokens.Count - 1)
			{
				jTokens = this.ChildrenTokens[index + 1];
			}
			else
			{
				jTokens = null;
			}
			JToken jTokens2 = jTokens;
			if (jTokens1 != null)
			{
				jTokens1.Next = jTokens2;
			}
			if (jTokens2 != null)
			{
				jTokens2.Previous = jTokens1;
			}
			item1.Parent = null;
			item1.Previous = null;
			item1.Next = null;
			this.ChildrenTokens.RemoveAt(index);
		}

		public void ReplaceAll(object content)
		{
			this.ClearItems();
			this.Add(content);
		}

		internal virtual void ReplaceItem(JToken existing, JToken replacement)
		{
			if (existing == null || existing.Parent != this)
			{
				return;
			}
			this.SetItem(this.IndexOfItem(existing), replacement);
		}

		internal virtual void SetItem(int index, JToken item)
		{
			JToken jTokens;
			JToken jTokens1;
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "Index is less than 0.");
			}
			if (index >= this.ChildrenTokens.Count)
			{
				throw new ArgumentOutOfRangeException("index", "Index is equal to or greater than Count.");
			}
			JToken jTokens2 = this.ChildrenTokens[index];
			if (JContainer.IsTokenUnchanged(jTokens2, item))
			{
				return;
			}
			this.CheckReentrancy();
			item = this.EnsureParentToken(item);
			this.ValidateToken(item, jTokens2);
			if (index != 0)
			{
				jTokens = this.ChildrenTokens[index - 1];
			}
			else
			{
				jTokens = null;
			}
			JToken jTokens3 = jTokens;
			if (index != this.ChildrenTokens.Count - 1)
			{
				jTokens1 = this.ChildrenTokens[index + 1];
			}
			else
			{
				jTokens1 = null;
			}
			JToken jTokens4 = jTokens1;
			item.Parent = this;
			item.Previous = jTokens3;
			if (jTokens3 != null)
			{
				jTokens3.Next = item;
			}
			item.Next = jTokens4;
			if (jTokens4 != null)
			{
				jTokens4.Previous = item;
			}
			this.ChildrenTokens[index] = item;
			jTokens2.Parent = null;
			jTokens2.Previous = null;
			jTokens2.Next = null;
		}

		void System.Collections.Generic.ICollection<Newtonsoft.Json.Linq.JToken>.Add(JToken item)
		{
			this.Add(item);
		}

		void System.Collections.Generic.ICollection<Newtonsoft.Json.Linq.JToken>.Clear()
		{
			this.ClearItems();
		}

		bool System.Collections.Generic.ICollection<Newtonsoft.Json.Linq.JToken>.Contains(JToken item)
		{
			return this.ContainsItem(item);
		}

		void System.Collections.Generic.ICollection<Newtonsoft.Json.Linq.JToken>.CopyTo(JToken[] array, int arrayIndex)
		{
			this.CopyItemsTo(array, arrayIndex);
		}

		bool System.Collections.Generic.ICollection<Newtonsoft.Json.Linq.JToken>.Remove(JToken item)
		{
			return this.RemoveItem(item);
		}

		int System.Collections.Generic.IList<Newtonsoft.Json.Linq.JToken>.IndexOf(JToken item)
		{
			return this.IndexOfItem(item);
		}

		void System.Collections.Generic.IList<Newtonsoft.Json.Linq.JToken>.Insert(int index, JToken item)
		{
			this.InsertItem(index, item);
		}

		void System.Collections.Generic.IList<Newtonsoft.Json.Linq.JToken>.RemoveAt(int index)
		{
			this.RemoveItemAt(index);
		}

		void System.Collections.ICollection.CopyTo(Array array, int index)
		{
			this.CopyItemsTo(array, index);
		}

		int System.Collections.IList.Add(object value)
		{
			this.Add(this.EnsureValue(value));
			return this.Count - 1;
		}

		void System.Collections.IList.Clear()
		{
			this.ClearItems();
		}

		bool System.Collections.IList.Contains(object value)
		{
			return this.ContainsItem(this.EnsureValue(value));
		}

		int System.Collections.IList.IndexOf(object value)
		{
			return this.IndexOfItem(this.EnsureValue(value));
		}

		void System.Collections.IList.Insert(int index, object value)
		{
			this.InsertItem(index, this.EnsureValue(value));
		}

		void System.Collections.IList.Remove(object value)
		{
			this.RemoveItem(this.EnsureValue(value));
		}

		void System.Collections.IList.RemoveAt(int index)
		{
			this.RemoveItemAt(index);
		}

		internal virtual void ValidateToken(JToken o, JToken existing)
		{
			ValidationUtils.ArgumentNotNull(o, "o");
			if (o.Type == JTokenType.Property)
			{
				throw new ArgumentException("Can not add {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, new object[] { o.GetType(), base.GetType() }));
			}
		}

		public override IEnumerable<T> Values<T>()
		{
			return this.ChildrenTokens.Convert<JToken, T>();
		}

		private class JTokenReferenceEqualityComparer : IEqualityComparer<JToken>
		{
			public readonly static JContainer.JTokenReferenceEqualityComparer Instance;

			static JTokenReferenceEqualityComparer()
			{
				JContainer.JTokenReferenceEqualityComparer.Instance = new JContainer.JTokenReferenceEqualityComparer();
			}

			public JTokenReferenceEqualityComparer()
			{
			}

			public bool Equals(JToken x, JToken y)
			{
				return object.ReferenceEquals(x, y);
			}

			public int GetHashCode(JToken obj)
			{
				if (obj == null)
				{
					return 0;
				}
				return obj.GetHashCode();
			}
		}
	}
}