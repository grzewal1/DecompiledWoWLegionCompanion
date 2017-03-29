using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Newtonsoft.Json
{
	public class JsonTextReader : JsonReader, IJsonLineInfo
	{
		private const int LineFeedValue = 10;

		private const int CarriageReturnValue = 13;

		private readonly TextReader _reader;

		private readonly StringBuffer _buffer;

		private char? _lastChar;

		private int _currentLinePosition;

		private int _currentLineNumber;

		private bool _end;

		private JsonTextReader.ReadType _readType;

		private CultureInfo _culture;

		public CultureInfo Culture
		{
			get
			{
				return this._culture ?? CultureInfo.CurrentCulture;
			}
			set
			{
				this._culture = value;
			}
		}

		public int LineNumber
		{
			get
			{
				if (base.CurrentState == JsonReader.State.Start)
				{
					return 0;
				}
				return this._currentLineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				return this._currentLinePosition;
			}
		}

		public JsonTextReader(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			this._reader = reader;
			this._buffer = new StringBuffer(4096);
			this._currentLineNumber = 1;
		}

		public override void Close()
		{
			base.Close();
			if (base.CloseInput && this._reader != null)
			{
				this._reader.Close();
			}
			if (this._buffer != null)
			{
				this._buffer.Clear();
			}
		}

		private JsonReaderException CreateJsonReaderException(string format, params object[] args)
		{
			string str = format.FormatWith(CultureInfo.InvariantCulture, args);
			return new JsonReaderException(str, null, this._currentLineNumber, this._currentLinePosition);
		}

		private bool EatWhitespace(char initialChar, bool oneOrMore, out char finalChar)
		{
			char i;
			bool flag = false;
			for (i = initialChar; i == ' ' || char.IsWhiteSpace(i); i = this.MoveNext())
			{
				flag = true;
			}
			finalChar = i;
			return (!oneOrMore ? true : flag);
		}

		public bool HasLineInfo()
		{
			return true;
		}

		private bool HasNext()
		{
			return this._reader.Peek() != -1;
		}

		private bool IsSeperator(char c)
		{
			char chr = c;
			switch (chr)
			{
				case '\t':
				case '\n':
				case '\r':
				{
					return true;
				}
				default:
				{
					switch (chr)
					{
						case ')':
						{
							if (base.CurrentState == JsonReader.State.Constructor || base.CurrentState == JsonReader.State.ConstructorStart)
							{
								return true;
							}
							break;
						}
						case ',':
						{
							return true;
						}
						default:
						{
							if (chr == ' ')
							{
								return true;
							}
							if (chr == '/')
							{
								return (!this.HasNext() ? false : this.PeekNext() == 42);
							}
							if (chr == ']' || chr == '}')
							{
								return true;
							}
							if (char.IsWhiteSpace(c))
							{
								return true;
							}
							break;
						}
					}
					return false;
				}
			}
		}

		private bool MatchValue(char firstChar, string value)
		{
			char chr;
			char chr1 = firstChar;
			int num = 0;
			do
			{
				if (chr1 == value[num])
				{
					num++;
					if (num >= value.Length)
					{
						break;
					}
					chr = this.MoveNext();
					chr1 = chr;
				}
				else
				{
					break;
				}
			}
			while (chr != 0 || !this._end);
			return num == value.Length;
		}

		private bool MatchValue(char firstChar, string value, bool noTrailingNonSeperatorCharacters)
		{
			bool flag;
			bool flag1 = this.MatchValue(firstChar, value);
			if (!noTrailingNonSeperatorCharacters)
			{
				return flag1;
			}
			int num = this.PeekNext();
			char chr = (num == -1 ? '\0' : (char)num);
			if (!flag1)
			{
				flag = false;
			}
			else
			{
				flag = (chr == 0 ? true : this.IsSeperator(chr));
			}
			return flag;
		}

		private char MoveNext()
		{
			int num = this._reader.Read();
			int num1 = num;
			switch (num1)
			{
				case 10:
				{
					JsonTextReader jsonTextReader = this;
					jsonTextReader._currentLineNumber = jsonTextReader._currentLineNumber + 1;
					this._currentLinePosition = 0;
					break;
				}
				case 13:
				{
					if (this._reader.Peek() == 10)
					{
						this._reader.Read();
					}
					JsonTextReader jsonTextReader1 = this;
					jsonTextReader1._currentLineNumber = jsonTextReader1._currentLineNumber + 1;
					this._currentLinePosition = 0;
					break;
				}
				default:
				{
					if (num1 == -1)
					{
						this._end = true;
						return '\0';
					}
					JsonTextReader jsonTextReader2 = this;
					jsonTextReader2._currentLinePosition = jsonTextReader2._currentLinePosition + 1;
					break;
				}
			}
			return (char)num;
		}

		private void ParseComment()
		{
			char chr = this.MoveNext();
			if (chr != '*')
			{
				throw this.CreateJsonReaderException("Error parsing comment. Expected: *. Line {0}, position {1}.", new object[] { this._currentLineNumber, this._currentLinePosition });
			}
			while (true)
			{
				char chr1 = this.MoveNext();
				chr = chr1;
				if (chr1 == 0 && this._end)
				{
					break;
				}
				if (chr != '*')
				{
					this._buffer.Append(chr);
				}
				else
				{
					char chr2 = this.MoveNext();
					chr = chr2;
					if (chr2 != 0 || !this._end)
					{
						if (chr != '/')
						{
							this._buffer.Append('*');
							this._buffer.Append(chr);
						}
						else
						{
							break;
						}
					}
				}
			}
			this.SetToken(JsonToken.Comment, this._buffer.ToString());
			this._buffer.Position = 0;
		}

		private void ParseConstructor()
		{
			if (this.MatchValue('n', "new", true))
			{
				char chr = this.MoveNext();
				if (this.EatWhitespace(chr, true, out chr))
				{
					while (char.IsLetter(chr))
					{
						this._buffer.Append(chr);
						chr = this.MoveNext();
					}
					this.EatWhitespace(chr, false, out chr);
					if (chr != '(')
					{
						throw this.CreateJsonReaderException("Unexpected character while parsing constructor: {0}. Line {1}, position {2}.", new object[] { chr, this._currentLineNumber, this._currentLinePosition });
					}
					string str = this._buffer.ToString();
					this._buffer.Position = 0;
					this.SetToken(JsonToken.StartConstructor, str);
				}
			}
		}

		private void ParseDate(string text)
		{
			DateTime localTime;
			string str = text.Substring(6, text.Length - 8);
			DateTimeKind dateTimeKind = DateTimeKind.Utc;
			int num = str.IndexOf('+', 1);
			if (num == -1)
			{
				num = str.IndexOf('-', 1);
			}
			TimeSpan zero = TimeSpan.Zero;
			if (num != -1)
			{
				dateTimeKind = DateTimeKind.Local;
				zero = this.ReadOffset(str.Substring(num));
				str = str.Substring(0, num);
			}
			long num1 = long.Parse(str, NumberStyles.Integer, CultureInfo.InvariantCulture);
			DateTime dateTime = JsonConvert.ConvertJavaScriptTicksToDateTime(num1);
			if (this._readType != JsonTextReader.ReadType.ReadAsDateTimeOffset)
			{
				switch (dateTimeKind)
				{
					case DateTimeKind.Unspecified:
					{
						localTime = DateTime.SpecifyKind(dateTime.ToLocalTime(), DateTimeKind.Unspecified);
						break;
					}
					case DateTimeKind.Utc:
					{
						localTime = dateTime;
						break;
					}
					case DateTimeKind.Local:
					{
						localTime = dateTime.ToLocalTime();
						break;
					}
					default:
					{
						goto case DateTimeKind.Utc;
					}
				}
				this.SetToken(JsonToken.Date, localTime);
			}
			else
			{
				DateTime dateTime1 = dateTime.Add(zero);
				this.SetToken(JsonToken.Date, new DateTimeOffset(dateTime1.Ticks, zero));
			}
		}

		private void ParseFalse()
		{
			if (!this.MatchValue('f', JsonConvert.False, true))
			{
				throw this.CreateJsonReaderException("Error parsing boolean value. Line {0}, position {1}.", new object[] { this._currentLineNumber, this._currentLinePosition });
			}
			this.SetToken(JsonToken.Boolean, false);
		}

		private void ParseNull()
		{
			if (!this.MatchValue('n', JsonConvert.Null, true))
			{
				throw this.CreateJsonReaderException("Error parsing null value. Line {0}, position {1}.", new object[] { this._currentLineNumber, this._currentLinePosition });
			}
			base.SetToken(JsonToken.Null);
		}

		private void ParseNumber(char firstChar)
		{
			object num;
			JsonToken jsonToken;
			char chr;
			char chr1 = firstChar;
			bool flag = false;
			do
			{
				if (!this.IsSeperator(chr1))
				{
					this._buffer.Append(chr1);
				}
				else
				{
					flag = true;
					this._lastChar = new char?(chr1);
				}
				if (flag)
				{
					break;
				}
				chr = this.MoveNext();
				chr1 = chr;
			}
			while (chr != 0 || !this._end);
			string str = this._buffer.ToString();
			bool flag1 = (firstChar != '0' ? false : !str.StartsWith("0.", StringComparison.OrdinalIgnoreCase));
			if (this._readType == JsonTextReader.ReadType.ReadAsDecimal)
			{
				num = (!flag1 ? decimal.Parse(str, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent | NumberStyles.Integer | NumberStyles.Number | NumberStyles.Float, CultureInfo.InvariantCulture) : Convert.ToDecimal((!str.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(str, 8) : Convert.ToInt64(str, 16))));
				jsonToken = JsonToken.Float;
			}
			else if (flag1)
			{
				num = (!str.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(str, 8) : Convert.ToInt64(str, 16));
				jsonToken = JsonToken.Integer;
			}
			else if (str.IndexOf(".", StringComparison.OrdinalIgnoreCase) != -1 || str.IndexOf("e", StringComparison.OrdinalIgnoreCase) != -1)
			{
				num = Convert.ToDouble(str, CultureInfo.InvariantCulture);
				jsonToken = JsonToken.Float;
			}
			else
			{
				try
				{
					num = Convert.ToInt64(str, CultureInfo.InvariantCulture);
				}
				catch (OverflowException overflowException1)
				{
					OverflowException overflowException = overflowException1;
					throw new JsonReaderException("JSON integer {0} is too large or small for an Int64.".FormatWith(CultureInfo.InvariantCulture, new object[] { str }), overflowException);
				}
				jsonToken = JsonToken.Integer;
			}
			this._buffer.Position = 0;
			this.SetToken(jsonToken, num);
		}

		private void ParseNumberNaN()
		{
			if (!this.MatchValue('N', JsonConvert.NaN, true))
			{
				throw this.CreateJsonReaderException("Error parsing NaN value. Line {0}, position {1}.", new object[] { this._currentLineNumber, this._currentLinePosition });
			}
			this.SetToken(JsonToken.Float, Double.NaN);
		}

		private void ParseNumberNegativeInfinity()
		{
			if (!this.MatchValue('-', JsonConvert.NegativeInfinity, true))
			{
				throw this.CreateJsonReaderException("Error parsing negative infinity value. Line {0}, position {1}.", new object[] { this._currentLineNumber, this._currentLinePosition });
			}
			this.SetToken(JsonToken.Float, Double.NegativeInfinity);
		}

		private void ParseNumberPositiveInfinity()
		{
			if (!this.MatchValue('I', JsonConvert.PositiveInfinity, true))
			{
				throw this.CreateJsonReaderException("Error parsing positive infinity value. Line {0}, position {1}.", new object[] { this._currentLineNumber, this._currentLinePosition });
			}
			this.SetToken(JsonToken.Float, Double.PositiveInfinity);
		}

		private bool ParseObject(char currentChar)
		{
			char chr;
			do
			{
				char chr1 = currentChar;
				switch (chr1)
				{
					case '\t':
					case '\n':
					case '\r':
					{
						break;
					}
					default:
					{
						if (chr1 == ' ')
						{
							goto case '\r';
						}
						if (chr1 == '/')
						{
							this.ParseComment();
							return true;
						}
						if (chr1 == '}')
						{
							base.SetToken(JsonToken.EndObject);
							return true;
						}
						if (!char.IsWhiteSpace(currentChar))
						{
							return this.ParseProperty(currentChar);
						}
						break;
					}
				}
				chr = this.MoveNext();
				currentChar = chr;
			}
			while (chr != 0 || !this._end);
			return false;
		}

		private bool ParsePostValue(char currentChar)
		{
			char chr;
			do
			{
				char chr1 = currentChar;
				switch (chr1)
				{
					case '\t':
					case '\n':
					case '\r':
					{
					Label0:
						break;
					}
					default:
					{
						switch (chr1)
						{
							case ')':
							{
								base.SetToken(JsonToken.EndConstructor);
								return true;
							}
							case ',':
							{
								base.SetStateBasedOnCurrent();
								return false;
							}
							default:
							{
								if (chr1 == ' ')
								{
									goto Label0;
								}
								if (chr1 == '/')
								{
									this.ParseComment();
									return true;
								}
								if (chr1 == ']')
								{
									base.SetToken(JsonToken.EndArray);
									return true;
								}
								if (chr1 == '}')
								{
									base.SetToken(JsonToken.EndObject);
									return true;
								}
								if (!char.IsWhiteSpace(currentChar))
								{
									throw this.CreateJsonReaderException("After parsing a value an unexpected character was encountered: {0}. Line {1}, position {2}.", new object[] { currentChar, this._currentLineNumber, this._currentLinePosition });
								}
								break;
							}
						}
						break;
					}
				}
				chr = this.MoveNext();
				currentChar = chr;
			}
			while (chr != 0 || !this._end);
			return false;
		}

		private bool ParseProperty(char firstChar)
		{
			char chr;
			char chr1 = firstChar;
			if (!this.ValidIdentifierChar(chr1))
			{
				if (chr1 != '\"' && chr1 != '\'')
				{
					throw this.CreateJsonReaderException("Invalid property identifier character: {0}. Line {1}, position {2}.", new object[] { chr1, this._currentLineNumber, this._currentLinePosition });
				}
				chr = chr1;
				this.ReadStringIntoBuffer(chr);
				chr1 = this.MoveNext();
			}
			else
			{
				chr = '\0';
				chr1 = this.ParseUnquotedProperty(chr1);
			}
			if (chr1 != ':')
			{
				chr1 = this.MoveNext();
				this.EatWhitespace(chr1, false, out chr1);
				if (chr1 != ':')
				{
					throw this.CreateJsonReaderException("Invalid character after parsing property name. Expected ':' but got: {0}. Line {1}, position {2}.", new object[] { chr1, this._currentLineNumber, this._currentLinePosition });
				}
			}
			this.SetToken(JsonToken.PropertyName, this._buffer.ToString());
			this.QuoteChar = chr;
			this._buffer.Position = 0;
			return true;
		}

		private void ParseString(char quote)
		{
			byte[] numArray;
			this.ReadStringIntoBuffer(quote);
			if (this._readType != JsonTextReader.ReadType.ReadAsBytes)
			{
				string str = this._buffer.ToString();
				this._buffer.Position = 0;
				if (!str.StartsWith("/Date(", StringComparison.Ordinal) || !str.EndsWith(")/", StringComparison.Ordinal))
				{
					this.SetToken(JsonToken.String, str);
					this.QuoteChar = quote;
				}
				else
				{
					this.ParseDate(str);
				}
			}
			else
			{
				if (this._buffer.Position != 0)
				{
					numArray = Convert.FromBase64CharArray(this._buffer.GetInternalBuffer(), 0, this._buffer.Position);
					this._buffer.Position = 0;
				}
				else
				{
					numArray = new byte[0];
				}
				this.SetToken(JsonToken.Bytes, numArray);
			}
		}

		private void ParseTrue()
		{
			if (!this.MatchValue('t', JsonConvert.True, true))
			{
				throw this.CreateJsonReaderException("Error parsing boolean value. Line {0}, position {1}.", new object[] { this._currentLineNumber, this._currentLinePosition });
			}
			this.SetToken(JsonToken.Boolean, true);
		}

		private void ParseUndefined()
		{
			if (!this.MatchValue('u', JsonConvert.Undefined, true))
			{
				throw this.CreateJsonReaderException("Error parsing undefined value. Line {0}, position {1}.", new object[] { this._currentLineNumber, this._currentLinePosition });
			}
			base.SetToken(JsonToken.Undefined);
		}

		private char ParseUnquotedProperty(char firstChar)
		{
			char chr;
			this._buffer.Append(firstChar);
			while (true)
			{
				char chr1 = this.MoveNext();
				chr = chr1;
				if (chr1 == 0 && this._end)
				{
					throw this.CreateJsonReaderException("Unexpected end when parsing unquoted property name. Line {0}, position {1}.", new object[] { this._currentLineNumber, this._currentLinePosition });
				}
				if (char.IsWhiteSpace(chr) || chr == ':')
				{
					break;
				}
				if (!this.ValidIdentifierChar(chr))
				{
					throw this.CreateJsonReaderException("Invalid JavaScript property identifier character: {0}. Line {1}, position {2}.", new object[] { chr, this._currentLineNumber, this._currentLinePosition });
				}
				this._buffer.Append(chr);
			}
			return chr;
		}

		private bool ParseValue(char currentChar)
		{
			while (true)
			{
				char chr = currentChar;
				switch (chr)
				{
					case '\'':
					{
						break;
					}
					case ')':
					{
						base.SetToken(JsonToken.EndConstructor);
						return true;
					}
					case ',':
					{
						base.SetToken(JsonToken.Undefined);
						return true;
					}
					case '-':
					{
						if (this.PeekNext() != 73)
						{
							this.ParseNumber(currentChar);
						}
						else
						{
							this.ParseNumberNegativeInfinity();
						}
						return true;
					}
					case '/':
					{
						this.ParseComment();
						return true;
					}
					default:
					{
						switch (chr)
						{
							case '\t':
							case '\n':
							case '\r':
							{
							Label1:
								break;
							}
							default:
							{
								switch (chr)
								{
									case ' ':
									{
										goto Label1;
									}
									case '\"':
									{
										this.ParseString(currentChar);
										return true;
									}
									default:
									{
										switch (chr)
										{
											case '[':
											{
												base.SetToken(JsonToken.StartArray);
												return true;
											}
											case ']':
											{
												base.SetToken(JsonToken.EndArray);
												return true;
											}
											default:
											{
												switch (chr)
												{
													case '{':
													{
														base.SetToken(JsonToken.StartObject);
														return true;
													}
													case '}':
													{
														base.SetToken(JsonToken.EndObject);
														return true;
													}
													default:
													{
														if (chr == 't')
														{
															this.ParseTrue();
															return true;
														}
														if (chr == 'u')
														{
															this.ParseUndefined();
															return true;
														}
														if (chr == 'I')
														{
															this.ParseNumberPositiveInfinity();
															return true;
														}
														if (chr == 'N')
														{
															this.ParseNumberNaN();
															return true;
														}
														if (chr == 'f')
														{
															this.ParseFalse();
															return true;
														}
														if (chr == 'n')
														{
															if (!this.HasNext())
															{
																throw this.CreateJsonReaderException("Unexpected end. Line {0}, position {1}.", new object[] { this._currentLineNumber, this._currentLinePosition });
															}
															char chr1 = (char)this.PeekNext();
															if (chr1 != 'u')
															{
																if (chr1 != 'e')
																{
																	throw this.CreateJsonReaderException("Unexpected character encountered while parsing value: {0}. Line {1}, position {2}.", new object[] { currentChar, this._currentLineNumber, this._currentLinePosition });
																}
																this.ParseConstructor();
															}
															else
															{
																this.ParseNull();
															}
															return true;
														}
														if (!char.IsWhiteSpace(currentChar))
														{
															if (!char.IsNumber(currentChar) && currentChar != '-' && currentChar != '.')
															{
																throw this.CreateJsonReaderException("Unexpected character encountered while parsing value: {0}. Line {1}, position {2}.", new object[] { currentChar, this._currentLineNumber, this._currentLinePosition });
															}
															this.ParseNumber(currentChar);
															return true;
														}
														break;
													}
												}
												break;
											}
										}
										break;
									}
								}
								break;
							}
						}
						char chr2 = this.MoveNext();
						currentChar = chr2;
						if (chr2 != 0 || !this._end)
						{
							continue;
						}
						return false;
					}
				}
			}
			this.ParseString(currentChar);
			return true;
		}

		private int PeekNext()
		{
			return this._reader.Peek();
		}

		public override bool Read()
		{
			this._readType = JsonTextReader.ReadType.Read;
			return this.ReadInternal();
		}

		public override byte[] ReadAsBytes()
		{
			this._readType = JsonTextReader.ReadType.ReadAsBytes;
			do
			{
				if (this.ReadInternal())
				{
					continue;
				}
				throw this.CreateJsonReaderException("Unexpected end when reading bytes: Line {0}, position {1}.", new object[] { this._currentLineNumber, this._currentLinePosition });
			}
			while (this.TokenType == JsonToken.Comment);
			if (this.TokenType == JsonToken.Null)
			{
				return null;
			}
			if (this.TokenType == JsonToken.Bytes)
			{
				return (byte[])this.Value;
			}
			if (this.TokenType != JsonToken.StartArray)
			{
				throw this.CreateJsonReaderException("Unexpected token when reading bytes: {0}. Line {1}, position {2}.", new object[] { this.TokenType, this._currentLineNumber, this._currentLinePosition });
			}
			List<byte> nums = new List<byte>();
			while (this.ReadInternal())
			{
				JsonToken tokenType = this.TokenType;
				switch (tokenType)
				{
					case JsonToken.Comment:
					{
						continue;
					}
					case JsonToken.Integer:
					{
						nums.Add(Convert.ToByte(this.Value, CultureInfo.InvariantCulture));
						continue;
					}
					default:
					{
						if (tokenType == JsonToken.EndArray)
						{
							break;
						}
						else
						{
							throw this.CreateJsonReaderException("Unexpected token when reading bytes: {0}. Line {1}, position {2}.", new object[] { this.TokenType, this._currentLineNumber, this._currentLinePosition });
						}
					}
				}
				byte[] array = nums.ToArray();
				this.SetToken(JsonToken.Bytes, array);
				return array;
			}
			throw this.CreateJsonReaderException("Unexpected end when reading bytes: Line {0}, position {1}.", new object[] { this._currentLineNumber, this._currentLinePosition });
		}

		public override DateTimeOffset? ReadAsDateTimeOffset()
		{
			DateTimeOffset dateTimeOffset;
			this._readType = JsonTextReader.ReadType.ReadAsDateTimeOffset;
			do
			{
				if (this.ReadInternal())
				{
					continue;
				}
				throw this.CreateJsonReaderException("Unexpected end when reading date: Line {0}, position {1}.", new object[] { this._currentLineNumber, this._currentLinePosition });
			}
			while (this.TokenType == JsonToken.Comment);
			if (this.TokenType == JsonToken.Null)
			{
				return null;
			}
			if (this.TokenType == JsonToken.Date)
			{
				return new DateTimeOffset?((DateTimeOffset)this.Value);
			}
			if (this.TokenType != JsonToken.String || !DateTimeOffset.TryParse((string)this.Value, this.Culture, DateTimeStyles.None, out dateTimeOffset))
			{
				throw this.CreateJsonReaderException("Unexpected token when reading date: {0}. Line {1}, position {2}.", new object[] { this.TokenType, this._currentLineNumber, this._currentLinePosition });
			}
			this.SetToken(JsonToken.Date, dateTimeOffset);
			return new DateTimeOffset?(dateTimeOffset);
		}

		public override decimal? ReadAsDecimal()
		{
			decimal num;
			this._readType = JsonTextReader.ReadType.ReadAsDecimal;
			do
			{
				if (this.ReadInternal())
				{
					continue;
				}
				throw this.CreateJsonReaderException("Unexpected end when reading decimal: Line {0}, position {1}.", new object[] { this._currentLineNumber, this._currentLinePosition });
			}
			while (this.TokenType == JsonToken.Comment);
			if (this.TokenType == JsonToken.Null)
			{
				return null;
			}
			if (this.TokenType == JsonToken.Float)
			{
				return (decimal?)this.Value;
			}
			if (this.TokenType != JsonToken.String || !decimal.TryParse((string)this.Value, NumberStyles.Number, this.Culture, out num))
			{
				throw this.CreateJsonReaderException("Unexpected token when reading decimal: {0}. Line {1}, position {2}.", new object[] { this.TokenType, this._currentLineNumber, this._currentLinePosition });
			}
			this.SetToken(JsonToken.Float, num);
			return new decimal?(num);
		}

		private bool ReadInternal()
		{
			char value;
			while (true)
			{
				if (!this._lastChar.HasValue)
				{
					value = this.MoveNext();
				}
				else
				{
					value = this._lastChar.Value;
					this._lastChar = null;
				}
				if (value == 0 && this._end)
				{
					return false;
				}
				switch (base.CurrentState)
				{
					case JsonReader.State.Start:
					case JsonReader.State.Property:
					case JsonReader.State.ArrayStart:
					case JsonReader.State.Array:
					case JsonReader.State.ConstructorStart:
					case JsonReader.State.Constructor:
					{
						return this.ParseValue(value);
					}
					case JsonReader.State.Complete:
					{
						break;
					}
					case JsonReader.State.ObjectStart:
					case JsonReader.State.Object:
					{
						return this.ParseObject(value);
					}
					case JsonReader.State.Closed:
					{
						break;
					}
					case JsonReader.State.PostValue:
					{
						if (this.ParsePostValue(value))
						{
							return true;
						}
						break;
					}
					case JsonReader.State.Error:
					{
						break;
					}
					default:
					{
						throw this.CreateJsonReaderException("Unexpected state: {0}. Line {1}, position {2}.", new object[] { base.CurrentState, this._currentLineNumber, this._currentLinePosition });
					}
				}
			}
			return true;
		}

		private TimeSpan ReadOffset(string offsetText)
		{
			bool flag = offsetText[0] == '-';
			int num = int.Parse(offsetText.Substring(1, 2), NumberStyles.Integer, CultureInfo.InvariantCulture);
			int num1 = 0;
			if (offsetText.Length >= 5)
			{
				num1 = int.Parse(offsetText.Substring(3, 2), NumberStyles.Integer, CultureInfo.InvariantCulture);
			}
			TimeSpan timeSpan = TimeSpan.FromHours((double)num) + TimeSpan.FromMinutes((double)num1);
			if (flag)
			{
				timeSpan = timeSpan.Negate();
			}
			return timeSpan;
		}

		private void ReadStringIntoBuffer(char quote)
		{
			while (true)
			{
				char chr = this.MoveNext();
				char chr1 = chr;
				if (chr1 == '\0')
				{
					if (this._end)
					{
						throw this.CreateJsonReaderException("Unterminated string. Expected delimiter: {0}. Line {1}, position {2}.", new object[] { quote, this._currentLineNumber, this._currentLinePosition });
					}
					this._buffer.Append('\0');
				}
				else if (chr1 == '\"' || chr1 == '\'')
				{
					if (chr == quote)
					{
						break;
					}
					this._buffer.Append(chr);
				}
				else if (chr1 == '\\')
				{
					char chr2 = this.MoveNext();
					chr = chr2;
					if (chr2 == 0 && this._end)
					{
						throw this.CreateJsonReaderException("Unterminated string. Expected delimiter: {0}. Line {1}, position {2}.", new object[] { quote, this._currentLineNumber, this._currentLinePosition });
					}
					char chr3 = chr;
					switch (chr3)
					{
						case 'n':
						{
							this._buffer.Append('\n');
							break;
						}
						case 'r':
						{
							this._buffer.Append('\r');
							break;
						}
						case 't':
						{
							this._buffer.Append('\t');
							break;
						}
						case 'u':
						{
							char[] chrArray = new char[4];
							for (int i = 0; i < (int)chrArray.Length; i++)
							{
								char chr4 = this.MoveNext();
								chr = chr4;
								if (chr4 == 0 && this._end)
								{
									throw this.CreateJsonReaderException("Unexpected end while parsing unicode character. Line {0}, position {1}.", new object[] { this._currentLineNumber, this._currentLinePosition });
								}
								chrArray[i] = chr;
							}
							char chr5 = Convert.ToChar(int.Parse(new string(chrArray), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo));
							this._buffer.Append(chr5);
							break;
						}
						default:
						{
							if (chr3 == '\"' || chr3 == '\'' || chr3 == '/')
							{
								this._buffer.Append(chr);
								break;
							}
							else if (chr3 == '\\')
							{
								this._buffer.Append('\\');
								break;
							}
							else if (chr3 == 'b')
							{
								this._buffer.Append('\b');
								break;
							}
							else
							{
								if (chr3 != 'f')
								{
									throw this.CreateJsonReaderException("Bad JSON escape sequence: {0}. Line {1}, position {2}.", new object[] { string.Concat("\\", chr), this._currentLineNumber, this._currentLinePosition });
								}
								this._buffer.Append('\f');
								break;
							}
						}
					}
				}
				else
				{
					this._buffer.Append(chr);
				}
			}
		}

		private bool ValidIdentifierChar(char value)
		{
			return (char.IsLetterOrDigit(value) || value == '\u005F' ? true : value == '$');
		}

		private enum ReadType
		{
			Read,
			ReadAsBytes,
			ReadAsDecimal,
			ReadAsDateTimeOffset
		}
	}
}