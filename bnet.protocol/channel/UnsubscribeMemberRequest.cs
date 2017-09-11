using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel
{
	public class UnsubscribeMemberRequest : IProtoBuf
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

		public EntityId MemberId
		{
			get;
			set;
		}

		public UnsubscribeMemberRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			UnsubscribeMemberRequest.Deserialize(stream, this);
		}

		public static UnsubscribeMemberRequest Deserialize(Stream stream, UnsubscribeMemberRequest instance)
		{
			return UnsubscribeMemberRequest.Deserialize(stream, instance, (long)-1);
		}

		public static UnsubscribeMemberRequest Deserialize(Stream stream, UnsubscribeMemberRequest instance, long limit)
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
						else if (instance.MemberId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.MemberId);
						}
						else
						{
							instance.MemberId = EntityId.DeserializeLengthDelimited(stream);
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

		public static UnsubscribeMemberRequest DeserializeLengthDelimited(Stream stream)
		{
			UnsubscribeMemberRequest unsubscribeMemberRequest = new UnsubscribeMemberRequest();
			UnsubscribeMemberRequest.DeserializeLengthDelimited(stream, unsubscribeMemberRequest);
			return unsubscribeMemberRequest;
		}

		public static UnsubscribeMemberRequest DeserializeLengthDelimited(Stream stream, UnsubscribeMemberRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return UnsubscribeMemberRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			UnsubscribeMemberRequest unsubscribeMemberRequest = obj as UnsubscribeMemberRequest;
			if (unsubscribeMemberRequest == null)
			{
				return false;
			}
			if (this.HasAgentId != unsubscribeMemberRequest.HasAgentId || this.HasAgentId && !this.AgentId.Equals(unsubscribeMemberRequest.AgentId))
			{
				return false;
			}
			if (!this.MemberId.Equals(unsubscribeMemberRequest.MemberId))
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
			hashCode ^= this.MemberId.GetHashCode();
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
			uint serializedSize1 = this.MemberId.GetSerializedSize();
			num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			num++;
			return num;
		}

		public static UnsubscribeMemberRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<UnsubscribeMemberRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			UnsubscribeMemberRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, UnsubscribeMemberRequest instance)
		{
			if (instance.HasAgentId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentId.GetSerializedSize());
				EntityId.Serialize(stream, instance.AgentId);
			}
			if (instance.MemberId == null)
			{
				throw new ArgumentNullException("MemberId", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.MemberId.GetSerializedSize());
			EntityId.Serialize(stream, instance.MemberId);
		}

		public void SetAgentId(EntityId val)
		{
			this.AgentId = val;
		}

		public void SetMemberId(EntityId val)
		{
			this.MemberId = val;
		}
	}
}