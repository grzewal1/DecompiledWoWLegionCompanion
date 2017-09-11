using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel_invitation
{
	public class RevokeInvitationRequest : IProtoBuf
	{
		public bool HasAgentId;

		private EntityId _AgentId;

		public bool HasTargetId;

		private EntityId _TargetId;

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

		public ulong InvitationId
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

		public EntityId TargetId
		{
			get
			{
				return this._TargetId;
			}
			set
			{
				this._TargetId = value;
				this.HasTargetId = value != null;
			}
		}

		public RevokeInvitationRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			RevokeInvitationRequest.Deserialize(stream, this);
		}

		public static RevokeInvitationRequest Deserialize(Stream stream, RevokeInvitationRequest instance)
		{
			return RevokeInvitationRequest.Deserialize(stream, instance, (long)-1);
		}

		public static RevokeInvitationRequest Deserialize(Stream stream, RevokeInvitationRequest instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
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
						else if (num1 == 18)
						{
							if (instance.TargetId != null)
							{
								EntityId.DeserializeLengthDelimited(stream, instance.TargetId);
							}
							else
							{
								instance.TargetId = EntityId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 25)
						{
							instance.InvitationId = binaryReader.ReadUInt64();
						}
						else if (num1 != 34)
						{
							Key key = ProtocolParser.ReadKey((byte)num, stream);
							if (key.Field == 0)
							{
								throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
							}
							ProtocolParser.SkipKey(stream, key);
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

		public static RevokeInvitationRequest DeserializeLengthDelimited(Stream stream)
		{
			RevokeInvitationRequest revokeInvitationRequest = new RevokeInvitationRequest();
			RevokeInvitationRequest.DeserializeLengthDelimited(stream, revokeInvitationRequest);
			return revokeInvitationRequest;
		}

		public static RevokeInvitationRequest DeserializeLengthDelimited(Stream stream, RevokeInvitationRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return RevokeInvitationRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			RevokeInvitationRequest revokeInvitationRequest = obj as RevokeInvitationRequest;
			if (revokeInvitationRequest == null)
			{
				return false;
			}
			if (this.HasAgentId != revokeInvitationRequest.HasAgentId || this.HasAgentId && !this.AgentId.Equals(revokeInvitationRequest.AgentId))
			{
				return false;
			}
			if (this.HasTargetId != revokeInvitationRequest.HasTargetId || this.HasTargetId && !this.TargetId.Equals(revokeInvitationRequest.TargetId))
			{
				return false;
			}
			if (!this.InvitationId.Equals(revokeInvitationRequest.InvitationId))
			{
				return false;
			}
			if (!this.ChannelId.Equals(revokeInvitationRequest.ChannelId))
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
			if (this.HasTargetId)
			{
				hashCode ^= this.TargetId.GetHashCode();
			}
			hashCode ^= this.InvitationId.GetHashCode();
			hashCode ^= this.ChannelId.GetHashCode();
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
			if (this.HasTargetId)
			{
				num++;
				uint serializedSize1 = this.TargetId.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			num += 8;
			uint num1 = this.ChannelId.GetSerializedSize();
			num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
			num += 2;
			return num;
		}

		public static RevokeInvitationRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<RevokeInvitationRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			RevokeInvitationRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, RevokeInvitationRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasAgentId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentId.GetSerializedSize());
				EntityId.Serialize(stream, instance.AgentId);
			}
			if (instance.HasTargetId)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.TargetId.GetSerializedSize());
				EntityId.Serialize(stream, instance.TargetId);
			}
			stream.WriteByte(25);
			binaryWriter.Write(instance.InvitationId);
			if (instance.ChannelId == null)
			{
				throw new ArgumentNullException("ChannelId", "Required by proto specification.");
			}
			stream.WriteByte(34);
			ProtocolParser.WriteUInt32(stream, instance.ChannelId.GetSerializedSize());
			EntityId.Serialize(stream, instance.ChannelId);
		}

		public void SetAgentId(EntityId val)
		{
			this.AgentId = val;
		}

		public void SetChannelId(EntityId val)
		{
			this.ChannelId = val;
		}

		public void SetInvitationId(ulong val)
		{
			this.InvitationId = val;
		}

		public void SetTargetId(EntityId val)
		{
			this.TargetId = val;
		}
	}
}