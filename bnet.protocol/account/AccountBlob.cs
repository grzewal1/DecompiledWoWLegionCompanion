using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.account
{
	public class AccountBlob : IProtoBuf
	{
		private List<string> _Email = new List<string>();

		public bool HasSecureRelease;

		private ulong _SecureRelease;

		public bool HasWhitelistStart;

		private ulong _WhitelistStart;

		public bool HasWhitelistEnd;

		private ulong _WhitelistEnd;

		private List<AccountLicense> _Licenses = new List<AccountLicense>();

		private List<AccountCredential> _Credentials = new List<AccountCredential>();

		private List<GameAccountLink> _AccountLinks = new List<GameAccountLink>();

		public bool HasBattleTag;

		private string _BattleTag;

		public bool HasDefaultCurrency;

		private uint _DefaultCurrency;

		public bool HasLegalRegion;

		private uint _LegalRegion;

		public bool HasLegalLocale;

		private uint _LegalLocale;

		public bool HasParentalControlInfo;

		private bnet.protocol.account.ParentalControlInfo _ParentalControlInfo;

		public bool HasCountry;

		private string _Country;

		public bool HasPreferredRegion;

		private uint _PreferredRegion;

		public List<GameAccountLink> AccountLinks
		{
			get
			{
				return this._AccountLinks;
			}
			set
			{
				this._AccountLinks = value;
			}
		}

		public int AccountLinksCount
		{
			get
			{
				return this._AccountLinks.Count;
			}
		}

		public List<GameAccountLink> AccountLinksList
		{
			get
			{
				return this._AccountLinks;
			}
		}

		public string BattleTag
		{
			get
			{
				return this._BattleTag;
			}
			set
			{
				this._BattleTag = value;
				this.HasBattleTag = value != null;
			}
		}

		public ulong CacheExpiration
		{
			get;
			set;
		}

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

		public List<AccountCredential> Credentials
		{
			get
			{
				return this._Credentials;
			}
			set
			{
				this._Credentials = value;
			}
		}

		public int CredentialsCount
		{
			get
			{
				return this._Credentials.Count;
			}
		}

		public List<AccountCredential> CredentialsList
		{
			get
			{
				return this._Credentials;
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

		public List<string> Email
		{
			get
			{
				return this._Email;
			}
			set
			{
				this._Email = value;
			}
		}

		public int EmailCount
		{
			get
			{
				return this._Email.Count;
			}
		}

		public List<string> EmailList
		{
			get
			{
				return this._Email;
			}
		}

		public ulong Flags
		{
			get;
			set;
		}

		public string FullName
		{
			get;
			set;
		}

		public uint Id
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

		public uint LegalLocale
		{
			get
			{
				return this._LegalLocale;
			}
			set
			{
				this._LegalLocale = value;
				this.HasLegalLocale = true;
			}
		}

		public uint LegalRegion
		{
			get
			{
				return this._LegalRegion;
			}
			set
			{
				this._LegalRegion = value;
				this.HasLegalRegion = true;
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

		public bnet.protocol.account.ParentalControlInfo ParentalControlInfo
		{
			get
			{
				return this._ParentalControlInfo;
			}
			set
			{
				this._ParentalControlInfo = value;
				this.HasParentalControlInfo = value != null;
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

		public uint Region
		{
			get;
			set;
		}

		public ulong SecureRelease
		{
			get
			{
				return this._SecureRelease;
			}
			set
			{
				this._SecureRelease = value;
				this.HasSecureRelease = true;
			}
		}

		public ulong WhitelistEnd
		{
			get
			{
				return this._WhitelistEnd;
			}
			set
			{
				this._WhitelistEnd = value;
				this.HasWhitelistEnd = true;
			}
		}

		public ulong WhitelistStart
		{
			get
			{
				return this._WhitelistStart;
			}
			set
			{
				this._WhitelistStart = value;
				this.HasWhitelistStart = true;
			}
		}

		public AccountBlob()
		{
		}

		public void AddAccountLinks(GameAccountLink val)
		{
			this._AccountLinks.Add(val);
		}

		public void AddCredentials(AccountCredential val)
		{
			this._Credentials.Add(val);
		}

		public void AddEmail(string val)
		{
			this._Email.Add(val);
		}

		public void AddLicenses(AccountLicense val)
		{
			this._Licenses.Add(val);
		}

		public void ClearAccountLinks()
		{
			this._AccountLinks.Clear();
		}

		public void ClearCredentials()
		{
			this._Credentials.Clear();
		}

		public void ClearEmail()
		{
			this._Email.Clear();
		}

		public void ClearLicenses()
		{
			this._Licenses.Clear();
		}

		public void Deserialize(Stream stream)
		{
			AccountBlob.Deserialize(stream, this);
		}

		public static AccountBlob Deserialize(Stream stream, AccountBlob instance)
		{
			return AccountBlob.Deserialize(stream, instance, (long)-1);
		}

		public static AccountBlob Deserialize(Stream stream, AccountBlob instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			if (instance.Email == null)
			{
				instance.Email = new List<string>();
			}
			if (instance.Licenses == null)
			{
				instance.Licenses = new List<AccountLicense>();
			}
			if (instance.Credentials == null)
			{
				instance.Credentials = new List<AccountCredential>();
			}
			if (instance.AccountLinks == null)
			{
				instance.AccountLinks = new List<GameAccountLink>();
			}
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						switch (num1)
						{
							case 21:
							{
								instance.Id = binaryReader.ReadUInt32();
								continue;
							}
							case 24:
							{
								instance.Region = ProtocolParser.ReadUInt32(stream);
								continue;
							}
							default:
							{
								if (num1 == 34)
								{
									instance.Email.Add(ProtocolParser.ReadString(stream));
									continue;
								}
								else if (num1 == 40)
								{
									instance.Flags = ProtocolParser.ReadUInt64(stream);
									continue;
								}
								else if (num1 == 48)
								{
									instance.SecureRelease = ProtocolParser.ReadUInt64(stream);
									continue;
								}
								else if (num1 == 56)
								{
									instance.WhitelistStart = ProtocolParser.ReadUInt64(stream);
									continue;
								}
								else if (num1 == 64)
								{
									instance.WhitelistEnd = ProtocolParser.ReadUInt64(stream);
									continue;
								}
								else if (num1 == 82)
								{
									instance.FullName = ProtocolParser.ReadString(stream);
									continue;
								}
								else
								{
									Key key = ProtocolParser.ReadKey((byte)num, stream);
									uint field = key.Field;
									switch (field)
									{
										case 20:
										{
											if (key.WireType == Wire.LengthDelimited)
											{
												instance.Licenses.Add(AccountLicense.DeserializeLengthDelimited(stream));
												continue;
											}
											else
											{
												break;
											}
										}
										case 21:
										{
											if (key.WireType == Wire.LengthDelimited)
											{
												instance.Credentials.Add(AccountCredential.DeserializeLengthDelimited(stream));
												continue;
											}
											else
											{
												break;
											}
										}
										case 22:
										{
											if (key.WireType == Wire.LengthDelimited)
											{
												instance.AccountLinks.Add(GameAccountLink.DeserializeLengthDelimited(stream));
												continue;
											}
											else
											{
												break;
											}
										}
										case 23:
										{
											if (key.WireType == Wire.LengthDelimited)
											{
												instance.BattleTag = ProtocolParser.ReadString(stream);
												continue;
											}
											else
											{
												break;
											}
										}
										case 25:
										{
											if (key.WireType == Wire.Fixed32)
											{
												instance.DefaultCurrency = binaryReader.ReadUInt32();
												continue;
											}
											else
											{
												break;
											}
										}
										case 26:
										{
											if (key.WireType == Wire.Varint)
											{
												instance.LegalRegion = ProtocolParser.ReadUInt32(stream);
												continue;
											}
											else
											{
												break;
											}
										}
										case 27:
										{
											if (key.WireType == Wire.Fixed32)
											{
												instance.LegalLocale = binaryReader.ReadUInt32();
												continue;
											}
											else
											{
												break;
											}
										}
										case 30:
										{
											if (key.WireType == Wire.Varint)
											{
												instance.CacheExpiration = ProtocolParser.ReadUInt64(stream);
												continue;
											}
											else
											{
												break;
											}
										}
										case 31:
										{
											if (key.WireType == Wire.LengthDelimited)
											{
												if (instance.ParentalControlInfo != null)
												{
													bnet.protocol.account.ParentalControlInfo.DeserializeLengthDelimited(stream, instance.ParentalControlInfo);
												}
												else
												{
													instance.ParentalControlInfo = bnet.protocol.account.ParentalControlInfo.DeserializeLengthDelimited(stream);
												}
												continue;
											}
											else
											{
												break;
											}
										}
										case 32:
										{
											if (key.WireType == Wire.LengthDelimited)
											{
												instance.Country = ProtocolParser.ReadString(stream);
												continue;
											}
											else
											{
												break;
											}
										}
										case 33:
										{
											if (key.WireType == Wire.Varint)
											{
												instance.PreferredRegion = ProtocolParser.ReadUInt32(stream);
												continue;
											}
											else
											{
												break;
											}
										}
										default:
										{
											if (field == 0)
											{
												throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
											}
											ProtocolParser.SkipKey(stream, key);
											break;
										}
									}
									continue;
								}
							}
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

		public static AccountBlob DeserializeLengthDelimited(Stream stream)
		{
			AccountBlob accountBlob = new AccountBlob();
			AccountBlob.DeserializeLengthDelimited(stream, accountBlob);
			return accountBlob;
		}

		public static AccountBlob DeserializeLengthDelimited(Stream stream, AccountBlob instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return AccountBlob.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			AccountBlob accountBlob = obj as AccountBlob;
			if (accountBlob == null)
			{
				return false;
			}
			if (!this.Id.Equals(accountBlob.Id))
			{
				return false;
			}
			if (!this.Region.Equals(accountBlob.Region))
			{
				return false;
			}
			if (this.Email.Count != accountBlob.Email.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Email.Count; i++)
			{
				if (!this.Email[i].Equals(accountBlob.Email[i]))
				{
					return false;
				}
			}
			if (!this.Flags.Equals(accountBlob.Flags))
			{
				return false;
			}
			if (this.HasSecureRelease != accountBlob.HasSecureRelease || this.HasSecureRelease && !this.SecureRelease.Equals(accountBlob.SecureRelease))
			{
				return false;
			}
			if (this.HasWhitelistStart != accountBlob.HasWhitelistStart || this.HasWhitelistStart && !this.WhitelistStart.Equals(accountBlob.WhitelistStart))
			{
				return false;
			}
			if (this.HasWhitelistEnd != accountBlob.HasWhitelistEnd || this.HasWhitelistEnd && !this.WhitelistEnd.Equals(accountBlob.WhitelistEnd))
			{
				return false;
			}
			if (!this.FullName.Equals(accountBlob.FullName))
			{
				return false;
			}
			if (this.Licenses.Count != accountBlob.Licenses.Count)
			{
				return false;
			}
			for (int j = 0; j < this.Licenses.Count; j++)
			{
				if (!this.Licenses[j].Equals(accountBlob.Licenses[j]))
				{
					return false;
				}
			}
			if (this.Credentials.Count != accountBlob.Credentials.Count)
			{
				return false;
			}
			for (int k = 0; k < this.Credentials.Count; k++)
			{
				if (!this.Credentials[k].Equals(accountBlob.Credentials[k]))
				{
					return false;
				}
			}
			if (this.AccountLinks.Count != accountBlob.AccountLinks.Count)
			{
				return false;
			}
			for (int l = 0; l < this.AccountLinks.Count; l++)
			{
				if (!this.AccountLinks[l].Equals(accountBlob.AccountLinks[l]))
				{
					return false;
				}
			}
			if (this.HasBattleTag != accountBlob.HasBattleTag || this.HasBattleTag && !this.BattleTag.Equals(accountBlob.BattleTag))
			{
				return false;
			}
			if (this.HasDefaultCurrency != accountBlob.HasDefaultCurrency || this.HasDefaultCurrency && !this.DefaultCurrency.Equals(accountBlob.DefaultCurrency))
			{
				return false;
			}
			if (this.HasLegalRegion != accountBlob.HasLegalRegion || this.HasLegalRegion && !this.LegalRegion.Equals(accountBlob.LegalRegion))
			{
				return false;
			}
			if (this.HasLegalLocale != accountBlob.HasLegalLocale || this.HasLegalLocale && !this.LegalLocale.Equals(accountBlob.LegalLocale))
			{
				return false;
			}
			if (!this.CacheExpiration.Equals(accountBlob.CacheExpiration))
			{
				return false;
			}
			if (this.HasParentalControlInfo != accountBlob.HasParentalControlInfo || this.HasParentalControlInfo && !this.ParentalControlInfo.Equals(accountBlob.ParentalControlInfo))
			{
				return false;
			}
			if (this.HasCountry != accountBlob.HasCountry || this.HasCountry && !this.Country.Equals(accountBlob.Country))
			{
				return false;
			}
			if (this.HasPreferredRegion == accountBlob.HasPreferredRegion && (!this.HasPreferredRegion || this.PreferredRegion.Equals(accountBlob.PreferredRegion)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Id.GetHashCode();
			hashCode ^= this.Region.GetHashCode();
			foreach (string email in this.Email)
			{
				hashCode ^= email.GetHashCode();
			}
			hashCode ^= this.Flags.GetHashCode();
			if (this.HasSecureRelease)
			{
				hashCode ^= this.SecureRelease.GetHashCode();
			}
			if (this.HasWhitelistStart)
			{
				hashCode ^= this.WhitelistStart.GetHashCode();
			}
			if (this.HasWhitelistEnd)
			{
				hashCode ^= this.WhitelistEnd.GetHashCode();
			}
			hashCode ^= this.FullName.GetHashCode();
			foreach (AccountLicense license in this.Licenses)
			{
				hashCode ^= license.GetHashCode();
			}
			foreach (AccountCredential credential in this.Credentials)
			{
				hashCode ^= credential.GetHashCode();
			}
			foreach (GameAccountLink accountLink in this.AccountLinks)
			{
				hashCode ^= accountLink.GetHashCode();
			}
			if (this.HasBattleTag)
			{
				hashCode ^= this.BattleTag.GetHashCode();
			}
			if (this.HasDefaultCurrency)
			{
				hashCode ^= this.DefaultCurrency.GetHashCode();
			}
			if (this.HasLegalRegion)
			{
				hashCode ^= this.LegalRegion.GetHashCode();
			}
			if (this.HasLegalLocale)
			{
				hashCode ^= this.LegalLocale.GetHashCode();
			}
			hashCode ^= this.CacheExpiration.GetHashCode();
			if (this.HasParentalControlInfo)
			{
				hashCode ^= this.ParentalControlInfo.GetHashCode();
			}
			if (this.HasCountry)
			{
				hashCode ^= this.Country.GetHashCode();
			}
			if (this.HasPreferredRegion)
			{
				hashCode ^= this.PreferredRegion.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num += 4;
			num += ProtocolParser.SizeOfUInt32(this.Region);
			if (this.Email.Count > 0)
			{
				foreach (string email in this.Email)
				{
					num++;
					uint byteCount = (uint)Encoding.UTF8.GetByteCount(email);
					num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
				}
			}
			num += ProtocolParser.SizeOfUInt64(this.Flags);
			if (this.HasSecureRelease)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.SecureRelease);
			}
			if (this.HasWhitelistStart)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.WhitelistStart);
			}
			if (this.HasWhitelistEnd)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.WhitelistEnd);
			}
			uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.FullName);
			num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			if (this.Licenses.Count > 0)
			{
				foreach (AccountLicense license in this.Licenses)
				{
					num += 2;
					uint serializedSize = license.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			if (this.Credentials.Count > 0)
			{
				foreach (AccountCredential credential in this.Credentials)
				{
					num += 2;
					uint serializedSize1 = credential.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			if (this.AccountLinks.Count > 0)
			{
				foreach (GameAccountLink accountLink in this.AccountLinks)
				{
					num += 2;
					uint num1 = accountLink.GetSerializedSize();
					num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
				}
			}
			if (this.HasBattleTag)
			{
				num += 2;
				uint byteCount2 = (uint)Encoding.UTF8.GetByteCount(this.BattleTag);
				num = num + ProtocolParser.SizeOfUInt32(byteCount2) + byteCount2;
			}
			if (this.HasDefaultCurrency)
			{
				num += 2;
				num += 4;
			}
			if (this.HasLegalRegion)
			{
				num += 2;
				num += ProtocolParser.SizeOfUInt32(this.LegalRegion);
			}
			if (this.HasLegalLocale)
			{
				num += 2;
				num += 4;
			}
			num += ProtocolParser.SizeOfUInt64(this.CacheExpiration);
			if (this.HasParentalControlInfo)
			{
				num += 2;
				uint serializedSize2 = this.ParentalControlInfo.GetSerializedSize();
				num = num + serializedSize2 + ProtocolParser.SizeOfUInt32(serializedSize2);
			}
			if (this.HasCountry)
			{
				num += 2;
				uint num2 = (uint)Encoding.UTF8.GetByteCount(this.Country);
				num = num + ProtocolParser.SizeOfUInt32(num2) + num2;
			}
			if (this.HasPreferredRegion)
			{
				num += 2;
				num += ProtocolParser.SizeOfUInt32(this.PreferredRegion);
			}
			num += 6;
			return num;
		}

		public static AccountBlob ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<AccountBlob>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			AccountBlob.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, AccountBlob instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			stream.WriteByte(21);
			binaryWriter.Write(instance.Id);
			stream.WriteByte(24);
			ProtocolParser.WriteUInt32(stream, instance.Region);
			if (instance.Email.Count > 0)
			{
				foreach (string email in instance.Email)
				{
					stream.WriteByte(34);
					ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(email));
				}
			}
			stream.WriteByte(40);
			ProtocolParser.WriteUInt64(stream, instance.Flags);
			if (instance.HasSecureRelease)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteUInt64(stream, instance.SecureRelease);
			}
			if (instance.HasWhitelistStart)
			{
				stream.WriteByte(56);
				ProtocolParser.WriteUInt64(stream, instance.WhitelistStart);
			}
			if (instance.HasWhitelistEnd)
			{
				stream.WriteByte(64);
				ProtocolParser.WriteUInt64(stream, instance.WhitelistEnd);
			}
			if (instance.FullName == null)
			{
				throw new ArgumentNullException("FullName", "Required by proto specification.");
			}
			stream.WriteByte(82);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.FullName));
			if (instance.Licenses.Count > 0)
			{
				foreach (AccountLicense license in instance.Licenses)
				{
					stream.WriteByte(162);
					stream.WriteByte(1);
					ProtocolParser.WriteUInt32(stream, license.GetSerializedSize());
					AccountLicense.Serialize(stream, license);
				}
			}
			if (instance.Credentials.Count > 0)
			{
				foreach (AccountCredential credential in instance.Credentials)
				{
					stream.WriteByte(170);
					stream.WriteByte(1);
					ProtocolParser.WriteUInt32(stream, credential.GetSerializedSize());
					AccountCredential.Serialize(stream, credential);
				}
			}
			if (instance.AccountLinks.Count > 0)
			{
				foreach (GameAccountLink accountLink in instance.AccountLinks)
				{
					stream.WriteByte(178);
					stream.WriteByte(1);
					ProtocolParser.WriteUInt32(stream, accountLink.GetSerializedSize());
					GameAccountLink.Serialize(stream, accountLink);
				}
			}
			if (instance.HasBattleTag)
			{
				stream.WriteByte(186);
				stream.WriteByte(1);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.BattleTag));
			}
			if (instance.HasDefaultCurrency)
			{
				stream.WriteByte(205);
				stream.WriteByte(1);
				binaryWriter.Write(instance.DefaultCurrency);
			}
			if (instance.HasLegalRegion)
			{
				stream.WriteByte(208);
				stream.WriteByte(1);
				ProtocolParser.WriteUInt32(stream, instance.LegalRegion);
			}
			if (instance.HasLegalLocale)
			{
				stream.WriteByte(221);
				stream.WriteByte(1);
				binaryWriter.Write(instance.LegalLocale);
			}
			stream.WriteByte(240);
			stream.WriteByte(1);
			ProtocolParser.WriteUInt64(stream, instance.CacheExpiration);
			if (instance.HasParentalControlInfo)
			{
				stream.WriteByte(250);
				stream.WriteByte(1);
				ProtocolParser.WriteUInt32(stream, instance.ParentalControlInfo.GetSerializedSize());
				bnet.protocol.account.ParentalControlInfo.Serialize(stream, instance.ParentalControlInfo);
			}
			if (instance.HasCountry)
			{
				stream.WriteByte(130);
				stream.WriteByte(2);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Country));
			}
			if (instance.HasPreferredRegion)
			{
				stream.WriteByte(136);
				stream.WriteByte(2);
				ProtocolParser.WriteUInt32(stream, instance.PreferredRegion);
			}
		}

		public void SetAccountLinks(List<GameAccountLink> val)
		{
			this.AccountLinks = val;
		}

		public void SetBattleTag(string val)
		{
			this.BattleTag = val;
		}

		public void SetCacheExpiration(ulong val)
		{
			this.CacheExpiration = val;
		}

		public void SetCountry(string val)
		{
			this.Country = val;
		}

		public void SetCredentials(List<AccountCredential> val)
		{
			this.Credentials = val;
		}

		public void SetDefaultCurrency(uint val)
		{
			this.DefaultCurrency = val;
		}

		public void SetEmail(List<string> val)
		{
			this.Email = val;
		}

		public void SetFlags(ulong val)
		{
			this.Flags = val;
		}

		public void SetFullName(string val)
		{
			this.FullName = val;
		}

		public void SetId(uint val)
		{
			this.Id = val;
		}

		public void SetLegalLocale(uint val)
		{
			this.LegalLocale = val;
		}

		public void SetLegalRegion(uint val)
		{
			this.LegalRegion = val;
		}

		public void SetLicenses(List<AccountLicense> val)
		{
			this.Licenses = val;
		}

		public void SetParentalControlInfo(bnet.protocol.account.ParentalControlInfo val)
		{
			this.ParentalControlInfo = val;
		}

		public void SetPreferredRegion(uint val)
		{
			this.PreferredRegion = val;
		}

		public void SetRegion(uint val)
		{
			this.Region = val;
		}

		public void SetSecureRelease(ulong val)
		{
			this.SecureRelease = val;
		}

		public void SetWhitelistEnd(ulong val)
		{
			this.WhitelistEnd = val;
		}

		public void SetWhitelistStart(ulong val)
		{
			this.WhitelistStart = val;
		}
	}
}