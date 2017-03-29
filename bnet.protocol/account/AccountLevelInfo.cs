using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace bnet.protocol.account
{
	public class AccountLevelInfo : IProtoBuf
	{
		private List<AccountLicense> _Licenses = new List<AccountLicense>();

		public bool HasDefaultCurrency;

		private uint _DefaultCurrency;

		public bool HasCountry;

		private string _Country;

		public bool HasPreferredRegion;

		private uint _PreferredRegion;

		public string Country
		{
			get
			{
				return this._Country;
			}
			set
			{
				this._Country = value;
				this.HasCountry = value != null;
			}
		}

		public uint DefaultCurrency
		{
			get
			{
				return this._DefaultCurrency;
			}
			set
			{
				this._DefaultCurrency = value;
				this.HasDefaultCurrency = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public List<AccountLicense> Licenses
		{
			get
			{
				return this._Licenses;
			}
			set
			{
				this._Licenses = value;
			}
		}

		public int LicensesCount
		{
			get
			{
				return this._Licenses.Count;
			}
		}

		public List<AccountLicense> LicensesList
		{
			get
			{
				return this._Licenses;
			}
		}

		public uint PreferredRegion
		{
			get
			{
				return this._PreferredRegion;
			}
			set
			{
				this._PreferredRegion = value;
				this.HasPreferredRegion = true;
			}
		}

		public AccountLevelInfo()
		{
		}

		public void AddLicenses(AccountLicense val)
		{
			this._Licenses.Add(val);
		}

		public void ClearLicenses()
		{
			this._Licenses.Clear();
		}

		public void Deserialize(Stream stream)
		{
			AccountLevelInfo.Deserialize(stream, this);
		}

		public static AccountLevelInfo Deserialize(Stream stream, AccountLevelInfo instance)
		{
			return AccountLevelInfo.Deserialize(stream, instance, (long)-1);
		}

		public static AccountLevelInfo Deserialize(Stream stream, AccountLevelInfo instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			if (instance.Licenses == null)
			{
				instance.Licenses = new List<AccountLicense>();
			}
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 == 26)
						{
							instance.Licenses.Add(AccountLicense.DeserializeLengthDelimited(stream));
						}
						else if (num1 == 37)
						{
							instance.DefaultCurrency = binaryReader.ReadUInt32();
						}
						else if (num1 == 42)
						{
							instance.Country = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 48)
						{
							instance.PreferredRegion = ProtocolParser.ReadUInt32(stream);
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

		public static AccountLevelInfo DeserializeLengthDelimited(Stream stream)
		{
			AccountLevelInfo accountLevelInfo = new AccountLevelInfo();
			AccountLevelInfo.DeserializeLengthDelimited(stream, accountLevelInfo);
			return accountLevelInfo;
		}

		public static AccountLevelInfo DeserializeLengthDelimited(Stream stream, AccountLevelInfo instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return AccountLevelInfo.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			AccountLevelInfo accountLevelInfo = obj as AccountLevelInfo;
			if (accountLevelInfo == null)
			{
				return false;
			}
			if (this.Licenses.Count != accountLevelInfo.Licenses.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Licenses.Count; i++)
			{
				if (!this.Licenses[i].Equals(accountLevelInfo.Licenses[i]))
				{
					return false;
				}
			}
			if (this.HasDefaultCurrency != accountLevelInfo.HasDefaultCurrency || this.HasDefaultCurrency && !this.DefaultCurrency.Equals(accountLevelInfo.DefaultCurrency))
			{
				return false;
			}
			if (this.HasCountry != accountLevelInfo.HasCountry || this.HasCountry && !this.Country.Equals(accountLevelInfo.Country))
			{
				return false;
			}
			if (this.HasPreferredRegion == accountLevelInfo.HasPreferredRegion && (!this.HasPreferredRegion || this.PreferredRegion.Equals(accountLevelInfo.PreferredRegion)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (AccountLicense license in this.Licenses)
			{
				hashCode = hashCode ^ license.GetHashCode();
			}
			if (this.HasDefaultCurrency)
			{
				hashCode = hashCode ^ this.DefaultCurrency.GetHashCode();
			}
			if (this.HasCountry)
			{
				hashCode = hashCode ^ this.Country.GetHashCode();
			}
			if (this.HasPreferredRegion)
			{
				hashCode = hashCode ^ this.PreferredRegion.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Licenses.Count > 0)
			{
				foreach (AccountLicense license in this.Licenses)
				{
					num++;
					uint serializedSize = license.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			if (this.HasDefaultCurrency)
			{
				num++;
				num = num + 4;
			}
			if (this.HasCountry)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Country);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasPreferredRegion)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.PreferredRegion);
			}
			return num;
		}

		public static AccountLevelInfo ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<AccountLevelInfo>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			AccountLevelInfo.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, AccountLevelInfo instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.Licenses.Count > 0)
			{
				foreach (AccountLicense license in instance.Licenses)
				{
					stream.WriteByte(26);
					ProtocolParser.WriteUInt32(stream, license.GetSerializedSize());
					AccountLicense.Serialize(stream, license);
				}
			}
			if (instance.HasDefaultCurrency)
			{
				stream.WriteByte(37);
				binaryWriter.Write(instance.DefaultCurrency);
			}
			if (instance.HasCountry)
			{
				stream.WriteByte(42);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Country));
			}
			if (instance.HasPreferredRegion)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteUInt32(stream, instance.PreferredRegion);
			}
		}

		public void SetCountry(string val)
		{
			this.Country = val;
		}

		public void SetDefaultCurrency(uint val)
		{
			this.DefaultCurrency = val;
		}

		public void SetLicenses(List<AccountLicense> val)
		{
			this.Licenses = val;
		}

		public void SetPreferredRegion(uint val)
		{
			this.PreferredRegion = val;
		}
	}
}