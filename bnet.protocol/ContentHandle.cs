using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol
{
	public class ContentHandle : IProtoBuf
	{
		public bool HasProtoUrl;

		private string _ProtoUrl;

		public byte[] Hash
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

		public string ProtoUrl
		{
			get
			{
				return this._ProtoUrl;
			}
			set
			{
				this._ProtoUrl = value;
				this.HasProtoUrl = value != null;
			}
		}

		public uint Region
		{
			get;
			set;
		}

		public uint Usage
		{
			get;
			set;
		}

		public ContentHandle()
		{
		}

		public void Deserialize(Stream stream)
		{
			ContentHandle.Deserialize(stream, this);
		}

		public static ContentHandle Deserialize(Stream stream, ContentHandle instance)
		{
			return ContentHandle.Deserialize(stream, instance, (long)-1);
		}

		public static ContentHandle Deserialize(Stream stream, ContentHandle instance, long limit)
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
							instance.Usage = binaryReader.ReadUInt32();
						}
						else if (num1 == 26)
						{
							instance.Hash = ProtocolParser.ReadBytes(stream);
						}
						else if (num1 == 34)
						{
							instance.ProtoUrl = ProtocolParser.ReadString(stream);
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

		public static ContentHandle DeserializeLengthDelimited(Stream stream)
		{
			ContentHandle contentHandle = new ContentHandle();
			ContentHandle.DeserializeLengthDelimited(stream, contentHandle);
			return contentHandle;
		}

		public static ContentHandle DeserializeLengthDelimited(Stream stream, ContentHandle instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return ContentHandle.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ContentHandle contentHandle = obj as ContentHandle;
			if (contentHandle == null)
			{
				return false;
			}
			if (!this.Region.Equals(contentHandle.Region))
			{
				return false;
			}
			if (!this.Usage.Equals(contentHandle.Usage))
			{
				return false;
			}
			if (!this.Hash.Equals(contentHandle.Hash))
			{
				return false;
			}
			if (this.HasProtoUrl == contentHandle.HasProtoUrl && (!this.HasProtoUrl || this.ProtoUrl.Equals(contentHandle.ProtoUrl)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.Region.GetHashCode();
			hashCode = hashCode ^ this.Usage.GetHashCode();
			hashCode = hashCode ^ this.Hash.GetHashCode();
			if (this.HasProtoUrl)
			{
				hashCode = hashCode ^ this.ProtoUrl.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num = num + 4;
			num = num + 4;
			num = num + ProtocolParser.SizeOfUInt32((int)this.Hash.Length) + (int)this.Hash.Length;
			if (this.HasProtoUrl)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.ProtoUrl);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			num = num + 3;
			return num;
		}

		public static ContentHandle ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ContentHandle>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ContentHandle.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ContentHandle instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			stream.WriteByte(13);
			binaryWriter.Write(instance.Region);
			stream.WriteByte(21);
			binaryWriter.Write(instance.Usage);
			if (instance.Hash == null)
			{
				throw new ArgumentNullException("Hash", "Required by proto specification.");
			}
			stream.WriteByte(26);
			ProtocolParser.WriteBytes(stream, instance.Hash);
			if (instance.HasProtoUrl)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.ProtoUrl));
			}
		}

		public void SetHash(byte[] val)
		{
			this.Hash = val;
		}

		public void SetProtoUrl(string val)
		{
			this.ProtoUrl = val;
		}

		public void SetRegion(uint val)
		{
			this.Region = val;
		}

		public void SetUsage(uint val)
		{
			this.Usage = val;
		}
	}
}