using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel
{
	public class Member : IProtoBuf
	{
		public bnet.protocol.Identity Identity
		{
			get;
			set;
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public MemberState State
		{
			get;
			set;
		}

		public Member()
		{
		}

		public void Deserialize(Stream stream)
		{
			Member.Deserialize(stream, this);
		}

		public static Member Deserialize(Stream stream, Member instance)
		{
			return Member.Deserialize(stream, instance, (long)-1);
		}

		public static Member Deserialize(Stream stream, Member instance, long limit)
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
						if (instance.Identity != null)
						{
							bnet.protocol.Identity.DeserializeLengthDelimited(stream, instance.Identity);
						}
						else
						{
							instance.Identity = bnet.protocol.Identity.DeserializeLengthDelimited(stream);
						}
					}
					else if (num != 18)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.State != null)
					{
						MemberState.DeserializeLengthDelimited(stream, instance.State);
					}
					else
					{
						instance.State = MemberState.DeserializeLengthDelimited(stream);
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

		public static Member DeserializeLengthDelimited(Stream stream)
		{
			Member member = new Member();
			Member.DeserializeLengthDelimited(stream, member);
			return member;
		}

		public static Member DeserializeLengthDelimited(Stream stream, Member instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return Member.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			Member member = obj as Member;
			if (member == null)
			{
				return false;
			}
			if (!this.Identity.Equals(member.Identity))
			{
				return false;
			}
			if (!this.State.Equals(member.State))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Identity.GetHashCode();
			return hashCode ^ this.State.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.Identity.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			uint serializedSize1 = this.State.GetSerializedSize();
			num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			return num + 2;
		}

		public static Member ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<Member>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			Member.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, Member instance)
		{
			if (instance.Identity == null)
			{
				throw new ArgumentNullException("Identity", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.Identity.GetSerializedSize());
			bnet.protocol.Identity.Serialize(stream, instance.Identity);
			if (instance.State == null)
			{
				throw new ArgumentNullException("State", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.State.GetSerializedSize());
			MemberState.Serialize(stream, instance.State);
		}

		public void SetIdentity(bnet.protocol.Identity val)
		{
			this.Identity = val;
		}

		public void SetState(MemberState val)
		{
			this.State = val;
		}
	}
}