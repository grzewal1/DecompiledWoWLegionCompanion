using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json
{
	public abstract class JsonWriter : IDisposable
	{
		private readonly static JsonWriter.State[][] stateArray;

		private int _top;

		private readonly List<JTokenType> _stack;

		private JsonWriter.State _currentState;

		private Newtonsoft.Json.Formatting _formatting;

		public bool CloseOutput
		{
			get;
			set;
		}

		public Newtonsoft.Json.Formatting Formatting
		{
			get
			{
				return this._formatting;
			}
			set
			{
				this._formatting = value;
			}
		}

		protected internal int Top
		{
			get
			{
				return this._top;
			}
		}

		public Newtonsoft.Json.WriteState WriteState
		{
			get
			{
				switch (this._currentState)
				{
					case JsonWriter.State.Start:
					{
						return Newtonsoft.Json.WriteState.Start;
					}
					case JsonWriter.State.Property:
					{
						return Newtonsoft.Json.WriteState.Property;
					}
					case JsonWriter.State.ObjectStart:
					case JsonWriter.State.Object:
					{
						return Newtonsoft.Json.WriteState.Object;
					}
					case JsonWriter.State.ArrayStart:
					case JsonWriter.State.Array:
					{
						return Newtonsoft.Json.WriteState.Array;
					}
					case JsonWriter.State.ConstructorStart:
					case JsonWriter.State.Constructor:
					{
						return Newtonsoft.Json.WriteState.Constructor;
					}
					case JsonWriter.State.Bytes:
					{
						throw new JsonWriterException(string.Concat("Invalid state: ", this._currentState));
					}
					case JsonWriter.State.Closed:
					{
						return Newtonsoft.Json.WriteState.Closed;
					}
					case JsonWriter.State.Error:
					{
						return Newtonsoft.Json.WriteState.Error;
					}
					default:
					{
						throw new JsonWriterException(string.Concat("Invalid state: ", this._currentState));
					}
				}
			}
		}

		static JsonWriter()
		{
			JsonWriter.stateArray = new JsonWriter.State[][] { new JsonWriter.State[] { JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error }, new JsonWriter.State[] { JsonWriter.State.ObjectStart, JsonWriter.State.ObjectStart, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.ObjectStart, JsonWriter.State.ObjectStart, JsonWriter.State.ObjectStart, JsonWriter.State.ObjectStart, JsonWriter.State.Error, JsonWriter.State.Error }, new JsonWriter.State[] { JsonWriter.State.ArrayStart, JsonWriter.State.ArrayStart, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.ArrayStart, JsonWriter.State.ArrayStart, JsonWriter.State.ArrayStart, JsonWriter.State.ArrayStart, JsonWriter.State.Error, JsonWriter.State.Error }, new JsonWriter.State[] { JsonWriter.State.ConstructorStart, JsonWriter.State.ConstructorStart, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.ConstructorStart, JsonWriter.State.ConstructorStart, JsonWriter.State.ConstructorStart, JsonWriter.State.ConstructorStart, JsonWriter.State.Error, JsonWriter.State.Error }, new JsonWriter.State[] { JsonWriter.State.Property, JsonWriter.State.Error, JsonWriter.State.Property, JsonWriter.State.Property, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error }, new JsonWriter.State[] { JsonWriter.State.Start, JsonWriter.State.Property, JsonWriter.State.ObjectStart, JsonWriter.State.Object, JsonWriter.State.ArrayStart, JsonWriter.State.Array, JsonWriter.State.Constructor, JsonWriter.State.Constructor, JsonWriter.State.Error, JsonWriter.State.Error }, new JsonWriter.State[] { JsonWriter.State.Start, JsonWriter.State.Property, JsonWriter.State.ObjectStart, JsonWriter.State.Object, JsonWriter.State.ArrayStart, JsonWriter.State.Array, JsonWriter.State.Constructor, JsonWriter.State.Constructor, JsonWriter.State.Error, JsonWriter.State.Error }, new JsonWriter.State[] { JsonWriter.State.Start, JsonWriter.State.Object, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Array, JsonWriter.State.Array, JsonWriter.State.Constructor, JsonWriter.State.Constructor, JsonWriter.State.Error, JsonWriter.State.Error } };
		}

		protected JsonWriter()
		{
			this._stack = new List<JTokenType>(8)
			{
				JTokenType.None
			};
			this._currentState = JsonWriter.State.Start;
			this._formatting = Newtonsoft.Json.Formatting.None;
			this.CloseOutput = true;
		}

		internal void AutoComplete(JsonToken tokenBeingWritten)
		{
			int num;
			switch (tokenBeingWritten)
			{
				case JsonToken.Integer:
				case JsonToken.Float:
				case JsonToken.String:
				case JsonToken.Boolean:
				case JsonToken.Null:
				case JsonToken.Undefined:
				case JsonToken.Date:
				case JsonToken.Bytes:
				{
					num = 7;
					break;
				}
				case JsonToken.EndObject:
				case JsonToken.EndArray:
				case JsonToken.EndConstructor:
				{
					num = (int)tokenBeingWritten;
					break;
				}
				default:
				{
					goto case JsonToken.EndConstructor;
				}
			}
			JsonWriter.State state = JsonWriter.stateArray[num][(int)this._currentState];
			if (state == JsonWriter.State.Error)
			{
				throw new JsonWriterException("Token {0} in state {1} would result in an invalid JavaScript object.".FormatWith(CultureInfo.InvariantCulture, new object[] { tokenBeingWritten.ToString(), this._currentState.ToString() }));
			}
			if ((this._currentState == JsonWriter.State.Object || this._currentState == JsonWriter.State.Array || this._currentState == JsonWriter.State.Constructor) && tokenBeingWritten != JsonToken.Comment)
			{
				this.WriteValueDelimiter();
			}
			else if (this._currentState == JsonWriter.State.Property && this._formatting == Newtonsoft.Json.Formatting.Indented)
			{
				this.WriteIndentSpace();
			}
			Newtonsoft.Json.WriteState writeState = this.WriteState;
			if (tokenBeingWritten == JsonToken.PropertyName && writeState != Newtonsoft.Json.WriteState.Start || writeState == Newtonsoft.Json.WriteState.Array || writeState == Newtonsoft.Json.WriteState.Constructor)
			{
				this.WriteIndent();
			}
			this._currentState = state;
		}

		private void AutoCompleteAll()
		{
			while (this._top > 0)
			{
				this.WriteEnd();
			}
		}

		private void AutoCompleteClose(JsonToken tokenBeingClosed)
		{
			int num = 0;
			int num1 = 0;
			while (num1 < this._top)
			{
				if (this._stack[this._top - num1] != this.GetTypeForCloseToken(tokenBeingClosed))
				{
					num1++;
				}
				else
				{
					num = num1 + 1;
					break;
				}
			}
			if (num == 0)
			{
				throw new JsonWriterException("No token to close.");
			}
			for (int i = 0; i < num; i++)
			{
				JsonToken closeTokenForType = this.GetCloseTokenForType(this.Pop());
				if (this._currentState != JsonWriter.State.ObjectStart && this._currentState != JsonWriter.State.ArrayStart)
				{
					this.WriteIndent();
				}
				this.WriteEnd(closeTokenForType);
			}
			JTokenType jTokenType = this.Peek();
			switch (jTokenType)
			{
				case JTokenType.None:
				{
					this._currentState = JsonWriter.State.Start;
					break;
				}
				case JTokenType.Object:
				{
					this._currentState = JsonWriter.State.Object;
					break;
				}
				case JTokenType.Array:
				{
					this._currentState = JsonWriter.State.Array;
					break;
				}
				case JTokenType.Constructor:
				{
					this._currentState = JsonWriter.State.Array;
					break;
				}
				default:
				{
					throw new JsonWriterException(string.Concat("Unknown JsonType: ", jTokenType));
				}
			}
		}

		public virtual void Close()
		{
			this.AutoCompleteAll();
		}

		private void Dispose(bool disposing)
		{
			if (this.WriteState != Newtonsoft.Json.WriteState.Closed)
			{
				this.Close();
			}
		}

		public abstract void Flush();

		private JsonToken GetCloseTokenForType(JTokenType type)
		{
			switch (type)
			{
				case JTokenType.Object:
				{
					return JsonToken.EndObject;
				}
				case JTokenType.Array:
				{
					return JsonToken.EndArray;
				}
				case JTokenType.Constructor:
				{
					return JsonToken.EndConstructor;
				}
			}
			throw new JsonWriterException(string.Concat("No close token for type: ", type));
		}

		private JTokenType GetTypeForCloseToken(JsonToken token)
		{
			switch (token)
			{
				case JsonToken.EndObject:
				{
					return JTokenType.Object;
				}
				case JsonToken.EndArray:
				{
					return JTokenType.Array;
				}
				case JsonToken.EndConstructor:
				{
					return JTokenType.Constructor;
				}
			}
			throw new JsonWriterException(string.Concat("No type for token: ", token));
		}

		private bool IsEndToken(JsonToken token)
		{
			switch (token)
			{
				case JsonToken.EndObject:
				case JsonToken.EndArray:
				case JsonToken.EndConstructor:
				{
					return true;
				}
			}
			return false;
		}

		private bool IsStartToken(JsonToken token)
		{
			switch (token)
			{
				case JsonToken.StartObject:
				case JsonToken.StartArray:
				case JsonToken.StartConstructor:
				{
					return true;
				}
			}
			return false;
		}

		private JTokenType Peek()
		{
			return this._stack[this._top];
		}

		private JTokenType Pop()
		{
			JTokenType jTokenType = this.Peek();
			JsonWriter jsonWriter = this;
			jsonWriter._top = jsonWriter._top - 1;
			return jTokenType;
		}

		private void Push(JTokenType value)
		{
			JsonWriter jsonWriter = this;
			jsonWriter._top = jsonWriter._top + 1;
			if (this._stack.Count > this._top)
			{
				this._stack[this._top] = value;
			}
			else
			{
				this._stack.Add(value);
			}
		}

		void System.IDisposable.Dispose()
		{
			this.Dispose(true);
		}

		public virtual void WriteComment(string text)
		{
			this.AutoComplete(JsonToken.Comment);
		}

		private void WriteConstructorDate(JsonReader reader)
		{
			if (!reader.Read())
			{
				throw new Exception("Unexpected end while reading date constructor.");
			}
			if (reader.TokenType != JsonToken.Integer)
			{
				throw new Exception(string.Concat("Unexpected token while reading date constructor. Expected Integer, got ", reader.TokenType));
			}
			DateTime dateTime = JsonConvert.ConvertJavaScriptTicksToDateTime((long)reader.Value);
			if (!reader.Read())
			{
				throw new Exception("Unexpected end while reading date constructor.");
			}
			if (reader.TokenType != JsonToken.EndConstructor)
			{
				throw new Exception(string.Concat("Unexpected token while reading date constructor. Expected EndConstructor, got ", reader.TokenType));
			}
			this.WriteValue(dateTime);
		}

		public virtual void WriteEnd()
		{
			this.WriteEnd(this.Peek());
		}

		private void WriteEnd(JTokenType type)
		{
			switch (type)
			{
				case JTokenType.Object:
				{
					this.WriteEndObject();
					break;
				}
				case JTokenType.Array:
				{
					this.WriteEndArray();
					break;
				}
				case JTokenType.Constructor:
				{
					this.WriteEndConstructor();
					break;
				}
				default:
				{
					throw new JsonWriterException(string.Concat("Unexpected type when writing end: ", type));
				}
			}
		}

		protected virtual void WriteEnd(JsonToken token)
		{
		}

		public virtual void WriteEndArray()
		{
			this.AutoCompleteClose(JsonToken.EndArray);
		}

		public virtual void WriteEndConstructor()
		{
			this.AutoCompleteClose(JsonToken.EndConstructor);
		}

		public virtual void WriteEndObject()
		{
			this.AutoCompleteClose(JsonToken.EndObject);
		}

		protected virtual void WriteIndent()
		{
		}

		protected virtual void WriteIndentSpace()
		{
		}

		public virtual void WriteNull()
		{
			this.AutoComplete(JsonToken.Null);
		}

		public virtual void WritePropertyName(string name)
		{
			this.AutoComplete(JsonToken.PropertyName);
		}

		public virtual void WriteRaw(string json)
		{
		}

		public virtual void WriteRawValue(string json)
		{
			this.AutoComplete(JsonToken.Undefined);
			this.WriteRaw(json);
		}

		public virtual void WriteStartArray()
		{
			this.AutoComplete(JsonToken.StartArray);
			this.Push(JTokenType.Array);
		}

		public virtual void WriteStartConstructor(string name)
		{
			this.AutoComplete(JsonToken.StartConstructor);
			this.Push(JTokenType.Constructor);
		}

		public virtual void WriteStartObject()
		{
			this.AutoComplete(JsonToken.StartObject);
			this.Push(JTokenType.Object);
		}

		public void WriteToken(JsonReader reader)
		{
			int num;
			ValidationUtils.ArgumentNotNull(reader, "reader");
			if (reader.TokenType != JsonToken.None)
			{
				num = (this.IsStartToken(reader.TokenType) ? reader.Depth : reader.Depth + 1);
			}
			else
			{
				num = -1;
			}
			this.WriteToken(reader, num);
		}

		internal void WriteToken(JsonReader reader, int initialDepth)
		{
			int depth;
			int num;
			int num1;
			do
			{
				switch (reader.TokenType)
				{
					case JsonToken.None:
					{
						break;
					}
					case JsonToken.StartObject:
					{
						this.WriteStartObject();
						break;
					}
					case JsonToken.StartArray:
					{
						this.WriteStartArray();
						break;
					}
					case JsonToken.StartConstructor:
					{
						if (string.Compare(reader.Value.ToString(), "Date", StringComparison.Ordinal) != 0)
						{
							this.WriteStartConstructor(reader.Value.ToString());
						}
						else
						{
							this.WriteConstructorDate(reader);
						}
						break;
					}
					case JsonToken.PropertyName:
					{
						this.WritePropertyName(reader.Value.ToString());
						break;
					}
					case JsonToken.Comment:
					{
						this.WriteComment(reader.Value.ToString());
						break;
					}
					case JsonToken.Raw:
					{
						this.WriteRawValue((string)reader.Value);
						break;
					}
					case JsonToken.Integer:
					{
						this.WriteValue(Convert.ToInt64(reader.Value, CultureInfo.InvariantCulture));
						break;
					}
					case JsonToken.Float:
					{
						this.WriteValue(Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture));
						break;
					}
					case JsonToken.String:
					{
						this.WriteValue(reader.Value.ToString());
						break;
					}
					case JsonToken.Boolean:
					{
						this.WriteValue(Convert.ToBoolean(reader.Value, CultureInfo.InvariantCulture));
						break;
					}
					case JsonToken.Null:
					{
						this.WriteNull();
						break;
					}
					case JsonToken.Undefined:
					{
						this.WriteUndefined();
						break;
					}
					case JsonToken.EndObject:
					{
						this.WriteEndObject();
						break;
					}
					case JsonToken.EndArray:
					{
						this.WriteEndArray();
						break;
					}
					case JsonToken.EndConstructor:
					{
						this.WriteEndConstructor();
						break;
					}
					case JsonToken.Date:
					{
						this.WriteValue((DateTime)reader.Value);
						break;
					}
					case JsonToken.Bytes:
					{
						this.WriteValue((byte[])reader.Value);
						break;
					}
					default:
					{
						throw MiscellaneousUtils.CreateArgumentOutOfRangeException("TokenType", reader.TokenType, "Unexpected token type.");
					}
				}
				num1 = initialDepth - 1;
				depth = reader.Depth;
				num = (!this.IsEndToken(reader.TokenType) ? 0 : 1);
			}
			while (num1 < depth - num && reader.Read());
		}

		public virtual void WriteUndefined()
		{
			this.AutoComplete(JsonToken.Undefined);
		}

		public virtual void WriteValue(string value)
		{
			this.AutoComplete(JsonToken.String);
		}

		public virtual void WriteValue(int value)
		{
			this.AutoComplete(JsonToken.Integer);
		}

		public virtual void WriteValue(uint value)
		{
			this.AutoComplete(JsonToken.Integer);
		}

		public virtual void WriteValue(long value)
		{
			this.AutoComplete(JsonToken.Integer);
		}

		public virtual void WriteValue(ulong value)
		{
			this.AutoComplete(JsonToken.Integer);
		}

		public virtual void WriteValue(float value)
		{
			this.AutoComplete(JsonToken.Float);
		}

		public virtual void WriteValue(double value)
		{
			this.AutoComplete(JsonToken.Float);
		}

		public virtual void WriteValue(bool value)
		{
			this.AutoComplete(JsonToken.Boolean);
		}

		public virtual void WriteValue(short value)
		{
			this.AutoComplete(JsonToken.Integer);
		}

		public virtual void WriteValue(ushort value)
		{
			this.AutoComplete(JsonToken.Integer);
		}

		public virtual void WriteValue(char value)
		{
			this.AutoComplete(JsonToken.String);
		}

		public virtual void WriteValue(byte value)
		{
			this.AutoComplete(JsonToken.Integer);
		}

		public virtual void WriteValue(sbyte value)
		{
			this.AutoComplete(JsonToken.Integer);
		}

		public virtual void WriteValue(decimal value)
		{
			this.AutoComplete(JsonToken.Float);
		}

		public virtual void WriteValue(DateTime value)
		{
			this.AutoComplete(JsonToken.Date);
		}

		public virtual void WriteValue(DateTimeOffset value)
		{
			this.AutoComplete(JsonToken.Date);
		}

		public virtual void WriteValue(Guid value)
		{
			this.AutoComplete(JsonToken.String);
		}

		public virtual void WriteValue(TimeSpan value)
		{
			this.AutoComplete(JsonToken.String);
		}

		public virtual void WriteValue(int? value)
		{
			if (value.HasValue)
			{
				this.WriteValue(value.Value);
			}
			else
			{
				this.WriteNull();
			}
		}

		public virtual void WriteValue(uint? value)
		{
			if (value.HasValue)
			{
				this.WriteValue(value.Value);
			}
			else
			{
				this.WriteNull();
			}
		}

		public virtual void WriteValue(long? value)
		{
			if (value.HasValue)
			{
				this.WriteValue(value.Value);
			}
			else
			{
				this.WriteNull();
			}
		}

		public virtual void WriteValue(ulong? value)
		{
			if (value.HasValue)
			{
				this.WriteValue(value.Value);
			}
			else
			{
				this.WriteNull();
			}
		}

		public virtual void WriteValue(float? value)
		{
			if (value.HasValue)
			{
				this.WriteValue(value.Value);
			}
			else
			{
				this.WriteNull();
			}
		}

		public virtual void WriteValue(double? value)
		{
			if (value.HasValue)
			{
				this.WriteValue(value.Value);
			}
			else
			{
				this.WriteNull();
			}
		}

		public virtual void WriteValue(bool? value)
		{
			if (value.HasValue)
			{
				this.WriteValue(value.Value);
			}
			else
			{
				this.WriteNull();
			}
		}

		public virtual void WriteValue(short? value)
		{
			if (value.HasValue)
			{
				this.WriteValue(value.Value);
			}
			else
			{
				this.WriteNull();
			}
		}

		public virtual void WriteValue(ushort? value)
		{
			if (value.HasValue)
			{
				this.WriteValue(value.Value);
			}
			else
			{
				this.WriteNull();
			}
		}

		public virtual void WriteValue(char? value)
		{
			if (value.HasValue)
			{
				this.WriteValue(value.Value);
			}
			else
			{
				this.WriteNull();
			}
		}

		public virtual void WriteValue(byte? value)
		{
			if (value.HasValue)
			{
				this.WriteValue(value.Value);
			}
			else
			{
				this.WriteNull();
			}
		}

		public virtual void WriteValue(sbyte? value)
		{
			if (value.HasValue)
			{
				this.WriteValue(value.Value);
			}
			else
			{
				this.WriteNull();
			}
		}

		public virtual void WriteValue(decimal? value)
		{
			if (value.HasValue)
			{
				this.WriteValue(value.Value);
			}
			else
			{
				this.WriteNull();
			}
		}

		public virtual void WriteValue(DateTime? value)
		{
			if (value.HasValue)
			{
				this.WriteValue(value.Value);
			}
			else
			{
				this.WriteNull();
			}
		}

		public virtual void WriteValue(DateTimeOffset? value)
		{
			if (value.HasValue)
			{
				this.WriteValue(value.Value);
			}
			else
			{
				this.WriteNull();
			}
		}

		public virtual void WriteValue(Guid? value)
		{
			if (value.HasValue)
			{
				this.WriteValue(value.Value);
			}
			else
			{
				this.WriteNull();
			}
		}

		public virtual void WriteValue(TimeSpan? value)
		{
			if (value.HasValue)
			{
				this.WriteValue(value.Value);
			}
			else
			{
				this.WriteNull();
			}
		}

		public virtual void WriteValue(byte[] value)
		{
			if (value != null)
			{
				this.AutoComplete(JsonToken.Bytes);
			}
			else
			{
				this.WriteNull();
			}
		}

		public virtual void WriteValue(Uri value)
		{
			if (value != null)
			{
				this.AutoComplete(JsonToken.String);
			}
			else
			{
				this.WriteNull();
			}
		}

		public virtual void WriteValue(object value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			if (!(value is IConvertible))
			{
				if (value is DateTimeOffset)
				{
					this.WriteValue((DateTimeOffset)value);
					return;
				}
				if (value is byte[])
				{
					this.WriteValue((byte[])value);
					return;
				}
				if (value is Guid)
				{
					this.WriteValue((Guid)value);
					return;
				}
				if (value is Uri)
				{
					this.WriteValue((Uri)value);
					return;
				}
				if (value is TimeSpan)
				{
					this.WriteValue((TimeSpan)value);
					return;
				}
			}
			else
			{
				IConvertible convertible = value as IConvertible;
				switch (convertible.GetTypeCode())
				{
					case TypeCode.DBNull:
					{
						this.WriteNull();
						return;
					}
					case TypeCode.Boolean:
					{
						this.WriteValue(convertible.ToBoolean(CultureInfo.InvariantCulture));
						return;
					}
					case TypeCode.Char:
					{
						this.WriteValue(convertible.ToChar(CultureInfo.InvariantCulture));
						return;
					}
					case TypeCode.SByte:
					{
						this.WriteValue(convertible.ToSByte(CultureInfo.InvariantCulture));
						return;
					}
					case TypeCode.Byte:
					{
						this.WriteValue(convertible.ToByte(CultureInfo.InvariantCulture));
						return;
					}
					case TypeCode.Int16:
					{
						this.WriteValue(convertible.ToInt16(CultureInfo.InvariantCulture));
						return;
					}
					case TypeCode.UInt16:
					{
						this.WriteValue(convertible.ToUInt16(CultureInfo.InvariantCulture));
						return;
					}
					case TypeCode.Int32:
					{
						this.WriteValue(convertible.ToInt32(CultureInfo.InvariantCulture));
						return;
					}
					case TypeCode.UInt32:
					{
						this.WriteValue(convertible.ToUInt32(CultureInfo.InvariantCulture));
						return;
					}
					case TypeCode.Int64:
					{
						this.WriteValue(convertible.ToInt64(CultureInfo.InvariantCulture));
						return;
					}
					case TypeCode.UInt64:
					{
						this.WriteValue(convertible.ToUInt64(CultureInfo.InvariantCulture));
						return;
					}
					case TypeCode.Single:
					{
						this.WriteValue(convertible.ToSingle(CultureInfo.InvariantCulture));
						return;
					}
					case TypeCode.Double:
					{
						this.WriteValue(convertible.ToDouble(CultureInfo.InvariantCulture));
						return;
					}
					case TypeCode.Decimal:
					{
						this.WriteValue(convertible.ToDecimal(CultureInfo.InvariantCulture));
						return;
					}
					case TypeCode.DateTime:
					{
						this.WriteValue(convertible.ToDateTime(CultureInfo.InvariantCulture));
						return;
					}
					case TypeCode.Object | TypeCode.DateTime:
					{
						break;
					}
					case TypeCode.String:
					{
						this.WriteValue(convertible.ToString(CultureInfo.InvariantCulture));
						return;
					}
					default:
					{
						goto case TypeCode.Object | TypeCode.DateTime;
					}
				}
			}
			throw new ArgumentException("Unsupported type: {0}. Use the JsonSerializer class to get the object's JSON representation.".FormatWith(CultureInfo.InvariantCulture, new object[] { value.GetType() }));
		}

		protected virtual void WriteValueDelimiter()
		{
		}

		public virtual void WriteWhitespace(string ws)
		{
			if (ws != null && !StringUtils.IsWhiteSpace(ws))
			{
				throw new JsonWriterException("Only white space characters should be used.");
			}
		}

		private enum State
		{
			Start,
			Property,
			ObjectStart,
			Object,
			ArrayStart,
			Array,
			ConstructorStart,
			Constructor,
			Bytes,
			Closed,
			Error
		}
	}
}