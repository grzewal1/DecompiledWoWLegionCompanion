using bnet.protocol;
using bnet.protocol.channel;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel_invitation
{
	public class AcceptInvitationRequest : IProtoBuf
	{
		public bool HasAgentId;

		private EntityId _AgentId;

		public bool HasMemberState;

		private bnet.protocol.channel.MemberState _MemberState;

		public bool HasChannelId;

		private EntityId _ChannelId;

		public bool HasServiceType;

		private uint _ServiceType;

		public bool HasLocalSubscriber;

		private bool _LocalSubscriber;

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
			get
			{
				return this._ChannelId;
			}
			set
			{
				this._ChannelId = value;
				this.HasChannelId = value != null;
			}
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

		public bool LocalSubscriber
		{
			get
			{
				return this._LocalSubscriber;
			}
			set
			{
				this._LocalSubscriber = value;
				this.HasLocalSubscriber = true;
			}
		}

		public bnet.protocol.channel.MemberState MemberState
		{
			get
			{
				return this._MemberState;
			}
			set
			{
				this._MemberState = value;
				this.HasMemberState = value != null;
			}
		}

		public ulong ObjectId
		{
			get;
			set;
		}

		public uint ServiceType
		{
			get
			{
				return this._ServiceType;
			}
			set
			{
				this._ServiceType = value;
				this.HasServiceType = true;
			}
		}

		public AcceptInvitationRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			AcceptInvitationRequest.Deserialize(stream, this);
		}

		public static AcceptInvitationRequest Deserialize(Stream stream, AcceptInvitationRequest instance)
		{
			return AcceptInvitationRequest.Deserialize(stream, instance, (long)-1);
		}

		public static AcceptInvitationRequest Deserialize(Stream stream, AcceptInvitationRequest instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			instance.LocalSubscriber = true;
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
					else if (num == 18)
					{
						if (instance.MemberState != null)
						{
							bnet.protocol.channel.MemberState.DeserializeLengthDelimited(stream, instance.MemberState);
						}
						else
						{
							instance.MemberState = bnet.protocol.channel.MemberState.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 25)
					{
						instance.InvitationId = binaryReader.ReadUInt64();
					}
					else if (num == 32)
					{
						instance.ObjectId = ProtocolParser.ReadUInt64(stream);
					}
					else if (num == 42)
					{
						if (instance.ChannelId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.ChannelId);
						}
						else
						{
							instance.ChannelId = EntityId.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 48)
					{
						instance.ServiceType = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 56)
					{
						instance.LocalSubscriber = ProtocolParser.ReadBool(stream);
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

		public static AcceptInvitationRequest DeserializeLengthDelimited(Stream stream)
		{
			AcceptInvitationRequest acceptInvitationRequest = new AcceptInvitationRequest();
			AcceptInvitationRequest.DeserializeLengthDelimited(stream, acceptInvitationRequest);
			return acceptInvitationRequest;
		}

		public static AcceptInvitationRequest DeserializeLengthDelimited(Stream stream, AcceptInvitationRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return AcceptInvitationRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			AcceptInvitationRequest acceptInvitationRequest = obj as AcceptInvitationRequest;
			if (acceptInvitationRequest == null)
			{
				return false;
			}
			if (this.HasAgentId != acceptInvitationRequest.HasAgentId || this.HasAgentId && !this.AgentId.Equals(acceptInvitationRequest.AgentId))
			{
				return false;
			}
			if (this.HasMemberState != acceptInvitationRequest.HasMemberState || this.HasMemberState && !this.MemberState.Equals(acceptInvitationRequest.MemberState))
			{
				return false;
			}
			if (!this.InvitationId.Equals(acceptInvitationRequest.InvitationId))
			{
				return false;
			}
			if (!this.ObjectId.Equals(acceptInvitationRequest.ObjectId))
			{
				return false;
			}
			if (this.HasChannelId != acceptInvitationRequest.HasChannelId || this.HasChannelId && !this.ChannelId.Equals(acceptInvitationRequest.ChannelId))
			{
				return false;
			}
			if (this.HasServiceType != acceptInvitationRequest.HasServiceType || this.HasServiceType && !this.ServiceType.Equals(acceptInvitationRequest.ServiceType))
			{
				return false;
			}
			if (this.HasLocalSubscriber == acceptInvitationRequest.HasLocalSubscriber && (!this.HasLocalSubscriber || this.LocalSubscriber.Equals(acceptInvitationRequest.LocalSubscriber)))
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
				hashCode ^= this.AgentId.GetHashCode();
			}
			if (this.HasMemberState)
			{
				hashCode ^= this.MemberState.GetHashCode();
			}
			hashCode ^= this.InvitationId.GetHashCode();
			hashCode ^= this.ObjectId.GetHashCode();
			if (this.HasChannelId)
			{
				hashCode ^= this.ChannelId.GetHashCode();
			}
			if (this.HasServiceType)
			{
				hashCode ^= this.ServiceType.GetHashCode();
			}
			if (this.HasLocalSubscriber)
			{
				hashCode ^= this.LocalSubscriber.GetHashCode();
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
			if (this.HasMemberState)
			{
				num++;
				uint serializedSize1 = this.MemberState.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			num += 8;
			num += ProtocolParser.SizeOfUInt64(this.ObjectId);
			if (this.HasChannelId)
			{
				num++;
				uint num1 = this.ChannelId.GetSerializedSize();
				num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
			}
			if (this.HasServiceType)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.ServiceType);
			}
			if (this.HasLocalSubscriber)
			{
				num++;
				num++;
			}
			num += 2;
			return num;
		}

		public static AcceptInvitationRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<AcceptInvitationRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			AcceptInvitationRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, AcceptInvitationRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasAgentId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentId.GetSerializedSize());
				EntityId.Serialize(stream, instance.AgentId);
			}
			if (instance.HasMemberState)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.MemberState.GetSerializedSize());
				bnet.protocol.channel.MemberState.Serialize(stream, instance.MemberState);
			}
			stream.WriteByte(25);
			binaryWriter.Write(instance.InvitationId);
			stream.WriteByte(32);
			ProtocolParser.WriteUInt64(stream, instance.ObjectId);
			if (instance.HasChannelId)
			{
				stream.WriteByte(42);
				ProtocolParser.WriteUInt32(stream, instance.ChannelId.GetSerializedSize());
				EntityId.Serialize(stream, instance.ChannelId);
			}
			if (instance.HasServiceType)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteUInt32(stream, instance.ServiceType);
			}
			if (instance.HasLocalSubscriber)
			{
				stream.WriteByte(56);
				ProtocolParser.WriteBool(stream, instance.LocalSubscriber);
			}
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

		public void SetLocalSubscriber(bool val)
		{
			this.LocalSubscriber = val;
		}

		public void SetMemberState(bnet.protocol.channel.MemberState val)
		{
			this.MemberState = val;
		}

		public void SetObjectId(ulong val)
		{
			this.ObjectId = val;
		}

		public void SetServiceType(uint val)
		{
			this.ServiceType = val;
		}
	}
}