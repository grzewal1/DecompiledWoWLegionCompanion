using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.invitation
{
	public class SendInvitationRequest : IProtoBuf
	{
		public bool HasAgentIdentity;

		private Identity _AgentIdentity;

		public bool HasAgentInfo;

		private AccountInfo _AgentInfo;

		public bool HasTarget;

		private InvitationTarget _Target;

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

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public InvitationParams Params
		{
			get;
			set;
		}

		public InvitationTarget Target
		{
			get
			{
				return this._Target;
			}
			set
			{
				this._Target = value;
				this.HasTarget = value != null;
			}
		}

		public EntityId TargetId
		{
			get;
			set;
		}

		public SendInvitationRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			SendInvitationRequest.Deserialize(stream, this);
		}

		public static SendInvitationRequest Deserialize(Stream stream, SendInvitationRequest instance)
		{
			return SendInvitationRequest.Deserialize(stream, instance, (long)-1);
		}

		public static SendInvitationRequest Deserialize(Stream stream, SendInvitationRequest instance, long limit)
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
							if (instance.AgentIdentity != null)
							{
								Identity.DeserializeLengthDelimited(stream, instance.AgentIdentity);
							}
							else
							{
								instance.AgentIdentity = Identity.DeserializeLengthDelimited(stream);
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
						else if (num1 == 26)
						{
							if (instance.Params != null)
							{
								InvitationParams.DeserializeLengthDelimited(stream, instance.Params);
							}
							else
							{
								instance.Params = InvitationParams.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 34)
						{
							if (instance.AgentInfo != null)
							{
								AccountInfo.DeserializeLengthDelimited(stream, instance.AgentInfo);
							}
							else
							{
								instance.AgentInfo = AccountInfo.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 != 42)
						{
							Key key = ProtocolParser.ReadKey((byte)num, stream);
							if (key.Field == 0)
							{
								throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
							}
							ProtocolParser.SkipKey(stream, key);
						}
						else if (instance.Target != null)
						{
							InvitationTarget.DeserializeLengthDelimited(stream, instance.Target);
						}
						else
						{
							instance.Target = InvitationTarget.DeserializeLengthDelimited(stream);
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

		public static SendInvitationRequest DeserializeLengthDelimited(Stream stream)
		{
			SendInvitationRequest sendInvitationRequest = new SendInvitationRequest();
			SendInvitationRequest.DeserializeLengthDelimited(stream, sendInvitationRequest);
			return sendInvitationRequest;
		}

		public static SendInvitationRequest DeserializeLengthDelimited(Stream stream, SendInvitationRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return SendInvitationRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			SendInvitationRequest sendInvitationRequest = obj as SendInvitationRequest;
			if (sendInvitationRequest == null)
			{
				return false;
			}
			if (this.HasAgentIdentity != sendInvitationRequest.HasAgentIdentity || this.HasAgentIdentity && !this.AgentIdentity.Equals(sendInvitationRequest.AgentIdentity))
			{
				return false;
			}
			if (!this.TargetId.Equals(sendInvitationRequest.TargetId))
			{
				return false;
			}
			if (!this.Params.Equals(sendInvitationRequest.Params))
			{
				return false;
			}
			if (this.HasAgentInfo != sendInvitationRequest.HasAgentInfo || this.HasAgentInfo && !this.AgentInfo.Equals(sendInvitationRequest.AgentInfo))
			{
				return false;
			}
			if (this.HasTarget == sendInvitationRequest.HasTarget && (!this.HasTarget || this.Target.Equals(sendInvitationRequest.Target)))
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
				hashCode = hashCode ^ this.AgentIdentity.GetHashCode();
			}
			hashCode = hashCode ^ this.TargetId.GetHashCode();
			hashCode = hashCode ^ this.Params.GetHashCode();
			if (this.HasAgentInfo)
			{
				hashCode = hashCode ^ this.AgentInfo.GetHashCode();
			}
			if (this.HasTarget)
			{
				hashCode = hashCode ^ this.Target.GetHashCode();
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
			uint serializedSize1 = this.TargetId.GetSerializedSize();
			num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			uint num1 = this.Params.GetSerializedSize();
			num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
			if (this.HasAgentInfo)
			{
				num++;
				uint serializedSize2 = this.AgentInfo.GetSerializedSize();
				num = num + serializedSize2 + ProtocolParser.SizeOfUInt32(serializedSize2);
			}
			if (this.HasTarget)
			{
				num++;
				uint num2 = this.Target.GetSerializedSize();
				num = num + num2 + ProtocolParser.SizeOfUInt32(num2);
			}
			num = num + 2;
			return num;
		}

		public static SendInvitationRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<SendInvitationRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			SendInvitationRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, SendInvitationRequest instance)
		{
			if (instance.HasAgentIdentity)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentIdentity.GetSerializedSize());
				Identity.Serialize(stream, instance.AgentIdentity);
			}
			if (instance.TargetId == null)
			{
				throw new ArgumentNullException("TargetId", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.TargetId.GetSerializedSize());
			EntityId.Serialize(stream, instance.TargetId);
			if (instance.Params == null)
			{
				throw new ArgumentNullException("Params", "Required by proto specification.");
			}
			stream.WriteByte(26);
			ProtocolParser.WriteUInt32(stream, instance.Params.GetSerializedSize());
			InvitationParams.Serialize(stream, instance.Params);
			if (instance.HasAgentInfo)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteUInt32(stream, instance.AgentInfo.GetSerializedSize());
				AccountInfo.Serialize(stream, instance.AgentInfo);
			}
			if (instance.HasTarget)
			{
				stream.WriteByte(42);
				ProtocolParser.WriteUInt32(stream, instance.Target.GetSerializedSize());
				InvitationTarget.Serialize(stream, instance.Target);
			}
		}

		public void SetAgentIdentity(Identity val)
		{
			this.AgentIdentity = val;
		}

		public void SetAgentInfo(AccountInfo val)
		{
			this.AgentInfo = val;
		}

		public void SetParams(InvitationParams val)
		{
			this.Params = val;
		}

		public void SetTarget(InvitationTarget val)
		{
			this.Target = val;
		}

		public void SetTargetId(EntityId val)
		{
			this.TargetId = val;
		}
	}
}