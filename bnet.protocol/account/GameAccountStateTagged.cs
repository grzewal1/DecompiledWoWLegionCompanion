using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GameAccountStateTagged : IProtoBuf
	{
		public bool HasGameAccountState;

		private bnet.protocol.account.GameAccountState _GameAccountState;

		public bool HasGameAccountTags;

		private GameAccountFieldTags _GameAccountTags;

		public bnet.protocol.account.GameAccountState GameAccountState
		{
			get
			{
				return this._GameAccountState;
			}
			set
			{
				this._GameAccountState = value;
				this.HasGameAccountState = value != null;
			}
		}

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

		public GameAccountStateTagged()
		{
		}

		public void Deserialize(Stream stream)
		{
			GameAccountStateTagged.Deserialize(stream, this);
		}

		public static GameAccountStateTagged Deserialize(Stream stream, GameAccountStateTagged instance)
		{
			return GameAccountStateTagged.Deserialize(stream, instance, (long)-1);
		}

		public static GameAccountStateTagged Deserialize(Stream stream, GameAccountStateTagged instance, long limit)
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
							if (instance.GameAccountState != null)
							{
								bnet.protocol.account.GameAccountState.DeserializeLengthDelimited(stream, instance.GameAccountState);
							}
							else
							{
								instance.GameAccountState = bnet.protocol.account.GameAccountState.DeserializeLengthDelimited(stream);
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

		public static GameAccountStateTagged DeserializeLengthDelimited(Stream stream)
		{
			GameAccountStateTagged gameAccountStateTagged = new GameAccountStateTagged();
			GameAccountStateTagged.DeserializeLengthDelimited(stream, gameAccountStateTagged);
			return gameAccountStateTagged;
		}

		public static GameAccountStateTagged DeserializeLengthDelimited(Stream stream, GameAccountStateTagged instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GameAccountStateTagged.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameAccountStateTagged gameAccountStateTagged = obj as GameAccountStateTagged;
			if (gameAccountStateTagged == null)
			{
				return false;
			}
			if (this.HasGameAccountState != gameAccountStateTagged.HasGameAccountState || this.HasGameAccountState && !this.GameAccountState.Equals(gameAccountStateTagged.GameAccountState))
			{
				return false;
			}
			if (this.HasGameAccountTags == gameAccountStateTagged.HasGameAccountTags && (!this.HasGameAccountTags || this.GameAccountTags.Equals(gameAccountStateTagged.GameAccountTags)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasGameAccountState)
			{
				hashCode ^= this.GameAccountState.GetHashCode();
			}
			if (this.HasGameAccountTags)
			{
				hashCode ^= this.GameAccountTags.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasGameAccountState)
			{
				num++;
				uint serializedSize = this.GameAccountState.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasGameAccountTags)
			{
				num++;
				uint serializedSize1 = this.GameAccountTags.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			return num;
		}

		public static GameAccountStateTagged ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameAccountStateTagged>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameAccountStateTagged.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameAccountStateTagged instance)
		{
			if (instance.HasGameAccountState)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.GameAccountState.GetSerializedSize());
				bnet.protocol.account.GameAccountState.Serialize(stream, instance.GameAccountState);
			}
			if (instance.HasGameAccountTags)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.GameAccountTags.GetSerializedSize());
				GameAccountFieldTags.Serialize(stream, instance.GameAccountTags);
			}
		}

		public void SetGameAccountState(bnet.protocol.account.GameAccountState val)
		{
			this.GameAccountState = val;
		}

		public void SetGameAccountTags(GameAccountFieldTags val)
		{
			this.GameAccountTags = val;
		}
	}
}