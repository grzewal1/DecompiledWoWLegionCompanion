using System;
using System.IO;
using System.Text;

namespace bnet.protocol.account
{
	public class GetEBalanceResponse : IProtoBuf
	{
		public bool HasBalance;

		private string _Balance;

		public string Balance
		{
			get
			{
				return this._Balance;
			}
			set
			{
				this._Balance = value;
				this.HasBalance = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public GetEBalanceResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetEBalanceResponse.Deserialize(stream, this);
		}

		public static GetEBalanceResponse Deserialize(Stream stream, GetEBalanceResponse instance)
		{
			return GetEBalanceResponse.Deserialize(stream, instance, (long)-1);
		}

		public static GetEBalanceResponse Deserialize(Stream stream, GetEBalanceResponse instance, long limit)
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
						instance.Balance = ProtocolParser.ReadString(stream);
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
					if (stream.Position != limit)
					{
						throw new ProtocolBufferException("Read past max limit");
					}
					break;
				}
			}
			return instance;
		}

		public static GetEBalanceResponse DeserializeLengthDelimited(Stream stream)
		{
			GetEBalanceResponse getEBalanceResponse = new GetEBalanceResponse();
			GetEBalanceResponse.DeserializeLengthDelimited(stream, getEBalanceResponse);
			return getEBalanceResponse;
		}

		public static GetEBalanceResponse DeserializeLengthDelimited(Stream stream, GetEBalanceResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GetEBalanceResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetEBalanceResponse getEBalanceResponse = obj as GetEBalanceResponse;
			if (getEBalanceResponse == null)
			{
				return false;
			}
			if (this.HasBalance == getEBalanceResponse.HasBalance && (!this.HasBalance || this.Balance.Equals(getEBalanceResponse.Balance)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasBalance)
			{
				hashCode ^= this.Balance.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasBalance)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Balance);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			return num;
		}

		public static GetEBalanceResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetEBalanceResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetEBalanceResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetEBalanceResponse instance)
		{
			if (instance.HasBalance)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Balance));
			}
		}

		public void SetBalance(string val)
		{
			this.Balance = val;
		}
	}
}