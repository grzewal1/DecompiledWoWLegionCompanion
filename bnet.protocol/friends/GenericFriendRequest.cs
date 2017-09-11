using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.friends
{
	public class GenericFriendRequest : IProtoBuf
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

		public EntityId TargetId
		{
			get;
			set;
		}

		public GenericFriendRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			GenericFriendRequest.Deserialize(stream, this);
		}

		public static GenericFriendRequest Deserialize(Stream stream, GenericFriendRequest instance)
		{
			return GenericFriendRequest.Deserialize(stream, instance, (long)-1);
		}

		public static GenericFriendRequest Deserialize(Stream stream, GenericFriendRequest instance, long limit)
		{
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 == 10)
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
						else if (num1 != 18)
						{
							Key key = ProtocolParser.ReadKey((byte)num, stream);
							if (key.Field == 0)
							{
								throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
							}
							ProtocolParser.SkipKey(stream, key);
						}
						else if (instance.TargetId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.TargetId);
						}
						else
						{
							instance.TargetId = EntityId.DeserializeLengthDelimited(stream);
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

		public static GenericFriendRequest DeserializeLengthDelimited(Stream stream)
		{
			GenericFriendRequest genericFriendRequest = new GenericFriendRequest();
			GenericFriendRequest.DeserializeLengthDelimited(stream, genericFriendRequest);
			return genericFriendRequest;
		}

		public static GenericFriendRequest DeserializeLengthDelimited(Stream stream, GenericFriendRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GenericFriendRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GenericFriendRequest genericFriendRequest = obj as GenericFriendRequest;
			if (genericFriendRequest == null)
			{
				return false;
			}
			if (this.HasAgentId != genericFriendRequest.HasAgentId || this.HasAgentId && !this.AgentId.Equals(genericFriendRequest.AgentId))
			{
				return false;
			}
			if (!this.TargetId.Equals(genericFriendRequest.TargetId))
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
			hashCode ^= this.TargetId.GetHashCode();
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
			uint serializedSize1 = this.TargetId.GetSerializedSize();
			num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			num++;
			return num;
		}

		public static GenericFriendRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GenericFriendRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GenericFriendRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GenericFriendRequest instance)
		{
			if (instance.HasAgentId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentId.GetSerializedSize());
				EntityId.Serialize(stream, instance.AgentId);
			}
			if (instance.TargetId == null)
			{
				throw new ArgumentNullException("TargetId", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.TargetId.GetSerializedSize());
			EntityId.Serialize(stream, instance.TargetId);
		}

		public void SetAgentId(EntityId val)
		{
			this.AgentId = val;
		}

		public void SetTargetId(EntityId val)
		{
			this.TargetId = val;
		}
	}
}