using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel_invitation
{
	public class DecrementChannelCountRequest : IProtoBuf
	{
		public bool HasChannelId;

		private EntityId _ChannelId;

		public bool HasReservationToken;

		private ulong _ReservationToken;

		public EntityId AgentId
		{
			get;
			set;
		}

		public EntityId ChannelId
		{
			get
			{
				return this._ChannelId;
			}
			set
			{
				this._ChannelId = value;
				this.HasChannelId = value != null;
			}
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

		public DecrementChannelCountRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			DecrementChannelCountRequest.Deserialize(stream, this);
		}

		public static DecrementChannelCountRequest Deserialize(Stream stream, DecrementChannelCountRequest instance)
		{
			return DecrementChannelCountRequest.Deserialize(stream, instance, (long)-1);
		}

		public static DecrementChannelCountRequest Deserialize(Stream stream, DecrementChannelCountRequest instance, long limit)
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
							if (instance.AgentId != null)
							{
								EntityId.DeserializeLengthDelimited(stream, instance.AgentId);
							}
							else
							{
								instance.AgentId = EntityId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 != 18)
						{
							if (num1 == 24)
							{
								instance.ReservationToken = ProtocolParser.ReadUInt64(stream);
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

		public static DecrementChannelCountRequest DeserializeLengthDelimited(Stream stream)
		{
			DecrementChannelCountRequest decrementChannelCountRequest = new DecrementChannelCountRequest();
			DecrementChannelCountRequest.DeserializeLengthDelimited(stream, decrementChannelCountRequest);
			return decrementChannelCountRequest;
		}

		public static DecrementChannelCountRequest DeserializeLengthDelimited(Stream stream, DecrementChannelCountRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return DecrementChannelCountRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			DecrementChannelCountRequest decrementChannelCountRequest = obj as DecrementChannelCountRequest;
			if (decrementChannelCountRequest == null)
			{
				return false;
			}
			if (!this.AgentId.Equals(decrementChannelCountRequest.AgentId))
			{
				return false;
			}
			if (this.HasChannelId != decrementChannelCountRequest.HasChannelId || this.HasChannelId && !this.ChannelId.Equals(decrementChannelCountRequest.ChannelId))
			{
				return false;
			}
			if (this.HasReservationToken == decrementChannelCountRequest.HasReservationToken && (!this.HasReservationToken || this.ReservationToken.Equals(decrementChannelCountRequest.ReservationToken)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.AgentId.GetHashCode();
			if (this.HasChannelId)
			{
				hashCode = hashCode ^ this.ChannelId.GetHashCode();
			}
			if (this.HasReservationToken)
			{
				hashCode = hashCode ^ this.ReservationToken.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.AgentId.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasChannelId)
			{
				num++;
				uint serializedSize1 = this.ChannelId.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			if (this.HasReservationToken)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt64(this.ReservationToken);
			}
			num++;
			return num;
		}

		public static DecrementChannelCountRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<DecrementChannelCountRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			DecrementChannelCountRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, DecrementChannelCountRequest instance)
		{
			if (instance.AgentId == null)
			{
				throw new ArgumentNullException("AgentId", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.AgentId.GetSerializedSize());
			EntityId.Serialize(stream, instance.AgentId);
			if (instance.HasChannelId)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.ChannelId.GetSerializedSize());
				EntityId.Serialize(stream, instance.ChannelId);
			}
			if (instance.HasReservationToken)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt64(stream, instance.ReservationToken);
			}
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