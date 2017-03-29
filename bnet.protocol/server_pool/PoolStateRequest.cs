using System;
using System.IO;

namespace bnet.protocol.server_pool
{
	public class PoolStateRequest : IProtoBuf
	{
		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public PoolStateRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			PoolStateRequest.Deserialize(stream, this);
		}

		public static PoolStateRequest Deserialize(Stream stream, PoolStateRequest instance)
		{
			return PoolStateRequest.Deserialize(stream, instance, (long)-1);
		}

		public static PoolStateRequest Deserialize(Stream stream, PoolStateRequest instance, long limit)
		{
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else
					{
						if (limit >= (long)0)
						{
							throw new EndOfStreamException();
						}
						break;
					}
				}
				else
				{
					if (stream.Position != limit)
					{
						throw new ProtocolBufferException("Read past max limit");
					}
					break;
				}
			}
			return instance;
		}

		public static PoolStateRequest DeserializeLengthDelimited(Stream stream)
		{
			PoolStateRequest poolStateRequest = new PoolStateRequest();
			PoolStateRequest.DeserializeLengthDelimited(stream, poolStateRequest);
			return poolStateRequest;
		}

		public static PoolStateRequest DeserializeLengthDelimited(Stream stream, PoolStateRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return PoolStateRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is PoolStateRequest))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return this.GetType().GetHashCode();
		}

		public uint GetSerializedSize()
		{
			return (uint)0;
		}

		public static PoolStateRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<PoolStateRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			PoolStateRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, PoolStateRequest instance)
		{
		}
	}
}