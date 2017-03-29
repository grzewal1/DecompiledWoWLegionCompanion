using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.account
{
	public class AccountLicense : IProtoBuf
	{
		public bool HasExpires;

		private ulong _Expires;

		public ulong Expires
		{
			get
			{
				return this._Expires;
			}
			set
			{
				this._Expires = value;
				this.HasExpires = true;
			}
		}

		public uint Id
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

		public AccountLicense()
		{
		}

		public void Deserialize(Stream stream)
		{
			AccountLicense.Deserialize(stream, this);
		}

		public static AccountLicense Deserialize(Stream stream, AccountLicense instance)
		{
			return AccountLicense.Deserialize(stream, instance, (long)-1);
		}

		public static AccountLicense Deserialize(Stream stream, AccountLicense instance, long limit)
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
							instance.Id = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 16)
						{
							instance.Expires = ProtocolParser.ReadUInt64(stream);
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

		public static AccountLicense DeserializeLengthDelimited(Stream stream)
		{
			AccountLicense accountLicense = new AccountLicense();
			AccountLicense.DeserializeLengthDelimited(stream, accountLicense);
			return accountLicense;
		}

		public static AccountLicense DeserializeLengthDelimited(Stream stream, AccountLicense instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return AccountLicense.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			AccountLicense accountLicense = obj as AccountLicense;
			if (accountLicense == null)
			{
				return false;
			}
			if (!this.Id.Equals(accountLicense.Id))
			{
				return false;
			}
			if (this.HasExpires == accountLicense.HasExpires && (!this.HasExpires || this.Expires.Equals(accountLicense.Expires)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.Id.GetHashCode();
			if (this.HasExpires)
			{
				hashCode = hashCode ^ this.Expires.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num = num + ProtocolParser.SizeOfUInt32(this.Id);
			if (this.HasExpires)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt64(this.Expires);
			}
			num++;
			return num;
		}

		public static AccountLicense ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<AccountLicense>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			AccountLicense.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, AccountLicense instance)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.Id);
			if (instance.HasExpires)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, instance.Expires);
			}
		}

		public void SetExpires(ulong val)
		{
			this.Expires = val;
		}

		public void SetId(uint val)
		{
			this.Id = val;
		}
	}
}