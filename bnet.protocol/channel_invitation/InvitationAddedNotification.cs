using bnet.protocol.invitation;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel_invitation
{
	public class InvitationAddedNotification : IProtoBuf
	{
		public bnet.protocol.invitation.Invitation Invitation
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

		public InvitationAddedNotification()
		{
		}

		public void Deserialize(Stream stream)
		{
			InvitationAddedNotification.Deserialize(stream, this);
		}

		public static InvitationAddedNotification Deserialize(Stream stream, InvitationAddedNotification instance)
		{
			return InvitationAddedNotification.Deserialize(stream, instance, (long)-1);
		}

		public static InvitationAddedNotification Deserialize(Stream stream, InvitationAddedNotification instance, long limit)
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
					else if (num != 10)
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

		public static InvitationAddedNotification DeserializeLengthDelimited(Stream stream)
		{
			InvitationAddedNotification invitationAddedNotification = new InvitationAddedNotification();
			InvitationAddedNotification.DeserializeLengthDelimited(stream, invitationAddedNotification);
			return invitationAddedNotification;
		}

		public static InvitationAddedNotification DeserializeLengthDelimited(Stream stream, InvitationAddedNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return InvitationAddedNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			InvitationAddedNotification invitationAddedNotification = obj as InvitationAddedNotification;
			if (invitationAddedNotification == null)
			{
				return false;
			}
			if (!this.Invitation.Equals(invitationAddedNotification.Invitation))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			return hashCode ^ this.Invitation.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.Invitation.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			return num + 1;
		}

		public static InvitationAddedNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<InvitationAddedNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			InvitationAddedNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, InvitationAddedNotification instance)
		{
			if (instance.Invitation == null)
			{
				throw new ArgumentNullException("Invitation", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.Invitation.GetSerializedSize());
			bnet.protocol.invitation.Invitation.Serialize(stream, instance.Invitation);
		}

		public void SetInvitation(bnet.protocol.invitation.Invitation val)
		{
			this.Invitation = val;
		}
	}
}