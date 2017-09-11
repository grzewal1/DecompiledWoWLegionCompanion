using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.account
{
	public class AccountServiceRegion : IProtoBuf
	{
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

		public string Shard
		{
			get;
			set;
		}

		public AccountServiceRegion()
		{
		}

		public void Deserialize(Stream stream)
		{
			AccountServiceRegion.Deserialize(stream, this);
		}

		public static AccountServiceRegion Deserialize(Stream stream, AccountServiceRegion instance)
		{
			return AccountServiceRegion.Deserialize(stream, instance, (long)-1);
		}

		public static AccountServiceRegion Deserialize(Stream stream, AccountServiceRegion instance, long limit)
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
							instance.Shard = ProtocolParser.ReadString(stream);
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

		public static AccountServiceRegion DeserializeLengthDelimited(Stream stream)
		{
			AccountServiceRegion accountServiceRegion = new AccountServiceRegion();
			AccountServiceRegion.DeserializeLengthDelimited(stream, accountServiceRegion);
			return accountServiceRegion;
		}

		public static AccountServiceRegion DeserializeLengthDelimited(Stream stream, AccountServiceRegion instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return AccountServiceRegion.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			AccountServiceRegion accountServiceRegion = obj as AccountServiceRegion;
			if (accountServiceRegion == null)
			{
				return false;
			}
			if (!this.Id.Equals(accountServiceRegion.Id))
			{
				return false;
			}
			if (!this.Shard.Equals(accountServiceRegion.Shard))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Id.GetHashCode();
			return hashCode ^ this.Shard.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0 + ProtocolParser.SizeOfUInt32(this.Id);
			uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Shard);
			num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			return num + 2;
		}

		public static AccountServiceRegion ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<AccountServiceRegion>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			AccountServiceRegion.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, AccountServiceRegion instance)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.Id);
			if (instance.Shard == null)
			{
				throw new ArgumentNullException("Shard", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Shard));
		}

		public void SetId(uint val)
		{
			this.Id = val;
		}

		public void SetShard(string val)
		{
			this.Shard = val;
		}
	}
}