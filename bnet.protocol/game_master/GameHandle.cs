using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.game_master
{
	public class GameHandle : IProtoBuf
	{
		public ulong FactoryId
		{
			get;
			set;
		}

		public EntityId GameId
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

		public GameHandle()
		{
		}

		public void Deserialize(Stream stream)
		{
			GameHandle.Deserialize(stream, this);
		}

		public static GameHandle Deserialize(Stream stream, GameHandle instance)
		{
			return GameHandle.Deserialize(stream, instance, (long)-1);
		}

		public static GameHandle Deserialize(Stream stream, GameHandle instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 == 9)
						{
							instance.FactoryId = binaryReader.ReadUInt64();
						}
						else if (num1 != 18)
						{
							Key key = ProtocolParser.ReadKey((byte)num, stream);
							if (key.Field == 0)
							{
								throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
							}
							ProtocolParser.SkipKey(stream, key);
						}
						else if (instance.GameId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.GameId);
						}
						else
						{
							instance.GameId = EntityId.DeserializeLengthDelimited(stream);
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

		public static GameHandle DeserializeLengthDelimited(Stream stream)
		{
			GameHandle gameHandle = new GameHandle();
			GameHandle.DeserializeLengthDelimited(stream, gameHandle);
			return gameHandle;
		}

		public static GameHandle DeserializeLengthDelimited(Stream stream, GameHandle instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GameHandle.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameHandle gameHandle = obj as GameHandle;
			if (gameHandle == null)
			{
				return false;
			}
			if (!this.FactoryId.Equals(gameHandle.FactoryId))
			{
				return false;
			}
			if (!this.GameId.Equals(gameHandle.GameId))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.FactoryId.GetHashCode();
			return hashCode ^ this.GameId.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0 + 8;
			uint serializedSize = this.GameId.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			return num + 2;
		}

		public static GameHandle ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameHandle>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameHandle.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameHandle instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			stream.WriteByte(9);
			binaryWriter.Write(instance.FactoryId);
			if (instance.GameId == null)
			{
				throw new ArgumentNullException("GameId", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.GameId.GetSerializedSize());
			EntityId.Serialize(stream, instance.GameId);
		}

		public void SetFactoryId(ulong val)
		{
			this.FactoryId = val;
		}

		public void SetGameId(EntityId val)
		{
			this.GameId = val;
		}
	}
}