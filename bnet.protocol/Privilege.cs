using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol
{
	public class Privilege : IProtoBuf
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

		public uint Value
		{
			get;
			set;
		}

		public Privilege()
		{
		}

		public void Deserialize(Stream stream)
		{
			Privilege.Deserialize(stream, this);
		}

		public static Privilege Deserialize(Stream stream, Privilege instance)
		{
			return Privilege.Deserialize(stream, instance, (long)-1);
		}

		public static Privilege Deserialize(Stream stream, Privilege instance, long limit)
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
						else if (num1 == 16)
						{
							instance.Value = ProtocolParser.ReadUInt32(stream);
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

		public static Privilege DeserializeLengthDelimited(Stream stream)
		{
			Privilege privilege = new Privilege();
			Privilege.DeserializeLengthDelimited(stream, privilege);
			return privilege;
		}

		public static Privilege DeserializeLengthDelimited(Stream stream, Privilege instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return Privilege.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			Privilege privilege = obj as Privilege;
			if (privilege == null)
			{
				return false;
			}
			if (!this.Name.Equals(privilege.Name))
			{
				return false;
			}
			if (!this.Value.Equals(privilege.Value))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Name.GetHashCode();
			return hashCode ^ this.Value.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Name);
			num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			num += ProtocolParser.SizeOfUInt32(this.Value);
			return num + 2;
		}

		public static Privilege ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<Privilege>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			Privilege.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, Privilege instance)
		{
			if (instance.Name == null)
			{
				throw new ArgumentNullException("Name", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Name));
			stream.WriteByte(16);
			ProtocolParser.WriteUInt32(stream, instance.Value);
		}

		public void SetName(string val)
		{
			this.Name = val;
		}

		public void SetValue(uint val)
		{
			this.Value = val;
		}
	}
}