using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GameAccountState : IProtoBuf
	{
		public bool HasGameLevelInfo;

		private bnet.protocol.account.GameLevelInfo _GameLevelInfo;

		public bool HasGameTimeInfo;

		private bnet.protocol.account.GameTimeInfo _GameTimeInfo;

		public bool HasGameStatus;

		private bnet.protocol.account.GameStatus _GameStatus;

		public bnet.protocol.account.GameLevelInfo GameLevelInfo
		{
			get
			{
				return this._GameLevelInfo;
			}
			set
			{
				this._GameLevelInfo = value;
				this.HasGameLevelInfo = value != null;
			}
		}

		public bnet.protocol.account.GameStatus GameStatus
		{
			get
			{
				return this._GameStatus;
			}
			set
			{
				this._GameStatus = value;
				this.HasGameStatus = value != null;
			}
		}

		public bnet.protocol.account.GameTimeInfo GameTimeInfo
		{
			get
			{
				return this._GameTimeInfo;
			}
			set
			{
				this._GameTimeInfo = value;
				this.HasGameTimeInfo = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public GameAccountState()
		{
		}

		public void Deserialize(Stream stream)
		{
			GameAccountState.Deserialize(stream, this);
		}

		public static GameAccountState Deserialize(Stream stream, GameAccountState instance)
		{
			return GameAccountState.Deserialize(stream, instance, (long)-1);
		}

		public static GameAccountState Deserialize(Stream stream, GameAccountState instance, long limit)
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
							if (instance.GameLevelInfo != null)
							{
								bnet.protocol.account.GameLevelInfo.DeserializeLengthDelimited(stream, instance.GameLevelInfo);
							}
							else
							{
								instance.GameLevelInfo = bnet.protocol.account.GameLevelInfo.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 18)
						{
							if (instance.GameTimeInfo != null)
							{
								bnet.protocol.account.GameTimeInfo.DeserializeLengthDelimited(stream, instance.GameTimeInfo);
							}
							else
							{
								instance.GameTimeInfo = bnet.protocol.account.GameTimeInfo.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 != 26)
						{
							Key key = ProtocolParser.ReadKey((byte)num, stream);
							if (key.Field == 0)
							{
								throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
							}
							ProtocolParser.SkipKey(stream, key);
						}
						else if (instance.GameStatus != null)
						{
							bnet.protocol.account.GameStatus.DeserializeLengthDelimited(stream, instance.GameStatus);
						}
						else
						{
							instance.GameStatus = bnet.protocol.account.GameStatus.DeserializeLengthDelimited(stream);
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

		public static GameAccountState DeserializeLengthDelimited(Stream stream)
		{
			GameAccountState gameAccountState = new GameAccountState();
			GameAccountState.DeserializeLengthDelimited(stream, gameAccountState);
			return gameAccountState;
		}

		public static GameAccountState DeserializeLengthDelimited(Stream stream, GameAccountState instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GameAccountState.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameAccountState gameAccountState = obj as GameAccountState;
			if (gameAccountState == null)
			{
				return false;
			}
			if (this.HasGameLevelInfo != gameAccountState.HasGameLevelInfo || this.HasGameLevelInfo && !this.GameLevelInfo.Equals(gameAccountState.GameLevelInfo))
			{
				return false;
			}
			if (this.HasGameTimeInfo != gameAccountState.HasGameTimeInfo || this.HasGameTimeInfo && !this.GameTimeInfo.Equals(gameAccountState.GameTimeInfo))
			{
				return false;
			}
			if (this.HasGameStatus == gameAccountState.HasGameStatus && (!this.HasGameStatus || this.GameStatus.Equals(gameAccountState.GameStatus)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasGameLevelInfo)
			{
				hashCode = hashCode ^ this.GameLevelInfo.GetHashCode();
			}
			if (this.HasGameTimeInfo)
			{
				hashCode = hashCode ^ this.GameTimeInfo.GetHashCode();
			}
			if (this.HasGameStatus)
			{
				hashCode = hashCode ^ this.GameStatus.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasGameLevelInfo)
			{
				num++;
				uint serializedSize = this.GameLevelInfo.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasGameTimeInfo)
			{
				num++;
				uint serializedSize1 = this.GameTimeInfo.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			if (this.HasGameStatus)
			{
				num++;
				uint num1 = this.GameStatus.GetSerializedSize();
				num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
			}
			return num;
		}

		public static GameAccountState ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameAccountState>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameAccountState.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameAccountState instance)
		{
			if (instance.HasGameLevelInfo)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.GameLevelInfo.GetSerializedSize());
				bnet.protocol.account.GameLevelInfo.Serialize(stream, instance.GameLevelInfo);
			}
			if (instance.HasGameTimeInfo)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.GameTimeInfo.GetSerializedSize());
				bnet.protocol.account.GameTimeInfo.Serialize(stream, instance.GameTimeInfo);
			}
			if (instance.HasGameStatus)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteUInt32(stream, instance.GameStatus.GetSerializedSize());
				bnet.protocol.account.GameStatus.Serialize(stream, instance.GameStatus);
			}
		}

		public void SetGameLevelInfo(bnet.protocol.account.GameLevelInfo val)
		{
			this.GameLevelInfo = val;
		}

		public void SetGameStatus(bnet.protocol.account.GameStatus val)
		{
			this.GameStatus = val;
		}

		public void SetGameTimeInfo(bnet.protocol.account.GameTimeInfo val)
		{
			this.GameTimeInfo = val;
		}
	}
}