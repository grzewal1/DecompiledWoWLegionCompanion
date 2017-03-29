using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace bnet.protocol.account
{
	public class CacheExpireRequest : IProtoBuf
	{
		private List<AccountId> _Account = new List<AccountId>();

		private List<GameAccountHandle> _GameAccount = new List<GameAccountHandle>();

		private List<string> _Email = new List<string>();

		public List<AccountId> Account
		{
			get
			{
				return this._Account;
			}
			set
			{
				this._Account = value;
			}
		}

		public int AccountCount
		{
			get
			{
				return this._Account.Count;
			}
		}

		public List<AccountId> AccountList
		{
			get
			{
				return this._Account;
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

		public List<GameAccountHandle> GameAccount
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

		public List<GameAccountHandle> GameAccountList
		{
			get
			{
				return this._GameAccount;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public CacheExpireRequest()
		{
		}

		public void AddAccount(AccountId val)
		{
			this._Account.Add(val);
		}

		public void AddEmail(string val)
		{
			this._Email.Add(val);
		}

		public void AddGameAccount(GameAccountHandle val)
		{
			this._GameAccount.Add(val);
		}

		public void ClearAccount()
		{
			this._Account.Clear();
		}

		public void ClearEmail()
		{
			this._Email.Clear();
		}

		public void ClearGameAccount()
		{
			this._GameAccount.Clear();
		}

		public void Deserialize(Stream stream)
		{
			CacheExpireRequest.Deserialize(stream, this);
		}

		public static CacheExpireRequest Deserialize(Stream stream, CacheExpireRequest instance)
		{
			return CacheExpireRequest.Deserialize(stream, instance, (long)-1);
		}

		public static CacheExpireRequest Deserialize(Stream stream, CacheExpireRequest instance, long limit)
		{
			if (instance.Account == null)
			{
				instance.Account = new List<AccountId>();
			}
			if (instance.GameAccount == null)
			{
				instance.GameAccount = new List<GameAccountHandle>();
			}
			if (instance.Email == null)
			{
				instance.Email = new List<string>();
			}
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
							instance.Account.Add(AccountId.DeserializeLengthDelimited(stream));
						}
						else if (num1 == 18)
						{
							instance.GameAccount.Add(GameAccountHandle.DeserializeLengthDelimited(stream));
						}
						else if (num1 == 26)
						{
							instance.Email.Add(ProtocolParser.ReadString(stream));
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

		public static CacheExpireRequest DeserializeLengthDelimited(Stream stream)
		{
			CacheExpireRequest cacheExpireRequest = new CacheExpireRequest();
			CacheExpireRequest.DeserializeLengthDelimited(stream, cacheExpireRequest);
			return cacheExpireRequest;
		}

		public static CacheExpireRequest DeserializeLengthDelimited(Stream stream, CacheExpireRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return CacheExpireRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			CacheExpireRequest cacheExpireRequest = obj as CacheExpireRequest;
			if (cacheExpireRequest == null)
			{
				return false;
			}
			if (this.Account.Count != cacheExpireRequest.Account.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Account.Count; i++)
			{
				if (!this.Account[i].Equals(cacheExpireRequest.Account[i]))
				{
					return false;
				}
			}
			if (this.GameAccount.Count != cacheExpireRequest.GameAccount.Count)
			{
				return false;
			}
			for (int j = 0; j < this.GameAccount.Count; j++)
			{
				if (!this.GameAccount[j].Equals(cacheExpireRequest.GameAccount[j]))
				{
					return false;
				}
			}
			if (this.Email.Count != cacheExpireRequest.Email.Count)
			{
				return false;
			}
			for (int k = 0; k < this.Email.Count; k++)
			{
				if (!this.Email[k].Equals(cacheExpireRequest.Email[k]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (AccountId account in this.Account)
			{
				hashCode = hashCode ^ account.GetHashCode();
			}
			foreach (GameAccountHandle gameAccount in this.GameAccount)
			{
				hashCode = hashCode ^ gameAccount.GetHashCode();
			}
			foreach (string email in this.Email)
			{
				hashCode = hashCode ^ email.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Account.Count > 0)
			{
				foreach (AccountId account in this.Account)
				{
					num++;
					uint serializedSize = account.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			if (this.GameAccount.Count > 0)
			{
				foreach (GameAccountHandle gameAccount in this.GameAccount)
				{
					num++;
					uint serializedSize1 = gameAccount.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			if (this.Email.Count > 0)
			{
				foreach (string email in this.Email)
				{
					num++;
					uint byteCount = (uint)Encoding.UTF8.GetByteCount(email);
					num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
				}
			}
			return num;
		}

		public static CacheExpireRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<CacheExpireRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			CacheExpireRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, CacheExpireRequest instance)
		{
			if (instance.Account.Count > 0)
			{
				foreach (AccountId account in instance.Account)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, account.GetSerializedSize());
					AccountId.Serialize(stream, account);
				}
			}
			if (instance.GameAccount.Count > 0)
			{
				foreach (GameAccountHandle gameAccount in instance.GameAccount)
				{
					stream.WriteByte(18);
					ProtocolParser.WriteUInt32(stream, gameAccount.GetSerializedSize());
					GameAccountHandle.Serialize(stream, gameAccount);
				}
			}
			if (instance.Email.Count > 0)
			{
				foreach (string email in instance.Email)
				{
					stream.WriteByte(26);
					ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(email));
				}
			}
		}

		public void SetAccount(List<AccountId> val)
		{
			this.Account = val;
		}

		public void SetEmail(List<string> val)
		{
			this.Email = val;
		}

		public void SetGameAccount(List<GameAccountHandle> val)
		{
			this.GameAccount = val;
		}
	}
}