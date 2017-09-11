using System;
using System.IO;

namespace bnet.protocol.account
{
	public class ProgramTag : IProtoBuf
	{
		public bool HasProgram;

		private uint _Program;

		public bool HasTag;

		private uint _Tag;

		public bool IsInitialized
		{
			get
			{
				return true;
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

		public uint Tag
		{
			get
			{
				return this._Tag;
			}
			set
			{
				this._Tag = value;
				this.HasTag = true;
			}
		}

		public ProgramTag()
		{
		}

		public void Deserialize(Stream stream)
		{
			ProgramTag.Deserialize(stream, this);
		}

		public static ProgramTag Deserialize(Stream stream, ProgramTag instance)
		{
			return ProgramTag.Deserialize(stream, instance, (long)-1);
		}

		public static ProgramTag Deserialize(Stream stream, ProgramTag instance, long limit)
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
						if (num1 == 13)
						{
							instance.Program = binaryReader.ReadUInt32();
						}
						else if (num1 == 21)
						{
							instance.Tag = binaryReader.ReadUInt32();
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

		public static ProgramTag DeserializeLengthDelimited(Stream stream)
		{
			ProgramTag programTag = new ProgramTag();
			ProgramTag.DeserializeLengthDelimited(stream, programTag);
			return programTag;
		}

		public static ProgramTag DeserializeLengthDelimited(Stream stream, ProgramTag instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ProgramTag.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ProgramTag programTag = obj as ProgramTag;
			if (programTag == null)
			{
				return false;
			}
			if (this.HasProgram != programTag.HasProgram || this.HasProgram && !this.Program.Equals(programTag.Program))
			{
				return false;
			}
			if (this.HasTag == programTag.HasTag && (!this.HasTag || this.Tag.Equals(programTag.Tag)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasProgram)
			{
				hashCode ^= this.Program.GetHashCode();
			}
			if (this.HasTag)
			{
				hashCode ^= this.Tag.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasProgram)
			{
				num++;
				num += 4;
			}
			if (this.HasTag)
			{
				num++;
				num += 4;
			}
			return num;
		}

		public static ProgramTag ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ProgramTag>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ProgramTag.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ProgramTag instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasProgram)
			{
				stream.WriteByte(13);
				binaryWriter.Write(instance.Program);
			}
			if (instance.HasTag)
			{
				stream.WriteByte(21);
				binaryWriter.Write(instance.Tag);
			}
		}

		public void SetProgram(uint val)
		{
			this.Program = val;
		}

		public void SetTag(uint val)
		{
			this.Tag = val;
		}
	}
}