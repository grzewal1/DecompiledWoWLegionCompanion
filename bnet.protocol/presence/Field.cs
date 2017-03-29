using bnet.protocol.attribute;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.presence
{
	public class Field : IProtoBuf
	{
		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public FieldKey Key
		{
			get;
			set;
		}

		public bnet.protocol.attribute.Variant Value
		{
			get;
			set;
		}

		public Field()
		{
		}

		public void Deserialize(Stream stream)
		{
			Field.Deserialize(stream, this);
		}

		public static Field Deserialize(Stream stream, Field instance)
		{
			return Field.Deserialize(stream, instance, (long)-1);
		}

		public static Field Deserialize(Stream stream, Field instance, long limit)
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
							if (instance.Key != null)
							{
								FieldKey.DeserializeLengthDelimited(stream, instance.Key);
							}
							else
							{
								instance.Key = FieldKey.DeserializeLengthDelimited(stream);
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

		public static Field DeserializeLengthDelimited(Stream stream)
		{
			Field field = new Field();
			Field.DeserializeLengthDelimited(stream, field);
			return field;
		}

		public static Field DeserializeLengthDelimited(Stream stream, Field instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return Field.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			Field field = obj as Field;
			if (field == null)
			{
				return false;
			}
			if (!this.Key.Equals(field.Key))
			{
				return false;
			}
			if (!this.Value.Equals(field.Value))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.Key.GetHashCode();
			return hashCode ^ this.Value.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.Key.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			uint serializedSize1 = this.Value.GetSerializedSize();
			num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			return num + 2;
		}

		public static Field ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<Field>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			Field.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, Field instance)
		{
			if (instance.Key == null)
			{
				throw new ArgumentNullException("Key", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.Key.GetSerializedSize());
			FieldKey.Serialize(stream, instance.Key);
			if (instance.Value == null)
			{
				throw new ArgumentNullException("Value", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.Value.GetSerializedSize());
			bnet.protocol.attribute.Variant.Serialize(stream, instance.Value);
		}

		public void SetKey(FieldKey val)
		{
			this.Key = val;
		}

		public void SetValue(bnet.protocol.attribute.Variant val)
		{
			this.Value = val;
		}
	}
}