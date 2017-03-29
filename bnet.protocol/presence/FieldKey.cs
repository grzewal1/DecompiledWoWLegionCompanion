using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.presence
{
	public class FieldKey : IProtoBuf
	{
		public bool HasIndex;

		private ulong _Index;

		public uint Field
		{
			get;
			set;
		}

		public uint Group
		{
			get;
			set;
		}

		public ulong Index
		{
			get
			{
				return this._Index;
			}
			set
			{
				this._Index = value;
				this.HasIndex = true;
			}
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

		public FieldKey()
		{
		}

		public void Deserialize(Stream stream)
		{
			FieldKey.Deserialize(stream, this);
		}

		public static FieldKey Deserialize(Stream stream, FieldKey instance)
		{
			return FieldKey.Deserialize(stream, instance, (long)-1);
		}

		public static FieldKey Deserialize(Stream stream, FieldKey instance, long limit)
		{
			instance.Index = (ulong)0;
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 == 8)
						{
							instance.Program = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 16)
						{
							instance.Group = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 24)
						{
							instance.Field = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 32)
						{
							instance.Index = ProtocolParser.ReadUInt64(stream);
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

		public static FieldKey DeserializeLengthDelimited(Stream stream)
		{
			FieldKey fieldKey = new FieldKey();
			FieldKey.DeserializeLengthDelimited(stream, fieldKey);
			return fieldKey;
		}

		public static FieldKey DeserializeLengthDelimited(Stream stream, FieldKey instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return FieldKey.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			FieldKey fieldKey = obj as FieldKey;
			if (fieldKey == null)
			{
				return false;
			}
			if (!this.Program.Equals(fieldKey.Program))
			{
				return false;
			}
			if (!this.Group.Equals(fieldKey.Group))
			{
				return false;
			}
			if (!this.Field.Equals(fieldKey.Field))
			{
				return false;
			}
			if (this.HasIndex == fieldKey.HasIndex && (!this.HasIndex || this.Index.Equals(fieldKey.Index)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.Program.GetHashCode();
			hashCode = hashCode ^ this.Group.GetHashCode();
			hashCode = hashCode ^ this.Field.GetHashCode();
			if (this.HasIndex)
			{
				hashCode = hashCode ^ this.Index.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num = num + ProtocolParser.SizeOfUInt32(this.Program);
			num = num + ProtocolParser.SizeOfUInt32(this.Group);
			num = num + ProtocolParser.SizeOfUInt32(this.Field);
			if (this.HasIndex)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt64(this.Index);
			}
			num = num + 3;
			return num;
		}

		public static FieldKey ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<FieldKey>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			FieldKey.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, FieldKey instance)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.Program);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt32(stream, instance.Group);
			stream.WriteByte(24);
			ProtocolParser.WriteUInt32(stream, instance.Field);
			if (instance.HasIndex)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteUInt64(stream, instance.Index);
			}
		}

		public void SetField(uint val)
		{
			this.Field = val;
		}

		public void SetGroup(uint val)
		{
			this.Group = val;
		}

		public void SetIndex(ulong val)
		{
			this.Index = val;
		}

		public void SetProgram(uint val)
		{
			this.Program = val;
		}
	}
}