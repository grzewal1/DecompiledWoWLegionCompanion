using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.account
{
	public class GameAccountBlob : IProtoBuf
	{
		public bool HasName;

		private string _Name;

		public bool HasRealmPermissions;

		private uint _RealmPermissions;

		public bool HasFlags;

		private ulong _Flags;

		public bool HasBillingFlags;

		private uint _BillingFlags;

		public bool HasSubscriptionExpiration;

		private ulong _SubscriptionExpiration;

		public bool HasUnitsRemaining;

		private uint _UnitsRemaining;

		public bool HasStatusExpiration;

		private ulong _StatusExpiration;

		public bool HasBoxLevel;

		private uint _BoxLevel;

		public bool HasBoxLevelExpiration;

		private ulong _BoxLevelExpiration;

		private List<AccountLicense> _Licenses = new List<AccountLicense>();

		public uint BillingFlags
		{
			get
			{
				return this._BillingFlags;
			}
			set
			{
				this._BillingFlags = value;
				this.HasBillingFlags = true;
			}
		}

		public uint BoxLevel
		{
			get
			{
				return this._BoxLevel;
			}
			set
			{
				this._BoxLevel = value;
				this.HasBoxLevel = true;
			}
		}

		public ulong BoxLevelExpiration
		{
			get
			{
				return this._BoxLevelExpiration;
			}
			set
			{
				this._BoxLevelExpiration = value;
				this.HasBoxLevelExpiration = true;
			}
		}

		public ulong CacheExpiration
		{
			get;
			set;
		}

		public ulong Flags
		{
			get
			{
				return this._Flags;
			}
			set
			{
				this._Flags = value;
				this.HasFlags = true;
			}
		}

		public GameAccountHandle GameAccount
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

		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				this._Name = value;
				this.HasName = value != null;
			}
		}

		public uint RealmPermissions
		{
			get
			{
				return this._RealmPermissions;
			}
			set
			{
				this._RealmPermissions = value;
				this.HasRealmPermissions = true;
			}
		}

		public uint Status
		{
			get;
			set;
		}

		public ulong StatusExpiration
		{
			get
			{
				return this._StatusExpiration;
			}
			set
			{
				this._StatusExpiration = value;
				this.HasStatusExpiration = true;
			}
		}

		public ulong SubscriptionExpiration
		{
			get
			{
				return this._SubscriptionExpiration;
			}
			set
			{
				this._SubscriptionExpiration = value;
				this.HasSubscriptionExpiration = true;
			}
		}

		public uint UnitsRemaining
		{
			get
			{
				return this._UnitsRemaining;
			}
			set
			{
				this._UnitsRemaining = value;
				this.HasUnitsRemaining = true;
			}
		}

		public GameAccountBlob()
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
			GameAccountBlob.Deserialize(stream, this);
		}

		public static GameAccountBlob Deserialize(Stream stream, GameAccountBlob instance)
		{
			return GameAccountBlob.Deserialize(stream, instance, (long)-1);
		}

		public static GameAccountBlob Deserialize(Stream stream, GameAccountBlob instance, long limit)
		{
			instance.Name = string.Empty;
			instance.RealmPermissions = 0;
			instance.Flags = (ulong)0;
			instance.BillingFlags = 0;
			if (instance.Licenses == null)
			{
				instance.Licenses = new List<AccountLicense>();
			}
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
						if (instance.GameAccount != null)
						{
							GameAccountHandle.DeserializeLengthDelimited(stream, instance.GameAccount);
						}
						else
						{
							instance.GameAccount = GameAccountHandle.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 18)
					{
						instance.Name = ProtocolParser.ReadString(stream);
					}
					else if (num == 24)
					{
						instance.RealmPermissions = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 32)
					{
						instance.Status = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 40)
					{
						instance.Flags = ProtocolParser.ReadUInt64(stream);
					}
					else if (num == 48)
					{
						instance.BillingFlags = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 56)
					{
						instance.CacheExpiration = ProtocolParser.ReadUInt64(stream);
					}
					else if (num == 80)
					{
						instance.SubscriptionExpiration = ProtocolParser.ReadUInt64(stream);
					}
					else if (num == 88)
					{
						instance.UnitsRemaining = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 96)
					{
						instance.StatusExpiration = ProtocolParser.ReadUInt64(stream);
					}
					else if (num == 104)
					{
						instance.BoxLevel = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 112)
					{
						instance.BoxLevelExpiration = ProtocolParser.ReadUInt64(stream);
					}
					else
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						uint field = key.Field;
						if (field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						if (field != 20)
						{
							ProtocolParser.SkipKey(stream, key);
						}
						else if (key.WireType == Wire.LengthDelimited)
						{
							instance.Licenses.Add(AccountLicense.DeserializeLengthDelimited(stream));
							continue;
						}
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

		public static GameAccountBlob DeserializeLengthDelimited(Stream stream)
		{
			GameAccountBlob gameAccountBlob = new GameAccountBlob();
			GameAccountBlob.DeserializeLengthDelimited(stream, gameAccountBlob);
			return gameAccountBlob;
		}

		public static GameAccountBlob DeserializeLengthDelimited(Stream stream, GameAccountBlob instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GameAccountBlob.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameAccountBlob gameAccountBlob = obj as GameAccountBlob;
			if (gameAccountBlob == null)
			{
				return false;
			}
			if (!this.GameAccount.Equals(gameAccountBlob.GameAccount))
			{
				return false;
			}
			if (this.HasName != gameAccountBlob.HasName || this.HasName && !this.Name.Equals(gameAccountBlob.Name))
			{
				return false;
			}
			if (this.HasRealmPermissions != gameAccountBlob.HasRealmPermissions || this.HasRealmPermissions && !this.RealmPermissions.Equals(gameAccountBlob.RealmPermissions))
			{
				return false;
			}
			if (!this.Status.Equals(gameAccountBlob.Status))
			{
				return false;
			}
			if (this.HasFlags != gameAccountBlob.HasFlags || this.HasFlags && !this.Flags.Equals(gameAccountBlob.Flags))
			{
				return false;
			}
			if (this.HasBillingFlags != gameAccountBlob.HasBillingFlags || this.HasBillingFlags && !this.BillingFlags.Equals(gameAccountBlob.BillingFlags))
			{
				return false;
			}
			if (!this.CacheExpiration.Equals(gameAccountBlob.CacheExpiration))
			{
				return false;
			}
			if (this.HasSubscriptionExpiration != gameAccountBlob.HasSubscriptionExpiration || this.HasSubscriptionExpiration && !this.SubscriptionExpiration.Equals(gameAccountBlob.SubscriptionExpiration))
			{
				return false;
			}
			if (this.HasUnitsRemaining != gameAccountBlob.HasUnitsRemaining || this.HasUnitsRemaining && !this.UnitsRemaining.Equals(gameAccountBlob.UnitsRemaining))
			{
				return false;
			}
			if (this.HasStatusExpiration != gameAccountBlob.HasStatusExpiration || this.HasStatusExpiration && !this.StatusExpiration.Equals(gameAccountBlob.StatusExpiration))
			{
				return false;
			}
			if (this.HasBoxLevel != gameAccountBlob.HasBoxLevel || this.HasBoxLevel && !this.BoxLevel.Equals(gameAccountBlob.BoxLevel))
			{
				return false;
			}
			if (this.HasBoxLevelExpiration != gameAccountBlob.HasBoxLevelExpiration || this.HasBoxLevelExpiration && !this.BoxLevelExpiration.Equals(gameAccountBlob.BoxLevelExpiration))
			{
				return false;
			}
			if (this.Licenses.Count != gameAccountBlob.Licenses.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Licenses.Count; i++)
			{
				if (!this.Licenses[i].Equals(gameAccountBlob.Licenses[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.GameAccount.GetHashCode();
			if (this.HasName)
			{
				hashCode ^= this.Name.GetHashCode();
			}
			if (this.HasRealmPermissions)
			{
				hashCode ^= this.RealmPermissions.GetHashCode();
			}
			hashCode ^= this.Status.GetHashCode();
			if (this.HasFlags)
			{
				hashCode ^= this.Flags.GetHashCode();
			}
			if (this.HasBillingFlags)
			{
				hashCode ^= this.BillingFlags.GetHashCode();
			}
			hashCode ^= this.CacheExpiration.GetHashCode();
			if (this.HasSubscriptionExpiration)
			{
				hashCode ^= this.SubscriptionExpiration.GetHashCode();
			}
			if (this.HasUnitsRemaining)
			{
				hashCode ^= this.UnitsRemaining.GetHashCode();
			}
			if (this.HasStatusExpiration)
			{
				hashCode ^= this.StatusExpiration.GetHashCode();
			}
			if (this.HasBoxLevel)
			{
				hashCode ^= this.BoxLevel.GetHashCode();
			}
			if (this.HasBoxLevelExpiration)
			{
				hashCode ^= this.BoxLevelExpiration.GetHashCode();
			}
			foreach (AccountLicense license in this.Licenses)
			{
				hashCode ^= license.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.GameAccount.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasName)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Name);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasRealmPermissions)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.RealmPermissions);
			}
			num += ProtocolParser.SizeOfUInt32(this.Status);
			if (this.HasFlags)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.Flags);
			}
			if (this.HasBillingFlags)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.BillingFlags);
			}
			num += ProtocolParser.SizeOfUInt64(this.CacheExpiration);
			if (this.HasSubscriptionExpiration)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.SubscriptionExpiration);
			}
			if (this.HasUnitsRemaining)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.UnitsRemaining);
			}
			if (this.HasStatusExpiration)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.StatusExpiration);
			}
			if (this.HasBoxLevel)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.BoxLevel);
			}
			if (this.HasBoxLevelExpiration)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.BoxLevelExpiration);
			}
			if (this.Licenses.Count > 0)
			{
				foreach (AccountLicense license in this.Licenses)
				{
					num += 2;
					uint serializedSize1 = license.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			num += 3;
			return num;
		}

		public static GameAccountBlob ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameAccountBlob>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameAccountBlob.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameAccountBlob instance)
		{
			if (instance.GameAccount == null)
			{
				throw new ArgumentNullException("GameAccount", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.GameAccount.GetSerializedSize());
			GameAccountHandle.Serialize(stream, instance.GameAccount);
			if (instance.HasName)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Name));
			}
			if (instance.HasRealmPermissions)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.RealmPermissions);
			}
			stream.WriteByte(32);
			ProtocolParser.WriteUInt32(stream, instance.Status);
			if (instance.HasFlags)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteUInt64(stream, instance.Flags);
			}
			if (instance.HasBillingFlags)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteUInt32(stream, instance.BillingFlags);
			}
			stream.WriteByte(56);
			ProtocolParser.WriteUInt64(stream, instance.CacheExpiration);
			if (instance.HasSubscriptionExpiration)
			{
				stream.WriteByte(80);
				ProtocolParser.WriteUInt64(stream, instance.SubscriptionExpiration);
			}
			if (instance.HasUnitsRemaining)
			{
				stream.WriteByte(88);
				ProtocolParser.WriteUInt32(stream, instance.UnitsRemaining);
			}
			if (instance.HasStatusExpiration)
			{
				stream.WriteByte(96);
				ProtocolParser.WriteUInt64(stream, instance.StatusExpiration);
			}
			if (instance.HasBoxLevel)
			{
				stream.WriteByte(104);
				ProtocolParser.WriteUInt32(stream, instance.BoxLevel);
			}
			if (instance.HasBoxLevelExpiration)
			{
				stream.WriteByte(112);
				ProtocolParser.WriteUInt64(stream, instance.BoxLevelExpiration);
			}
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
		}

		public void SetBillingFlags(uint val)
		{
			this.BillingFlags = val;
		}

		public void SetBoxLevel(uint val)
		{
			this.BoxLevel = val;
		}

		public void SetBoxLevelExpiration(ulong val)
		{
			this.BoxLevelExpiration = val;
		}

		public void SetCacheExpiration(ulong val)
		{
			this.CacheExpiration = val;
		}

		public void SetFlags(ulong val)
		{
			this.Flags = val;
		}

		public void SetGameAccount(GameAccountHandle val)
		{
			this.GameAccount = val;
		}

		public void SetLicenses(List<AccountLicense> val)
		{
			this.Licenses = val;
		}

		public void SetName(string val)
		{
			this.Name = val;
		}

		public void SetRealmPermissions(uint val)
		{
			this.RealmPermissions = val;
		}

		public void SetStatus(uint val)
		{
			this.Status = val;
		}

		public void SetStatusExpiration(ulong val)
		{
			this.StatusExpiration = val;
		}

		public void SetSubscriptionExpiration(ulong val)
		{
			this.SubscriptionExpiration = val;
		}

		public void SetUnitsRemaining(uint val)
		{
			this.UnitsRemaining = val;
		}
	}
}