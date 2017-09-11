using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.account
{
	public class AccountCredential : IProtoBuf
	{
		public bool HasData;

		private byte[] _Data;

		public byte[] Data
		{
			get
			{
				return this._Data;
			}
			set
			{
				this._Data = value;
				this.HasData = value != null;
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

		public AccountCredential()
		{
		}

		public void Deserialize(Stream stream)
		{
			AccountCredential.Deserialize(stream, this);
		}

		public static AccountCredential Deserialize(Stream stream, AccountCredential instance)
		{
			return AccountCredential.Deserialize(stream, instance, (long)-1);
		}

		public static AccountCredential Deserialize(Stream stream, AccountCredential instance, long limit)
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
						else if (num1 == 18)
						{
							instance.Data = ProtocolParser.ReadBytes(stream);
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

		public static AccountCredential DeserializeLengthDelimited(Stream stream)
		{
			AccountCredential accountCredential = new AccountCredential();
			AccountCredential.DeserializeLengthDelimited(stream, accountCredential);
			return accountCredential;
		}

		public static AccountCredential DeserializeLengthDelimited(Stream stream, AccountCredential instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return AccountCredential.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			AccountCredential accountCredential = obj as AccountCredential;
			if (accountCredential == null)
			{
				return false;
			}
			if (!this.Id.Equals(accountCredential.Id))
			{
				return false;
			}
			if (this.HasData == accountCredential.HasData && (!this.HasData || this.Data.Equals(accountCredential.Data)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Id.GetHashCode();
			if (this.HasData)
			{
				hashCode ^= this.Data.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num += ProtocolParser.SizeOfUInt32(this.Id);
			if (this.HasData)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32((int)this.Data.Length) + (int)this.Data.Length;
			}
			num++;
			return num;
		}

		public static AccountCredential ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<AccountCredential>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			AccountCredential.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, AccountCredential instance)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.Id);
			if (instance.HasData)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteBytes(stream, instance.Data);
			}
		}

		public void SetData(byte[] val)
		{
			this.Data = val;
		}

		public void SetId(uint val)
		{
			this.Id = val;
		}
	}
}