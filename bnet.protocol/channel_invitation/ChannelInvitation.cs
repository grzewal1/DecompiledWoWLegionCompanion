using bnet.protocol.channel;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel_invitation
{
	public class ChannelInvitation : IProtoBuf
	{
		public bool HasReserved;

		private bool _Reserved;

		public bool HasRejoin;

		private bool _Rejoin;

		public bnet.protocol.channel.ChannelDescription ChannelDescription
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

		public ChannelInvitation()
		{
		}

		public void Deserialize(Stream stream)
		{
			ChannelInvitation.Deserialize(stream, this);
		}

		public static ChannelInvitation Deserialize(Stream stream, ChannelInvitation instance)
		{
			return ChannelInvitation.Deserialize(stream, instance, (long)-1);
		}

		public static ChannelInvitation Deserialize(Stream stream, ChannelInvitation instance, long limit)
		{
			instance.Reserved = false;
			instance.Rejoin = false;
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
							if (instance.ChannelDescription != null)
							{
								bnet.protocol.channel.ChannelDescription.DeserializeLengthDelimited(stream, instance.ChannelDescription);
							}
							else
							{
								instance.ChannelDescription = bnet.protocol.channel.ChannelDescription.DeserializeLengthDelimited(stream);
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

		public static ChannelInvitation DeserializeLengthDelimited(Stream stream)
		{
			ChannelInvitation channelInvitation = new ChannelInvitation();
			ChannelInvitation.DeserializeLengthDelimited(stream, channelInvitation);
			return channelInvitation;
		}

		public static ChannelInvitation DeserializeLengthDelimited(Stream stream, ChannelInvitation instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ChannelInvitation.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ChannelInvitation channelInvitation = obj as ChannelInvitation;
			if (channelInvitation == null)
			{
				return false;
			}
			if (!this.ChannelDescription.Equals(channelInvitation.ChannelDescription))
			{
				return false;
			}
			if (this.HasReserved != channelInvitation.HasReserved || this.HasReserved && !this.Reserved.Equals(channelInvitation.Reserved))
			{
				return false;
			}
			if (this.HasRejoin != channelInvitation.HasRejoin || this.HasRejoin && !this.Rejoin.Equals(channelInvitation.Rejoin))
			{
				return false;
			}
			if (!this.ServiceType.Equals(channelInvitation.ServiceType))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.ChannelDescription.GetHashCode();
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
			uint serializedSize = this.ChannelDescription.GetSerializedSize();
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

		public static ChannelInvitation ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ChannelInvitation>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ChannelInvitation.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ChannelInvitation instance)
		{
			if (instance.ChannelDescription == null)
			{
				throw new ArgumentNullException("ChannelDescription", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.ChannelDescription.GetSerializedSize());
			bnet.protocol.channel.ChannelDescription.Serialize(stream, instance.ChannelDescription);
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

		public void SetChannelDescription(bnet.protocol.channel.ChannelDescription val)
		{
			this.ChannelDescription = val;
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