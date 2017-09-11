using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.game_master
{
	public class SubscribeRequest : IProtoBuf
	{
		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public ulong ObjectId
		{
			get;
			set;
		}

		public SubscribeRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			SubscribeRequest.Deserialize(stream, this);
		}

		public static SubscribeRequest Deserialize(Stream stream, SubscribeRequest instance)
		{
			return SubscribeRequest.Deserialize(stream, instance, (long)-1);
		}

		public static SubscribeRequest Deserialize(Stream stream, SubscribeRequest instance, long limit)
		{
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num == -1)
					{
						if (limit >= (long)0)
						{
							throw new EndOfStreamException();
						}
						break;
					}
					else if (num == 8)
					{
						instance.ObjectId = ProtocolParser.ReadUInt64(stream);
					}
					else
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
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

		public static SubscribeRequest DeserializeLengthDelimited(Stream stream)
		{
			SubscribeRequest subscribeRequest = new SubscribeRequest();
			SubscribeRequest.DeserializeLengthDelimited(stream, subscribeRequest);
			return subscribeRequest;
		}

		public static SubscribeRequest DeserializeLengthDelimited(Stream stream, SubscribeRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return SubscribeRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			SubscribeRequest subscribeRequest = obj as SubscribeRequest;
			if (subscribeRequest == null)
			{
				return false;
			}
			if (!this.ObjectId.Equals(subscribeRequest.ObjectId))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			return hashCode ^ this.ObjectId.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0 + ProtocolParser.SizeOfUInt64(this.ObjectId);
			return num + 1;
		}

		public static SubscribeRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<SubscribeRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			SubscribeRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, SubscribeRequest instance)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, instance.ObjectId);
		}

		public void SetObjectId(ulong val)
		{
			this.ObjectId = val;
		}
	}
}