using System;
using System.IO;

namespace bnet.protocol.connection
{
	public class EchoRequest : IProtoBuf
	{
		public bool HasTime;

		private ulong _Time;

		public bool HasNetworkOnly;

		private bool _NetworkOnly;

		public bool HasPayload;

		private byte[] _Payload;

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public bool NetworkOnly
		{
			get
			{
				return this._NetworkOnly;
			}
			set
			{
				this._NetworkOnly = value;
				this.HasNetworkOnly = true;
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

		public EchoRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			EchoRequest.Deserialize(stream, this);
		}

		public static EchoRequest Deserialize(Stream stream, EchoRequest instance)
		{
			return EchoRequest.Deserialize(stream, instance, (long)-1);
		}

		public static EchoRequest Deserialize(Stream stream, EchoRequest instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			instance.NetworkOnly = false;
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
					else if (num == 9)
					{
						instance.Time = binaryReader.ReadUInt64();
					}
					else if (num == 16)
					{
						instance.NetworkOnly = ProtocolParser.ReadBool(stream);
					}
					else if (num == 26)
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
					if (stream.Position != limit)
					{
						throw new ProtocolBufferException("Read past max limit");
					}
					break;
				}
			}
			return instance;
		}

		public static EchoRequest DeserializeLengthDelimited(Stream stream)
		{
			EchoRequest echoRequest = new EchoRequest();
			EchoRequest.DeserializeLengthDelimited(stream, echoRequest);
			return echoRequest;
		}

		public static EchoRequest DeserializeLengthDelimited(Stream stream, EchoRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return EchoRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			EchoRequest echoRequest = obj as EchoRequest;
			if (echoRequest == null)
			{
				return false;
			}
			if (this.HasTime != echoRequest.HasTime || this.HasTime && !this.Time.Equals(echoRequest.Time))
			{
				return false;
			}
			if (this.HasNetworkOnly != echoRequest.HasNetworkOnly || this.HasNetworkOnly && !this.NetworkOnly.Equals(echoRequest.NetworkOnly))
			{
				return false;
			}
			if (this.HasPayload == echoRequest.HasPayload && (!this.HasPayload || this.Payload.Equals(echoRequest.Payload)))
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
				hashCode ^= this.Time.GetHashCode();
			}
			if (this.HasNetworkOnly)
			{
				hashCode ^= this.NetworkOnly.GetHashCode();
			}
			if (this.HasPayload)
			{
				hashCode ^= this.Payload.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasTime)
			{
				num++;
				num += 8;
			}
			if (this.HasNetworkOnly)
			{
				num++;
				num++;
			}
			if (this.HasPayload)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32((int)this.Payload.Length) + (int)this.Payload.Length;
			}
			return num;
		}

		public static EchoRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<EchoRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			EchoRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, EchoRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasTime)
			{
				stream.WriteByte(9);
				binaryWriter.Write(instance.Time);
			}
			if (instance.HasNetworkOnly)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteBool(stream, instance.NetworkOnly);
			}
			if (instance.HasPayload)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteBytes(stream, instance.Payload);
			}
		}

		public void SetNetworkOnly(bool val)
		{
			this.NetworkOnly = val;
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