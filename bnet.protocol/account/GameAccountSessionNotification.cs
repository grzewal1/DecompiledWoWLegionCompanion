using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GameAccountSessionNotification : IProtoBuf
	{
		public bool HasGameAccount;

		private GameAccountHandle _GameAccount;

		public bool HasSessionInfo;

		private GameSessionUpdateInfo _SessionInfo;

		public GameAccountHandle GameAccount
		{
			get
			{
				return this._GameAccount;
			}
			set
			{
				this._GameAccount = value;
				this.HasGameAccount = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public GameSessionUpdateInfo SessionInfo
		{
			get
			{
				return this._SessionInfo;
			}
			set
			{
				this._SessionInfo = value;
				this.HasSessionInfo = value != null;
			}
		}

		public GameAccountSessionNotification()
		{
		}

		public void Deserialize(Stream stream)
		{
			GameAccountSessionNotification.Deserialize(stream, this);
		}

		public static GameAccountSessionNotification Deserialize(Stream stream, GameAccountSessionNotification instance)
		{
			return GameAccountSessionNotification.Deserialize(stream, instance, (long)-1);
		}

		public static GameAccountSessionNotification Deserialize(Stream stream, GameAccountSessionNotification instance, long limit)
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
							if (instance.GameAccount != null)
							{
								GameAccountHandle.DeserializeLengthDelimited(stream, instance.GameAccount);
							}
							else
							{
								instance.GameAccount = GameAccountHandle.DeserializeLengthDelimited(stream);
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
						else if (instance.SessionInfo != null)
						{
							GameSessionUpdateInfo.DeserializeLengthDelimited(stream, instance.SessionInfo);
						}
						else
						{
							instance.SessionInfo = GameSessionUpdateInfo.DeserializeLengthDelimited(stream);
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

		public static GameAccountSessionNotification DeserializeLengthDelimited(Stream stream)
		{
			GameAccountSessionNotification gameAccountSessionNotification = new GameAccountSessionNotification();
			GameAccountSessionNotification.DeserializeLengthDelimited(stream, gameAccountSessionNotification);
			return gameAccountSessionNotification;
		}

		public static GameAccountSessionNotification DeserializeLengthDelimited(Stream stream, GameAccountSessionNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GameAccountSessionNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameAccountSessionNotification gameAccountSessionNotification = obj as GameAccountSessionNotification;
			if (gameAccountSessionNotification == null)
			{
				return false;
			}
			if (this.HasGameAccount != gameAccountSessionNotification.HasGameAccount || this.HasGameAccount && !this.GameAccount.Equals(gameAccountSessionNotification.GameAccount))
			{
				return false;
			}
			if (this.HasSessionInfo == gameAccountSessionNotification.HasSessionInfo && (!this.HasSessionInfo || this.SessionInfo.Equals(gameAccountSessionNotification.SessionInfo)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasGameAccount)
			{
				hashCode = hashCode ^ this.GameAccount.GetHashCode();
			}
			if (this.HasSessionInfo)
			{
				hashCode = hashCode ^ this.SessionInfo.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasGameAccount)
			{
				num++;
				uint serializedSize = this.GameAccount.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasSessionInfo)
			{
				num++;
				uint serializedSize1 = this.SessionInfo.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			return num;
		}

		public static GameAccountSessionNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameAccountSessionNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameAccountSessionNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameAccountSessionNotification instance)
		{
			if (instance.HasGameAccount)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.GameAccount.GetSerializedSize());
				GameAccountHandle.Serialize(stream, instance.GameAccount);
			}
			if (instance.HasSessionInfo)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.SessionInfo.GetSerializedSize());
				GameSessionUpdateInfo.Serialize(stream, instance.SessionInfo);
			}
		}

		public void SetGameAccount(GameAccountHandle val)
		{
			this.GameAccount = val;
		}

		public void SetSessionInfo(GameSessionUpdateInfo val)
		{
			this.SessionInfo = val;
		}
	}
}