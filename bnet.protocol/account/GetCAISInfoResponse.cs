using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GetCAISInfoResponse : IProtoBuf
	{
		public bool HasCaisInfo;

		private CAIS _CaisInfo;

		public CAIS CaisInfo
		{
			get
			{
				return this._CaisInfo;
			}
			set
			{
				this._CaisInfo = value;
				this.HasCaisInfo = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public GetCAISInfoResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetCAISInfoResponse.Deserialize(stream, this);
		}

		public static GetCAISInfoResponse Deserialize(Stream stream, GetCAISInfoResponse instance)
		{
			return GetCAISInfoResponse.Deserialize(stream, instance, (long)-1);
		}

		public static GetCAISInfoResponse Deserialize(Stream stream, GetCAISInfoResponse instance, long limit)
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
					else if (instance.CaisInfo != null)
					{
						CAIS.DeserializeLengthDelimited(stream, instance.CaisInfo);
					}
					else
					{
						instance.CaisInfo = CAIS.DeserializeLengthDelimited(stream);
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

		public static GetCAISInfoResponse DeserializeLengthDelimited(Stream stream)
		{
			GetCAISInfoResponse getCAISInfoResponse = new GetCAISInfoResponse();
			GetCAISInfoResponse.DeserializeLengthDelimited(stream, getCAISInfoResponse);
			return getCAISInfoResponse;
		}

		public static GetCAISInfoResponse DeserializeLengthDelimited(Stream stream, GetCAISInfoResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GetCAISInfoResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetCAISInfoResponse getCAISInfoResponse = obj as GetCAISInfoResponse;
			if (getCAISInfoResponse == null)
			{
				return false;
			}
			if (this.HasCaisInfo == getCAISInfoResponse.HasCaisInfo && (!this.HasCaisInfo || this.CaisInfo.Equals(getCAISInfoResponse.CaisInfo)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasCaisInfo)
			{
				hashCode = hashCode ^ this.CaisInfo.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasCaisInfo)
			{
				num++;
				uint serializedSize = this.CaisInfo.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			return num;
		}

		public static GetCAISInfoResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetCAISInfoResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetCAISInfoResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetCAISInfoResponse instance)
		{
			if (instance.HasCaisInfo)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.CaisInfo.GetSerializedSize());
				CAIS.Serialize(stream, instance.CaisInfo);
			}
		}

		public void SetCaisInfo(CAIS val)
		{
			this.CaisInfo = val;
		}
	}
}