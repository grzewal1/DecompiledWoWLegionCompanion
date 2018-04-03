using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.friends
{
	public class SubscribeToFriendsRequest : IProtoBuf
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

		public SubscribeToFriendsRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			SubscribeToFriendsRequest.Deserialize(stream, this);
		}

		public static SubscribeToFriendsRequest Deserialize(Stream stream, SubscribeToFriendsRequest instance)
		{
			return SubscribeToFriendsRequest.Deserialize(stream, instance, (long)-1);
		}

		public static SubscribeToFriendsRequest Deserialize(Stream stream, SubscribeToFriendsRequest instance, long limit)
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
						if (num == 16)
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
					else if (instance.AgentId != null)
					{
						EntityId.DeserializeLengthDelimited(stream, instance.AgentId);
					}
					else
					{
						instance.AgentId = EntityId.DeserializeLengthDelimited(stream);
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

		public static SubscribeToFriendsRequest DeserializeLengthDelimited(Stream stream)
		{
			SubscribeToFriendsRequest subscribeToFriendsRequest = new SubscribeToFriendsRequest();
			SubscribeToFriendsRequest.DeserializeLengthDelimited(stream, subscribeToFriendsRequest);
			return subscribeToFriendsRequest;
		}

		public static SubscribeToFriendsRequest DeserializeLengthDelimited(Stream stream, SubscribeToFriendsRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return SubscribeToFriendsRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			SubscribeToFriendsRequest subscribeToFriendsRequest = obj as SubscribeToFriendsRequest;
			if (subscribeToFriendsRequest == null)
			{
				return false;
			}
			if (this.HasAgentId != subscribeToFriendsRequest.HasAgentId || this.HasAgentId && !this.AgentId.Equals(subscribeToFriendsRequest.AgentId))
			{
				return false;
			}
			if (!this.ObjectId.Equals(subscribeToFriendsRequest.ObjectId))
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
			num += ProtocolParser.SizeOfUInt64(this.ObjectId);
			num++;
			return num;
		}

		public static SubscribeToFriendsRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<SubscribeToFriendsRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			SubscribeToFriendsRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, SubscribeToFriendsRequest instance)
		{
			if (instance.HasAgentId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentId.GetSerializedSize());
				EntityId.Serialize(stream, instance.AgentId);
			}
			stream.WriteByte(16);
			ProtocolParser.WriteUInt64(stream, instance.ObjectId);
		}

		public void SetAgentId(EntityId val)
		{
			this.AgentId = val;
		}

		public void SetObjectId(ulong val)
		{
			this.ObjectId = val;
		}
	}
}