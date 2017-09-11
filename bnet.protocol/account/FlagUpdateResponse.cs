using System;
using System.IO;

namespace bnet.protocol.account
{
	public class FlagUpdateResponse : IProtoBuf
	{
		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public FlagUpdateResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			FlagUpdateResponse.Deserialize(stream, this);
		}

		public static FlagUpdateResponse Deserialize(Stream stream, FlagUpdateResponse instance)
		{
			return FlagUpdateResponse.Deserialize(stream, instance, (long)-1);
		}

		public static FlagUpdateResponse Deserialize(Stream stream, FlagUpdateResponse instance, long limit)
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

		public static FlagUpdateResponse DeserializeLengthDelimited(Stream stream)
		{
			FlagUpdateResponse flagUpdateResponse = new FlagUpdateResponse();
			FlagUpdateResponse.DeserializeLengthDelimited(stream, flagUpdateResponse);
			return flagUpdateResponse;
		}

		public static FlagUpdateResponse DeserializeLengthDelimited(Stream stream, FlagUpdateResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return FlagUpdateResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is FlagUpdateResponse))
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

		public static FlagUpdateResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<FlagUpdateResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			FlagUpdateResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, FlagUpdateResponse instance)
		{
		}
	}
}