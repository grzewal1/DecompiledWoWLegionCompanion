using System;
using System.Runtime.InteropServices;
using System.Text;

namespace bgs
{
	public static class MemUtils
	{
		public static void FreePtr(IntPtr ptr)
		{
			if (ptr == IntPtr.Zero)
			{
				return;
			}
			Marshal.FreeHGlobal(ptr);
		}

		public static IntPtr PtrFromBytes(byte[] bytes)
		{
			return MemUtils.PtrFromBytes(bytes, 0);
		}

		public static IntPtr PtrFromBytes(byte[] bytes, int offset)
		{
			if (bytes == null)
			{
				return IntPtr.Zero;
			}
			int length = (int)bytes.Length - offset;
			return MemUtils.PtrFromBytes(bytes, offset, length);
		}

		public static IntPtr PtrFromBytes(byte[] bytes, int offset, int len)
		{
			if (bytes == null)
			{
				return IntPtr.Zero;
			}
			if (len <= 0)
			{
				return IntPtr.Zero;
			}
			IntPtr intPtr = Marshal.AllocHGlobal(len);
			Marshal.Copy(bytes, offset, intPtr, len);
			return intPtr;
		}

		public static byte[] PtrToBytes(IntPtr ptr, int size)
		{
			if (ptr == IntPtr.Zero)
			{
				return null;
			}
			if (size == 0)
			{
				return null;
			}
			byte[] numArray = new byte[size];
			Marshal.Copy(ptr, numArray, 0, size);
			return numArray;
		}

		public static string StringFromUtf8Ptr(IntPtr ptr)
		{
			int num;
			return MemUtils.StringFromUtf8Ptr(ptr, out num);
		}

		public static string StringFromUtf8Ptr(IntPtr ptr, out int len)
		{
			len = 0;
			if (ptr == IntPtr.Zero)
			{
				return null;
			}
			len = MemUtils.StringPtrByteLen(ptr);
			if (len == 0)
			{
				return null;
			}
			byte[] numArray = new byte[len];
			Marshal.Copy(ptr, numArray, 0, len);
			return Encoding.UTF8.GetString(numArray);
		}

		public static int StringPtrByteLen(IntPtr ptr)
		{
			if (ptr == IntPtr.Zero)
			{
				return 0;
			}
			int num = 0;
			while (Marshal.ReadByte(ptr, num) != 0)
			{
				num++;
			}
			return num;
		}

		public static T StructFromBytes<T>(byte[] bytes)
		{
			return MemUtils.StructFromBytes<T>(bytes, 0);
		}

		public static T StructFromBytes<T>(byte[] bytes, int offset)
		{
			T t;
			Type type = typeof(T);
			int num = Marshal.SizeOf(type);
			if (bytes == null)
			{
				t = default(T);
				return t;
			}
			if ((int)bytes.Length - offset < num)
			{
				t = default(T);
				return t;
			}
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			Marshal.Copy(bytes, offset, intPtr, num);
			T structure = (T)Marshal.PtrToStructure(intPtr, type);
			Marshal.FreeHGlobal(intPtr);
			return structure;
		}

		public static byte[] StructToBytes<T>(T t)
		{
			int num = Marshal.SizeOf(typeof(T));
			byte[] numArray = new byte[num];
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			Marshal.StructureToPtr(t, intPtr, true);
			Marshal.Copy(intPtr, numArray, 0, num);
			Marshal.FreeHGlobal(intPtr);
			return numArray;
		}

		public static IntPtr Utf8PtrFromString(string managedString)
		{
			if (managedString == null)
			{
				return IntPtr.Zero;
			}
			int byteCount = 1 + Encoding.UTF8.GetByteCount(managedString);
			byte[] numArray = new byte[byteCount];
			Encoding.UTF8.GetBytes(managedString, 0, managedString.Length, numArray, 0);
			IntPtr intPtr = Marshal.AllocHGlobal(byteCount);
			Marshal.Copy(numArray, 0, intPtr, byteCount);
			return intPtr;
		}
	}
}