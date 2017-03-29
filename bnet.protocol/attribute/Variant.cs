using bnet.protocol;
using System;
using System.IO;
using System.Text;

namespace bnet.protocol.attribute
{
	public class Variant : IProtoBuf
	{
		public bool HasBoolValue;

		private bool _BoolValue;

		public bool HasIntValue;

		private long _IntValue;

		public bool HasFloatValue;

		private double _FloatValue;

		public bool HasStringValue;

		private string _StringValue;

		public bool HasBlobValue;

		private byte[] _BlobValue;

		public bool HasMessageValue;

		private byte[] _MessageValue;

		public bool HasFourccValue;

		private string _FourccValue;

		public bool HasUintValue;

		private ulong _UintValue;

		public bool HasEntityidValue;

		private EntityId _EntityidValue;

		public byte[] BlobValue
		{
			get
			{
				return this._BlobValue;
			}
			set
			{
				this._BlobValue = value;
				this.HasBlobValue = value != null;
			}
		}

		public bool BoolValue
		{
			get
			{
				return this._BoolValue;
			}
			set
			{
				this._BoolValue = value;
				this.HasBoolValue = true;
			}
		}

		public EntityId EntityidValue
		{
			get
			{
				return this._EntityidValue;
			}
			set
			{
				this._EntityidValue = value;
				this.HasEntityidValue = value != null;
			}
		}

		public double FloatValue
		{
			get
			{
				return this._FloatValue;
			}
			set
			{
				this._FloatValue = value;
				this.HasFloatValue = true;
			}
		}

		public string FourccValue
		{
			get
			{
				return this._FourccValue;
			}
			set
			{
				this._FourccValue = value;
				this.HasFourccValue = value != null;
			}
		}

		public long IntValue
		{
			get
			{
				return this._IntValue;
			}
			set
			{
				this._IntValue = value;
				this.HasIntValue = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public byte[] MessageValue
		{
			get
			{
				return this._MessageValue;
			}
			set
			{
				this._MessageValue = value;
				this.HasMessageValue = value != null;
			}
		}

		public string StringValue
		{
			get
			{
				return this._StringValue;
			}
			set
			{
				this._StringValue = value;
				this.HasStringValue = value != null;
			}
		}

		public ulong UintValue
		{
			get
			{
				return this._UintValue;
			}
			set
			{
				this._UintValue = value;
				this.HasUintValue = true;
			}
		}

		public Variant()
		{
		}

		public void Deserialize(Stream stream)
		{
			bnet.protocol.attribute.Variant.Deserialize(stream, this);
		}

		public static bnet.protocol.attribute.Variant Deserialize(Stream stream, bnet.protocol.attribute.Variant instance)
		{
			return bnet.protocol.attribute.Variant.Deserialize(stream, instance, (long)-1);
		}

		public static bnet.protocol.attribute.Variant Deserialize(Stream stream, bnet.protocol.attribute.Variant instance, long limit)
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
						if (num1 == 16)
						{
							instance.BoolValue = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 24)
						{
							instance.IntValue = (long)ProtocolParser.ReadUInt64(stream);
						}
						else if (num1 == 33)
						{
							instance.FloatValue = binaryReader.ReadDouble();
						}
						else if (num1 == 42)
						{
							instance.StringValue = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 50)
						{
							instance.BlobValue = ProtocolParser.ReadBytes(stream);
						}
						else if (num1 == 58)
						{
							instance.MessageValue = ProtocolParser.ReadBytes(stream);
						}
						else if (num1 == 66)
						{
							instance.FourccValue = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 72)
						{
							instance.UintValue = ProtocolParser.ReadUInt64(stream);
						}
						else if (num1 != 82)
						{
							Key key = ProtocolParser.ReadKey((byte)num, stream);
							if (key.Field == 0)
							{
								throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
							}
							ProtocolParser.SkipKey(stream, key);
						}
						else if (instance.EntityidValue != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.EntityidValue);
						}
						else
						{
							instance.EntityidValue = EntityId.DeserializeLengthDelimited(stream);
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

		public static bnet.protocol.attribute.Variant DeserializeLengthDelimited(Stream stream)
		{
			bnet.protocol.attribute.Variant variant = new bnet.protocol.attribute.Variant();
			bnet.protocol.attribute.Variant.DeserializeLengthDelimited(stream, variant);
			return variant;
		}

		public static bnet.protocol.attribute.Variant DeserializeLengthDelimited(Stream stream, bnet.protocol.attribute.Variant instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return bnet.protocol.attribute.Variant.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			bnet.protocol.attribute.Variant variant = obj as bnet.protocol.attribute.Variant;
			if (variant == null)
			{
				return false;
			}
			if (this.HasBoolValue != variant.HasBoolValue || this.HasBoolValue && !this.BoolValue.Equals(variant.BoolValue))
			{
				return false;
			}
			if (this.HasIntValue != variant.HasIntValue || this.HasIntValue && !this.IntValue.Equals(variant.IntValue))
			{
				return false;
			}
			if (this.HasFloatValue != variant.HasFloatValue || this.HasFloatValue && !this.FloatValue.Equals(variant.FloatValue))
			{
				return false;
			}
			if (this.HasStringValue != variant.HasStringValue || this.HasStringValue && !this.StringValue.Equals(variant.StringValue))
			{
				return false;
			}
			if (this.HasBlobValue != variant.HasBlobValue || this.HasBlobValue && !this.BlobValue.Equals(variant.BlobValue))
			{
				return false;
			}
			if (this.HasMessageValue != variant.HasMessageValue || this.HasMessageValue && !this.MessageValue.Equals(variant.MessageValue))
			{
				return false;
			}
			if (this.HasFourccValue != variant.HasFourccValue || this.HasFourccValue && !this.FourccValue.Equals(variant.FourccValue))
			{
				return false;
			}
			if (this.HasUintValue != variant.HasUintValue || this.HasUintValue && !this.UintValue.Equals(variant.UintValue))
			{
				return false;
			}
			if (this.HasEntityidValue == variant.HasEntityidValue && (!this.HasEntityidValue || this.EntityidValue.Equals(variant.EntityidValue)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasBoolValue)
			{
				hashCode = hashCode ^ this.BoolValue.GetHashCode();
			}
			if (this.HasIntValue)
			{
				hashCode = hashCode ^ this.IntValue.GetHashCode();
			}
			if (this.HasFloatValue)
			{
				hashCode = hashCode ^ this.FloatValue.GetHashCode();
			}
			if (this.HasStringValue)
			{
				hashCode = hashCode ^ this.StringValue.GetHashCode();
			}
			if (this.HasBlobValue)
			{
				hashCode = hashCode ^ this.BlobValue.GetHashCode();
			}
			if (this.HasMessageValue)
			{
				hashCode = hashCode ^ this.MessageValue.GetHashCode();
			}
			if (this.HasFourccValue)
			{
				hashCode = hashCode ^ this.FourccValue.GetHashCode();
			}
			if (this.HasUintValue)
			{
				hashCode = hashCode ^ this.UintValue.GetHashCode();
			}
			if (this.HasEntityidValue)
			{
				hashCode = hashCode ^ this.EntityidValue.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasBoolValue)
			{
				num++;
				num++;
			}
			if (this.HasIntValue)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt64((ulong)this.IntValue);
			}
			if (this.HasFloatValue)
			{
				num++;
				num = num + 8;
			}
			if (this.HasStringValue)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.StringValue);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasBlobValue)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32((int)this.BlobValue.Length) + (int)this.BlobValue.Length;
			}
			if (this.HasMessageValue)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32((int)this.MessageValue.Length) + (int)this.MessageValue.Length;
			}
			if (this.HasFourccValue)
			{
				num++;
				uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.FourccValue);
				num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			}
			if (this.HasUintValue)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt64(this.UintValue);
			}
			if (this.HasEntityidValue)
			{
				num++;
				uint serializedSize = this.EntityidValue.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			return num;
		}

		public static bnet.protocol.attribute.Variant ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<bnet.protocol.attribute.Variant>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			bnet.protocol.attribute.Variant.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, bnet.protocol.attribute.Variant instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasBoolValue)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteBool(stream, instance.BoolValue);
			}
			if (instance.HasIntValue)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.IntValue);
			}
			if (instance.HasFloatValue)
			{
				stream.WriteByte(33);
				binaryWriter.Write(instance.FloatValue);
			}
			if (instance.HasStringValue)
			{
				stream.WriteByte(42);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.StringValue));
			}
			if (instance.HasBlobValue)
			{
				stream.WriteByte(50);
				ProtocolParser.WriteBytes(stream, instance.BlobValue);
			}
			if (instance.HasMessageValue)
			{
				stream.WriteByte(58);
				ProtocolParser.WriteBytes(stream, instance.MessageValue);
			}
			if (instance.HasFourccValue)
			{
				stream.WriteByte(66);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.FourccValue));
			}
			if (instance.HasUintValue)
			{
				stream.WriteByte(72);
				ProtocolParser.WriteUInt64(stream, instance.UintValue);
			}
			if (instance.HasEntityidValue)
			{
				stream.WriteByte(82);
				ProtocolParser.WriteUInt32(stream, instance.EntityidValue.GetSerializedSize());
				EntityId.Serialize(stream, instance.EntityidValue);
			}
		}

		public void SetBlobValue(byte[] val)
		{
			this.BlobValue = val;
		}

		public void SetBoolValue(bool val)
		{
			this.BoolValue = val;
		}

		public void SetEntityidValue(EntityId val)
		{
			this.EntityidValue = val;
		}

		public void SetFloatValue(double val)
		{
			this.FloatValue = val;
		}

		public void SetFourccValue(string val)
		{
			this.FourccValue = val;
		}

		public void SetIntValue(long val)
		{
			this.IntValue = val;
		}

		public void SetMessageValue(byte[] val)
		{
			this.MessageValue = val;
		}

		public void SetStringValue(string val)
		{
			this.StringValue = val;
		}

		public void SetUintValue(ulong val)
		{
			this.UintValue = val;
		}
	}
}