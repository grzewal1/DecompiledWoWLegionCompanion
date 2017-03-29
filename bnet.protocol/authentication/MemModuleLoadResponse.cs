using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.authentication
{
	public class MemModuleLoadResponse : IProtoBuf
	{
		public byte[] Data
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

		public MemModuleLoadResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			MemModuleLoadResponse.Deserialize(stream, this);
		}

		public static MemModuleLoadResponse Deserialize(Stream stream, MemModuleLoadResponse instance)
		{
			return MemModuleLoadResponse.Deserialize(stream, instance, (long)-1);
		}

		public static MemModuleLoadResponse Deserialize(Stream stream, MemModuleLoadResponse instance, long limit)
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
					else if (num == 10)
					{
						instance.Data = ProtocolParser.ReadBytes(stream);
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

		public static MemModuleLoadResponse DeserializeLengthDelimited(Stream stream)
		{
			MemModuleLoadResponse memModuleLoadResponse = new MemModuleLoadResponse();
			MemModuleLoadResponse.DeserializeLengthDelimited(stream, memModuleLoadResponse);
			return memModuleLoadResponse;
		}

		public static MemModuleLoadResponse DeserializeLengthDelimited(Stream stream, MemModuleLoadResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return MemModuleLoadResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			MemModuleLoadResponse memModuleLoadResponse = obj as MemModuleLoadResponse;
			if (memModuleLoadResponse == null)
			{
				return false;
			}
			if (!this.Data.Equals(memModuleLoadResponse.Data))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			return hashCode ^ this.Data.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num = num + ProtocolParser.SizeOfUInt32((int)this.Data.Length) + (int)this.Data.Length;
			return num + 1;
		}

		public static MemModuleLoadResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<MemModuleLoadResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			MemModuleLoadResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, MemModuleLoadResponse instance)
		{
			if (instance.Data == null)
			{
				throw new ArgumentNullException("Data", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteBytes(stream, instance.Data);
		}

		public void SetData(byte[] val)
		{
			this.Data = val;
		}
	}
}