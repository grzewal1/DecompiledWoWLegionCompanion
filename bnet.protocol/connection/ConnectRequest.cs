using bnet.protocol;
using System;
using System.IO;

namespace bnet.protocol.connection
{
	public class ConnectRequest : IProtoBuf
	{
		public bool HasClientId;

		private ProcessId _ClientId;

		public bool HasBindRequest;

		private bnet.protocol.connection.BindRequest _BindRequest;

		public bnet.protocol.connection.BindRequest BindRequest
		{
			get
			{
				return this._BindRequest;
			}
			set
			{
				this._BindRequest = value;
				this.HasBindRequest = value != null;
			}
		}

		public ProcessId ClientId
		{
			get
			{
				return this._ClientId;
			}
			set
			{
				this._ClientId = value;
				this.HasClientId = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public ConnectRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			ConnectRequest.Deserialize(stream, this);
		}

		public static ConnectRequest Deserialize(Stream stream, ConnectRequest instance)
		{
			return ConnectRequest.Deserialize(stream, instance, (long)-1);
		}

		public static ConnectRequest Deserialize(Stream stream, ConnectRequest instance, long limit)
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
							if (instance.ClientId != null)
							{
								ProcessId.DeserializeLengthDelimited(stream, instance.ClientId);
							}
							else
							{
								instance.ClientId = ProcessId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 != 18)
						{
							Key key = ProtocolParser.ReadKey((byte)num, stream);
							if (key.Field == 0)
							{
								throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
							}
							ProtocolParser.SkipKey(stream, key);
						}
						else if (instance.BindRequest != null)
						{
							bnet.protocol.connection.BindRequest.DeserializeLengthDelimited(stream, instance.BindRequest);
						}
						else
						{
							instance.BindRequest = bnet.protocol.connection.BindRequest.DeserializeLengthDelimited(stream);
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

		public static ConnectRequest DeserializeLengthDelimited(Stream stream)
		{
			ConnectRequest connectRequest = new ConnectRequest();
			ConnectRequest.DeserializeLengthDelimited(stream, connectRequest);
			return connectRequest;
		}

		public static ConnectRequest DeserializeLengthDelimited(Stream stream, ConnectRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ConnectRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ConnectRequest connectRequest = obj as ConnectRequest;
			if (connectRequest == null)
			{
				return false;
			}
			if (this.HasClientId != connectRequest.HasClientId || this.HasClientId && !this.ClientId.Equals(connectRequest.ClientId))
			{
				return false;
			}
			if (this.HasBindRequest == connectRequest.HasBindRequest && (!this.HasBindRequest || this.BindRequest.Equals(connectRequest.BindRequest)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasClientId)
			{
				hashCode ^= this.ClientId.GetHashCode();
			}
			if (this.HasBindRequest)
			{
				hashCode ^= this.BindRequest.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasClientId)
			{
				num++;
				uint serializedSize = this.ClientId.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasBindRequest)
			{
				num++;
				uint serializedSize1 = this.BindRequest.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			return num;
		}

		public static ConnectRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ConnectRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ConnectRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ConnectRequest instance)
		{
			if (instance.HasClientId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.ClientId.GetSerializedSize());
				ProcessId.Serialize(stream, instance.ClientId);
			}
			if (instance.HasBindRequest)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.BindRequest.GetSerializedSize());
				bnet.protocol.connection.BindRequest.Serialize(stream, instance.BindRequest);
			}
		}

		public void SetBindRequest(bnet.protocol.connection.BindRequest val)
		{
			this.BindRequest = val;
		}

		public void SetClientId(ProcessId val)
		{
			this.ClientId = val;
		}
	}
}