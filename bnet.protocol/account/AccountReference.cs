using System;
using System.IO;
using System.Text;

namespace bnet.protocol.account
{
	public class AccountReference : IProtoBuf
	{
		public bool HasId;

		private uint _Id;

		public bool HasEmail;

		private string _Email;

		public bool HasHandle;

		private GameAccountHandle _Handle;

		public bool HasBattleTag;

		private string _BattleTag;

		public bool HasRegion;

		private uint _Region;

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

		public string Email
		{
			get
			{
				return this._Email;
			}
			set
			{
				this._Email = value;
				this.HasEmail = value != null;
			}
		}

		public GameAccountHandle Handle
		{
			get
			{
				return this._Handle;
			}
			set
			{
				this._Handle = value;
				this.HasHandle = value != null;
			}
		}

		public uint Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				this._Id = value;
				this.HasId = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
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

		public AccountReference()
		{
		}

		public void Deserialize(Stream stream)
		{
			AccountReference.Deserialize(stream, this);
		}

		public static AccountReference Deserialize(Stream stream, AccountReference instance)
		{
			return AccountReference.Deserialize(stream, instance, (long)-1);
		}

		public static AccountReference Deserialize(Stream stream, AccountReference instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			instance.Region = 0;
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 == 13)
						{
							instance.Id = binaryReader.ReadUInt32();
						}
						else if (num1 == 18)
						{
							instance.Email = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 26)
						{
							if (instance.Handle != null)
							{
								GameAccountHandle.DeserializeLengthDelimited(stream, instance.Handle);
							}
							else
							{
								instance.Handle = GameAccountHandle.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 34)
						{
							instance.BattleTag = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 80)
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

		public static AccountReference DeserializeLengthDelimited(Stream stream)
		{
			AccountReference accountReference = new AccountReference();
			AccountReference.DeserializeLengthDelimited(stream, accountReference);
			return accountReference;
		}

		public static AccountReference DeserializeLengthDelimited(Stream stream, AccountReference instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return AccountReference.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			AccountReference accountReference = obj as AccountReference;
			if (accountReference == null)
			{
				return false;
			}
			if (this.HasId != accountReference.HasId || this.HasId && !this.Id.Equals(accountReference.Id))
			{
				return false;
			}
			if (this.HasEmail != accountReference.HasEmail || this.HasEmail && !this.Email.Equals(accountReference.Email))
			{
				return false;
			}
			if (this.HasHandle != accountReference.HasHandle || this.HasHandle && !this.Handle.Equals(accountReference.Handle))
			{
				return false;
			}
			if (this.HasBattleTag != accountReference.HasBattleTag || this.HasBattleTag && !this.BattleTag.Equals(accountReference.BattleTag))
			{
				return false;
			}
			if (this.HasRegion == accountReference.HasRegion && (!this.HasRegion || this.Region.Equals(accountReference.Region)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasId)
			{
				hashCode = hashCode ^ this.Id.GetHashCode();
			}
			if (this.HasEmail)
			{
				hashCode = hashCode ^ this.Email.GetHashCode();
			}
			if (this.HasHandle)
			{
				hashCode = hashCode ^ this.Handle.GetHashCode();
			}
			if (this.HasBattleTag)
			{
				hashCode = hashCode ^ this.BattleTag.GetHashCode();
			}
			if (this.HasRegion)
			{
				hashCode = hashCode ^ this.Region.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasId)
			{
				num++;
				num = num + 4;
			}
			if (this.HasEmail)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Email);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasHandle)
			{
				num++;
				uint serializedSize = this.Handle.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasBattleTag)
			{
				num++;
				uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.BattleTag);
				num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			}
			if (this.HasRegion)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.Region);
			}
			return num;
		}

		public static AccountReference ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<AccountReference>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			AccountReference.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, AccountReference instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasId)
			{
				stream.WriteByte(13);
				binaryWriter.Write(instance.Id);
			}
			if (instance.HasEmail)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Email));
			}
			if (instance.HasHandle)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteUInt32(stream, instance.Handle.GetSerializedSize());
				GameAccountHandle.Serialize(stream, instance.Handle);
			}
			if (instance.HasBattleTag)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.BattleTag));
			}
			if (instance.HasRegion)
			{
				stream.WriteByte(80);
				ProtocolParser.WriteUInt32(stream, instance.Region);
			}
		}

		public void SetBattleTag(string val)
		{
			this.BattleTag = val;
		}

		public void SetEmail(string val)
		{
			this.Email = val;
		}

		public void SetHandle(GameAccountHandle val)
		{
			this.Handle = val;
		}

		public void SetId(uint val)
		{
			this.Id = val;
		}

		public void SetRegion(uint val)
		{
			this.Region = val;
		}
	}
}