using bgs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace MiniJSON
{
	public static class Json
	{
		public static object Deserialize(string json)
		{
			object obj;
			try
			{
				obj = (json != null ? Json.Parser.Parse(json) : null);
			}
			catch (OverflowException overflowException)
			{
				obj = null;
			}
			return obj;
		}

		public static string Serialize(object obj)
		{
			return Json.Serializer.Serialize(obj);
		}

		private sealed class Parser : IDisposable
		{
			private const string WORD_BREAK = "{}[],:\"";

			private StringReader json;

			private char NextChar
			{
				get
				{
					return Convert.ToChar(this.json.Read());
				}
			}

			private Json.Parser.TOKEN NextToken
			{
				get
				{
					string nextWord;
					Dictionary<string, int> strs;
					int num;
					this.EatWhitespace();
					if (this.json.Peek() == -1)
					{
						return Json.Parser.TOKEN.NONE;
					}
					char peekChar = this.PeekChar;
					switch (peekChar)
					{
						case '\"':
						{
							return Json.Parser.TOKEN.STRING;
						}
						case ',':
						{
							this.json.Read();
							return Json.Parser.TOKEN.COMMA;
						}
						case '-':
						case '0':
						case '1':
						case '2':
						case '3':
						case '4':
						case '5':
						case '6':
						case '7':
						case '8':
						case '9':
						{
							return Json.Parser.TOKEN.NUMBER;
						}
						case ':':
						{
							return Json.Parser.TOKEN.COLON;
						}
						default:
						{
							switch (peekChar)
							{
								case '[':
								{
									return Json.Parser.TOKEN.SQUARED_OPEN;
								}
								case ']':
								{
									this.json.Read();
									return Json.Parser.TOKEN.SQUARED_CLOSE;
								}
								default:
								{
									switch (peekChar)
									{
										case '{':
										{
											return Json.Parser.TOKEN.CURLY_OPEN;
										}
										case '|':
										{
											nextWord = this.NextWord;
											if (nextWord != null)
											{
												if (Json.Parser.<>f__switch$mapD == null)
												{
													strs = new Dictionary<string, int>(3)
													{
														{ "false", 0 },
														{ "true", 1 },
														{ "null", 2 }
													};
													Json.Parser.<>f__switch$mapD = strs;
												}
												if (Json.Parser.<>f__switch$mapD.TryGetValue(nextWord, out num))
												{
													switch (num)
													{
														case 0:
														{
															return Json.Parser.TOKEN.FALSE;
														}
														case 1:
														{
															return Json.Parser.TOKEN.TRUE;
														}
														case 2:
														{
															return Json.Parser.TOKEN.NULL;
														}
													}
												}
											}
											return Json.Parser.TOKEN.NONE;
										}
										case '}':
										{
											this.json.Read();
											return Json.Parser.TOKEN.CURLY_CLOSE;
										}
										default:
										{
											nextWord = this.NextWord;
											if (nextWord != null)
											{
												if (Json.Parser.<>f__switch$mapD == null)
												{
													strs = new Dictionary<string, int>(3)
													{
														{ "false", 0 },
														{ "true", 1 },
														{ "null", 2 }
													};
													Json.Parser.<>f__switch$mapD = strs;
												}
												if (Json.Parser.<>f__switch$mapD.TryGetValue(nextWord, out num))
												{
													switch (num)
													{
														case 0:
														{
															return Json.Parser.TOKEN.FALSE;
														}
														case 1:
														{
															return Json.Parser.TOKEN.TRUE;
														}
														case 2:
														{
															return Json.Parser.TOKEN.NULL;
														}
													}
												}
											}
											return Json.Parser.TOKEN.NONE;
										}
									}
									break;
								}
							}
							break;
						}
					}
				}
			}

			private string NextWord
			{
				get
				{
					StringBuilder stringBuilder = new StringBuilder();
					while (!Json.Parser.IsWordBreak(this.PeekChar))
					{
						stringBuilder.Append(this.NextChar);
						if (this.json.Peek() != -1)
						{
							continue;
						}
						break;
					}
					return stringBuilder.ToString();
				}
			}

			private char PeekChar
			{
				get
				{
					return Convert.ToChar(this.json.Peek());
				}
			}

			private Parser(string jsonString)
			{
				this.json = new StringReader(jsonString);
			}

			public void Dispose()
			{
				this.json.Dispose();
				this.json = null;
			}

			private void EatWhitespace()
			{
				while (char.IsWhiteSpace(this.PeekChar))
				{
					this.json.Read();
					if (this.json.Peek() != -1)
					{
						continue;
					}
					break;
				}
			}

			public static bool IsWordBreak(char c)
			{
				return (char.IsWhiteSpace(c) ? true : "{}[],:\"".IndexOf(c) != -1);
			}

			public static object Parse(string jsonString)
			{
				object obj;
				using (Json.Parser parser = new Json.Parser(jsonString))
				{
					obj = parser.ParseValue();
				}
				return obj;
			}

			private JsonList ParseArray()
			{
				Json.Parser.TOKEN nextToken;
				JsonList jsonList = new JsonList();
				this.json.Read();
				bool flag = true;
			Label1:
				while (flag)
				{
					nextToken = this.NextToken;
					Json.Parser.TOKEN tOKEN = nextToken;
					switch (tOKEN)
					{
						case Json.Parser.TOKEN.SQUARED_CLOSE:
						{
							flag = false;
							continue;
						}
						case Json.Parser.TOKEN.COMMA:
						{
							continue;
						}
						default:
						{
							if (tOKEN == Json.Parser.TOKEN.NONE)
							{
								break;
							}
							else
							{
								goto Label0;
							}
						}
					}
					return null;
				}
				return jsonList;
			Label0:
				object obj = this.ParseByToken(nextToken);
				jsonList.Add(obj);
				if (obj == null)
				{
					this.json.Read();
				}
				goto Label1;
			}

			private object ParseByToken(Json.Parser.TOKEN token)
			{
				switch (token)
				{
					case Json.Parser.TOKEN.CURLY_OPEN:
					{
						return this.ParseObject();
					}
					case Json.Parser.TOKEN.CURLY_CLOSE:
					case Json.Parser.TOKEN.SQUARED_CLOSE:
					case Json.Parser.TOKEN.COLON:
					case Json.Parser.TOKEN.COMMA:
					{
						return null;
					}
					case Json.Parser.TOKEN.SQUARED_OPEN:
					{
						return this.ParseArray();
					}
					case Json.Parser.TOKEN.STRING:
					{
						return this.ParseString();
					}
					case Json.Parser.TOKEN.NUMBER:
					{
						return this.ParseNumber();
					}
					case Json.Parser.TOKEN.TRUE:
					{
						return true;
					}
					case Json.Parser.TOKEN.FALSE:
					{
						return false;
					}
					case Json.Parser.TOKEN.NULL:
					{
						return null;
					}
					default:
					{
						return null;
					}
				}
			}

			private object ParseNumber()
			{
				long num;
				double num1;
				string nextWord = this.NextWord;
				if (nextWord.IndexOf('.') == -1)
				{
					long.TryParse(nextWord, NumberStyles.Any, CultureInfo.InvariantCulture, out num);
					return num;
				}
				double.TryParse(nextWord, NumberStyles.Any, CultureInfo.InvariantCulture, out num1);
				return num1;
			}

			private JsonNode ParseObject()
			{
				JsonNode jsonNode = new JsonNode();
				this.json.Read();
				while (true)
				{
					Json.Parser.TOKEN nextToken = this.NextToken;
					switch (nextToken)
					{
						case Json.Parser.TOKEN.NONE:
						{
							return null;
						}
						case Json.Parser.TOKEN.CURLY_CLOSE:
						{
							return jsonNode;
						}
						default:
						{
							if (nextToken == Json.Parser.TOKEN.COMMA)
							{
								continue;
							}
							else
							{
								string str = this.ParseString();
								if (str == null)
								{
									return null;
								}
								if (this.NextToken != Json.Parser.TOKEN.COLON)
								{
									return null;
								}
								this.json.Read();
								jsonNode[str] = this.ParseValue();
								continue;
							}
						}
					}
				}
				return null;
			}

			private string ParseString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				this.json.Read();
				bool flag = true;
				while (flag)
				{
					if (this.json.Peek() != -1)
					{
						char nextChar = this.NextChar;
						char chr = nextChar;
						if (chr == '\"')
						{
							flag = false;
						}
						else if (chr != '\\')
						{
							stringBuilder.Append(nextChar);
						}
						else if (this.json.Peek() != -1)
						{
							nextChar = this.NextChar;
							char chr1 = nextChar;
							switch (chr1)
							{
								case 'n':
								{
									stringBuilder.Append('\n');
									break;
								}
								case 'r':
								{
									stringBuilder.Append('\r');
									break;
								}
								case 't':
								{
									stringBuilder.Append('\t');
									break;
								}
								case 'u':
								{
									char[] chrArray = new char[4];
									for (int i = 0; i < 4; i++)
									{
										chrArray[i] = this.NextChar;
									}
									stringBuilder.Append((char)Convert.ToInt32(new string(chrArray), 16));
									break;
								}
								default:
								{
									if (chr1 == '\"' || chr1 == '/' || chr1 == '\\')
									{
										stringBuilder.Append(nextChar);
										break;
									}
									else if (chr1 == 'b')
									{
										stringBuilder.Append('\b');
										break;
									}
									else if (chr1 == 'f')
									{
										stringBuilder.Append('\f');
										break;
									}
									else
									{
										break;
									}
								}
							}
						}
						else
						{
							flag = false;
						}
					}
					else
					{
						flag = false;
						break;
					}
				}
				return stringBuilder.ToString();
			}

			private object ParseValue()
			{
				return this.ParseByToken(this.NextToken);
			}

			private enum TOKEN
			{
				NONE,
				CURLY_OPEN,
				CURLY_CLOSE,
				SQUARED_OPEN,
				SQUARED_CLOSE,
				COLON,
				COMMA,
				STRING,
				NUMBER,
				TRUE,
				FALSE,
				NULL
			}
		}

		private sealed class Serializer
		{
			private StringBuilder builder;

			private Serializer()
			{
				this.builder = new StringBuilder();
			}

			public static string Serialize(object obj)
			{
				Json.Serializer serializer = new Json.Serializer();
				serializer.SerializeValue(obj);
				return serializer.builder.ToString();
			}

			private void SerializeArray(JsonList anArray)
			{
				this.builder.Append('[');
				bool flag = true;
				for (int i = 0; i < anArray.Count; i++)
				{
					object item = anArray[i];
					if (!flag)
					{
						this.builder.Append(',');
					}
					this.SerializeValue(item);
					flag = false;
				}
				this.builder.Append(']');
			}

			private void SerializeObject(JsonNode obj)
			{
				bool flag = true;
				this.builder.Append('{');
				foreach (object key in obj.Keys)
				{
					if (!flag)
					{
						this.builder.Append(',');
					}
					this.SerializeString(key.ToString());
					this.builder.Append(':');
					this.SerializeValue(obj[(string)key]);
					flag = false;
				}
				this.builder.Append('}');
			}

			private void SerializeOther(object value)
			{
				if (value is float)
				{
					StringBuilder stringBuilder = this.builder;
					float single = (float)value;
					stringBuilder.Append(single.ToString("R", CultureInfo.InvariantCulture));
				}
				else if (value is int || value is uint || value is long || value is sbyte || value is byte || value is short || value is ushort || value is ulong)
				{
					this.builder.Append(value);
				}
				else if (value is double || value is decimal)
				{
					StringBuilder stringBuilder1 = this.builder;
					double num = Convert.ToDouble(value);
					stringBuilder1.Append(num.ToString("R", CultureInfo.InvariantCulture));
				}
				else
				{
					this.SerializeString(value.ToString());
				}
			}

			private void SerializeString(string str)
			{
				this.builder.Append('\"');
				char[] charArray = str.ToCharArray();
				for (int i = 0; i < (int)charArray.Length; i++)
				{
					char chr = charArray[i];
					char chr1 = chr;
					switch (chr1)
					{
						case '\b':
						{
							this.builder.Append("\\b");
							break;
						}
						case '\t':
						{
							this.builder.Append("\\t");
							break;
						}
						case '\n':
						{
							this.builder.Append("\\n");
							break;
						}
						case '\f':
						{
							this.builder.Append("\\f");
							break;
						}
						case '\r':
						{
							this.builder.Append("\\r");
							break;
						}
						default:
						{
							if (chr1 == '\"')
							{
								this.builder.Append("\\\"");
								break;
							}
							else if (chr1 == '\\')
							{
								this.builder.Append("\\\\");
								break;
							}
							else
							{
								int num = Convert.ToInt32(chr);
								if (num < 32 || num > 126)
								{
									this.builder.Append("\\u");
									this.builder.Append(num.ToString("x4"));
								}
								else
								{
									this.builder.Append(chr);
								}
								break;
							}
						}
					}
				}
				this.builder.Append('\"');
			}

			private void SerializeValue(object value)
			{
				if (value != null)
				{
					string str = value as string;
					string str1 = str;
					if (str != null)
					{
						this.SerializeString(str1);
					}
					else if (!(value is bool))
					{
						JsonList jsonList = value as JsonList;
						JsonList jsonList1 = jsonList;
						if (jsonList == null)
						{
							JsonNode jsonNode = value as JsonNode;
							JsonNode jsonNode1 = jsonNode;
							if (jsonNode != null)
							{
								this.SerializeObject(jsonNode1);
							}
							else if (!(value is char))
							{
								this.SerializeOther(value);
							}
							else
							{
								this.SerializeString(new string((char)value, 1));
							}
						}
						else
						{
							this.SerializeArray(jsonList1);
						}
					}
					else
					{
						this.builder.Append((!(bool)value ? "false" : "true"));
					}
				}
				else
				{
					this.builder.Append("null");
				}
			}
		}
	}
}