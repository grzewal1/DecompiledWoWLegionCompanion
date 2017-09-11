using System;
using System.IO;

namespace bnet.protocol.account
{
	public class CredentialUpdateResponse : IProtoBuf
	{
		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public CredentialUpdateResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			CredentialUpdateResponse.Deserialize(stream, this);
		}

		public static CredentialUpdateResponse Deserialize(Stream stream, CredentialUpdateResponse instance)
		{
			return CredentialUpdateResponse.Deserialize(stream, instance, (long)-1);
		}

		public static CredentialUpdateResponse Deserialize(Stream stream, CredentialUpdateResponse instance, long limit)
		{
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
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

		public static CredentialUpdateResponse DeserializeLengthDelimited(Stream stream)
		{
			CredentialUpdateResponse credentialUpdateResponse = new CredentialUpdateResponse();
			CredentialUpdateResponse.DeserializeLengthDelimited(stream, credentialUpdateResponse);
			return credentialUpdateResponse;
		}

		public static CredentialUpdateResponse DeserializeLengthDelimited(Stream stream, CredentialUpdateResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return CredentialUpdateResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is CredentialUpdateResponse))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return this.GetType().GetHashCode();
		}

		public uint GetSerializedSize()
		{
			return (uint)0;
		}

		public static CredentialUpdateResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<CredentialUpdateResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			CredentialUpdateResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, CredentialUpdateResponse instance)
		{
		}
	}
}