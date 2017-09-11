using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.game_master
{
	public class UnsubscribeRequest : IProtoBuf
	{
		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public ulong SubscriptionId
		{
			get;
			set;
		}

		public UnsubscribeRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			UnsubscribeRequest.Deserialize(stream, this);
		}

		public static UnsubscribeRequest Deserialize(Stream stream, UnsubscribeRequest instance)
		{
			return UnsubscribeRequest.Deserialize(stream, instance, (long)-1);
		}

		public static UnsubscribeRequest Deserialize(Stream stream, UnsubscribeRequest instance, long limit)
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
						instance.SubscriptionId = ProtocolParser.ReadUInt64(stream);
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

		public static UnsubscribeRequest DeserializeLengthDelimited(Stream stream)
		{
			UnsubscribeRequest unsubscribeRequest = new UnsubscribeRequest();
			UnsubscribeRequest.DeserializeLengthDelimited(stream, unsubscribeRequest);
			return unsubscribeRequest;
		}

		public static UnsubscribeRequest DeserializeLengthDelimited(Stream stream, UnsubscribeRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return UnsubscribeRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			UnsubscribeRequest unsubscribeRequest = obj as UnsubscribeRequest;
			if (unsubscribeRequest == null)
			{
				return false;
			}
			if (!this.SubscriptionId.Equals(unsubscribeRequest.SubscriptionId))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			return hashCode ^ this.SubscriptionId.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0 + ProtocolParser.SizeOfUInt64(this.SubscriptionId);
			return num + 1;
		}

		public static UnsubscribeRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<UnsubscribeRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			UnsubscribeRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, UnsubscribeRequest instance)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, instance.SubscriptionId);
		}

		public void SetSubscriptionId(ulong val)
		{
			this.SubscriptionId = val;
		}
	}
}