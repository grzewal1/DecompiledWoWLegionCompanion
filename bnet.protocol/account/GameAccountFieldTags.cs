using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GameAccountFieldTags : IProtoBuf
	{
		public bool HasGameLevelInfoTag;

		private uint _GameLevelInfoTag;

		public bool HasGameTimeInfoTag;

		private uint _GameTimeInfoTag;

		public bool HasGameStatusTag;

		private uint _GameStatusTag;

		public uint GameLevelInfoTag
		{
			get
			{
				return this._GameLevelInfoTag;
			}
			set
			{
				this._GameLevelInfoTag = value;
				this.HasGameLevelInfoTag = true;
			}
		}

		public uint GameStatusTag
		{
			get
			{
				return this._GameStatusTag;
			}
			set
			{
				this._GameStatusTag = value;
				this.HasGameStatusTag = true;
			}
		}

		public uint GameTimeInfoTag
		{
			get
			{
				return this._GameTimeInfoTag;
			}
			set
			{
				this._GameTimeInfoTag = value;
				this.HasGameTimeInfoTag = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public GameAccountFieldTags()
		{
		}

		public void Deserialize(Stream stream)
		{
			GameAccountFieldTags.Deserialize(stream, this);
		}

		public static GameAccountFieldTags Deserialize(Stream stream, GameAccountFieldTags instance)
		{
			return GameAccountFieldTags.Deserialize(stream, instance, (long)-1);
		}

		public static GameAccountFieldTags Deserialize(Stream stream, GameAccountFieldTags instance, long limit)
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
					else if (num == 21)
					{
						instance.GameLevelInfoTag = binaryReader.ReadUInt32();
					}
					else if (num == 29)
					{
						instance.GameTimeInfoTag = binaryReader.ReadUInt32();
					}
					else if (num == 37)
					{
						instance.GameStatusTag = binaryReader.ReadUInt32();
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

		public static GameAccountFieldTags DeserializeLengthDelimited(Stream stream)
		{
			GameAccountFieldTags gameAccountFieldTag = new GameAccountFieldTags();
			GameAccountFieldTags.DeserializeLengthDelimited(stream, gameAccountFieldTag);
			return gameAccountFieldTag;
		}

		public static GameAccountFieldTags DeserializeLengthDelimited(Stream stream, GameAccountFieldTags instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GameAccountFieldTags.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameAccountFieldTags gameAccountFieldTag = obj as GameAccountFieldTags;
			if (gameAccountFieldTag == null)
			{
				return false;
			}
			if (this.HasGameLevelInfoTag != gameAccountFieldTag.HasGameLevelInfoTag || this.HasGameLevelInfoTag && !this.GameLevelInfoTag.Equals(gameAccountFieldTag.GameLevelInfoTag))
			{
				return false;
			}
			if (this.HasGameTimeInfoTag != gameAccountFieldTag.HasGameTimeInfoTag || this.HasGameTimeInfoTag && !this.GameTimeInfoTag.Equals(gameAccountFieldTag.GameTimeInfoTag))
			{
				return false;
			}
			if (this.HasGameStatusTag == gameAccountFieldTag.HasGameStatusTag && (!this.HasGameStatusTag || this.GameStatusTag.Equals(gameAccountFieldTag.GameStatusTag)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasGameLevelInfoTag)
			{
				hashCode ^= this.GameLevelInfoTag.GetHashCode();
			}
			if (this.HasGameTimeInfoTag)
			{
				hashCode ^= this.GameTimeInfoTag.GetHashCode();
			}
			if (this.HasGameStatusTag)
			{
				hashCode ^= this.GameStatusTag.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasGameLevelInfoTag)
			{
				num++;
				num += 4;
			}
			if (this.HasGameTimeInfoTag)
			{
				num++;
				num += 4;
			}
			if (this.HasGameStatusTag)
			{
				num++;
				num += 4;
			}
			return num;
		}

		public static GameAccountFieldTags ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameAccountFieldTags>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameAccountFieldTags.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameAccountFieldTags instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasGameLevelInfoTag)
			{
				stream.WriteByte(21);
				binaryWriter.Write(instance.GameLevelInfoTag);
			}
			if (instance.HasGameTimeInfoTag)
			{
				stream.WriteByte(29);
				binaryWriter.Write(instance.GameTimeInfoTag);
			}
			if (instance.HasGameStatusTag)
			{
				stream.WriteByte(37);
				binaryWriter.Write(instance.GameStatusTag);
			}
		}

		public void SetGameLevelInfoTag(uint val)
		{
			this.GameLevelInfoTag = val;
		}

		public void SetGameStatusTag(uint val)
		{
			this.GameStatusTag = val;
		}

		public void SetGameTimeInfoTag(uint val)
		{
			this.GameTimeInfoTag = val;
		}
	}
}