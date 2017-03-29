using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.account
{
	public class GameAccountHandle : IProtoBuf
	{
		public uint Id
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

		public uint Program
		{
			get;
			set;
		}

		public uint Region
		{
			get;
			set;
		}

		public GameAccountHandle()
		{
		}

		public void Deserialize(Stream stream)
		{
			GameAccountHandle.Deserialize(stream, this);
		}

		public static GameAccountHandle Deserialize(Stream stream, GameAccountHandle instance)
		{
			return GameAccountHandle.Deserialize(stream, instance, (long)-1);
		}

		public static GameAccountHandle Deserialize(Stream stream, GameAccountHandle instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						switch (num1)
						{
							case 21:
							{
								instance.Program = binaryReader.ReadUInt32();
								continue;
							}
							case 24:
							{
								instance.Region = ProtocolParser.ReadUInt32(stream);
								continue;
							}
							default:
							{
								if (num1 == 13)
								{
									instance.Id = binaryReader.ReadUInt32();
									continue;
								}
								else
								{
									Key key = ProtocolParser.ReadKey((byte)num, stream);
									if (key.Field == 0)
									{
										throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
									}
									ProtocolParser.SkipKey(stream, key);
									continue;
								}
							}
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

		public static GameAccountHandle DeserializeLengthDelimited(Stream stream)
		{
			GameAccountHandle gameAccountHandle = new GameAccountHandle();
			GameAccountHandle.DeserializeLengthDelimited(stream, gameAccountHandle);
			return gameAccountHandle;
		}

		public static GameAccountHandle DeserializeLengthDelimited(Stream stream, GameAccountHandle instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GameAccountHandle.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameAccountHandle gameAccountHandle = obj as GameAccountHandle;
			if (gameAccountHandle == null)
			{
				return false;
			}
			if (!this.Id.Equals(gameAccountHandle.Id))
			{
				return false;
			}
			if (!this.Program.Equals(gameAccountHandle.Program))
			{
				return false;
			}
			if (!this.Region.Equals(gameAccountHandle.Region))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.Id.GetHashCode();
			hashCode = hashCode ^ this.Program.GetHashCode();
			return hashCode ^ this.Region.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0 + 4 + 4;
			num = num + ProtocolParser.SizeOfUInt32(this.Region);
			return num + 3;
		}

		public static GameAccountHandle ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameAccountHandle>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameAccountHandle.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameAccountHandle instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			stream.WriteByte(13);
			binaryWriter.Write(instance.Id);
			stream.WriteByte(21);
			binaryWriter.Write(instance.Program);
			stream.WriteByte(24);
			ProtocolParser.WriteUInt32(stream, instance.Region);
		}

		public void SetId(uint val)
		{
			this.Id = val;
		}

		public void SetProgram(uint val)
		{
			this.Program = val;
		}

		public void SetRegion(uint val)
		{
			this.Region = val;
		}
	}
}