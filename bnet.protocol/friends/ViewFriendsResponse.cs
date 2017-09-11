using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.friends
{
	public class ViewFriendsResponse : IProtoBuf
	{
		private List<Friend> _Friends = new List<Friend>();

		public List<Friend> Friends
		{
			get
			{
				return this._Friends;
			}
			set
			{
				this._Friends = value;
			}
		}

		public int FriendsCount
		{
			get
			{
				return this._Friends.Count;
			}
		}

		public List<Friend> FriendsList
		{
			get
			{
				return this._Friends;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public ViewFriendsResponse()
		{
		}

		public void AddFriends(Friend val)
		{
			this._Friends.Add(val);
		}

		public void ClearFriends()
		{
			this._Friends.Clear();
		}

		public void Deserialize(Stream stream)
		{
			ViewFriendsResponse.Deserialize(stream, this);
		}

		public static ViewFriendsResponse Deserialize(Stream stream, ViewFriendsResponse instance)
		{
			return ViewFriendsResponse.Deserialize(stream, instance, (long)-1);
		}

		public static ViewFriendsResponse Deserialize(Stream stream, ViewFriendsResponse instance, long limit)
		{
			if (instance.Friends == null)
			{
				instance.Friends = new List<Friend>();
			}
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
						instance.Friends.Add(Friend.DeserializeLengthDelimited(stream));
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

		public static ViewFriendsResponse DeserializeLengthDelimited(Stream stream)
		{
			ViewFriendsResponse viewFriendsResponse = new ViewFriendsResponse();
			ViewFriendsResponse.DeserializeLengthDelimited(stream, viewFriendsResponse);
			return viewFriendsResponse;
		}

		public static ViewFriendsResponse DeserializeLengthDelimited(Stream stream, ViewFriendsResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ViewFriendsResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ViewFriendsResponse viewFriendsResponse = obj as ViewFriendsResponse;
			if (viewFriendsResponse == null)
			{
				return false;
			}
			if (this.Friends.Count != viewFriendsResponse.Friends.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Friends.Count; i++)
			{
				if (!this.Friends[i].Equals(viewFriendsResponse.Friends[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (Friend friend in this.Friends)
			{
				hashCode ^= friend.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Friends.Count > 0)
			{
				foreach (Friend friend in this.Friends)
				{
					num++;
					uint serializedSize = friend.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			return num;
		}

		public static ViewFriendsResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ViewFriendsResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ViewFriendsResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ViewFriendsResponse instance)
		{
			if (instance.Friends.Count > 0)
			{
				foreach (Friend friend in instance.Friends)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, friend.GetSerializedSize());
					Friend.Serialize(stream, friend);
				}
			}
		}

		public void SetFriends(List<Friend> val)
		{
			this.Friends = val;
		}
	}
}