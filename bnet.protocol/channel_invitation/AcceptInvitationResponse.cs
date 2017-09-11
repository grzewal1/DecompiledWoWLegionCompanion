using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel_invitation
{
	public class AcceptInvitationResponse : IProtoBuf
	{
		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public ulong ObjectId
		{
			get;
			set;
		}

		public AcceptInvitationResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			AcceptInvitationResponse.Deserialize(stream, this);
		}

		public static AcceptInvitationResponse Deserialize(Stream stream, AcceptInvitationResponse instance)
		{
			return AcceptInvitationResponse.Deserialize(stream, instance, (long)-1);
		}

		public static AcceptInvitationResponse Deserialize(Stream stream, AcceptInvitationResponse instance, long limit)
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
						instance.ObjectId = ProtocolParser.ReadUInt64(stream);
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

		public static AcceptInvitationResponse DeserializeLengthDelimited(Stream stream)
		{
			AcceptInvitationResponse acceptInvitationResponse = new AcceptInvitationResponse();
			AcceptInvitationResponse.DeserializeLengthDelimited(stream, acceptInvitationResponse);
			return acceptInvitationResponse;
		}

		public static AcceptInvitationResponse DeserializeLengthDelimited(Stream stream, AcceptInvitationResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return AcceptInvitationResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			AcceptInvitationResponse acceptInvitationResponse = obj as AcceptInvitationResponse;
			if (acceptInvitationResponse == null)
			{
				return false;
			}
			if (!this.ObjectId.Equals(acceptInvitationResponse.ObjectId))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			return hashCode ^ this.ObjectId.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0 + ProtocolParser.SizeOfUInt64(this.ObjectId);
			return num + 1;
		}

		public static AcceptInvitationResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<AcceptInvitationResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			AcceptInvitationResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, AcceptInvitationResponse instance)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, instance.ObjectId);
		}

		public void SetObjectId(ulong val)
		{
			this.ObjectId = val;
		}
	}
}