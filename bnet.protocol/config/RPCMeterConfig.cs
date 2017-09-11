using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.config
{
	public class RPCMeterConfig : IProtoBuf
	{
		private List<RPCMethodConfig> _Method = new List<RPCMethodConfig>();

		public bool HasIncomePerSecond;

		private uint _IncomePerSecond;

		public bool HasInitialBalance;

		private uint _InitialBalance;

		public bool HasCapBalance;

		private uint _CapBalance;

		public bool HasStartupPeriod;

		private float _StartupPeriod;

		public uint CapBalance
		{
			get
			{
				return this._CapBalance;
			}
			set
			{
				this._CapBalance = value;
				this.HasCapBalance = true;
			}
		}

		public uint IncomePerSecond
		{
			get
			{
				return this._IncomePerSecond;
			}
			set
			{
				this._IncomePerSecond = value;
				this.HasIncomePerSecond = true;
			}
		}

		public uint InitialBalance
		{
			get
			{
				return this._InitialBalance;
			}
			set
			{
				this._InitialBalance = value;
				this.HasInitialBalance = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public List<RPCMethodConfig> Method
		{
			get
			{
				return this._Method;
			}
			set
			{
				this._Method = value;
			}
		}

		public int MethodCount
		{
			get
			{
				return this._Method.Count;
			}
		}

		public List<RPCMethodConfig> MethodList
		{
			get
			{
				return this._Method;
			}
		}

		public float StartupPeriod
		{
			get
			{
				return this._StartupPeriod;
			}
			set
			{
				this._StartupPeriod = value;
				this.HasStartupPeriod = true;
			}
		}

		public RPCMeterConfig()
		{
		}

		public void AddMethod(RPCMethodConfig val)
		{
			this._Method.Add(val);
		}

		public void ClearMethod()
		{
			this._Method.Clear();
		}

		public void Deserialize(Stream stream)
		{
			RPCMeterConfig.Deserialize(stream, this);
		}

		public static RPCMeterConfig Deserialize(Stream stream, RPCMeterConfig instance)
		{
			return RPCMeterConfig.Deserialize(stream, instance, (long)-1);
		}

		public static RPCMeterConfig Deserialize(Stream stream, RPCMeterConfig instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			if (instance.Method == null)
			{
				instance.Method = new List<RPCMethodConfig>();
			}
			instance.IncomePerSecond = 1;
			instance.StartupPeriod = 0f;
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
							instance.Method.Add(RPCMethodConfig.DeserializeLengthDelimited(stream));
						}
						else if (num1 == 16)
						{
							instance.IncomePerSecond = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 24)
						{
							instance.InitialBalance = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 32)
						{
							instance.CapBalance = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 45)
						{
							instance.StartupPeriod = binaryReader.ReadSingle();
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

		public static RPCMeterConfig DeserializeLengthDelimited(Stream stream)
		{
			RPCMeterConfig rPCMeterConfig = new RPCMeterConfig();
			RPCMeterConfig.DeserializeLengthDelimited(stream, rPCMeterConfig);
			return rPCMeterConfig;
		}

		public static RPCMeterConfig DeserializeLengthDelimited(Stream stream, RPCMeterConfig instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return RPCMeterConfig.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			RPCMeterConfig rPCMeterConfig = obj as RPCMeterConfig;
			if (rPCMeterConfig == null)
			{
				return false;
			}
			if (this.Method.Count != rPCMeterConfig.Method.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Method.Count; i++)
			{
				if (!this.Method[i].Equals(rPCMeterConfig.Method[i]))
				{
					return false;
				}
			}
			if (this.HasIncomePerSecond != rPCMeterConfig.HasIncomePerSecond || this.HasIncomePerSecond && !this.IncomePerSecond.Equals(rPCMeterConfig.IncomePerSecond))
			{
				return false;
			}
			if (this.HasInitialBalance != rPCMeterConfig.HasInitialBalance || this.HasInitialBalance && !this.InitialBalance.Equals(rPCMeterConfig.InitialBalance))
			{
				return false;
			}
			if (this.HasCapBalance != rPCMeterConfig.HasCapBalance || this.HasCapBalance && !this.CapBalance.Equals(rPCMeterConfig.CapBalance))
			{
				return false;
			}
			if (this.HasStartupPeriod == rPCMeterConfig.HasStartupPeriod && (!this.HasStartupPeriod || this.StartupPeriod.Equals(rPCMeterConfig.StartupPeriod)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (RPCMethodConfig method in this.Method)
			{
				hashCode ^= method.GetHashCode();
			}
			if (this.HasIncomePerSecond)
			{
				hashCode ^= this.IncomePerSecond.GetHashCode();
			}
			if (this.HasInitialBalance)
			{
				hashCode ^= this.InitialBalance.GetHashCode();
			}
			if (this.HasCapBalance)
			{
				hashCode ^= this.CapBalance.GetHashCode();
			}
			if (this.HasStartupPeriod)
			{
				hashCode ^= this.StartupPeriod.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Method.Count > 0)
			{
				foreach (RPCMethodConfig method in this.Method)
				{
					num++;
					uint serializedSize = method.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			if (this.HasIncomePerSecond)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.IncomePerSecond);
			}
			if (this.HasInitialBalance)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.InitialBalance);
			}
			if (this.HasCapBalance)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.CapBalance);
			}
			if (this.HasStartupPeriod)
			{
				num++;
				num += 4;
			}
			return num;
		}

		public static RPCMeterConfig ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<RPCMeterConfig>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			RPCMeterConfig.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, RPCMeterConfig instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.Method.Count > 0)
			{
				foreach (RPCMethodConfig method in instance.Method)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, method.GetSerializedSize());
					RPCMethodConfig.Serialize(stream, method);
				}
			}
			if (instance.HasIncomePerSecond)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.IncomePerSecond);
			}
			if (instance.HasInitialBalance)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.InitialBalance);
			}
			if (instance.HasCapBalance)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteUInt32(stream, instance.CapBalance);
			}
			if (instance.HasStartupPeriod)
			{
				stream.WriteByte(45);
				binaryWriter.Write(instance.StartupPeriod);
			}
		}

		public void SetCapBalance(uint val)
		{
			this.CapBalance = val;
		}

		public void SetIncomePerSecond(uint val)
		{
			this.IncomePerSecond = val;
		}

		public void SetInitialBalance(uint val)
		{
			this.InitialBalance = val;
		}

		public void SetMethod(List<RPCMethodConfig> val)
		{
			this.Method = val;
		}

		public void SetStartupPeriod(float val)
		{
			this.StartupPeriod = val;
		}
	}
}