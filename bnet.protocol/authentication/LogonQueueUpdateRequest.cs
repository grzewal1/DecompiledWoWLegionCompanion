using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.authentication
{
	public class LogonQueueUpdateRequest : IProtoBuf
	{
		public ulong EstimatedTime
		{
			get;
			set;
		}

		public ulong EtaDeviationInSec
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

		public uint Position
		{
			get;
			set;
		}

		public LogonQueueUpdateRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			LogonQueueUpdateRequest.Deserialize(stream, this);
		}

		public static LogonQueueUpdateRequest Deserialize(Stream stream, LogonQueueUpdateRequest instance)
		{
			return LogonQueueUpdateRequest.Deserialize(stream, instance, (long)-1);
		}

		public static LogonQueueUpdateRequest Deserialize(Stream stream, LogonQueueUpdateRequest instance, long limit)
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
							instance.Position = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 16)
						{
							instance.EstimatedTime = ProtocolParser.ReadUInt64(stream);
						}
						else if (num1 == 24)
						{
							instance.EtaDeviationInSec = ProtocolParser.ReadUInt64(stream);
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

		public static LogonQueueUpdateRequest DeserializeLengthDelimited(Stream stream)
		{
			LogonQueueUpdateRequest logonQueueUpdateRequest = new LogonQueueUpdateRequest();
			LogonQueueUpdateRequest.DeserializeLengthDelimited(stream, logonQueueUpdateRequest);
			return logonQueueUpdateRequest;
		}

		public static LogonQueueUpdateRequest DeserializeLengthDelimited(Stream stream, LogonQueueUpdateRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return LogonQueueUpdateRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			LogonQueueUpdateRequest logonQueueUpdateRequest = obj as LogonQueueUpdateRequest;
			if (logonQueueUpdateRequest == null)
			{
				return false;
			}
			if (!this.Position.Equals(logonQueueUpdateRequest.Position))
			{
				return false;
			}
			if (!this.EstimatedTime.Equals(logonQueueUpdateRequest.EstimatedTime))
			{
				return false;
			}
			if (!this.EtaDeviationInSec.Equals(logonQueueUpdateRequest.EtaDeviationInSec))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.Position.GetHashCode();
			hashCode = hashCode ^ this.EstimatedTime.GetHashCode();
			return hashCode ^ this.EtaDeviationInSec.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0 + ProtocolParser.SizeOfUInt32(this.Position);
			num = num + ProtocolParser.SizeOfUInt64(this.EstimatedTime);
			num = num + ProtocolParser.SizeOfUInt64(this.EtaDeviationInSec);
			return num + 3;
		}

		public static LogonQueueUpdateRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<LogonQueueUpdateRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			LogonQueueUpdateRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, LogonQueueUpdateRequest instance)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.Position);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt64(stream, instance.EstimatedTime);
			stream.WriteByte(24);
			ProtocolParser.WriteUInt64(stream, instance.EtaDeviationInSec);
		}

		public void SetEstimatedTime(ulong val)
		{
			this.EstimatedTime = val;
		}

		public void SetEtaDeviationInSec(ulong val)
		{
			this.EtaDeviationInSec = val;
		}

		public void SetPosition(uint val)
		{
			this.Position = val;
		}
	}
}