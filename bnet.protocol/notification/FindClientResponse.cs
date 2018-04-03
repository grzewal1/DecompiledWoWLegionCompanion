using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.notification
{
	public class FindClientResponse : IProtoBuf
	{
		public bool HasClientProcessId;

		private ProcessId _ClientProcessId;

		public ProcessId ClientProcessId
		{
			get
			{
				return this._ClientProcessId;
			}
			set
			{
				this._ClientProcessId = value;
				this.HasClientProcessId = value != null;
			}
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

		public FindClientResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			FindClientResponse.Deserialize(stream, this);
		}

		public static FindClientResponse Deserialize(Stream stream, FindClientResponse instance)
		{
			return FindClientResponse.Deserialize(stream, instance, (long)-1);
		}

		public static FindClientResponse Deserialize(Stream stream, FindClientResponse instance, long limit)
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
						instance.Label = ProtocolParser.ReadUInt32(stream);
					}
					else if (num != 18)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.ClientProcessId != null)
					{
						ProcessId.DeserializeLengthDelimited(stream, instance.ClientProcessId);
					}
					else
					{
						instance.ClientProcessId = ProcessId.DeserializeLengthDelimited(stream);
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

		public static FindClientResponse DeserializeLengthDelimited(Stream stream)
		{
			FindClientResponse findClientResponse = new FindClientResponse();
			FindClientResponse.DeserializeLengthDelimited(stream, findClientResponse);
			return findClientResponse;
		}

		public static FindClientResponse DeserializeLengthDelimited(Stream stream, FindClientResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return FindClientResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			FindClientResponse findClientResponse = obj as FindClientResponse;
			if (findClientResponse == null)
			{
				return false;
			}
			if (!this.Label.Equals(findClientResponse.Label))
			{
				return false;
			}
			if (this.HasClientProcessId == findClientResponse.HasClientProcessId && (!this.HasClientProcessId || this.ClientProcessId.Equals(findClientResponse.ClientProcessId)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Label.GetHashCode();
			if (this.HasClientProcessId)
			{
				hashCode ^= this.ClientProcessId.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num += ProtocolParser.SizeOfUInt32(this.Label);
			if (this.HasClientProcessId)
			{
				num++;
				uint serializedSize = this.ClientProcessId.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			num++;
			return num;
		}

		public static FindClientResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<FindClientResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			FindClientResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, FindClientResponse instance)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.Label);
			if (instance.HasClientProcessId)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.ClientProcessId.GetSerializedSize());
				ProcessId.Serialize(stream, instance.ClientProcessId);
			}
		}

		public void SetClientProcessId(ProcessId val)
		{
			this.ClientProcessId = val;
		}

		public void SetLabel(uint val)
		{
			this.Label = val;
		}
	}
}