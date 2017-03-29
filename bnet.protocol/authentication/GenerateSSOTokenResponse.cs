using System;
using System.IO;

namespace bnet.protocol.authentication
{
	public class GenerateSSOTokenResponse : IProtoBuf
	{
		public bool HasSsoId;

		private byte[] _SsoId;

		public bool HasSsoSecret;

		private byte[] _SsoSecret;

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public byte[] SsoId
		{
			get
			{
				return this._SsoId;
			}
			set
			{
				this._SsoId = value;
				this.HasSsoId = value != null;
			}
		}

		public byte[] SsoSecret
		{
			get
			{
				return this._SsoSecret;
			}
			set
			{
				this._SsoSecret = value;
				this.HasSsoSecret = value != null;
			}
		}

		public GenerateSSOTokenResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			GenerateSSOTokenResponse.Deserialize(stream, this);
		}

		public static GenerateSSOTokenResponse Deserialize(Stream stream, GenerateSSOTokenResponse instance)
		{
			return GenerateSSOTokenResponse.Deserialize(stream, instance, (long)-1);
		}

		public static GenerateSSOTokenResponse Deserialize(Stream stream, GenerateSSOTokenResponse instance, long limit)
		{
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 == 10)
						{
							instance.SsoId = ProtocolParser.ReadBytes(stream);
						}
						else if (num1 == 18)
						{
							instance.SsoSecret = ProtocolParser.ReadBytes(stream);
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

		public static GenerateSSOTokenResponse DeserializeLengthDelimited(Stream stream)
		{
			GenerateSSOTokenResponse generateSSOTokenResponse = new GenerateSSOTokenResponse();
			GenerateSSOTokenResponse.DeserializeLengthDelimited(stream, generateSSOTokenResponse);
			return generateSSOTokenResponse;
		}

		public static GenerateSSOTokenResponse DeserializeLengthDelimited(Stream stream, GenerateSSOTokenResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GenerateSSOTokenResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GenerateSSOTokenResponse generateSSOTokenResponse = obj as GenerateSSOTokenResponse;
			if (generateSSOTokenResponse == null)
			{
				return false;
			}
			if (this.HasSsoId != generateSSOTokenResponse.HasSsoId || this.HasSsoId && !this.SsoId.Equals(generateSSOTokenResponse.SsoId))
			{
				return false;
			}
			if (this.HasSsoSecret == generateSSOTokenResponse.HasSsoSecret && (!this.HasSsoSecret || this.SsoSecret.Equals(generateSSOTokenResponse.SsoSecret)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasSsoId)
			{
				hashCode = hashCode ^ this.SsoId.GetHashCode();
			}
			if (this.HasSsoSecret)
			{
				hashCode = hashCode ^ this.SsoSecret.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasSsoId)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32((int)this.SsoId.Length) + (int)this.SsoId.Length;
			}
			if (this.HasSsoSecret)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32((int)this.SsoSecret.Length) + (int)this.SsoSecret.Length;
			}
			return num;
		}

		public static GenerateSSOTokenResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GenerateSSOTokenResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GenerateSSOTokenResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GenerateSSOTokenResponse instance)
		{
			if (instance.HasSsoId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteBytes(stream, instance.SsoId);
			}
			if (instance.HasSsoSecret)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteBytes(stream, instance.SsoSecret);
			}
		}

		public void SetSsoId(byte[] val)
		{
			this.SsoId = val;
		}

		public void SetSsoSecret(byte[] val)
		{
			this.SsoSecret = val;
		}
	}
}