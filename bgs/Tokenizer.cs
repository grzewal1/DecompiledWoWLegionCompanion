using System;
using System.Text;

namespace bgs
{
	public class Tokenizer
	{
		private char[] m_chars;

		private int m_index;

		private const char NULLCHAR = '\0';

		public Tokenizer(string str)
		{
			this.m_chars = str.ToCharArray();
			this.m_index = 0;
		}

		public void ClearWhiteSpace()
		{
			while (this.IsWhiteSpace(this.CurrentChar()))
			{
				this.NextChar();
			}
		}

		private char CurrentChar()
		{
			if (this.m_index >= (int)this.m_chars.Length)
			{
				return '\0';
			}
			return this.m_chars[this.m_index];
		}

		private bool IsDecimal(char c)
		{
			return (c < '0' ? false : c <= '9');
		}

		private bool IsEOF()
		{
			return this.m_index >= (int)this.m_chars.Length;
		}

		private bool IsWhiteSpace(char c)
		{
			return (c == 0 ? false : c <= ' ');
		}

		private char NextChar()
		{
			if (this.m_index >= (int)this.m_chars.Length)
			{
				return '\0';
			}
			char mChars = this.m_chars[this.m_index];
			this.m_index++;
			return mChars;
		}

		private bool NextCharIsWhiteSpace()
		{
			if (this.m_index + 1 >= (int)this.m_chars.Length)
			{
				return false;
			}
			return this.IsWhiteSpace(this.m_chars[this.m_index + 1]);
		}

		public float NextFloat()
		{
			this.ClearWhiteSpace();
			float single = 1f;
			float single1 = 0f;
			if (this.CurrentChar() == '-')
			{
				single = -1f;
				this.NextChar();
			}
			bool flag = false;
			float single2 = 1f;
			while (true)
			{
				char chr = this.CurrentChar();
				if (chr == 0 || this.IsWhiteSpace(chr))
				{
					break;
				}
				else if (chr == 'f')
				{
					this.NextChar();
					break;
				}
				else if (chr != '.')
				{
					if (!this.IsDecimal(chr))
					{
						throw new Exception(string.Format("Found a non-numeric value while parsing an int: {0}", chr));
					}
					float single3 = (float)((float)(chr - 48));
					if (flag)
					{
						float single4 = single3 * (float)Math.Pow(0.1, (double)single2);
						single1 += single4;
						single2 += 1f;
					}
					else
					{
						single1 *= 10f;
						single1 += single3;
					}
					this.NextChar();
				}
				else
				{
					flag = true;
					this.NextChar();
				}
			}
			return single * single1;
		}

		public void NextOpenBracket()
		{
			this.ClearWhiteSpace();
			if (this.CurrentChar() != '{')
			{
				throw new Exception("Expected open bracket.");
			}
			this.NextChar();
		}

		public string NextQuotedString()
		{
			this.ClearWhiteSpace();
			if (this.IsEOF())
			{
				return null;
			}
			char chr = this.NextChar();
			if (chr != '\"')
			{
				throw new Exception(string.Format("Expected quoted string.  Found {0} instead of quote.", chr));
			}
			StringBuilder stringBuilder = new StringBuilder();
			while (true)
			{
				char chr1 = this.CurrentChar();
				if (chr1 == '\"')
				{
					this.NextChar();
					return stringBuilder.ToString();
				}
				if (chr1 == 0)
				{
					break;
				}
				stringBuilder.Append(chr1);
				this.NextChar();
			}
			throw new Exception("Parsing ended before quoted string was completed.");
		}

		public string NextString()
		{
			this.ClearWhiteSpace();
			if (this.IsEOF())
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			while (true)
			{
				char chr = this.CurrentChar();
				if (chr == 0 || this.IsWhiteSpace(chr))
				{
					break;
				}
				stringBuilder.Append(chr);
				this.NextChar();
			}
			return stringBuilder.ToString();
		}

		public uint NextUInt32()
		{
			this.ClearWhiteSpace();
			uint num = 0;
			while (true)
			{
				char chr = this.CurrentChar();
				if (chr == 0 || this.IsWhiteSpace(chr))
				{
					break;
				}
				if (!this.IsDecimal(chr))
				{
					throw new Exception(string.Format("Found a non-numeric value while parsing an int: {0}", chr));
				}
				uint num1 = chr - 48;
				num *= 10;
				num += num1;
				this.NextChar();
			}
			return num;
		}

		private void PrevChar()
		{
			if (this.m_index > 0)
			{
				this.m_index--;
			}
		}

		public void SkipUnknownToken()
		{
			this.ClearWhiteSpace();
			if (this.IsEOF())
			{
				return;
			}
			if (this.CurrentChar() != '\"')
			{
				this.NextString();
			}
			else
			{
				this.NextQuotedString();
			}
		}
	}
}