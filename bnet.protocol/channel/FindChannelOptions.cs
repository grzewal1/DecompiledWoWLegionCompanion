using bnet.protocol.attribute;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.channel
{
	public class FindChannelOptions : IProtoBuf
	{
		public bool HasStartIndex;

		private uint _StartIndex;

		public bool HasMaxResults;

		private uint _MaxResults;

		public bool HasName;

		private string _Name;

		public bool HasProgram;

		private uint _Program;

		public bool HasLocale;

		private uint _Locale;

		public bool HasCapacityFull;

		private uint _CapacityFull;

		public bool HasChannelType;

		private string _ChannelType;

		public bnet.protocol.attribute.AttributeFilter AttributeFilter
		{
			get;
			set;
		}

		public uint CapacityFull
		{
			get
			{
				return this._CapacityFull;
			}
			set
			{
				this._CapacityFull = value;
				this.HasCapacityFull = true;
			}
		}

		public string ChannelType
		{
			get
			{
				return this._ChannelType;
			}
			set
			{
				this._ChannelType = value;
				this.HasChannelType = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public uint Locale
		{
			get
			{
				return this._Locale;
			}
			set
			{
				this._Locale = value;
				this.HasLocale = true;
			}
		}

		public uint MaxResults
		{
			get
			{
				return this._MaxResults;
			}
			set
			{
				this._MaxResults = value;
				this.HasMaxResults = true;
			}
		}

		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				this._Name = value;
				this.HasName = value != null;
			}
		}

		public uint Program
		{
			get
			{
				return this._Program;
			}
			set
			{
				this._Program = value;
				this.HasProgram = true;
			}
		}

		public uint StartIndex
		{
			get
			{
				return this._StartIndex;
			}
			set
			{
				this._StartIndex = value;
				this.HasStartIndex = true;
			}
		}

		public FindChannelOptions()
		{
		}

		public void Deserialize(Stream stream)
		{
			FindChannelOptions.Deserialize(stream, this);
		}

		public static FindChannelOptions Deserialize(Stream stream, FindChannelOptions instance)
		{
			return FindChannelOptions.Deserialize(stream, instance, (long)-1);
		}

		public static FindChannelOptions Deserialize(Stream stream, FindChannelOptions instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			instance.StartIndex = 0;
			instance.MaxResults = 16;
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						switch (num)
						{
							case 45:
							{
								instance.Locale = binaryReader.ReadUInt32();
								continue;
							}
							case 48:
							{
								instance.CapacityFull = ProtocolParser.ReadUInt32(stream);
								continue;
							}
							default:
							{
								if (num == 8)
								{
									instance.StartIndex = ProtocolParser.ReadUInt32(stream);
									continue;
								}
								else if (num == 16)
								{
									instance.MaxResults = ProtocolParser.ReadUInt32(stream);
									continue;
								}
								else if (num == 26)
								{
									instance.Name = ProtocolParser.ReadString(stream);
									continue;
								}
								else if (num == 37)
								{
									instance.Program = binaryReader.ReadUInt32();
									continue;
								}
								else if (num == 58)
								{
									if (instance.AttributeFilter != null)
									{
										bnet.protocol.attribute.AttributeFilter.DeserializeLengthDelimited(stream, instance.AttributeFilter);
									}
									else
									{
										instance.AttributeFilter = bnet.protocol.attribute.AttributeFilter.DeserializeLengthDelimited(stream);
									}
									continue;
								}
								else if (num == 66)
								{
									instance.ChannelType = ProtocolParser.ReadString(stream);
									continue;
								}
								else
								{
									Key key = ProtocolParser.ReadKey((byte)num, stream);
									if (key.Field == 0)
									{
										throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
									}
									ProtocolParser.SkipKey(stream, key);
									continue;
								}
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

		public static FindChannelOptions DeserializeLengthDelimited(Stream stream)
		{
			FindChannelOptions findChannelOption = new FindChannelOptions();
			FindChannelOptions.DeserializeLengthDelimited(stream, findChannelOption);
			return findChannelOption;
		}

		public static FindChannelOptions DeserializeLengthDelimited(Stream stream, FindChannelOptions instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return FindChannelOptions.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			FindChannelOptions findChannelOption = obj as FindChannelOptions;
			if (findChannelOption == null)
			{
				return false;
			}
			if (this.HasStartIndex != findChannelOption.HasStartIndex || this.HasStartIndex && !this.StartIndex.Equals(findChannelOption.StartIndex))
			{
				return false;
			}
			if (this.HasMaxResults != findChannelOption.HasMaxResults || this.HasMaxResults && !this.MaxResults.Equals(findChannelOption.MaxResults))
			{
				return false;
			}
			if (this.HasName != findChannelOption.HasName || this.HasName && !this.Name.Equals(findChannelOption.Name))
			{
				return false;
			}
			if (this.HasProgram != findChannelOption.HasProgram || this.HasProgram && !this.Program.Equals(findChannelOption.Program))
			{
				return false;
			}
			if (this.HasLocale != findChannelOption.HasLocale || this.HasLocale && !this.Locale.Equals(findChannelOption.Locale))
			{
				return false;
			}
			if (this.HasCapacityFull != findChannelOption.HasCapacityFull || this.HasCapacityFull && !this.CapacityFull.Equals(findChannelOption.CapacityFull))
			{
				return false;
			}
			if (!this.AttributeFilter.Equals(findChannelOption.AttributeFilter))
			{
				return false;
			}
			if (this.HasChannelType == findChannelOption.HasChannelType && (!this.HasChannelType || this.ChannelType.Equals(findChannelOption.ChannelType)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasStartIndex)
			{
				hashCode ^= this.StartIndex.GetHashCode();
			}
			if (this.HasMaxResults)
			{
				hashCode ^= this.MaxResults.GetHashCode();
			}
			if (this.HasName)
			{
				hashCode ^= this.Name.GetHashCode();
			}
			if (this.HasProgram)
			{
				hashCode ^= this.Program.GetHashCode();
			}
			if (this.HasLocale)
			{
				hashCode ^= this.Locale.GetHashCode();
			}
			if (this.HasCapacityFull)
			{
				hashCode ^= this.CapacityFull.GetHashCode();
			}
			hashCode ^= this.AttributeFilter.GetHashCode();
			if (this.HasChannelType)
			{
				hashCode ^= this.ChannelType.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasStartIndex)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.StartIndex);
			}
			if (this.HasMaxResults)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.MaxResults);
			}
			if (this.HasName)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Name);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasProgram)
			{
				num++;
				num += 4;
			}
			if (this.HasLocale)
			{
				num++;
				num += 4;
			}
			if (this.HasCapacityFull)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.CapacityFull);
			}
			uint serializedSize = this.AttributeFilter.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasChannelType)
			{
				num++;
				uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.ChannelType);
				num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			}
			num++;
			return num;
		}

		public static FindChannelOptions ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<FindChannelOptions>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			FindChannelOptions.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, FindChannelOptions instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasStartIndex)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.StartIndex);
			}
			if (instance.HasMaxResults)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.MaxResults);
			}
			if (instance.HasName)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Name));
			}
			if (instance.HasProgram)
			{
				stream.WriteByte(37);
				binaryWriter.Write(instance.Program);
			}
			if (instance.HasLocale)
			{
				stream.WriteByte(45);
				binaryWriter.Write(instance.Locale);
			}
			if (instance.HasCapacityFull)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteUInt32(stream, instance.CapacityFull);
			}
			if (instance.AttributeFilter == null)
			{
				throw new ArgumentNullException("AttributeFilter", "Required by proto specification.");
			}
			stream.WriteByte(58);
			ProtocolParser.WriteUInt32(stream, instance.AttributeFilter.GetSerializedSize());
			bnet.protocol.attribute.AttributeFilter.Serialize(stream, instance.AttributeFilter);
			if (instance.HasChannelType)
			{
				stream.WriteByte(66);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.ChannelType));
			}
		}

		public void SetAttributeFilter(bnet.protocol.attribute.AttributeFilter val)
		{
			this.AttributeFilter = val;
		}

		public void SetCapacityFull(uint val)
		{
			this.CapacityFull = val;
		}

		public void SetChannelType(string val)
		{
			this.ChannelType = val;
		}

		public void SetLocale(uint val)
		{
			this.Locale = val;
		}

		public void SetMaxResults(uint val)
		{
			this.MaxResults = val;
		}

		public void SetName(string val)
		{
			this.Name = val;
		}

		public void SetProgram(uint val)
		{
			this.Program = val;
		}

		public void SetStartIndex(uint val)
		{
			this.StartIndex = val;
		}
	}
}