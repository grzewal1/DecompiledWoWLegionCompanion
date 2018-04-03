using System;
using System.IO;

namespace bnet.protocol.challenge
{
	public class ChallengeResultRequest : IProtoBuf
	{
		public bool HasId;

		private uint _Id;

		public bool HasType;

		private uint _Type;

		public bool HasErrorId;

		private uint _ErrorId;

		public bool HasAnswer;

		private byte[] _Answer;

		public byte[] Answer
		{
			get
			{
				return this._Answer;
			}
			set
			{
				this._Answer = value;
				this.HasAnswer = value != null;
			}
		}

		public uint ErrorId
		{
			get
			{
				return this._ErrorId;
			}
			set
			{
				this._ErrorId = value;
				this.HasErrorId = true;
			}
		}

		public uint Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				this._Id = value;
				this.HasId = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public uint Type
		{
			get
			{
				return this._Type;
			}
			set
			{
				this._Type = value;
				this.HasType = true;
			}
		}

		public ChallengeResultRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			ChallengeResultRequest.Deserialize(stream, this);
		}

		public static ChallengeResultRequest Deserialize(Stream stream, ChallengeResultRequest instance)
		{
			return ChallengeResultRequest.Deserialize(stream, instance, (long)-1);
		}

		public static ChallengeResultRequest Deserialize(Stream stream, ChallengeResultRequest instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						switch (num)
						{
							case 21:
							{
								instance.Type = binaryReader.ReadUInt32();
								continue;
							}
							case 24:
							{
								instance.ErrorId = ProtocolParser.ReadUInt32(stream);
								continue;
							}
							default:
							{
								if (num == 8)
								{
									instance.Id = ProtocolParser.ReadUInt32(stream);
									continue;
								}
								else if (num == 34)
								{
									instance.Answer = ProtocolParser.ReadBytes(stream);
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

		public static ChallengeResultRequest DeserializeLengthDelimited(Stream stream)
		{
			ChallengeResultRequest challengeResultRequest = new ChallengeResultRequest();
			ChallengeResultRequest.DeserializeLengthDelimited(stream, challengeResultRequest);
			return challengeResultRequest;
		}

		public static ChallengeResultRequest DeserializeLengthDelimited(Stream stream, ChallengeResultRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ChallengeResultRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ChallengeResultRequest challengeResultRequest = obj as ChallengeResultRequest;
			if (challengeResultRequest == null)
			{
				return false;
			}
			if (this.HasId != challengeResultRequest.HasId || this.HasId && !this.Id.Equals(challengeResultRequest.Id))
			{
				return false;
			}
			if (this.HasType != challengeResultRequest.HasType || this.HasType && !this.Type.Equals(challengeResultRequest.Type))
			{
				return false;
			}
			if (this.HasErrorId != challengeResultRequest.HasErrorId || this.HasErrorId && !this.ErrorId.Equals(challengeResultRequest.ErrorId))
			{
				return false;
			}
			if (this.HasAnswer == challengeResultRequest.HasAnswer && (!this.HasAnswer || this.Answer.Equals(challengeResultRequest.Answer)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasId)
			{
				hashCode ^= this.Id.GetHashCode();
			}
			if (this.HasType)
			{
				hashCode ^= this.Type.GetHashCode();
			}
			if (this.HasErrorId)
			{
				hashCode ^= this.ErrorId.GetHashCode();
			}
			if (this.HasAnswer)
			{
				hashCode ^= this.Answer.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasId)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.Id);
			}
			if (this.HasType)
			{
				num++;
				num += 4;
			}
			if (this.HasErrorId)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.ErrorId);
			}
			if (this.HasAnswer)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32((int)this.Answer.Length) + (int)this.Answer.Length;
			}
			return num;
		}

		public static ChallengeResultRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ChallengeResultRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ChallengeResultRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ChallengeResultRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasId)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.Id);
			}
			if (instance.HasType)
			{
				stream.WriteByte(21);
				binaryWriter.Write(instance.Type);
			}
			if (instance.HasErrorId)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.ErrorId);
			}
			if (instance.HasAnswer)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteBytes(stream, instance.Answer);
			}
		}

		public void SetAnswer(byte[] val)
		{
			this.Answer = val;
		}

		public void SetErrorId(uint val)
		{
			this.ErrorId = val;
		}

		public void SetId(uint val)
		{
			this.Id = val;
		}

		public void SetType(uint val)
		{
			this.Type = val;
		}
	}
}