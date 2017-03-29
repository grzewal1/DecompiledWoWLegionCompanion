using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.account
{
	public class GetEBalanceRequest : IProtoBuf
	{
		public bool HasCurrencyHomeRegion;

		private uint _CurrencyHomeRegion;

		public bnet.protocol.account.AccountId AccountId
		{
			get;
			set;
		}

		public string Currency
		{
			get;
			set;
		}

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

		public GetEBalanceRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetEBalanceRequest.Deserialize(stream, this);
		}

		public static GetEBalanceRequest Deserialize(Stream stream, GetEBalanceRequest instance)
		{
			return GetEBalanceRequest.Deserialize(stream, instance, (long)-1);
		}

		public static GetEBalanceRequest Deserialize(Stream stream, GetEBalanceRequest instance, long limit)
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
							if (instance.AccountId != null)
							{
								bnet.protocol.account.AccountId.DeserializeLengthDelimited(stream, instance.AccountId);
							}
							else
							{
								instance.AccountId = bnet.protocol.account.AccountId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 18)
						{
							instance.Currency = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 24)
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

		public static GetEBalanceRequest DeserializeLengthDelimited(Stream stream)
		{
			GetEBalanceRequest getEBalanceRequest = new GetEBalanceRequest();
			GetEBalanceRequest.DeserializeLengthDelimited(stream, getEBalanceRequest);
			return getEBalanceRequest;
		}

		public static GetEBalanceRequest DeserializeLengthDelimited(Stream stream, GetEBalanceRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GetEBalanceRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetEBalanceRequest getEBalanceRequest = obj as GetEBalanceRequest;
			if (getEBalanceRequest == null)
			{
				return false;
			}
			if (!this.AccountId.Equals(getEBalanceRequest.AccountId))
			{
				return false;
			}
			if (!this.Currency.Equals(getEBalanceRequest.Currency))
			{
				return false;
			}
			if (this.HasCurrencyHomeRegion == getEBalanceRequest.HasCurrencyHomeRegion && (!this.HasCurrencyHomeRegion || this.CurrencyHomeRegion.Equals(getEBalanceRequest.CurrencyHomeRegion)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.AccountId.GetHashCode();
			hashCode = hashCode ^ this.Currency.GetHashCode();
			if (this.HasCurrencyHomeRegion)
			{
				hashCode = hashCode ^ this.CurrencyHomeRegion.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.AccountId.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Currency);
			num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			if (this.HasCurrencyHomeRegion)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.CurrencyHomeRegion);
			}
			num = num + 2;
			return num;
		}

		public static GetEBalanceRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetEBalanceRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetEBalanceRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetEBalanceRequest instance)
		{
			if (instance.AccountId == null)
			{
				throw new ArgumentNullException("AccountId", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.AccountId.GetSerializedSize());
			bnet.protocol.account.AccountId.Serialize(stream, instance.AccountId);
			if (instance.Currency == null)
			{
				throw new ArgumentNullException("Currency", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Currency));
			if (instance.HasCurrencyHomeRegion)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.CurrencyHomeRegion);
			}
		}

		public void SetAccountId(bnet.protocol.account.AccountId val)
		{
			this.AccountId = val;
		}

		public void SetCurrency(string val)
		{
			this.Currency = val;
		}

		public void SetCurrencyHomeRegion(uint val)
		{
			this.CurrencyHomeRegion = val;
		}
	}
}