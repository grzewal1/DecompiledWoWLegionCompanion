using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GetGameSessionInfoResponse : IProtoBuf
	{
		public bool HasSessionInfo;

		private GameSessionInfo _SessionInfo;

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public GameSessionInfo SessionInfo
		{
			get
			{
				return this._SessionInfo;
			}
			set
			{
				this._SessionInfo = value;
				this.HasSessionInfo = value != null;
			}
		}

		public GetGameSessionInfoResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetGameSessionInfoResponse.Deserialize(stream, this);
		}

		public static GetGameSessionInfoResponse Deserialize(Stream stream, GetGameSessionInfoResponse instance)
		{
			return GetGameSessionInfoResponse.Deserialize(stream, instance, (long)-1);
		}

		public static GetGameSessionInfoResponse Deserialize(Stream stream, GetGameSessionInfoResponse instance, long limit)
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
					else if (num != 18)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.SessionInfo != null)
					{
						GameSessionInfo.DeserializeLengthDelimited(stream, instance.SessionInfo);
					}
					else
					{
						instance.SessionInfo = GameSessionInfo.DeserializeLengthDelimited(stream);
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

		public static GetGameSessionInfoResponse DeserializeLengthDelimited(Stream stream)
		{
			GetGameSessionInfoResponse getGameSessionInfoResponse = new GetGameSessionInfoResponse();
			GetGameSessionInfoResponse.DeserializeLengthDelimited(stream, getGameSessionInfoResponse);
			return getGameSessionInfoResponse;
		}

		public static GetGameSessionInfoResponse DeserializeLengthDelimited(Stream stream, GetGameSessionInfoResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GetGameSessionInfoResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetGameSessionInfoResponse getGameSessionInfoResponse = obj as GetGameSessionInfoResponse;
			if (getGameSessionInfoResponse == null)
			{
				return false;
			}
			if (this.HasSessionInfo == getGameSessionInfoResponse.HasSessionInfo && (!this.HasSessionInfo || this.SessionInfo.Equals(getGameSessionInfoResponse.SessionInfo)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasSessionInfo)
			{
				hashCode = hashCode ^ this.SessionInfo.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasSessionInfo)
			{
				num++;
				uint serializedSize = this.SessionInfo.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			return num;
		}

		public static GetGameSessionInfoResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetGameSessionInfoResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetGameSessionInfoResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetGameSessionInfoResponse instance)
		{
			if (instance.HasSessionInfo)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.SessionInfo.GetSerializedSize());
				GameSessionInfo.Serialize(stream, instance.SessionInfo);
			}
		}

		public void SetSessionInfo(GameSessionInfo val)
		{
			this.SessionInfo = val;
		}
	}
}