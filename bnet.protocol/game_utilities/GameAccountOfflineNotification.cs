using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.game_utilities
{
	public class GameAccountOfflineNotification : IProtoBuf
	{
		public bool HasHost;

		private ProcessId _Host;

		public EntityId GameAccountId
		{
			get;
			set;
		}

		public ProcessId Host
		{
			get
			{
				return this._Host;
			}
			set
			{
				this._Host = value;
				this.HasHost = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public GameAccountOfflineNotification()
		{
		}

		public void Deserialize(Stream stream)
		{
			GameAccountOfflineNotification.Deserialize(stream, this);
		}

		public static GameAccountOfflineNotification Deserialize(Stream stream, GameAccountOfflineNotification instance)
		{
			return GameAccountOfflineNotification.Deserialize(stream, instance, (long)-1);
		}

		public static GameAccountOfflineNotification Deserialize(Stream stream, GameAccountOfflineNotification instance, long limit)
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
							if (instance.GameAccountId != null)
							{
								EntityId.DeserializeLengthDelimited(stream, instance.GameAccountId);
							}
							else
							{
								instance.GameAccountId = EntityId.DeserializeLengthDelimited(stream);
							}
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
						else if (instance.Host != null)
						{
							ProcessId.DeserializeLengthDelimited(stream, instance.Host);
						}
						else
						{
							instance.Host = ProcessId.DeserializeLengthDelimited(stream);
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

		public static GameAccountOfflineNotification DeserializeLengthDelimited(Stream stream)
		{
			GameAccountOfflineNotification gameAccountOfflineNotification = new GameAccountOfflineNotification();
			GameAccountOfflineNotification.DeserializeLengthDelimited(stream, gameAccountOfflineNotification);
			return gameAccountOfflineNotification;
		}

		public static GameAccountOfflineNotification DeserializeLengthDelimited(Stream stream, GameAccountOfflineNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GameAccountOfflineNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameAccountOfflineNotification gameAccountOfflineNotification = obj as GameAccountOfflineNotification;
			if (gameAccountOfflineNotification == null)
			{
				return false;
			}
			if (!this.GameAccountId.Equals(gameAccountOfflineNotification.GameAccountId))
			{
				return false;
			}
			if (this.HasHost == gameAccountOfflineNotification.HasHost && (!this.HasHost || this.Host.Equals(gameAccountOfflineNotification.Host)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.GameAccountId.GetHashCode();
			if (this.HasHost)
			{
				hashCode ^= this.Host.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.GameAccountId.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasHost)
			{
				num++;
				uint serializedSize1 = this.Host.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			num++;
			return num;
		}

		public static GameAccountOfflineNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameAccountOfflineNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameAccountOfflineNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameAccountOfflineNotification instance)
		{
			if (instance.GameAccountId == null)
			{
				throw new ArgumentNullException("GameAccountId", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.GameAccountId.GetSerializedSize());
			EntityId.Serialize(stream, instance.GameAccountId);
			if (instance.HasHost)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.Host.GetSerializedSize());
				ProcessId.Serialize(stream, instance.Host);
			}
		}

		public void SetGameAccountId(EntityId val)
		{
			this.GameAccountId = val;
		}

		public void SetHost(ProcessId val)
		{
			this.Host = val;
		}
	}
}