using System;

namespace Newtonsoft.Json.Utilities
{
	internal class StringBuffer
	{
		private char[] _buffer;

		private int _position;

		private readonly static char[] _emptyBuffer;

		public int Position
		{
			get
			{
				return this._position;
			}
			set
			{
				this._position = value;
			}
		}

		static StringBuffer()
		{
			StringBuffer._emptyBuffer = new char[0];
		}

		public StringBuffer()
		{
			this._buffer = StringBuffer._emptyBuffer;
		}

		public StringBuffer(int initalSize)
		{
			this._buffer = new char[initalSize];
		}

		public void Append(char value)
		{
			if (this._position == (int)this._buffer.Length)
			{
				this.EnsureSize(1);
			}
			char[] chrArray = this._buffer;
			StringBuffer stringBuffer = this;
			int num = stringBuffer._position;
			int num1 = num;
			stringBuffer._position = num + 1;
			chrArray[num1] = value;
		}

		public void Clear()
		{
			this._buffer = StringBuffer._emptyBuffer;
			this._position = 0;
		}

		private void EnsureSize(int appendLength)
		{
			char[] chrArray = new char[(this._position + appendLength) * 2];
			Array.Copy(this._buffer, chrArray, this._position);
			this._buffer = chrArray;
		}

		public char[] GetInternalBuffer()
		{
			return this._buffer;
		}

		public override string ToString()
		{
			return this.ToString(0, this._position);
		}

		public string ToString(int start, int length)
		{
			return new string(this._buffer, start, length);
		}
	}
}