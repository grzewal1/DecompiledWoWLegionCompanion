using bnet.protocol;
using System;
using System.IO;

namespace bnet.protocol.channel
{
	public class CreateChannelRequest : IProtoBuf
	{
		public bool HasAgentIdentity;

		private Identity _AgentIdentity;

		public bool HasMemberState;

		private bnet.protocol.channel.MemberState _MemberState;

		public bool HasChannelState;

		private bnet.protocol.channel.ChannelState _ChannelState;

		public bool HasChannelId;

		private EntityId _ChannelId;

		public bool HasObjectId;

		private ulong _ObjectId;

		public bool HasLocalAgent;

		private EntityId _LocalAgent;

		public bool HasLocalMemberState;

		private bnet.protocol.channel.MemberState _LocalMemberState;

		public Identity AgentIdentity
		{
			get
			{
				return this._AgentIdentity;
			}
			set
			{
				this._AgentIdentity = value;
				this.HasAgentIdentity = value != null;
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

		public bnet.protocol.channel.ChannelState ChannelState
		{
			get
			{
				return this._ChannelState;
			}
			set
			{
				this._ChannelState = value;
				this.HasChannelState = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public EntityId LocalAgent
		{
			get
			{
				return this._LocalAgent;
			}
			set
			{
				this._LocalAgent = value;
				this.HasLocalAgent = value != null;
			}
		}

		public bnet.protocol.channel.MemberState LocalMemberState
		{
			get
			{
				return this._LocalMemberState;
			}
			set
			{
				this._LocalMemberState = value;
				this.HasLocalMemberState = value != null;
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

		public CreateChannelRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			CreateChannelRequest.Deserialize(stream, this);
		}

		public static CreateChannelRequest Deserialize(Stream stream, CreateChannelRequest instance)
		{
			return CreateChannelRequest.Deserialize(stream, instance, (long)-1);
		}

		public static CreateChannelRequest Deserialize(Stream stream, CreateChannelRequest instance, long limit)
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
						if (instance.AgentIdentity != null)
						{
							Identity.DeserializeLengthDelimited(stream, instance.AgentIdentity);
						}
						else
						{
							instance.AgentIdentity = Identity.DeserializeLengthDelimited(stream);
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
					else if (num == 26)
					{
						if (instance.ChannelState != null)
						{
							bnet.protocol.channel.ChannelState.DeserializeLengthDelimited(stream, instance.ChannelState);
						}
						else
						{
							instance.ChannelState = bnet.protocol.channel.ChannelState.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 34)
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
					else if (num == 40)
					{
						instance.ObjectId = ProtocolParser.ReadUInt64(stream);
					}
					else if (num == 50)
					{
						if (instance.LocalAgent != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.LocalAgent);
						}
						else
						{
							instance.LocalAgent = EntityId.DeserializeLengthDelimited(stream);
						}
					}
					else if (num != 58)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.LocalMemberState != null)
					{
						bnet.protocol.channel.MemberState.DeserializeLengthDelimited(stream, instance.LocalMemberState);
					}
					else
					{
						instance.LocalMemberState = bnet.protocol.channel.MemberState.DeserializeLengthDelimited(stream);
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

		public static CreateChannelRequest DeserializeLengthDelimited(Stream stream)
		{
			CreateChannelRequest createChannelRequest = new CreateChannelRequest();
			CreateChannelRequest.DeserializeLengthDelimited(stream, createChannelRequest);
			return createChannelRequest;
		}

		public static CreateChannelRequest DeserializeLengthDelimited(Stream stream, CreateChannelRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return CreateChannelRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			CreateChannelRequest createChannelRequest = obj as CreateChannelRequest;
			if (createChannelRequest == null)
			{
				return false;
			}
			if (this.HasAgentIdentity != createChannelRequest.HasAgentIdentity || this.HasAgentIdentity && !this.AgentIdentity.Equals(createChannelRequest.AgentIdentity))
			{
				return false;
			}
			if (this.HasMemberState != createChannelRequest.HasMemberState || this.HasMemberState && !this.MemberState.Equals(createChannelRequest.MemberState))
			{
				return false;
			}
			if (this.HasChannelState != createChannelRequest.HasChannelState || this.HasChannelState && !this.ChannelState.Equals(createChannelRequest.ChannelState))
			{
				return false;
			}
			if (this.HasChannelId != createChannelRequest.HasChannelId || this.HasChannelId && !this.ChannelId.Equals(createChannelRequest.ChannelId))
			{
				return false;
			}
			if (this.HasObjectId != createChannelRequest.HasObjectId || this.HasObjectId && !this.ObjectId.Equals(createChannelRequest.ObjectId))
			{
				return false;
			}
			if (this.HasLocalAgent != createChannelRequest.HasLocalAgent || this.HasLocalAgent && !this.LocalAgent.Equals(createChannelRequest.LocalAgent))
			{
				return false;
			}
			if (this.HasLocalMemberState == createChannelRequest.HasLocalMemberState && (!this.HasLocalMemberState || this.LocalMemberState.Equals(createChannelRequest.LocalMemberState)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasAgentIdentity)
			{
				hashCode ^= this.AgentIdentity.GetHashCode();
			}
			if (this.HasMemberState)
			{
				hashCode ^= this.MemberState.GetHashCode();
			}
			if (this.HasChannelState)
			{
				hashCode ^= this.ChannelState.GetHashCode();
			}
			if (this.HasChannelId)
			{
				hashCode ^= this.ChannelId.GetHashCode();
			}
			if (this.HasObjectId)
			{
				hashCode ^= this.ObjectId.GetHashCode();
			}
			if (this.HasLocalAgent)
			{
				hashCode ^= this.LocalAgent.GetHashCode();
			}
			if (this.HasLocalMemberState)
			{
				hashCode ^= this.LocalMemberState.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasAgentIdentity)
			{
				num++;
				uint serializedSize = this.AgentIdentity.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasMemberState)
			{
				num++;
				uint serializedSize1 = this.MemberState.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			if (this.HasChannelState)
			{
				num++;
				uint num1 = this.ChannelState.GetSerializedSize();
				num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
			}
			if (this.HasChannelId)
			{
				num++;
				uint serializedSize2 = this.ChannelId.GetSerializedSize();
				num = num + serializedSize2 + ProtocolParser.SizeOfUInt32(serializedSize2);
			}
			if (this.HasObjectId)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.ObjectId);
			}
			if (this.HasLocalAgent)
			{
				num++;
				uint num2 = this.LocalAgent.GetSerializedSize();
				num = num + num2 + ProtocolParser.SizeOfUInt32(num2);
			}
			if (this.HasLocalMemberState)
			{
				num++;
				uint serializedSize3 = this.LocalMemberState.GetSerializedSize();
				num = num + serializedSize3 + ProtocolParser.SizeOfUInt32(serializedSize3);
			}
			return num;
		}

		public static CreateChannelRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<CreateChannelRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			CreateChannelRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, CreateChannelRequest instance)
		{
			if (instance.HasAgentIdentity)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentIdentity.GetSerializedSize());
				Identity.Serialize(stream, instance.AgentIdentity);
			}
			if (instance.HasMemberState)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.MemberState.GetSerializedSize());
				bnet.protocol.channel.MemberState.Serialize(stream, instance.MemberState);
			}
			if (instance.HasChannelState)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteUInt32(stream, instance.ChannelState.GetSerializedSize());
				bnet.protocol.channel.ChannelState.Serialize(stream, instance.ChannelState);
			}
			if (instance.HasChannelId)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteUInt32(stream, instance.ChannelId.GetSerializedSize());
				EntityId.Serialize(stream, instance.ChannelId);
			}
			if (instance.HasObjectId)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteUInt64(stream, instance.ObjectId);
			}
			if (instance.HasLocalAgent)
			{
				stream.WriteByte(50);
				ProtocolParser.WriteUInt32(stream, instance.LocalAgent.GetSerializedSize());
				EntityId.Serialize(stream, instance.LocalAgent);
			}
			if (instance.HasLocalMemberState)
			{
				stream.WriteByte(58);
				ProtocolParser.WriteUInt32(stream, instance.LocalMemberState.GetSerializedSize());
				bnet.protocol.channel.MemberState.Serialize(stream, instance.LocalMemberState);
			}
		}

		public void SetAgentIdentity(Identity val)
		{
			this.AgentIdentity = val;
		}

		public void SetChannelId(EntityId val)
		{
			this.ChannelId = val;
		}

		public void SetChannelState(bnet.protocol.channel.ChannelState val)
		{
			this.ChannelState = val;
		}

		public void SetLocalAgent(EntityId val)
		{
			this.LocalAgent = val;
		}

		public void SetLocalMemberState(bnet.protocol.channel.MemberState val)
		{
			this.LocalMemberState = val;
		}

		public void SetMemberState(bnet.protocol.channel.MemberState val)
		{
			this.MemberState = val;
		}

		public void SetObjectId(ulong val)
		{
			this.ObjectId = val;
		}
	}
}