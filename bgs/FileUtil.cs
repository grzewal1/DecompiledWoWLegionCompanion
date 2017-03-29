using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.IO;

namespace bgs
{
	public class FileUtil
	{
		private readonly static uint BNET_COMPRESSED_MAGIC;

		private readonly static uint BNET_COMPRESSED_VERSION;

		private readonly static byte[] BNET_COMPRESSED_MAGIC_BYTES;

		private readonly static byte[] BNET_COMPRESSED_VERSION_BYTES;

		private readonly static int BNET_COMPRESSED_HEADER_SIZE;

		static FileUtil()
		{
			FileUtil.BNET_COMPRESSED_MAGIC = 1131245658;
			FileUtil.BNET_COMPRESSED_VERSION = 0;
			FileUtil.BNET_COMPRESSED_MAGIC_BYTES = BitConverter.GetBytes(FileUtil.BNET_COMPRESSED_MAGIC);
			FileUtil.BNET_COMPRESSED_VERSION_BYTES = BitConverter.GetBytes(FileUtil.BNET_COMPRESSED_VERSION);
			FileUtil.BNET_COMPRESSED_HEADER_SIZE = (int)FileUtil.BNET_COMPRESSED_MAGIC_BYTES.Length + (int)FileUtil.BNET_COMPRESSED_VERSION_BYTES.Length + 8;
		}

		public FileUtil()
		{
		}

		private static byte[] Compress(byte[] data)
		{
			byte[] numArray;
			try
			{
				Deflater deflater = new Deflater();
				MemoryStream memoryStream = new MemoryStream((int)data.Length);
				DeflaterOutputStream deflaterOutputStream = new DeflaterOutputStream(memoryStream, deflater);
				deflaterOutputStream.Write(data, 0, (int)data.Length);
				deflaterOutputStream.Flush();
				deflaterOutputStream.Finish();
				byte[] array = memoryStream.ToArray();
				byte[] bytes = BitConverter.GetBytes((ulong)((long)((int)data.Length)));
				byte[] numArray1 = new byte[(int)array.Length + FileUtil.BNET_COMPRESSED_HEADER_SIZE];
				int length = 0;
				Array.Copy(FileUtil.BNET_COMPRESSED_MAGIC_BYTES, 0, numArray1, length, (int)FileUtil.BNET_COMPRESSED_MAGIC_BYTES.Length);
				length = length + (int)FileUtil.BNET_COMPRESSED_MAGIC_BYTES.Length;
				Array.Copy(FileUtil.BNET_COMPRESSED_VERSION_BYTES, 0, numArray1, length, (int)FileUtil.BNET_COMPRESSED_VERSION_BYTES.Length);
				length = length + (int)FileUtil.BNET_COMPRESSED_VERSION_BYTES.Length;
				Array.Copy(bytes, 0, numArray1, length, (int)bytes.Length);
				length = length + (int)bytes.Length;
				Array.Copy(array, 0, numArray1, length, (int)array.Length);
				return numArray1;
			}
			catch (Exception exception)
			{
				numArray = null;
			}
			return numArray;
		}

		private static byte[] Decompress(byte[] data)
		{
			byte[] numArray;
			MemoryStream memoryStream = new MemoryStream(data, FileUtil.BNET_COMPRESSED_HEADER_SIZE, (int)data.Length - FileUtil.BNET_COMPRESSED_HEADER_SIZE);
			int decompressedLength = (int)FileUtil.GetDecompressedLength(data);
			memoryStream.Seek((long)0, SeekOrigin.Begin);
			InflaterInputStream inflaterInputStream = new InflaterInputStream(memoryStream, new Inflater(false));
			byte[] numArray1 = new byte[decompressedLength];
			int num = 0;
			int length = (int)numArray1.Length;
			try
			{
				while (true)
				{
					int num1 = inflaterInputStream.Read(numArray1, num, length);
					if (num1 <= 0)
					{
						break;
					}
					num = num + num1;
					length = length - num1;
				}
				if (num != decompressedLength)
				{
					return null;
				}
				return numArray1;
			}
			catch (Exception exception)
			{
				numArray = null;
			}
			return numArray;
		}

		private static ulong GetDecompressedLength(byte[] data)
		{
			return BitConverter.ToUInt64(data, (int)FileUtil.BNET_COMPRESSED_MAGIC_BYTES.Length + (int)FileUtil.BNET_COMPRESSED_VERSION_BYTES.Length);
		}

		private static bool IsCompressedStream(byte[] data)
		{
			bool flag;
			try
			{
				if ((int)data.Length < FileUtil.BNET_COMPRESSED_HEADER_SIZE)
				{
					flag = false;
				}
				else if (BitConverter.ToUInt32(data, 0) != FileUtil.BNET_COMPRESSED_MAGIC)
				{
					flag = false;
				}
				else if (BitConverter.ToUInt32(data, (int)FileUtil.BNET_COMPRESSED_MAGIC_BYTES.Length) == FileUtil.BNET_COMPRESSED_VERSION)
				{
					return true;
				}
				else
				{
					flag = false;
				}
			}
			catch (Exception exception)
			{
				flag = false;
			}
			return flag;
		}

		public static bool LoadFromDrive(string filePath, out byte[] data)
		{
			bool flag;
			data = null;
			try
			{
				if (File.Exists(filePath))
				{
					FileStream fileStream = File.OpenRead(filePath);
					byte[] numArray = new byte[checked((IntPtr)fileStream.Length)];
					fileStream.Read(numArray, 0, (int)numArray.Length);
					fileStream.Close();
					if (!FileUtil.IsCompressedStream(numArray))
					{
						data = numArray;
					}
					else
					{
						byte[] numArray1 = FileUtil.Decompress(numArray);
						if (numArray1 != null)
						{
							data = numArray1;
						}
						else
						{
							flag = false;
							return flag;
						}
					}
					return true;
				}
				else
				{
					flag = false;
				}
			}
			catch (Exception exception)
			{
				flag = false;
			}
			return flag;
		}

		public static bool StoreToDrive(byte[] data, string filePath, bool overwrite, bool compress)
		{
			bool flag;
			if (data == null)
			{
				return false;
			}
			try
			{
				bool flag1 = File.Exists(filePath);
				if (!flag1 || overwrite)
				{
					byte[] numArray = data;
					if (compress)
					{
						numArray = FileUtil.Compress(numArray);
						if (numArray == null)
						{
							flag = false;
							return flag;
						}
					}
					if (flag1 && overwrite)
					{
						File.Delete(filePath);
					}
					if (compress || (int)data.Length != 0)
					{
						FileStream fileStream = File.Create(filePath, (int)numArray.Length);
						fileStream.Write(numArray, 0, (int)numArray.Length);
						fileStream.Flush();
						fileStream.Close();
						return true;
					}
					else
					{
						File.Create(filePath).Close();
						flag = true;
					}
				}
				else
				{
					flag = false;
				}
			}
			catch (Exception exception)
			{
				flag = false;
			}
			return flag;
		}
	}
}