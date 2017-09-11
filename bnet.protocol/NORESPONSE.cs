using System;
using System.IO;

namespace bnet.protocol
{
	public class NORESPONSE : IProtoBuf
	{
		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public NORESPONSE()
		{
		}

		public void Deserialize(Stream stream)
		{
			NORESPONSE.Deserialize(stream, this);
		}

		public static NORESPONSE Deserialize(Stream stream, NORESPONSE instance)
		{
			return NORESPONSE.Deserialize(stream, instance, (long)-1);
		}

		public static NORESPONSE Deserialize(Stream stream, NORESPONSE instance, long limit)
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

		public static NORESPONSE DeserializeLengthDelimited(Stream stream)
		{
			NORESPONSE nORESPONSE = new NORESPONSE();
			NORESPONSE.DeserializeLengthDelimited(stream, nORESPONSE);
			return nORESPONSE;
		}

		public static NORESPONSE DeserializeLengthDelimited(Stream stream, NORESPONSE instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return NORESPONSE.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is NORESPONSE))
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

		public static NORESPONSE ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<NORESPONSE>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			NORESPONSE.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, NORESPONSE instance)
		{
		}
	}
}