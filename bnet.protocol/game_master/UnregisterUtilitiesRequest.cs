using System;
using System.IO;

namespace bnet.protocol.game_master
{
	public class UnregisterUtilitiesRequest : IProtoBuf
	{
		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public UnregisterUtilitiesRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			UnregisterUtilitiesRequest.Deserialize(stream, this);
		}

		public static UnregisterUtilitiesRequest Deserialize(Stream stream, UnregisterUtilitiesRequest instance)
		{
			return UnregisterUtilitiesRequest.Deserialize(stream, instance, (long)-1);
		}

		public static UnregisterUtilitiesRequest Deserialize(Stream stream, UnregisterUtilitiesRequest instance, long limit)
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

		public static UnregisterUtilitiesRequest DeserializeLengthDelimited(Stream stream)
		{
			UnregisterUtilitiesRequest unregisterUtilitiesRequest = new UnregisterUtilitiesRequest();
			UnregisterUtilitiesRequest.DeserializeLengthDelimited(stream, unregisterUtilitiesRequest);
			return unregisterUtilitiesRequest;
		}

		public static UnregisterUtilitiesRequest DeserializeLengthDelimited(Stream stream, UnregisterUtilitiesRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return UnregisterUtilitiesRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is UnregisterUtilitiesRequest))
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

		public static UnregisterUtilitiesRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<UnregisterUtilitiesRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			UnregisterUtilitiesRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, UnregisterUtilitiesRequest instance)
		{
		}
	}
}