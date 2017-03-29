using bnet.protocol;
using System;
using System.IO;

namespace bnet.protocol.friends
{
	public class UnsubscribeToFriendsRequest : IProtoBuf
	{
		public bool HasAgentId;

		private EntityId _AgentId;

		public bool HasObjectId;

		private ulong _ObjectId;

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
			get
			{
				return this._ObjectId;
			}
			set
			{
				this._ObjectId = value;
				this.HasObjectId = true;
			}
		}

		public UnsubscribeToFriendsRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			UnsubscribeToFriendsRequest.Deserialize(stream, this);
		}

		public static UnsubscribeToFriendsRequest Deserialize(Stream stream, UnsubscribeToFriendsRequest instance)
		{
			return UnsubscribeToFriendsRequest.Deserialize(stream, instance, (long)-1);
		}

		public static UnsubscribeToFriendsRequest Deserialize(Stream stream, UnsubscribeToFriendsRequest instance, long limit)
		{
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 != 10)
						{
							if (num1 == 16)
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

		public static UnsubscribeToFriendsRequest DeserializeLengthDelimited(Stream stream)
		{
			UnsubscribeToFriendsRequest unsubscribeToFriendsRequest = new UnsubscribeToFriendsRequest();
			UnsubscribeToFriendsRequest.DeserializeLengthDelimited(stream, unsubscribeToFriendsRequest);
			return unsubscribeToFriendsRequest;
		}

		public static UnsubscribeToFriendsRequest DeserializeLengthDelimited(Stream stream, UnsubscribeToFriendsRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return UnsubscribeToFriendsRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			UnsubscribeToFriendsRequest unsubscribeToFriendsRequest = obj as UnsubscribeToFriendsRequest;
			if (unsubscribeToFriendsRequest == null)
			{
				return false;
			}
			if (this.HasAgentId != unsubscribeToFriendsRequest.HasAgentId || this.HasAgentId && !this.AgentId.Equals(unsubscribeToFriendsRequest.AgentId))
			{
				return false;
			}
			if (this.HasObjectId == unsubscribeToFriendsRequest.HasObjectId && (!this.HasObjectId || this.ObjectId.Equals(unsubscribeToFriendsRequest.ObjectId)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasAgentId)
			{
				hashCode = hashCode ^ this.AgentId.GetHashCode();
			}
			if (this.HasObjectId)
			{
				hashCode = hashCode ^ this.ObjectId.GetHashCode();
			}
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
			if (this.HasObjectId)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt64(this.ObjectId);
			}
			return num;
		}

		public static UnsubscribeToFriendsRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<UnsubscribeToFriendsRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			UnsubscribeToFriendsRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, UnsubscribeToFriendsRequest instance)
		{
			if (instance.HasAgentId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentId.GetSerializedSize());
				EntityId.Serialize(stream, instance.AgentId);
			}
			if (instance.HasObjectId)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, instance.ObjectId);
			}
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