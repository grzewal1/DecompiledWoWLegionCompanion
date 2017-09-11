using System;
using System.IO;

namespace bnet.protocol.server_pool
{
	public class GetLoadRequest : IProtoBuf
	{
		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public GetLoadRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetLoadRequest.Deserialize(stream, this);
		}

		public static GetLoadRequest Deserialize(Stream stream, GetLoadRequest instance)
		{
			return GetLoadRequest.Deserialize(stream, instance, (long)-1);
		}

		public static GetLoadRequest Deserialize(Stream stream, GetLoadRequest instance, long limit)
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

		public static GetLoadRequest DeserializeLengthDelimited(Stream stream)
		{
			GetLoadRequest getLoadRequest = new GetLoadRequest();
			GetLoadRequest.DeserializeLengthDelimited(stream, getLoadRequest);
			return getLoadRequest;
		}

		public static GetLoadRequest DeserializeLengthDelimited(Stream stream, GetLoadRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GetLoadRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is GetLoadRequest))
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

		public static GetLoadRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetLoadRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetLoadRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetLoadRequest instance)
		{
		}
	}
}