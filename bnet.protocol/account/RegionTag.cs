using System;
using System.IO;

namespace bnet.protocol.account
{
	public class RegionTag : IProtoBuf
	{
		public bool HasRegion;

		private uint _Region;

		public bool HasTag;

		private uint _Tag;

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public uint Region
		{
			get
			{
				return this._Region;
			}
			set
			{
				this._Region = value;
				this.HasRegion = true;
			}
		}

		public uint Tag
		{
			get
			{
				return this._Tag;
			}
			set
			{
				this._Tag = value;
				this.HasTag = true;
			}
		}

		public RegionTag()
		{
		}

		public void Deserialize(Stream stream)
		{
			RegionTag.Deserialize(stream, this);
		}

		public static RegionTag Deserialize(Stream stream, RegionTag instance)
		{
			return RegionTag.Deserialize(stream, instance, (long)-1);
		}

		public static RegionTag Deserialize(Stream stream, RegionTag instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 == 13)
						{
							instance.Region = binaryReader.ReadUInt32();
						}
						else if (num1 == 21)
						{
							instance.Tag = binaryReader.ReadUInt32();
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

		public static RegionTag DeserializeLengthDelimited(Stream stream)
		{
			RegionTag regionTag = new RegionTag();
			RegionTag.DeserializeLengthDelimited(stream, regionTag);
			return regionTag;
		}

		public static RegionTag DeserializeLengthDelimited(Stream stream, RegionTag instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return RegionTag.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			RegionTag regionTag = obj as RegionTag;
			if (regionTag == null)
			{
				return false;
			}
			if (this.HasRegion != regionTag.HasRegion || this.HasRegion && !this.Region.Equals(regionTag.Region))
			{
				return false;
			}
			if (this.HasTag == regionTag.HasTag && (!this.HasTag || this.Tag.Equals(regionTag.Tag)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasRegion)
			{
				hashCode = hashCode ^ this.Region.GetHashCode();
			}
			if (this.HasTag)
			{
				hashCode = hashCode ^ this.Tag.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasRegion)
			{
				num++;
				num = num + 4;
			}
			if (this.HasTag)
			{
				num++;
				num = num + 4;
			}
			return num;
		}

		public static RegionTag ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<RegionTag>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			RegionTag.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, RegionTag instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasRegion)
			{
				stream.WriteByte(13);
				binaryWriter.Write(instance.Region);
			}
			if (instance.HasTag)
			{
				stream.WriteByte(21);
				binaryWriter.Write(instance.Tag);
			}
		}

		public void SetRegion(uint val)
		{
			this.Region = val;
		}

		public void SetTag(uint val)
		{
			this.Tag = val;
		}
	}
}