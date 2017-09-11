using System;
using System.IO;

namespace bnet.protocol.challenge
{
	public class SendChallengeToUserResponse : IProtoBuf
	{
		public bool HasId;

		private uint _Id;

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

		public SendChallengeToUserResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			SendChallengeToUserResponse.Deserialize(stream, this);
		}

		public static SendChallengeToUserResponse Deserialize(Stream stream, SendChallengeToUserResponse instance)
		{
			return SendChallengeToUserResponse.Deserialize(stream, instance, (long)-1);
		}

		public static SendChallengeToUserResponse Deserialize(Stream stream, SendChallengeToUserResponse instance, long limit)
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
					else if (num == 8)
					{
						instance.Id = ProtocolParser.ReadUInt32(stream);
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

		public static SendChallengeToUserResponse DeserializeLengthDelimited(Stream stream)
		{
			SendChallengeToUserResponse sendChallengeToUserResponse = new SendChallengeToUserResponse();
			SendChallengeToUserResponse.DeserializeLengthDelimited(stream, sendChallengeToUserResponse);
			return sendChallengeToUserResponse;
		}

		public static SendChallengeToUserResponse DeserializeLengthDelimited(Stream stream, SendChallengeToUserResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return SendChallengeToUserResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			SendChallengeToUserResponse sendChallengeToUserResponse = obj as SendChallengeToUserResponse;
			if (sendChallengeToUserResponse == null)
			{
				return false;
			}
			if (this.HasId == sendChallengeToUserResponse.HasId && (!this.HasId || this.Id.Equals(sendChallengeToUserResponse.Id)))
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
			return num;
		}

		public static SendChallengeToUserResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<SendChallengeToUserResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			SendChallengeToUserResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, SendChallengeToUserResponse instance)
		{
			if (instance.HasId)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.Id);
			}
		}

		public void SetId(uint val)
		{
			this.Id = val;
		}
	}
}