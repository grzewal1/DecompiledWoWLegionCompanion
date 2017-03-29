using System;
using System.Text;

namespace bgs
{
	public class Compute32
	{
		public const uint FNV_32_PRIME = 16777619;

		public const uint COMPUTE_32_OFFSET = 2166136261;

		public Compute32()
		{
		}

		public static uint Hash(string str)
		{
			uint num = -2128831035;
			byte[] bytes = Encoding.ASCII.GetBytes(str);
			for (int i = 0; i < (int)bytes.Length; i++)
			{
				num = num ^ bytes[i];
				num = num * 16777619;
			}
			return num;
		}
	}
}