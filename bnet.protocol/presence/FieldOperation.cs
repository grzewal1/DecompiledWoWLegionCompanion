using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.presence
{
	public class FieldOperation : IProtoBuf
	{
		public bool HasOperation;

		private FieldOperation.Types.OperationType _Operation;

		public bnet.protocol.presence.Field Field
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

		public FieldOperation.Types.OperationType Operation
		{
			get
			{
				return this._Operation;
			}
			set
			{
				this._Operation = value;
				this.HasOperation = true;
			}
		}

		public FieldOperation()
		{
		}

		public void Deserialize(Stream stream)
		{
			FieldOperation.Deserialize(stream, this);
		}

		public static FieldOperation Deserialize(Stream stream, FieldOperation instance)
		{
			return FieldOperation.Deserialize(stream, instance, (long)-1);
		}

		public static FieldOperation Deserialize(Stream stream, FieldOperation instance, long limit)
		{
			instance.Operation = FieldOperation.Types.OperationType.SET;
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
					else if (num != 10)
					{
						if (num == 16)
						{
							instance.Operation = (FieldOperation.Types.OperationType)((int)ProtocolParser.ReadUInt64(stream));
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
					else if (instance.Field != null)
					{
						bnet.protocol.presence.Field.DeserializeLengthDelimited(stream, instance.Field);
					}
					else
					{
						instance.Field = bnet.protocol.presence.Field.DeserializeLengthDelimited(stream);
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

		public static FieldOperation DeserializeLengthDelimited(Stream stream)
		{
			FieldOperation fieldOperation = new FieldOperation();
			FieldOperation.DeserializeLengthDelimited(stream, fieldOperation);
			return fieldOperation;
		}

		public static FieldOperation DeserializeLengthDelimited(Stream stream, FieldOperation instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return FieldOperation.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			FieldOperation fieldOperation = obj as FieldOperation;
			if (fieldOperation == null)
			{
				return false;
			}
			if (!this.Field.Equals(fieldOperation.Field))
			{
				return false;
			}
			if (this.HasOperation == fieldOperation.HasOperation && (!this.HasOperation || this.Operation.Equals(fieldOperation.Operation)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Field.GetHashCode();
			if (this.HasOperation)
			{
				hashCode ^= this.Operation.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.Field.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasOperation)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64((ulong)this.Operation);
			}
			num++;
			return num;
		}

		public static FieldOperation ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<FieldOperation>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			FieldOperation.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, FieldOperation instance)
		{
			if (instance.Field == null)
			{
				throw new ArgumentNullException("Field", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.Field.GetSerializedSize());
			bnet.protocol.presence.Field.Serialize(stream, instance.Field);
			if (instance.HasOperation)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.Operation);
			}
		}

		public void SetField(bnet.protocol.presence.Field val)
		{
			this.Field = val;
		}

		public void SetOperation(FieldOperation.Types.OperationType val)
		{
			this.Operation = val;
		}

		public static class Types
		{
			public enum OperationType
			{
				SET,
				CLEAR
			}
		}
	}
}