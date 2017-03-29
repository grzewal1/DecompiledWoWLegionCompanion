using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.invitation
{
	public class UpdateInvitationRequest : IProtoBuf
	{
		public bool HasAgentIdentity;

		private Identity _AgentIdentity;

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

		public InvitationParams Params
		{
			get;
			set;
		}

		public UpdateInvitationRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			UpdateInvitationRequest.Deserialize(stream, this);
		}

		public static UpdateInvitationRequest Deserialize(Stream stream, UpdateInvitationRequest instance)
		{
			return UpdateInvitationRequest.Deserialize(stream, instance, (long)-1);
		}

		public static UpdateInvitationRequest Deserialize(Stream stream, UpdateInvitationRequest instance, long limit)
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
							if (instance.AgentIdentity != null)
							{
								Identity.DeserializeLengthDelimited(stream, instance.AgentIdentity);
							}
							else
							{
								instance.AgentIdentity = Identity.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 17)
						{
							instance.InvitationId = binaryReader.ReadUInt64();
						}
						else if (num1 != 26)
						{
							Key key = ProtocolParser.ReadKey((byte)num, stream);
							if (key.Field == 0)
							{
								throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
							}
							ProtocolParser.SkipKey(stream, key);
						}
						else if (instance.Params != null)
						{
							InvitationParams.DeserializeLengthDelimited(stream, instance.Params);
						}
						else
						{
							instance.Params = InvitationParams.DeserializeLengthDelimited(stream);
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

		public static UpdateInvitationRequest DeserializeLengthDelimited(Stream stream)
		{
			UpdateInvitationRequest updateInvitationRequest = new UpdateInvitationRequest();
			UpdateInvitationRequest.DeserializeLengthDelimited(stream, updateInvitationRequest);
			return updateInvitationRequest;
		}

		public static UpdateInvitationRequest DeserializeLengthDelimited(Stream stream, UpdateInvitationRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return UpdateInvitationRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			UpdateInvitationRequest updateInvitationRequest = obj as UpdateInvitationRequest;
			if (updateInvitationRequest == null)
			{
				return false;
			}
			if (this.HasAgentIdentity != updateInvitationRequest.HasAgentIdentity || this.HasAgentIdentity && !this.AgentIdentity.Equals(updateInvitationRequest.AgentIdentity))
			{
				return false;
			}
			if (!this.InvitationId.Equals(updateInvitationRequest.InvitationId))
			{
				return false;
			}
			if (!this.Params.Equals(updateInvitationRequest.Params))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasAgentIdentity)
			{
				hashCode = hashCode ^ this.AgentIdentity.GetHashCode();
			}
			hashCode = hashCode ^ this.InvitationId.GetHashCode();
			hashCode = hashCode ^ this.Params.GetHashCode();
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
			num = num + 8;
			uint serializedSize1 = this.Params.GetSerializedSize();
			num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			num = num + 2;
			return num;
		}

		public static UpdateInvitationRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<UpdateInvitationRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			UpdateInvitationRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, UpdateInvitationRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasAgentIdentity)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentIdentity.GetSerializedSize());
				Identity.Serialize(stream, instance.AgentIdentity);
			}
			stream.WriteByte(17);
			binaryWriter.Write(instance.InvitationId);
			if (instance.Params == null)
			{
				throw new ArgumentNullException("Params", "Required by proto specification.");
			}
			stream.WriteByte(26);
			ProtocolParser.WriteUInt32(stream, instance.Params.GetSerializedSize());
			InvitationParams.Serialize(stream, instance.Params);
		}

		public void SetAgentIdentity(Identity val)
		{
			this.AgentIdentity = val;
		}

		public void SetInvitationId(ulong val)
		{
			this.InvitationId = val;
		}

		public void SetParams(InvitationParams val)
		{
			this.Params = val;
		}
	}
}