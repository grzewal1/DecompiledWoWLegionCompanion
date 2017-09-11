using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.game_master
{
	public class GetFactoryInfoRequest : IProtoBuf
	{
		public ulong FactoryId
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

		public GetFactoryInfoRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetFactoryInfoRequest.Deserialize(stream, this);
		}

		public static GetFactoryInfoRequest Deserialize(Stream stream, GetFactoryInfoRequest instance)
		{
			return GetFactoryInfoRequest.Deserialize(stream, instance, (long)-1);
		}

		public static GetFactoryInfoRequest Deserialize(Stream stream, GetFactoryInfoRequest instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
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
						instance.FactoryId = binaryReader.ReadUInt64();
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

		public static GetFactoryInfoRequest DeserializeLengthDelimited(Stream stream)
		{
			GetFactoryInfoRequest getFactoryInfoRequest = new GetFactoryInfoRequest();
			GetFactoryInfoRequest.DeserializeLengthDelimited(stream, getFactoryInfoRequest);
			return getFactoryInfoRequest;
		}

		public static GetFactoryInfoRequest DeserializeLengthDelimited(Stream stream, GetFactoryInfoRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GetFactoryInfoRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetFactoryInfoRequest getFactoryInfoRequest = obj as GetFactoryInfoRequest;
			if (getFactoryInfoRequest == null)
			{
				return false;
			}
			if (!this.FactoryId.Equals(getFactoryInfoRequest.FactoryId))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			return hashCode ^ this.FactoryId.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			return 0 + 8 + 1;
		}

		public static GetFactoryInfoRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetFactoryInfoRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetFactoryInfoRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetFactoryInfoRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			stream.WriteByte(9);
			binaryWriter.Write(instance.FactoryId);
		}

		public void SetFactoryId(ulong val)
		{
			this.FactoryId = val;
		}
	}
}