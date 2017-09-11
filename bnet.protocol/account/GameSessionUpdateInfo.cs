using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GameSessionUpdateInfo : IProtoBuf
	{
		public bool HasCais;

		private CAIS _Cais;

		public CAIS Cais
		{
			get
			{
				return this._Cais;
			}
			set
			{
				this._Cais = value;
				this.HasCais = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public GameSessionUpdateInfo()
		{
		}

		public void Deserialize(Stream stream)
		{
			GameSessionUpdateInfo.Deserialize(stream, this);
		}

		public static GameSessionUpdateInfo Deserialize(Stream stream, GameSessionUpdateInfo instance)
		{
			return GameSessionUpdateInfo.Deserialize(stream, instance, (long)-1);
		}

		public static GameSessionUpdateInfo Deserialize(Stream stream, GameSessionUpdateInfo instance, long limit)
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
					else if (num != 66)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.Cais != null)
					{
						CAIS.DeserializeLengthDelimited(stream, instance.Cais);
					}
					else
					{
						instance.Cais = CAIS.DeserializeLengthDelimited(stream);
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

		public static GameSessionUpdateInfo DeserializeLengthDelimited(Stream stream)
		{
			GameSessionUpdateInfo gameSessionUpdateInfo = new GameSessionUpdateInfo();
			GameSessionUpdateInfo.DeserializeLengthDelimited(stream, gameSessionUpdateInfo);
			return gameSessionUpdateInfo;
		}

		public static GameSessionUpdateInfo DeserializeLengthDelimited(Stream stream, GameSessionUpdateInfo instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GameSessionUpdateInfo.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameSessionUpdateInfo gameSessionUpdateInfo = obj as GameSessionUpdateInfo;
			if (gameSessionUpdateInfo == null)
			{
				return false;
			}
			if (this.HasCais == gameSessionUpdateInfo.HasCais && (!this.HasCais || this.Cais.Equals(gameSessionUpdateInfo.Cais)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasCais)
			{
				hashCode ^= this.Cais.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasCais)
			{
				num++;
				uint serializedSize = this.Cais.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			return num;
		}

		public static GameSessionUpdateInfo ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameSessionUpdateInfo>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameSessionUpdateInfo.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameSessionUpdateInfo instance)
		{
			if (instance.HasCais)
			{
				stream.WriteByte(66);
				ProtocolParser.WriteUInt32(stream, instance.Cais.GetSerializedSize());
				CAIS.Serialize(stream, instance.Cais);
			}
		}

		public void SetCais(CAIS val)
		{
			this.Cais = val;
		}
	}
}