using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol
{
	public class EntityId : IProtoBuf
	{
		public ulong High
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

		public ulong Low
		{
			get;
			set;
		}

		public EntityId()
		{
		}

		public void Deserialize(Stream stream)
		{
			EntityId.Deserialize(stream, this);
		}

		public static EntityId Deserialize(Stream stream, EntityId instance)
		{
			return EntityId.Deserialize(stream, instance, (long)-1);
		}

		public static EntityId Deserialize(Stream stream, EntityId instance, long limit)
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
						if (num1 == 9)
						{
							instance.High = binaryReader.ReadUInt64();
						}
						else if (num1 == 17)
						{
							instance.Low = binaryReader.ReadUInt64();
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

		public static EntityId DeserializeLengthDelimited(Stream stream)
		{
			EntityId entityId = new EntityId();
			EntityId.DeserializeLengthDelimited(stream, entityId);
			return entityId;
		}

		public static EntityId DeserializeLengthDelimited(Stream stream, EntityId instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return EntityId.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			EntityId entityId = obj as EntityId;
			if (entityId == null)
			{
				return false;
			}
			if (!this.High.Equals(entityId.High))
			{
				return false;
			}
			if (!this.Low.Equals(entityId.Low))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.High.GetHashCode();
			return hashCode ^ this.Low.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			return 0 + 8 + 8 + 2;
		}

		public static EntityId ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<EntityId>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			EntityId.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, EntityId instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			stream.WriteByte(9);
			binaryWriter.Write(instance.High);
			stream.WriteByte(17);
			binaryWriter.Write(instance.Low);
		}

		public void SetHigh(ulong val)
		{
			this.High = val;
		}

		public void SetLow(ulong val)
		{
			this.Low = val;
		}
	}
}