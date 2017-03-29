using bnet.protocol.account;
using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.authentication
{
	public class AccountSettingsNotification : IProtoBuf
	{
		private List<AccountLicense> _Licenses = new List<AccountLicense>();

		public bool HasIsUsingRid;

		private bool _IsUsingRid;

		public bool HasIsPlayingFromIgr;

		private bool _IsPlayingFromIgr;

		public bool HasCanReceiveVoice;

		private bool _CanReceiveVoice;

		public bool HasCanSendVoice;

		private bool _CanSendVoice;

		public bool CanReceiveVoice
		{
			get
			{
				return this._CanReceiveVoice;
			}
			set
			{
				this._CanReceiveVoice = value;
				this.HasCanReceiveVoice = true;
			}
		}

		public bool CanSendVoice
		{
			get
			{
				return this._CanSendVoice;
			}
			set
			{
				this._CanSendVoice = value;
				this.HasCanSendVoice = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public bool IsPlayingFromIgr
		{
			get
			{
				return this._IsPlayingFromIgr;
			}
			set
			{
				this._IsPlayingFromIgr = value;
				this.HasIsPlayingFromIgr = true;
			}
		}

		public bool IsUsingRid
		{
			get
			{
				return this._IsUsingRid;
			}
			set
			{
				this._IsUsingRid = value;
				this.HasIsUsingRid = true;
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

		public AccountSettingsNotification()
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
			AccountSettingsNotification.Deserialize(stream, this);
		}

		public static AccountSettingsNotification Deserialize(Stream stream, AccountSettingsNotification instance)
		{
			return AccountSettingsNotification.Deserialize(stream, instance, (long)-1);
		}

		public static AccountSettingsNotification Deserialize(Stream stream, AccountSettingsNotification instance, long limit)
		{
			if (instance.Licenses == null)
			{
				instance.Licenses = new List<AccountLicense>();
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
							instance.Licenses.Add(AccountLicense.DeserializeLengthDelimited(stream));
						}
						else if (num1 == 16)
						{
							instance.IsUsingRid = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 24)
						{
							instance.IsPlayingFromIgr = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 32)
						{
							instance.CanReceiveVoice = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 40)
						{
							instance.CanSendVoice = ProtocolParser.ReadBool(stream);
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

		public static AccountSettingsNotification DeserializeLengthDelimited(Stream stream)
		{
			AccountSettingsNotification accountSettingsNotification = new AccountSettingsNotification();
			AccountSettingsNotification.DeserializeLengthDelimited(stream, accountSettingsNotification);
			return accountSettingsNotification;
		}

		public static AccountSettingsNotification DeserializeLengthDelimited(Stream stream, AccountSettingsNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return AccountSettingsNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			AccountSettingsNotification accountSettingsNotification = obj as AccountSettingsNotification;
			if (accountSettingsNotification == null)
			{
				return false;
			}
			if (this.Licenses.Count != accountSettingsNotification.Licenses.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Licenses.Count; i++)
			{
				if (!this.Licenses[i].Equals(accountSettingsNotification.Licenses[i]))
				{
					return false;
				}
			}
			if (this.HasIsUsingRid != accountSettingsNotification.HasIsUsingRid || this.HasIsUsingRid && !this.IsUsingRid.Equals(accountSettingsNotification.IsUsingRid))
			{
				return false;
			}
			if (this.HasIsPlayingFromIgr != accountSettingsNotification.HasIsPlayingFromIgr || this.HasIsPlayingFromIgr && !this.IsPlayingFromIgr.Equals(accountSettingsNotification.IsPlayingFromIgr))
			{
				return false;
			}
			if (this.HasCanReceiveVoice != accountSettingsNotification.HasCanReceiveVoice || this.HasCanReceiveVoice && !this.CanReceiveVoice.Equals(accountSettingsNotification.CanReceiveVoice))
			{
				return false;
			}
			if (this.HasCanSendVoice == accountSettingsNotification.HasCanSendVoice && (!this.HasCanSendVoice || this.CanSendVoice.Equals(accountSettingsNotification.CanSendVoice)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (AccountLicense license in this.Licenses)
			{
				hashCode = hashCode ^ license.GetHashCode();
			}
			if (this.HasIsUsingRid)
			{
				hashCode = hashCode ^ this.IsUsingRid.GetHashCode();
			}
			if (this.HasIsPlayingFromIgr)
			{
				hashCode = hashCode ^ this.IsPlayingFromIgr.GetHashCode();
			}
			if (this.HasCanReceiveVoice)
			{
				hashCode = hashCode ^ this.CanReceiveVoice.GetHashCode();
			}
			if (this.HasCanSendVoice)
			{
				hashCode = hashCode ^ this.CanSendVoice.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Licenses.Count > 0)
			{
				foreach (AccountLicense license in this.Licenses)
				{
					num++;
					uint serializedSize = license.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			if (this.HasIsUsingRid)
			{
				num++;
				num++;
			}
			if (this.HasIsPlayingFromIgr)
			{
				num++;
				num++;
			}
			if (this.HasCanReceiveVoice)
			{
				num++;
				num++;
			}
			if (this.HasCanSendVoice)
			{
				num++;
				num++;
			}
			return num;
		}

		public static AccountSettingsNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<AccountSettingsNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			AccountSettingsNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, AccountSettingsNotification instance)
		{
			if (instance.Licenses.Count > 0)
			{
				foreach (AccountLicense license in instance.Licenses)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, license.GetSerializedSize());
					AccountLicense.Serialize(stream, license);
				}
			}
			if (instance.HasIsUsingRid)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteBool(stream, instance.IsUsingRid);
			}
			if (instance.HasIsPlayingFromIgr)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteBool(stream, instance.IsPlayingFromIgr);
			}
			if (instance.HasCanReceiveVoice)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteBool(stream, instance.CanReceiveVoice);
			}
			if (instance.HasCanSendVoice)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteBool(stream, instance.CanSendVoice);
			}
		}

		public void SetCanReceiveVoice(bool val)
		{
			this.CanReceiveVoice = val;
		}

		public void SetCanSendVoice(bool val)
		{
			this.CanSendVoice = val;
		}

		public void SetIsPlayingFromIgr(bool val)
		{
			this.IsPlayingFromIgr = val;
		}

		public void SetIsUsingRid(bool val)
		{
			this.IsUsingRid = val;
		}

		public void SetLicenses(List<AccountLicense> val)
		{
			this.Licenses = val;
		}
	}
}