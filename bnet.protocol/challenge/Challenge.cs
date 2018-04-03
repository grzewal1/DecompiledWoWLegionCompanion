using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.challenge
{
	public class Challenge : IProtoBuf
	{
		public bool HasInfo;

		private string _Info;

		public bool HasAnswer;

		private string _Answer;

		public bool HasRetries;

		private uint _Retries;

		public string Answer
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

		public string Info
		{
			get
			{
				return this._Info;
			}
			set
			{
				this._Info = value;
				this.HasInfo = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public uint Retries
		{
			get
			{
				return this._Retries;
			}
			set
			{
				this._Retries = value;
				this.HasRetries = true;
			}
		}

		public uint Type
		{
			get;
			set;
		}

		public Challenge()
		{
		}

		public void Deserialize(Stream stream)
		{
			Challenge.Deserialize(stream, this);
		}

		public static Challenge Deserialize(Stream stream, Challenge instance)
		{
			return Challenge.Deserialize(stream, instance, (long)-1);
		}

		public static Challenge Deserialize(Stream stream, Challenge instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
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
						instance.Type = binaryReader.ReadUInt32();
					}
					else if (num == 18)
					{
						instance.Info = ProtocolParser.ReadString(stream);
					}
					else if (num == 26)
					{
						instance.Answer = ProtocolParser.ReadString(stream);
					}
					else if (num == 32)
					{
						instance.Retries = ProtocolParser.ReadUInt32(stream);
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

		public static Challenge DeserializeLengthDelimited(Stream stream)
		{
			Challenge challenge = new Challenge();
			Challenge.DeserializeLengthDelimited(stream, challenge);
			return challenge;
		}

		public static Challenge DeserializeLengthDelimited(Stream stream, Challenge instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return Challenge.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			Challenge challenge = obj as Challenge;
			if (challenge == null)
			{
				return false;
			}
			if (!this.Type.Equals(challenge.Type))
			{
				return false;
			}
			if (this.HasInfo != challenge.HasInfo || this.HasInfo && !this.Info.Equals(challenge.Info))
			{
				return false;
			}
			if (this.HasAnswer != challenge.HasAnswer || this.HasAnswer && !this.Answer.Equals(challenge.Answer))
			{
				return false;
			}
			if (this.HasRetries == challenge.HasRetries && (!this.HasRetries || this.Retries.Equals(challenge.Retries)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Type.GetHashCode();
			if (this.HasInfo)
			{
				hashCode ^= this.Info.GetHashCode();
			}
			if (this.HasAnswer)
			{
				hashCode ^= this.Answer.GetHashCode();
			}
			if (this.HasRetries)
			{
				hashCode ^= this.Retries.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num += 4;
			if (this.HasInfo)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Info);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasAnswer)
			{
				num++;
				uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.Answer);
				num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			}
			if (this.HasRetries)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.Retries);
			}
			num++;
			return num;
		}

		public static Challenge ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<Challenge>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			Challenge.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, Challenge instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			stream.WriteByte(13);
			binaryWriter.Write(instance.Type);
			if (instance.HasInfo)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Info));
			}
			if (instance.HasAnswer)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Answer));
			}
			if (instance.HasRetries)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteUInt32(stream, instance.Retries);
			}
		}

		public void SetAnswer(string val)
		{
			this.Answer = val;
		}

		public void SetInfo(string val)
		{
			this.Info = val;
		}

		public void SetRetries(uint val)
		{
			this.Retries = val;
		}

		public void SetType(uint val)
		{
			this.Type = val;
		}
	}
}