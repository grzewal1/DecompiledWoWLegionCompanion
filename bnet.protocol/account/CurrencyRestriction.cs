using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.account
{
	public class CurrencyRestriction : IProtoBuf
	{
		public string AuthenticatorCap
		{
			get;
			set;
		}

		public string Currency
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

		public string SoftCap
		{
			get;
			set;
		}

		public CurrencyRestriction()
		{
		}

		public void Deserialize(Stream stream)
		{
			CurrencyRestriction.Deserialize(stream, this);
		}

		public static CurrencyRestriction Deserialize(Stream stream, CurrencyRestriction instance)
		{
			return CurrencyRestriction.Deserialize(stream, instance, (long)-1);
		}

		public static CurrencyRestriction Deserialize(Stream stream, CurrencyRestriction instance, long limit)
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
						instance.Currency = ProtocolParser.ReadString(stream);
					}
					else if (num == 18)
					{
						instance.AuthenticatorCap = ProtocolParser.ReadString(stream);
					}
					else if (num == 26)
					{
						instance.SoftCap = ProtocolParser.ReadString(stream);
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

		public static CurrencyRestriction DeserializeLengthDelimited(Stream stream)
		{
			CurrencyRestriction currencyRestriction = new CurrencyRestriction();
			CurrencyRestriction.DeserializeLengthDelimited(stream, currencyRestriction);
			return currencyRestriction;
		}

		public static CurrencyRestriction DeserializeLengthDelimited(Stream stream, CurrencyRestriction instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return CurrencyRestriction.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			CurrencyRestriction currencyRestriction = obj as CurrencyRestriction;
			if (currencyRestriction == null)
			{
				return false;
			}
			if (!this.Currency.Equals(currencyRestriction.Currency))
			{
				return false;
			}
			if (!this.AuthenticatorCap.Equals(currencyRestriction.AuthenticatorCap))
			{
				return false;
			}
			if (!this.SoftCap.Equals(currencyRestriction.SoftCap))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Currency.GetHashCode();
			hashCode ^= this.AuthenticatorCap.GetHashCode();
			return hashCode ^ this.SoftCap.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Currency);
			num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.AuthenticatorCap);
			num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			uint num1 = (uint)Encoding.UTF8.GetByteCount(this.SoftCap);
			num = num + ProtocolParser.SizeOfUInt32(num1) + num1;
			return num + 3;
		}

		public static CurrencyRestriction ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<CurrencyRestriction>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			CurrencyRestriction.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, CurrencyRestriction instance)
		{
			if (instance.Currency == null)
			{
				throw new ArgumentNullException("Currency", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Currency));
			if (instance.AuthenticatorCap == null)
			{
				throw new ArgumentNullException("AuthenticatorCap", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.AuthenticatorCap));
			if (instance.SoftCap == null)
			{
				throw new ArgumentNullException("SoftCap", "Required by proto specification.");
			}
			stream.WriteByte(26);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.SoftCap));
		}

		public void SetAuthenticatorCap(string val)
		{
			this.AuthenticatorCap = val;
		}

		public void SetCurrency(string val)
		{
			this.Currency = val;
		}

		public void SetSoftCap(string val)
		{
			this.SoftCap = val;
		}
	}
}