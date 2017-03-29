using System;
using System.IO;

namespace bnet.protocol.channel
{
	public class GetChannelInfoResponse : IProtoBuf
	{
		public bool HasChannelInfo;

		private bnet.protocol.channel.ChannelInfo _ChannelInfo;

		public bnet.protocol.channel.ChannelInfo ChannelInfo
		{
			get
			{
				return this._ChannelInfo;
			}
			set
			{
				this._ChannelInfo = value;
				this.HasChannelInfo = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public GetChannelInfoResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetChannelInfoResponse.Deserialize(stream, this);
		}

		public static GetChannelInfoResponse Deserialize(Stream stream, GetChannelInfoResponse instance)
		{
			return GetChannelInfoResponse.Deserialize(stream, instance, (long)-1);
		}

		public static GetChannelInfoResponse Deserialize(Stream stream, GetChannelInfoResponse instance, long limit)
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
					else if (instance.ChannelInfo != null)
					{
						bnet.protocol.channel.ChannelInfo.DeserializeLengthDelimited(stream, instance.ChannelInfo);
					}
					else
					{
						instance.ChannelInfo = bnet.protocol.channel.ChannelInfo.DeserializeLengthDelimited(stream);
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

		public static GetChannelInfoResponse DeserializeLengthDelimited(Stream stream)
		{
			GetChannelInfoResponse getChannelInfoResponse = new GetChannelInfoResponse();
			GetChannelInfoResponse.DeserializeLengthDelimited(stream, getChannelInfoResponse);
			return getChannelInfoResponse;
		}

		public static GetChannelInfoResponse DeserializeLengthDelimited(Stream stream, GetChannelInfoResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GetChannelInfoResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetChannelInfoResponse getChannelInfoResponse = obj as GetChannelInfoResponse;
			if (getChannelInfoResponse == null)
			{
				return false;
			}
			if (this.HasChannelInfo == getChannelInfoResponse.HasChannelInfo && (!this.HasChannelInfo || this.ChannelInfo.Equals(getChannelInfoResponse.ChannelInfo)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasChannelInfo)
			{
				hashCode = hashCode ^ this.ChannelInfo.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasChannelInfo)
			{
				num++;
				uint serializedSize = this.ChannelInfo.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			return num;
		}

		public static GetChannelInfoResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetChannelInfoResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetChannelInfoResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetChannelInfoResponse instance)
		{
			if (instance.HasChannelInfo)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.ChannelInfo.GetSerializedSize());
				bnet.protocol.channel.ChannelInfo.Serialize(stream, instance.ChannelInfo);
			}
		}

		public void SetChannelInfo(bnet.protocol.channel.ChannelInfo val)
		{
			this.ChannelInfo = val;
		}
	}
}