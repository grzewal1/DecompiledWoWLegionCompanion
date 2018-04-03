using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Newtonsoft.Json.Linq
{
	public class JValue : JToken, IEquatable<JValue>, IFormattable, IComparable, IComparable<JValue>
	{
		private JTokenType _valueType;

		private object _value;

		public override bool HasValues
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
				return this._valueType;
			}
		}

		public object Value
		{
			get
			{
				return this._value;
			}
			set
			{
				System.Type type;
				System.Type type1;
				if (this._value == null)
				{
					type = null;
				}
				else
				{
					type = this._value.GetType();
				}
				System.Type type2 = type;
				if (value == null)
				{
					type1 = null;
				}
				else
				{
					type1 = value.GetType();
				}
				if (type2 != type1)
				{
					this._valueType = JValue.GetValueType(new JTokenType?(this._valueType), value);
				}
				this._value = value;
			}
		}

		internal JValue(object value, JTokenType type)
		{
			this._value = value;
			this._valueType = type;
		}

		public JValue(JValue other) : this(other.Value, other.Type)
		{
		}

		public JValue(long value) : this(value, JTokenType.Integer)
		{
		}

		public JValue(ulong value) : this(value, JTokenType.Integer)
		{
		}

		public JValue(double value) : this(value, JTokenType.Float)
		{
		}

		public JValue(DateTime value) : this(value, JTokenType.Date)
		{
		}

		public JValue(bool value) : this(value, JTokenType.Boolean)
		{
		}

		public JValue(string value) : this(value, JTokenType.String)
		{
		}

		public JValue(Guid value) : this(value, JTokenType.String)
		{
		}

		public JValue(Uri value) : this(value, JTokenType.String)
		{
		}

		public JValue(TimeSpan value) : this(value, JTokenType.String)
		{
		}

		public JValue(object value) : this(value, JValue.GetValueType(null, value))
		{
		}

		internal override JToken CloneToken()
		{
			return new JValue(this);
		}

		private static int Compare(JTokenType valueType, object objA, object objB)
		{
			object[] objArray;
			CultureInfo invariantCulture;
			object obj;
			if (objA == null && objB == null)
			{
				return 0;
			}
			if (objA != null && objB == null)
			{
				return 1;
			}
			if (objA == null && objB != null)
			{
				return -1;
			}
			switch (valueType)
			{
				case JTokenType.Comment:
				case JTokenType.String:
				case JTokenType.Raw:
				{
					string str = Convert.ToString(objA, CultureInfo.InvariantCulture);
					string str1 = Convert.ToString(objB, CultureInfo.InvariantCulture);
					return str.CompareTo(str1);
				}
				case JTokenType.Integer:
				{
					if (objA is ulong || objB is ulong || objA is decimal || objB is decimal)
					{
						decimal num = Convert.ToDecimal(objA, CultureInfo.InvariantCulture);
						return num.CompareTo(Convert.ToDecimal(objB, CultureInfo.InvariantCulture));
					}
					if (objA is float || objB is float || objA is double || objB is double)
					{
						return JValue.CompareFloat(objA, objB);
					}
					long num1 = Convert.ToInt64(objA, CultureInfo.InvariantCulture);
					return num1.CompareTo(Convert.ToInt64(objB, CultureInfo.InvariantCulture));
				}
				case JTokenType.Float:
				{
					return JValue.CompareFloat(objA, objB);
				}
				case JTokenType.Boolean:
				{
					bool flag = Convert.ToBoolean(objA, CultureInfo.InvariantCulture);
					bool flag1 = Convert.ToBoolean(objB, CultureInfo.InvariantCulture);
					return flag.CompareTo(flag1);
				}
				case JTokenType.Null:
				case JTokenType.Undefined:
				{
					obj = valueType;
					invariantCulture = CultureInfo.InvariantCulture;
					objArray = new object[] { valueType };
					throw MiscellaneousUtils.CreateArgumentOutOfRangeException("valueType", obj, "Unexpected value type: {0}".FormatWith(invariantCulture, objArray));
				}
				case JTokenType.Date:
				{
					if (objA is DateTime)
					{
						DateTime dateTime = Convert.ToDateTime(objA, CultureInfo.InvariantCulture);
						DateTime dateTime1 = Convert.ToDateTime(objB, CultureInfo.InvariantCulture);
						return dateTime.CompareTo(dateTime1);
					}
					if (!(objB is DateTimeOffset))
					{
						throw new ArgumentException("Object must be of type DateTimeOffset.");
					}
					return ((DateTimeOffset)objA).CompareTo((DateTimeOffset)objB);
				}
				case JTokenType.Bytes:
				{
					if (!(objB is byte[]))
					{
						throw new ArgumentException("Object must be of type byte[].");
					}
					byte[] numArray = objA as byte[];
					byte[] numArray1 = objB as byte[];
					if (numArray == null)
					{
						return -1;
					}
					if (numArray1 == null)
					{
						return 1;
					}
					return MiscellaneousUtils.ByteArrayCompare(numArray, numArray1);
				}
				case JTokenType.Guid:
				{
					if (!(objB is Guid))
					{
						throw new ArgumentException("Object must be of type Guid.");
					}
					return ((Guid)objA).CompareTo((Guid)objB);
				}
				case JTokenType.Uri:
				{
					if (!(objB is Uri))
					{
						throw new ArgumentException("Object must be of type Uri.");
					}
					Uri uri = (Uri)objA;
					Uri uri1 = (Uri)objB;
					return Comparer<string>.Default.Compare(uri.ToString(), uri1.ToString());
				}
				case JTokenType.TimeSpan:
				{
					if (!(objB is TimeSpan))
					{
						throw new ArgumentException("Object must be of type TimeSpan.");
					}
					return ((TimeSpan)objA).CompareTo((TimeSpan)objB);
				}
				default:
				{
					obj = valueType;
					invariantCulture = CultureInfo.InvariantCulture;
					objArray = new object[] { valueType };
					throw MiscellaneousUtils.CreateArgumentOutOfRangeException("valueType", obj, "Unexpected value type: {0}".FormatWith(invariantCulture, objArray));
				}
			}
		}

		private static int CompareFloat(object objA, object objB)
		{
			double num = Convert.ToDouble(objA, CultureInfo.InvariantCulture);
			double num1 = Convert.ToDouble(objB, CultureInfo.InvariantCulture);
			if (MathUtils.ApproxEquals(num, num1))
			{
				return 0;
			}
			return num.CompareTo(num1);
		}

		public int CompareTo(JValue obj)
		{
			if (obj == null)
			{
				return 1;
			}
			return JValue.Compare(this._valueType, this._value, obj._value);
		}

		public static JValue CreateComment(string value)
		{
			return new JValue(value, JTokenType.Comment);
		}

		public static JValue CreateString(string value)
		{
			return new JValue(value, JTokenType.String);
		}

		internal override bool DeepEquals(JToken node)
		{
			JValue jValue = node as JValue;
			if (jValue == null)
			{
				return false;
			}
			return JValue.ValuesEquals(this, jValue);
		}

		public bool Equals(JValue other)
		{
			if (other == null)
			{
				return false;
			}
			return JValue.ValuesEquals(this, other);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			JValue jValue = obj as JValue;
			if (jValue != null)
			{
				return this.Equals(jValue);
			}
			return base.Equals(obj);
		}

		internal override int GetDeepHashCode()
		{
			int num;
			num = (this._value == null ? 0 : this._value.GetHashCode());
			return this._valueType.GetHashCode() ^ num;
		}

		public override int GetHashCode()
		{
			if (this._value == null)
			{
				return 0;
			}
			return this._value.GetHashCode();
		}

		private static JTokenType GetStringValueType(JTokenType? current)
		{
			if (!current.HasValue)
			{
				return JTokenType.String;
			}
			JTokenType value = current.Value;
			switch (value)
			{
				case JTokenType.Comment:
				case JTokenType.String:
				{
					return current.Value;
				}
				default:
				{
					if (value == JTokenType.Raw)
					{
						return current.Value;
					}
					return JTokenType.String;
				}
			}
		}

		private static JTokenType GetValueType(JTokenType? current, object value)
		{
			if (value == null)
			{
				return JTokenType.Null;
			}
			if (value == DBNull.Value)
			{
				return JTokenType.Null;
			}
			if (value is string)
			{
				return JValue.GetStringValueType(current);
			}
			if (value is long || value is int || value is short || value is sbyte || value is ulong || value is uint || value is ushort || value is byte)
			{
				return JTokenType.Integer;
			}
			if (value is Enum)
			{
				return JTokenType.Integer;
			}
			if (value is double || value is float || value is decimal)
			{
				return JTokenType.Float;
			}
			if (value is DateTime)
			{
				return JTokenType.Date;
			}
			if (value is DateTimeOffset)
			{
				return JTokenType.Date;
			}
			if (value is byte[])
			{
				return JTokenType.Bytes;
			}
			if (value is bool)
			{
				return JTokenType.Boolean;
			}
			if (value is Guid)
			{
				return JTokenType.Guid;
			}
			if (value is Uri)
			{
				return JTokenType.Uri;
			}
			if (!(value is TimeSpan))
			{
				throw new ArgumentException("Could not determine JSON object type for type {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { value.GetType() }));
			}
			return JTokenType.TimeSpan;
		}

		int System.IComparable.CompareTo(object obj)
		{
			object obj1;
			if (obj == null)
			{
				return 1;
			}
			obj1 = (!(obj is JValue) ? obj : ((JValue)obj).Value);
			return JValue.Compare(this._valueType, this._value, obj1);
		}

		public override string ToString()
		{
			if (this._value == null)
			{
				return string.Empty;
			}
			return this._value.ToString();
		}

		public string ToString(string format)
		{
			return this.ToString(format, CultureInfo.CurrentCulture);
		}

		public string ToString(IFormatProvider formatProvider)
		{
			return this.ToString(null, formatProvider);
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (this._value == null)
			{
				return string.Empty;
			}
			IFormattable formattable = this._value as IFormattable;
			if (formattable == null)
			{
				return this._value.ToString();
			}
			return formattable.ToString(format, formatProvider);
		}

		private static bool ValuesEquals(JValue v1, JValue v2)
		{
			bool flag;
			if (v1 == v2)
			{
				flag = true;
			}
			else
			{
				flag = (v1._valueType != v2._valueType ? false : JValue.Compare(v1._valueType, v1._value, v2._value) == 0);
			}
			return flag;
		}

		public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
		{
			string str;
			string str1;
			string str2;
			JTokenType jTokenType = this._valueType;
			switch (jTokenType)
			{
				case JTokenType.Null:
				{
					writer.WriteNull();
					return;
				}
				case JTokenType.Undefined:
				{
					writer.WriteUndefined();
					return;
				}
				case JTokenType.Raw:
				{
					JsonWriter jsonWriter = writer;
					if (this._value == null)
					{
						str2 = null;
					}
					else
					{
						str2 = this._value.ToString();
					}
					jsonWriter.WriteRawValue(str2);
					return;
				}
				default:
				{
					if (jTokenType == JTokenType.Comment)
					{
						break;
					}
					else
					{
						goto Label0;
					}
				}
			}
			writer.WriteComment(this._value.ToString());
			return;
		Label0:
			if (this._value != null)
			{
				JsonConverter matchingConverter = JsonSerializer.GetMatchingConverter(converters, this._value.GetType());
				JsonConverter jsonConverter = matchingConverter;
				if (matchingConverter != null)
				{
					jsonConverter.WriteJson(writer, this._value, new JsonSerializer());
					return;
				}
			}
			switch (this._valueType)
			{
				case JTokenType.Integer:
				{
					writer.WriteValue(Convert.ToInt64(this._value, CultureInfo.InvariantCulture));
					return;
				}
				case JTokenType.Float:
				{
					writer.WriteValue(Convert.ToDouble(this._value, CultureInfo.InvariantCulture));
					return;
				}
				case JTokenType.String:
				{
					JsonWriter jsonWriter1 = writer;
					if (this._value == null)
					{
						str = null;
					}
					else
					{
						str = this._value.ToString();
					}
					jsonWriter1.WriteValue(str);
					return;
				}
				case JTokenType.Boolean:
				{
					writer.WriteValue(Convert.ToBoolean(this._value, CultureInfo.InvariantCulture));
					return;
				}
				case JTokenType.Null:
				case JTokenType.Undefined:
				case JTokenType.Raw:
				{
					throw MiscellaneousUtils.CreateArgumentOutOfRangeException("TokenType", this._valueType, "Unexpected token type.");
				}
				case JTokenType.Date:
				{
					if (!(this._value is DateTimeOffset))
					{
						writer.WriteValue(Convert.ToDateTime(this._value, CultureInfo.InvariantCulture));
					}
					else
					{
						writer.WriteValue((DateTimeOffset)this._value);
					}
					return;
				}
				case JTokenType.Bytes:
				{
					writer.WriteValue((byte[])this._value);
					return;
				}
				case JTokenType.Guid:
				case JTokenType.Uri:
				case JTokenType.TimeSpan:
				{
					JsonWriter jsonWriter2 = writer;
					if (this._value == null)
					{
						str1 = null;
					}
					else
					{
						str1 = this._value.ToString();
					}
					jsonWriter2.WriteValue(str1);
					return;
				}
				default:
				{
					throw MiscellaneousUtils.CreateArgumentOutOfRangeException("TokenType", this._valueType, "Unexpected token type.");
				}
			}
		}
	}
}