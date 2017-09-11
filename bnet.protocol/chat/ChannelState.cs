using System;
using System.IO;
using System.Text;

namespace bnet.protocol.chat
{
	public class ChannelState : IProtoBuf
	{
		public bool HasIdentity;

		private string _Identity;

		public bool HasProgram;

		private uint _Program;

		public bool HasLocale;

		private uint _Locale;

		public bool HasPublic;

		private bool _Public;

		public bool HasBucketIndex;

		private uint _BucketIndex;

		public uint BucketIndex
		{
			get
			{
				return this._BucketIndex;
			}
			set
			{
				this._BucketIndex = value;
				this.HasBucketIndex = true;
			}
		}

		public string Identity
		{
			get
			{
				return this._Identity;
			}
			set
			{
				this._Identity = value;
				this.HasIdentity = value != null;
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

		public bool Public
		{
			get
			{
				return this._Public;
			}
			set
			{
				this._Public = value;
				this.HasPublic = true;
			}
		}

		public ChannelState()
		{
		}

		public void Deserialize(Stream stream)
		{
			ChannelState.Deserialize(stream, this);
		}

		public static ChannelState Deserialize(Stream stream, ChannelState instance)
		{
			return ChannelState.Deserialize(stream, instance, (long)-1);
		}

		public static ChannelState Deserialize(Stream stream, ChannelState instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			instance.Public = false;
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						switch (num1)
						{
							case 29:
							{
								instance.Locale = binaryReader.ReadUInt32();
								continue;
							}
							case 32:
							{
								instance.Public = ProtocolParser.ReadBool(stream);
								continue;
							}
							default:
							{
								if (num1 == 10)
								{
									instance.Identity = ProtocolParser.ReadString(stream);
									continue;
								}
								else if (num1 == 21)
								{
									instance.Program = binaryReader.ReadUInt32();
									continue;
								}
								else if (num1 == 40)
								{
									instance.BucketIndex = ProtocolParser.ReadUInt32(stream);
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

		public static ChannelState DeserializeLengthDelimited(Stream stream)
		{
			ChannelState channelState = new ChannelState();
			ChannelState.DeserializeLengthDelimited(stream, channelState);
			return channelState;
		}

		public static ChannelState DeserializeLengthDelimited(Stream stream, ChannelState instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ChannelState.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ChannelState channelState = obj as ChannelState;
			if (channelState == null)
			{
				return false;
			}
			if (this.HasIdentity != channelState.HasIdentity || this.HasIdentity && !this.Identity.Equals(channelState.Identity))
			{
				return false;
			}
			if (this.HasProgram != channelState.HasProgram || this.HasProgram && !this.Program.Equals(channelState.Program))
			{
				return false;
			}
			if (this.HasLocale != channelState.HasLocale || this.HasLocale && !this.Locale.Equals(channelState.Locale))
			{
				return false;
			}
			if (this.HasPublic != channelState.HasPublic || this.HasPublic && !this.Public.Equals(channelState.Public))
			{
				return false;
			}
			if (this.HasBucketIndex == channelState.HasBucketIndex && (!this.HasBucketIndex || this.BucketIndex.Equals(channelState.BucketIndex)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasIdentity)
			{
				hashCode ^= this.Identity.GetHashCode();
			}
			if (this.HasProgram)
			{
				hashCode ^= this.Program.GetHashCode();
			}
			if (this.HasLocale)
			{
				hashCode ^= this.Locale.GetHashCode();
			}
			if (this.HasPublic)
			{
				hashCode ^= this.Public.GetHashCode();
			}
			if (this.HasBucketIndex)
			{
				hashCode ^= this.BucketIndex.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasIdentity)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Identity);
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
			if (this.HasPublic)
			{
				num++;
				num++;
			}
			if (this.HasBucketIndex)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.BucketIndex);
			}
			return num;
		}

		public static ChannelState ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ChannelState>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ChannelState.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ChannelState instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasIdentity)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Identity));
			}
			if (instance.HasProgram)
			{
				stream.WriteByte(21);
				binaryWriter.Write(instance.Program);
			}
			if (instance.HasLocale)
			{
				stream.WriteByte(29);
				binaryWriter.Write(instance.Locale);
			}
			if (instance.HasPublic)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteBool(stream, instance.Public);
			}
			if (instance.HasBucketIndex)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteUInt32(stream, instance.BucketIndex);
			}
		}

		public void SetBucketIndex(uint val)
		{
			this.BucketIndex = val;
		}

		public void SetIdentity(string val)
		{
			this.Identity = val;
		}

		public void SetLocale(uint val)
		{
			this.Locale = val;
		}

		public void SetProgram(uint val)
		{
			this.Program = val;
		}

		public void SetPublic(bool val)
		{
			this.Public = val;
		}
	}
}