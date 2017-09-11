using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel
{
	public class JoinNotification : IProtoBuf
	{
		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public bnet.protocol.channel.Member Member
		{
			get;
			set;
		}

		public JoinNotification()
		{
		}

		public void Deserialize(Stream stream)
		{
			JoinNotification.Deserialize(stream, this);
		}

		public static JoinNotification Deserialize(Stream stream, JoinNotification instance)
		{
			return JoinNotification.Deserialize(stream, instance, (long)-1);
		}

		public static JoinNotification Deserialize(Stream stream, JoinNotification instance, long limit)
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
					else if (num != 10)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.Member != null)
					{
						bnet.protocol.channel.Member.DeserializeLengthDelimited(stream, instance.Member);
					}
					else
					{
						instance.Member = bnet.protocol.channel.Member.DeserializeLengthDelimited(stream);
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

		public static JoinNotification DeserializeLengthDelimited(Stream stream)
		{
			JoinNotification joinNotification = new JoinNotification();
			JoinNotification.DeserializeLengthDelimited(stream, joinNotification);
			return joinNotification;
		}

		public static JoinNotification DeserializeLengthDelimited(Stream stream, JoinNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return JoinNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			JoinNotification joinNotification = obj as JoinNotification;
			if (joinNotification == null)
			{
				return false;
			}
			if (!this.Member.Equals(joinNotification.Member))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			return hashCode ^ this.Member.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.Member.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			return num + 1;
		}

		public static JoinNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<JoinNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			JoinNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, JoinNotification instance)
		{
			if (instance.Member == null)
			{
				throw new ArgumentNullException("Member", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.Member.GetSerializedSize());
			bnet.protocol.channel.Member.Serialize(stream, instance.Member);
		}

		public void SetMember(bnet.protocol.channel.Member val)
		{
			this.Member = val;
		}
	}
}