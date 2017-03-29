using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Newtonsoft.Json.Utilities
{
	internal class ListWrapper<T> : CollectionWrapper<T>, IEnumerable, IList, ICollection, IWrappedList, IList<T>, ICollection<T>, IEnumerable<T>
	{
		private readonly IList<T> _genericList;

		public override int Count
		{
			get
			{
				if (this._genericList == null)
				{
					return base.Count;
				}
				return this._genericList.Count;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				if (this._genericList == null)
				{
					return base.IsReadOnly;
				}
				return this._genericList.IsReadOnly;
			}
		}

		public T this[int index]
		{
			get
			{
				if (this._genericList != null)
				{
					return this._genericList[index];
				}
				return (T)((IList)this)[index];
			}
			set
			{
				if (this._genericList == null)
				{
					((IList)this)[index] = value;
				}
				else
				{
					this._genericList[index] = value;
				}
			}
		}

		public object UnderlyingList
		{
			get
			{
				if (this._genericList != null)
				{
					return this._genericList;
				}
				return this.UnderlyingCollection;
			}
		}

		public ListWrapper(IList list) : base(list)
		{
			ValidationUtils.ArgumentNotNull(list, "list");
			if (list is IList<T>)
			{
				this._genericList = (IList<T>)list;
			}
		}

		public ListWrapper(IList<T> list) : base(list)
		{
			ValidationUtils.ArgumentNotNull(list, "list");
			this._genericList = list;
		}

		public override void Add(T item)
		{
			if (this._genericList == null)
			{
				base.Add(item);
			}
			else
			{
				this._genericList.Add(item);
			}
		}

		public override void Clear()
		{
			if (this._genericList == null)
			{
				base.Clear();
			}
			else
			{
				this._genericList.Clear();
			}
		}

		public override bool Contains(T item)
		{
			if (this._genericList == null)
			{
				return base.Contains(item);
			}
			return this._genericList.Contains(item);
		}

		public override void CopyTo(T[] array, int arrayIndex)
		{
			if (this._genericList == null)
			{
				base.CopyTo(array, arrayIndex);
			}
			else
			{
				this._genericList.CopyTo(array, arrayIndex);
			}
		}

		public override IEnumerator<T> GetEnumerator()
		{
			if (this._genericList == null)
			{
				return base.GetEnumerator();
			}
			return this._genericList.GetEnumerator();
		}

		public int IndexOf(T item)
		{
			if (this._genericList != null)
			{
				return this._genericList.IndexOf(item);
			}
			return ((IList)this).IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			if (this._genericList == null)
			{
				((IList)this).Insert(index, item);
			}
			else
			{
				this._genericList.Insert(index, item);
			}
		}

		public override bool Remove(T item)
		{
			if (this._genericList != null)
			{
				return this._genericList.Remove(item);
			}
			bool flag = base.Contains(item);
			if (flag)
			{
				base.Remove(item);
			}
			return flag;
		}

		public void RemoveAt(int index)
		{
			if (this._genericList == null)
			{
				((IList)this).RemoveAt(index);
			}
			else
			{
				this._genericList.RemoveAt(index);
			}
		}
	}
}