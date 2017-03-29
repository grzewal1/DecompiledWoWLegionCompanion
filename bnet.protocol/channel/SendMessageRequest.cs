using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel
{
	public class SendMessageRequest : IProtoBuf
	{
		public bool HasAgentId;

		private EntityId _AgentId;

		public bool HasRequiredPrivileges;

		private ulong _RequiredPrivileges;

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

		public bnet.protocol.channel.Message Message
		{
			get;
			set;
		}

		public ulong RequiredPrivileges
		{
			get
			{
				return this._RequiredPrivileges;
			}
			set
			{
				this._RequiredPrivileges = value;
				this.HasRequiredPrivileges = true;
			}
		}

		public SendMessageRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			SendMessageRequest.Deserialize(stream, this);
		}

		public static SendMessageRequest Deserialize(Stream stream, SendMessageRequest instance)
		{
			return SendMessageRequest.Deserialize(stream, instance, (long)-1);
		}

		public static SendMessageRequest Deserialize(Stream stream, SendMessageRequest instance, long limit)
		{
			instance.RequiredPrivileges = (ulong)0;
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
							if (num1 == 24)
							{
								instance.RequiredPrivileges = ProtocolParser.ReadUInt64(stream);
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
						else if (instance.Message != null)
						{
							bnet.protocol.channel.Message.DeserializeLengthDelimited(stream, instance.Message);
						}
						else
						{
							instance.Message = bnet.protocol.channel.Message.DeserializeLengthDelimited(stream);
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

		public static SendMessageRequest DeserializeLengthDelimited(Stream stream)
		{
			SendMessageRequest sendMessageRequest = new SendMessageRequest();
			SendMessageRequest.DeserializeLengthDelimited(stream, sendMessageRequest);
			return sendMessageRequest;
		}

		public static SendMessageRequest DeserializeLengthDelimited(Stream stream, SendMessageRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return SendMessageRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			SendMessageRequest sendMessageRequest = obj as SendMessageRequest;
			if (sendMessageRequest == null)
			{
				return false;
			}
			if (this.HasAgentId != sendMessageRequest.HasAgentId || this.HasAgentId && !this.AgentId.Equals(sendMessageRequest.AgentId))
			{
				return false;
			}
			if (!this.Message.Equals(sendMessageRequest.Message))
			{
				return false;
			}
			if (this.HasRequiredPrivileges == sendMessageRequest.HasRequiredPrivileges && (!this.HasRequiredPrivileges || this.RequiredPrivileges.Equals(sendMessageRequest.RequiredPrivileges)))
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
			hashCode = hashCode ^ this.Message.GetHashCode();
			if (this.HasRequiredPrivileges)
			{
				hashCode = hashCode ^ this.RequiredPrivileges.GetHashCode();
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
			uint serializedSize1 = this.Message.GetSerializedSize();
			num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			if (this.HasRequiredPrivileges)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt64(this.RequiredPrivileges);
			}
			num++;
			return num;
		}

		public static SendMessageRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<SendMessageRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			SendMessageRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, SendMessageRequest instance)
		{
			if (instance.HasAgentId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentId.GetSerializedSize());
				EntityId.Serialize(stream, instance.AgentId);
			}
			if (instance.Message == null)
			{
				throw new ArgumentNullException("Message", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.Message.GetSerializedSize());
			bnet.protocol.channel.Message.Serialize(stream, instance.Message);
			if (instance.HasRequiredPrivileges)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt64(stream, instance.RequiredPrivileges);
			}
		}

		public void SetAgentId(EntityId val)
		{
			this.AgentId = val;
		}

		public void SetMessage(bnet.protocol.channel.Message val)
		{
			this.Message = val;
		}

		public void SetRequiredPrivileges(ulong val)
		{
			this.RequiredPrivileges = val;
		}
	}
}