using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GetEBalanceRestrictionsRequest : IProtoBuf
	{
		public bool HasCurrencyHomeRegion;

		private uint _CurrencyHomeRegion;

		public uint CurrencyHomeRegion
		{
			get
			{
				return this._CurrencyHomeRegion;
			}
			set
			{
				this._CurrencyHomeRegion = value;
				this.HasCurrencyHomeRegion = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public GetEBalanceRestrictionsRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetEBalanceRestrictionsRequest.Deserialize(stream, this);
		}

		public static GetEBalanceRestrictionsRequest Deserialize(Stream stream, GetEBalanceRestrictionsRequest instance)
		{
			return GetEBalanceRestrictionsRequest.Deserialize(stream, instance, (long)-1);
		}

		public static GetEBalanceRestrictionsRequest Deserialize(Stream stream, GetEBalanceRestrictionsRequest instance, long limit)
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
					else if (num == 8)
					{
						instance.CurrencyHomeRegion = ProtocolParser.ReadUInt32(stream);
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

		public static GetEBalanceRestrictionsRequest DeserializeLengthDelimited(Stream stream)
		{
			GetEBalanceRestrictionsRequest getEBalanceRestrictionsRequest = new GetEBalanceRestrictionsRequest();
			GetEBalanceRestrictionsRequest.DeserializeLengthDelimited(stream, getEBalanceRestrictionsRequest);
			return getEBalanceRestrictionsRequest;
		}

		public static GetEBalanceRestrictionsRequest DeserializeLengthDelimited(Stream stream, GetEBalanceRestrictionsRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GetEBalanceRestrictionsRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetEBalanceRestrictionsRequest getEBalanceRestrictionsRequest = obj as GetEBalanceRestrictionsRequest;
			if (getEBalanceRestrictionsRequest == null)
			{
				return false;
			}
			if (this.HasCurrencyHomeRegion == getEBalanceRestrictionsRequest.HasCurrencyHomeRegion && (!this.HasCurrencyHomeRegion || this.CurrencyHomeRegion.Equals(getEBalanceRestrictionsRequest.CurrencyHomeRegion)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasCurrencyHomeRegion)
			{
				hashCode ^= this.CurrencyHomeRegion.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasCurrencyHomeRegion)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.CurrencyHomeRegion);
			}
			return num;
		}

		public static GetEBalanceRestrictionsRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetEBalanceRestrictionsRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetEBalanceRestrictionsRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetEBalanceRestrictionsRequest instance)
		{
			if (instance.HasCurrencyHomeRegion)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.CurrencyHomeRegion);
			}
		}

		public void SetCurrencyHomeRegion(uint val)
		{
			this.CurrencyHomeRegion = val;
		}
	}
}