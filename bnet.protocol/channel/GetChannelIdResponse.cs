using bnet.protocol;
using System;
using System.IO;

namespace bnet.protocol.channel
{
	public class GetChannelIdResponse : IProtoBuf
	{
		public bool HasChannelId;

		private EntityId _ChannelId;

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

		public GetChannelIdResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetChannelIdResponse.Deserialize(stream, this);
		}

		public static GetChannelIdResponse Deserialize(Stream stream, GetChannelIdResponse instance)
		{
			return GetChannelIdResponse.Deserialize(stream, instance, (long)-1);
		}

		public static GetChannelIdResponse Deserialize(Stream stream, GetChannelIdResponse instance, long limit)
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

		public static GetChannelIdResponse DeserializeLengthDelimited(Stream stream)
		{
			GetChannelIdResponse getChannelIdResponse = new GetChannelIdResponse();
			GetChannelIdResponse.DeserializeLengthDelimited(stream, getChannelIdResponse);
			return getChannelIdResponse;
		}

		public static GetChannelIdResponse DeserializeLengthDelimited(Stream stream, GetChannelIdResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GetChannelIdResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetChannelIdResponse getChannelIdResponse = obj as GetChannelIdResponse;
			if (getChannelIdResponse == null)
			{
				return false;
			}
			if (this.HasChannelId == getChannelIdResponse.HasChannelId && (!this.HasChannelId || this.ChannelId.Equals(getChannelIdResponse.ChannelId)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasChannelId)
			{
				hashCode = hashCode ^ this.ChannelId.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasChannelId)
			{
				num++;
				uint serializedSize = this.ChannelId.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			return num;
		}

		public static GetChannelIdResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetChannelIdResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetChannelIdResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetChannelIdResponse instance)
		{
			if (instance.HasChannelId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.ChannelId.GetSerializedSize());
				EntityId.Serialize(stream, instance.ChannelId);
			}
		}

		public void SetChannelId(EntityId val)
		{
			this.ChannelId = val;
		}
	}
}