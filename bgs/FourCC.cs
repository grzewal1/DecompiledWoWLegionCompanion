using System;
using System.Text;

namespace bgs
{
	[Serializable]
	public class FourCC
	{
		protected uint m_value;

		public FourCC()
		{
		}

		public FourCC(uint value)
		{
			this.m_value = value;
		}

		public FourCC(string stringVal)
		{
			this.SetString(stringVal);
		}

		public FourCC Clone()
		{
			FourCC fourCC = new FourCC();
			fourCC.CopyFrom(this);
			return fourCC;
		}

		public void CopyFrom(FourCC other)
		{
			this.m_value = other.m_value;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			FourCC fourCC = obj as FourCC;
			if (fourCC == null)
			{
				return false;
			}
			return this.m_value == fourCC.m_value;
		}

		public bool Equals(FourCC other)
		{
			if (other == null)
			{
				return false;
			}
			return this.m_value == other.m_value;
		}

		public override int GetHashCode()
		{
			return this.m_value.GetHashCode();
		}

		public string GetString()
		{
			StringBuilder stringBuilder = new StringBuilder(4);
			for (int i = 24; i >= 0; i = i - 8)
			{
				char mValue = (char)(this.m_value >> (i & 31) & 255);
				if (mValue != 0)
				{
					stringBuilder.Append(mValue);
				}
			}
			return stringBuilder.ToString();
		}

		public uint GetValue()
		{
			return this.m_value;
		}

		public static bool operator ==(uint val, FourCC fourCC)
		{
			if (fourCC == null)
			{
				return false;
			}
			return val == fourCC.m_value;
		}

		public static bool operator ==(FourCC fourCC, uint val)
		{
			if (fourCC == null)
			{
				return false;
			}
			return fourCC.m_value == val;
		}

		public static bool operator ==(FourCC a, FourCC b)
		{
			if (object.ReferenceEquals(a, b))
			{
				return true;
			}
			if (a == null || b == null)
			{
				return false;
			}
			return a.m_value == b.m_value;
		}

		public static implicit operator FourCC(uint val)
		{
			return new FourCC(val);
		}

		public static bool operator !=(uint val, FourCC fourCC)
		{
			return !(val == fourCC);
		}

		public static bool operator !=(FourCC fourCC, uint val)
		{
			return !(fourCC == val);
		}

		public static bool operator !=(FourCC a, FourCC b)
		{
			return !(a == b);
		}

		public void SetString(string str)
		{
			this.m_value = 0;
			for (int i = 0; i < str.Length && i < 4; i++)
			{
				this.m_value = this.m_value << 8 | (byte)str[i];
			}
		}

		public void SetValue(uint val)
		{
			this.m_value = val;
		}

		public override string ToString()
		{
			return this.GetString();
		}
	}
}