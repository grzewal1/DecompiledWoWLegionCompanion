using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.attribute
{
	public class Attribute : IProtoBuf
	{
		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public string Name
		{
			get;
			set;
		}

		public bnet.protocol.attribute.Variant Value
		{
			get;
			set;
		}

		public Attribute()
		{
		}

		public void Deserialize(Stream stream)
		{
			bnet.protocol.attribute.Attribute.Deserialize(stream, this);
		}

		public static bnet.protocol.attribute.Attribute Deserialize(Stream stream, bnet.protocol.attribute.Attribute instance)
		{
			return bnet.protocol.attribute.Attribute.Deserialize(stream, instance, (long)-1);
		}

		public static bnet.protocol.attribute.Attribute Deserialize(Stream stream, bnet.protocol.attribute.Attribute instance, long limit)
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
							instance.Name = ProtocolParser.ReadString(stream);
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
						else if (instance.Value != null)
						{
							bnet.protocol.attribute.Variant.DeserializeLengthDelimited(stream, instance.Value);
						}
						else
						{
							instance.Value = bnet.protocol.attribute.Variant.DeserializeLengthDelimited(stream);
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

		public static bnet.protocol.attribute.Attribute DeserializeLengthDelimited(Stream stream)
		{
			bnet.protocol.attribute.Attribute attribute = new bnet.protocol.attribute.Attribute();
			bnet.protocol.attribute.Attribute.DeserializeLengthDelimited(stream, attribute);
			return attribute;
		}

		public static bnet.protocol.attribute.Attribute DeserializeLengthDelimited(Stream stream, bnet.protocol.attribute.Attribute instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return bnet.protocol.attribute.Attribute.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			bnet.protocol.attribute.Attribute attribute = obj as bnet.protocol.attribute.Attribute;
			if (attribute == null)
			{
				return false;
			}
			if (!this.Name.Equals(attribute.Name))
			{
				return false;
			}
			if (!this.Value.Equals(attribute.Value))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.Name.GetHashCode();
			return hashCode ^ this.Value.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Name);
			num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			uint serializedSize = this.Value.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			return num + 2;
		}

		public static bnet.protocol.attribute.Attribute ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<bnet.protocol.attribute.Attribute>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			bnet.protocol.attribute.Attribute.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, bnet.protocol.attribute.Attribute instance)
		{
			if (instance.Name == null)
			{
				throw new ArgumentNullException("Name", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Name));
			if (instance.Value == null)
			{
				throw new ArgumentNullException("Value", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.Value.GetSerializedSize());
			bnet.protocol.attribute.Variant.Serialize(stream, instance.Value);
		}

		public void SetName(string val)
		{
			this.Name = val;
		}

		public void SetValue(bnet.protocol.attribute.Variant val)
		{
			this.Value = val;
		}
	}
}