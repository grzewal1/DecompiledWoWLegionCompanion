using System;
using System.IO;

namespace bnet.protocol.challenge
{
	public class ChallengeCancelledRequest : IProtoBuf
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

		public ChallengeCancelledRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			ChallengeCancelledRequest.Deserialize(stream, this);
		}

		public static ChallengeCancelledRequest Deserialize(Stream stream, ChallengeCancelledRequest instance)
		{
			return ChallengeCancelledRequest.Deserialize(stream, instance, (long)-1);
		}

		public static ChallengeCancelledRequest Deserialize(Stream stream, ChallengeCancelledRequest instance, long limit)
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

		public static ChallengeCancelledRequest DeserializeLengthDelimited(Stream stream)
		{
			ChallengeCancelledRequest challengeCancelledRequest = new ChallengeCancelledRequest();
			ChallengeCancelledRequest.DeserializeLengthDelimited(stream, challengeCancelledRequest);
			return challengeCancelledRequest;
		}

		public static ChallengeCancelledRequest DeserializeLengthDelimited(Stream stream, ChallengeCancelledRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ChallengeCancelledRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ChallengeCancelledRequest challengeCancelledRequest = obj as ChallengeCancelledRequest;
			if (challengeCancelledRequest == null)
			{
				return false;
			}
			if (this.HasId == challengeCancelledRequest.HasId && (!this.HasId || this.Id.Equals(challengeCancelledRequest.Id)))
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

		public static ChallengeCancelledRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ChallengeCancelledRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ChallengeCancelledRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ChallengeCancelledRequest instance)
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