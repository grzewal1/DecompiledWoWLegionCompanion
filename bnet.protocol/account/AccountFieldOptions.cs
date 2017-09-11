using System;
using System.IO;

namespace bnet.protocol.account
{
	public class AccountFieldOptions : IProtoBuf
	{
		public bool HasAllFields;

		private bool _AllFields;

		public bool HasFieldAccountLevelInfo;

		private bool _FieldAccountLevelInfo;

		public bool HasFieldPrivacyInfo;

		private bool _FieldPrivacyInfo;

		public bool HasFieldParentalControlInfo;

		private bool _FieldParentalControlInfo;

		public bool HasFieldGameLevelInfo;

		private bool _FieldGameLevelInfo;

		public bool HasFieldGameStatus;

		private bool _FieldGameStatus;

		public bool HasFieldGameAccounts;

		private bool _FieldGameAccounts;

		public bool AllFields
		{
			get
			{
				return this._AllFields;
			}
			set
			{
				this._AllFields = value;
				this.HasAllFields = true;
			}
		}

		public bool FieldAccountLevelInfo
		{
			get
			{
				return this._FieldAccountLevelInfo;
			}
			set
			{
				this._FieldAccountLevelInfo = value;
				this.HasFieldAccountLevelInfo = true;
			}
		}

		public bool FieldGameAccounts
		{
			get
			{
				return this._FieldGameAccounts;
			}
			set
			{
				this._FieldGameAccounts = value;
				this.HasFieldGameAccounts = true;
			}
		}

		public bool FieldGameLevelInfo
		{
			get
			{
				return this._FieldGameLevelInfo;
			}
			set
			{
				this._FieldGameLevelInfo = value;
				this.HasFieldGameLevelInfo = true;
			}
		}

		public bool FieldGameStatus
		{
			get
			{
				return this._FieldGameStatus;
			}
			set
			{
				this._FieldGameStatus = value;
				this.HasFieldGameStatus = true;
			}
		}

		public bool FieldParentalControlInfo
		{
			get
			{
				return this._FieldParentalControlInfo;
			}
			set
			{
				this._FieldParentalControlInfo = value;
				this.HasFieldParentalControlInfo = true;
			}
		}

		public bool FieldPrivacyInfo
		{
			get
			{
				return this._FieldPrivacyInfo;
			}
			set
			{
				this._FieldPrivacyInfo = value;
				this.HasFieldPrivacyInfo = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public AccountFieldOptions()
		{
		}

		public void Deserialize(Stream stream)
		{
			AccountFieldOptions.Deserialize(stream, this);
		}

		public static AccountFieldOptions Deserialize(Stream stream, AccountFieldOptions instance)
		{
			return AccountFieldOptions.Deserialize(stream, instance, (long)-1);
		}

		public static AccountFieldOptions Deserialize(Stream stream, AccountFieldOptions instance, long limit)
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
							instance.AllFields = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 16)
						{
							instance.FieldAccountLevelInfo = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 24)
						{
							instance.FieldPrivacyInfo = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 32)
						{
							instance.FieldParentalControlInfo = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 48)
						{
							instance.FieldGameLevelInfo = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 56)
						{
							instance.FieldGameStatus = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 64)
						{
							instance.FieldGameAccounts = ProtocolParser.ReadBool(stream);
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

		public static AccountFieldOptions DeserializeLengthDelimited(Stream stream)
		{
			AccountFieldOptions accountFieldOption = new AccountFieldOptions();
			AccountFieldOptions.DeserializeLengthDelimited(stream, accountFieldOption);
			return accountFieldOption;
		}

		public static AccountFieldOptions DeserializeLengthDelimited(Stream stream, AccountFieldOptions instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return AccountFieldOptions.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			AccountFieldOptions accountFieldOption = obj as AccountFieldOptions;
			if (accountFieldOption == null)
			{
				return false;
			}
			if (this.HasAllFields != accountFieldOption.HasAllFields || this.HasAllFields && !this.AllFields.Equals(accountFieldOption.AllFields))
			{
				return false;
			}
			if (this.HasFieldAccountLevelInfo != accountFieldOption.HasFieldAccountLevelInfo || this.HasFieldAccountLevelInfo && !this.FieldAccountLevelInfo.Equals(accountFieldOption.FieldAccountLevelInfo))
			{
				return false;
			}
			if (this.HasFieldPrivacyInfo != accountFieldOption.HasFieldPrivacyInfo || this.HasFieldPrivacyInfo && !this.FieldPrivacyInfo.Equals(accountFieldOption.FieldPrivacyInfo))
			{
				return false;
			}
			if (this.HasFieldParentalControlInfo != accountFieldOption.HasFieldParentalControlInfo || this.HasFieldParentalControlInfo && !this.FieldParentalControlInfo.Equals(accountFieldOption.FieldParentalControlInfo))
			{
				return false;
			}
			if (this.HasFieldGameLevelInfo != accountFieldOption.HasFieldGameLevelInfo || this.HasFieldGameLevelInfo && !this.FieldGameLevelInfo.Equals(accountFieldOption.FieldGameLevelInfo))
			{
				return false;
			}
			if (this.HasFieldGameStatus != accountFieldOption.HasFieldGameStatus || this.HasFieldGameStatus && !this.FieldGameStatus.Equals(accountFieldOption.FieldGameStatus))
			{
				return false;
			}
			if (this.HasFieldGameAccounts == accountFieldOption.HasFieldGameAccounts && (!this.HasFieldGameAccounts || this.FieldGameAccounts.Equals(accountFieldOption.FieldGameAccounts)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasAllFields)
			{
				hashCode ^= this.AllFields.GetHashCode();
			}
			if (this.HasFieldAccountLevelInfo)
			{
				hashCode ^= this.FieldAccountLevelInfo.GetHashCode();
			}
			if (this.HasFieldPrivacyInfo)
			{
				hashCode ^= this.FieldPrivacyInfo.GetHashCode();
			}
			if (this.HasFieldParentalControlInfo)
			{
				hashCode ^= this.FieldParentalControlInfo.GetHashCode();
			}
			if (this.HasFieldGameLevelInfo)
			{
				hashCode ^= this.FieldGameLevelInfo.GetHashCode();
			}
			if (this.HasFieldGameStatus)
			{
				hashCode ^= this.FieldGameStatus.GetHashCode();
			}
			if (this.HasFieldGameAccounts)
			{
				hashCode ^= this.FieldGameAccounts.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasAllFields)
			{
				num++;
				num++;
			}
			if (this.HasFieldAccountLevelInfo)
			{
				num++;
				num++;
			}
			if (this.HasFieldPrivacyInfo)
			{
				num++;
				num++;
			}
			if (this.HasFieldParentalControlInfo)
			{
				num++;
				num++;
			}
			if (this.HasFieldGameLevelInfo)
			{
				num++;
				num++;
			}
			if (this.HasFieldGameStatus)
			{
				num++;
				num++;
			}
			if (this.HasFieldGameAccounts)
			{
				num++;
				num++;
			}
			return num;
		}

		public static AccountFieldOptions ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<AccountFieldOptions>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			AccountFieldOptions.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, AccountFieldOptions instance)
		{
			if (instance.HasAllFields)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteBool(stream, instance.AllFields);
			}
			if (instance.HasFieldAccountLevelInfo)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteBool(stream, instance.FieldAccountLevelInfo);
			}
			if (instance.HasFieldPrivacyInfo)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteBool(stream, instance.FieldPrivacyInfo);
			}
			if (instance.HasFieldParentalControlInfo)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteBool(stream, instance.FieldParentalControlInfo);
			}
			if (instance.HasFieldGameLevelInfo)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteBool(stream, instance.FieldGameLevelInfo);
			}
			if (instance.HasFieldGameStatus)
			{
				stream.WriteByte(56);
				ProtocolParser.WriteBool(stream, instance.FieldGameStatus);
			}
			if (instance.HasFieldGameAccounts)
			{
				stream.WriteByte(64);
				ProtocolParser.WriteBool(stream, instance.FieldGameAccounts);
			}
		}

		public void SetAllFields(bool val)
		{
			this.AllFields = val;
		}

		public void SetFieldAccountLevelInfo(bool val)
		{
			this.FieldAccountLevelInfo = val;
		}

		public void SetFieldGameAccounts(bool val)
		{
			this.FieldGameAccounts = val;
		}

		public void SetFieldGameLevelInfo(bool val)
		{
			this.FieldGameLevelInfo = val;
		}

		public void SetFieldGameStatus(bool val)
		{
			this.FieldGameStatus = val;
		}

		public void SetFieldParentalControlInfo(bool val)
		{
			this.FieldParentalControlInfo = val;
		}

		public void SetFieldPrivacyInfo(bool val)
		{
			this.FieldPrivacyInfo = val;
		}
	}
}