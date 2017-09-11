using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.friends
{
	public class UpdateFriendStateNotification : IProtoBuf
	{
		public bool HasGameAccountId;

		private EntityId _GameAccountId;

		public Friend ChangedFriend
		{
			get;
			set;
		}

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

		public UpdateFriendStateNotification()
		{
		}

		public void Deserialize(Stream stream)
		{
			UpdateFriendStateNotification.Deserialize(stream, this);
		}

		public static UpdateFriendStateNotification Deserialize(Stream stream, UpdateFriendStateNotification instance)
		{
			return UpdateFriendStateNotification.Deserialize(stream, instance, (long)-1);
		}

		public static UpdateFriendStateNotification Deserialize(Stream stream, UpdateFriendStateNotification instance, long limit)
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
							if (instance.ChangedFriend != null)
							{
								Friend.DeserializeLengthDelimited(stream, instance.ChangedFriend);
							}
							else
							{
								instance.ChangedFriend = Friend.DeserializeLengthDelimited(stream);
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

		public static UpdateFriendStateNotification DeserializeLengthDelimited(Stream stream)
		{
			UpdateFriendStateNotification updateFriendStateNotification = new UpdateFriendStateNotification();
			UpdateFriendStateNotification.DeserializeLengthDelimited(stream, updateFriendStateNotification);
			return updateFriendStateNotification;
		}

		public static UpdateFriendStateNotification DeserializeLengthDelimited(Stream stream, UpdateFriendStateNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return UpdateFriendStateNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			UpdateFriendStateNotification updateFriendStateNotification = obj as UpdateFriendStateNotification;
			if (updateFriendStateNotification == null)
			{
				return false;
			}
			if (!this.ChangedFriend.Equals(updateFriendStateNotification.ChangedFriend))
			{
				return false;
			}
			if (this.HasGameAccountId == updateFriendStateNotification.HasGameAccountId && (!this.HasGameAccountId || this.GameAccountId.Equals(updateFriendStateNotification.GameAccountId)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.ChangedFriend.GetHashCode();
			if (this.HasGameAccountId)
			{
				hashCode ^= this.GameAccountId.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.ChangedFriend.GetSerializedSize();
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

		public static UpdateFriendStateNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<UpdateFriendStateNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			UpdateFriendStateNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, UpdateFriendStateNotification instance)
		{
			if (instance.ChangedFriend == null)
			{
				throw new ArgumentNullException("ChangedFriend", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.ChangedFriend.GetSerializedSize());
			Friend.Serialize(stream, instance.ChangedFriend);
			if (instance.HasGameAccountId)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.GameAccountId.GetSerializedSize());
				EntityId.Serialize(stream, instance.GameAccountId);
			}
		}

		public void SetChangedFriend(Friend val)
		{
			this.ChangedFriend = val;
		}

		public void SetGameAccountId(EntityId val)
		{
			this.GameAccountId = val;
		}
	}
}