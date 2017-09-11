using System;
using System.IO;
using System.Text;

namespace bnet.protocol.challenge
{
	public class ChallengeExternalRequest : IProtoBuf
	{
		public bool HasRequestToken;

		private string _RequestToken;

		public bool HasPayloadType;

		private string _PayloadType;

		public bool HasPayload;

		private byte[] _Payload;

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public byte[] Payload
		{
			get
			{
				return this._Payload;
			}
			set
			{
				this._Payload = value;
				this.HasPayload = value != null;
			}
		}

		public string PayloadType
		{
			get
			{
				return this._PayloadType;
			}
			set
			{
				this._PayloadType = value;
				this.HasPayloadType = value != null;
			}
		}

		public string RequestToken
		{
			get
			{
				return this._RequestToken;
			}
			set
			{
				this._RequestToken = value;
				this.HasRequestToken = value != null;
			}
		}

		public ChallengeExternalRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			ChallengeExternalRequest.Deserialize(stream, this);
		}

		public static ChallengeExternalRequest Deserialize(Stream stream, ChallengeExternalRequest instance)
		{
			return ChallengeExternalRequest.Deserialize(stream, instance, (long)-1);
		}

		public static ChallengeExternalRequest Deserialize(Stream stream, ChallengeExternalRequest instance, long limit)
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
							instance.RequestToken = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 18)
						{
							instance.PayloadType = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 26)
						{
							instance.Payload = ProtocolParser.ReadBytes(stream);
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

		public static ChallengeExternalRequest DeserializeLengthDelimited(Stream stream)
		{
			ChallengeExternalRequest challengeExternalRequest = new ChallengeExternalRequest();
			ChallengeExternalRequest.DeserializeLengthDelimited(stream, challengeExternalRequest);
			return challengeExternalRequest;
		}

		public static ChallengeExternalRequest DeserializeLengthDelimited(Stream stream, ChallengeExternalRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ChallengeExternalRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ChallengeExternalRequest challengeExternalRequest = obj as ChallengeExternalRequest;
			if (challengeExternalRequest == null)
			{
				return false;
			}
			if (this.HasRequestToken != challengeExternalRequest.HasRequestToken || this.HasRequestToken && !this.RequestToken.Equals(challengeExternalRequest.RequestToken))
			{
				return false;
			}
			if (this.HasPayloadType != challengeExternalRequest.HasPayloadType || this.HasPayloadType && !this.PayloadType.Equals(challengeExternalRequest.PayloadType))
			{
				return false;
			}
			if (this.HasPayload == challengeExternalRequest.HasPayload && (!this.HasPayload || this.Payload.Equals(challengeExternalRequest.Payload)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasRequestToken)
			{
				hashCode ^= this.RequestToken.GetHashCode();
			}
			if (this.HasPayloadType)
			{
				hashCode ^= this.PayloadType.GetHashCode();
			}
			if (this.HasPayload)
			{
				hashCode ^= this.Payload.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasRequestToken)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.RequestToken);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasPayloadType)
			{
				num++;
				uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.PayloadType);
				num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			}
			if (this.HasPayload)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32((int)this.Payload.Length) + (int)this.Payload.Length;
			}
			return num;
		}

		public static ChallengeExternalRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ChallengeExternalRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ChallengeExternalRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ChallengeExternalRequest instance)
		{
			if (instance.HasRequestToken)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.RequestToken));
			}
			if (instance.HasPayloadType)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.PayloadType));
			}
			if (instance.HasPayload)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteBytes(stream, instance.Payload);
			}
		}

		public void SetPayload(byte[] val)
		{
			this.Payload = val;
		}

		public void SetPayloadType(string val)
		{
			this.PayloadType = val;
		}

		public void SetRequestToken(string val)
		{
			this.RequestToken = val;
		}
	}
}