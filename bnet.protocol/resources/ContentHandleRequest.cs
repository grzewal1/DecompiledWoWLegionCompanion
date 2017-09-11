using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.resources
{
	public class ContentHandleRequest : IProtoBuf
	{
		public bool HasLocale;

		private uint _Locale;

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

		public uint ProgramId
		{
			get;
			set;
		}

		public uint StreamId
		{
			get;
			set;
		}

		public ContentHandleRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			ContentHandleRequest.Deserialize(stream, this);
		}

		public static ContentHandleRequest Deserialize(Stream stream, ContentHandleRequest instance)
		{
			return ContentHandleRequest.Deserialize(stream, instance, (long)-1);
		}

		public static ContentHandleRequest Deserialize(Stream stream, ContentHandleRequest instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			instance.Locale = 1701729619;
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
							instance.ProgramId = binaryReader.ReadUInt32();
						}
						else if (num1 == 21)
						{
							instance.StreamId = binaryReader.ReadUInt32();
						}
						else if (num1 == 29)
						{
							instance.Locale = binaryReader.ReadUInt32();
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

		public static ContentHandleRequest DeserializeLengthDelimited(Stream stream)
		{
			ContentHandleRequest contentHandleRequest = new ContentHandleRequest();
			ContentHandleRequest.DeserializeLengthDelimited(stream, contentHandleRequest);
			return contentHandleRequest;
		}

		public static ContentHandleRequest DeserializeLengthDelimited(Stream stream, ContentHandleRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ContentHandleRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ContentHandleRequest contentHandleRequest = obj as ContentHandleRequest;
			if (contentHandleRequest == null)
			{
				return false;
			}
			if (!this.ProgramId.Equals(contentHandleRequest.ProgramId))
			{
				return false;
			}
			if (!this.StreamId.Equals(contentHandleRequest.StreamId))
			{
				return false;
			}
			if (this.HasLocale == contentHandleRequest.HasLocale && (!this.HasLocale || this.Locale.Equals(contentHandleRequest.Locale)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.ProgramId.GetHashCode();
			hashCode ^= this.StreamId.GetHashCode();
			if (this.HasLocale)
			{
				hashCode ^= this.Locale.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num += 4;
			num += 4;
			if (this.HasLocale)
			{
				num++;
				num += 4;
			}
			num += 2;
			return num;
		}

		public static ContentHandleRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ContentHandleRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ContentHandleRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ContentHandleRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			stream.WriteByte(13);
			binaryWriter.Write(instance.ProgramId);
			stream.WriteByte(21);
			binaryWriter.Write(instance.StreamId);
			if (instance.HasLocale)
			{
				stream.WriteByte(29);
				binaryWriter.Write(instance.Locale);
			}
		}

		public void SetLocale(uint val)
		{
			this.Locale = val;
		}

		public void SetProgramId(uint val)
		{
			this.ProgramId = val;
		}

		public void SetStreamId(uint val)
		{
			this.StreamId = val;
		}
	}
}