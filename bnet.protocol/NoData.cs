using System;
using System.IO;

namespace bnet.protocol
{
	public class NoData : IProtoBuf
	{
		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public NoData()
		{
		}

		public void Deserialize(Stream stream)
		{
			NoData.Deserialize(stream, this);
		}

		public static NoData Deserialize(Stream stream, NoData instance)
		{
			return NoData.Deserialize(stream, instance, (long)-1);
		}

		public static NoData Deserialize(Stream stream, NoData instance, long limit)
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

		public static NoData DeserializeLengthDelimited(Stream stream)
		{
			NoData noDatum = new NoData();
			NoData.DeserializeLengthDelimited(stream, noDatum);
			return noDatum;
		}

		public static NoData DeserializeLengthDelimited(Stream stream, NoData instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return NoData.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is NoData))
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

		public static NoData ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<NoData>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			NoData.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, NoData instance)
		{
		}
	}
}