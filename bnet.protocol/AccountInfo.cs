using System;
using System.IO;
using System.Text;

namespace bnet.protocol
{
	public class AccountInfo : IProtoBuf
	{
		public bool HasAccountPaid;

		private bool _AccountPaid;

		public bool HasCountryId;

		private uint _CountryId;

		public bool HasBattleTag;

		private string _BattleTag;

		public bool HasManualReview;

		private bool _ManualReview;

		public bool AccountPaid
		{
			get
			{
				return this._AccountPaid;
			}
			set
			{
				this._AccountPaid = value;
				this.HasAccountPaid = true;
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

		public uint CountryId
		{
			get
			{
				return this._CountryId;
			}
			set
			{
				this._CountryId = value;
				this.HasCountryId = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public bool ManualReview
		{
			get
			{
				return this._ManualReview;
			}
			set
			{
				this._ManualReview = value;
				this.HasManualReview = true;
			}
		}

		public AccountInfo()
		{
		}

		public void Deserialize(Stream stream)
		{
			AccountInfo.Deserialize(stream, this);
		}

		public static AccountInfo Deserialize(Stream stream, AccountInfo instance)
		{
			return AccountInfo.Deserialize(stream, instance, (long)-1);
		}

		public static AccountInfo Deserialize(Stream stream, AccountInfo instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			instance.AccountPaid = false;
			instance.CountryId = 0;
			instance.ManualReview = false;
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
							instance.AccountPaid = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 21)
						{
							instance.CountryId = binaryReader.ReadUInt32();
						}
						else if (num1 == 26)
						{
							instance.BattleTag = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 32)
						{
							instance.ManualReview = ProtocolParser.ReadBool(stream);
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

		public static AccountInfo DeserializeLengthDelimited(Stream stream)
		{
			AccountInfo accountInfo = new AccountInfo();
			AccountInfo.DeserializeLengthDelimited(stream, accountInfo);
			return accountInfo;
		}

		public static AccountInfo DeserializeLengthDelimited(Stream stream, AccountInfo instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return AccountInfo.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			AccountInfo accountInfo = obj as AccountInfo;
			if (accountInfo == null)
			{
				return false;
			}
			if (this.HasAccountPaid != accountInfo.HasAccountPaid || this.HasAccountPaid && !this.AccountPaid.Equals(accountInfo.AccountPaid))
			{
				return false;
			}
			if (this.HasCountryId != accountInfo.HasCountryId || this.HasCountryId && !this.CountryId.Equals(accountInfo.CountryId))
			{
				return false;
			}
			if (this.HasBattleTag != accountInfo.HasBattleTag || this.HasBattleTag && !this.BattleTag.Equals(accountInfo.BattleTag))
			{
				return false;
			}
			if (this.HasManualReview == accountInfo.HasManualReview && (!this.HasManualReview || this.ManualReview.Equals(accountInfo.ManualReview)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasAccountPaid)
			{
				hashCode ^= this.AccountPaid.GetHashCode();
			}
			if (this.HasCountryId)
			{
				hashCode ^= this.CountryId.GetHashCode();
			}
			if (this.HasBattleTag)
			{
				hashCode ^= this.BattleTag.GetHashCode();
			}
			if (this.HasManualReview)
			{
				hashCode ^= this.ManualReview.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasAccountPaid)
			{
				num++;
				num++;
			}
			if (this.HasCountryId)
			{
				num++;
				num += 4;
			}
			if (this.HasBattleTag)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.BattleTag);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasManualReview)
			{
				num++;
				num++;
			}
			return num;
		}

		public static AccountInfo ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<AccountInfo>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			AccountInfo.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, AccountInfo instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasAccountPaid)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteBool(stream, instance.AccountPaid);
			}
			if (instance.HasCountryId)
			{
				stream.WriteByte(21);
				binaryWriter.Write(instance.CountryId);
			}
			if (instance.HasBattleTag)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.BattleTag));
			}
			if (instance.HasManualReview)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteBool(stream, instance.ManualReview);
			}
		}

		public void SetAccountPaid(bool val)
		{
			this.AccountPaid = val;
		}

		public void SetBattleTag(string val)
		{
			this.BattleTag = val;
		}

		public void SetCountryId(uint val)
		{
			this.CountryId = val;
		}

		public void SetManualReview(bool val)
		{
			this.ManualReview = val;
		}
	}
}