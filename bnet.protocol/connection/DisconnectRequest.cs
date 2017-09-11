using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.connection
{
	public class DisconnectRequest : IProtoBuf
	{
		public uint ErrorCode
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

		public DisconnectRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			DisconnectRequest.Deserialize(stream, this);
		}

		public static DisconnectRequest Deserialize(Stream stream, DisconnectRequest instance)
		{
			return DisconnectRequest.Deserialize(stream, instance, (long)-1);
		}

		public static DisconnectRequest Deserialize(Stream stream, DisconnectRequest instance, long limit)
		{
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
					else if (num == 8)
					{
						instance.ErrorCode = ProtocolParser.ReadUInt32(stream);
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

		public static DisconnectRequest DeserializeLengthDelimited(Stream stream)
		{
			DisconnectRequest disconnectRequest = new DisconnectRequest();
			DisconnectRequest.DeserializeLengthDelimited(stream, disconnectRequest);
			return disconnectRequest;
		}

		public static DisconnectRequest DeserializeLengthDelimited(Stream stream, DisconnectRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return DisconnectRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			DisconnectRequest disconnectRequest = obj as DisconnectRequest;
			if (disconnectRequest == null)
			{
				return false;
			}
			if (!this.ErrorCode.Equals(disconnectRequest.ErrorCode))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			return hashCode ^ this.ErrorCode.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0 + ProtocolParser.SizeOfUInt32(this.ErrorCode);
			return num + 1;
		}

		public static DisconnectRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<DisconnectRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			DisconnectRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, DisconnectRequest instance)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.ErrorCode);
		}

		public void SetErrorCode(uint val)
		{
			this.ErrorCode = val;
		}
	}
}