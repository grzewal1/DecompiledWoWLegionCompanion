using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.profanity
{
	public class WordFilter : IProtoBuf
	{
		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public string Regex
		{
			get;
			set;
		}

		public string Type
		{
			get;
			set;
		}

		public WordFilter()
		{
		}

		public void Deserialize(Stream stream)
		{
			WordFilter.Deserialize(stream, this);
		}

		public static WordFilter Deserialize(Stream stream, WordFilter instance)
		{
			return WordFilter.Deserialize(stream, instance, (long)-1);
		}

		public static WordFilter Deserialize(Stream stream, WordFilter instance, long limit)
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
							instance.Type = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 18)
						{
							instance.Regex = ProtocolParser.ReadString(stream);
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

		public static WordFilter DeserializeLengthDelimited(Stream stream)
		{
			WordFilter wordFilter = new WordFilter();
			WordFilter.DeserializeLengthDelimited(stream, wordFilter);
			return wordFilter;
		}

		public static WordFilter DeserializeLengthDelimited(Stream stream, WordFilter instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return WordFilter.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			WordFilter wordFilter = obj as WordFilter;
			if (wordFilter == null)
			{
				return false;
			}
			if (!this.Type.Equals(wordFilter.Type))
			{
				return false;
			}
			if (!this.Regex.Equals(wordFilter.Regex))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Type.GetHashCode();
			return hashCode ^ this.Regex.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Type);
			num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.Regex);
			num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			return num + 2;
		}

		public static WordFilter ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<WordFilter>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			WordFilter.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, WordFilter instance)
		{
			if (instance.Type == null)
			{
				throw new ArgumentNullException("Type", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Type));
			if (instance.Regex == null)
			{
				throw new ArgumentNullException("Regex", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Regex));
		}

		public void SetRegex(string val)
		{
			this.Regex = val;
		}

		public void SetType(string val)
		{
			this.Type = val;
		}
	}
}