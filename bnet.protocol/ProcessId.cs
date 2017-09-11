using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol
{
	public class ProcessId : IProtoBuf
	{
		public uint Epoch
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

		public uint Label
		{
			get;
			set;
		}

		public ProcessId()
		{
		}

		public void Deserialize(Stream stream)
		{
			ProcessId.Deserialize(stream, this);
		}

		public static ProcessId Deserialize(Stream stream, ProcessId instance)
		{
			return ProcessId.Deserialize(stream, instance, (long)-1);
		}

		public static ProcessId Deserialize(Stream stream, ProcessId instance, long limit)
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
							instance.Label = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 16)
						{
							instance.Epoch = ProtocolParser.ReadUInt32(stream);
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

		public static ProcessId DeserializeLengthDelimited(Stream stream)
		{
			ProcessId processId = new ProcessId();
			ProcessId.DeserializeLengthDelimited(stream, processId);
			return processId;
		}

		public static ProcessId DeserializeLengthDelimited(Stream stream, ProcessId instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ProcessId.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ProcessId processId = obj as ProcessId;
			if (processId == null)
			{
				return false;
			}
			if (!this.Label.Equals(processId.Label))
			{
				return false;
			}
			if (!this.Epoch.Equals(processId.Epoch))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Label.GetHashCode();
			return hashCode ^ this.Epoch.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0 + ProtocolParser.SizeOfUInt32(this.Label);
			num += ProtocolParser.SizeOfUInt32(this.Epoch);
			return num + 2;
		}

		public static ProcessId ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ProcessId>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ProcessId.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ProcessId instance)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.Label);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt32(stream, instance.Epoch);
		}

		public void SetEpoch(uint val)
		{
			this.Epoch = val;
		}

		public void SetLabel(uint val)
		{
			this.Label = val;
		}
	}
}