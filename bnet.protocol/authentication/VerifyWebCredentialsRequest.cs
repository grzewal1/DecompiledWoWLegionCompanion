using System;
using System.IO;

namespace bnet.protocol.authentication
{
	public class VerifyWebCredentialsRequest : IProtoBuf
	{
		public bool HasWebCredentials;

		private byte[] _WebCredentials;

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public byte[] WebCredentials
		{
			get
			{
				return this._WebCredentials;
			}
			set
			{
				this._WebCredentials = value;
				this.HasWebCredentials = value != null;
			}
		}

		public VerifyWebCredentialsRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			VerifyWebCredentialsRequest.Deserialize(stream, this);
		}

		public static VerifyWebCredentialsRequest Deserialize(Stream stream, VerifyWebCredentialsRequest instance)
		{
			return VerifyWebCredentialsRequest.Deserialize(stream, instance, (long)-1);
		}

		public static VerifyWebCredentialsRequest Deserialize(Stream stream, VerifyWebCredentialsRequest instance, long limit)
		{
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
					else if (num == 10)
					{
						instance.WebCredentials = ProtocolParser.ReadBytes(stream);
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

		public static VerifyWebCredentialsRequest DeserializeLengthDelimited(Stream stream)
		{
			VerifyWebCredentialsRequest verifyWebCredentialsRequest = new VerifyWebCredentialsRequest();
			VerifyWebCredentialsRequest.DeserializeLengthDelimited(stream, verifyWebCredentialsRequest);
			return verifyWebCredentialsRequest;
		}

		public static VerifyWebCredentialsRequest DeserializeLengthDelimited(Stream stream, VerifyWebCredentialsRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return VerifyWebCredentialsRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			VerifyWebCredentialsRequest verifyWebCredentialsRequest = obj as VerifyWebCredentialsRequest;
			if (verifyWebCredentialsRequest == null)
			{
				return false;
			}
			if (this.HasWebCredentials == verifyWebCredentialsRequest.HasWebCredentials && (!this.HasWebCredentials || this.WebCredentials.Equals(verifyWebCredentialsRequest.WebCredentials)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasWebCredentials)
			{
				hashCode ^= this.WebCredentials.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasWebCredentials)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32((int)this.WebCredentials.Length) + (int)this.WebCredentials.Length;
			}
			return num;
		}

		public static VerifyWebCredentialsRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<VerifyWebCredentialsRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			VerifyWebCredentialsRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, VerifyWebCredentialsRequest instance)
		{
			if (instance.HasWebCredentials)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteBytes(stream, instance.WebCredentials);
			}
		}

		public void SetWebCredentials(byte[] val)
		{
			this.WebCredentials = val;
		}
	}
}