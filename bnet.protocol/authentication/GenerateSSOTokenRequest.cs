using System;
using System.IO;

namespace bnet.protocol.authentication
{
	public class GenerateSSOTokenRequest : IProtoBuf
	{
		public bool HasProgram;

		private uint _Program;

		public bool IsInitialized
		{
			get
			{
				return true;
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

		public GenerateSSOTokenRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			GenerateSSOTokenRequest.Deserialize(stream, this);
		}

		public static GenerateSSOTokenRequest Deserialize(Stream stream, GenerateSSOTokenRequest instance)
		{
			return GenerateSSOTokenRequest.Deserialize(stream, instance, (long)-1);
		}

		public static GenerateSSOTokenRequest Deserialize(Stream stream, GenerateSSOTokenRequest instance, long limit)
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
					else if (num == 13)
					{
						instance.Program = binaryReader.ReadUInt32();
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

		public static GenerateSSOTokenRequest DeserializeLengthDelimited(Stream stream)
		{
			GenerateSSOTokenRequest generateSSOTokenRequest = new GenerateSSOTokenRequest();
			GenerateSSOTokenRequest.DeserializeLengthDelimited(stream, generateSSOTokenRequest);
			return generateSSOTokenRequest;
		}

		public static GenerateSSOTokenRequest DeserializeLengthDelimited(Stream stream, GenerateSSOTokenRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GenerateSSOTokenRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GenerateSSOTokenRequest generateSSOTokenRequest = obj as GenerateSSOTokenRequest;
			if (generateSSOTokenRequest == null)
			{
				return false;
			}
			if (this.HasProgram == generateSSOTokenRequest.HasProgram && (!this.HasProgram || this.Program.Equals(generateSSOTokenRequest.Program)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasProgram)
			{
				hashCode = hashCode ^ this.Program.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasProgram)
			{
				num++;
				num = num + 4;
			}
			return num;
		}

		public static GenerateSSOTokenRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GenerateSSOTokenRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GenerateSSOTokenRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GenerateSSOTokenRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasProgram)
			{
				stream.WriteByte(13);
				binaryWriter.Write(instance.Program);
			}
		}

		public void SetProgram(uint val)
		{
			this.Program = val;
		}
	}
}