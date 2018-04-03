using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.authentication
{
	public class ServerStateChangeRequest : IProtoBuf
	{
		public ulong EventTime
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

		public uint State
		{
			get;
			set;
		}

		public ServerStateChangeRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			ServerStateChangeRequest.Deserialize(stream, this);
		}

		public static ServerStateChangeRequest Deserialize(Stream stream, ServerStateChangeRequest instance)
		{
			return ServerStateChangeRequest.Deserialize(stream, instance, (long)-1);
		}

		public static ServerStateChangeRequest Deserialize(Stream stream, ServerStateChangeRequest instance, long limit)
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
						instance.State = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 16)
					{
						instance.EventTime = ProtocolParser.ReadUInt64(stream);
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

		public static ServerStateChangeRequest DeserializeLengthDelimited(Stream stream)
		{
			ServerStateChangeRequest serverStateChangeRequest = new ServerStateChangeRequest();
			ServerStateChangeRequest.DeserializeLengthDelimited(stream, serverStateChangeRequest);
			return serverStateChangeRequest;
		}

		public static ServerStateChangeRequest DeserializeLengthDelimited(Stream stream, ServerStateChangeRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ServerStateChangeRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ServerStateChangeRequest serverStateChangeRequest = obj as ServerStateChangeRequest;
			if (serverStateChangeRequest == null)
			{
				return false;
			}
			if (!this.State.Equals(serverStateChangeRequest.State))
			{
				return false;
			}
			if (!this.EventTime.Equals(serverStateChangeRequest.EventTime))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.State.GetHashCode();
			return hashCode ^ this.EventTime.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0 + ProtocolParser.SizeOfUInt32(this.State);
			num += ProtocolParser.SizeOfUInt64(this.EventTime);
			return num + 2;
		}

		public static ServerStateChangeRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ServerStateChangeRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ServerStateChangeRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ServerStateChangeRequest instance)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.State);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt64(stream, instance.EventTime);
		}

		public void SetEventTime(ulong val)
		{
			this.EventTime = val;
		}

		public void SetState(uint val)
		{
			this.State = val;
		}
	}
}