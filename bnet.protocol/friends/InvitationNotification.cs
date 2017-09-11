using bnet.protocol;
using bnet.protocol.invitation;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.friends
{
	public class InvitationNotification : IProtoBuf
	{
		public bool HasGameAccountId;

		private EntityId _GameAccountId;

		public bool HasReason;

		private uint _Reason;

		public EntityId GameAccountId
		{
			get
			{
				return this._GameAccountId;
			}
			set
			{
				this._GameAccountId = value;
				this.HasGameAccountId = value != null;
			}
		}

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

		public InvitationNotification()
		{
		}

		public void Deserialize(Stream stream)
		{
			InvitationNotification.Deserialize(stream, this);
		}

		public static InvitationNotification Deserialize(Stream stream, InvitationNotification instance)
		{
			return InvitationNotification.Deserialize(stream, instance, (long)-1);
		}

		public static InvitationNotification Deserialize(Stream stream, InvitationNotification instance, long limit)
		{
			instance.Reason = 0;
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
							if (instance.Invitation != null)
							{
								bnet.protocol.invitation.Invitation.DeserializeLengthDelimited(stream, instance.Invitation);
							}
							else
							{
								instance.Invitation = bnet.protocol.invitation.Invitation.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 != 18)
						{
							if (num1 == 24)
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
						else if (instance.GameAccountId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.GameAccountId);
						}
						else
						{
							instance.GameAccountId = EntityId.DeserializeLengthDelimited(stream);
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

		public static InvitationNotification DeserializeLengthDelimited(Stream stream)
		{
			InvitationNotification invitationNotification = new InvitationNotification();
			InvitationNotification.DeserializeLengthDelimited(stream, invitationNotification);
			return invitationNotification;
		}

		public static InvitationNotification DeserializeLengthDelimited(Stream stream, InvitationNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return InvitationNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			InvitationNotification invitationNotification = obj as InvitationNotification;
			if (invitationNotification == null)
			{
				return false;
			}
			if (!this.Invitation.Equals(invitationNotification.Invitation))
			{
				return false;
			}
			if (this.HasGameAccountId != invitationNotification.HasGameAccountId || this.HasGameAccountId && !this.GameAccountId.Equals(invitationNotification.GameAccountId))
			{
				return false;
			}
			if (this.HasReason == invitationNotification.HasReason && (!this.HasReason || this.Reason.Equals(invitationNotification.Reason)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Invitation.GetHashCode();
			if (this.HasGameAccountId)
			{
				hashCode ^= this.GameAccountId.GetHashCode();
			}
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
			if (this.HasGameAccountId)
			{
				num++;
				uint serializedSize1 = this.GameAccountId.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			if (this.HasReason)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.Reason);
			}
			num++;
			return num;
		}

		public static InvitationNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<InvitationNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			InvitationNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, InvitationNotification instance)
		{
			if (instance.Invitation == null)
			{
				throw new ArgumentNullException("Invitation", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.Invitation.GetSerializedSize());
			bnet.protocol.invitation.Invitation.Serialize(stream, instance.Invitation);
			if (instance.HasGameAccountId)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.GameAccountId.GetSerializedSize());
				EntityId.Serialize(stream, instance.GameAccountId);
			}
			if (instance.HasReason)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.Reason);
			}
		}

		public void SetGameAccountId(EntityId val)
		{
			this.GameAccountId = val;
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