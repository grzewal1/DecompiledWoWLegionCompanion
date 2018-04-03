using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Newtonsoft.Json.Bson
{
	public class BsonReader : JsonReader
	{
		private const int MaxCharBytesSize = 128;

		private readonly static byte[] _seqRange1;

		private readonly static byte[] _seqRange2;

		private readonly static byte[] _seqRange3;

		private readonly static byte[] _seqRange4;

		private readonly BinaryReader _reader;

		private readonly List<BsonReader.ContainerContext> _stack;

		private byte[] _byteBuffer;

		private char[] _charBuffer;

		private BsonType _currentElementType;

		private BsonReader.BsonReaderState _bsonReaderState;

		private BsonReader.ContainerContext _currentContext;

		private bool _readRootValueAsArray;

		private bool _jsonNet35BinaryCompatibility;

		private DateTimeKind _dateTimeKindHandling;

		public DateTimeKind DateTimeKindHandling
		{
			get
			{
				return this._dateTimeKindHandling;
			}
			set
			{
				this._dateTimeKindHandling = value;
			}
		}

		public bool JsonNet35BinaryCompatibility
		{
			get
			{
				return this._jsonNet35BinaryCompatibility;
			}
			set
			{
				this._jsonNet35BinaryCompatibility = value;
			}
		}

		public bool ReadRootValueAsArray
		{
			get
			{
				return this._readRootValueAsArray;
			}
			set
			{
				this._readRootValueAsArray = value;
			}
		}

		static BsonReader()
		{
			BsonReader._seqRange1 = new byte[] { 0, 127 };
			BsonReader._seqRange2 = new byte[] { 194, 223 };
			BsonReader._seqRange3 = new byte[] { 224, 239 };
			BsonReader._seqRange4 = new byte[] { 240, 244 };
		}

		public BsonReader(Stream stream) : this(stream, false, DateTimeKind.Local)
		{
		}

		public BsonReader(Stream stream, bool readRootValueAsArray, DateTimeKind dateTimeKindHandling)
		{
			ValidationUtils.ArgumentNotNull(stream, "stream");
			this._reader = new BinaryReader(stream);
			this._stack = new List<BsonReader.ContainerContext>();
			this._readRootValueAsArray = readRootValueAsArray;
			this._dateTimeKindHandling = dateTimeKindHandling;
		}

		private int BytesInSequence(byte b)
		{
			if (b <= BsonReader._seqRange1[1])
			{
				return 1;
			}
			if (b >= BsonReader._seqRange2[0] && b <= BsonReader._seqRange2[1])
			{
				return 2;
			}
			if (b >= BsonReader._seqRange3[0] && b <= BsonReader._seqRange3[1])
			{
				return 3;
			}
			if (b >= BsonReader._seqRange4[0] && b <= BsonReader._seqRange4[1])
			{
				return 4;
			}
			return 0;
		}

		public override void Close()
		{
			base.Close();
			if (base.CloseInput && this._reader != null)
			{
				this._reader.Close();
			}
		}

		private void EnsureBuffers()
		{
			if (this._byteBuffer == null)
			{
				this._byteBuffer = new byte[128];
			}
			if (this._charBuffer == null)
			{
				int maxCharCount = Encoding.UTF8.GetMaxCharCount(128);
				this._charBuffer = new char[maxCharCount];
			}
		}

		private int GetLastFullCharStop(int start)
		{
			int num = start;
			int num1 = 0;
			while (num >= 0)
			{
				num1 = this.BytesInSequence(this._byteBuffer[num]);
				if (num1 == 0)
				{
					num--;
				}
				else if (num1 != 1)
				{
					num--;
					break;
				}
				else
				{
					break;
				}
			}
			if (num1 == start - num)
			{
				return start;
			}
			return num;
		}

		private string GetString(int length)
		{
			if (length == 0)
			{
				return string.Empty;
			}
			this.EnsureBuffers();
			StringBuilder stringBuilder = null;
			int num = 0;
			int num1 = 0;
			do
			{
				int num2 = (length - num <= 128 - num1 ? length - num : 128 - num1);
				int num3 = this._reader.BaseStream.Read(this._byteBuffer, num1, num2);
				if (num3 == 0)
				{
					throw new EndOfStreamException("Unable to read beyond the end of the stream.");
				}
				num += num3;
				num3 += num1;
				if (num3 == length)
				{
					int chars = Encoding.UTF8.GetChars(this._byteBuffer, 0, num3, this._charBuffer, 0);
					return new string(this._charBuffer, 0, chars);
				}
				int lastFullCharStop = this.GetLastFullCharStop(num3 - 1);
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder(length);
				}
				int chars1 = Encoding.UTF8.GetChars(this._byteBuffer, 0, lastFullCharStop + 1, this._charBuffer, 0);
				stringBuilder.Append(this._charBuffer, 0, chars1);
				if (lastFullCharStop >= num3 - 1)
				{
					num1 = 0;
				}
				else
				{
					num1 = num3 - lastFullCharStop - 1;
					Array.Copy(this._byteBuffer, lastFullCharStop + 1, this._byteBuffer, 0, num1);
				}
			}
			while (num < length);
			return stringBuilder.ToString();
		}

		private bool IsWrappedInTypeObject()
		{
			if (this.TokenType != JsonToken.StartObject)
			{
				return false;
			}
			this.Read();
			if (this.Value.ToString() == "$type")
			{
				this.Read();
				if (this.Value != null && this.Value.ToString().StartsWith("System.Byte[]"))
				{
					this.Read();
					if (this.Value.ToString() == "$value")
					{
						return true;
					}
				}
			}
			throw new JsonReaderException("Unexpected token when reading bytes: {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { JsonToken.StartObject }));
		}

		private void MovePosition(int count)
		{
			this._currentContext.Position += count;
		}

		private void PopContext()
		{
			this._stack.RemoveAt(this._stack.Count - 1);
			if (this._stack.Count != 0)
			{
				this._currentContext = this._stack[this._stack.Count - 1];
			}
			else
			{
				this._currentContext = null;
			}
		}

		private void PushContext(BsonReader.ContainerContext newContext)
		{
			this._stack.Add(newContext);
			this._currentContext = newContext;
		}

		public override bool Read()
		{
			bool flag;
			try
			{
				switch (this._bsonReaderState)
				{
					case BsonReader.BsonReaderState.Normal:
					{
						flag = this.ReadNormal();
						return flag;
					}
					case BsonReader.BsonReaderState.ReferenceStart:
					case BsonReader.BsonReaderState.ReferenceRef:
					case BsonReader.BsonReaderState.ReferenceId:
					{
						flag = this.ReadReference();
						return flag;
					}
					case BsonReader.BsonReaderState.CodeWScopeStart:
					case BsonReader.BsonReaderState.CodeWScopeCode:
					case BsonReader.BsonReaderState.CodeWScopeScope:
					case BsonReader.BsonReaderState.CodeWScopeScopeObject:
					case BsonReader.BsonReaderState.CodeWScopeScopeEnd:
					{
						flag = this.ReadCodeWScope();
						return flag;
					}
				}
				throw new JsonReaderException("Unexpected state: {0}".FormatWith(CultureInfo.InvariantCulture, new object[] { this._bsonReaderState }));
			}
			catch (EndOfStreamException endOfStreamException)
			{
				flag = false;
			}
			return flag;
		}

		public override byte[] ReadAsBytes()
		{
			this.Read();
			if (this.IsWrappedInTypeObject())
			{
				byte[] numArray = this.ReadAsBytes();
				this.Read();
				this.SetToken(JsonToken.Bytes, numArray);
				return numArray;
			}
			if (this.TokenType == JsonToken.Null)
			{
				return null;
			}
			if (this.TokenType != JsonToken.Bytes)
			{
				throw new JsonReaderException("Error reading bytes. Expected bytes but got {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { this.TokenType }));
			}
			return (byte[])this.Value;
		}

		public override DateTimeOffset? ReadAsDateTimeOffset()
		{
			this.Read();
			if (this.TokenType == JsonToken.Null)
			{
				return null;
			}
			if (this.TokenType != JsonToken.Date)
			{
				throw new JsonReaderException("Error reading date. Expected bytes but got {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { this.TokenType }));
			}
			this.SetToken(JsonToken.Date, new DateTimeOffset((DateTime)this.Value));
			return new DateTimeOffset?((DateTimeOffset)this.Value);
		}

		public override decimal? ReadAsDecimal()
		{
			this.Read();
			if (this.TokenType == JsonToken.Null)
			{
				return null;
			}
			if (this.TokenType != JsonToken.Integer && this.TokenType != JsonToken.Float)
			{
				throw new JsonReaderException("Error reading decimal. Expected a number but got {0}.".FormatWith(CultureInfo.InvariantCulture, new object[] { this.TokenType }));
			}
			this.SetToken(JsonToken.Float, Convert.ToDecimal(this.Value, CultureInfo.InvariantCulture));
			return new decimal?((decimal)this.Value);
		}

		private byte[] ReadBinary()
		{
			int num = this.ReadInt32();
			if (this.ReadByte() == 2 && !this._jsonNet35BinaryCompatibility)
			{
				num = this.ReadInt32();
			}
			return this.ReadBytes(num);
		}

		private byte ReadByte()
		{
			this.MovePosition(1);
			return this._reader.ReadByte();
		}

		private byte[] ReadBytes(int count)
		{
			this.MovePosition(count);
			return this._reader.ReadBytes(count);
		}

		private bool ReadCodeWScope()
		{
			switch (this._bsonReaderState)
			{
				case BsonReader.BsonReaderState.CodeWScopeStart:
				{
					this.SetToken(JsonToken.PropertyName, "$code");
					this._bsonReaderState = BsonReader.BsonReaderState.CodeWScopeCode;
					return true;
				}
				case BsonReader.BsonReaderState.CodeWScopeCode:
				{
					this.ReadInt32();
					this.SetToken(JsonToken.String, this.ReadLengthString());
					this._bsonReaderState = BsonReader.BsonReaderState.CodeWScopeScope;
					return true;
				}
				case BsonReader.BsonReaderState.CodeWScopeScope:
				{
					if (base.CurrentState == JsonReader.State.PostValue)
					{
						this.SetToken(JsonToken.PropertyName, "$scope");
						return true;
					}
					base.SetToken(JsonToken.StartObject);
					this._bsonReaderState = BsonReader.BsonReaderState.CodeWScopeScopeObject;
					BsonReader.ContainerContext containerContext = new BsonReader.ContainerContext(BsonType.Object);
					this.PushContext(containerContext);
					containerContext.Length = this.ReadInt32();
					return true;
				}
				case BsonReader.BsonReaderState.CodeWScopeScopeObject:
				{
					bool flag = this.ReadNormal();
					if (flag && this.TokenType == JsonToken.EndObject)
					{
						this._bsonReaderState = BsonReader.BsonReaderState.CodeWScopeScopeEnd;
					}
					return flag;
				}
				case BsonReader.BsonReaderState.CodeWScopeScopeEnd:
				{
					base.SetToken(JsonToken.EndObject);
					this._bsonReaderState = BsonReader.BsonReaderState.Normal;
					return true;
				}
			}
			throw new ArgumentOutOfRangeException();
		}

		private double ReadDouble()
		{
			this.MovePosition(8);
			return this._reader.ReadDouble();
		}

		private string ReadElement()
		{
			this._currentElementType = this.ReadType();
			return this.ReadString();
		}

		private int ReadInt32()
		{
			this.MovePosition(4);
			return this._reader.ReadInt32();
		}

		private long ReadInt64()
		{
			this.MovePosition(8);
			return this._reader.ReadInt64();
		}

		private string ReadLengthString()
		{
			int num = this.ReadInt32();
			this.MovePosition(num);
			string str = this.GetString(num - 1);
			this._reader.ReadByte();
			return str;
		}

		private bool ReadNormal()
		{
			switch (base.CurrentState)
			{
				case JsonReader.State.Start:
				{
					JsonToken jsonToken = (this._readRootValueAsArray ? JsonToken.StartArray : JsonToken.StartObject);
					BsonType bsonType = (this._readRootValueAsArray ? BsonType.Array : BsonType.Object);
					base.SetToken(jsonToken);
					BsonReader.ContainerContext containerContext = new BsonReader.ContainerContext(bsonType);
					this.PushContext(containerContext);
					containerContext.Length = this.ReadInt32();
					return true;
				}
				case JsonReader.State.Complete:
				case JsonReader.State.Closed:
				{
					return false;
				}
				case JsonReader.State.Property:
				{
					this.ReadType(this._currentElementType);
					return true;
				}
				case JsonReader.State.ObjectStart:
				case JsonReader.State.ArrayStart:
				case JsonReader.State.PostValue:
				{
					BsonReader.ContainerContext containerContext1 = this._currentContext;
					if (containerContext1 == null)
					{
						return false;
					}
					int length = containerContext1.Length - 1;
					if (containerContext1.Position < length)
					{
						if (containerContext1.Type != BsonType.Array)
						{
							this.SetToken(JsonToken.PropertyName, this.ReadElement());
							return true;
						}
						this.ReadElement();
						this.ReadType(this._currentElementType);
						return true;
					}
					if (containerContext1.Position != length)
					{
						throw new JsonReaderException("Read past end of current container context.");
					}
					if (this.ReadByte() != 0)
					{
						throw new JsonReaderException("Unexpected end of object byte value.");
					}
					this.PopContext();
					if (this._currentContext != null)
					{
						this.MovePosition(containerContext1.Length);
					}
					base.SetToken((containerContext1.Type != BsonType.Object ? JsonToken.EndArray : JsonToken.EndObject));
					return true;
				}
				case JsonReader.State.Object:
				case JsonReader.State.Array:
				{
					throw new ArgumentOutOfRangeException();
				}
				case JsonReader.State.ConstructorStart:
				{
					break;
				}
				case JsonReader.State.Constructor:
				{
					break;
				}
				case JsonReader.State.Error:
				{
					break;
				}
				case JsonReader.State.Finished:
				{
					break;
				}
				default:
				{
					throw new ArgumentOutOfRangeException();
				}
			}
			return false;
		}

		private bool ReadReference()
		{
			JsonReader.State currentState = base.CurrentState;
			if (currentState == JsonReader.State.Property)
			{
				if (this._bsonReaderState == BsonReader.BsonReaderState.ReferenceRef)
				{
					this.SetToken(JsonToken.String, this.ReadLengthString());
					return true;
				}
				if (this._bsonReaderState != BsonReader.BsonReaderState.ReferenceId)
				{
					throw new JsonReaderException(string.Concat("Unexpected state when reading BSON reference: ", this._bsonReaderState));
				}
				this.SetToken(JsonToken.Bytes, this.ReadBytes(12));
				return true;
			}
			if (currentState == JsonReader.State.ObjectStart)
			{
				this.SetToken(JsonToken.PropertyName, "$ref");
				this._bsonReaderState = BsonReader.BsonReaderState.ReferenceRef;
				return true;
			}
			if (currentState != JsonReader.State.PostValue)
			{
				throw new JsonReaderException(string.Concat("Unexpected state when reading BSON reference: ", base.CurrentState));
			}
			if (this._bsonReaderState == BsonReader.BsonReaderState.ReferenceRef)
			{
				this.SetToken(JsonToken.PropertyName, "$id");
				this._bsonReaderState = BsonReader.BsonReaderState.ReferenceId;
				return true;
			}
			if (this._bsonReaderState != BsonReader.BsonReaderState.ReferenceId)
			{
				throw new JsonReaderException(string.Concat("Unexpected state when reading BSON reference: ", this._bsonReaderState));
			}
			base.SetToken(JsonToken.EndObject);
			this._bsonReaderState = BsonReader.BsonReaderState.Normal;
			return true;
		}

		private string ReadString()
		{
			this.EnsureBuffers();
			StringBuilder stringBuilder = null;
			int num = 0;
			int num1 = 0;
			while (true)
			{
				int num2 = num1;
				while (num2 < 128)
				{
					byte num3 = this._reader.ReadByte();
					byte num4 = num3;
					if (num3 <= 0)
					{
						break;
					}
					int num5 = num2;
					num2 = num5 + 1;
					this._byteBuffer[num5] = num4;
				}
				int num6 = num2 - num1;
				num += num6;
				if (num2 < 128 && stringBuilder == null)
				{
					int chars = Encoding.UTF8.GetChars(this._byteBuffer, 0, num6, this._charBuffer, 0);
					this.MovePosition(num + 1);
					return new string(this._charBuffer, 0, chars);
				}
				int lastFullCharStop = this.GetLastFullCharStop(num2 - 1);
				int chars1 = Encoding.UTF8.GetChars(this._byteBuffer, 0, lastFullCharStop + 1, this._charBuffer, 0);
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder(256);
				}
				stringBuilder.Append(this._charBuffer, 0, chars1);
				if (lastFullCharStop >= num6 - 1)
				{
					if (num2 < 128)
					{
						break;
					}
					num1 = 0;
				}
				else
				{
					num1 = num6 - lastFullCharStop - 1;
					Array.Copy(this._byteBuffer, lastFullCharStop + 1, this._byteBuffer, 0, num1);
				}
			}
			this.MovePosition(num + 1);
			return stringBuilder.ToString();
		}

		private void ReadType(BsonType type)
		{
			DateTime dateTime;
			switch (type)
			{
				case BsonType.Number:
				{
					this.SetToken(JsonToken.Float, this.ReadDouble());
					break;
				}
				case BsonType.String:
				case BsonType.Symbol:
				{
					this.SetToken(JsonToken.String, this.ReadLengthString());
					break;
				}
				case BsonType.Object:
				{
					base.SetToken(JsonToken.StartObject);
					BsonReader.ContainerContext containerContext = new BsonReader.ContainerContext(BsonType.Object);
					this.PushContext(containerContext);
					containerContext.Length = this.ReadInt32();
					break;
				}
				case BsonType.Array:
				{
					base.SetToken(JsonToken.StartArray);
					BsonReader.ContainerContext containerContext1 = new BsonReader.ContainerContext(BsonType.Array);
					this.PushContext(containerContext1);
					containerContext1.Length = this.ReadInt32();
					break;
				}
				case BsonType.Binary:
				{
					this.SetToken(JsonToken.Bytes, this.ReadBinary());
					break;
				}
				case BsonType.Undefined:
				{
					base.SetToken(JsonToken.Undefined);
					break;
				}
				case BsonType.Oid:
				{
					this.SetToken(JsonToken.Bytes, this.ReadBytes(12));
					break;
				}
				case BsonType.Boolean:
				{
					bool flag = Convert.ToBoolean(this.ReadByte());
					this.SetToken(JsonToken.Boolean, flag);
					break;
				}
				case BsonType.Date:
				{
					DateTime dateTime1 = JsonConvert.ConvertJavaScriptTicksToDateTime(this.ReadInt64());
					DateTimeKind dateTimeKindHandling = this.DateTimeKindHandling;
					if (dateTimeKindHandling == DateTimeKind.Unspecified)
					{
						dateTime = DateTime.SpecifyKind(dateTime1, DateTimeKind.Unspecified);
					}
					else
					{
						dateTime = (dateTimeKindHandling == DateTimeKind.Local ? dateTime1.ToLocalTime() : dateTime1);
					}
					this.SetToken(JsonToken.Date, dateTime);
					break;
				}
				case BsonType.Null:
				{
					base.SetToken(JsonToken.Null);
					break;
				}
				case BsonType.Regex:
				{
					string str = this.ReadString();
					string str1 = this.ReadString();
					string str2 = string.Concat("/", str, "/", str1);
					this.SetToken(JsonToken.String, str2);
					break;
				}
				case BsonType.Reference:
				{
					base.SetToken(JsonToken.StartObject);
					this._bsonReaderState = BsonReader.BsonReaderState.ReferenceStart;
					break;
				}
				case BsonType.Code:
				{
					this.SetToken(JsonToken.String, this.ReadLengthString());
					break;
				}
				case BsonType.CodeWScope:
				{
					base.SetToken(JsonToken.StartObject);
					this._bsonReaderState = BsonReader.BsonReaderState.CodeWScopeStart;
					break;
				}
				case BsonType.Integer:
				{
					this.SetToken(JsonToken.Integer, (long)this.ReadInt32());
					break;
				}
				case BsonType.TimeStamp:
				case BsonType.Long:
				{
					this.SetToken(JsonToken.Integer, this.ReadInt64());
					break;
				}
				default:
				{
					throw new ArgumentOutOfRangeException("type", string.Concat("Unexpected BsonType value: ", type));
				}
			}
		}

		private BsonType ReadType()
		{
			this.MovePosition(1);
			return (BsonType)this._reader.ReadSByte();
		}

		private enum BsonReaderState
		{
			Normal,
			ReferenceStart,
			ReferenceRef,
			ReferenceId,
			CodeWScopeStart,
			CodeWScopeCode,
			CodeWScopeScope,
			CodeWScopeScopeObject,
			CodeWScopeScopeEnd
		}

		private class ContainerContext
		{
			public readonly BsonType Type;

			public int Length;

			public int Position;

			public ContainerContext(BsonType type)
			{
				this.Type = type;
			}
		}
	}
}