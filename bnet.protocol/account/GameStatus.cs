using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GameStatus : IProtoBuf
	{
		public bool HasIsSuspended;

		private bool _IsSuspended;

		public bool HasIsBanned;

		private bool _IsBanned;

		public bool HasSuspensionExpires;

		private ulong _SuspensionExpires;

		public bool HasProgram;

		private uint _Program;

		public bool IsBanned
		{
			get
			{
				return this._IsBanned;
			}
			set
			{
				this._IsBanned = value;
				this.HasIsBanned = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public bool IsSuspended
		{
			get
			{
				return this._IsSuspended;
			}
			set
			{
				this._IsSuspended = value;
				this.HasIsSuspended = true;
			}
		}

		public uint Program
		{
			get
			{
				return this._Program;
			}
			set
			{
				this._Program = value;
				this.HasProgram = true;
			}
		}

		public ulong SuspensionExpires
		{
			get
			{
				return this._SuspensionExpires;
			}
			set
			{
				this._SuspensionExpires = value;
				this.HasSuspensionExpires = true;
			}
		}

		public GameStatus()
		{
		}

		public void Deserialize(Stream stream)
		{
			GameStatus.Deserialize(stream, this);
		}

		public static GameStatus Deserialize(Stream stream, GameStatus instance)
		{
			return GameStatus.Deserialize(stream, instance, (long)-1);
		}

		public static GameStatus Deserialize(Stream stream, GameStatus instance, long limit)
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
					else if (num == 32)
					{
						instance.IsSuspended = ProtocolParser.ReadBool(stream);
					}
					else if (num == 40)
					{
						instance.IsBanned = ProtocolParser.ReadBool(stream);
					}
					else if (num == 48)
					{
						instance.SuspensionExpires = ProtocolParser.ReadUInt64(stream);
					}
					else if (num == 61)
					{
						instance.Program = binaryReader.ReadUInt32();
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

		public static GameStatus DeserializeLengthDelimited(Stream stream)
		{
			GameStatus gameStatu = new GameStatus();
			GameStatus.DeserializeLengthDelimited(stream, gameStatu);
			return gameStatu;
		}

		public static GameStatus DeserializeLengthDelimited(Stream stream, GameStatus instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GameStatus.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameStatus gameStatu = obj as GameStatus;
			if (gameStatu == null)
			{
				return false;
			}
			if (this.HasIsSuspended != gameStatu.HasIsSuspended || this.HasIsSuspended && !this.IsSuspended.Equals(gameStatu.IsSuspended))
			{
				return false;
			}
			if (this.HasIsBanned != gameStatu.HasIsBanned || this.HasIsBanned && !this.IsBanned.Equals(gameStatu.IsBanned))
			{
				return false;
			}
			if (this.HasSuspensionExpires != gameStatu.HasSuspensionExpires || this.HasSuspensionExpires && !this.SuspensionExpires.Equals(gameStatu.SuspensionExpires))
			{
				return false;
			}
			if (this.HasProgram == gameStatu.HasProgram && (!this.HasProgram || this.Program.Equals(gameStatu.Program)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasIsSuspended)
			{
				hashCode ^= this.IsSuspended.GetHashCode();
			}
			if (this.HasIsBanned)
			{
				hashCode ^= this.IsBanned.GetHashCode();
			}
			if (this.HasSuspensionExpires)
			{
				hashCode ^= this.SuspensionExpires.GetHashCode();
			}
			if (this.HasProgram)
			{
				hashCode ^= this.Program.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasIsSuspended)
			{
				num++;
				num++;
			}
			if (this.HasIsBanned)
			{
				num++;
				num++;
			}
			if (this.HasSuspensionExpires)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.SuspensionExpires);
			}
			if (this.HasProgram)
			{
				num++;
				num += 4;
			}
			return num;
		}

		public static GameStatus ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameStatus>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameStatus.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameStatus instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasIsSuspended)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteBool(stream, instance.IsSuspended);
			}
			if (instance.HasIsBanned)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteBool(stream, instance.IsBanned);
			}
			if (instance.HasSuspensionExpires)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteUInt64(stream, instance.SuspensionExpires);
			}
			if (instance.HasProgram)
			{
				stream.WriteByte(61);
				binaryWriter.Write(instance.Program);
			}
		}

		public void SetIsBanned(bool val)
		{
			this.IsBanned = val;
		}

		public void SetIsSuspended(bool val)
		{
			this.IsSuspended = val;
		}

		public void SetProgram(uint val)
		{
			this.Program = val;
		}

		public void SetSuspensionExpires(ulong val)
		{
			this.SuspensionExpires = val;
		}
	}
}