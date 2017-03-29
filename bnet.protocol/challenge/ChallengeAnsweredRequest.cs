using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.challenge
{
	public class ChallengeAnsweredRequest : IProtoBuf
	{
		public bool HasData;

		private byte[] _Data;

		public bool HasId;

		private uint _Id;

		public string Answer
		{
			get;
			set;
		}

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

		public ChallengeAnsweredRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			ChallengeAnsweredRequest.Deserialize(stream, this);
		}

		public static ChallengeAnsweredRequest Deserialize(Stream stream, ChallengeAnsweredRequest instance)
		{
			return ChallengeAnsweredRequest.Deserialize(stream, instance, (long)-1);
		}

		public static ChallengeAnsweredRequest Deserialize(Stream stream, ChallengeAnsweredRequest instance, long limit)
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
							instance.Answer = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 18)
						{
							instance.Data = ProtocolParser.ReadBytes(stream);
						}
						else if (num1 == 24)
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

		public static ChallengeAnsweredRequest DeserializeLengthDelimited(Stream stream)
		{
			ChallengeAnsweredRequest challengeAnsweredRequest = new ChallengeAnsweredRequest();
			ChallengeAnsweredRequest.DeserializeLengthDelimited(stream, challengeAnsweredRequest);
			return challengeAnsweredRequest;
		}

		public static ChallengeAnsweredRequest DeserializeLengthDelimited(Stream stream, ChallengeAnsweredRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return ChallengeAnsweredRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ChallengeAnsweredRequest challengeAnsweredRequest = obj as ChallengeAnsweredRequest;
			if (challengeAnsweredRequest == null)
			{
				return false;
			}
			if (!this.Answer.Equals(challengeAnsweredRequest.Answer))
			{
				return false;
			}
			if (this.HasData != challengeAnsweredRequest.HasData || this.HasData && !this.Data.Equals(challengeAnsweredRequest.Data))
			{
				return false;
			}
			if (this.HasId == challengeAnsweredRequest.HasId && (!this.HasId || this.Id.Equals(challengeAnsweredRequest.Id)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.Answer.GetHashCode();
			if (this.HasData)
			{
				hashCode = hashCode ^ this.Data.GetHashCode();
			}
			if (this.HasId)
			{
				hashCode = hashCode ^ this.Id.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Answer);
			num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			if (this.HasData)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32((int)this.Data.Length) + (int)this.Data.Length;
			}
			if (this.HasId)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.Id);
			}
			num++;
			return num;
		}

		public static ChallengeAnsweredRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ChallengeAnsweredRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ChallengeAnsweredRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ChallengeAnsweredRequest instance)
		{
			if (instance.Answer == null)
			{
				throw new ArgumentNullException("Answer", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Answer));
			if (instance.HasData)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteBytes(stream, instance.Data);
			}
			if (instance.HasId)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.Id);
			}
		}

		public void SetAnswer(string val)
		{
			this.Answer = val;
		}

		public void SetData(byte[] val)
		{
			this.Data = val;
		}

		public void SetId(uint val)
		{
			this.Id = val;
		}
	}
}