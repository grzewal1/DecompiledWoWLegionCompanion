using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace bgs
{
	public class Map<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
	{
		private const int INITIAL_SIZE = 4;

		private const float DEFAULT_LOAD_FACTOR = 0.9f;

		private const int NO_SLOT = -1;

		private const int HASH_FLAG = -2147483648;

		private int[] table;

		private bgs.Link[] linkSlots;

		private TKey[] keySlots;

		private TValue[] valueSlots;

		private IEqualityComparer<TKey> hcp;

		private int touchedSlots;

		private int emptySlot;

		private int count;

		private int threshold;

		private int generation;

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				int hashCode = this.hcp.GetHashCode(key) | -2147483648;
				for (int i = this.table[(hashCode & 2147483647) % (int)this.table.Length] - 1; i != -1; i = this.linkSlots[i].Next)
				{
					if (this.linkSlots[i].HashCode == hashCode && this.hcp.Equals(this.keySlots[i], key))
					{
						return this.valueSlots[i];
					}
				}
				throw new KeyNotFoundException();
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				int hashCode = this.hcp.GetHashCode(key) | -2147483648;
				int length = (hashCode & 2147483647) % (int)this.table.Length;
				int next = this.table[length] - 1;
				int num = -1;
				if (next != -1)
				{
					while (this.linkSlots[next].HashCode != hashCode || !this.hcp.Equals(this.keySlots[next], key))
					{
						num = next;
						next = this.linkSlots[next].Next;
						if (next == -1)
						{
							goto Label0;
						}
					}
				}
				if (next == -1)
				{
					Map<TKey, TValue> map = this;
					int num1 = map.count + 1;
					int num2 = num1;
					map.count = num1;
					if (num2 > this.threshold)
					{
						this.Resize();
						length = (hashCode & 2147483647) % (int)this.table.Length;
					}
					next = this.emptySlot;
					if (next != -1)
					{
						this.emptySlot = this.linkSlots[next].Next;
					}
					else
					{
						Map<TKey, TValue> map1 = this;
						int num3 = map1.touchedSlots;
						num2 = num3;
						map1.touchedSlots = num3 + 1;
						next = num2;
					}
					this.linkSlots[next].Next = this.table[length] - 1;
					this.table[length] = next + 1;
					this.linkSlots[next].HashCode = hashCode;
					this.keySlots[next] = key;
				}
				else if (num != -1)
				{
					this.linkSlots[num].Next = this.linkSlots[next].Next;
					this.linkSlots[next].Next = this.table[length] - 1;
					this.table[length] = next + 1;
				}
				this.valueSlots[next] = value;
				this.generation++;
			}
		}

		public Map<TKey, TValue>.KeyCollection Keys
		{
			get
			{
				return new Map<TKey, TValue>.KeyCollection(this);
			}
		}

		public Map<TKey, TValue>.ValueCollection Values
		{
			get
			{
				return new Map<TKey, TValue>.ValueCollection(this);
			}
		}

		public Map()
		{
			this.Init(4, null);
		}

		public Map(int count)
		{
			this.Init(count, null);
		}

		public Map(IEqualityComparer<TKey> comparer)
		{
			this.Init(4, comparer);
		}

		public void Add(TKey key, TValue value)
		{
			int i;
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int hashCode = this.hcp.GetHashCode(key) | -2147483648;
			int length = (hashCode & 2147483647) % (int)this.table.Length;
			for (i = this.table[length] - 1; i != -1; i = this.linkSlots[i].Next)
			{
				if (this.linkSlots[i].HashCode == hashCode && this.hcp.Equals(this.keySlots[i], key))
				{
					throw new ArgumentException("An element with the same key already exists in the dictionary.");
				}
			}
			Map<TKey, TValue> map = this;
			int num = map.count + 1;
			int num1 = num;
			map.count = num;
			if (num1 > this.threshold)
			{
				this.Resize();
				length = (hashCode & 2147483647) % (int)this.table.Length;
			}
			i = this.emptySlot;
			if (i != -1)
			{
				this.emptySlot = this.linkSlots[i].Next;
			}
			else
			{
				Map<TKey, TValue> map1 = this;
				int num2 = map1.touchedSlots;
				num1 = num2;
				map1.touchedSlots = num2 + 1;
				i = num1;
			}
			this.linkSlots[i].HashCode = hashCode;
			this.linkSlots[i].Next = this.table[length] - 1;
			this.table[length] = i + 1;
			this.keySlots[i] = key;
			this.valueSlots[i] = value;
			this.generation++;
		}

		public void Clear()
		{
			if (this.count == 0)
			{
				return;
			}
			this.count = 0;
			Array.Clear(this.table, 0, (int)this.table.Length);
			Array.Clear(this.keySlots, 0, (int)this.keySlots.Length);
			Array.Clear(this.valueSlots, 0, (int)this.valueSlots.Length);
			Array.Clear(this.linkSlots, 0, (int)this.linkSlots.Length);
			this.emptySlot = -1;
			this.touchedSlots = 0;
			this.generation++;
		}

		public bool ContainsKey(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int hashCode = this.hcp.GetHashCode(key) | -2147483648;
			for (int i = this.table[(hashCode & 2147483647) % (int)this.table.Length] - 1; i != -1; i = this.linkSlots[i].Next)
			{
				if (this.linkSlots[i].HashCode == hashCode && this.hcp.Equals(this.keySlots[i], key))
				{
					return true;
				}
			}
			return false;
		}

		public bool ContainsValue(TValue value)
		{
			IEqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
			for (int i = 0; i < (int)this.table.Length; i++)
			{
				for (int j = this.table[i] - 1; j != -1; j = this.linkSlots[j].Next)
				{
					if (@default.Equals(this.valueSlots[j], value))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void CopyKeys(TKey[] array, int index)
		{
			for (int i = 0; i < this.touchedSlots; i++)
			{
				if ((this.linkSlots[i].HashCode & -2147483648) != 0)
				{
					int num = index;
					index = num + 1;
					array[num] = this.keySlots[i];
				}
			}
		}

		private void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
		{
			this.CopyToCheck(array, index);
			for (int i = 0; i < this.touchedSlots; i++)
			{
				if ((this.linkSlots[i].HashCode & -2147483648) != 0)
				{
					int num = index;
					index = num + 1;
					array[num] = new KeyValuePair<TKey, TValue>(this.keySlots[i], this.valueSlots[i]);
				}
			}
		}

		private void CopyToCheck(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (index > array.Length)
			{
				throw new ArgumentException("index larger than largest valid index of array");
			}
			if (array.Length - index < this.Count)
			{
				throw new ArgumentException("Destination array cannot hold the requested elements!");
			}
		}

		private void CopyValues(TValue[] array, int index)
		{
			for (int i = 0; i < this.touchedSlots; i++)
			{
				if ((this.linkSlots[i].HashCode & -2147483648) != 0)
				{
					int num = index;
					index = num + 1;
					array[num] = this.valueSlots[i];
				}
			}
		}

		private void Do_ICollectionCopyTo<TRet>(Array array, int index, Map<TKey, TValue>.Transform<TRet> transform)
		{
			Type type = typeof(TRet);
			Type elementType = array.GetType().GetElementType();
			try
			{
				if ((type.IsPrimitive || elementType.IsPrimitive) && !elementType.IsAssignableFrom(type))
				{
					throw new Exception();
				}
				object[] objArray = (object[])array;
				for (int i = 0; i < this.touchedSlots; i++)
				{
					if ((this.linkSlots[i].HashCode & -2147483648) != 0)
					{
						int num = index;
						index = num + 1;
						objArray[num] = transform(this.keySlots[i], this.valueSlots[i]);
					}
				}
			}
			catch (Exception exception)
			{
				throw new ArgumentException("Cannot copy source collection elements to destination array", "array", exception);
			}
		}

		public Map<TKey, TValue>.Enumerator GetEnumerator()
		{
			return new Map<TKey, TValue>.Enumerator(this);
		}

		private void Init(int capacity, IEqualityComparer<TKey> hcp)
		{
			object @default = hcp;
			if (@default == null)
			{
				@default = EqualityComparer<TKey>.Default;
			}
			this.hcp = (IEqualityComparer<TKey>)@default;
			capacity = Math.Max(1, (int)((float)capacity / 0.9f));
			this.InitArrays(capacity);
		}

		private void InitArrays(int size)
		{
			this.table = new int[size];
			this.linkSlots = new bgs.Link[size];
			this.emptySlot = -1;
			this.keySlots = new TKey[size];
			this.valueSlots = new TValue[size];
			this.touchedSlots = 0;
			this.threshold = (int)((float)((int)this.table.Length) * 0.9f);
			if (this.threshold == 0 && (int)this.table.Length > 0)
			{
				this.threshold = 1;
			}
		}

		private static KeyValuePair<TKey, TValue> make_pair(TKey key, TValue value)
		{
			return new KeyValuePair<TKey, TValue>(key, value);
		}

		private static TKey pick_key(TKey key, TValue value)
		{
			return key;
		}

		private static TValue pick_value(TKey key, TValue value)
		{
			return value;
		}

		public bool Remove(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int hashCode = this.hcp.GetHashCode(key) | -2147483648;
			int length = (hashCode & 2147483647) % (int)this.table.Length;
			int next = this.table[length] - 1;
			if (next == -1)
			{
				return false;
			}
			int num = -1;
			do
			{
				if (this.linkSlots[next].HashCode != hashCode || !this.hcp.Equals(this.keySlots[next], key))
				{
					num = next;
					next = this.linkSlots[next].Next;
				}
				else
				{
					break;
				}
			}
			while (next != -1);
			if (next == -1)
			{
				return false;
			}
			this.count--;
			if (num != -1)
			{
				this.linkSlots[num].Next = this.linkSlots[next].Next;
			}
			else
			{
				this.table[length] = this.linkSlots[next].Next + 1;
			}
			this.linkSlots[next].Next = this.emptySlot;
			this.emptySlot = next;
			this.linkSlots[next].HashCode = 0;
			this.keySlots[next] = default(TKey);
			this.valueSlots[next] = default(TValue);
			this.generation++;
			return true;
		}

		private void Resize()
		{
			int prime = HashPrimeNumbers.ToPrime((int)this.table.Length << 1 | 1);
			int[] numArray = new int[prime];
			bgs.Link[] linkArray = new bgs.Link[prime];
			for (int i = 0; i < (int)this.table.Length; i++)
			{
				for (int j = this.table[i] - 1; j != -1; j = this.linkSlots[j].Next)
				{
					int hashCode = this.hcp.GetHashCode(this.keySlots[j]) | -2147483648;
					int num = hashCode;
					linkArray[j].HashCode = hashCode;
					int num1 = (num & 2147483647) % prime;
					linkArray[j].Next = numArray[num1] - 1;
					numArray[num1] = j + 1;
				}
			}
			this.table = numArray;
			this.linkSlots = linkArray;
			TKey[] tKeyArray = new TKey[prime];
			TValue[] tValueArray = new TValue[prime];
			Array.Copy(this.keySlots, 0, tKeyArray, 0, this.touchedSlots);
			Array.Copy(this.valueSlots, 0, tValueArray, 0, this.touchedSlots);
			this.keySlots = tKeyArray;
			this.valueSlots = tValueArray;
			this.threshold = (int)((float)prime * 0.9f);
		}

		IEnumerator<KeyValuePair<TKey, TValue>> System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey,TValue>>.GetEnumerator()
		{
			return new Map<TKey, TValue>.Enumerator(this);
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return new Map<TKey, TValue>.Enumerator(this);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int hashCode = this.hcp.GetHashCode(key) | -2147483648;
			for (int i = this.table[(hashCode & 2147483647) % (int)this.table.Length] - 1; i != -1; i = this.linkSlots[i].Next)
			{
				if (this.linkSlots[i].HashCode == hashCode && this.hcp.Equals(this.keySlots[i], key))
				{
					value = this.valueSlots[i];
					return true;
				}
			}
			value = default(TValue);
			return false;
		}

		public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator, IDisposable
		{
			private Map<TKey, TValue> dictionary;

			private int next;

			private int stamp;

			internal KeyValuePair<TKey, TValue> current;

			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					return this.current;
				}
			}

			internal TKey CurrentKey
			{
				get
				{
					this.VerifyCurrent();
					return this.current.Key;
				}
			}

			internal TValue CurrentValue
			{
				get
				{
					this.VerifyCurrent();
					return this.current.Value;
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					this.VerifyCurrent();
					return this.current;
				}
			}

			internal Enumerator(Map<TKey, TValue> dictionary)
			{
				this = new Map<TKey, TValue>.Enumerator()
				{
					dictionary = dictionary,
					stamp = dictionary.generation
				};
			}

			public void Dispose()
			{
				this.dictionary = null;
			}

			public bool MoveNext()
			{
				this.VerifyState();
				if (this.next < 0)
				{
					return false;
				}
				while (this.next < this.dictionary.touchedSlots)
				{
					Map<TKey, TValue>.Enumerator enumerator = this;
					int num = enumerator.next;
					int num1 = num;
					enumerator.next = num + 1;
					int num2 = num1;
					if ((this.dictionary.linkSlots[num2].HashCode & -2147483648) == 0)
					{
						continue;
					}
					this.current = new KeyValuePair<TKey, TValue>(this.dictionary.keySlots[num2], this.dictionary.valueSlots[num2]);
					return true;
				}
				this.next = -1;
				return false;
			}

			internal void Reset()
			{
				this.VerifyState();
				this.next = 0;
			}

			void System.Collections.IEnumerator.Reset()
			{
				this.Reset();
			}

			private void VerifyCurrent()
			{
				this.VerifyState();
				if (this.next <= 0)
				{
					throw new InvalidOperationException("Current is not valid");
				}
			}

			private void VerifyState()
			{
				if (this.dictionary == null)
				{
					throw new ObjectDisposedException(null);
				}
				if (this.dictionary.generation != this.stamp)
				{
					throw new InvalidOperationException("out of sync");
				}
			}
		}

		public sealed class KeyCollection : ICollection<TKey>, IEnumerable<TKey>, ICollection, IEnumerable
		{
			private Map<TKey, TValue> dictionary;

			public int Count
			{
				get
				{
					return this.dictionary.Count;
				}
			}

			bool System.Collections.Generic.ICollection<TKey>.IsReadOnly
			{
				get
				{
					return true;
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
					return ((ICollection)this.dictionary).SyncRoot;
				}
			}

			public KeyCollection(Map<TKey, TValue> dictionary)
			{
				if (dictionary == null)
				{
					throw new ArgumentNullException("dictionary");
				}
				this.dictionary = dictionary;
			}

			public void CopyTo(TKey[] array, int index)
			{
				this.dictionary.CopyToCheck(array, index);
				this.dictionary.CopyKeys(array, index);
			}

			public Map<TKey, TValue>.KeyCollection.Enumerator GetEnumerator()
			{
				return new Map<TKey, TValue>.KeyCollection.Enumerator(this.dictionary);
			}

			void System.Collections.Generic.ICollection<TKey>.Add(TKey item)
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			void System.Collections.Generic.ICollection<TKey>.Clear()
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			bool System.Collections.Generic.ICollection<TKey>.Contains(TKey item)
			{
				return this.dictionary.ContainsKey(item);
			}

			bool System.Collections.Generic.ICollection<TKey>.Remove(TKey item)
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			IEnumerator<TKey> System.Collections.Generic.IEnumerable<TKey>.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			void System.Collections.ICollection.CopyTo(Array array, int index)
			{
				TKey[] tKeyArray = array as TKey[];
				if (tKeyArray != null)
				{
					this.CopyTo(tKeyArray, index);
					return;
				}
				this.dictionary.CopyToCheck(array, index);
				this.dictionary.Do_ICollectionCopyTo<TKey>(array, index, new Map<TKey, TValue>.Transform<TKey>(Map<TKey, TValue>.pick_key));
			}

			IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			public struct Enumerator : IEnumerator<TKey>, IDisposable, IEnumerator
			{
				private Map<TKey, TValue>.Enumerator host_enumerator;

				public TKey Current
				{
					get
					{
						return this.host_enumerator.current.Key;
					}
				}

				object System.Collections.IEnumerator.Current
				{
					get
					{
						return this.host_enumerator.CurrentKey;
					}
				}

				internal Enumerator(Map<TKey, TValue> host)
				{
					this.host_enumerator = host.GetEnumerator();
				}

				public void Dispose()
				{
					this.host_enumerator.Dispose();
				}

				public bool MoveNext()
				{
					return this.host_enumerator.MoveNext();
				}

				void System.Collections.IEnumerator.Reset()
				{
					this.host_enumerator.Reset();
				}
			}
		}

		private delegate TRet Transform<TRet>(TKey key, TValue value);

		public sealed class ValueCollection : ICollection<TValue>, IEnumerable<TValue>, ICollection, IEnumerable
		{
			private Map<TKey, TValue> dictionary;

			public int Count
			{
				get
				{
					return this.dictionary.Count;
				}
			}

			bool System.Collections.Generic.ICollection<TValue>.IsReadOnly
			{
				get
				{
					return true;
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
					return ((ICollection)this.dictionary).SyncRoot;
				}
			}

			public ValueCollection(Map<TKey, TValue> dictionary)
			{
				if (dictionary == null)
				{
					throw new ArgumentNullException("dictionary");
				}
				this.dictionary = dictionary;
			}

			public void CopyTo(TValue[] array, int index)
			{
				this.dictionary.CopyToCheck(array, index);
				this.dictionary.CopyValues(array, index);
			}

			public Map<TKey, TValue>.ValueCollection.Enumerator GetEnumerator()
			{
				return new Map<TKey, TValue>.ValueCollection.Enumerator(this.dictionary);
			}

			void System.Collections.Generic.ICollection<TValue>.Add(TValue item)
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			void System.Collections.Generic.ICollection<TValue>.Clear()
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			bool System.Collections.Generic.ICollection<TValue>.Contains(TValue item)
			{
				return this.dictionary.ContainsValue(item);
			}

			bool System.Collections.Generic.ICollection<TValue>.Remove(TValue item)
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			IEnumerator<TValue> System.Collections.Generic.IEnumerable<TValue>.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			void System.Collections.ICollection.CopyTo(Array array, int index)
			{
				TValue[] tValueArray = array as TValue[];
				if (tValueArray != null)
				{
					this.CopyTo(tValueArray, index);
					return;
				}
				this.dictionary.CopyToCheck(array, index);
				this.dictionary.Do_ICollectionCopyTo<TValue>(array, index, new Map<TKey, TValue>.Transform<TValue>(Map<TKey, TValue>.pick_value));
			}

			IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			public struct Enumerator : IEnumerator<TValue>, IDisposable, IEnumerator
			{
				private Map<TKey, TValue>.Enumerator host_enumerator;

				public TValue Current
				{
					get
					{
						return this.host_enumerator.current.Value;
					}
				}

				object System.Collections.IEnumerator.Current
				{
					get
					{
						return this.host_enumerator.CurrentValue;
					}
				}

				internal Enumerator(Map<TKey, TValue> host)
				{
					this.host_enumerator = host.GetEnumerator();
				}

				public void Dispose()
				{
					this.host_enumerator.Dispose();
				}

				public bool MoveNext()
				{
					return this.host_enumerator.MoveNext();
				}

				void System.Collections.IEnumerator.Reset()
				{
					this.host_enumerator.Reset();
				}
			}
		}
	}
}