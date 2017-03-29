using System;
using System.IO;

namespace bnet.protocol.account
{
	public class CAIS : IProtoBuf
	{
		public bool HasPlayedMinutes;

		private uint _PlayedMinutes;

		public bool HasRestedMinutes;

		private uint _RestedMinutes;

		public bool HasLastHeardTime;

		private ulong _LastHeardTime;

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public ulong LastHeardTime
		{
			get
			{
				return this._LastHeardTime;
			}
			set
			{
				this._LastHeardTime = value;
				this.HasLastHeardTime = true;
			}
		}

		public uint PlayedMinutes
		{
			get
			{
				return this._PlayedMinutes;
			}
			set
			{
				this._PlayedMinutes = value;
				this.HasPlayedMinutes = true;
			}
		}

		public uint RestedMinutes
		{
			get
			{
				return this._RestedMinutes;
			}
			set
			{
				this._RestedMinutes = value;
				this.HasRestedMinutes = true;
			}
		}

		public CAIS()
		{
		}

		public void Deserialize(Stream stream)
		{
			CAIS.Deserialize(stream, this);
		}

		public static CAIS Deserialize(Stream stream, CAIS instance)
		{
			return CAIS.Deserialize(stream, instance, (long)-1);
		}

		public static CAIS Deserialize(Stream stream, CAIS instance, long limit)
		{
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
							instance.PlayedMinutes = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 16)
						{
							instance.RestedMinutes = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 24)
						{
							instance.LastHeardTime = ProtocolParser.ReadUInt64(stream);
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

		public static CAIS DeserializeLengthDelimited(Stream stream)
		{
			CAIS cAI = new CAIS();
			CAIS.DeserializeLengthDelimited(stream, cAI);
			return cAI;
		}

		public static CAIS DeserializeLengthDelimited(Stream stream, CAIS instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return CAIS.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			CAIS cAI = obj as CAIS;
			if (cAI == null)
			{
				return false;
			}
			if (this.HasPlayedMinutes != cAI.HasPlayedMinutes || this.HasPlayedMinutes && !this.PlayedMinutes.Equals(cAI.PlayedMinutes))
			{
				return false;
			}
			if (this.HasRestedMinutes != cAI.HasRestedMinutes || this.HasRestedMinutes && !this.RestedMinutes.Equals(cAI.RestedMinutes))
			{
				return false;
			}
			if (this.HasLastHeardTime == cAI.HasLastHeardTime && (!this.HasLastHeardTime || this.LastHeardTime.Equals(cAI.LastHeardTime)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasPlayedMinutes)
			{
				hashCode = hashCode ^ this.PlayedMinutes.GetHashCode();
			}
			if (this.HasRestedMinutes)
			{
				hashCode = hashCode ^ this.RestedMinutes.GetHashCode();
			}
			if (this.HasLastHeardTime)
			{
				hashCode = hashCode ^ this.LastHeardTime.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasPlayedMinutes)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.PlayedMinutes);
			}
			if (this.HasRestedMinutes)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.RestedMinutes);
			}
			if (this.HasLastHeardTime)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt64(this.LastHeardTime);
			}
			return num;
		}

		public static CAIS ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<CAIS>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			CAIS.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, CAIS instance)
		{
			if (instance.HasPlayedMinutes)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.PlayedMinutes);
			}
			if (instance.HasRestedMinutes)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.RestedMinutes);
			}
			if (instance.HasLastHeardTime)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt64(stream, instance.LastHeardTime);
			}
		}

		public void SetLastHeardTime(ulong val)
		{
			this.LastHeardTime = val;
		}

		public void SetPlayedMinutes(uint val)
		{
			this.PlayedMinutes = val;
		}

		public void SetRestedMinutes(uint val)
		{
			this.RestedMinutes = val;
		}
	}
}