using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.authentication
{
	public class LogonUpdateRequest : IProtoBuf
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

		public LogonUpdateRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			LogonUpdateRequest.Deserialize(stream, this);
		}

		public static LogonUpdateRequest Deserialize(Stream stream, LogonUpdateRequest instance)
		{
			return LogonUpdateRequest.Deserialize(stream, instance, (long)-1);
		}

		public static LogonUpdateRequest Deserialize(Stream stream, LogonUpdateRequest instance, long limit)
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

		public static LogonUpdateRequest DeserializeLengthDelimited(Stream stream)
		{
			LogonUpdateRequest logonUpdateRequest = new LogonUpdateRequest();
			LogonUpdateRequest.DeserializeLengthDelimited(stream, logonUpdateRequest);
			return logonUpdateRequest;
		}

		public static LogonUpdateRequest DeserializeLengthDelimited(Stream stream, LogonUpdateRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return LogonUpdateRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			LogonUpdateRequest logonUpdateRequest = obj as LogonUpdateRequest;
			if (logonUpdateRequest == null)
			{
				return false;
			}
			if (!this.ErrorCode.Equals(logonUpdateRequest.ErrorCode))
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

		public static LogonUpdateRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<LogonUpdateRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			LogonUpdateRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, LogonUpdateRequest instance)
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