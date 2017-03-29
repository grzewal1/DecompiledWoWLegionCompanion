using bnet.protocol.attribute;
using bnet.protocol.chat;
using bnet.protocol.invitation;
using bnet.protocol.presence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace bnet.protocol.channel
{
	public class ChannelState : IProtoBuf
	{
		public bool HasMaxMembers;

		private uint _MaxMembers;

		public bool HasMinMembers;

		private uint _MinMembers;

		private List<bnet.protocol.attribute.Attribute> _Attribute = new List<bnet.protocol.attribute.Attribute>();

		private List<bnet.protocol.invitation.Invitation> _Invitation = new List<bnet.protocol.invitation.Invitation>();

		public bool HasMaxInvitations;

		private uint _MaxInvitations;

		public bool HasReason;

		private uint _Reason;

		public bool HasPrivacyLevel;

		private bnet.protocol.channel.ChannelState.Types.PrivacyLevel _PrivacyLevel;

		public bool HasName;

		private string _Name;

		public bool HasDelegateName;

		private string _DelegateName;

		public bool HasChannelType;

		private string _ChannelType;

		public bool HasProgram;

		private uint _Program;

		public bool HasAllowOfflineMembers;

		private bool _AllowOfflineMembers;

		public bool HasSubscribeToPresence;

		private bool _SubscribeToPresence;

		public bool HasChat;

		private bnet.protocol.chat.ChannelState _Chat;

		public bool HasPresence;

		private bnet.protocol.presence.ChannelState _Presence;

		public bool AllowOfflineMembers
		{
			get
			{
				return this._AllowOfflineMembers;
			}
			set
			{
				this._AllowOfflineMembers = value;
				this.HasAllowOfflineMembers = true;
			}
		}

		public List<bnet.protocol.attribute.Attribute> Attribute
		{
			get
			{
				return this._Attribute;
			}
			set
			{
				this._Attribute = value;
			}
		}

		public int AttributeCount
		{
			get
			{
				return this._Attribute.Count;
			}
		}

		public List<bnet.protocol.attribute.Attribute> AttributeList
		{
			get
			{
				return this._Attribute;
			}
		}

		public string ChannelType
		{
			get
			{
				return this._ChannelType;
			}
			set
			{
				this._ChannelType = value;
				this.HasChannelType = value != null;
			}
		}

		public bnet.protocol.chat.ChannelState Chat
		{
			get
			{
				return this._Chat;
			}
			set
			{
				this._Chat = value;
				this.HasChat = value != null;
			}
		}

		public string DelegateName
		{
			get
			{
				return this._DelegateName;
			}
			set
			{
				this._DelegateName = value;
				this.HasDelegateName = value != null;
			}
		}

		public List<bnet.protocol.invitation.Invitation> Invitation
		{
			get
			{
				return this._Invitation;
			}
			set
			{
				this._Invitation = value;
			}
		}

		public int InvitationCount
		{
			get
			{
				return this._Invitation.Count;
			}
		}

		public List<bnet.protocol.invitation.Invitation> InvitationList
		{
			get
			{
				return this._Invitation;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public uint MaxInvitations
		{
			get
			{
				return this._MaxInvitations;
			}
			set
			{
				this._MaxInvitations = value;
				this.HasMaxInvitations = true;
			}
		}

		public uint MaxMembers
		{
			get
			{
				return this._MaxMembers;
			}
			set
			{
				this._MaxMembers = value;
				this.HasMaxMembers = true;
			}
		}

		public uint MinMembers
		{
			get
			{
				return this._MinMembers;
			}
			set
			{
				this._MinMembers = value;
				this.HasMinMembers = true;
			}
		}

		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				this._Name = value;
				this.HasName = value != null;
			}
		}

		public bnet.protocol.presence.ChannelState Presence
		{
			get
			{
				return this._Presence;
			}
			set
			{
				this._Presence = value;
				this.HasPresence = value != null;
			}
		}

		public bnet.protocol.channel.ChannelState.Types.PrivacyLevel PrivacyLevel
		{
			get
			{
				return this._PrivacyLevel;
			}
			set
			{
				this._PrivacyLevel = value;
				this.HasPrivacyLevel = true;
			}
		}

		public uint Program
		{
			get
			{
				return this._Program;
			}
			set
			{
				this._Program = value;
				this.HasProgram = true;
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

		public bool SubscribeToPresence
		{
			get
			{
				return this._SubscribeToPresence;
			}
			set
			{
				this._SubscribeToPresence = value;
				this.HasSubscribeToPresence = true;
			}
		}

		public ChannelState()
		{
		}

		public void AddAttribute(bnet.protocol.attribute.Attribute val)
		{
			this._Attribute.Add(val);
		}

		public void AddInvitation(bnet.protocol.invitation.Invitation val)
		{
			this._Invitation.Add(val);
		}

		public void ClearAttribute()
		{
			this._Attribute.Clear();
		}

		public void ClearInvitation()
		{
			this._Invitation.Clear();
		}

		public void Deserialize(Stream stream)
		{
			bnet.protocol.channel.ChannelState.Deserialize(stream, this);
		}

		public static bnet.protocol.channel.ChannelState Deserialize(Stream stream, bnet.protocol.channel.ChannelState instance)
		{
			return bnet.protocol.channel.ChannelState.Deserialize(stream, instance, (long)-1);
		}

		public static bnet.protocol.channel.ChannelState Deserialize(Stream stream, bnet.protocol.channel.ChannelState instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			if (instance.Attribute == null)
			{
				instance.Attribute = new List<bnet.protocol.attribute.Attribute>();
			}
			if (instance.Invitation == null)
			{
				instance.Invitation = new List<bnet.protocol.invitation.Invitation>();
			}
			instance.PrivacyLevel = bnet.protocol.channel.ChannelState.Types.PrivacyLevel.PRIVACY_LEVEL_OPEN;
			instance.ChannelType = "default";
			instance.Program = 0;
			instance.AllowOfflineMembers = false;
			instance.SubscribeToPresence = true;
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						switch (num1)
						{
							case 93:
							{
								instance.Program = binaryReader.ReadUInt32();
								continue;
							}
							case 96:
							{
								instance.AllowOfflineMembers = ProtocolParser.ReadBool(stream);
								continue;
							}
							default:
							{
								if (num1 == 8)
								{
									instance.MaxMembers = ProtocolParser.ReadUInt32(stream);
									continue;
								}
								else if (num1 == 16)
								{
									instance.MinMembers = ProtocolParser.ReadUInt32(stream);
									continue;
								}
								else if (num1 == 26)
								{
									instance.Attribute.Add(bnet.protocol.attribute.Attribute.DeserializeLengthDelimited(stream));
									continue;
								}
								else if (num1 == 34)
								{
									instance.Invitation.Add(bnet.protocol.invitation.Invitation.DeserializeLengthDelimited(stream));
									continue;
								}
								else if (num1 == 40)
								{
									instance.MaxInvitations = ProtocolParser.ReadUInt32(stream);
									continue;
								}
								else if (num1 == 48)
								{
									instance.Reason = ProtocolParser.ReadUInt32(stream);
									continue;
								}
								else if (num1 == 56)
								{
									instance.PrivacyLevel = (bnet.protocol.channel.ChannelState.Types.PrivacyLevel)((int)ProtocolParser.ReadUInt64(stream));
									continue;
								}
								else if (num1 == 66)
								{
									instance.Name = ProtocolParser.ReadString(stream);
									continue;
								}
								else if (num1 == 74)
								{
									instance.DelegateName = ProtocolParser.ReadString(stream);
									continue;
								}
								else if (num1 == 82)
								{
									instance.ChannelType = ProtocolParser.ReadString(stream);
									continue;
								}
								else if (num1 == 104)
								{
									instance.SubscribeToPresence = ProtocolParser.ReadBool(stream);
									continue;
								}
								else
								{
									Key key = ProtocolParser.ReadKey((byte)num, stream);
									uint field = key.Field;
									if (field == 100)
									{
										if (key.WireType == Wire.LengthDelimited)
										{
											if (instance.Chat != null)
											{
												bnet.protocol.chat.ChannelState.DeserializeLengthDelimited(stream, instance.Chat);
											}
											else
											{
												instance.Chat = bnet.protocol.chat.ChannelState.DeserializeLengthDelimited(stream);
											}
											continue;
										}
									}
									else if (field != 101)
									{
										if (field == 0)
										{
											throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
										}
										ProtocolParser.SkipKey(stream, key);
									}
									else if (key.WireType == Wire.LengthDelimited)
									{
										if (instance.Presence != null)
										{
											bnet.protocol.presence.ChannelState.DeserializeLengthDelimited(stream, instance.Presence);
										}
										else
										{
											instance.Presence = bnet.protocol.presence.ChannelState.DeserializeLengthDelimited(stream);
										}
										continue;
									}
									continue;
								}
							}
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

		public static bnet.protocol.channel.ChannelState DeserializeLengthDelimited(Stream stream)
		{
			bnet.protocol.channel.ChannelState channelState = new bnet.protocol.channel.ChannelState();
			bnet.protocol.channel.ChannelState.DeserializeLengthDelimited(stream, channelState);
			return channelState;
		}

		public static bnet.protocol.channel.ChannelState DeserializeLengthDelimited(Stream stream, bnet.protocol.channel.ChannelState instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return bnet.protocol.channel.ChannelState.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			bnet.protocol.channel.ChannelState channelState = obj as bnet.protocol.channel.ChannelState;
			if (channelState == null)
			{
				return false;
			}
			if (this.HasMaxMembers != channelState.HasMaxMembers || this.HasMaxMembers && !this.MaxMembers.Equals(channelState.MaxMembers))
			{
				return false;
			}
			if (this.HasMinMembers != channelState.HasMinMembers || this.HasMinMembers && !this.MinMembers.Equals(channelState.MinMembers))
			{
				return false;
			}
			if (this.Attribute.Count != channelState.Attribute.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Attribute.Count; i++)
			{
				if (!this.Attribute[i].Equals(channelState.Attribute[i]))
				{
					return false;
				}
			}
			if (this.Invitation.Count != channelState.Invitation.Count)
			{
				return false;
			}
			for (int j = 0; j < this.Invitation.Count; j++)
			{
				if (!this.Invitation[j].Equals(channelState.Invitation[j]))
				{
					return false;
				}
			}
			if (this.HasMaxInvitations != channelState.HasMaxInvitations || this.HasMaxInvitations && !this.MaxInvitations.Equals(channelState.MaxInvitations))
			{
				return false;
			}
			if (this.HasReason != channelState.HasReason || this.HasReason && !this.Reason.Equals(channelState.Reason))
			{
				return false;
			}
			if (this.HasPrivacyLevel != channelState.HasPrivacyLevel || this.HasPrivacyLevel && !this.PrivacyLevel.Equals(channelState.PrivacyLevel))
			{
				return false;
			}
			if (this.HasName != channelState.HasName || this.HasName && !this.Name.Equals(channelState.Name))
			{
				return false;
			}
			if (this.HasDelegateName != channelState.HasDelegateName || this.HasDelegateName && !this.DelegateName.Equals(channelState.DelegateName))
			{
				return false;
			}
			if (this.HasChannelType != channelState.HasChannelType || this.HasChannelType && !this.ChannelType.Equals(channelState.ChannelType))
			{
				return false;
			}
			if (this.HasProgram != channelState.HasProgram || this.HasProgram && !this.Program.Equals(channelState.Program))
			{
				return false;
			}
			if (this.HasAllowOfflineMembers != channelState.HasAllowOfflineMembers || this.HasAllowOfflineMembers && !this.AllowOfflineMembers.Equals(channelState.AllowOfflineMembers))
			{
				return false;
			}
			if (this.HasSubscribeToPresence != channelState.HasSubscribeToPresence || this.HasSubscribeToPresence && !this.SubscribeToPresence.Equals(channelState.SubscribeToPresence))
			{
				return false;
			}
			if (this.HasChat != channelState.HasChat || this.HasChat && !this.Chat.Equals(channelState.Chat))
			{
				return false;
			}
			if (this.HasPresence == channelState.HasPresence && (!this.HasPresence || this.Presence.Equals(channelState.Presence)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasMaxMembers)
			{
				hashCode = hashCode ^ this.MaxMembers.GetHashCode();
			}
			if (this.HasMinMembers)
			{
				hashCode = hashCode ^ this.MinMembers.GetHashCode();
			}
			foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
			{
				hashCode = hashCode ^ attribute.GetHashCode();
			}
			foreach (bnet.protocol.invitation.Invitation invitation in this.Invitation)
			{
				hashCode = hashCode ^ invitation.GetHashCode();
			}
			if (this.HasMaxInvitations)
			{
				hashCode = hashCode ^ this.MaxInvitations.GetHashCode();
			}
			if (this.HasReason)
			{
				hashCode = hashCode ^ this.Reason.GetHashCode();
			}
			if (this.HasPrivacyLevel)
			{
				hashCode = hashCode ^ this.PrivacyLevel.GetHashCode();
			}
			if (this.HasName)
			{
				hashCode = hashCode ^ this.Name.GetHashCode();
			}
			if (this.HasDelegateName)
			{
				hashCode = hashCode ^ this.DelegateName.GetHashCode();
			}
			if (this.HasChannelType)
			{
				hashCode = hashCode ^ this.ChannelType.GetHashCode();
			}
			if (this.HasProgram)
			{
				hashCode = hashCode ^ this.Program.GetHashCode();
			}
			if (this.HasAllowOfflineMembers)
			{
				hashCode = hashCode ^ this.AllowOfflineMembers.GetHashCode();
			}
			if (this.HasSubscribeToPresence)
			{
				hashCode = hashCode ^ this.SubscribeToPresence.GetHashCode();
			}
			if (this.HasChat)
			{
				hashCode = hashCode ^ this.Chat.GetHashCode();
			}
			if (this.HasPresence)
			{
				hashCode = hashCode ^ this.Presence.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasMaxMembers)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.MaxMembers);
			}
			if (this.HasMinMembers)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.MinMembers);
			}
			if (this.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
				{
					num++;
					uint serializedSize = attribute.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			if (this.Invitation.Count > 0)
			{
				foreach (bnet.protocol.invitation.Invitation invitation in this.Invitation)
				{
					num++;
					uint serializedSize1 = invitation.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			if (this.HasMaxInvitations)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.MaxInvitations);
			}
			if (this.HasReason)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.Reason);
			}
			if (this.HasPrivacyLevel)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt64((ulong)this.PrivacyLevel);
			}
			if (this.HasName)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Name);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasDelegateName)
			{
				num++;
				uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.DelegateName);
				num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			}
			if (this.HasChannelType)
			{
				num++;
				uint num1 = (uint)Encoding.UTF8.GetByteCount(this.ChannelType);
				num = num + ProtocolParser.SizeOfUInt32(num1) + num1;
			}
			if (this.HasProgram)
			{
				num++;
				num = num + 4;
			}
			if (this.HasAllowOfflineMembers)
			{
				num++;
				num++;
			}
			if (this.HasSubscribeToPresence)
			{
				num++;
				num++;
			}
			if (this.HasChat)
			{
				num = num + 2;
				uint serializedSize2 = this.Chat.GetSerializedSize();
				num = num + serializedSize2 + ProtocolParser.SizeOfUInt32(serializedSize2);
			}
			if (this.HasPresence)
			{
				num = num + 2;
				uint num2 = this.Presence.GetSerializedSize();
				num = num + num2 + ProtocolParser.SizeOfUInt32(num2);
			}
			return num;
		}

		public static bnet.protocol.channel.ChannelState ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<bnet.protocol.channel.ChannelState>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			bnet.protocol.channel.ChannelState.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, bnet.protocol.channel.ChannelState instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasMaxMembers)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.MaxMembers);
			}
			if (instance.HasMinMembers)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.MinMembers);
			}
			if (instance.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in instance.Attribute)
				{
					stream.WriteByte(26);
					ProtocolParser.WriteUInt32(stream, attribute.GetSerializedSize());
					bnet.protocol.attribute.Attribute.Serialize(stream, attribute);
				}
			}
			if (instance.Invitation.Count > 0)
			{
				foreach (bnet.protocol.invitation.Invitation invitation in instance.Invitation)
				{
					stream.WriteByte(34);
					ProtocolParser.WriteUInt32(stream, invitation.GetSerializedSize());
					bnet.protocol.invitation.Invitation.Serialize(stream, invitation);
				}
			}
			if (instance.HasMaxInvitations)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteUInt32(stream, instance.MaxInvitations);
			}
			if (instance.HasReason)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteUInt32(stream, instance.Reason);
			}
			if (instance.HasPrivacyLevel)
			{
				stream.WriteByte(56);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.PrivacyLevel);
			}
			if (instance.HasName)
			{
				stream.WriteByte(66);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Name));
			}
			if (instance.HasDelegateName)
			{
				stream.WriteByte(74);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.DelegateName));
			}
			if (instance.HasChannelType)
			{
				stream.WriteByte(82);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.ChannelType));
			}
			if (instance.HasProgram)
			{
				stream.WriteByte(93);
				binaryWriter.Write(instance.Program);
			}
			if (instance.HasAllowOfflineMembers)
			{
				stream.WriteByte(96);
				ProtocolParser.WriteBool(stream, instance.AllowOfflineMembers);
			}
			if (instance.HasSubscribeToPresence)
			{
				stream.WriteByte(104);
				ProtocolParser.WriteBool(stream, instance.SubscribeToPresence);
			}
			if (instance.HasChat)
			{
				stream.WriteByte(162);
				stream.WriteByte(6);
				ProtocolParser.WriteUInt32(stream, instance.Chat.GetSerializedSize());
				bnet.protocol.chat.ChannelState.Serialize(stream, instance.Chat);
			}
			if (instance.HasPresence)
			{
				stream.WriteByte(170);
				stream.WriteByte(6);
				ProtocolParser.WriteUInt32(stream, instance.Presence.GetSerializedSize());
				bnet.protocol.presence.ChannelState.Serialize(stream, instance.Presence);
			}
		}

		public void SetAllowOfflineMembers(bool val)
		{
			this.AllowOfflineMembers = val;
		}

		public void SetAttribute(List<bnet.protocol.attribute.Attribute> val)
		{
			this.Attribute = val;
		}

		public void SetChannelType(string val)
		{
			this.ChannelType = val;
		}

		public void SetChat(bnet.protocol.chat.ChannelState val)
		{
			this.Chat = val;
		}

		public void SetDelegateName(string val)
		{
			this.DelegateName = val;
		}

		public void SetInvitation(List<bnet.protocol.invitation.Invitation> val)
		{
			this.Invitation = val;
		}

		public void SetMaxInvitations(uint val)
		{
			this.MaxInvitations = val;
		}

		public void SetMaxMembers(uint val)
		{
			this.MaxMembers = val;
		}

		public void SetMinMembers(uint val)
		{
			this.MinMembers = val;
		}

		public void SetName(string val)
		{
			this.Name = val;
		}

		public void SetPresence(bnet.protocol.presence.ChannelState val)
		{
			this.Presence = val;
		}

		public void SetPrivacyLevel(bnet.protocol.channel.ChannelState.Types.PrivacyLevel val)
		{
			this.PrivacyLevel = val;
		}

		public void SetProgram(uint val)
		{
			this.Program = val;
		}

		public void SetReason(uint val)
		{
			this.Reason = val;
		}

		public void SetSubscribeToPresence(bool val)
		{
			this.SubscribeToPresence = val;
		}

		public static class Types
		{
			public enum PrivacyLevel
			{
				PRIVACY_LEVEL_OPEN = 1,
				PRIVACY_LEVEL_OPEN_INVITATION_AND_FRIEND = 2,
				PRIVACY_LEVEL_OPEN_INVITATION = 3,
				PRIVACY_LEVEL_CLOSED = 4
			}
		}
	}
}