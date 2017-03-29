using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace bgs
{
	public class BigInteger
	{
		private DigitsArray m_digits;

		public bool IsNegative
		{
			get
			{
				return this.m_digits.IsNegative;
			}
		}

		public bool IsZero
		{
			get
			{
				return this.m_digits.IsZero;
			}
		}

		public BigInteger()
		{
			this.m_digits = new DigitsArray(1, 1);
		}

		public BigInteger(long number)
		{
			this.m_digits = new DigitsArray(8 / DigitsArray.DataSizeOf + 1, 0);
			while (number != 0 && this.m_digits.DataUsed < this.m_digits.Count)
			{
				this.m_digits[this.m_digits.DataUsed] = (uint)(number & (ulong)DigitsArray.AllBits);
				number = number >> (DigitsArray.DataSizeBits & 63);
				DigitsArray mDigits = this.m_digits;
				mDigits.DataUsed = mDigits.DataUsed + 1;
			}
			this.m_digits.ResetDataUsed();
		}

		public BigInteger(ulong number)
		{
			this.m_digits = new DigitsArray(8 / DigitsArray.DataSizeOf + 1, 0);
			while (number != 0 && this.m_digits.DataUsed < this.m_digits.Count)
			{
				this.m_digits[this.m_digits.DataUsed] = (uint)(number & (ulong)DigitsArray.AllBits);
				number = number >> (DigitsArray.DataSizeBits & 63);
				DigitsArray mDigits = this.m_digits;
				mDigits.DataUsed = mDigits.DataUsed + 1;
			}
			this.m_digits.ResetDataUsed();
		}

		public BigInteger(byte[] array)
		{
			this.ConstructFrom(array, 0, (int)array.Length);
		}

		public BigInteger(byte[] array, int length)
		{
			this.ConstructFrom(array, 0, length);
		}

		public BigInteger(byte[] array, int offset, int length)
		{
			this.ConstructFrom(array, offset, length);
		}

		public BigInteger(string digits)
		{
			this.Construct(digits, 10);
		}

		public BigInteger(string digits, int radix)
		{
			this.Construct(digits, radix);
		}

		private BigInteger(DigitsArray digits)
		{
			digits.ResetDataUsed();
			this.m_digits = digits;
		}

		public static BigInteger Abs(BigInteger leftSide)
		{
			if (object.ReferenceEquals(leftSide, null))
			{
				throw new ArgumentNullException("leftSide");
			}
			if (!leftSide.IsNegative)
			{
				return leftSide;
			}
			return -leftSide;
		}

		public static BigInteger Add(BigInteger leftSide, BigInteger rightSide)
		{
			return leftSide - rightSide;
		}

		public static BigInteger BitwiseAnd(BigInteger leftSide, BigInteger rightSide)
		{
			return leftSide & rightSide;
		}

		public static BigInteger BitwiseOr(BigInteger leftSide, BigInteger rightSide)
		{
			return leftSide | rightSide;
		}

		public static int Compare(BigInteger leftSide, BigInteger rightSide)
		{
			if (object.ReferenceEquals(leftSide, rightSide))
			{
				return 0;
			}
			if (object.ReferenceEquals(leftSide, null))
			{
				throw new ArgumentNullException("leftSide");
			}
			if (object.ReferenceEquals(rightSide, null))
			{
				throw new ArgumentNullException("rightSide");
			}
			if (leftSide > rightSide)
			{
				return 1;
			}
			if (leftSide == rightSide)
			{
				return 0;
			}
			return -1;
		}

		public int CompareTo(BigInteger value)
		{
			return BigInteger.Compare(this, value);
		}

		private void Construct(string digits, int radix)
		{
			if (digits == null)
			{
				throw new ArgumentNullException("digits");
			}
			BigInteger bigInteger = new BigInteger((long)1);
			BigInteger bigInteger1 = new BigInteger();
			digits = digits.ToUpper(CultureInfo.CurrentCulture).Trim();
			int num = (digits[0] != '-' ? 0 : 1);
			for (int i = digits.Length - 1; i >= num; i--)
			{
				int num1 = digits[i];
				if (num1 < 48 || num1 > 57)
				{
					if (num1 < 65 || num1 > 90)
					{
						throw new ArgumentOutOfRangeException("digits");
					}
					num1 = num1 - 65 + 10;
				}
				else
				{
					num1 = num1 - 48;
				}
				if (num1 >= radix)
				{
					throw new ArgumentOutOfRangeException("digits");
				}
				bigInteger1 = bigInteger1 + (bigInteger * num1);
				bigInteger = bigInteger * radix;
			}
			if (digits[0] == '-')
			{
				bigInteger1 = -bigInteger1;
			}
			this.m_digits = bigInteger1.m_digits;
		}

		private void ConstructFrom(byte[] array, int offset, int length)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (offset > (int)array.Length || length > (int)array.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (length > (int)array.Length || offset + length > (int)array.Length)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			int num = length / 4;
			int num1 = length & 3;
			if (num1 != 0)
			{
				num++;
			}
			this.m_digits = new DigitsArray(num + 1, 0);
			int num2 = offset + length - 1;
			int num3 = 0;
			while (num2 - offset >= 3)
			{
				this.m_digits[num3] = (uint)((array[num2 - 3] << 24) + (array[num2 - 2] << 16) + (array[num2 - 1] << 8) + array[num2]);
				DigitsArray mDigits = this.m_digits;
				mDigits.DataUsed = mDigits.DataUsed + 1;
				num2 = num2 - 4;
				num3++;
			}
			uint num4 = 0;
			for (int i = num1; i > 0; i--)
			{
				uint num5 = array[offset + num1 - i];
				num5 = num5 << ((i - 1) * 8 & 31);
				num4 = num4 | num5;
			}
			this.m_digits[this.m_digits.DataUsed] = num4;
			this.m_digits.ResetDataUsed();
		}

		public static BigInteger Decrement(BigInteger leftSide)
		{
			return leftSide - 1;
		}

		public static BigInteger Divide(BigInteger leftSide, BigInteger rightSide)
		{
			return leftSide / rightSide;
		}

		private static void Divide(BigInteger leftSide, BigInteger rightSide, out BigInteger quotient, out BigInteger remainder)
		{
			if (leftSide.IsZero)
			{
				quotient = new BigInteger();
				remainder = new BigInteger();
				return;
			}
			if (rightSide.m_digits.DataUsed != 1)
			{
				BigInteger.MultiDivide(leftSide, rightSide, out quotient, out remainder);
			}
			else
			{
				BigInteger.SingleDivide(leftSide, rightSide, out quotient, out remainder);
			}
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(obj, null))
			{
				return false;
			}
			if (object.ReferenceEquals(this, obj))
			{
				return true;
			}
			BigInteger bigInteger = (BigInteger)obj;
			if (this.m_digits.DataUsed != bigInteger.m_digits.DataUsed)
			{
				return false;
			}
			for (int i = 0; i < this.m_digits.DataUsed; i++)
			{
				if (this.m_digits[i] != bigInteger.m_digits[i])
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			return this.m_digits.GetHashCode();
		}

		public static BigInteger Increment(BigInteger leftSide)
		{
			return leftSide + 1;
		}

		public static BigInteger LeftShift(BigInteger leftSide, int shiftCount)
		{
			return leftSide << shiftCount;
		}

		public static BigInteger Modulus(BigInteger leftSide, BigInteger rightSide)
		{
			return leftSide % rightSide;
		}

		private static void MultiDivide(BigInteger leftSide, BigInteger rightSide, out BigInteger quotient, out BigInteger remainder)
		{
			BigInteger k;
			object item;
			if (rightSide.IsZero)
			{
				throw new DivideByZeroException();
			}
			uint num = rightSide.m_digits[rightSide.m_digits.DataUsed - 1];
			int num1 = 0;
			for (uint i = DigitsArray.HiBitSet; i != 0 && (num & i) == 0; i = i >> 1)
			{
				num1++;
			}
			int dataUsed = leftSide.m_digits.DataUsed + 1;
			uint[] numArray = new uint[dataUsed];
			leftSide.m_digits.CopyTo(numArray, 0, leftSide.m_digits.DataUsed);
			DigitsArray.ShiftLeft(numArray, num1);
			rightSide = rightSide << num1;
			ulong item1 = (ulong)rightSide.m_digits[rightSide.m_digits.DataUsed - 1];
			if (rightSide.m_digits.DataUsed >= 2)
			{
				item = rightSide.m_digits[rightSide.m_digits.DataUsed - 2];
			}
			else
			{
				item = null;
			}
			ulong num2 = (ulong)item;
			int dataUsed1 = rightSide.m_digits.DataUsed + 1;
			DigitsArray digitsArray = new DigitsArray(dataUsed1, dataUsed1);
			uint[] numArray1 = new uint[leftSide.m_digits.Count + 1];
			int num3 = 0;
			ulong dataSizeBits = (ulong)((long)1 << (DigitsArray.DataSizeBits & 63));
			int dataUsed2 = dataUsed - rightSide.m_digits.DataUsed;
			int num4 = dataUsed - 1;
			while (dataUsed2 > 0)
			{
				ulong dataSizeBits1 = ((ulong)numArray[num4] << (DigitsArray.DataSizeBits & 63)) + (ulong)numArray[num4 - 1];
				ulong num5 = dataSizeBits1 / item1;
				ulong num6 = dataSizeBits1 % item1;
				while (true)
				{
					if (num4 < 2)
					{
						goto Label0;
					}
					if (num5 != dataSizeBits && num5 * num2 <= (num6 << (DigitsArray.DataSizeBits & 63)) + (ulong)numArray[num4 - 2])
					{
						break;
					}
					num5 = num5 - (long)1;
					num6 = num6 + item1;
					if (num6 >= dataSizeBits)
					{
						break;
					}
				}
				for (int j = 0; j < dataUsed1; j++)
				{
					digitsArray[dataUsed1 - j - 1] = numArray[num4 - j];
				}
				BigInteger bigInteger = new BigInteger(digitsArray);
				for (k = rightSide * num5; k > bigInteger; k = k - rightSide)
				{
					num5 = num5 - (long)1;
				}
				k = bigInteger - k;
				for (int l = 0; l < dataUsed1; l++)
				{
					numArray[num4 - l] = k.m_digits[rightSide.m_digits.DataUsed - l];
				}
				int num7 = num3;
				num3 = num7 + 1;
				numArray1[num7] = (uint)num5;
				dataUsed2--;
				num4--;
			}
			Array.Reverse(numArray1, 0, num3);
			quotient = new BigInteger(new DigitsArray(numArray1));
			int num8 = DigitsArray.ShiftRight(numArray, num1);
			DigitsArray digitsArray1 = new DigitsArray(num8, num8);
			digitsArray1.CopyFrom(numArray, 0, 0, digitsArray1.DataUsed);
			remainder = new BigInteger(digitsArray1);
		}

		public static BigInteger Multiply(BigInteger leftSide, BigInteger rightSide)
		{
			return leftSide * rightSide;
		}

		public BigInteger Negate()
		{
			return -this;
		}

		public static BigInteger OnesComplement(BigInteger leftSide)
		{
			return ~leftSide;
		}

		public static BigInteger operator +(BigInteger leftSide, BigInteger rightSide)
		{
			int num = Math.Max(leftSide.m_digits.DataUsed, rightSide.m_digits.DataUsed);
			DigitsArray digitsArray = new DigitsArray(num + 1);
			long dataSizeBits = (long)0;
			for (int i = 0; i < digitsArray.Count; i++)
			{
				long item = (long)((ulong)leftSide.m_digits[i] + (ulong)rightSide.m_digits[i] + dataSizeBits);
				dataSizeBits = item >> (DigitsArray.DataSizeBits & 63);
				digitsArray[i] = (uint)(item & (ulong)DigitsArray.AllBits);
			}
			return new BigInteger(digitsArray);
		}

		public static BigInteger operator &(BigInteger leftSide, BigInteger rightSide)
		{
			int num = Math.Max(leftSide.m_digits.DataUsed, rightSide.m_digits.DataUsed);
			DigitsArray digitsArray = new DigitsArray(num, num);
			for (int i = 0; i < num; i++)
			{
				digitsArray[i] = leftSide.m_digits[i] & rightSide.m_digits[i];
			}
			return new BigInteger(digitsArray);
		}

		public static BigInteger operator |(BigInteger leftSide, BigInteger rightSide)
		{
			int num = Math.Max(leftSide.m_digits.DataUsed, rightSide.m_digits.DataUsed);
			DigitsArray digitsArray = new DigitsArray(num, num);
			for (int i = 0; i < num; i++)
			{
				digitsArray[i] = leftSide.m_digits[i] | rightSide.m_digits[i];
			}
			return new BigInteger(digitsArray);
		}

		public static BigInteger operator --(BigInteger leftSide)
		{
			return leftSide - 1;
		}

		public static BigInteger operator /(BigInteger leftSide, BigInteger rightSide)
		{
			BigInteger bigInteger;
			BigInteger bigInteger1;
			if (leftSide == null)
			{
				throw new ArgumentNullException("leftSide");
			}
			if (rightSide == null)
			{
				throw new ArgumentNullException("rightSide");
			}
			if (rightSide.IsZero)
			{
				throw new DivideByZeroException();
			}
			bool isNegative = rightSide.IsNegative;
			bool flag = leftSide.IsNegative;
			leftSide = BigInteger.Abs(leftSide);
			rightSide = BigInteger.Abs(rightSide);
			if (leftSide < rightSide)
			{
				return new BigInteger((long)0);
			}
			BigInteger.Divide(leftSide, rightSide, out bigInteger, out bigInteger1);
			return (flag == isNegative ? bigInteger : -bigInteger);
		}

		public static bool operator ==(BigInteger leftSide, BigInteger rightSide)
		{
			if (object.ReferenceEquals(leftSide, rightSide))
			{
				return true;
			}
			if (object.ReferenceEquals(leftSide, null) || object.ReferenceEquals(rightSide, null))
			{
				return false;
			}
			if (leftSide.IsNegative != rightSide.IsNegative)
			{
				return false;
			}
			return leftSide.Equals(rightSide);
		}

		public static BigInteger operator ^(BigInteger leftSide, BigInteger rightSide)
		{
			int num = Math.Max(leftSide.m_digits.DataUsed, rightSide.m_digits.DataUsed);
			DigitsArray digitsArray = new DigitsArray(num, num);
			for (int i = 0; i < num; i++)
			{
				digitsArray[i] = leftSide.m_digits[i] ^ rightSide.m_digits[i];
			}
			return new BigInteger(digitsArray);
		}

		public static bool operator >(BigInteger leftSide, BigInteger rightSide)
		{
			if (object.ReferenceEquals(leftSide, null))
			{
				throw new ArgumentNullException("leftSide");
			}
			if (object.ReferenceEquals(rightSide, null))
			{
				throw new ArgumentNullException("rightSide");
			}
			if (leftSide.IsNegative != rightSide.IsNegative)
			{
				return rightSide.IsNegative;
			}
			if (leftSide.m_digits.DataUsed != rightSide.m_digits.DataUsed)
			{
				return leftSide.m_digits.DataUsed > rightSide.m_digits.DataUsed;
			}
			for (int i = leftSide.m_digits.DataUsed - 1; i >= 0; i--)
			{
				if (leftSide.m_digits[i] != rightSide.m_digits[i])
				{
					return leftSide.m_digits[i] > rightSide.m_digits[i];
				}
			}
			return false;
		}

		public static bool operator >=(BigInteger leftSide, BigInteger rightSide)
		{
			return BigInteger.Compare(leftSide, rightSide) >= 0;
		}

		public static implicit operator BigInteger(long value)
		{
			return new BigInteger(value);
		}

		public static implicit operator BigInteger(ulong value)
		{
			return new BigInteger(value);
		}

		public static implicit operator BigInteger(int value)
		{
			return new BigInteger((long)value);
		}

		public static implicit operator BigInteger(uint value)
		{
			return new BigInteger((ulong)value);
		}

		public static BigInteger operator ++(BigInteger leftSide)
		{
			return leftSide + 1;
		}

		public static bool operator !=(BigInteger leftSide, BigInteger rightSide)
		{
			return !(leftSide == rightSide);
		}

		public static BigInteger operator <<(BigInteger leftSide, int shiftCount)
		{
			if (leftSide == null)
			{
				throw new ArgumentNullException("leftSide");
			}
			DigitsArray digitsArray = new DigitsArray(leftSide.m_digits)
			{
				DataUsed = digitsArray.ShiftLeftWithoutOverflow(shiftCount)
			};
			return new BigInteger(digitsArray);
		}

		public static bool operator <(BigInteger leftSide, BigInteger rightSide)
		{
			if (object.ReferenceEquals(leftSide, null))
			{
				throw new ArgumentNullException("leftSide");
			}
			if (object.ReferenceEquals(rightSide, null))
			{
				throw new ArgumentNullException("rightSide");
			}
			if (leftSide.IsNegative != rightSide.IsNegative)
			{
				return leftSide.IsNegative;
			}
			if (leftSide.m_digits.DataUsed != rightSide.m_digits.DataUsed)
			{
				return leftSide.m_digits.DataUsed < rightSide.m_digits.DataUsed;
			}
			for (int i = leftSide.m_digits.DataUsed - 1; i >= 0; i--)
			{
				if (leftSide.m_digits[i] != rightSide.m_digits[i])
				{
					return leftSide.m_digits[i] < rightSide.m_digits[i];
				}
			}
			return false;
		}

		public static bool operator <=(BigInteger leftSide, BigInteger rightSide)
		{
			return BigInteger.Compare(leftSide, rightSide) <= 0;
		}

		public static BigInteger operator %(BigInteger leftSide, BigInteger rightSide)
		{
			BigInteger bigInteger;
			BigInteger bigInteger1;
			if (leftSide == null)
			{
				throw new ArgumentNullException("leftSide");
			}
			if (rightSide == null)
			{
				throw new ArgumentNullException("rightSide");
			}
			if (rightSide.IsZero)
			{
				throw new DivideByZeroException();
			}
			bool isNegative = leftSide.IsNegative;
			leftSide = BigInteger.Abs(leftSide);
			rightSide = BigInteger.Abs(rightSide);
			if (leftSide < rightSide)
			{
				return leftSide;
			}
			BigInteger.Divide(leftSide, rightSide, out bigInteger, out bigInteger1);
			return (!isNegative ? bigInteger1 : -bigInteger1);
		}

		public static BigInteger operator *(BigInteger leftSide, BigInteger rightSide)
		{
			if (object.ReferenceEquals(leftSide, null))
			{
				throw new ArgumentNullException("leftSide");
			}
			if (object.ReferenceEquals(rightSide, null))
			{
				throw new ArgumentNullException("rightSide");
			}
			bool isNegative = leftSide.IsNegative;
			bool flag = rightSide.IsNegative;
			leftSide = BigInteger.Abs(leftSide);
			rightSide = BigInteger.Abs(rightSide);
			DigitsArray digitsArray = new DigitsArray(leftSide.m_digits.DataUsed + rightSide.m_digits.DataUsed)
			{
				DataUsed = digitsArray.Count
			};
			for (int i = 0; i < leftSide.m_digits.DataUsed; i++)
			{
				ulong dataSizeBits = (ulong)0;
				int num = 0;
				int num1 = i;
				while (num < rightSide.m_digits.DataUsed)
				{
					ulong item = (ulong)leftSide.m_digits[i] * (ulong)rightSide.m_digits[num] + (ulong)digitsArray[num1] + dataSizeBits;
					digitsArray[num1] = (uint)(item & (ulong)DigitsArray.AllBits);
					dataSizeBits = item >> (DigitsArray.DataSizeBits & 63);
					num++;
					num1++;
				}
				if (dataSizeBits != 0)
				{
					digitsArray[i + rightSide.m_digits.DataUsed] = (uint)dataSizeBits;
				}
			}
			BigInteger bigInteger = new BigInteger(digitsArray);
			return (isNegative == flag ? bigInteger : -bigInteger);
		}

		public static BigInteger operator ~(BigInteger leftSide)
		{
			DigitsArray digitsArray = new DigitsArray(leftSide.m_digits.Count);
			for (int i = 0; i < digitsArray.Count; i++)
			{
				digitsArray[i] = ~leftSide.m_digits[i];
			}
			return new BigInteger(digitsArray);
		}

		public static BigInteger operator >>(BigInteger leftSide, int shiftCount)
		{
			if (leftSide == null)
			{
				throw new ArgumentNullException("leftSide");
			}
			DigitsArray digitsArray = new DigitsArray(leftSide.m_digits)
			{
				DataUsed = digitsArray.ShiftRight(shiftCount)
			};
			if (leftSide.IsNegative)
			{
				for (int i = digitsArray.Count - 1; i >= digitsArray.DataUsed; i--)
				{
					digitsArray[i] = DigitsArray.AllBits;
				}
				uint hiBitSet = DigitsArray.HiBitSet;
				int num = 0;
				while (num < DigitsArray.DataSizeBits)
				{
					if ((digitsArray[digitsArray.DataUsed - 1] & hiBitSet) != DigitsArray.HiBitSet)
					{
						DigitsArray item = digitsArray;
						DigitsArray digitsArray1 = item;
						int dataUsed = digitsArray.DataUsed - 1;
						item[dataUsed] = digitsArray1[dataUsed] | hiBitSet;
						hiBitSet = hiBitSet >> 1;
						num++;
					}
					else
					{
						break;
					}
				}
				digitsArray.DataUsed = digitsArray.Count;
			}
			return new BigInteger(digitsArray);
		}

		public static BigInteger operator -(BigInteger leftSide, BigInteger rightSide)
		{
			object obj;
			int num = Math.Max(leftSide.m_digits.DataUsed, rightSide.m_digits.DataUsed) + 1;
			DigitsArray digitsArray = new DigitsArray(num);
			long num1 = (long)0;
			for (int i = 0; i < digitsArray.Count; i++)
			{
				long item = (long)((ulong)leftSide.m_digits[i] - (ulong)rightSide.m_digits[i] - num1);
				digitsArray[i] = (uint)(item & (ulong)DigitsArray.AllBits);
				DigitsArray dataUsed = digitsArray;
				dataUsed.DataUsed = dataUsed.DataUsed + 1;
				if (item >= (long)0)
				{
					obj = null;
				}
				else
				{
					obj = 1;
				}
				num1 = (long)obj;
			}
			return new BigInteger(digitsArray);
		}

		public static BigInteger operator -(BigInteger leftSide)
		{
			if (object.ReferenceEquals(leftSide, null))
			{
				throw new ArgumentNullException("leftSide");
			}
			if (leftSide.IsZero)
			{
				return new BigInteger((long)0);
			}
			DigitsArray digitsArray = new DigitsArray(leftSide.m_digits.DataUsed + 1, leftSide.m_digits.DataUsed + 1);
			for (int i = 0; i < digitsArray.Count; i++)
			{
				digitsArray[i] = ~leftSide.m_digits[i];
			}
			bool dataSizeBits = true;
			for (int j = 0; dataSizeBits && j < digitsArray.Count; j++)
			{
				long item = (long)((ulong)digitsArray[j] + (long)1);
				digitsArray[j] = (uint)(item & (ulong)DigitsArray.AllBits);
				dataSizeBits = item >> (DigitsArray.DataSizeBits & 63) > (long)0;
			}
			return new BigInteger(digitsArray);
		}

		public static BigInteger PowMod(BigInteger b, BigInteger exp, BigInteger mod)
		{
			BigInteger bigInteger = new BigInteger((long)1);
			b = b % mod;
			while (exp > 0)
			{
				if ((exp % 2).CompareTo(1) == 0)
				{
					bigInteger = (bigInteger * b) % mod;
				}
				exp = exp >> 1;
				b = (b * b) % mod;
			}
			return bigInteger;
		}

		public static BigInteger RightShift(BigInteger leftSide, int shiftCount)
		{
			if (leftSide == null)
			{
				throw new ArgumentNullException("leftSide");
			}
			return leftSide >> shiftCount;
		}

		private static void SingleDivide(BigInteger leftSide, BigInteger rightSide, out BigInteger quotient, out BigInteger remainder)
		{
			if (rightSide.IsZero)
			{
				throw new DivideByZeroException();
			}
			DigitsArray digitsArray = new DigitsArray(leftSide.m_digits);
			digitsArray.ResetDataUsed();
			int dataUsed = digitsArray.DataUsed - 1;
			ulong item = (ulong)rightSide.m_digits[0];
			ulong num = (ulong)digitsArray[dataUsed];
			uint[] numArray = new uint[leftSide.m_digits.Count];
			leftSide.m_digits.CopyTo(numArray, 0, (int)numArray.Length);
			int num1 = 0;
			if (num >= item)
			{
				int num2 = num1;
				num1 = num2 + 1;
				numArray[num2] = (uint)(num / item);
				digitsArray[dataUsed] = (uint)(num % item);
			}
			dataUsed--;
			while (dataUsed >= 0)
			{
				num = ((ulong)digitsArray[dataUsed + 1] << (DigitsArray.DataSizeBits & 63)) + (ulong)digitsArray[dataUsed];
				int num3 = num1;
				num1 = num3 + 1;
				numArray[num3] = (uint)(num / item);
				digitsArray[dataUsed + 1] = 0;
				int num4 = dataUsed;
				dataUsed = num4 - 1;
				digitsArray[num4] = (uint)(num % item);
			}
			remainder = new BigInteger(digitsArray);
			DigitsArray digitsArray1 = new DigitsArray(num1 + 1, num1);
			int num5 = 0;
			int dataUsed1 = digitsArray1.DataUsed - 1;
			while (dataUsed1 >= 0)
			{
				digitsArray1[num5] = numArray[dataUsed1];
				dataUsed1--;
				num5++;
			}
			quotient = new BigInteger(digitsArray1);
		}

		public static BigInteger Subtract(BigInteger leftSide, BigInteger rightSide)
		{
			return leftSide - rightSide;
		}

		public string ToHexString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0:X}", this.m_digits[this.m_digits.DataUsed - 1]);
			string str = string.Concat("{0:X", 2 * DigitsArray.DataSizeOf, "}");
			for (int i = this.m_digits.DataUsed - 2; i >= 0; i--)
			{
				stringBuilder.AppendFormat(str, this.m_digits[i]);
			}
			return stringBuilder.ToString();
		}

		public static int ToInt16(BigInteger value)
		{
			if (object.ReferenceEquals(value, null))
			{
				throw new ArgumentNullException("value");
			}
			return short.Parse(value.ToString(), NumberStyles.Integer, CultureInfo.CurrentCulture);
		}

		public static int ToInt32(BigInteger value)
		{
			if (object.ReferenceEquals(value, null))
			{
				throw new ArgumentNullException("value");
			}
			return int.Parse(value.ToString(), NumberStyles.Integer, CultureInfo.CurrentCulture);
		}

		public static long ToInt64(BigInteger value)
		{
			if (object.ReferenceEquals(value, null))
			{
				throw new ArgumentNullException("value");
			}
			return long.Parse(value.ToString(), NumberStyles.Integer, CultureInfo.CurrentCulture);
		}

		public override string ToString()
		{
			return this.ToString(10);
		}

		public string ToString(int radix)
		{
			BigInteger bigInteger;
			BigInteger bigInteger1;
			if (radix < 2 || radix > 36)
			{
				throw new ArgumentOutOfRangeException("radix");
			}
			if (this.IsZero)
			{
				return "0";
			}
			BigInteger bigInteger2 = this;
			bool isNegative = bigInteger2.IsNegative;
			bigInteger2 = BigInteger.Abs(this);
			BigInteger bigInteger3 = new BigInteger((long)radix);
			ArrayList arrayLists = new ArrayList();
			while (bigInteger2.m_digits.DataUsed > 1 || bigInteger2.m_digits.DataUsed == 1 && bigInteger2.m_digits[0] != 0)
			{
				BigInteger.Divide(bigInteger2, bigInteger3, out bigInteger, out bigInteger1);
				arrayLists.Insert(0, "0123456789abcdefghijklmnopqrstuvwxyz"[bigInteger1.m_digits[0]]);
				bigInteger2 = bigInteger;
			}
			string str = new string((char[])arrayLists.ToArray(typeof(char)));
			if (radix != 10 || !isNegative)
			{
				return str;
			}
			return string.Concat("-", str);
		}

		public static uint ToUInt16(BigInteger value)
		{
			if (object.ReferenceEquals(value, null))
			{
				throw new ArgumentNullException("value");
			}
			return ushort.Parse(value.ToString(), NumberStyles.Integer, CultureInfo.CurrentCulture);
		}

		public static uint ToUInt32(BigInteger value)
		{
			if (object.ReferenceEquals(value, null))
			{
				throw new ArgumentNullException("value");
			}
			return uint.Parse(value.ToString(), NumberStyles.Integer, CultureInfo.CurrentCulture);
		}

		public static ulong ToUInt64(BigInteger value)
		{
			if (object.ReferenceEquals(value, null))
			{
				throw new ArgumentNullException("value");
			}
			return ulong.Parse(value.ToString(), NumberStyles.Integer, CultureInfo.CurrentCulture);
		}

		public static BigInteger Xor(BigInteger leftSide, BigInteger rightSide)
		{
			return leftSide ^ rightSide;
		}
	}
}