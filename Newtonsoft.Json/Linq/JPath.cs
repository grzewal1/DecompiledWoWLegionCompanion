using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Linq
{
	internal class JPath
	{
		private readonly string _expression;

		private int _currentIndex;

		public List<object> Parts
		{
			get;
			private set;
		}

		public JPath(string expression)
		{
			ValidationUtils.ArgumentNotNull(expression, "expression");
			this._expression = expression;
			this.Parts = new List<object>();
			this.ParseMain();
		}

		internal JToken Evaluate(JToken root, bool errorWhenNoMatch)
		{
			JToken jTokens;
			JToken item = root;
			List<object>.Enumerator enumerator = this.Parts.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					string str = current as string;
					if (str == null)
					{
						int num = (int)current;
						JArray jArrays = item as JArray;
						if (jArrays == null)
						{
							if (errorWhenNoMatch)
							{
								throw new Exception("Index {0} not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, new object[] { num, item.GetType().Name }));
							}
							jTokens = null;
							return jTokens;
						}
						else if (jArrays.Count > num)
						{
							item = jArrays[num];
						}
						else
						{
							if (errorWhenNoMatch)
							{
								throw new IndexOutOfRangeException("Index {0} outside the bounds of JArray.".FormatWith(CultureInfo.InvariantCulture, new object[] { num }));
							}
							jTokens = null;
							return jTokens;
						}
					}
					else
					{
						JObject jObjects = item as JObject;
						if (jObjects == null)
						{
							if (errorWhenNoMatch)
							{
								throw new Exception("Property '{0}' not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, new object[] { str, item.GetType().Name }));
							}
							jTokens = null;
							return jTokens;
						}
						else
						{
							item = jObjects[str];
							if (item == null && errorWhenNoMatch)
							{
								throw new Exception("Property '{0}' does not exist on JObject.".FormatWith(CultureInfo.InvariantCulture, new object[] { str }));
							}
						}
					}
				}
				return item;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return jTokens;
		}

		private void ParseIndexer(char indexerOpenChar)
		{
			this._currentIndex++;
			char chr = (indexerOpenChar != '[' ? ')' : ']');
			int num = this._currentIndex;
			int num1 = 0;
			bool flag = false;
			while (this._currentIndex < this._expression.Length)
			{
				char chr1 = this._expression[this._currentIndex];
				if (!char.IsDigit(chr1))
				{
					if (chr1 != chr)
					{
						throw new Exception(string.Concat("Unexpected character while parsing path indexer: ", chr1));
					}
					flag = true;
					break;
				}
				else
				{
					num1++;
					this._currentIndex++;
				}
			}
			if (!flag)
			{
				throw new Exception(string.Concat("Path ended with open indexer. Expected ", chr));
			}
			if (num1 == 0)
			{
				throw new Exception("Empty path indexer.");
			}
			string str = this._expression.Substring(num, num1);
			this.Parts.Add(Convert.ToInt32(str, CultureInfo.InvariantCulture));
		}

		private void ParseMain()
		{
			char chr;
			int num = this._currentIndex;
			bool flag = false;
			while (true)
			{
				if (this._currentIndex >= this._expression.Length)
				{
					if (this._currentIndex > num)
					{
						string str = this._expression.Substring(num, this._currentIndex - num);
						this.Parts.Add(str);
					}
					return;
				}
				chr = this._expression[this._currentIndex];
				if (chr != '(')
				{
					if (chr == ')')
					{
						break;
					}
					switch (chr)
					{
						case '[':
						{
							break;
						}
						case ']':
						{
							throw new Exception(string.Concat("Unexpected character while parsing path: ", chr));
						}
						default:
						{
							if (chr == '.')
							{
								if (this._currentIndex > num)
								{
									string str1 = this._expression.Substring(num, this._currentIndex - num);
									this.Parts.Add(str1);
								}
								num = this._currentIndex + 1;
								flag = false;
								goto Label1;
							}
							else
							{
								if (flag)
								{
									throw new Exception(string.Concat("Unexpected character following indexer: ", chr));
								}
								goto Label1;
							}
						}
					}
				}
				if (this._currentIndex > num)
				{
					string str2 = this._expression.Substring(num, this._currentIndex - num);
					this.Parts.Add(str2);
				}
				this.ParseIndexer(chr);
				num = this._currentIndex + 1;
				flag = true;
			Label1:
				JPath jPath = this;
				jPath._currentIndex++;
			}
			throw new Exception(string.Concat("Unexpected character while parsing path: ", chr));
		}
	}
}