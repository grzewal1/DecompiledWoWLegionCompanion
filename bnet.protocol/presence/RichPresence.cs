using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.presence
{
	public class RichPresence : IProtoBuf
	{
		public uint Index
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

		public uint ProgramId
		{
			get;
			set;
		}

		public uint StreamId
		{
			get;
			set;
		}

		public RichPresence()
		{
		}

		public void Deserialize(Stream stream)
		{
			RichPresence.Deserialize(stream, this);
		}

		public static RichPresence Deserialize(Stream stream, RichPresence instance)
		{
			return RichPresence.Deserialize(stream, instance, (long)-1);
		}

		public static RichPresence Deserialize(Stream stream, RichPresence instance, long limit)
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
								instance.StreamId = binaryReader.ReadUInt32();
								continue;
							}
							case 24:
							{
								instance.Index = ProtocolParser.ReadUInt32(stream);
								continue;
							}
							default:
							{
								if (num1 == 13)
								{
									instance.ProgramId = binaryReader.ReadUInt32();
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

		public static RichPresence DeserializeLengthDelimited(Stream stream)
		{
			RichPresence richPresence = new RichPresence();
			RichPresence.DeserializeLengthDelimited(stream, richPresence);
			return richPresence;
		}

		public static RichPresence DeserializeLengthDelimited(Stream stream, RichPresence instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return RichPresence.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			RichPresence richPresence = obj as RichPresence;
			if (richPresence == null)
			{
				return false;
			}
			if (!this.ProgramId.Equals(richPresence.ProgramId))
			{
				return false;
			}
			if (!this.StreamId.Equals(richPresence.StreamId))
			{
				return false;
			}
			if (!this.Index.Equals(richPresence.Index))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.ProgramId.GetHashCode();
			hashCode = hashCode ^ this.StreamId.GetHashCode();
			return hashCode ^ this.Index.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0 + 4 + 4;
			num = num + ProtocolParser.SizeOfUInt32(this.Index);
			return num + 3;
		}

		public static RichPresence ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<RichPresence>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			RichPresence.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, RichPresence instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			stream.WriteByte(13);
			binaryWriter.Write(instance.ProgramId);
			stream.WriteByte(21);
			binaryWriter.Write(instance.StreamId);
			stream.WriteByte(24);
			ProtocolParser.WriteUInt32(stream, instance.Index);
		}

		public void SetIndex(uint val)
		{
			this.Index = val;
		}

		public void SetProgramId(uint val)
		{
			this.ProgramId = val;
		}

		public void SetStreamId(uint val)
		{
			this.StreamId = val;
		}
	}
}