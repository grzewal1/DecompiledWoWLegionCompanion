using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel_invitation
{
	public class ChannelInvitationParams : IProtoBuf
	{
		public bool HasReserved;

		private bool _Reserved;

		public bool HasRejoin;

		private bool _Rejoin;

		public EntityId ChannelId
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

		public bool Rejoin
		{
			get
			{
				return this._Rejoin;
			}
			set
			{
				this._Rejoin = value;
				this.HasRejoin = true;
			}
		}

		public bool Reserved
		{
			get
			{
				return this._Reserved;
			}
			set
			{
				this._Reserved = value;
				this.HasReserved = true;
			}
		}

		public uint ServiceType
		{
			get;
			set;
		}

		public ChannelInvitationParams()
		{
		}

		public void Deserialize(Stream stream)
		{
			ChannelInvitationParams.Deserialize(stream, this);
		}

		public static ChannelInvitationParams Deserialize(Stream stream, ChannelInvitationParams instance)
		{
			return ChannelInvitationParams.Deserialize(stream, instance, (long)-1);
		}

		public static ChannelInvitationParams Deserialize(Stream stream, ChannelInvitationParams instance, long limit)
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
							if (instance.ChannelId != null)
							{
								EntityId.DeserializeLengthDelimited(stream, instance.ChannelId);
							}
							else
							{
								instance.ChannelId = EntityId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 16)
						{
							instance.Reserved = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 24)
						{
							instance.Rejoin = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 32)
						{
							instance.ServiceType = ProtocolParser.ReadUInt32(stream);
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

		public static ChannelInvitationParams DeserializeLengthDelimited(Stream stream)
		{
			ChannelInvitationParams channelInvitationParam = new ChannelInvitationParams();
			ChannelInvitationParams.DeserializeLengthDelimited(stream, channelInvitationParam);
			return channelInvitationParam;
		}

		public static ChannelInvitationParams DeserializeLengthDelimited(Stream stream, ChannelInvitationParams instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ChannelInvitationParams.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ChannelInvitationParams channelInvitationParam = obj as ChannelInvitationParams;
			if (channelInvitationParam == null)
			{
				return false;
			}
			if (!this.ChannelId.Equals(channelInvitationParam.ChannelId))
			{
				return false;
			}
			if (this.HasReserved != channelInvitationParam.HasReserved || this.HasReserved && !this.Reserved.Equals(channelInvitationParam.Reserved))
			{
				return false;
			}
			if (this.HasRejoin != channelInvitationParam.HasRejoin || this.HasRejoin && !this.Rejoin.Equals(channelInvitationParam.Rejoin))
			{
				return false;
			}
			if (!this.ServiceType.Equals(channelInvitationParam.ServiceType))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.ChannelId.GetHashCode();
			if (this.HasReserved)
			{
				hashCode ^= this.Reserved.GetHashCode();
			}
			if (this.HasRejoin)
			{
				hashCode ^= this.Rejoin.GetHashCode();
			}
			hashCode ^= this.ServiceType.GetHashCode();
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.ChannelId.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasReserved)
			{
				num++;
				num++;
			}
			if (this.HasRejoin)
			{
				num++;
				num++;
			}
			num += ProtocolParser.SizeOfUInt32(this.ServiceType);
			num += 2;
			return num;
		}

		public static ChannelInvitationParams ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ChannelInvitationParams>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ChannelInvitationParams.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ChannelInvitationParams instance)
		{
			if (instance.ChannelId == null)
			{
				throw new ArgumentNullException("ChannelId", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.ChannelId.GetSerializedSize());
			EntityId.Serialize(stream, instance.ChannelId);
			if (instance.HasReserved)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteBool(stream, instance.Reserved);
			}
			if (instance.HasRejoin)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteBool(stream, instance.Rejoin);
			}
			stream.WriteByte(32);
			ProtocolParser.WriteUInt32(stream, instance.ServiceType);
		}

		public void SetChannelId(EntityId val)
		{
			this.ChannelId = val;
		}

		public void SetRejoin(bool val)
		{
			this.Rejoin = val;
		}

		public void SetReserved(bool val)
		{
			this.Reserved = val;
		}

		public void SetServiceType(uint val)
		{
			this.ServiceType = val;
		}
	}
}