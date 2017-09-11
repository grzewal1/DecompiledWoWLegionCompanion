using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.friends
{
	public class FriendNotification : IProtoBuf
	{
		public bool HasGameAccountId;

		private EntityId _GameAccountId;

		public EntityId GameAccountId
		{
			get
			{
				return this._GameAccountId;
			}
			set
			{
				this._GameAccountId = value;
				this.HasGameAccountId = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public Friend Target
		{
			get;
			set;
		}

		public FriendNotification()
		{
		}

		public void Deserialize(Stream stream)
		{
			FriendNotification.Deserialize(stream, this);
		}

		public static FriendNotification Deserialize(Stream stream, FriendNotification instance)
		{
			return FriendNotification.Deserialize(stream, instance, (long)-1);
		}

		public static FriendNotification Deserialize(Stream stream, FriendNotification instance, long limit)
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
							if (instance.Target != null)
							{
								Friend.DeserializeLengthDelimited(stream, instance.Target);
							}
							else
							{
								instance.Target = Friend.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 != 18)
						{
							Key key = ProtocolParser.ReadKey((byte)num, stream);
							if (key.Field == 0)
							{
								throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
							}
							ProtocolParser.SkipKey(stream, key);
						}
						else if (instance.GameAccountId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.GameAccountId);
						}
						else
						{
							instance.GameAccountId = EntityId.DeserializeLengthDelimited(stream);
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

		public static FriendNotification DeserializeLengthDelimited(Stream stream)
		{
			FriendNotification friendNotification = new FriendNotification();
			FriendNotification.DeserializeLengthDelimited(stream, friendNotification);
			return friendNotification;
		}

		public static FriendNotification DeserializeLengthDelimited(Stream stream, FriendNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return FriendNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			FriendNotification friendNotification = obj as FriendNotification;
			if (friendNotification == null)
			{
				return false;
			}
			if (!this.Target.Equals(friendNotification.Target))
			{
				return false;
			}
			if (this.HasGameAccountId == friendNotification.HasGameAccountId && (!this.HasGameAccountId || this.GameAccountId.Equals(friendNotification.GameAccountId)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Target.GetHashCode();
			if (this.HasGameAccountId)
			{
				hashCode ^= this.GameAccountId.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.Target.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasGameAccountId)
			{
				num++;
				uint serializedSize1 = this.GameAccountId.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			num++;
			return num;
		}

		public static FriendNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<FriendNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			FriendNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, FriendNotification instance)
		{
			if (instance.Target == null)
			{
				throw new ArgumentNullException("Target", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.Target.GetSerializedSize());
			Friend.Serialize(stream, instance.Target);
			if (instance.HasGameAccountId)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.GameAccountId.GetSerializedSize());
				EntityId.Serialize(stream, instance.GameAccountId);
			}
		}

		public void SetGameAccountId(EntityId val)
		{
			this.GameAccountId = val;
		}

		public void SetTarget(Friend val)
		{
			this.Target = val;
		}
	}
}