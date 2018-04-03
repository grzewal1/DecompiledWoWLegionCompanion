using System;
using System.IO;
using System.Text;

namespace bnet.protocol.challenge
{
	public class ChallengeExternalResult : IProtoBuf
	{
		public bool HasRequestToken;

		private string _RequestToken;

		public bool HasPassed;

		private bool _Passed;

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public bool Passed
		{
			get
			{
				return this._Passed;
			}
			set
			{
				this._Passed = value;
				this.HasPassed = true;
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

		public ChallengeExternalResult()
		{
		}

		public void Deserialize(Stream stream)
		{
			ChallengeExternalResult.Deserialize(stream, this);
		}

		public static ChallengeExternalResult Deserialize(Stream stream, ChallengeExternalResult instance)
		{
			return ChallengeExternalResult.Deserialize(stream, instance, (long)-1);
		}

		public static ChallengeExternalResult Deserialize(Stream stream, ChallengeExternalResult instance, long limit)
		{
			instance.Passed = true;
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
						instance.RequestToken = ProtocolParser.ReadString(stream);
					}
					else if (num == 16)
					{
						instance.Passed = ProtocolParser.ReadBool(stream);
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

		public static ChallengeExternalResult DeserializeLengthDelimited(Stream stream)
		{
			ChallengeExternalResult challengeExternalResult = new ChallengeExternalResult();
			ChallengeExternalResult.DeserializeLengthDelimited(stream, challengeExternalResult);
			return challengeExternalResult;
		}

		public static ChallengeExternalResult DeserializeLengthDelimited(Stream stream, ChallengeExternalResult instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ChallengeExternalResult.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ChallengeExternalResult challengeExternalResult = obj as ChallengeExternalResult;
			if (challengeExternalResult == null)
			{
				return false;
			}
			if (this.HasRequestToken != challengeExternalResult.HasRequestToken || this.HasRequestToken && !this.RequestToken.Equals(challengeExternalResult.RequestToken))
			{
				return false;
			}
			if (this.HasPassed == challengeExternalResult.HasPassed && (!this.HasPassed || this.Passed.Equals(challengeExternalResult.Passed)))
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
			if (this.HasPassed)
			{
				hashCode ^= this.Passed.GetHashCode();
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
			if (this.HasPassed)
			{
				num++;
				num++;
			}
			return num;
		}

		public static ChallengeExternalResult ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ChallengeExternalResult>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ChallengeExternalResult.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ChallengeExternalResult instance)
		{
			if (instance.HasRequestToken)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.RequestToken));
			}
			if (instance.HasPassed)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteBool(stream, instance.Passed);
			}
		}

		public void SetPassed(bool val)
		{
			this.Passed = val;
		}

		public void SetRequestToken(string val)
		{
			this.RequestToken = val;
		}
	}
}