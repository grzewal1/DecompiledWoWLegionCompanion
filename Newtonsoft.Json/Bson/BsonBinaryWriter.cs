using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Newtonsoft.Json.Bson
{
	internal class BsonBinaryWriter
	{
		private readonly static System.Text.Encoding Encoding;

		private readonly BinaryWriter _writer;

		private byte[] _largeByteBuffer;

		private int _maxChars;

		public DateTimeKind DateTimeKindHandling
		{
			get;
			set;
		}

		static BsonBinaryWriter()
		{
			BsonBinaryWriter.Encoding = System.Text.Encoding.UTF8;
		}

		public BsonBinaryWriter(Stream stream)
		{
			this.DateTimeKindHandling = DateTimeKind.Utc;
			this._writer = new BinaryWriter(stream);
		}

		private int CalculateSize(int stringByteCount)
		{
			return stringByteCount + 1;
		}

		private int CalculateSize(BsonToken t)
		{
			object[] type;
			CultureInfo invariantCulture;
			switch (t.Type)
			{
				case BsonType.Number:
				{
					return 8;
				}
				case BsonType.String:
				{
					BsonString bsonString = (BsonString)t;
					string value = (string)bsonString.Value;
					bsonString.ByteCount = (value == null ? 0 : BsonBinaryWriter.Encoding.GetByteCount(value));
					bsonString.CalculatedSize = this.CalculateSizeWithLength(bsonString.ByteCount, bsonString.IncludeLength);
					return bsonString.CalculatedSize;
				}
				case BsonType.Object:
				{
					BsonObject bsonObjects = (BsonObject)t;
					int num = 4;
					IEnumerator<BsonProperty> enumerator = bsonObjects.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							BsonProperty current = enumerator.Current;
							int num1 = 1 + this.CalculateSize(current.Name);
							num1 = num1 + this.CalculateSize(current.Value);
							num = num + num1;
						}
					}
					finally
					{
						if (enumerator == null)
						{
						}
						enumerator.Dispose();
					}
					num++;
					bsonObjects.CalculatedSize = num;
					return num;
				}
				case BsonType.Array:
				{
					BsonArray bsonArrays = (BsonArray)t;
					int num2 = 4;
					int num3 = 0;
					IEnumerator<BsonToken> enumerator1 = bsonArrays.GetEnumerator();
					try
					{
						while (enumerator1.MoveNext())
						{
							BsonToken bsonToken = enumerator1.Current;
							num2++;
							num2 = num2 + this.CalculateSize(MathUtils.IntLength(num3));
							num2 = num2 + this.CalculateSize(bsonToken);
							num3++;
						}
					}
					finally
					{
						if (enumerator1 == null)
						{
						}
						enumerator1.Dispose();
					}
					num2++;
					bsonArrays.CalculatedSize = num2;
					return bsonArrays.CalculatedSize;
				}
				case BsonType.Binary:
				{
					BsonValue length = (BsonValue)t;
					byte[] numArray = (byte[])length.Value;
					length.CalculatedSize = 5 + (int)numArray.Length;
					return length.CalculatedSize;
				}
				case BsonType.Undefined:
				case BsonType.Null:
				{
					return 0;
				}
				case BsonType.Oid:
				{
					return 12;
				}
				case BsonType.Boolean:
				{
					return 1;
				}
				case BsonType.Date:
				{
					return 8;
				}
				case BsonType.Regex:
				{
					BsonRegex bsonRegex = (BsonRegex)t;
					int num4 = 0 + this.CalculateSize(bsonRegex.Pattern);
					num4 = num4 + this.CalculateSize(bsonRegex.Options);
					bsonRegex.CalculatedSize = num4;
					return bsonRegex.CalculatedSize;
				}
				case BsonType.Reference:
				case BsonType.Code:
				case BsonType.Symbol:
				case BsonType.CodeWScope:
				case BsonType.TimeStamp:
				{
					invariantCulture = CultureInfo.InvariantCulture;
					type = new object[] { t.Type };
					throw new ArgumentOutOfRangeException("t", "Unexpected token when writing BSON: {0}".FormatWith(invariantCulture, type));
				}
				case BsonType.Integer:
				{
					return 4;
				}
				case BsonType.Long:
				{
					return 8;
				}
				default:
				{
					invariantCulture = CultureInfo.InvariantCulture;
					type = new object[] { t.Type };
					throw new ArgumentOutOfRangeException("t", "Unexpected token when writing BSON: {0}".FormatWith(invariantCulture, type));
				}
			}
		}

		private int CalculateSizeWithLength(int stringByteCount, bool includeSize)
		{
			return (!includeSize ? 1 : 5) + stringByteCount;
		}

		public void Close()
		{
			this._writer.Close();
		}

		public void Flush()
		{
			this._writer.Flush();
		}

		private void WriteString(string s, int byteCount, int? calculatedlengthPrefix)
		{
			int num = 0;
			if (calculatedlengthPrefix.HasValue)
			{
				this._writer.Write(calculatedlengthPrefix.Value);
			}
			if (s != null)
			{
				if (this._largeByteBuffer == null)
				{
					this._largeByteBuffer = new byte[256];
					this._maxChars = 256 / BsonBinaryWriter.Encoding.GetMaxByteCount(1);
				}
				if (byteCount > 256)
				{
					int num1 = 0;
					for (int i = s.Length; i > 0; i = i - num)
					{
						num = (i <= this._maxChars ? i : this._maxChars);
						int bytes = BsonBinaryWriter.Encoding.GetBytes(s, num1, num, this._largeByteBuffer, 0);
						this._writer.Write(this._largeByteBuffer, 0, bytes);
						num1 = num1 + num;
					}
				}
				else
				{
					BsonBinaryWriter.Encoding.GetBytes(s, 0, s.Length, this._largeByteBuffer, 0);
					this._writer.Write(this._largeByteBuffer, 0, byteCount);
				}
			}
			this._writer.Write((byte)0);
		}

		public void WriteToken(BsonToken t)
		{
			this.CalculateSize(t);
			this.WriteTokenInternal(t);
		}

		private void WriteTokenInternal(BsonToken t)
		{
			object[] type;
			CultureInfo invariantCulture;
			switch (t.Type)
			{
				case BsonType.Number:
				{
					BsonValue bsonValue = (BsonValue)t;
					this._writer.Write(Convert.ToDouble(bsonValue.Value, CultureInfo.InvariantCulture));
					break;
				}
				case BsonType.String:
				{
					BsonString bsonString = (BsonString)t;
					this.WriteString((string)bsonString.Value, bsonString.ByteCount, new int?(bsonString.CalculatedSize - 4));
					break;
				}
				case BsonType.Object:
				{
					BsonObject bsonObjects = (BsonObject)t;
					this._writer.Write(bsonObjects.CalculatedSize);
					IEnumerator<BsonProperty> enumerator = bsonObjects.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							BsonProperty current = enumerator.Current;
							this._writer.Write((sbyte)current.Value.Type);
							int? nullable = null;
							this.WriteString((string)current.Name.Value, current.Name.ByteCount, nullable);
							this.WriteTokenInternal(current.Value);
						}
					}
					finally
					{
						if (enumerator == null)
						{
						}
						enumerator.Dispose();
					}
					this._writer.Write((byte)0);
					break;
				}
				case BsonType.Array:
				{
					BsonArray bsonArrays = (BsonArray)t;
					this._writer.Write(bsonArrays.CalculatedSize);
					int num = 0;
					IEnumerator<BsonToken> enumerator1 = bsonArrays.GetEnumerator();
					try
					{
						while (enumerator1.MoveNext())
						{
							BsonToken bsonToken = enumerator1.Current;
							this._writer.Write((sbyte)bsonToken.Type);
							int? nullable1 = null;
							this.WriteString(num.ToString(CultureInfo.InvariantCulture), MathUtils.IntLength(num), nullable1);
							this.WriteTokenInternal(bsonToken);
							num++;
						}
					}
					finally
					{
						if (enumerator1 == null)
						{
						}
						enumerator1.Dispose();
					}
					this._writer.Write((byte)0);
					break;
				}
				case BsonType.Binary:
				{
					byte[] value = (byte[])((BsonValue)t).Value;
					this._writer.Write((int)value.Length);
					this._writer.Write((byte)0);
					this._writer.Write(value);
					break;
				}
				case BsonType.Undefined:
				case BsonType.Null:
				{
					break;
				}
				case BsonType.Oid:
				{
					byte[] numArray = (byte[])((BsonValue)t).Value;
					this._writer.Write(numArray);
					break;
				}
				case BsonType.Boolean:
				{
					BsonValue bsonValue1 = (BsonValue)t;
					this._writer.Write((bool)bsonValue1.Value);
					break;
				}
				case BsonType.Date:
				{
					BsonValue bsonValue2 = (BsonValue)t;
					long javaScriptTicks = (long)0;
					if (!(bsonValue2.Value is DateTime))
					{
						DateTimeOffset dateTimeOffset = (DateTimeOffset)bsonValue2.Value;
						javaScriptTicks = JsonConvert.ConvertDateTimeToJavaScriptTicks(dateTimeOffset.UtcDateTime, dateTimeOffset.Offset);
					}
					else
					{
						DateTime universalTime = (DateTime)bsonValue2.Value;
						if (this.DateTimeKindHandling == DateTimeKind.Utc)
						{
							universalTime = universalTime.ToUniversalTime();
						}
						else if (this.DateTimeKindHandling == DateTimeKind.Local)
						{
							universalTime = universalTime.ToLocalTime();
						}
						javaScriptTicks = JsonConvert.ConvertDateTimeToJavaScriptTicks(universalTime, false);
					}
					this._writer.Write(javaScriptTicks);
					break;
				}
				case BsonType.Regex:
				{
					BsonRegex bsonRegex = (BsonRegex)t;
					int? nullable2 = null;
					this.WriteString((string)bsonRegex.Pattern.Value, bsonRegex.Pattern.ByteCount, nullable2);
					int? nullable3 = null;
					this.WriteString((string)bsonRegex.Options.Value, bsonRegex.Options.ByteCount, nullable3);
					break;
				}
				case BsonType.Reference:
				case BsonType.Code:
				case BsonType.Symbol:
				case BsonType.CodeWScope:
				case BsonType.TimeStamp:
				{
					invariantCulture = CultureInfo.InvariantCulture;
					type = new object[] { t.Type };
					throw new ArgumentOutOfRangeException("t", "Unexpected token when writing BSON: {0}".FormatWith(invariantCulture, type));
				}
				case BsonType.Integer:
				{
					BsonValue bsonValue3 = (BsonValue)t;
					this._writer.Write(Convert.ToInt32(bsonValue3.Value, CultureInfo.InvariantCulture));
					break;
				}
				case BsonType.Long:
				{
					BsonValue bsonValue4 = (BsonValue)t;
					this._writer.Write(Convert.ToInt64(bsonValue4.Value, CultureInfo.InvariantCulture));
					break;
				}
				default:
				{
					invariantCulture = CultureInfo.InvariantCulture;
					type = new object[] { t.Type };
					throw new ArgumentOutOfRangeException("t", "Unexpected token when writing BSON: {0}".FormatWith(invariantCulture, type));
				}
			}
		}
	}
}