using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.account
{
	public class AccountState : IProtoBuf
	{
		public bool HasAccountLevelInfo;

		private bnet.protocol.account.AccountLevelInfo _AccountLevelInfo;

		public bool HasPrivacyInfo;

		private bnet.protocol.account.PrivacyInfo _PrivacyInfo;

		public bool HasParentalControlInfo;

		private bnet.protocol.account.ParentalControlInfo _ParentalControlInfo;

		private List<bnet.protocol.account.GameLevelInfo> _GameLevelInfo = new List<bnet.protocol.account.GameLevelInfo>();

		private List<bnet.protocol.account.GameStatus> _GameStatus = new List<bnet.protocol.account.GameStatus>();

		private List<GameAccountList> _GameAccounts = new List<GameAccountList>();

		public bnet.protocol.account.AccountLevelInfo AccountLevelInfo
		{
			get
			{
				return this._AccountLevelInfo;
			}
			set
			{
				this._AccountLevelInfo = value;
				this.HasAccountLevelInfo = value != null;
			}
		}

		public List<GameAccountList> GameAccounts
		{
			get
			{
				return this._GameAccounts;
			}
			set
			{
				this._GameAccounts = value;
			}
		}

		public int GameAccountsCount
		{
			get
			{
				return this._GameAccounts.Count;
			}
		}

		public List<GameAccountList> GameAccountsList
		{
			get
			{
				return this._GameAccounts;
			}
		}

		public List<bnet.protocol.account.GameLevelInfo> GameLevelInfo
		{
			get
			{
				return this._GameLevelInfo;
			}
			set
			{
				this._GameLevelInfo = value;
			}
		}

		public int GameLevelInfoCount
		{
			get
			{
				return this._GameLevelInfo.Count;
			}
		}

		public List<bnet.protocol.account.GameLevelInfo> GameLevelInfoList
		{
			get
			{
				return this._GameLevelInfo;
			}
		}

		public List<bnet.protocol.account.GameStatus> GameStatus
		{
			get
			{
				return this._GameStatus;
			}
			set
			{
				this._GameStatus = value;
			}
		}

		public int GameStatusCount
		{
			get
			{
				return this._GameStatus.Count;
			}
		}

		public List<bnet.protocol.account.GameStatus> GameStatusList
		{
			get
			{
				return this._GameStatus;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
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

		public bnet.protocol.account.PrivacyInfo PrivacyInfo
		{
			get
			{
				return this._PrivacyInfo;
			}
			set
			{
				this._PrivacyInfo = value;
				this.HasPrivacyInfo = value != null;
			}
		}

		public AccountState()
		{
		}

		public void AddGameAccounts(GameAccountList val)
		{
			this._GameAccounts.Add(val);
		}

		public void AddGameLevelInfo(bnet.protocol.account.GameLevelInfo val)
		{
			this._GameLevelInfo.Add(val);
		}

		public void AddGameStatus(bnet.protocol.account.GameStatus val)
		{
			this._GameStatus.Add(val);
		}

		public void ClearGameAccounts()
		{
			this._GameAccounts.Clear();
		}

		public void ClearGameLevelInfo()
		{
			this._GameLevelInfo.Clear();
		}

		public void ClearGameStatus()
		{
			this._GameStatus.Clear();
		}

		public void Deserialize(Stream stream)
		{
			AccountState.Deserialize(stream, this);
		}

		public static AccountState Deserialize(Stream stream, AccountState instance)
		{
			return AccountState.Deserialize(stream, instance, (long)-1);
		}

		public static AccountState Deserialize(Stream stream, AccountState instance, long limit)
		{
			if (instance.GameLevelInfo == null)
			{
				instance.GameLevelInfo = new List<bnet.protocol.account.GameLevelInfo>();
			}
			if (instance.GameStatus == null)
			{
				instance.GameStatus = new List<bnet.protocol.account.GameStatus>();
			}
			if (instance.GameAccounts == null)
			{
				instance.GameAccounts = new List<GameAccountList>();
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
						if (instance.AccountLevelInfo != null)
						{
							bnet.protocol.account.AccountLevelInfo.DeserializeLengthDelimited(stream, instance.AccountLevelInfo);
						}
						else
						{
							instance.AccountLevelInfo = bnet.protocol.account.AccountLevelInfo.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 18)
					{
						if (instance.PrivacyInfo != null)
						{
							bnet.protocol.account.PrivacyInfo.DeserializeLengthDelimited(stream, instance.PrivacyInfo);
						}
						else
						{
							instance.PrivacyInfo = bnet.protocol.account.PrivacyInfo.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 26)
					{
						if (instance.ParentalControlInfo != null)
						{
							bnet.protocol.account.ParentalControlInfo.DeserializeLengthDelimited(stream, instance.ParentalControlInfo);
						}
						else
						{
							instance.ParentalControlInfo = bnet.protocol.account.ParentalControlInfo.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 42)
					{
						instance.GameLevelInfo.Add(bnet.protocol.account.GameLevelInfo.DeserializeLengthDelimited(stream));
					}
					else if (num == 50)
					{
						instance.GameStatus.Add(bnet.protocol.account.GameStatus.DeserializeLengthDelimited(stream));
					}
					else if (num == 58)
					{
						instance.GameAccounts.Add(GameAccountList.DeserializeLengthDelimited(stream));
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

		public static AccountState DeserializeLengthDelimited(Stream stream)
		{
			AccountState accountState = new AccountState();
			AccountState.DeserializeLengthDelimited(stream, accountState);
			return accountState;
		}

		public static AccountState DeserializeLengthDelimited(Stream stream, AccountState instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return AccountState.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			AccountState accountState = obj as AccountState;
			if (accountState == null)
			{
				return false;
			}
			if (this.HasAccountLevelInfo != accountState.HasAccountLevelInfo || this.HasAccountLevelInfo && !this.AccountLevelInfo.Equals(accountState.AccountLevelInfo))
			{
				return false;
			}
			if (this.HasPrivacyInfo != accountState.HasPrivacyInfo || this.HasPrivacyInfo && !this.PrivacyInfo.Equals(accountState.PrivacyInfo))
			{
				return false;
			}
			if (this.HasParentalControlInfo != accountState.HasParentalControlInfo || this.HasParentalControlInfo && !this.ParentalControlInfo.Equals(accountState.ParentalControlInfo))
			{
				return false;
			}
			if (this.GameLevelInfo.Count != accountState.GameLevelInfo.Count)
			{
				return false;
			}
			for (int i = 0; i < this.GameLevelInfo.Count; i++)
			{
				if (!this.GameLevelInfo[i].Equals(accountState.GameLevelInfo[i]))
				{
					return false;
				}
			}
			if (this.GameStatus.Count != accountState.GameStatus.Count)
			{
				return false;
			}
			for (int j = 0; j < this.GameStatus.Count; j++)
			{
				if (!this.GameStatus[j].Equals(accountState.GameStatus[j]))
				{
					return false;
				}
			}
			if (this.GameAccounts.Count != accountState.GameAccounts.Count)
			{
				return false;
			}
			for (int k = 0; k < this.GameAccounts.Count; k++)
			{
				if (!this.GameAccounts[k].Equals(accountState.GameAccounts[k]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasAccountLevelInfo)
			{
				hashCode ^= this.AccountLevelInfo.GetHashCode();
			}
			if (this.HasPrivacyInfo)
			{
				hashCode ^= this.PrivacyInfo.GetHashCode();
			}
			if (this.HasParentalControlInfo)
			{
				hashCode ^= this.ParentalControlInfo.GetHashCode();
			}
			foreach (bnet.protocol.account.GameLevelInfo gameLevelInfo in this.GameLevelInfo)
			{
				hashCode ^= gameLevelInfo.GetHashCode();
			}
			foreach (bnet.protocol.account.GameStatus gameStatu in this.GameStatus)
			{
				hashCode ^= gameStatu.GetHashCode();
			}
			foreach (GameAccountList gameAccount in this.GameAccounts)
			{
				hashCode ^= gameAccount.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasAccountLevelInfo)
			{
				num++;
				uint serializedSize = this.AccountLevelInfo.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasPrivacyInfo)
			{
				num++;
				uint serializedSize1 = this.PrivacyInfo.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			if (this.HasParentalControlInfo)
			{
				num++;
				uint num1 = this.ParentalControlInfo.GetSerializedSize();
				num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
			}
			if (this.GameLevelInfo.Count > 0)
			{
				foreach (bnet.protocol.account.GameLevelInfo gameLevelInfo in this.GameLevelInfo)
				{
					num++;
					uint serializedSize2 = gameLevelInfo.GetSerializedSize();
					num = num + serializedSize2 + ProtocolParser.SizeOfUInt32(serializedSize2);
				}
			}
			if (this.GameStatus.Count > 0)
			{
				foreach (bnet.protocol.account.GameStatus gameStatu in this.GameStatus)
				{
					num++;
					uint num2 = gameStatu.GetSerializedSize();
					num = num + num2 + ProtocolParser.SizeOfUInt32(num2);
				}
			}
			if (this.GameAccounts.Count > 0)
			{
				foreach (GameAccountList gameAccount in this.GameAccounts)
				{
					num++;
					uint serializedSize3 = gameAccount.GetSerializedSize();
					num = num + serializedSize3 + ProtocolParser.SizeOfUInt32(serializedSize3);
				}
			}
			return num;
		}

		public static AccountState ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<AccountState>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			AccountState.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, AccountState instance)
		{
			if (instance.HasAccountLevelInfo)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AccountLevelInfo.GetSerializedSize());
				bnet.protocol.account.AccountLevelInfo.Serialize(stream, instance.AccountLevelInfo);
			}
			if (instance.HasPrivacyInfo)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.PrivacyInfo.GetSerializedSize());
				bnet.protocol.account.PrivacyInfo.Serialize(stream, instance.PrivacyInfo);
			}
			if (instance.HasParentalControlInfo)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteUInt32(stream, instance.ParentalControlInfo.GetSerializedSize());
				bnet.protocol.account.ParentalControlInfo.Serialize(stream, instance.ParentalControlInfo);
			}
			if (instance.GameLevelInfo.Count > 0)
			{
				foreach (bnet.protocol.account.GameLevelInfo gameLevelInfo in instance.GameLevelInfo)
				{
					stream.WriteByte(42);
					ProtocolParser.WriteUInt32(stream, gameLevelInfo.GetSerializedSize());
					bnet.protocol.account.GameLevelInfo.Serialize(stream, gameLevelInfo);
				}
			}
			if (instance.GameStatus.Count > 0)
			{
				foreach (bnet.protocol.account.GameStatus gameStatu in instance.GameStatus)
				{
					stream.WriteByte(50);
					ProtocolParser.WriteUInt32(stream, gameStatu.GetSerializedSize());
					bnet.protocol.account.GameStatus.Serialize(stream, gameStatu);
				}
			}
			if (instance.GameAccounts.Count > 0)
			{
				foreach (GameAccountList gameAccount in instance.GameAccounts)
				{
					stream.WriteByte(58);
					ProtocolParser.WriteUInt32(stream, gameAccount.GetSerializedSize());
					GameAccountList.Serialize(stream, gameAccount);
				}
			}
		}

		public void SetAccountLevelInfo(bnet.protocol.account.AccountLevelInfo val)
		{
			this.AccountLevelInfo = val;
		}

		public void SetGameAccounts(List<GameAccountList> val)
		{
			this.GameAccounts = val;
		}

		public void SetGameLevelInfo(List<bnet.protocol.account.GameLevelInfo> val)
		{
			this.GameLevelInfo = val;
		}

		public void SetGameStatus(List<bnet.protocol.account.GameStatus> val)
		{
			this.GameStatus = val;
		}

		public void SetParentalControlInfo(bnet.protocol.account.ParentalControlInfo val)
		{
			this.ParentalControlInfo = val;
		}

		public void SetPrivacyInfo(bnet.protocol.account.PrivacyInfo val)
		{
			this.PrivacyInfo = val;
		}
	}
}