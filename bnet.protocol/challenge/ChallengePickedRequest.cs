using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.challenge
{
	public class ChallengePickedRequest : IProtoBuf
	{
		public bool HasId;

		private uint _Id;

		public bool HasNewChallengeProtocol;

		private bool _NewChallengeProtocol;

		public uint Challenge
		{
			get;
			set;
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

		public bool NewChallengeProtocol
		{
			get
			{
				return this._NewChallengeProtocol;
			}
			set
			{
				this._NewChallengeProtocol = value;
				this.HasNewChallengeProtocol = true;
			}
		}

		public ChallengePickedRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			ChallengePickedRequest.Deserialize(stream, this);
		}

		public static ChallengePickedRequest Deserialize(Stream stream, ChallengePickedRequest instance)
		{
			return ChallengePickedRequest.Deserialize(stream, instance, (long)-1);
		}

		public static ChallengePickedRequest Deserialize(Stream stream, ChallengePickedRequest instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			instance.NewChallengeProtocol = false;
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
						instance.Challenge = binaryReader.ReadUInt32();
					}
					else if (num == 16)
					{
						instance.Id = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 24)
					{
						instance.NewChallengeProtocol = ProtocolParser.ReadBool(stream);
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

		public static ChallengePickedRequest DeserializeLengthDelimited(Stream stream)
		{
			ChallengePickedRequest challengePickedRequest = new ChallengePickedRequest();
			ChallengePickedRequest.DeserializeLengthDelimited(stream, challengePickedRequest);
			return challengePickedRequest;
		}

		public static ChallengePickedRequest DeserializeLengthDelimited(Stream stream, ChallengePickedRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ChallengePickedRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ChallengePickedRequest challengePickedRequest = obj as ChallengePickedRequest;
			if (challengePickedRequest == null)
			{
				return false;
			}
			if (!this.Challenge.Equals(challengePickedRequest.Challenge))
			{
				return false;
			}
			if (this.HasId != challengePickedRequest.HasId || this.HasId && !this.Id.Equals(challengePickedRequest.Id))
			{
				return false;
			}
			if (this.HasNewChallengeProtocol == challengePickedRequest.HasNewChallengeProtocol && (!this.HasNewChallengeProtocol || this.NewChallengeProtocol.Equals(challengePickedRequest.NewChallengeProtocol)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Challenge.GetHashCode();
			if (this.HasId)
			{
				hashCode ^= this.Id.GetHashCode();
			}
			if (this.HasNewChallengeProtocol)
			{
				hashCode ^= this.NewChallengeProtocol.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num += 4;
			if (this.HasId)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.Id);
			}
			if (this.HasNewChallengeProtocol)
			{
				num++;
				num++;
			}
			num++;
			return num;
		}

		public static ChallengePickedRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ChallengePickedRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ChallengePickedRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ChallengePickedRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			stream.WriteByte(13);
			binaryWriter.Write(instance.Challenge);
			if (instance.HasId)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.Id);
			}
			if (instance.HasNewChallengeProtocol)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteBool(stream, instance.NewChallengeProtocol);
			}
		}

		public void SetChallenge(uint val)
		{
			this.Challenge = val;
		}

		public void SetId(uint val)
		{
			this.Id = val;
		}

		public void SetNewChallengeProtocol(bool val)
		{
			this.NewChallengeProtocol = val;
		}
	}
}