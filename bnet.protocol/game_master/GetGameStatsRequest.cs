using bnet.protocol.attribute;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.game_master
{
	public class GetGameStatsRequest : IProtoBuf
	{
		public ulong FactoryId
		{
			get;
			set;
		}

		public AttributeFilter Filter
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

		public GetGameStatsRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetGameStatsRequest.Deserialize(stream, this);
		}

		public static GetGameStatsRequest Deserialize(Stream stream, GetGameStatsRequest instance)
		{
			return GetGameStatsRequest.Deserialize(stream, instance, (long)-1);
		}

		public static GetGameStatsRequest Deserialize(Stream stream, GetGameStatsRequest instance, long limit)
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
					else if (num != 18)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.Filter != null)
					{
						AttributeFilter.DeserializeLengthDelimited(stream, instance.Filter);
					}
					else
					{
						instance.Filter = AttributeFilter.DeserializeLengthDelimited(stream);
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

		public static GetGameStatsRequest DeserializeLengthDelimited(Stream stream)
		{
			GetGameStatsRequest getGameStatsRequest = new GetGameStatsRequest();
			GetGameStatsRequest.DeserializeLengthDelimited(stream, getGameStatsRequest);
			return getGameStatsRequest;
		}

		public static GetGameStatsRequest DeserializeLengthDelimited(Stream stream, GetGameStatsRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GetGameStatsRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetGameStatsRequest getGameStatsRequest = obj as GetGameStatsRequest;
			if (getGameStatsRequest == null)
			{
				return false;
			}
			if (!this.FactoryId.Equals(getGameStatsRequest.FactoryId))
			{
				return false;
			}
			if (!this.Filter.Equals(getGameStatsRequest.Filter))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.FactoryId.GetHashCode();
			return hashCode ^ this.Filter.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0 + 8;
			uint serializedSize = this.Filter.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			return num + 2;
		}

		public static GetGameStatsRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetGameStatsRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetGameStatsRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetGameStatsRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			stream.WriteByte(9);
			binaryWriter.Write(instance.FactoryId);
			if (instance.Filter == null)
			{
				throw new ArgumentNullException("Filter", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.Filter.GetSerializedSize());
			AttributeFilter.Serialize(stream, instance.Filter);
		}

		public void SetFactoryId(ulong val)
		{
			this.FactoryId = val;
		}

		public void SetFilter(AttributeFilter val)
		{
			this.Filter = val;
		}
	}
}