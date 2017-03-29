using System;
using System.IO;

namespace bnet.protocol.account
{
	public class AccountStateNotification : IProtoBuf
	{
		public bool HasState;

		private AccountState _State;

		public bool HasSubscriberId;

		private ulong _SubscriberId;

		public bool HasAccountTags;

		private AccountFieldTags _AccountTags;

		public bool HasSubscriptionCompleted;

		private bool _SubscriptionCompleted;

		public AccountFieldTags AccountTags
		{
			get
			{
				return this._AccountTags;
			}
			set
			{
				this._AccountTags = value;
				this.HasAccountTags = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public AccountState State
		{
			get
			{
				return this._State;
			}
			set
			{
				this._State = value;
				this.HasState = value != null;
			}
		}

		public ulong SubscriberId
		{
			get
			{
				return this._SubscriberId;
			}
			set
			{
				this._SubscriberId = value;
				this.HasSubscriberId = true;
			}
		}

		public bool SubscriptionCompleted
		{
			get
			{
				return this._SubscriptionCompleted;
			}
			set
			{
				this._SubscriptionCompleted = value;
				this.HasSubscriptionCompleted = true;
			}
		}

		public AccountStateNotification()
		{
		}

		public void Deserialize(Stream stream)
		{
			AccountStateNotification.Deserialize(stream, this);
		}

		public static AccountStateNotification Deserialize(Stream stream, AccountStateNotification instance)
		{
			return AccountStateNotification.Deserialize(stream, instance, (long)-1);
		}

		public static AccountStateNotification Deserialize(Stream stream, AccountStateNotification instance, long limit)
		{
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
							if (instance.State != null)
							{
								AccountState.DeserializeLengthDelimited(stream, instance.State);
							}
							else
							{
								instance.State = AccountState.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 16)
						{
							instance.SubscriberId = ProtocolParser.ReadUInt64(stream);
						}
						else if (num1 != 26)
						{
							if (num1 == 32)
							{
								instance.SubscriptionCompleted = ProtocolParser.ReadBool(stream);
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
						else if (instance.AccountTags != null)
						{
							AccountFieldTags.DeserializeLengthDelimited(stream, instance.AccountTags);
						}
						else
						{
							instance.AccountTags = AccountFieldTags.DeserializeLengthDelimited(stream);
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

		public static AccountStateNotification DeserializeLengthDelimited(Stream stream)
		{
			AccountStateNotification accountStateNotification = new AccountStateNotification();
			AccountStateNotification.DeserializeLengthDelimited(stream, accountStateNotification);
			return accountStateNotification;
		}

		public static AccountStateNotification DeserializeLengthDelimited(Stream stream, AccountStateNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return AccountStateNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			AccountStateNotification accountStateNotification = obj as AccountStateNotification;
			if (accountStateNotification == null)
			{
				return false;
			}
			if (this.HasState != accountStateNotification.HasState || this.HasState && !this.State.Equals(accountStateNotification.State))
			{
				return false;
			}
			if (this.HasSubscriberId != accountStateNotification.HasSubscriberId || this.HasSubscriberId && !this.SubscriberId.Equals(accountStateNotification.SubscriberId))
			{
				return false;
			}
			if (this.HasAccountTags != accountStateNotification.HasAccountTags || this.HasAccountTags && !this.AccountTags.Equals(accountStateNotification.AccountTags))
			{
				return false;
			}
			if (this.HasSubscriptionCompleted == accountStateNotification.HasSubscriptionCompleted && (!this.HasSubscriptionCompleted || this.SubscriptionCompleted.Equals(accountStateNotification.SubscriptionCompleted)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasState)
			{
				hashCode = hashCode ^ this.State.GetHashCode();
			}
			if (this.HasSubscriberId)
			{
				hashCode = hashCode ^ this.SubscriberId.GetHashCode();
			}
			if (this.HasAccountTags)
			{
				hashCode = hashCode ^ this.AccountTags.GetHashCode();
			}
			if (this.HasSubscriptionCompleted)
			{
				hashCode = hashCode ^ this.SubscriptionCompleted.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasState)
			{
				num++;
				uint serializedSize = this.State.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasSubscriberId)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt64(this.SubscriberId);
			}
			if (this.HasAccountTags)
			{
				num++;
				uint serializedSize1 = this.AccountTags.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			if (this.HasSubscriptionCompleted)
			{
				num++;
				num++;
			}
			return num;
		}

		public static AccountStateNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<AccountStateNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			AccountStateNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, AccountStateNotification instance)
		{
			if (instance.HasState)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.State.GetSerializedSize());
				AccountState.Serialize(stream, instance.State);
			}
			if (instance.HasSubscriberId)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, instance.SubscriberId);
			}
			if (instance.HasAccountTags)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteUInt32(stream, instance.AccountTags.GetSerializedSize());
				AccountFieldTags.Serialize(stream, instance.AccountTags);
			}
			if (instance.HasSubscriptionCompleted)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteBool(stream, instance.SubscriptionCompleted);
			}
		}

		public void SetAccountTags(AccountFieldTags val)
		{
			this.AccountTags = val;
		}

		public void SetState(AccountState val)
		{
			this.State = val;
		}

		public void SetSubscriberId(ulong val)
		{
			this.SubscriberId = val;
		}

		public void SetSubscriptionCompleted(bool val)
		{
			this.SubscriptionCompleted = val;
		}
	}
}