using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace bnet.protocol.account
{
	public class GetAccountResponse : IProtoBuf
	{
		public bool HasBlob;

		private AccountBlob _Blob;

		public bool HasId;

		private AccountId _Id;

		private List<string> _Email = new List<string>();

		public bool HasBattleTag;

		private string _BattleTag;

		public bool HasFullName;

		private string _FullName;

		private List<GameAccountLink> _Links = new List<GameAccountLink>();

		public bool HasParentalControlInfo;

		private bnet.protocol.account.ParentalControlInfo _ParentalControlInfo;

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

		public AccountBlob Blob
		{
			get
			{
				return this._Blob;
			}
			set
			{
				this._Blob = value;
				this.HasBlob = value != null;
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

		public string FullName
		{
			get
			{
				return this._FullName;
			}
			set
			{
				this._FullName = value;
				this.HasFullName = value != null;
			}
		}

		public AccountId Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				this._Id = value;
				this.HasId = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public List<GameAccountLink> Links
		{
			get
			{
				return this._Links;
			}
			set
			{
				this._Links = value;
			}
		}

		public int LinksCount
		{
			get
			{
				return this._Links.Count;
			}
		}

		public List<GameAccountLink> LinksList
		{
			get
			{
				return this._Links;
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

		public GetAccountResponse()
		{
		}

		public void AddEmail(string val)
		{
			this._Email.Add(val);
		}

		public void AddLinks(GameAccountLink val)
		{
			this._Links.Add(val);
		}

		public void ClearEmail()
		{
			this._Email.Clear();
		}

		public void ClearLinks()
		{
			this._Links.Clear();
		}

		public void Deserialize(Stream stream)
		{
			GetAccountResponse.Deserialize(stream, this);
		}

		public static GetAccountResponse Deserialize(Stream stream, GetAccountResponse instance)
		{
			return GetAccountResponse.Deserialize(stream, instance, (long)-1);
		}

		public static GetAccountResponse Deserialize(Stream stream, GetAccountResponse instance, long limit)
		{
			if (instance.Email == null)
			{
				instance.Email = new List<string>();
			}
			if (instance.Links == null)
			{
				instance.Links = new List<GameAccountLink>();
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
					else if (num == 90)
					{
						if (instance.Blob != null)
						{
							AccountBlob.DeserializeLengthDelimited(stream, instance.Blob);
						}
						else
						{
							instance.Blob = AccountBlob.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 98)
					{
						if (instance.Id != null)
						{
							AccountId.DeserializeLengthDelimited(stream, instance.Id);
						}
						else
						{
							instance.Id = AccountId.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 106)
					{
						instance.Email.Add(ProtocolParser.ReadString(stream));
					}
					else if (num == 114)
					{
						instance.BattleTag = ProtocolParser.ReadString(stream);
					}
					else if (num == 122)
					{
						instance.FullName = ProtocolParser.ReadString(stream);
					}
					else
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						uint field = key.Field;
						if (field == 16)
						{
							if (key.WireType == Wire.LengthDelimited)
							{
								instance.Links.Add(GameAccountLink.DeserializeLengthDelimited(stream));
								continue;
							}
						}
						else if (field != 17)
						{
							if (field == 0)
							{
								throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
							}
							ProtocolParser.SkipKey(stream, key);
						}
						else if (key.WireType == Wire.LengthDelimited)
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

		public static GetAccountResponse DeserializeLengthDelimited(Stream stream)
		{
			GetAccountResponse getAccountResponse = new GetAccountResponse();
			GetAccountResponse.DeserializeLengthDelimited(stream, getAccountResponse);
			return getAccountResponse;
		}

		public static GetAccountResponse DeserializeLengthDelimited(Stream stream, GetAccountResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GetAccountResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetAccountResponse getAccountResponse = obj as GetAccountResponse;
			if (getAccountResponse == null)
			{
				return false;
			}
			if (this.HasBlob != getAccountResponse.HasBlob || this.HasBlob && !this.Blob.Equals(getAccountResponse.Blob))
			{
				return false;
			}
			if (this.HasId != getAccountResponse.HasId || this.HasId && !this.Id.Equals(getAccountResponse.Id))
			{
				return false;
			}
			if (this.Email.Count != getAccountResponse.Email.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Email.Count; i++)
			{
				if (!this.Email[i].Equals(getAccountResponse.Email[i]))
				{
					return false;
				}
			}
			if (this.HasBattleTag != getAccountResponse.HasBattleTag || this.HasBattleTag && !this.BattleTag.Equals(getAccountResponse.BattleTag))
			{
				return false;
			}
			if (this.HasFullName != getAccountResponse.HasFullName || this.HasFullName && !this.FullName.Equals(getAccountResponse.FullName))
			{
				return false;
			}
			if (this.Links.Count != getAccountResponse.Links.Count)
			{
				return false;
			}
			for (int j = 0; j < this.Links.Count; j++)
			{
				if (!this.Links[j].Equals(getAccountResponse.Links[j]))
				{
					return false;
				}
			}
			if (this.HasParentalControlInfo == getAccountResponse.HasParentalControlInfo && (!this.HasParentalControlInfo || this.ParentalControlInfo.Equals(getAccountResponse.ParentalControlInfo)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasBlob)
			{
				hashCode ^= this.Blob.GetHashCode();
			}
			if (this.HasId)
			{
				hashCode ^= this.Id.GetHashCode();
			}
			foreach (string email in this.Email)
			{
				hashCode ^= email.GetHashCode();
			}
			if (this.HasBattleTag)
			{
				hashCode ^= this.BattleTag.GetHashCode();
			}
			if (this.HasFullName)
			{
				hashCode ^= this.FullName.GetHashCode();
			}
			foreach (GameAccountLink link in this.Links)
			{
				hashCode ^= link.GetHashCode();
			}
			if (this.HasParentalControlInfo)
			{
				hashCode ^= this.ParentalControlInfo.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasBlob)
			{
				num++;
				uint serializedSize = this.Blob.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasId)
			{
				num++;
				uint serializedSize1 = this.Id.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
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
			if (this.HasBattleTag)
			{
				num++;
				uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.BattleTag);
				num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			}
			if (this.HasFullName)
			{
				num++;
				uint num1 = (uint)Encoding.UTF8.GetByteCount(this.FullName);
				num = num + ProtocolParser.SizeOfUInt32(num1) + num1;
			}
			if (this.Links.Count > 0)
			{
				foreach (GameAccountLink link in this.Links)
				{
					num += 2;
					uint serializedSize2 = link.GetSerializedSize();
					num = num + serializedSize2 + ProtocolParser.SizeOfUInt32(serializedSize2);
				}
			}
			if (this.HasParentalControlInfo)
			{
				num += 2;
				uint num2 = this.ParentalControlInfo.GetSerializedSize();
				num = num + num2 + ProtocolParser.SizeOfUInt32(num2);
			}
			return num;
		}

		public static GetAccountResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetAccountResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetAccountResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetAccountResponse instance)
		{
			if (instance.HasBlob)
			{
				stream.WriteByte(90);
				ProtocolParser.WriteUInt32(stream, instance.Blob.GetSerializedSize());
				AccountBlob.Serialize(stream, instance.Blob);
			}
			if (instance.HasId)
			{
				stream.WriteByte(98);
				ProtocolParser.WriteUInt32(stream, instance.Id.GetSerializedSize());
				AccountId.Serialize(stream, instance.Id);
			}
			if (instance.Email.Count > 0)
			{
				foreach (string email in instance.Email)
				{
					stream.WriteByte(106);
					ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(email));
				}
			}
			if (instance.HasBattleTag)
			{
				stream.WriteByte(114);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.BattleTag));
			}
			if (instance.HasFullName)
			{
				stream.WriteByte(122);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.FullName));
			}
			if (instance.Links.Count > 0)
			{
				foreach (GameAccountLink link in instance.Links)
				{
					stream.WriteByte(130);
					stream.WriteByte(1);
					ProtocolParser.WriteUInt32(stream, link.GetSerializedSize());
					GameAccountLink.Serialize(stream, link);
				}
			}
			if (instance.HasParentalControlInfo)
			{
				stream.WriteByte(138);
				stream.WriteByte(1);
				ProtocolParser.WriteUInt32(stream, instance.ParentalControlInfo.GetSerializedSize());
				bnet.protocol.account.ParentalControlInfo.Serialize(stream, instance.ParentalControlInfo);
			}
		}

		public void SetBattleTag(string val)
		{
			this.BattleTag = val;
		}

		public void SetBlob(AccountBlob val)
		{
			this.Blob = val;
		}

		public void SetEmail(List<string> val)
		{
			this.Email = val;
		}

		public void SetFullName(string val)
		{
			this.FullName = val;
		}

		public void SetId(AccountId val)
		{
			this.Id = val;
		}

		public void SetLinks(List<GameAccountLink> val)
		{
			this.Links = val;
		}

		public void SetParentalControlInfo(bnet.protocol.account.ParentalControlInfo val)
		{
			this.ParentalControlInfo = val;
		}
	}
}