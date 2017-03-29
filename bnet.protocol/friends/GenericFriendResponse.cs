using System;
using System.IO;

namespace bnet.protocol.friends
{
	public class GenericFriendResponse : IProtoBuf
	{
		public bool HasTargetFriend;

		private Friend _TargetFriend;

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public Friend TargetFriend
		{
			get
			{
				return this._TargetFriend;
			}
			set
			{
				this._TargetFriend = value;
				this.HasTargetFriend = value != null;
			}
		}

		public GenericFriendResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			GenericFriendResponse.Deserialize(stream, this);
		}

		public static GenericFriendResponse Deserialize(Stream stream, GenericFriendResponse instance)
		{
			return GenericFriendResponse.Deserialize(stream, instance, (long)-1);
		}

		public static GenericFriendResponse Deserialize(Stream stream, GenericFriendResponse instance, long limit)
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
					else if (instance.TargetFriend != null)
					{
						Friend.DeserializeLengthDelimited(stream, instance.TargetFriend);
					}
					else
					{
						instance.TargetFriend = Friend.DeserializeLengthDelimited(stream);
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

		public static GenericFriendResponse DeserializeLengthDelimited(Stream stream)
		{
			GenericFriendResponse genericFriendResponse = new GenericFriendResponse();
			GenericFriendResponse.DeserializeLengthDelimited(stream, genericFriendResponse);
			return genericFriendResponse;
		}

		public static GenericFriendResponse DeserializeLengthDelimited(Stream stream, GenericFriendResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GenericFriendResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GenericFriendResponse genericFriendResponse = obj as GenericFriendResponse;
			if (genericFriendResponse == null)
			{
				return false;
			}
			if (this.HasTargetFriend == genericFriendResponse.HasTargetFriend && (!this.HasTargetFriend || this.TargetFriend.Equals(genericFriendResponse.TargetFriend)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasTargetFriend)
			{
				hashCode = hashCode ^ this.TargetFriend.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasTargetFriend)
			{
				num++;
				uint serializedSize = this.TargetFriend.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			return num;
		}

		public static GenericFriendResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GenericFriendResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GenericFriendResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GenericFriendResponse instance)
		{
			if (instance.HasTargetFriend)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.TargetFriend.GetSerializedSize());
				Friend.Serialize(stream, instance.TargetFriend);
			}
		}

		public void SetTargetFriend(Friend val)
		{
			this.TargetFriend = val;
		}
	}
}