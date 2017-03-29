using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.connection
{
	public class ConnectResponse : IProtoBuf
	{
		public bool HasClientId;

		private ProcessId _ClientId;

		public bool HasBindResult;

		private uint _BindResult;

		public bool HasBindResponse;

		private bnet.protocol.connection.BindResponse _BindResponse;

		public bool HasContentHandleArray;

		private ConnectionMeteringContentHandles _ContentHandleArray;

		public bool HasServerTime;

		private ulong _ServerTime;

		public bnet.protocol.connection.BindResponse BindResponse
		{
			get
			{
				return this._BindResponse;
			}
			set
			{
				this._BindResponse = value;
				this.HasBindResponse = value != null;
			}
		}

		public uint BindResult
		{
			get
			{
				return this._BindResult;
			}
			set
			{
				this._BindResult = value;
				this.HasBindResult = true;
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

		public ConnectionMeteringContentHandles ContentHandleArray
		{
			get
			{
				return this._ContentHandleArray;
			}
			set
			{
				this._ContentHandleArray = value;
				this.HasContentHandleArray = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public ProcessId ServerId
		{
			get;
			set;
		}

		public ulong ServerTime
		{
			get
			{
				return this._ServerTime;
			}
			set
			{
				this._ServerTime = value;
				this.HasServerTime = true;
			}
		}

		public ConnectResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			ConnectResponse.Deserialize(stream, this);
		}

		public static ConnectResponse Deserialize(Stream stream, ConnectResponse instance)
		{
			return ConnectResponse.Deserialize(stream, instance, (long)-1);
		}

		public static ConnectResponse Deserialize(Stream stream, ConnectResponse instance, long limit)
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
							if (instance.ServerId != null)
							{
								ProcessId.DeserializeLengthDelimited(stream, instance.ServerId);
							}
							else
							{
								instance.ServerId = ProcessId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 18)
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
						else if (num1 == 24)
						{
							instance.BindResult = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 34)
						{
							if (instance.BindResponse != null)
							{
								bnet.protocol.connection.BindResponse.DeserializeLengthDelimited(stream, instance.BindResponse);
							}
							else
							{
								instance.BindResponse = bnet.protocol.connection.BindResponse.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 != 42)
						{
							if (num1 == 48)
							{
								instance.ServerTime = ProtocolParser.ReadUInt64(stream);
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
						else if (instance.ContentHandleArray != null)
						{
							ConnectionMeteringContentHandles.DeserializeLengthDelimited(stream, instance.ContentHandleArray);
						}
						else
						{
							instance.ContentHandleArray = ConnectionMeteringContentHandles.DeserializeLengthDelimited(stream);
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

		public static ConnectResponse DeserializeLengthDelimited(Stream stream)
		{
			ConnectResponse connectResponse = new ConnectResponse();
			ConnectResponse.DeserializeLengthDelimited(stream, connectResponse);
			return connectResponse;
		}

		public static ConnectResponse DeserializeLengthDelimited(Stream stream, ConnectResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return ConnectResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ConnectResponse connectResponse = obj as ConnectResponse;
			if (connectResponse == null)
			{
				return false;
			}
			if (!this.ServerId.Equals(connectResponse.ServerId))
			{
				return false;
			}
			if (this.HasClientId != connectResponse.HasClientId || this.HasClientId && !this.ClientId.Equals(connectResponse.ClientId))
			{
				return false;
			}
			if (this.HasBindResult != connectResponse.HasBindResult || this.HasBindResult && !this.BindResult.Equals(connectResponse.BindResult))
			{
				return false;
			}
			if (this.HasBindResponse != connectResponse.HasBindResponse || this.HasBindResponse && !this.BindResponse.Equals(connectResponse.BindResponse))
			{
				return false;
			}
			if (this.HasContentHandleArray != connectResponse.HasContentHandleArray || this.HasContentHandleArray && !this.ContentHandleArray.Equals(connectResponse.ContentHandleArray))
			{
				return false;
			}
			if (this.HasServerTime == connectResponse.HasServerTime && (!this.HasServerTime || this.ServerTime.Equals(connectResponse.ServerTime)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.ServerId.GetHashCode();
			if (this.HasClientId)
			{
				hashCode = hashCode ^ this.ClientId.GetHashCode();
			}
			if (this.HasBindResult)
			{
				hashCode = hashCode ^ this.BindResult.GetHashCode();
			}
			if (this.HasBindResponse)
			{
				hashCode = hashCode ^ this.BindResponse.GetHashCode();
			}
			if (this.HasContentHandleArray)
			{
				hashCode = hashCode ^ this.ContentHandleArray.GetHashCode();
			}
			if (this.HasServerTime)
			{
				hashCode = hashCode ^ this.ServerTime.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.ServerId.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasClientId)
			{
				num++;
				uint serializedSize1 = this.ClientId.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			if (this.HasBindResult)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.BindResult);
			}
			if (this.HasBindResponse)
			{
				num++;
				uint num1 = this.BindResponse.GetSerializedSize();
				num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
			}
			if (this.HasContentHandleArray)
			{
				num++;
				uint serializedSize2 = this.ContentHandleArray.GetSerializedSize();
				num = num + serializedSize2 + ProtocolParser.SizeOfUInt32(serializedSize2);
			}
			if (this.HasServerTime)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt64(this.ServerTime);
			}
			num++;
			return num;
		}

		public static ConnectResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ConnectResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ConnectResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ConnectResponse instance)
		{
			if (instance.ServerId == null)
			{
				throw new ArgumentNullException("ServerId", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.ServerId.GetSerializedSize());
			ProcessId.Serialize(stream, instance.ServerId);
			if (instance.HasClientId)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.ClientId.GetSerializedSize());
				ProcessId.Serialize(stream, instance.ClientId);
			}
			if (instance.HasBindResult)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.BindResult);
			}
			if (instance.HasBindResponse)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteUInt32(stream, instance.BindResponse.GetSerializedSize());
				bnet.protocol.connection.BindResponse.Serialize(stream, instance.BindResponse);
			}
			if (instance.HasContentHandleArray)
			{
				stream.WriteByte(42);
				ProtocolParser.WriteUInt32(stream, instance.ContentHandleArray.GetSerializedSize());
				ConnectionMeteringContentHandles.Serialize(stream, instance.ContentHandleArray);
			}
			if (instance.HasServerTime)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteUInt64(stream, instance.ServerTime);
			}
		}

		public void SetBindResponse(bnet.protocol.connection.BindResponse val)
		{
			this.BindResponse = val;
		}

		public void SetBindResult(uint val)
		{
			this.BindResult = val;
		}

		public void SetClientId(ProcessId val)
		{
			this.ClientId = val;
		}

		public void SetContentHandleArray(ConnectionMeteringContentHandles val)
		{
			this.ContentHandleArray = val;
		}

		public void SetServerId(ProcessId val)
		{
			this.ServerId = val;
		}

		public void SetServerTime(ulong val)
		{
			this.ServerTime = val;
		}
	}
}