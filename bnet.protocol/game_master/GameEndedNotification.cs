using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.game_master
{
	public class GameEndedNotification : IProtoBuf
	{
		public bool HasReason;

		private uint _Reason;

		public bnet.protocol.game_master.GameHandle GameHandle
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

		public uint Reason
		{
			get
			{
				return this._Reason;
			}
			set
			{
				this._Reason = value;
				this.HasReason = true;
			}
		}

		public GameEndedNotification()
		{
		}

		public void Deserialize(Stream stream)
		{
			GameEndedNotification.Deserialize(stream, this);
		}

		public static GameEndedNotification Deserialize(Stream stream, GameEndedNotification instance)
		{
			return GameEndedNotification.Deserialize(stream, instance, (long)-1);
		}

		public static GameEndedNotification Deserialize(Stream stream, GameEndedNotification instance, long limit)
		{
			instance.Reason = 0;
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 != 10)
						{
							if (num1 == 16)
							{
								instance.Reason = ProtocolParser.ReadUInt32(stream);
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
						else if (instance.GameHandle != null)
						{
							bnet.protocol.game_master.GameHandle.DeserializeLengthDelimited(stream, instance.GameHandle);
						}
						else
						{
							instance.GameHandle = bnet.protocol.game_master.GameHandle.DeserializeLengthDelimited(stream);
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

		public static GameEndedNotification DeserializeLengthDelimited(Stream stream)
		{
			GameEndedNotification gameEndedNotification = new GameEndedNotification();
			GameEndedNotification.DeserializeLengthDelimited(stream, gameEndedNotification);
			return gameEndedNotification;
		}

		public static GameEndedNotification DeserializeLengthDelimited(Stream stream, GameEndedNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GameEndedNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameEndedNotification gameEndedNotification = obj as GameEndedNotification;
			if (gameEndedNotification == null)
			{
				return false;
			}
			if (!this.GameHandle.Equals(gameEndedNotification.GameHandle))
			{
				return false;
			}
			if (this.HasReason == gameEndedNotification.HasReason && (!this.HasReason || this.Reason.Equals(gameEndedNotification.Reason)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.GameHandle.GetHashCode();
			if (this.HasReason)
			{
				hashCode ^= this.Reason.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.GameHandle.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasReason)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.Reason);
			}
			num++;
			return num;
		}

		public static GameEndedNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameEndedNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameEndedNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameEndedNotification instance)
		{
			if (instance.GameHandle == null)
			{
				throw new ArgumentNullException("GameHandle", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.GameHandle.GetSerializedSize());
			bnet.protocol.game_master.GameHandle.Serialize(stream, instance.GameHandle);
			if (instance.HasReason)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.Reason);
			}
		}

		public void SetGameHandle(bnet.protocol.game_master.GameHandle val)
		{
			this.GameHandle = val;
		}

		public void SetReason(uint val)
		{
			this.Reason = val;
		}
	}
}