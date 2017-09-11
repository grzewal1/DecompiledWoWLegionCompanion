using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GameAccountStateNotification : IProtoBuf
	{
		public bool HasState;

		private GameAccountState _State;

		public bool HasSubscriberId;

		private ulong _SubscriberId;

		public bool HasGameAccountTags;

		private GameAccountFieldTags _GameAccountTags;

		public bool HasSubscriptionCompleted;

		private bool _SubscriptionCompleted;

		public GameAccountFieldTags GameAccountTags
		{
			get
			{
				return this._GameAccountTags;
			}
			set
			{
				this._GameAccountTags = value;
				this.HasGameAccountTags = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public GameAccountState State
		{
			get
			{
				return this._State;
			}
			set
			{
				this._State = value;
				this.HasState = value != null;
			}
		}

		public ulong SubscriberId
		{
			get
			{
				return this._SubscriberId;
			}
			set
			{
				this._SubscriberId = value;
				this.HasSubscriberId = true;
			}
		}

		public bool SubscriptionCompleted
		{
			get
			{
				return this._SubscriptionCompleted;
			}
			set
			{
				this._SubscriptionCompleted = value;
				this.HasSubscriptionCompleted = true;
			}
		}

		public GameAccountStateNotification()
		{
		}

		public void Deserialize(Stream stream)
		{
			GameAccountStateNotification.Deserialize(stream, this);
		}

		public static GameAccountStateNotification Deserialize(Stream stream, GameAccountStateNotification instance)
		{
			return GameAccountStateNotification.Deserialize(stream, instance, (long)-1);
		}

		public static GameAccountStateNotification Deserialize(Stream stream, GameAccountStateNotification instance, long limit)
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
							if (instance.State != null)
							{
								GameAccountState.DeserializeLengthDelimited(stream, instance.State);
							}
							else
							{
								instance.State = GameAccountState.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 16)
						{
							instance.SubscriberId = ProtocolParser.ReadUInt64(stream);
						}
						else if (num1 != 26)
						{
							if (num1 == 32)
							{
								instance.SubscriptionCompleted = ProtocolParser.ReadBool(stream);
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
						else if (instance.GameAccountTags != null)
						{
							GameAccountFieldTags.DeserializeLengthDelimited(stream, instance.GameAccountTags);
						}
						else
						{
							instance.GameAccountTags = GameAccountFieldTags.DeserializeLengthDelimited(stream);
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

		public static GameAccountStateNotification DeserializeLengthDelimited(Stream stream)
		{
			GameAccountStateNotification gameAccountStateNotification = new GameAccountStateNotification();
			GameAccountStateNotification.DeserializeLengthDelimited(stream, gameAccountStateNotification);
			return gameAccountStateNotification;
		}

		public static GameAccountStateNotification DeserializeLengthDelimited(Stream stream, GameAccountStateNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GameAccountStateNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameAccountStateNotification gameAccountStateNotification = obj as GameAccountStateNotification;
			if (gameAccountStateNotification == null)
			{
				return false;
			}
			if (this.HasState != gameAccountStateNotification.HasState || this.HasState && !this.State.Equals(gameAccountStateNotification.State))
			{
				return false;
			}
			if (this.HasSubscriberId != gameAccountStateNotification.HasSubscriberId || this.HasSubscriberId && !this.SubscriberId.Equals(gameAccountStateNotification.SubscriberId))
			{
				return false;
			}
			if (this.HasGameAccountTags != gameAccountStateNotification.HasGameAccountTags || this.HasGameAccountTags && !this.GameAccountTags.Equals(gameAccountStateNotification.GameAccountTags))
			{
				return false;
			}
			if (this.HasSubscriptionCompleted == gameAccountStateNotification.HasSubscriptionCompleted && (!this.HasSubscriptionCompleted || this.SubscriptionCompleted.Equals(gameAccountStateNotification.SubscriptionCompleted)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasState)
			{
				hashCode ^= this.State.GetHashCode();
			}
			if (this.HasSubscriberId)
			{
				hashCode ^= this.SubscriberId.GetHashCode();
			}
			if (this.HasGameAccountTags)
			{
				hashCode ^= this.GameAccountTags.GetHashCode();
			}
			if (this.HasSubscriptionCompleted)
			{
				hashCode ^= this.SubscriptionCompleted.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasState)
			{
				num++;
				uint serializedSize = this.State.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasSubscriberId)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.SubscriberId);
			}
			if (this.HasGameAccountTags)
			{
				num++;
				uint serializedSize1 = this.GameAccountTags.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			if (this.HasSubscriptionCompleted)
			{
				num++;
				num++;
			}
			return num;
		}

		public static GameAccountStateNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameAccountStateNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameAccountStateNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameAccountStateNotification instance)
		{
			if (instance.HasState)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.State.GetSerializedSize());
				GameAccountState.Serialize(stream, instance.State);
			}
			if (instance.HasSubscriberId)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, instance.SubscriberId);
			}
			if (instance.HasGameAccountTags)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteUInt32(stream, instance.GameAccountTags.GetSerializedSize());
				GameAccountFieldTags.Serialize(stream, instance.GameAccountTags);
			}
			if (instance.HasSubscriptionCompleted)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteBool(stream, instance.SubscriptionCompleted);
			}
		}

		public void SetGameAccountTags(GameAccountFieldTags val)
		{
			this.GameAccountTags = val;
		}

		public void SetState(GameAccountState val)
		{
			this.State = val;
		}

		public void SetSubscriberId(ulong val)
		{
			this.SubscriberId = val;
		}

		public void SetSubscriptionCompleted(bool val)
		{
			this.SubscriptionCompleted = val;
		}
	}
}