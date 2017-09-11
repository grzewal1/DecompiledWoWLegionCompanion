using System;
using System.IO;

namespace bnet.protocol.connection
{
	public class EncryptRequest : IProtoBuf
	{
		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public EncryptRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			EncryptRequest.Deserialize(stream, this);
		}

		public static EncryptRequest Deserialize(Stream stream, EncryptRequest instance)
		{
			return EncryptRequest.Deserialize(stream, instance, (long)-1);
		}

		public static EncryptRequest Deserialize(Stream stream, EncryptRequest instance, long limit)
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

		public static EncryptRequest DeserializeLengthDelimited(Stream stream)
		{
			EncryptRequest encryptRequest = new EncryptRequest();
			EncryptRequest.DeserializeLengthDelimited(stream, encryptRequest);
			return encryptRequest;
		}

		public static EncryptRequest DeserializeLengthDelimited(Stream stream, EncryptRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return EncryptRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is EncryptRequest))
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

		public static EncryptRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<EncryptRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			EncryptRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, EncryptRequest instance)
		{
		}
	}
}