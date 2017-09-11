using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol
{
	public class Address : IProtoBuf
	{
		public bool HasPort;

		private uint _Port;

		public string Address_
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

		public uint Port
		{
			get
			{
				return this._Port;
			}
			set
			{
				this._Port = value;
				this.HasPort = true;
			}
		}

		public Address()
		{
		}

		public void Deserialize(Stream stream)
		{
			Address.Deserialize(stream, this);
		}

		public static Address Deserialize(Stream stream, Address instance)
		{
			return Address.Deserialize(stream, instance, (long)-1);
		}

		public static Address Deserialize(Stream stream, Address instance, long limit)
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
							instance.Address_ = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 16)
						{
							instance.Port = ProtocolParser.ReadUInt32(stream);
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

		public static Address DeserializeLengthDelimited(Stream stream)
		{
			Address address = new Address();
			Address.DeserializeLengthDelimited(stream, address);
			return address;
		}

		public static Address DeserializeLengthDelimited(Stream stream, Address instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return Address.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			Address address = obj as Address;
			if (address == null)
			{
				return false;
			}
			if (!this.Address_.Equals(address.Address_))
			{
				return false;
			}
			if (this.HasPort == address.HasPort && (!this.HasPort || this.Port.Equals(address.Port)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Address_.GetHashCode();
			if (this.HasPort)
			{
				hashCode ^= this.Port.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Address_);
			num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			if (this.HasPort)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.Port);
			}
			num++;
			return num;
		}

		public static Address ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<Address>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			Address.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, Address instance)
		{
			if (instance.Address_ == null)
			{
				throw new ArgumentNullException("Address_", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Address_));
			if (instance.HasPort)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.Port);
			}
		}

		public void SetAddress_(string val)
		{
			this.Address_ = val;
		}

		public void SetPort(uint val)
		{
			this.Port = val;
		}
	}
}