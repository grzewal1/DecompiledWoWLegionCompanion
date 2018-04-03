using bnet.protocol;
using bnet.protocol.channel_invitation;
using bnet.protocol.friends;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.invitation
{
	public class Invitation : IProtoBuf
	{
		public bool HasInviterName;

		private string _InviterName;

		public bool HasInviteeName;

		private string _InviteeName;

		public bool HasInvitationMessage;

		private string _InvitationMessage;

		public bool HasCreationTime;

		private ulong _CreationTime;

		public bool HasExpirationTime;

		private ulong _ExpirationTime;

		public bool HasChannelInvitation;

		private bnet.protocol.channel_invitation.ChannelInvitation _ChannelInvitation;

		public bool HasFriendInvite;

		private FriendInvitation _FriendInvite;

		public bnet.protocol.channel_invitation.ChannelInvitation ChannelInvitation
		{
			get
			{
				return this._ChannelInvitation;
			}
			set
			{
				this._ChannelInvitation = value;
				this.HasChannelInvitation = value != null;
			}
		}

		public ulong CreationTime
		{
			get
			{
				return this._CreationTime;
			}
			set
			{
				this._CreationTime = value;
				this.HasCreationTime = true;
			}
		}

		public ulong ExpirationTime
		{
			get
			{
				return this._ExpirationTime;
			}
			set
			{
				this._ExpirationTime = value;
				this.HasExpirationTime = true;
			}
		}

		public FriendInvitation FriendInvite
		{
			get
			{
				return this._FriendInvite;
			}
			set
			{
				this._FriendInvite = value;
				this.HasFriendInvite = value != null;
			}
		}

		public ulong Id
		{
			get;
			set;
		}

		public string InvitationMessage
		{
			get
			{
				return this._InvitationMessage;
			}
			set
			{
				this._InvitationMessage = value;
				this.HasInvitationMessage = value != null;
			}
		}

		public Identity InviteeIdentity
		{
			get;
			set;
		}

		public string InviteeName
		{
			get
			{
				return this._InviteeName;
			}
			set
			{
				this._InviteeName = value;
				this.HasInviteeName = value != null;
			}
		}

		public Identity InviterIdentity
		{
			get;
			set;
		}

		public string InviterName
		{
			get
			{
				return this._InviterName;
			}
			set
			{
				this._InviterName = value;
				this.HasInviterName = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public Invitation()
		{
		}

		public void Deserialize(Stream stream)
		{
			Invitation.Deserialize(stream, this);
		}

		public static Invitation Deserialize(Stream stream, Invitation instance)
		{
			return Invitation.Deserialize(stream, instance, (long)-1);
		}

		public static Invitation Deserialize(Stream stream, Invitation instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
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
					else if (num == 9)
					{
						instance.Id = binaryReader.ReadUInt64();
					}
					else if (num == 18)
					{
						if (instance.InviterIdentity != null)
						{
							Identity.DeserializeLengthDelimited(stream, instance.InviterIdentity);
						}
						else
						{
							instance.InviterIdentity = Identity.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 26)
					{
						if (instance.InviteeIdentity != null)
						{
							Identity.DeserializeLengthDelimited(stream, instance.InviteeIdentity);
						}
						else
						{
							instance.InviteeIdentity = Identity.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 34)
					{
						instance.InviterName = ProtocolParser.ReadString(stream);
					}
					else if (num == 42)
					{
						instance.InviteeName = ProtocolParser.ReadString(stream);
					}
					else if (num == 50)
					{
						instance.InvitationMessage = ProtocolParser.ReadString(stream);
					}
					else if (num == 56)
					{
						instance.CreationTime = ProtocolParser.ReadUInt64(stream);
					}
					else if (num == 64)
					{
						instance.ExpirationTime = ProtocolParser.ReadUInt64(stream);
					}
					else
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						uint field = key.Field;
						switch (field)
						{
							case 103:
							{
								if (key.WireType == Wire.LengthDelimited)
								{
									if (instance.FriendInvite != null)
									{
										FriendInvitation.DeserializeLengthDelimited(stream, instance.FriendInvite);
									}
									else
									{
										instance.FriendInvite = FriendInvitation.DeserializeLengthDelimited(stream);
									}
									continue;
								}
								else
								{
									break;
								}
							}
							case 105:
							{
								if (key.WireType == Wire.LengthDelimited)
								{
									if (instance.ChannelInvitation != null)
									{
										bnet.protocol.channel_invitation.ChannelInvitation.DeserializeLengthDelimited(stream, instance.ChannelInvitation);
									}
									else
									{
										instance.ChannelInvitation = bnet.protocol.channel_invitation.ChannelInvitation.DeserializeLengthDelimited(stream);
									}
									continue;
								}
								else
								{
									break;
								}
							}
							default:
							{
								if (field == 0)
								{
									throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
								}
								ProtocolParser.SkipKey(stream, key);
								break;
							}
						}
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

		public static Invitation DeserializeLengthDelimited(Stream stream)
		{
			Invitation invitation = new Invitation();
			Invitation.DeserializeLengthDelimited(stream, invitation);
			return invitation;
		}

		public static Invitation DeserializeLengthDelimited(Stream stream, Invitation instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return Invitation.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			Invitation invitation = obj as Invitation;
			if (invitation == null)
			{
				return false;
			}
			if (!this.Id.Equals(invitation.Id))
			{
				return false;
			}
			if (!this.InviterIdentity.Equals(invitation.InviterIdentity))
			{
				return false;
			}
			if (!this.InviteeIdentity.Equals(invitation.InviteeIdentity))
			{
				return false;
			}
			if (this.HasInviterName != invitation.HasInviterName || this.HasInviterName && !this.InviterName.Equals(invitation.InviterName))
			{
				return false;
			}
			if (this.HasInviteeName != invitation.HasInviteeName || this.HasInviteeName && !this.InviteeName.Equals(invitation.InviteeName))
			{
				return false;
			}
			if (this.HasInvitationMessage != invitation.HasInvitationMessage || this.HasInvitationMessage && !this.InvitationMessage.Equals(invitation.InvitationMessage))
			{
				return false;
			}
			if (this.HasCreationTime != invitation.HasCreationTime || this.HasCreationTime && !this.CreationTime.Equals(invitation.CreationTime))
			{
				return false;
			}
			if (this.HasExpirationTime != invitation.HasExpirationTime || this.HasExpirationTime && !this.ExpirationTime.Equals(invitation.ExpirationTime))
			{
				return false;
			}
			if (this.HasChannelInvitation != invitation.HasChannelInvitation || this.HasChannelInvitation && !this.ChannelInvitation.Equals(invitation.ChannelInvitation))
			{
				return false;
			}
			if (this.HasFriendInvite == invitation.HasFriendInvite && (!this.HasFriendInvite || this.FriendInvite.Equals(invitation.FriendInvite)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Id.GetHashCode();
			hashCode ^= this.InviterIdentity.GetHashCode();
			hashCode ^= this.InviteeIdentity.GetHashCode();
			if (this.HasInviterName)
			{
				hashCode ^= this.InviterName.GetHashCode();
			}
			if (this.HasInviteeName)
			{
				hashCode ^= this.InviteeName.GetHashCode();
			}
			if (this.HasInvitationMessage)
			{
				hashCode ^= this.InvitationMessage.GetHashCode();
			}
			if (this.HasCreationTime)
			{
				hashCode ^= this.CreationTime.GetHashCode();
			}
			if (this.HasExpirationTime)
			{
				hashCode ^= this.ExpirationTime.GetHashCode();
			}
			if (this.HasChannelInvitation)
			{
				hashCode ^= this.ChannelInvitation.GetHashCode();
			}
			if (this.HasFriendInvite)
			{
				hashCode ^= this.FriendInvite.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num += 8;
			uint serializedSize = this.InviterIdentity.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			uint serializedSize1 = this.InviteeIdentity.GetSerializedSize();
			num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			if (this.HasInviterName)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.InviterName);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasInviteeName)
			{
				num++;
				uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.InviteeName);
				num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			}
			if (this.HasInvitationMessage)
			{
				num++;
				uint num1 = (uint)Encoding.UTF8.GetByteCount(this.InvitationMessage);
				num = num + ProtocolParser.SizeOfUInt32(num1) + num1;
			}
			if (this.HasCreationTime)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.CreationTime);
			}
			if (this.HasExpirationTime)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.ExpirationTime);
			}
			if (this.HasChannelInvitation)
			{
				num += 2;
				uint serializedSize2 = this.ChannelInvitation.GetSerializedSize();
				num = num + serializedSize2 + ProtocolParser.SizeOfUInt32(serializedSize2);
			}
			if (this.HasFriendInvite)
			{
				num += 2;
				uint num2 = this.FriendInvite.GetSerializedSize();
				num = num + num2 + ProtocolParser.SizeOfUInt32(num2);
			}
			num += 3;
			return num;
		}

		public static Invitation ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<Invitation>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			Invitation.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, Invitation instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			stream.WriteByte(9);
			binaryWriter.Write(instance.Id);
			if (instance.InviterIdentity == null)
			{
				throw new ArgumentNullException("InviterIdentity", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.InviterIdentity.GetSerializedSize());
			Identity.Serialize(stream, instance.InviterIdentity);
			if (instance.InviteeIdentity == null)
			{
				throw new ArgumentNullException("InviteeIdentity", "Required by proto specification.");
			}
			stream.WriteByte(26);
			ProtocolParser.WriteUInt32(stream, instance.InviteeIdentity.GetSerializedSize());
			Identity.Serialize(stream, instance.InviteeIdentity);
			if (instance.HasInviterName)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.InviterName));
			}
			if (instance.HasInviteeName)
			{
				stream.WriteByte(42);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.InviteeName));
			}
			if (instance.HasInvitationMessage)
			{
				stream.WriteByte(50);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.InvitationMessage));
			}
			if (instance.HasCreationTime)
			{
				stream.WriteByte(56);
				ProtocolParser.WriteUInt64(stream, instance.CreationTime);
			}
			if (instance.HasExpirationTime)
			{
				stream.WriteByte(64);
				ProtocolParser.WriteUInt64(stream, instance.ExpirationTime);
			}
			if (instance.HasChannelInvitation)
			{
				stream.WriteByte(202);
				stream.WriteByte(6);
				ProtocolParser.WriteUInt32(stream, instance.ChannelInvitation.GetSerializedSize());
				bnet.protocol.channel_invitation.ChannelInvitation.Serialize(stream, instance.ChannelInvitation);
			}
			if (instance.HasFriendInvite)
			{
				stream.WriteByte(186);
				stream.WriteByte(6);
				ProtocolParser.WriteUInt32(stream, instance.FriendInvite.GetSerializedSize());
				FriendInvitation.Serialize(stream, instance.FriendInvite);
			}
		}

		public void SetChannelInvitation(bnet.protocol.channel_invitation.ChannelInvitation val)
		{
			this.ChannelInvitation = val;
		}

		public void SetCreationTime(ulong val)
		{
			this.CreationTime = val;
		}

		public void SetExpirationTime(ulong val)
		{
			this.ExpirationTime = val;
		}

		public void SetFriendInvite(FriendInvitation val)
		{
			this.FriendInvite = val;
		}

		public void SetId(ulong val)
		{
			this.Id = val;
		}

		public void SetInvitationMessage(string val)
		{
			this.InvitationMessage = val;
		}

		public void SetInviteeIdentity(Identity val)
		{
			this.InviteeIdentity = val;
		}

		public void SetInviteeName(string val)
		{
			this.InviteeName = val;
		}

		public void SetInviterIdentity(Identity val)
		{
			this.InviterIdentity = val;
		}

		public void SetInviterName(string val)
		{
			this.InviterName = val;
		}
	}
}