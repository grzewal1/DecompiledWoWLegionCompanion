using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel_invitation
{
	public class SuggestInvitationRequest : IProtoBuf
	{
		public bool HasAgentId;

		private EntityId _AgentId;

		public bool HasApprovalId;

		private EntityId _ApprovalId;

		public bool HasAgentIdentity;

		private Identity _AgentIdentity;

		public bool HasAgentInfo;

		private AccountInfo _AgentInfo;

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

		public AccountInfo AgentInfo
		{
			get
			{
				return this._AgentInfo;
			}
			set
			{
				this._AgentInfo = value;
				this.HasAgentInfo = value != null;
			}
		}

		public EntityId ApprovalId
		{
			get
			{
				return this._ApprovalId;
			}
			set
			{
				this._ApprovalId = value;
				this.HasApprovalId = value != null;
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

		public EntityId TargetId
		{
			get;
			set;
		}

		public SuggestInvitationRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			SuggestInvitationRequest.Deserialize(stream, this);
		}

		public static SuggestInvitationRequest Deserialize(Stream stream, SuggestInvitationRequest instance)
		{
			return SuggestInvitationRequest.Deserialize(stream, instance, (long)-1);
		}

		public static SuggestInvitationRequest Deserialize(Stream stream, SuggestInvitationRequest instance, long limit)
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
						else if (num1 == 18)
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
						else if (num1 == 26)
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
						else if (num1 == 34)
						{
							if (instance.ApprovalId != null)
							{
								EntityId.DeserializeLengthDelimited(stream, instance.ApprovalId);
							}
							else
							{
								instance.ApprovalId = EntityId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 42)
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
						else if (num1 != 50)
						{
							Key key = ProtocolParser.ReadKey((byte)num, stream);
							if (key.Field == 0)
							{
								throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
							}
							ProtocolParser.SkipKey(stream, key);
						}
						else if (instance.AgentInfo != null)
						{
							AccountInfo.DeserializeLengthDelimited(stream, instance.AgentInfo);
						}
						else
						{
							instance.AgentInfo = AccountInfo.DeserializeLengthDelimited(stream);
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

		public static SuggestInvitationRequest DeserializeLengthDelimited(Stream stream)
		{
			SuggestInvitationRequest suggestInvitationRequest = new SuggestInvitationRequest();
			SuggestInvitationRequest.DeserializeLengthDelimited(stream, suggestInvitationRequest);
			return suggestInvitationRequest;
		}

		public static SuggestInvitationRequest DeserializeLengthDelimited(Stream stream, SuggestInvitationRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return SuggestInvitationRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			SuggestInvitationRequest suggestInvitationRequest = obj as SuggestInvitationRequest;
			if (suggestInvitationRequest == null)
			{
				return false;
			}
			if (this.HasAgentId != suggestInvitationRequest.HasAgentId || this.HasAgentId && !this.AgentId.Equals(suggestInvitationRequest.AgentId))
			{
				return false;
			}
			if (!this.ChannelId.Equals(suggestInvitationRequest.ChannelId))
			{
				return false;
			}
			if (!this.TargetId.Equals(suggestInvitationRequest.TargetId))
			{
				return false;
			}
			if (this.HasApprovalId != suggestInvitationRequest.HasApprovalId || this.HasApprovalId && !this.ApprovalId.Equals(suggestInvitationRequest.ApprovalId))
			{
				return false;
			}
			if (this.HasAgentIdentity != suggestInvitationRequest.HasAgentIdentity || this.HasAgentIdentity && !this.AgentIdentity.Equals(suggestInvitationRequest.AgentIdentity))
			{
				return false;
			}
			if (this.HasAgentInfo == suggestInvitationRequest.HasAgentInfo && (!this.HasAgentInfo || this.AgentInfo.Equals(suggestInvitationRequest.AgentInfo)))
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
			hashCode ^= this.ChannelId.GetHashCode();
			hashCode ^= this.TargetId.GetHashCode();
			if (this.HasApprovalId)
			{
				hashCode ^= this.ApprovalId.GetHashCode();
			}
			if (this.HasAgentIdentity)
			{
				hashCode ^= this.AgentIdentity.GetHashCode();
			}
			if (this.HasAgentInfo)
			{
				hashCode ^= this.AgentInfo.GetHashCode();
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
			uint serializedSize1 = this.ChannelId.GetSerializedSize();
			num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			uint num1 = this.TargetId.GetSerializedSize();
			num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
			if (this.HasApprovalId)
			{
				num++;
				uint serializedSize2 = this.ApprovalId.GetSerializedSize();
				num = num + serializedSize2 + ProtocolParser.SizeOfUInt32(serializedSize2);
			}
			if (this.HasAgentIdentity)
			{
				num++;
				uint num2 = this.AgentIdentity.GetSerializedSize();
				num = num + num2 + ProtocolParser.SizeOfUInt32(num2);
			}
			if (this.HasAgentInfo)
			{
				num++;
				uint serializedSize3 = this.AgentInfo.GetSerializedSize();
				num = num + serializedSize3 + ProtocolParser.SizeOfUInt32(serializedSize3);
			}
			num += 2;
			return num;
		}

		public static SuggestInvitationRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<SuggestInvitationRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			SuggestInvitationRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, SuggestInvitationRequest instance)
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
			if (instance.TargetId == null)
			{
				throw new ArgumentNullException("TargetId", "Required by proto specification.");
			}
			stream.WriteByte(26);
			ProtocolParser.WriteUInt32(stream, instance.TargetId.GetSerializedSize());
			EntityId.Serialize(stream, instance.TargetId);
			if (instance.HasApprovalId)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteUInt32(stream, instance.ApprovalId.GetSerializedSize());
				EntityId.Serialize(stream, instance.ApprovalId);
			}
			if (instance.HasAgentIdentity)
			{
				stream.WriteByte(42);
				ProtocolParser.WriteUInt32(stream, instance.AgentIdentity.GetSerializedSize());
				Identity.Serialize(stream, instance.AgentIdentity);
			}
			if (instance.HasAgentInfo)
			{
				stream.WriteByte(50);
				ProtocolParser.WriteUInt32(stream, instance.AgentInfo.GetSerializedSize());
				AccountInfo.Serialize(stream, instance.AgentInfo);
			}
		}

		public void SetAgentId(EntityId val)
		{
			this.AgentId = val;
		}

		public void SetAgentIdentity(Identity val)
		{
			this.AgentIdentity = val;
		}

		public void SetAgentInfo(AccountInfo val)
		{
			this.AgentInfo = val;
		}

		public void SetApprovalId(EntityId val)
		{
			this.ApprovalId = val;
		}

		public void SetChannelId(EntityId val)
		{
			this.ChannelId = val;
		}

		public void SetTargetId(EntityId val)
		{
			this.TargetId = val;
		}
	}
}