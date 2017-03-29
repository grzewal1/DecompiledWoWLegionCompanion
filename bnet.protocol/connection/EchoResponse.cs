using System;
using System.IO;

namespace bnet.protocol.connection
{
	public class EchoResponse : IProtoBuf
	{
		public bool HasTime;

		private ulong _Time;

		public bool HasPayload;

		private byte[] _Payload;

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public byte[] Payload
		{
			get
			{
				return this._Payload;
			}
			set
			{
				this._Payload = value;
				this.HasPayload = value != null;
			}
		}

		public ulong Time
		{
			get
			{
				return this._Time;
			}
			set
			{
				this._Time = value;
				this.HasTime = true;
			}
		}

		public EchoResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			EchoResponse.Deserialize(stream, this);
		}

		public static EchoResponse Deserialize(Stream stream, EchoResponse instance)
		{
			return EchoResponse.Deserialize(stream, instance, (long)-1);
		}

		public static EchoResponse Deserialize(Stream stream, EchoResponse instance, long limit)
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
							instance.Time = binaryReader.ReadUInt64();
						}
						else if (num1 == 18)
						{
							instance.Payload = ProtocolParser.ReadBytes(stream);
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

		public static EchoResponse DeserializeLengthDelimited(Stream stream)
		{
			EchoResponse echoResponse = new EchoResponse();
			EchoResponse.DeserializeLengthDelimited(stream, echoResponse);
			return echoResponse;
		}

		public static EchoResponse DeserializeLengthDelimited(Stream stream, EchoResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return EchoResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			EchoResponse echoResponse = obj as EchoResponse;
			if (echoResponse == null)
			{
				return false;
			}
			if (this.HasTime != echoResponse.HasTime || this.HasTime && !this.Time.Equals(echoResponse.Time))
			{
				return false;
			}
			if (this.HasPayload == echoResponse.HasPayload && (!this.HasPayload || this.Payload.Equals(echoResponse.Payload)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasTime)
			{
				hashCode = hashCode ^ this.Time.GetHashCode();
			}
			if (this.HasPayload)
			{
				hashCode = hashCode ^ this.Payload.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasTime)
			{
				num++;
				num = num + 8;
			}
			if (this.HasPayload)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32((int)this.Payload.Length) + (int)this.Payload.Length;
			}
			return num;
		}

		public static EchoResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<EchoResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			EchoResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, EchoResponse instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasTime)
			{
				stream.WriteByte(9);
				binaryWriter.Write(instance.Time);
			}
			if (instance.HasPayload)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteBytes(stream, instance.Payload);
			}
		}

		public void SetPayload(byte[] val)
		{
			this.Payload = val;
		}

		public void SetTime(ulong val)
		{
			this.Time = val;
		}
	}
}