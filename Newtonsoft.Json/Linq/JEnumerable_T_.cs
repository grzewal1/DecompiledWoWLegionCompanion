using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Newtonsoft.Json.Linq
{
	public struct JEnumerable<T> : IJEnumerable<T>, IEnumerable<T>, IEnumerable
	where T : JToken
	{
		public readonly static JEnumerable<T> Empty;

		private IEnumerable<T> _enumerable;

		public IJEnumerable<JToken> this[object key]
		{
			get
			{
				return new JEnumerable<JToken>(this._enumerable.Values<T, JToken>(key));
			}
		}

		static JEnumerable()
		{
			JEnumerable<T>.Empty = new JEnumerable<T>(Enumerable.Empty<T>());
		}

		public JEnumerable(IEnumerable<T> enumerable)
		{
			ValidationUtils.ArgumentNotNull(enumerable, "enumerable");
			this._enumerable = enumerable;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is JEnumerable<T>))
			{
				return false;
			}
			return this._enumerable.Equals(((JEnumerable<T>)obj)._enumerable);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this._enumerable.GetEnumerator();
		}

		public override int GetHashCode()
		{
			return this._enumerable.GetHashCode();
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}