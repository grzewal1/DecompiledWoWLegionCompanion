using bnet.protocol;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace bgs
{
	public class ContentHandle
	{
		public string Region
		{
			get;
			set;
		}

		public string Sha256Digest
		{
			get;
			set;
		}

		public string Usage
		{
			get;
			set;
		}

		public ContentHandle()
		{
		}

		public static string ByteArrayToString(byte[] ba)
		{
			StringBuilder stringBuilder = new StringBuilder((int)ba.Length * 2);
			byte[] numArray = ba;
			for (int i = 0; i < (int)numArray.Length; i++)
			{
				stringBuilder.AppendFormat("{0:x2}", numArray[i]);
			}
			return stringBuilder.ToString();
		}

		public static bgs.ContentHandle FromProtocol(bnet.protocol.ContentHandle contentHandle)
		{
			if (contentHandle == null || !contentHandle.IsInitialized)
			{
				return null;
			}
			bgs.ContentHandle contentHandle1 = new bgs.ContentHandle()
			{
				Region = (new FourCC(contentHandle.Region)).ToString(),
				Usage = (new FourCC(contentHandle.Usage)).ToString(),
				Sha256Digest = bgs.ContentHandle.ByteArrayToString(contentHandle.Hash)
			};
			return contentHandle1;
		}

		public override string ToString()
		{
			return string.Format("Region={0} Usage={1} Sha256={2}", this.Region, this.Usage, this.Sha256Digest);
		}
	}
}