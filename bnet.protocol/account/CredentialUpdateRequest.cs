using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.account
{
	public class CredentialUpdateRequest : IProtoBuf
	{
		private List<AccountCredential> _OldCredentials = new List<AccountCredential>();

		private List<AccountCredential> _NewCredentials = new List<AccountCredential>();

		public bool HasRegion;

		private uint _Region;

		public AccountId Account
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

		public List<AccountCredential> NewCredentials
		{
			get
			{
				return this._NewCredentials;
			}
			set
			{
				this._NewCredentials = value;
			}
		}

		public int NewCredentialsCount
		{
			get
			{
				return this._NewCredentials.Count;
			}
		}

		public List<AccountCredential> NewCredentialsList
		{
			get
			{
				return this._NewCredentials;
			}
		}

		public List<AccountCredential> OldCredentials
		{
			get
			{
				return this._OldCredentials;
			}
			set
			{
				this._OldCredentials = value;
			}
		}

		public int OldCredentialsCount
		{
			get
			{
				return this._OldCredentials.Count;
			}
		}

		public List<AccountCredential> OldCredentialsList
		{
			get
			{
				return this._OldCredentials;
			}
		}

		public uint Region
		{
			get
			{
				return this._Region;
			}
			set
			{
				this._Region = value;
				this.HasRegion = true;
			}
		}

		public CredentialUpdateRequest()
		{
		}

		public void AddNewCredentials(AccountCredential val)
		{
			this._NewCredentials.Add(val);
		}

		public void AddOldCredentials(AccountCredential val)
		{
			this._OldCredentials.Add(val);
		}

		public void ClearNewCredentials()
		{
			this._NewCredentials.Clear();
		}

		public void ClearOldCredentials()
		{
			this._OldCredentials.Clear();
		}

		public void Deserialize(Stream stream)
		{
			CredentialUpdateRequest.Deserialize(stream, this);
		}

		public static CredentialUpdateRequest Deserialize(Stream stream, CredentialUpdateRequest instance)
		{
			return CredentialUpdateRequest.Deserialize(stream, instance, (long)-1);
		}

		public static CredentialUpdateRequest Deserialize(Stream stream, CredentialUpdateRequest instance, long limit)
		{
			if (instance.OldCredentials == null)
			{
				instance.OldCredentials = new List<AccountCredential>();
			}
			if (instance.NewCredentials == null)
			{
				instance.NewCredentials = new List<AccountCredential>();
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
							if (instance.Account != null)
							{
								AccountId.DeserializeLengthDelimited(stream, instance.Account);
							}
							else
							{
								instance.Account = AccountId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 18)
						{
							instance.OldCredentials.Add(AccountCredential.DeserializeLengthDelimited(stream));
						}
						else if (num1 == 26)
						{
							instance.NewCredentials.Add(AccountCredential.DeserializeLengthDelimited(stream));
						}
						else if (num1 == 32)
						{
							instance.Region = ProtocolParser.ReadUInt32(stream);
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

		public static CredentialUpdateRequest DeserializeLengthDelimited(Stream stream)
		{
			CredentialUpdateRequest credentialUpdateRequest = new CredentialUpdateRequest();
			CredentialUpdateRequest.DeserializeLengthDelimited(stream, credentialUpdateRequest);
			return credentialUpdateRequest;
		}

		public static CredentialUpdateRequest DeserializeLengthDelimited(Stream stream, CredentialUpdateRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return CredentialUpdateRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			CredentialUpdateRequest credentialUpdateRequest = obj as CredentialUpdateRequest;
			if (credentialUpdateRequest == null)
			{
				return false;
			}
			if (!this.Account.Equals(credentialUpdateRequest.Account))
			{
				return false;
			}
			if (this.OldCredentials.Count != credentialUpdateRequest.OldCredentials.Count)
			{
				return false;
			}
			for (int i = 0; i < this.OldCredentials.Count; i++)
			{
				if (!this.OldCredentials[i].Equals(credentialUpdateRequest.OldCredentials[i]))
				{
					return false;
				}
			}
			if (this.NewCredentials.Count != credentialUpdateRequest.NewCredentials.Count)
			{
				return false;
			}
			for (int j = 0; j < this.NewCredentials.Count; j++)
			{
				if (!this.NewCredentials[j].Equals(credentialUpdateRequest.NewCredentials[j]))
				{
					return false;
				}
			}
			if (this.HasRegion == credentialUpdateRequest.HasRegion && (!this.HasRegion || this.Region.Equals(credentialUpdateRequest.Region)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Account.GetHashCode();
			foreach (AccountCredential oldCredential in this.OldCredentials)
			{
				hashCode ^= oldCredential.GetHashCode();
			}
			foreach (AccountCredential newCredential in this.NewCredentials)
			{
				hashCode ^= newCredential.GetHashCode();
			}
			if (this.HasRegion)
			{
				hashCode ^= this.Region.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.Account.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.OldCredentials.Count > 0)
			{
				foreach (AccountCredential oldCredential in this.OldCredentials)
				{
					num++;
					uint serializedSize1 = oldCredential.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			if (this.NewCredentials.Count > 0)
			{
				foreach (AccountCredential newCredential in this.NewCredentials)
				{
					num++;
					uint num1 = newCredential.GetSerializedSize();
					num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
				}
			}
			if (this.HasRegion)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.Region);
			}
			num++;
			return num;
		}

		public static CredentialUpdateRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<CredentialUpdateRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			CredentialUpdateRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, CredentialUpdateRequest instance)
		{
			if (instance.Account == null)
			{
				throw new ArgumentNullException("Account", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.Account.GetSerializedSize());
			AccountId.Serialize(stream, instance.Account);
			if (instance.OldCredentials.Count > 0)
			{
				foreach (AccountCredential oldCredential in instance.OldCredentials)
				{
					stream.WriteByte(18);
					ProtocolParser.WriteUInt32(stream, oldCredential.GetSerializedSize());
					AccountCredential.Serialize(stream, oldCredential);
				}
			}
			if (instance.NewCredentials.Count > 0)
			{
				foreach (AccountCredential newCredential in instance.NewCredentials)
				{
					stream.WriteByte(26);
					ProtocolParser.WriteUInt32(stream, newCredential.GetSerializedSize());
					AccountCredential.Serialize(stream, newCredential);
				}
			}
			if (instance.HasRegion)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteUInt32(stream, instance.Region);
			}
		}

		public void SetAccount(AccountId val)
		{
			this.Account = val;
		}

		public void SetNewCredentials(List<AccountCredential> val)
		{
			this.NewCredentials = val;
		}

		public void SetOldCredentials(List<AccountCredential> val)
		{
			this.OldCredentials = val;
		}

		public void SetRegion(uint val)
		{
			this.Region = val;
		}
	}
}