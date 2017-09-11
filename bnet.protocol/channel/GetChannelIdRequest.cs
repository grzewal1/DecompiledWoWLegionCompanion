using System;
using System.IO;

namespace bnet.protocol.channel
{
	public class GetChannelIdRequest : IProtoBuf
	{
		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public GetChannelIdRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetChannelIdRequest.Deserialize(stream, this);
		}

		public static GetChannelIdRequest Deserialize(Stream stream, GetChannelIdRequest instance)
		{
			return GetChannelIdRequest.Deserialize(stream, instance, (long)-1);
		}

		public static GetChannelIdRequest Deserialize(Stream stream, GetChannelIdRequest instance, long limit)
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

		public static GetChannelIdRequest DeserializeLengthDelimited(Stream stream)
		{
			GetChannelIdRequest getChannelIdRequest = new GetChannelIdRequest();
			GetChannelIdRequest.DeserializeLengthDelimited(stream, getChannelIdRequest);
			return getChannelIdRequest;
		}

		public static GetChannelIdRequest DeserializeLengthDelimited(Stream stream, GetChannelIdRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GetChannelIdRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is GetChannelIdRequest))
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

		public static GetChannelIdRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetChannelIdRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetChannelIdRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetChannelIdRequest instance)
		{
		}
	}
}