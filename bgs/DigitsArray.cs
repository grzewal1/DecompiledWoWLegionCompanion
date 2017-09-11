using System;
using System.Collections.Generic;
using System.Reflection;

namespace bgs
{
	internal class DigitsArray
	{
		private uint[] m_data;

		internal readonly static uint AllBits;

		internal readonly static uint HiBitSet;

		private int m_dataUsed;

		internal int Count
		{
			get
			{
				return (int)this.m_data.Length;
			}
		}

		internal static int DataSizeBits
		{
			get
			{
				return 32;
			}
		}

		internal static int DataSizeOf
		{
			get
			{
				return 4;
			}
		}

		internal int DataUsed
		{
			get
			{
				return this.m_dataUsed;
			}
			set
			{
				this.m_dataUsed = value;
			}
		}

		internal bool IsNegative
		{
			get
			{
				return (this.m_data[(int)this.m_data.Length - 1] & DigitsArray.HiBitSet) == DigitsArray.HiBitSet;
			}
		}

		internal bool IsZero
		{
			get
			{
				bool flag;
				if (this.m_dataUsed == 0)
				{
					flag = true;
				}
				else
				{
					flag = (this.m_dataUsed != 1 ? false : this.m_data[0] == 0);
				}
				return flag;
			}
		}

		internal uint this[int index]
		{
			get
			{
				uint allBits;
				if (index < this.m_dataUsed)
				{
					return this.m_data[index];
				}
				if (!this.IsNegative)
				{
					allBits = 0;
				}
				else
				{
					allBits = DigitsArray.AllBits;
				}
				return allBits;
			}
			set
			{
				this.m_data[index] = value;
			}
		}

		static DigitsArray()
		{
			DigitsArray.AllBits = -1;
			DigitsArray.HiBitSet = (uint)(1 << (DigitsArray.DataSizeBits - 1 & 31));
		}

		internal DigitsArray(int size)
		{
			this.Allocate(size, 0);
		}

		internal DigitsArray(int size, int used)
		{
			this.Allocate(size, used);
		}

		internal DigitsArray(uint[] copyFrom)
		{
			this.Allocate((int)copyFrom.Length);
			this.CopyFrom(copyFrom, 0, 0, (int)copyFrom.Length);
			this.ResetDataUsed();
		}

		internal DigitsArray(DigitsArray copyFrom)
		{
			this.Allocate(copyFrom.Count, copyFrom.DataUsed);
			Array.Copy(copyFrom.m_data, 0, this.m_data, 0, copyFrom.Count);
		}

		public void Allocate(int size)
		{
			this.Allocate(size, 0);
		}

		public void Allocate(int size, int used)
		{
			this.m_data = new uint[size + 1];
			this.m_dataUsed = used;
		}

		internal void CopyFrom(uint[] source, int sourceOffset, int offset, int length)
		{
			Array.Copy(source, sourceOffset, this.m_data, 0, length);
		}

		internal void CopyTo(uint[] array, int offset, int length)
		{
			Array.Copy(this.m_data, 0, array, offset, length);
		}

		internal void ResetDataUsed()
		{
			this.m_dataUsed = (int)this.m_data.Length;
			if (!this.IsNegative)
			{
				while (this.m_dataUsed > 1 && this.m_data[this.m_dataUsed - 1] == 0)
				{
					this.m_dataUsed--;
				}
				if (this.m_dataUsed == 0)
				{
					this.m_dataUsed = 1;
				}
			}
			else
			{
				while (this.m_dataUsed > 1 && this.m_data[this.m_dataUsed - 1] == DigitsArray.AllBits)
				{
					this.m_dataUsed--;
				}
				this.m_dataUsed++;
			}
		}

		internal int ShiftLeft(int shiftCount)
		{
			return DigitsArray.ShiftLeft(this.m_data, shiftCount);
		}

		internal static int ShiftLeft(uint[] buffer, int shiftCount)
		{
			int dataSizeBits = DigitsArray.DataSizeBits;
			int length = (int)buffer.Length;
			while (length > 1 && buffer[length - 1] == 0)
			{
				length--;
			}
			for (int i = shiftCount; i > 0; i -= dataSizeBits)
			{
				if (i < dataSizeBits)
				{
					dataSizeBits = i;
				}
				ulong num = (ulong)0;
				for (int j = 0; j < length; j++)
				{
					ulong num1 = (ulong)buffer[j] << (dataSizeBits & 63);
					num1 |= num;
					buffer[j] = (uint)(num1 & (ulong)DigitsArray.AllBits);
					num = num1 >> (DigitsArray.DataSizeBits & 63);
				}
				if (num != 0)
				{
					if (length + 1 > (int)buffer.Length)
					{
						throw new OverflowException();
					}
					buffer[length] = (uint)num;
					length++;
					num = (ulong)0;
				}
			}
			return length;
		}

		internal int ShiftLeftWithoutOverflow(int shiftCount)
		{
			List<uint> nums = new List<uint>(this.m_data);
			int dataSizeBits = DigitsArray.DataSizeBits;
			for (int i = shiftCount; i > 0; i -= dataSizeBits)
			{
				if (i < dataSizeBits)
				{
					dataSizeBits = i;
				}
				ulong num = (ulong)0;
				for (int j = 0; j < nums.Count; j++)
				{
					ulong item = (ulong)nums[j] << (dataSizeBits & 63);
					item |= num;
					nums[j] = (uint)(item & (ulong)DigitsArray.AllBits);
					num = item >> (DigitsArray.DataSizeBits & 63);
				}
				if (num != 0)
				{
					nums.Add(0);
					nums[nums.Count - 1] = (uint)num;
				}
			}
			this.m_data = new uint[nums.Count];
			nums.CopyTo(this.m_data);
			return (int)this.m_data.Length;
		}

		internal int ShiftRight(int shiftCount)
		{
			return DigitsArray.ShiftRight(this.m_data, shiftCount);
		}

		internal static int ShiftRight(uint[] buffer, int shiftCount)
		{
			int dataSizeBits = DigitsArray.DataSizeBits;
			int num = 0;
			int length = (int)buffer.Length;
			while (length > 1 && buffer[length - 1] == 0)
			{
				length--;
			}
			for (int i = shiftCount; i > 0; i -= dataSizeBits)
			{
				if (i < dataSizeBits)
				{
					dataSizeBits = i;
					num = DigitsArray.DataSizeBits - dataSizeBits;
				}
				ulong num1 = (ulong)0;
				for (int j = length - 1; j >= 0; j--)
				{
					ulong num2 = (ulong)buffer[j] >> (dataSizeBits & 63);
					num2 |= num1;
					num1 = (ulong)buffer[j] << (num & 63);
					buffer[j] = (uint)num2;
				}
			}
			while (length > 1 && buffer[length - 1] == 0)
			{
				length--;
			}
			return length;
		}
	}
}