using System;
using System.IO;

namespace bnet.protocol.challenge
{
	public class ChallengePickedResponse : IProtoBuf
	{
		public bool HasData;

		private byte[] _Data;

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

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public ChallengePickedResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			ChallengePickedResponse.Deserialize(stream, this);
		}

		public static ChallengePickedResponse Deserialize(Stream stream, ChallengePickedResponse instance)
		{
			return ChallengePickedResponse.Deserialize(stream, instance, (long)-1);
		}

		public static ChallengePickedResponse Deserialize(Stream stream, ChallengePickedResponse instance, long limit)
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

		public static ChallengePickedResponse DeserializeLengthDelimited(Stream stream)
		{
			ChallengePickedResponse challengePickedResponse = new ChallengePickedResponse();
			ChallengePickedResponse.DeserializeLengthDelimited(stream, challengePickedResponse);
			return challengePickedResponse;
		}

		public static ChallengePickedResponse DeserializeLengthDelimited(Stream stream, ChallengePickedResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return ChallengePickedResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ChallengePickedResponse challengePickedResponse = obj as ChallengePickedResponse;
			if (challengePickedResponse == null)
			{
				return false;
			}
			if (this.HasData == challengePickedResponse.HasData && (!this.HasData || this.Data.Equals(challengePickedResponse.Data)))
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
				hashCode = hashCode ^ this.Data.GetHashCode();
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
			return num;
		}

		public static ChallengePickedResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ChallengePickedResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ChallengePickedResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ChallengePickedResponse instance)
		{
			if (instance.HasData)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteBytes(stream, instance.Data);
			}
		}

		public void SetData(byte[] val)
		{
			this.Data = val;
		}
	}
}