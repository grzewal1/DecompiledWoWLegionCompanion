using System;
using System.IO;
using System.Text;

namespace bnet.protocol.config
{
	public class RPCMethodConfig : IProtoBuf
	{
		public bool HasServiceName;

		private string _ServiceName;

		public bool HasMethodName;

		private string _MethodName;

		public bool HasFixedCallCost;

		private uint _FixedCallCost;

		public bool HasFixedPacketSize;

		private uint _FixedPacketSize;

		public bool HasVariableMultiplier;

		private float _VariableMultiplier;

		public bool HasMultiplier;

		private float _Multiplier;

		public bool HasRateLimitCount;

		private uint _RateLimitCount;

		public bool HasRateLimitSeconds;

		private uint _RateLimitSeconds;

		public bool HasMaxPacketSize;

		private uint _MaxPacketSize;

		public bool HasMaxEncodedSize;

		private uint _MaxEncodedSize;

		public bool HasTimeout;

		private float _Timeout;

		public uint FixedCallCost
		{
			get
			{
				return this._FixedCallCost;
			}
			set
			{
				this._FixedCallCost = value;
				this.HasFixedCallCost = true;
			}
		}

		public uint FixedPacketSize
		{
			get
			{
				return this._FixedPacketSize;
			}
			set
			{
				this._FixedPacketSize = value;
				this.HasFixedPacketSize = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public uint MaxEncodedSize
		{
			get
			{
				return this._MaxEncodedSize;
			}
			set
			{
				this._MaxEncodedSize = value;
				this.HasMaxEncodedSize = true;
			}
		}

		public uint MaxPacketSize
		{
			get
			{
				return this._MaxPacketSize;
			}
			set
			{
				this._MaxPacketSize = value;
				this.HasMaxPacketSize = true;
			}
		}

		public string MethodName
		{
			get
			{
				return this._MethodName;
			}
			set
			{
				this._MethodName = value;
				this.HasMethodName = value != null;
			}
		}

		public float Multiplier
		{
			get
			{
				return this._Multiplier;
			}
			set
			{
				this._Multiplier = value;
				this.HasMultiplier = true;
			}
		}

		public uint RateLimitCount
		{
			get
			{
				return this._RateLimitCount;
			}
			set
			{
				this._RateLimitCount = value;
				this.HasRateLimitCount = true;
			}
		}

		public uint RateLimitSeconds
		{
			get
			{
				return this._RateLimitSeconds;
			}
			set
			{
				this._RateLimitSeconds = value;
				this.HasRateLimitSeconds = true;
			}
		}

		public string ServiceName
		{
			get
			{
				return this._ServiceName;
			}
			set
			{
				this._ServiceName = value;
				this.HasServiceName = value != null;
			}
		}

		public float Timeout
		{
			get
			{
				return this._Timeout;
			}
			set
			{
				this._Timeout = value;
				this.HasTimeout = true;
			}
		}

		public float VariableMultiplier
		{
			get
			{
				return this._VariableMultiplier;
			}
			set
			{
				this._VariableMultiplier = value;
				this.HasVariableMultiplier = true;
			}
		}

		public RPCMethodConfig()
		{
		}

		public void Deserialize(Stream stream)
		{
			RPCMethodConfig.Deserialize(stream, this);
		}

		public static RPCMethodConfig Deserialize(Stream stream, RPCMethodConfig instance)
		{
			return RPCMethodConfig.Deserialize(stream, instance, (long)-1);
		}

		public static RPCMethodConfig Deserialize(Stream stream, RPCMethodConfig instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			instance.FixedCallCost = 1;
			instance.FixedPacketSize = 0;
			instance.VariableMultiplier = 0f;
			instance.Multiplier = 1f;
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						switch (num)
						{
							case 53:
							{
								instance.Multiplier = binaryReader.ReadSingle();
								continue;
							}
							case 56:
							{
								instance.RateLimitCount = ProtocolParser.ReadUInt32(stream);
								continue;
							}
							default:
							{
								if (num == 10)
								{
									instance.ServiceName = ProtocolParser.ReadString(stream);
									continue;
								}
								else if (num == 18)
								{
									instance.MethodName = ProtocolParser.ReadString(stream);
									continue;
								}
								else if (num == 24)
								{
									instance.FixedCallCost = ProtocolParser.ReadUInt32(stream);
									continue;
								}
								else if (num == 32)
								{
									instance.FixedPacketSize = ProtocolParser.ReadUInt32(stream);
									continue;
								}
								else if (num == 45)
								{
									instance.VariableMultiplier = binaryReader.ReadSingle();
									continue;
								}
								else if (num == 64)
								{
									instance.RateLimitSeconds = ProtocolParser.ReadUInt32(stream);
									continue;
								}
								else if (num == 72)
								{
									instance.MaxPacketSize = ProtocolParser.ReadUInt32(stream);
									continue;
								}
								else if (num == 80)
								{
									instance.MaxEncodedSize = ProtocolParser.ReadUInt32(stream);
									continue;
								}
								else if (num == 93)
								{
									instance.Timeout = binaryReader.ReadSingle();
									continue;
								}
								else
								{
									Key key = ProtocolParser.ReadKey((byte)num, stream);
									if (key.Field == 0)
									{
										throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
									}
									ProtocolParser.SkipKey(stream, key);
									continue;
								}
							}
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

		public static RPCMethodConfig DeserializeLengthDelimited(Stream stream)
		{
			RPCMethodConfig rPCMethodConfig = new RPCMethodConfig();
			RPCMethodConfig.DeserializeLengthDelimited(stream, rPCMethodConfig);
			return rPCMethodConfig;
		}

		public static RPCMethodConfig DeserializeLengthDelimited(Stream stream, RPCMethodConfig instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return RPCMethodConfig.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			RPCMethodConfig rPCMethodConfig = obj as RPCMethodConfig;
			if (rPCMethodConfig == null)
			{
				return false;
			}
			if (this.HasServiceName != rPCMethodConfig.HasServiceName || this.HasServiceName && !this.ServiceName.Equals(rPCMethodConfig.ServiceName))
			{
				return false;
			}
			if (this.HasMethodName != rPCMethodConfig.HasMethodName || this.HasMethodName && !this.MethodName.Equals(rPCMethodConfig.MethodName))
			{
				return false;
			}
			if (this.HasFixedCallCost != rPCMethodConfig.HasFixedCallCost || this.HasFixedCallCost && !this.FixedCallCost.Equals(rPCMethodConfig.FixedCallCost))
			{
				return false;
			}
			if (this.HasFixedPacketSize != rPCMethodConfig.HasFixedPacketSize || this.HasFixedPacketSize && !this.FixedPacketSize.Equals(rPCMethodConfig.FixedPacketSize))
			{
				return false;
			}
			if (this.HasVariableMultiplier != rPCMethodConfig.HasVariableMultiplier || this.HasVariableMultiplier && !this.VariableMultiplier.Equals(rPCMethodConfig.VariableMultiplier))
			{
				return false;
			}
			if (this.HasMultiplier != rPCMethodConfig.HasMultiplier || this.HasMultiplier && !this.Multiplier.Equals(rPCMethodConfig.Multiplier))
			{
				return false;
			}
			if (this.HasRateLimitCount != rPCMethodConfig.HasRateLimitCount || this.HasRateLimitCount && !this.RateLimitCount.Equals(rPCMethodConfig.RateLimitCount))
			{
				return false;
			}
			if (this.HasRateLimitSeconds != rPCMethodConfig.HasRateLimitSeconds || this.HasRateLimitSeconds && !this.RateLimitSeconds.Equals(rPCMethodConfig.RateLimitSeconds))
			{
				return false;
			}
			if (this.HasMaxPacketSize != rPCMethodConfig.HasMaxPacketSize || this.HasMaxPacketSize && !this.MaxPacketSize.Equals(rPCMethodConfig.MaxPacketSize))
			{
				return false;
			}
			if (this.HasMaxEncodedSize != rPCMethodConfig.HasMaxEncodedSize || this.HasMaxEncodedSize && !this.MaxEncodedSize.Equals(rPCMethodConfig.MaxEncodedSize))
			{
				return false;
			}
			if (this.HasTimeout == rPCMethodConfig.HasTimeout && (!this.HasTimeout || this.Timeout.Equals(rPCMethodConfig.Timeout)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasServiceName)
			{
				hashCode ^= this.ServiceName.GetHashCode();
			}
			if (this.HasMethodName)
			{
				hashCode ^= this.MethodName.GetHashCode();
			}
			if (this.HasFixedCallCost)
			{
				hashCode ^= this.FixedCallCost.GetHashCode();
			}
			if (this.HasFixedPacketSize)
			{
				hashCode ^= this.FixedPacketSize.GetHashCode();
			}
			if (this.HasVariableMultiplier)
			{
				hashCode ^= this.VariableMultiplier.GetHashCode();
			}
			if (this.HasMultiplier)
			{
				hashCode ^= this.Multiplier.GetHashCode();
			}
			if (this.HasRateLimitCount)
			{
				hashCode ^= this.RateLimitCount.GetHashCode();
			}
			if (this.HasRateLimitSeconds)
			{
				hashCode ^= this.RateLimitSeconds.GetHashCode();
			}
			if (this.HasMaxPacketSize)
			{
				hashCode ^= this.MaxPacketSize.GetHashCode();
			}
			if (this.HasMaxEncodedSize)
			{
				hashCode ^= this.MaxEncodedSize.GetHashCode();
			}
			if (this.HasTimeout)
			{
				hashCode ^= this.Timeout.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasServiceName)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.ServiceName);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasMethodName)
			{
				num++;
				uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.MethodName);
				num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			}
			if (this.HasFixedCallCost)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.FixedCallCost);
			}
			if (this.HasFixedPacketSize)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.FixedPacketSize);
			}
			if (this.HasVariableMultiplier)
			{
				num++;
				num += 4;
			}
			if (this.HasMultiplier)
			{
				num++;
				num += 4;
			}
			if (this.HasRateLimitCount)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.RateLimitCount);
			}
			if (this.HasRateLimitSeconds)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.RateLimitSeconds);
			}
			if (this.HasMaxPacketSize)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.MaxPacketSize);
			}
			if (this.HasMaxEncodedSize)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.MaxEncodedSize);
			}
			if (this.HasTimeout)
			{
				num++;
				num += 4;
			}
			return num;
		}

		public static RPCMethodConfig ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<RPCMethodConfig>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			RPCMethodConfig.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, RPCMethodConfig instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasServiceName)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.ServiceName));
			}
			if (instance.HasMethodName)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.MethodName));
			}
			if (instance.HasFixedCallCost)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.FixedCallCost);
			}
			if (instance.HasFixedPacketSize)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteUInt32(stream, instance.FixedPacketSize);
			}
			if (instance.HasVariableMultiplier)
			{
				stream.WriteByte(45);
				binaryWriter.Write(instance.VariableMultiplier);
			}
			if (instance.HasMultiplier)
			{
				stream.WriteByte(53);
				binaryWriter.Write(instance.Multiplier);
			}
			if (instance.HasRateLimitCount)
			{
				stream.WriteByte(56);
				ProtocolParser.WriteUInt32(stream, instance.RateLimitCount);
			}
			if (instance.HasRateLimitSeconds)
			{
				stream.WriteByte(64);
				ProtocolParser.WriteUInt32(stream, instance.RateLimitSeconds);
			}
			if (instance.HasMaxPacketSize)
			{
				stream.WriteByte(72);
				ProtocolParser.WriteUInt32(stream, instance.MaxPacketSize);
			}
			if (instance.HasMaxEncodedSize)
			{
				stream.WriteByte(80);
				ProtocolParser.WriteUInt32(stream, instance.MaxEncodedSize);
			}
			if (instance.HasTimeout)
			{
				stream.WriteByte(93);
				binaryWriter.Write(instance.Timeout);
			}
		}

		public void SetFixedCallCost(uint val)
		{
			this.FixedCallCost = val;
		}

		public void SetFixedPacketSize(uint val)
		{
			this.FixedPacketSize = val;
		}

		public void SetMaxEncodedSize(uint val)
		{
			this.MaxEncodedSize = val;
		}

		public void SetMaxPacketSize(uint val)
		{
			this.MaxPacketSize = val;
		}

		public void SetMethodName(string val)
		{
			this.MethodName = val;
		}

		public void SetMultiplier(float val)
		{
			this.Multiplier = val;
		}

		public void SetRateLimitCount(uint val)
		{
			this.RateLimitCount = val;
		}

		public void SetRateLimitSeconds(uint val)
		{
			this.RateLimitSeconds = val;
		}

		public void SetServiceName(string val)
		{
			this.ServiceName = val;
		}

		public void SetTimeout(float val)
		{
			this.Timeout = val;
		}

		public void SetVariableMultiplier(float val)
		{
			this.VariableMultiplier = val;
		}
	}
}