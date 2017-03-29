using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.connection
{
	public class BoundService : IProtoBuf
	{
		public uint Hash
		{
			get;
			set;
		}

		public uint Id
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

		public BoundService()
		{
		}

		public void Deserialize(Stream stream)
		{
			BoundService.Deserialize(stream, this);
		}

		public static BoundService Deserialize(Stream stream, BoundService instance)
		{
			return BoundService.Deserialize(stream, instance, (long)-1);
		}

		public static BoundService Deserialize(Stream stream, BoundService instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						switch (num)
						{
							case 13:
							{
								instance.Hash = binaryReader.ReadUInt32();
								continue;
							}
							case 14:
							case 15:
							{
								Key key = ProtocolParser.ReadKey((byte)num, stream);
								if (key.Field == 0)
								{
									throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
								}
								ProtocolParser.SkipKey(stream, key);
								continue;
							}
							case 16:
							{
								instance.Id = ProtocolParser.ReadUInt32(stream);
								continue;
							}
							default:
							{
								goto case 15;
							}
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

		public static BoundService DeserializeLengthDelimited(Stream stream)
		{
			BoundService boundService = new BoundService();
			BoundService.DeserializeLengthDelimited(stream, boundService);
			return boundService;
		}

		public static BoundService DeserializeLengthDelimited(Stream stream, BoundService instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return BoundService.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			BoundService boundService = obj as BoundService;
			if (boundService == null)
			{
				return false;
			}
			if (!this.Hash.Equals(boundService.Hash))
			{
				return false;
			}
			if (!this.Id.Equals(boundService.Id))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.Hash.GetHashCode();
			return hashCode ^ this.Id.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0 + 4;
			num = num + ProtocolParser.SizeOfUInt32(this.Id);
			return num + 2;
		}

		public static BoundService ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<BoundService>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			BoundService.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, BoundService instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			stream.WriteByte(13);
			binaryWriter.Write(instance.Hash);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt32(stream, instance.Id);
		}

		public void SetHash(uint val)
		{
			this.Hash = val;
		}

		public void SetId(uint val)
		{
			this.Id = val;
		}
	}
}