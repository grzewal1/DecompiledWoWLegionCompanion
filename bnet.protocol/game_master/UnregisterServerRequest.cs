using System;
using System.IO;

namespace bnet.protocol.game_master
{
	public class UnregisterServerRequest : IProtoBuf
	{
		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public UnregisterServerRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			UnregisterServerRequest.Deserialize(stream, this);
		}

		public static UnregisterServerRequest Deserialize(Stream stream, UnregisterServerRequest instance)
		{
			return UnregisterServerRequest.Deserialize(stream, instance, (long)-1);
		}

		public static UnregisterServerRequest Deserialize(Stream stream, UnregisterServerRequest instance, long limit)
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

		public static UnregisterServerRequest DeserializeLengthDelimited(Stream stream)
		{
			UnregisterServerRequest unregisterServerRequest = new UnregisterServerRequest();
			UnregisterServerRequest.DeserializeLengthDelimited(stream, unregisterServerRequest);
			return unregisterServerRequest;
		}

		public static UnregisterServerRequest DeserializeLengthDelimited(Stream stream, UnregisterServerRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return UnregisterServerRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is UnregisterServerRequest))
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

		public static UnregisterServerRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<UnregisterServerRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			UnregisterServerRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, UnregisterServerRequest instance)
		{
		}
	}
}