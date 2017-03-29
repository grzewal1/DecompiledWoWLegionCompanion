using System;
using System.IO;

namespace bnet.protocol.invitation
{
	public class SendInvitationResponse : IProtoBuf
	{
		public bool HasInvitation;

		private bnet.protocol.invitation.Invitation _Invitation;

		public bnet.protocol.invitation.Invitation Invitation
		{
			get
			{
				return this._Invitation;
			}
			set
			{
				this._Invitation = value;
				this.HasInvitation = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public SendInvitationResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			SendInvitationResponse.Deserialize(stream, this);
		}

		public static SendInvitationResponse Deserialize(Stream stream, SendInvitationResponse instance)
		{
			return SendInvitationResponse.Deserialize(stream, instance, (long)-1);
		}

		public static SendInvitationResponse Deserialize(Stream stream, SendInvitationResponse instance, long limit)
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
					else if (num != 18)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.Invitation != null)
					{
						bnet.protocol.invitation.Invitation.DeserializeLengthDelimited(stream, instance.Invitation);
					}
					else
					{
						instance.Invitation = bnet.protocol.invitation.Invitation.DeserializeLengthDelimited(stream);
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

		public static SendInvitationResponse DeserializeLengthDelimited(Stream stream)
		{
			SendInvitationResponse sendInvitationResponse = new SendInvitationResponse();
			SendInvitationResponse.DeserializeLengthDelimited(stream, sendInvitationResponse);
			return sendInvitationResponse;
		}

		public static SendInvitationResponse DeserializeLengthDelimited(Stream stream, SendInvitationResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return SendInvitationResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			SendInvitationResponse sendInvitationResponse = obj as SendInvitationResponse;
			if (sendInvitationResponse == null)
			{
				return false;
			}
			if (this.HasInvitation == sendInvitationResponse.HasInvitation && (!this.HasInvitation || this.Invitation.Equals(sendInvitationResponse.Invitation)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasInvitation)
			{
				hashCode = hashCode ^ this.Invitation.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasInvitation)
			{
				num++;
				uint serializedSize = this.Invitation.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			return num;
		}

		public static SendInvitationResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<SendInvitationResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			SendInvitationResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, SendInvitationResponse instance)
		{
			if (instance.HasInvitation)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.Invitation.GetSerializedSize());
				bnet.protocol.invitation.Invitation.Serialize(stream, instance.Invitation);
			}
		}

		public void SetInvitation(bnet.protocol.invitation.Invitation val)
		{
			this.Invitation = val;
		}
	}
}