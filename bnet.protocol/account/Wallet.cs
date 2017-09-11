using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.account
{
	public class Wallet : IProtoBuf
	{
		public bool HasDescription;

		private string _Description;

		public bool HasState;

		private string _State;

		public bool HasCity;

		private string _City;

		public bool HasPostalCode;

		private string _PostalCode;

		public bool HasPaymentInfo;

		private byte[] _PaymentInfo;

		public bool HasBin;

		private string _Bin;

		public bool HasLocaleId;

		private string _LocaleId;

		public bool HasStreet;

		private string _Street;

		public bool HasFirstName;

		private string _FirstName;

		public bool HasLastName;

		private string _LastName;

		public bool HasBirthDate;

		private ulong _BirthDate;

		public string Bin
		{
			get
			{
				return this._Bin;
			}
			set
			{
				this._Bin = value;
				this.HasBin = value != null;
			}
		}

		public ulong BirthDate
		{
			get
			{
				return this._BirthDate;
			}
			set
			{
				this._BirthDate = value;
				this.HasBirthDate = true;
			}
		}

		public string City
		{
			get
			{
				return this._City;
			}
			set
			{
				this._City = value;
				this.HasCity = value != null;
			}
		}

		public uint CountryId
		{
			get;
			set;
		}

		public string Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				this._Description = value;
				this.HasDescription = value != null;
			}
		}

		public string FirstName
		{
			get
			{
				return this._FirstName;
			}
			set
			{
				this._FirstName = value;
				this.HasFirstName = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public string LastName
		{
			get
			{
				return this._LastName;
			}
			set
			{
				this._LastName = value;
				this.HasLastName = value != null;
			}
		}

		public string LocaleId
		{
			get
			{
				return this._LocaleId;
			}
			set
			{
				this._LocaleId = value;
				this.HasLocaleId = value != null;
			}
		}

		public byte[] PaymentInfo
		{
			get
			{
				return this._PaymentInfo;
			}
			set
			{
				this._PaymentInfo = value;
				this.HasPaymentInfo = value != null;
			}
		}

		public string PostalCode
		{
			get
			{
				return this._PostalCode;
			}
			set
			{
				this._PostalCode = value;
				this.HasPostalCode = value != null;
			}
		}

		public uint Region
		{
			get;
			set;
		}

		public string State
		{
			get
			{
				return this._State;
			}
			set
			{
				this._State = value;
				this.HasState = value != null;
			}
		}

		public string Street
		{
			get
			{
				return this._Street;
			}
			set
			{
				this._Street = value;
				this.HasStreet = value != null;
			}
		}

		public ulong WalletId
		{
			get;
			set;
		}

		public uint WalletType
		{
			get;
			set;
		}

		public Wallet()
		{
		}

		public void Deserialize(Stream stream)
		{
			Wallet.Deserialize(stream, this);
		}

		public static Wallet Deserialize(Stream stream, Wallet instance)
		{
			return Wallet.Deserialize(stream, instance, (long)-1);
		}

		public static Wallet Deserialize(Stream stream, Wallet instance, long limit)
		{
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 == 8)
						{
							instance.Region = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 16)
						{
							instance.WalletId = ProtocolParser.ReadUInt64(stream);
						}
						else if (num1 == 24)
						{
							instance.WalletType = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 34)
						{
							instance.Description = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 40)
						{
							instance.CountryId = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 50)
						{
							instance.State = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 58)
						{
							instance.City = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 66)
						{
							instance.PostalCode = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 74)
						{
							instance.PaymentInfo = ProtocolParser.ReadBytes(stream);
						}
						else if (num1 == 82)
						{
							instance.Bin = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 90)
						{
							instance.LocaleId = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 98)
						{
							instance.Street = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 106)
						{
							instance.FirstName = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 114)
						{
							instance.LastName = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 120)
						{
							instance.BirthDate = ProtocolParser.ReadUInt64(stream);
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

		public static Wallet DeserializeLengthDelimited(Stream stream)
		{
			Wallet wallet = new Wallet();
			Wallet.DeserializeLengthDelimited(stream, wallet);
			return wallet;
		}

		public static Wallet DeserializeLengthDelimited(Stream stream, Wallet instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return Wallet.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			Wallet wallet = obj as Wallet;
			if (wallet == null)
			{
				return false;
			}
			if (!this.Region.Equals(wallet.Region))
			{
				return false;
			}
			if (!this.WalletId.Equals(wallet.WalletId))
			{
				return false;
			}
			if (!this.WalletType.Equals(wallet.WalletType))
			{
				return false;
			}
			if (this.HasDescription != wallet.HasDescription || this.HasDescription && !this.Description.Equals(wallet.Description))
			{
				return false;
			}
			if (!this.CountryId.Equals(wallet.CountryId))
			{
				return false;
			}
			if (this.HasState != wallet.HasState || this.HasState && !this.State.Equals(wallet.State))
			{
				return false;
			}
			if (this.HasCity != wallet.HasCity || this.HasCity && !this.City.Equals(wallet.City))
			{
				return false;
			}
			if (this.HasPostalCode != wallet.HasPostalCode || this.HasPostalCode && !this.PostalCode.Equals(wallet.PostalCode))
			{
				return false;
			}
			if (this.HasPaymentInfo != wallet.HasPaymentInfo || this.HasPaymentInfo && !this.PaymentInfo.Equals(wallet.PaymentInfo))
			{
				return false;
			}
			if (this.HasBin != wallet.HasBin || this.HasBin && !this.Bin.Equals(wallet.Bin))
			{
				return false;
			}
			if (this.HasLocaleId != wallet.HasLocaleId || this.HasLocaleId && !this.LocaleId.Equals(wallet.LocaleId))
			{
				return false;
			}
			if (this.HasStreet != wallet.HasStreet || this.HasStreet && !this.Street.Equals(wallet.Street))
			{
				return false;
			}
			if (this.HasFirstName != wallet.HasFirstName || this.HasFirstName && !this.FirstName.Equals(wallet.FirstName))
			{
				return false;
			}
			if (this.HasLastName != wallet.HasLastName || this.HasLastName && !this.LastName.Equals(wallet.LastName))
			{
				return false;
			}
			if (this.HasBirthDate == wallet.HasBirthDate && (!this.HasBirthDate || this.BirthDate.Equals(wallet.BirthDate)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Region.GetHashCode();
			hashCode ^= this.WalletId.GetHashCode();
			hashCode ^= this.WalletType.GetHashCode();
			if (this.HasDescription)
			{
				hashCode ^= this.Description.GetHashCode();
			}
			hashCode ^= this.CountryId.GetHashCode();
			if (this.HasState)
			{
				hashCode ^= this.State.GetHashCode();
			}
			if (this.HasCity)
			{
				hashCode ^= this.City.GetHashCode();
			}
			if (this.HasPostalCode)
			{
				hashCode ^= this.PostalCode.GetHashCode();
			}
			if (this.HasPaymentInfo)
			{
				hashCode ^= this.PaymentInfo.GetHashCode();
			}
			if (this.HasBin)
			{
				hashCode ^= this.Bin.GetHashCode();
			}
			if (this.HasLocaleId)
			{
				hashCode ^= this.LocaleId.GetHashCode();
			}
			if (this.HasStreet)
			{
				hashCode ^= this.Street.GetHashCode();
			}
			if (this.HasFirstName)
			{
				hashCode ^= this.FirstName.GetHashCode();
			}
			if (this.HasLastName)
			{
				hashCode ^= this.LastName.GetHashCode();
			}
			if (this.HasBirthDate)
			{
				hashCode ^= this.BirthDate.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num += ProtocolParser.SizeOfUInt32(this.Region);
			num += ProtocolParser.SizeOfUInt64(this.WalletId);
			num += ProtocolParser.SizeOfUInt32(this.WalletType);
			if (this.HasDescription)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Description);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			num += ProtocolParser.SizeOfUInt32(this.CountryId);
			if (this.HasState)
			{
				num++;
				uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.State);
				num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			}
			if (this.HasCity)
			{
				num++;
				uint num1 = (uint)Encoding.UTF8.GetByteCount(this.City);
				num = num + ProtocolParser.SizeOfUInt32(num1) + num1;
			}
			if (this.HasPostalCode)
			{
				num++;
				uint byteCount2 = (uint)Encoding.UTF8.GetByteCount(this.PostalCode);
				num = num + ProtocolParser.SizeOfUInt32(byteCount2) + byteCount2;
			}
			if (this.HasPaymentInfo)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32((int)this.PaymentInfo.Length) + (int)this.PaymentInfo.Length;
			}
			if (this.HasBin)
			{
				num++;
				uint num2 = (uint)Encoding.UTF8.GetByteCount(this.Bin);
				num = num + ProtocolParser.SizeOfUInt32(num2) + num2;
			}
			if (this.HasLocaleId)
			{
				num++;
				uint byteCount3 = (uint)Encoding.UTF8.GetByteCount(this.LocaleId);
				num = num + ProtocolParser.SizeOfUInt32(byteCount3) + byteCount3;
			}
			if (this.HasStreet)
			{
				num++;
				uint num3 = (uint)Encoding.UTF8.GetByteCount(this.Street);
				num = num + ProtocolParser.SizeOfUInt32(num3) + num3;
			}
			if (this.HasFirstName)
			{
				num++;
				uint byteCount4 = (uint)Encoding.UTF8.GetByteCount(this.FirstName);
				num = num + ProtocolParser.SizeOfUInt32(byteCount4) + byteCount4;
			}
			if (this.HasLastName)
			{
				num++;
				uint num4 = (uint)Encoding.UTF8.GetByteCount(this.LastName);
				num = num + ProtocolParser.SizeOfUInt32(num4) + num4;
			}
			if (this.HasBirthDate)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.BirthDate);
			}
			num += 4;
			return num;
		}

		public static Wallet ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<Wallet>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			Wallet.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, Wallet instance)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.Region);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt64(stream, instance.WalletId);
			stream.WriteByte(24);
			ProtocolParser.WriteUInt32(stream, instance.WalletType);
			if (instance.HasDescription)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Description));
			}
			stream.WriteByte(40);
			ProtocolParser.WriteUInt32(stream, instance.CountryId);
			if (instance.HasState)
			{
				stream.WriteByte(50);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.State));
			}
			if (instance.HasCity)
			{
				stream.WriteByte(58);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.City));
			}
			if (instance.HasPostalCode)
			{
				stream.WriteByte(66);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.PostalCode));
			}
			if (instance.HasPaymentInfo)
			{
				stream.WriteByte(74);
				ProtocolParser.WriteBytes(stream, instance.PaymentInfo);
			}
			if (instance.HasBin)
			{
				stream.WriteByte(82);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Bin));
			}
			if (instance.HasLocaleId)
			{
				stream.WriteByte(90);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.LocaleId));
			}
			if (instance.HasStreet)
			{
				stream.WriteByte(98);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Street));
			}
			if (instance.HasFirstName)
			{
				stream.WriteByte(106);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.FirstName));
			}
			if (instance.HasLastName)
			{
				stream.WriteByte(114);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.LastName));
			}
			if (instance.HasBirthDate)
			{
				stream.WriteByte(120);
				ProtocolParser.WriteUInt64(stream, instance.BirthDate);
			}
		}

		public void SetBin(string val)
		{
			this.Bin = val;
		}

		public void SetBirthDate(ulong val)
		{
			this.BirthDate = val;
		}

		public void SetCity(string val)
		{
			this.City = val;
		}

		public void SetCountryId(uint val)
		{
			this.CountryId = val;
		}

		public void SetDescription(string val)
		{
			this.Description = val;
		}

		public void SetFirstName(string val)
		{
			this.FirstName = val;
		}

		public void SetLastName(string val)
		{
			this.LastName = val;
		}

		public void SetLocaleId(string val)
		{
			this.LocaleId = val;
		}

		public void SetPaymentInfo(byte[] val)
		{
			this.PaymentInfo = val;
		}

		public void SetPostalCode(string val)
		{
			this.PostalCode = val;
		}

		public void SetRegion(uint val)
		{
			this.Region = val;
		}

		public void SetState(string val)
		{
			this.State = val;
		}

		public void SetStreet(string val)
		{
			this.Street = val;
		}

		public void SetWalletId(ulong val)
		{
			this.WalletId = val;
		}

		public void SetWalletType(uint val)
		{
			this.WalletType = val;
		}
	}
}