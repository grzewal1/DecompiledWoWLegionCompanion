using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel_invitation
{
	public class UpdateChannelCountRequest : IProtoBuf
	{
		public bool HasReservationToken;

		private ulong _ReservationToken;

		public EntityId AgentId
		{
			get;
			set;
		}

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

		public ulong ReservationToken
		{
			get
			{
				return this._ReservationToken;
			}
			set
			{
				this._ReservationToken = value;
				this.HasReservationToken = true;
			}
		}

		public UpdateChannelCountRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			UpdateChannelCountRequest.Deserialize(stream, this);
		}

		public static UpdateChannelCountRequest Deserialize(Stream stream, UpdateChannelCountRequest instance)
		{
			return UpdateChannelCountRequest.Deserialize(stream, instance, (long)-1);
		}

		public static UpdateChannelCountRequest Deserialize(Stream stream, UpdateChannelCountRequest instance, long limit)
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
					else if (num == 10)
					{
						if (instance.AgentId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.AgentId);
						}
						else
						{
							instance.AgentId = EntityId.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 16)
					{
						instance.ReservationToken = ProtocolParser.ReadUInt64(stream);
					}
					else if (num != 26)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.ChannelId != null)
					{
						EntityId.DeserializeLengthDelimited(stream, instance.ChannelId);
					}
					else
					{
						instance.ChannelId = EntityId.DeserializeLengthDelimited(stream);
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

		public static UpdateChannelCountRequest DeserializeLengthDelimited(Stream stream)
		{
			UpdateChannelCountRequest updateChannelCountRequest = new UpdateChannelCountRequest();
			UpdateChannelCountRequest.DeserializeLengthDelimited(stream, updateChannelCountRequest);
			return updateChannelCountRequest;
		}

		public static UpdateChannelCountRequest DeserializeLengthDelimited(Stream stream, UpdateChannelCountRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return UpdateChannelCountRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			UpdateChannelCountRequest updateChannelCountRequest = obj as UpdateChannelCountRequest;
			if (updateChannelCountRequest == null)
			{
				return false;
			}
			if (!this.AgentId.Equals(updateChannelCountRequest.AgentId))
			{
				return false;
			}
			if (this.HasReservationToken != updateChannelCountRequest.HasReservationToken || this.HasReservationToken && !this.ReservationToken.Equals(updateChannelCountRequest.ReservationToken))
			{
				return false;
			}
			if (!this.ChannelId.Equals(updateChannelCountRequest.ChannelId))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.AgentId.GetHashCode();
			if (this.HasReservationToken)
			{
				hashCode ^= this.ReservationToken.GetHashCode();
			}
			hashCode ^= this.ChannelId.GetHashCode();
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.AgentId.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasReservationToken)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.ReservationToken);
			}
			uint serializedSize1 = this.ChannelId.GetSerializedSize();
			num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			num += 2;
			return num;
		}

		public static UpdateChannelCountRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<UpdateChannelCountRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			UpdateChannelCountRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, UpdateChannelCountRequest instance)
		{
			if (instance.AgentId == null)
			{
				throw new ArgumentNullException("AgentId", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.AgentId.GetSerializedSize());
			EntityId.Serialize(stream, instance.AgentId);
			if (instance.HasReservationToken)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, instance.ReservationToken);
			}
			if (instance.ChannelId == null)
			{
				throw new ArgumentNullException("ChannelId", "Required by proto specification.");
			}
			stream.WriteByte(26);
			ProtocolParser.WriteUInt32(stream, instance.ChannelId.GetSerializedSize());
			EntityId.Serialize(stream, instance.ChannelId);
		}

		public void SetAgentId(EntityId val)
		{
			this.AgentId = val;
		}

		public void SetChannelId(EntityId val)
		{
			this.ChannelId = val;
		}

		public void SetReservationToken(ulong val)
		{
			this.ReservationToken = val;
		}
	}
}