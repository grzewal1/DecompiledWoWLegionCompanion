using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GameAccountFieldOptions : IProtoBuf
	{
		public bool HasAllFields;

		private bool _AllFields;

		public bool HasFieldGameLevelInfo;

		private bool _FieldGameLevelInfo;

		public bool HasFieldGameTimeInfo;

		private bool _FieldGameTimeInfo;

		public bool HasFieldGameStatus;

		private bool _FieldGameStatus;

		public bool AllFields
		{
			get
			{
				return this._AllFields;
			}
			set
			{
				this._AllFields = value;
				this.HasAllFields = true;
			}
		}

		public bool FieldGameLevelInfo
		{
			get
			{
				return this._FieldGameLevelInfo;
			}
			set
			{
				this._FieldGameLevelInfo = value;
				this.HasFieldGameLevelInfo = true;
			}
		}

		public bool FieldGameStatus
		{
			get
			{
				return this._FieldGameStatus;
			}
			set
			{
				this._FieldGameStatus = value;
				this.HasFieldGameStatus = true;
			}
		}

		public bool FieldGameTimeInfo
		{
			get
			{
				return this._FieldGameTimeInfo;
			}
			set
			{
				this._FieldGameTimeInfo = value;
				this.HasFieldGameTimeInfo = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public GameAccountFieldOptions()
		{
		}

		public void Deserialize(Stream stream)
		{
			GameAccountFieldOptions.Deserialize(stream, this);
		}

		public static GameAccountFieldOptions Deserialize(Stream stream, GameAccountFieldOptions instance)
		{
			return GameAccountFieldOptions.Deserialize(stream, instance, (long)-1);
		}

		public static GameAccountFieldOptions Deserialize(Stream stream, GameAccountFieldOptions instance, long limit)
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
					else if (num == 8)
					{
						instance.AllFields = ProtocolParser.ReadBool(stream);
					}
					else if (num == 16)
					{
						instance.FieldGameLevelInfo = ProtocolParser.ReadBool(stream);
					}
					else if (num == 24)
					{
						instance.FieldGameTimeInfo = ProtocolParser.ReadBool(stream);
					}
					else if (num == 32)
					{
						instance.FieldGameStatus = ProtocolParser.ReadBool(stream);
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

		public static GameAccountFieldOptions DeserializeLengthDelimited(Stream stream)
		{
			GameAccountFieldOptions gameAccountFieldOption = new GameAccountFieldOptions();
			GameAccountFieldOptions.DeserializeLengthDelimited(stream, gameAccountFieldOption);
			return gameAccountFieldOption;
		}

		public static GameAccountFieldOptions DeserializeLengthDelimited(Stream stream, GameAccountFieldOptions instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GameAccountFieldOptions.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameAccountFieldOptions gameAccountFieldOption = obj as GameAccountFieldOptions;
			if (gameAccountFieldOption == null)
			{
				return false;
			}
			if (this.HasAllFields != gameAccountFieldOption.HasAllFields || this.HasAllFields && !this.AllFields.Equals(gameAccountFieldOption.AllFields))
			{
				return false;
			}
			if (this.HasFieldGameLevelInfo != gameAccountFieldOption.HasFieldGameLevelInfo || this.HasFieldGameLevelInfo && !this.FieldGameLevelInfo.Equals(gameAccountFieldOption.FieldGameLevelInfo))
			{
				return false;
			}
			if (this.HasFieldGameTimeInfo != gameAccountFieldOption.HasFieldGameTimeInfo || this.HasFieldGameTimeInfo && !this.FieldGameTimeInfo.Equals(gameAccountFieldOption.FieldGameTimeInfo))
			{
				return false;
			}
			if (this.HasFieldGameStatus == gameAccountFieldOption.HasFieldGameStatus && (!this.HasFieldGameStatus || this.FieldGameStatus.Equals(gameAccountFieldOption.FieldGameStatus)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasAllFields)
			{
				hashCode ^= this.AllFields.GetHashCode();
			}
			if (this.HasFieldGameLevelInfo)
			{
				hashCode ^= this.FieldGameLevelInfo.GetHashCode();
			}
			if (this.HasFieldGameTimeInfo)
			{
				hashCode ^= this.FieldGameTimeInfo.GetHashCode();
			}
			if (this.HasFieldGameStatus)
			{
				hashCode ^= this.FieldGameStatus.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasAllFields)
			{
				num++;
				num++;
			}
			if (this.HasFieldGameLevelInfo)
			{
				num++;
				num++;
			}
			if (this.HasFieldGameTimeInfo)
			{
				num++;
				num++;
			}
			if (this.HasFieldGameStatus)
			{
				num++;
				num++;
			}
			return num;
		}

		public static GameAccountFieldOptions ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameAccountFieldOptions>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameAccountFieldOptions.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameAccountFieldOptions instance)
		{
			if (instance.HasAllFields)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteBool(stream, instance.AllFields);
			}
			if (instance.HasFieldGameLevelInfo)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteBool(stream, instance.FieldGameLevelInfo);
			}
			if (instance.HasFieldGameTimeInfo)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteBool(stream, instance.FieldGameTimeInfo);
			}
			if (instance.HasFieldGameStatus)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteBool(stream, instance.FieldGameStatus);
			}
		}

		public void SetAllFields(bool val)
		{
			this.AllFields = val;
		}

		public void SetFieldGameLevelInfo(bool val)
		{
			this.FieldGameLevelInfo = val;
		}

		public void SetFieldGameStatus(bool val)
		{
			this.FieldGameStatus = val;
		}

		public void SetFieldGameTimeInfo(bool val)
		{
			this.FieldGameTimeInfo = val;
		}
	}
}