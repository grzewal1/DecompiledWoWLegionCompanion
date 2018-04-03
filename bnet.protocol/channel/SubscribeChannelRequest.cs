using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel
{
	public class SubscribeChannelRequest : IProtoBuf
	{
		public bool HasAgentId;

		private EntityId _AgentId;

		public EntityId AgentId
		{
			get
			{
				return this._AgentId;
			}
			set
			{
				this._AgentId = value;
				this.HasAgentId = value != null;
			}
		}

		public EntityId ChannelId
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

		public ulong ObjectId
		{
			get;
			set;
		}

		public SubscribeChannelRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			SubscribeChannelRequest.Deserialize(stream, this);
		}

		public static SubscribeChannelRequest Deserialize(Stream stream, SubscribeChannelRequest instance)
		{
			return SubscribeChannelRequest.Deserialize(stream, instance, (long)-1);
		}

		public static SubscribeChannelRequest Deserialize(Stream stream, SubscribeChannelRequest instance, long limit)
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
					else if (num == 10)
					{
						if (instance.AgentId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.AgentId);
						}
						else
						{
							instance.AgentId = EntityId.DeserializeLengthDelimited(stream);
						}
					}
					else if (num != 18)
					{
						if (num == 24)
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
					else if (instance.ChannelId != null)
					{
						EntityId.DeserializeLengthDelimited(stream, instance.ChannelId);
					}
					else
					{
						instance.ChannelId = EntityId.DeserializeLengthDelimited(stream);
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

		public static SubscribeChannelRequest DeserializeLengthDelimited(Stream stream)
		{
			SubscribeChannelRequest subscribeChannelRequest = new SubscribeChannelRequest();
			SubscribeChannelRequest.DeserializeLengthDelimited(stream, subscribeChannelRequest);
			return subscribeChannelRequest;
		}

		public static SubscribeChannelRequest DeserializeLengthDelimited(Stream stream, SubscribeChannelRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return SubscribeChannelRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			SubscribeChannelRequest subscribeChannelRequest = obj as SubscribeChannelRequest;
			if (subscribeChannelRequest == null)
			{
				return false;
			}
			if (this.HasAgentId != subscribeChannelRequest.HasAgentId || this.HasAgentId && !this.AgentId.Equals(subscribeChannelRequest.AgentId))
			{
				return false;
			}
			if (!this.ChannelId.Equals(subscribeChannelRequest.ChannelId))
			{
				return false;
			}
			if (!this.ObjectId.Equals(subscribeChannelRequest.ObjectId))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasAgentId)
			{
				hashCode ^= this.AgentId.GetHashCode();
			}
			hashCode ^= this.ChannelId.GetHashCode();
			hashCode ^= this.ObjectId.GetHashCode();
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasAgentId)
			{
				num++;
				uint serializedSize = this.AgentId.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			uint serializedSize1 = this.ChannelId.GetSerializedSize();
			num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			num += ProtocolParser.SizeOfUInt64(this.ObjectId);
			num += 2;
			return num;
		}

		public static SubscribeChannelRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<SubscribeChannelRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			SubscribeChannelRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, SubscribeChannelRequest instance)
		{
			if (instance.HasAgentId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentId.GetSerializedSize());
				EntityId.Serialize(stream, instance.AgentId);
			}
			if (instance.ChannelId == null)
			{
				throw new ArgumentNullException("ChannelId", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.ChannelId.GetSerializedSize());
			EntityId.Serialize(stream, instance.ChannelId);
			stream.WriteByte(24);
			ProtocolParser.WriteUInt64(stream, instance.ObjectId);
		}

		public void SetAgentId(EntityId val)
		{
			this.AgentId = val;
		}

		public void SetChannelId(EntityId val)
		{
			this.ChannelId = val;
		}

		public void SetObjectId(ulong val)
		{
			this.ObjectId = val;
		}
	}
}