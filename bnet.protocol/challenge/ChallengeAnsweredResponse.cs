using System;
using System.IO;

namespace bnet.protocol.challenge
{
	public class ChallengeAnsweredResponse : IProtoBuf
	{
		public bool HasData;

		private byte[] _Data;

		public bool HasDoRetry;

		private bool _DoRetry;

		public bool HasRecordNotFound;

		private bool _RecordNotFound;

		public byte[] Data
		{
			get
			{
				return this._Data;
			}
			set
			{
				this._Data = value;
				this.HasData = value != null;
			}
		}

		public bool DoRetry
		{
			get
			{
				return this._DoRetry;
			}
			set
			{
				this._DoRetry = value;
				this.HasDoRetry = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public bool RecordNotFound
		{
			get
			{
				return this._RecordNotFound;
			}
			set
			{
				this._RecordNotFound = value;
				this.HasRecordNotFound = true;
			}
		}

		public ChallengeAnsweredResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			ChallengeAnsweredResponse.Deserialize(stream, this);
		}

		public static ChallengeAnsweredResponse Deserialize(Stream stream, ChallengeAnsweredResponse instance)
		{
			return ChallengeAnsweredResponse.Deserialize(stream, instance, (long)-1);
		}

		public static ChallengeAnsweredResponse Deserialize(Stream stream, ChallengeAnsweredResponse instance, long limit)
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
						instance.Data = ProtocolParser.ReadBytes(stream);
					}
					else if (num == 16)
					{
						instance.DoRetry = ProtocolParser.ReadBool(stream);
					}
					else if (num == 24)
					{
						instance.RecordNotFound = ProtocolParser.ReadBool(stream);
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

		public static ChallengeAnsweredResponse DeserializeLengthDelimited(Stream stream)
		{
			ChallengeAnsweredResponse challengeAnsweredResponse = new ChallengeAnsweredResponse();
			ChallengeAnsweredResponse.DeserializeLengthDelimited(stream, challengeAnsweredResponse);
			return challengeAnsweredResponse;
		}

		public static ChallengeAnsweredResponse DeserializeLengthDelimited(Stream stream, ChallengeAnsweredResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ChallengeAnsweredResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ChallengeAnsweredResponse challengeAnsweredResponse = obj as ChallengeAnsweredResponse;
			if (challengeAnsweredResponse == null)
			{
				return false;
			}
			if (this.HasData != challengeAnsweredResponse.HasData || this.HasData && !this.Data.Equals(challengeAnsweredResponse.Data))
			{
				return false;
			}
			if (this.HasDoRetry != challengeAnsweredResponse.HasDoRetry || this.HasDoRetry && !this.DoRetry.Equals(challengeAnsweredResponse.DoRetry))
			{
				return false;
			}
			if (this.HasRecordNotFound == challengeAnsweredResponse.HasRecordNotFound && (!this.HasRecordNotFound || this.RecordNotFound.Equals(challengeAnsweredResponse.RecordNotFound)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasData)
			{
				hashCode ^= this.Data.GetHashCode();
			}
			if (this.HasDoRetry)
			{
				hashCode ^= this.DoRetry.GetHashCode();
			}
			if (this.HasRecordNotFound)
			{
				hashCode ^= this.RecordNotFound.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasData)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32((int)this.Data.Length) + (int)this.Data.Length;
			}
			if (this.HasDoRetry)
			{
				num++;
				num++;
			}
			if (this.HasRecordNotFound)
			{
				num++;
				num++;
			}
			return num;
		}

		public static ChallengeAnsweredResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ChallengeAnsweredResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ChallengeAnsweredResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ChallengeAnsweredResponse instance)
		{
			if (instance.HasData)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteBytes(stream, instance.Data);
			}
			if (instance.HasDoRetry)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteBool(stream, instance.DoRetry);
			}
			if (instance.HasRecordNotFound)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteBool(stream, instance.RecordNotFound);
			}
		}

		public void SetData(byte[] val)
		{
			this.Data = val;
		}

		public void SetDoRetry(bool val)
		{
			this.DoRetry = val;
		}

		public void SetRecordNotFound(bool val)
		{
			this.RecordNotFound = val;
		}
	}
}