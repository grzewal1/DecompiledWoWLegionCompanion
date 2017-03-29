using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GetGameTimeRemainingInfoResponse : IProtoBuf
	{
		public bool HasGameTimeRemainingInfo;

		private bnet.protocol.account.GameTimeRemainingInfo _GameTimeRemainingInfo;

		public bnet.protocol.account.GameTimeRemainingInfo GameTimeRemainingInfo
		{
			get
			{
				return this._GameTimeRemainingInfo;
			}
			set
			{
				this._GameTimeRemainingInfo = value;
				this.HasGameTimeRemainingInfo = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public GetGameTimeRemainingInfoResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetGameTimeRemainingInfoResponse.Deserialize(stream, this);
		}

		public static GetGameTimeRemainingInfoResponse Deserialize(Stream stream, GetGameTimeRemainingInfoResponse instance)
		{
			return GetGameTimeRemainingInfoResponse.Deserialize(stream, instance, (long)-1);
		}

		public static GetGameTimeRemainingInfoResponse Deserialize(Stream stream, GetGameTimeRemainingInfoResponse instance, long limit)
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
					else if (instance.GameTimeRemainingInfo != null)
					{
						bnet.protocol.account.GameTimeRemainingInfo.DeserializeLengthDelimited(stream, instance.GameTimeRemainingInfo);
					}
					else
					{
						instance.GameTimeRemainingInfo = bnet.protocol.account.GameTimeRemainingInfo.DeserializeLengthDelimited(stream);
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

		public static GetGameTimeRemainingInfoResponse DeserializeLengthDelimited(Stream stream)
		{
			GetGameTimeRemainingInfoResponse getGameTimeRemainingInfoResponse = new GetGameTimeRemainingInfoResponse();
			GetGameTimeRemainingInfoResponse.DeserializeLengthDelimited(stream, getGameTimeRemainingInfoResponse);
			return getGameTimeRemainingInfoResponse;
		}

		public static GetGameTimeRemainingInfoResponse DeserializeLengthDelimited(Stream stream, GetGameTimeRemainingInfoResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GetGameTimeRemainingInfoResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetGameTimeRemainingInfoResponse getGameTimeRemainingInfoResponse = obj as GetGameTimeRemainingInfoResponse;
			if (getGameTimeRemainingInfoResponse == null)
			{
				return false;
			}
			if (this.HasGameTimeRemainingInfo == getGameTimeRemainingInfoResponse.HasGameTimeRemainingInfo && (!this.HasGameTimeRemainingInfo || this.GameTimeRemainingInfo.Equals(getGameTimeRemainingInfoResponse.GameTimeRemainingInfo)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasGameTimeRemainingInfo)
			{
				hashCode = hashCode ^ this.GameTimeRemainingInfo.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasGameTimeRemainingInfo)
			{
				num++;
				uint serializedSize = this.GameTimeRemainingInfo.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			return num;
		}

		public static GetGameTimeRemainingInfoResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetGameTimeRemainingInfoResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetGameTimeRemainingInfoResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetGameTimeRemainingInfoResponse instance)
		{
			if (instance.HasGameTimeRemainingInfo)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.GameTimeRemainingInfo.GetSerializedSize());
				bnet.protocol.account.GameTimeRemainingInfo.Serialize(stream, instance.GameTimeRemainingInfo);
			}
		}

		public void SetGameTimeRemainingInfo(bnet.protocol.account.GameTimeRemainingInfo val)
		{
			this.GameTimeRemainingInfo = val;
		}
	}
}