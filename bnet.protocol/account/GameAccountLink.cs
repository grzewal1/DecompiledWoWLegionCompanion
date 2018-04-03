using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.account
{
	public class GameAccountLink : IProtoBuf
	{
		public GameAccountHandle GameAccount
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

		public string Name
		{
			get;
			set;
		}

		public GameAccountLink()
		{
		}

		public void Deserialize(Stream stream)
		{
			GameAccountLink.Deserialize(stream, this);
		}

		public static GameAccountLink Deserialize(Stream stream, GameAccountLink instance)
		{
			return GameAccountLink.Deserialize(stream, instance, (long)-1);
		}

		public static GameAccountLink Deserialize(Stream stream, GameAccountLink instance, long limit)
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
					else if (num != 10)
					{
						if (num == 18)
						{
							instance.Name = ProtocolParser.ReadString(stream);
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
					else if (instance.GameAccount != null)
					{
						GameAccountHandle.DeserializeLengthDelimited(stream, instance.GameAccount);
					}
					else
					{
						instance.GameAccount = GameAccountHandle.DeserializeLengthDelimited(stream);
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

		public static GameAccountLink DeserializeLengthDelimited(Stream stream)
		{
			GameAccountLink gameAccountLink = new GameAccountLink();
			GameAccountLink.DeserializeLengthDelimited(stream, gameAccountLink);
			return gameAccountLink;
		}

		public static GameAccountLink DeserializeLengthDelimited(Stream stream, GameAccountLink instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GameAccountLink.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameAccountLink gameAccountLink = obj as GameAccountLink;
			if (gameAccountLink == null)
			{
				return false;
			}
			if (!this.GameAccount.Equals(gameAccountLink.GameAccount))
			{
				return false;
			}
			if (!this.Name.Equals(gameAccountLink.Name))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.GameAccount.GetHashCode();
			return hashCode ^ this.Name.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.GameAccount.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Name);
			num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			return num + 2;
		}

		public static GameAccountLink ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameAccountLink>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameAccountLink.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameAccountLink instance)
		{
			if (instance.GameAccount == null)
			{
				throw new ArgumentNullException("GameAccount", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.GameAccount.GetSerializedSize());
			GameAccountHandle.Serialize(stream, instance.GameAccount);
			if (instance.Name == null)
			{
				throw new ArgumentNullException("Name", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Name));
		}

		public void SetGameAccount(GameAccountHandle val)
		{
			this.GameAccount = val;
		}

		public void SetName(string val)
		{
			this.Name = val;
		}
	}
}