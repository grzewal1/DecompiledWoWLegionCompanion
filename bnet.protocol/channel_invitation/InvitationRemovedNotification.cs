using bnet.protocol.invitation;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel_invitation
{
	public class InvitationRemovedNotification : IProtoBuf
	{
		public bool HasReason;

		private uint _Reason;

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

		public uint Reason
		{
			get
			{
				return this._Reason;
			}
			set
			{
				this._Reason = value;
				this.HasReason = true;
			}
		}

		public InvitationRemovedNotification()
		{
		}

		public void Deserialize(Stream stream)
		{
			InvitationRemovedNotification.Deserialize(stream, this);
		}

		public static InvitationRemovedNotification Deserialize(Stream stream, InvitationRemovedNotification instance)
		{
			return InvitationRemovedNotification.Deserialize(stream, instance, (long)-1);
		}

		public static InvitationRemovedNotification Deserialize(Stream stream, InvitationRemovedNotification instance, long limit)
		{
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 != 10)
						{
							if (num1 == 16)
							{
								instance.Reason = ProtocolParser.ReadUInt32(stream);
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

		public static InvitationRemovedNotification DeserializeLengthDelimited(Stream stream)
		{
			InvitationRemovedNotification invitationRemovedNotification = new InvitationRemovedNotification();
			InvitationRemovedNotification.DeserializeLengthDelimited(stream, invitationRemovedNotification);
			return invitationRemovedNotification;
		}

		public static InvitationRemovedNotification DeserializeLengthDelimited(Stream stream, InvitationRemovedNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return InvitationRemovedNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			InvitationRemovedNotification invitationRemovedNotification = obj as InvitationRemovedNotification;
			if (invitationRemovedNotification == null)
			{
				return false;
			}
			if (!this.Invitation.Equals(invitationRemovedNotification.Invitation))
			{
				return false;
			}
			if (this.HasReason == invitationRemovedNotification.HasReason && (!this.HasReason || this.Reason.Equals(invitationRemovedNotification.Reason)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Invitation.GetHashCode();
			if (this.HasReason)
			{
				hashCode ^= this.Reason.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.Invitation.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasReason)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.Reason);
			}
			num++;
			return num;
		}

		public static InvitationRemovedNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<InvitationRemovedNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			InvitationRemovedNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, InvitationRemovedNotification instance)
		{
			if (instance.Invitation == null)
			{
				throw new ArgumentNullException("Invitation", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.Invitation.GetSerializedSize());
			bnet.protocol.invitation.Invitation.Serialize(stream, instance.Invitation);
			if (instance.HasReason)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.Reason);
			}
		}

		public void SetInvitation(bnet.protocol.invitation.Invitation val)
		{
			this.Invitation = val;
		}

		public void SetReason(uint val)
		{
			this.Reason = val;
		}
	}
}