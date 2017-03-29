using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.presence
{
	public class SubscribeNotificationRequest : IProtoBuf
	{
		public bnet.protocol.EntityId EntityId
		{
			get;
			set;
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public SubscribeNotificationRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			SubscribeNotificationRequest.Deserialize(stream, this);
		}

		public static SubscribeNotificationRequest Deserialize(Stream stream, SubscribeNotificationRequest instance)
		{
			return SubscribeNotificationRequest.Deserialize(stream, instance, (long)-1);
		}

		public static SubscribeNotificationRequest Deserialize(Stream stream, SubscribeNotificationRequest instance, long limit)
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
					else if (num != 10)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.EntityId != null)
					{
						bnet.protocol.EntityId.DeserializeLengthDelimited(stream, instance.EntityId);
					}
					else
					{
						instance.EntityId = bnet.protocol.EntityId.DeserializeLengthDelimited(stream);
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

		public static SubscribeNotificationRequest DeserializeLengthDelimited(Stream stream)
		{
			SubscribeNotificationRequest subscribeNotificationRequest = new SubscribeNotificationRequest();
			SubscribeNotificationRequest.DeserializeLengthDelimited(stream, subscribeNotificationRequest);
			return subscribeNotificationRequest;
		}

		public static SubscribeNotificationRequest DeserializeLengthDelimited(Stream stream, SubscribeNotificationRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return SubscribeNotificationRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			SubscribeNotificationRequest subscribeNotificationRequest = obj as SubscribeNotificationRequest;
			if (subscribeNotificationRequest == null)
			{
				return false;
			}
			if (!this.EntityId.Equals(subscribeNotificationRequest.EntityId))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			return hashCode ^ this.EntityId.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.EntityId.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			return num + 1;
		}

		public static SubscribeNotificationRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<SubscribeNotificationRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			SubscribeNotificationRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, SubscribeNotificationRequest instance)
		{
			if (instance.EntityId == null)
			{
				throw new ArgumentNullException("EntityId", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.EntityId.GetSerializedSize());
			bnet.protocol.EntityId.Serialize(stream, instance.EntityId);
		}

		public void SetEntityId(bnet.protocol.EntityId val)
		{
			this.EntityId = val;
		}
	}
}