using bnet.protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.authentication
{
	public class LogonResult : IProtoBuf
	{
		public bool HasAccount;

		private EntityId _Account;

		private List<EntityId> _GameAccount = new List<EntityId>();

		public bool HasEmail;

		private string _Email;

		private List<uint> _AvailableRegion = new List<uint>();

		public bool HasConnectedRegion;

		private uint _ConnectedRegion;

		public bool HasBattleTag;

		private string _BattleTag;

		public bool HasGeoipCountry;

		private string _GeoipCountry;

		public bool HasSessionKey;

		private byte[] _SessionKey;

		public EntityId Account
		{
			get
			{
				return this._Account;
			}
			set
			{
				this._Account = value;
				this.HasAccount = value != null;
			}
		}

		public List<uint> AvailableRegion
		{
			get
			{
				return this._AvailableRegion;
			}
			set
			{
				this._AvailableRegion = value;
			}
		}

		public int AvailableRegionCount
		{
			get
			{
				return this._AvailableRegion.Count;
			}
		}

		public List<uint> AvailableRegionList
		{
			get
			{
				return this._AvailableRegion;
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

		public uint ConnectedRegion
		{
			get
			{
				return this._ConnectedRegion;
			}
			set
			{
				this._ConnectedRegion = value;
				this.HasConnectedRegion = true;
			}
		}

		public string Email
		{
			get
			{
				return this._Email;
			}
			set
			{
				this._Email = value;
				this.HasEmail = value != null;
			}
		}

		public uint ErrorCode
		{
			get;
			set;
		}

		public List<EntityId> GameAccount
		{
			get
			{
				return this._GameAccount;
			}
			set
			{
				this._GameAccount = value;
			}
		}

		public int GameAccountCount
		{
			get
			{
				return this._GameAccount.Count;
			}
		}

		public List<EntityId> GameAccountList
		{
			get
			{
				return this._GameAccount;
			}
		}

		public string GeoipCountry
		{
			get
			{
				return this._GeoipCountry;
			}
			set
			{
				this._GeoipCountry = value;
				this.HasGeoipCountry = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public byte[] SessionKey
		{
			get
			{
				return this._SessionKey;
			}
			set
			{
				this._SessionKey = value;
				this.HasSessionKey = value != null;
			}
		}

		public LogonResult()
		{
		}

		public void AddAvailableRegion(uint val)
		{
			this._AvailableRegion.Add(val);
		}

		public void AddGameAccount(EntityId val)
		{
			this._GameAccount.Add(val);
		}

		public void ClearAvailableRegion()
		{
			this._AvailableRegion.Clear();
		}

		public void ClearGameAccount()
		{
			this._GameAccount.Clear();
		}

		public void Deserialize(Stream stream)
		{
			LogonResult.Deserialize(stream, this);
		}

		public static LogonResult Deserialize(Stream stream, LogonResult instance)
		{
			return LogonResult.Deserialize(stream, instance, (long)-1);
		}

		public static LogonResult Deserialize(Stream stream, LogonResult instance, long limit)
		{
			if (instance.GameAccount == null)
			{
				instance.GameAccount = new List<EntityId>();
			}
			if (instance.AvailableRegion == null)
			{
				instance.AvailableRegion = new List<uint>();
			}
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
							instance.ErrorCode = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 18)
						{
							if (instance.Account != null)
							{
								EntityId.DeserializeLengthDelimited(stream, instance.Account);
							}
							else
							{
								instance.Account = EntityId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 26)
						{
							instance.GameAccount.Add(EntityId.DeserializeLengthDelimited(stream));
						}
						else if (num1 == 34)
						{
							instance.Email = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 40)
						{
							instance.AvailableRegion.Add(ProtocolParser.ReadUInt32(stream));
						}
						else if (num1 == 48)
						{
							instance.ConnectedRegion = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 58)
						{
							instance.BattleTag = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 66)
						{
							instance.GeoipCountry = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 74)
						{
							instance.SessionKey = ProtocolParser.ReadBytes(stream);
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

		public static LogonResult DeserializeLengthDelimited(Stream stream)
		{
			LogonResult logonResult = new LogonResult();
			LogonResult.DeserializeLengthDelimited(stream, logonResult);
			return logonResult;
		}

		public static LogonResult DeserializeLengthDelimited(Stream stream, LogonResult instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return LogonResult.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			LogonResult logonResult = obj as LogonResult;
			if (logonResult == null)
			{
				return false;
			}
			if (!this.ErrorCode.Equals(logonResult.ErrorCode))
			{
				return false;
			}
			if (this.HasAccount != logonResult.HasAccount || this.HasAccount && !this.Account.Equals(logonResult.Account))
			{
				return false;
			}
			if (this.GameAccount.Count != logonResult.GameAccount.Count)
			{
				return false;
			}
			for (int i = 0; i < this.GameAccount.Count; i++)
			{
				if (!this.GameAccount[i].Equals(logonResult.GameAccount[i]))
				{
					return false;
				}
			}
			if (this.HasEmail != logonResult.HasEmail || this.HasEmail && !this.Email.Equals(logonResult.Email))
			{
				return false;
			}
			if (this.AvailableRegion.Count != logonResult.AvailableRegion.Count)
			{
				return false;
			}
			for (int j = 0; j < this.AvailableRegion.Count; j++)
			{
				if (!this.AvailableRegion[j].Equals(logonResult.AvailableRegion[j]))
				{
					return false;
				}
			}
			if (this.HasConnectedRegion != logonResult.HasConnectedRegion || this.HasConnectedRegion && !this.ConnectedRegion.Equals(logonResult.ConnectedRegion))
			{
				return false;
			}
			if (this.HasBattleTag != logonResult.HasBattleTag || this.HasBattleTag && !this.BattleTag.Equals(logonResult.BattleTag))
			{
				return false;
			}
			if (this.HasGeoipCountry == logonResult.HasGeoipCountry && (!this.HasGeoipCountry || this.GeoipCountry.Equals(logonResult.GeoipCountry)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.ErrorCode.GetHashCode();
			if (this.HasAccount)
			{
				hashCode ^= this.Account.GetHashCode();
			}
			foreach (EntityId gameAccount in this.GameAccount)
			{
				hashCode ^= gameAccount.GetHashCode();
			}
			if (this.HasEmail)
			{
				hashCode ^= this.Email.GetHashCode();
			}
			foreach (uint availableRegion in this.AvailableRegion)
			{
				hashCode ^= availableRegion.GetHashCode();
			}
			if (this.HasConnectedRegion)
			{
				hashCode ^= this.ConnectedRegion.GetHashCode();
			}
			if (this.HasBattleTag)
			{
				hashCode ^= this.BattleTag.GetHashCode();
			}
			if (this.HasGeoipCountry)
			{
				hashCode ^= this.GeoipCountry.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num += ProtocolParser.SizeOfUInt32(this.ErrorCode);
			if (this.HasAccount)
			{
				num++;
				uint serializedSize = this.Account.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.GameAccount.Count > 0)
			{
				foreach (EntityId gameAccount in this.GameAccount)
				{
					num++;
					uint serializedSize1 = gameAccount.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			if (this.HasEmail)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Email);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.AvailableRegion.Count > 0)
			{
				foreach (uint availableRegion in this.AvailableRegion)
				{
					num++;
					num += ProtocolParser.SizeOfUInt32(availableRegion);
				}
			}
			if (this.HasConnectedRegion)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.ConnectedRegion);
			}
			if (this.HasBattleTag)
			{
				num++;
				uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.BattleTag);
				num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			}
			if (this.HasGeoipCountry)
			{
				num++;
				uint num1 = (uint)Encoding.UTF8.GetByteCount(this.GeoipCountry);
				num = num + ProtocolParser.SizeOfUInt32(num1) + num1;
			}
			num++;
			return num;
		}

		public static LogonResult ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<LogonResult>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			LogonResult.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, LogonResult instance)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.ErrorCode);
			if (instance.HasAccount)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.Account.GetSerializedSize());
				EntityId.Serialize(stream, instance.Account);
			}
			if (instance.GameAccount.Count > 0)
			{
				foreach (EntityId gameAccount in instance.GameAccount)
				{
					stream.WriteByte(26);
					ProtocolParser.WriteUInt32(stream, gameAccount.GetSerializedSize());
					EntityId.Serialize(stream, gameAccount);
				}
			}
			if (instance.HasEmail)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Email));
			}
			if (instance.AvailableRegion.Count > 0)
			{
				foreach (uint availableRegion in instance.AvailableRegion)
				{
					stream.WriteByte(40);
					ProtocolParser.WriteUInt32(stream, availableRegion);
				}
			}
			if (instance.HasConnectedRegion)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteUInt32(stream, instance.ConnectedRegion);
			}
			if (instance.HasBattleTag)
			{
				stream.WriteByte(58);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.BattleTag));
			}
			if (instance.HasGeoipCountry)
			{
				stream.WriteByte(66);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.GeoipCountry));
			}
		}

		public void SetAccount(EntityId val)
		{
			this.Account = val;
		}

		public void SetAvailableRegion(List<uint> val)
		{
			this.AvailableRegion = val;
		}

		public void SetBattleTag(string val)
		{
			this.BattleTag = val;
		}

		public void SetConnectedRegion(uint val)
		{
			this.ConnectedRegion = val;
		}

		public void SetEmail(string val)
		{
			this.Email = val;
		}

		public void SetErrorCode(uint val)
		{
			this.ErrorCode = val;
		}

		public void SetGameAccount(List<EntityId> val)
		{
			this.GameAccount = val;
		}

		public void SetGeoipCountry(string val)
		{
			this.GeoipCountry = val;
		}

		public void SetSessionKey(byte[] val)
		{
			this.SessionKey = val;
		}
	}
}