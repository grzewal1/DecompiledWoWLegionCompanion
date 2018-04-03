using System;
using System.IO;

namespace bnet.protocol.account
{
	public class AccountStateTagged : IProtoBuf
	{
		public bool HasAccountState;

		private bnet.protocol.account.AccountState _AccountState;

		public bool HasTags;

		private AccountFieldTags _Tags;

		public bnet.protocol.account.AccountState AccountState
		{
			get
			{
				return this._AccountState;
			}
			set
			{
				this._AccountState = value;
				this.HasAccountState = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public AccountFieldTags Tags
		{
			get
			{
				return this._Tags;
			}
			set
			{
				this._Tags = value;
				this.HasTags = value != null;
			}
		}

		public AccountStateTagged()
		{
		}

		public void Deserialize(Stream stream)
		{
			AccountStateTagged.Deserialize(stream, this);
		}

		public static AccountStateTagged Deserialize(Stream stream, AccountStateTagged instance)
		{
			return AccountStateTagged.Deserialize(stream, instance, (long)-1);
		}

		public static AccountStateTagged Deserialize(Stream stream, AccountStateTagged instance, long limit)
		{
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
						if (instance.AccountState != null)
						{
							bnet.protocol.account.AccountState.DeserializeLengthDelimited(stream, instance.AccountState);
						}
						else
						{
							instance.AccountState = bnet.protocol.account.AccountState.DeserializeLengthDelimited(stream);
						}
					}
					else if (num != 18)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.Tags != null)
					{
						AccountFieldTags.DeserializeLengthDelimited(stream, instance.Tags);
					}
					else
					{
						instance.Tags = AccountFieldTags.DeserializeLengthDelimited(stream);
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

		public static AccountStateTagged DeserializeLengthDelimited(Stream stream)
		{
			AccountStateTagged accountStateTagged = new AccountStateTagged();
			AccountStateTagged.DeserializeLengthDelimited(stream, accountStateTagged);
			return accountStateTagged;
		}

		public static AccountStateTagged DeserializeLengthDelimited(Stream stream, AccountStateTagged instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return AccountStateTagged.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			AccountStateTagged accountStateTagged = obj as AccountStateTagged;
			if (accountStateTagged == null)
			{
				return false;
			}
			if (this.HasAccountState != accountStateTagged.HasAccountState || this.HasAccountState && !this.AccountState.Equals(accountStateTagged.AccountState))
			{
				return false;
			}
			if (this.HasTags == accountStateTagged.HasTags && (!this.HasTags || this.Tags.Equals(accountStateTagged.Tags)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasAccountState)
			{
				hashCode ^= this.AccountState.GetHashCode();
			}
			if (this.HasTags)
			{
				hashCode ^= this.Tags.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasAccountState)
			{
				num++;
				uint serializedSize = this.AccountState.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasTags)
			{
				num++;
				uint serializedSize1 = this.Tags.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			return num;
		}

		public static AccountStateTagged ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<AccountStateTagged>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			AccountStateTagged.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, AccountStateTagged instance)
		{
			if (instance.HasAccountState)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AccountState.GetSerializedSize());
				bnet.protocol.account.AccountState.Serialize(stream, instance.AccountState);
			}
			if (instance.HasTags)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.Tags.GetSerializedSize());
				AccountFieldTags.Serialize(stream, instance.Tags);
			}
		}

		public void SetAccountState(bnet.protocol.account.AccountState val)
		{
			this.AccountState = val;
		}

		public void SetTags(AccountFieldTags val)
		{
			this.Tags = val;
		}
	}
}