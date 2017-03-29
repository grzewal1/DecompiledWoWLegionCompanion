using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.account
{
	public class AccountId : IProtoBuf
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

		public AccountId()
		{
		}

		public void Deserialize(Stream stream)
		{
			AccountId.Deserialize(stream, this);
		}

		public static AccountId Deserialize(Stream stream, AccountId instance)
		{
			return AccountId.Deserialize(stream, instance, (long)-1);
		}

		public static AccountId Deserialize(Stream stream, AccountId instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
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
					else if (num == 13)
					{
						instance.Id = binaryReader.ReadUInt32();
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
					if (stream.Position != limit)
					{
						throw new ProtocolBufferException("Read past max limit");
					}
					break;
				}
			}
			return instance;
		}

		public static AccountId DeserializeLengthDelimited(Stream stream)
		{
			AccountId accountId = new AccountId();
			AccountId.DeserializeLengthDelimited(stream, accountId);
			return accountId;
		}

		public static AccountId DeserializeLengthDelimited(Stream stream, AccountId instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return AccountId.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			AccountId accountId = obj as AccountId;
			if (accountId == null)
			{
				return false;
			}
			if (!this.Id.Equals(accountId.Id))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			return hashCode ^ this.Id.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			return 0 + 4 + 1;
		}

		public static AccountId ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<AccountId>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			AccountId.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, AccountId instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			stream.WriteByte(13);
			binaryWriter.Write(instance.Id);
		}

		public void SetId(uint val)
		{
			this.Id = val;
		}
	}
}