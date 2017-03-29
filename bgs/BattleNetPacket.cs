using bnet.protocol;
using System;
using System.IO;

namespace bgs
{
	public class BattleNetPacket : PacketFormat
	{
		private Header header;

		private object body;

		private int headerSize = -1;

		private int bodySize = -1;

		public BattleNetPacket()
		{
			this.header = null;
			this.body = null;
		}

		public BattleNetPacket(Header h, IProtoBuf b)
		{
			this.header = h;
			this.body = b;
		}

		public override int Decode(byte[] bytes, int offset, int available)
		{
			int num = 0;
			if (this.headerSize < 0)
			{
				if (available < 2)
				{
					return num;
				}
				this.headerSize = (bytes[offset] << 8) + bytes[offset + 1];
				available = available - 2;
				num = num + 2;
				offset = offset + 2;
			}
			if (this.header == null)
			{
				if (available < this.headerSize)
				{
					return num;
				}
				this.header = new Header();
				this.header.Deserialize(new MemoryStream(bytes, offset, this.headerSize));
				this.bodySize = (!this.header.HasSize ? 0 : (int)this.header.Size);
				if (this.header == null)
				{
					throw new Exception("failed to parse BattleNet packet header");
				}
				available = available - this.headerSize;
				num = num + this.headerSize;
				offset = offset + this.headerSize;
			}
			if (this.body == null)
			{
				if (available < this.bodySize)
				{
					return num;
				}
				byte[] numArray = new byte[this.bodySize];
				Array.Copy(bytes, offset, numArray, 0, this.bodySize);
				this.body = numArray;
				num = num + this.bodySize;
			}
			return num;
		}

		public override byte[] Encode()
		{
			if (!(this.body is IProtoBuf))
			{
				return null;
			}
			IProtoBuf protoBuf = (IProtoBuf)this.body;
			int serializedSize = (int)this.header.GetSerializedSize();
			int num = (int)protoBuf.GetSerializedSize();
			byte[] numArray = new byte[2 + serializedSize + num];
			numArray[0] = (byte)(serializedSize >> 8 & 255);
			numArray[1] = (byte)(serializedSize & 255);
			this.header.Serialize(new MemoryStream(numArray, 2, serializedSize));
			protoBuf.Serialize(new MemoryStream(numArray, 2 + serializedSize, num));
			return numArray;
		}

		public object GetBody()
		{
			return this.body;
		}

		public Header GetHeader()
		{
			return this.header;
		}

		public override bool IsLoaded()
		{
			return (this.header == null ? false : this.body != null);
		}
	}
}