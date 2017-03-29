using System;
using System.IO;
using System.Text;

namespace bnet.protocol.account
{
	public class IsIgrAddressRequest : IProtoBuf
	{
		public bool HasClientAddress;

		private string _ClientAddress;

		public bool HasRegion;

		private uint _Region;

		public string ClientAddress
		{
			get
			{
				return this._ClientAddress;
			}
			set
			{
				this._ClientAddress = value;
				this.HasClientAddress = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public uint Region
		{
			get
			{
				return this._Region;
			}
			set
			{
				this._Region = value;
				this.HasRegion = true;
			}
		}

		public IsIgrAddressRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			IsIgrAddressRequest.Deserialize(stream, this);
		}

		public static IsIgrAddressRequest Deserialize(Stream stream, IsIgrAddressRequest instance)
		{
			return IsIgrAddressRequest.Deserialize(stream, instance, (long)-1);
		}

		public static IsIgrAddressRequest Deserialize(Stream stream, IsIgrAddressRequest instance, long limit)
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
							instance.ClientAddress = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 16)
						{
							instance.Region = ProtocolParser.ReadUInt32(stream);
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

		public static IsIgrAddressRequest DeserializeLengthDelimited(Stream stream)
		{
			IsIgrAddressRequest isIgrAddressRequest = new IsIgrAddressRequest();
			IsIgrAddressRequest.DeserializeLengthDelimited(stream, isIgrAddressRequest);
			return isIgrAddressRequest;
		}

		public static IsIgrAddressRequest DeserializeLengthDelimited(Stream stream, IsIgrAddressRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return IsIgrAddressRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			IsIgrAddressRequest isIgrAddressRequest = obj as IsIgrAddressRequest;
			if (isIgrAddressRequest == null)
			{
				return false;
			}
			if (this.HasClientAddress != isIgrAddressRequest.HasClientAddress || this.HasClientAddress && !this.ClientAddress.Equals(isIgrAddressRequest.ClientAddress))
			{
				return false;
			}
			if (this.HasRegion == isIgrAddressRequest.HasRegion && (!this.HasRegion || this.Region.Equals(isIgrAddressRequest.Region)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasClientAddress)
			{
				hashCode = hashCode ^ this.ClientAddress.GetHashCode();
			}
			if (this.HasRegion)
			{
				hashCode = hashCode ^ this.Region.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasClientAddress)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.ClientAddress);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasRegion)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.Region);
			}
			return num;
		}

		public static IsIgrAddressRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<IsIgrAddressRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			IsIgrAddressRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, IsIgrAddressRequest instance)
		{
			if (instance.HasClientAddress)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.ClientAddress));
			}
			if (instance.HasRegion)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.Region);
			}
		}

		public void SetClientAddress(string val)
		{
			this.ClientAddress = val;
		}

		public void SetRegion(uint val)
		{
			this.Region = val;
		}
	}
}