using bnet.protocol.channel_invitation;
using bnet.protocol.friends;
using System;
using System.IO;
using System.Text;

namespace bnet.protocol.invitation
{
	public class InvitationParams : IProtoBuf
	{
		public bool HasInvitationMessage;

		private string _InvitationMessage;

		public bool HasExpirationTime;

		private ulong _ExpirationTime;

		public bool HasChannelParams;

		private ChannelInvitationParams _ChannelParams;

		public bool HasFriendParams;

		private FriendInvitationParams _FriendParams;

		public ChannelInvitationParams ChannelParams
		{
			get
			{
				return this._ChannelParams;
			}
			set
			{
				this._ChannelParams = value;
				this.HasChannelParams = value != null;
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

		public FriendInvitationParams FriendParams
		{
			get
			{
				return this._FriendParams;
			}
			set
			{
				this._FriendParams = value;
				this.HasFriendParams = value != null;
			}
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

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public InvitationParams()
		{
		}

		public void Deserialize(Stream stream)
		{
			InvitationParams.Deserialize(stream, this);
		}

		public static InvitationParams Deserialize(Stream stream, InvitationParams instance)
		{
			return InvitationParams.Deserialize(stream, instance, (long)-1);
		}

		public static InvitationParams Deserialize(Stream stream, InvitationParams instance, long limit)
		{
			instance.ExpirationTime = (ulong)0;
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
							instance.InvitationMessage = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 16)
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
										if (instance.FriendParams != null)
										{
											FriendInvitationParams.DeserializeLengthDelimited(stream, instance.FriendParams);
										}
										else
										{
											instance.FriendParams = FriendInvitationParams.DeserializeLengthDelimited(stream);
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
										if (instance.ChannelParams != null)
										{
											ChannelInvitationParams.DeserializeLengthDelimited(stream, instance.ChannelParams);
										}
										else
										{
											instance.ChannelParams = ChannelInvitationParams.DeserializeLengthDelimited(stream);
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

		public static InvitationParams DeserializeLengthDelimited(Stream stream)
		{
			InvitationParams invitationParam = new InvitationParams();
			InvitationParams.DeserializeLengthDelimited(stream, invitationParam);
			return invitationParam;
		}

		public static InvitationParams DeserializeLengthDelimited(Stream stream, InvitationParams instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return InvitationParams.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			InvitationParams invitationParam = obj as InvitationParams;
			if (invitationParam == null)
			{
				return false;
			}
			if (this.HasInvitationMessage != invitationParam.HasInvitationMessage || this.HasInvitationMessage && !this.InvitationMessage.Equals(invitationParam.InvitationMessage))
			{
				return false;
			}
			if (this.HasExpirationTime != invitationParam.HasExpirationTime || this.HasExpirationTime && !this.ExpirationTime.Equals(invitationParam.ExpirationTime))
			{
				return false;
			}
			if (this.HasChannelParams != invitationParam.HasChannelParams || this.HasChannelParams && !this.ChannelParams.Equals(invitationParam.ChannelParams))
			{
				return false;
			}
			if (this.HasFriendParams == invitationParam.HasFriendParams && (!this.HasFriendParams || this.FriendParams.Equals(invitationParam.FriendParams)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasInvitationMessage)
			{
				hashCode ^= this.InvitationMessage.GetHashCode();
			}
			if (this.HasExpirationTime)
			{
				hashCode ^= this.ExpirationTime.GetHashCode();
			}
			if (this.HasChannelParams)
			{
				hashCode ^= this.ChannelParams.GetHashCode();
			}
			if (this.HasFriendParams)
			{
				hashCode ^= this.FriendParams.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasInvitationMessage)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.InvitationMessage);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasExpirationTime)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.ExpirationTime);
			}
			if (this.HasChannelParams)
			{
				num += 2;
				uint serializedSize = this.ChannelParams.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasFriendParams)
			{
				num += 2;
				uint serializedSize1 = this.FriendParams.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			return num;
		}

		public static InvitationParams ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<InvitationParams>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			InvitationParams.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, InvitationParams instance)
		{
			if (instance.HasInvitationMessage)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.InvitationMessage));
			}
			if (instance.HasExpirationTime)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, instance.ExpirationTime);
			}
			if (instance.HasChannelParams)
			{
				stream.WriteByte(202);
				stream.WriteByte(6);
				ProtocolParser.WriteUInt32(stream, instance.ChannelParams.GetSerializedSize());
				ChannelInvitationParams.Serialize(stream, instance.ChannelParams);
			}
			if (instance.HasFriendParams)
			{
				stream.WriteByte(186);
				stream.WriteByte(6);
				ProtocolParser.WriteUInt32(stream, instance.FriendParams.GetSerializedSize());
				FriendInvitationParams.Serialize(stream, instance.FriendParams);
			}
		}

		public void SetChannelParams(ChannelInvitationParams val)
		{
			this.ChannelParams = val;
		}

		public void SetExpirationTime(ulong val)
		{
			this.ExpirationTime = val;
		}

		public void SetFriendParams(FriendInvitationParams val)
		{
			this.FriendParams = val;
		}

		public void SetInvitationMessage(string val)
		{
			this.InvitationMessage = val;
		}
	}
}