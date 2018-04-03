using bnet.protocol;
using bnet.protocol.invitation;
using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.friends
{
	public class SubscribeToFriendsResponse : IProtoBuf
	{
		public bool HasMaxFriends;

		private uint _MaxFriends;

		public bool HasMaxReceivedInvitations;

		private uint _MaxReceivedInvitations;

		public bool HasMaxSentInvitations;

		private uint _MaxSentInvitations;

		private List<bnet.protocol.Role> _Role = new List<bnet.protocol.Role>();

		private List<Friend> _Friends = new List<Friend>();

		private List<Invitation> _SentInvitations = new List<Invitation>();

		private List<Invitation> _ReceivedInvitations = new List<Invitation>();

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

		public uint MaxFriends
		{
			get
			{
				return this._MaxFriends;
			}
			set
			{
				this._MaxFriends = value;
				this.HasMaxFriends = true;
			}
		}

		public uint MaxReceivedInvitations
		{
			get
			{
				return this._MaxReceivedInvitations;
			}
			set
			{
				this._MaxReceivedInvitations = value;
				this.HasMaxReceivedInvitations = true;
			}
		}

		public uint MaxSentInvitations
		{
			get
			{
				return this._MaxSentInvitations;
			}
			set
			{
				this._MaxSentInvitations = value;
				this.HasMaxSentInvitations = true;
			}
		}

		public List<Invitation> ReceivedInvitations
		{
			get
			{
				return this._ReceivedInvitations;
			}
			set
			{
				this._ReceivedInvitations = value;
			}
		}

		public int ReceivedInvitationsCount
		{
			get
			{
				return this._ReceivedInvitations.Count;
			}
		}

		public List<Invitation> ReceivedInvitationsList
		{
			get
			{
				return this._ReceivedInvitations;
			}
		}

		public List<bnet.protocol.Role> Role
		{
			get
			{
				return this._Role;
			}
			set
			{
				this._Role = value;
			}
		}

		public int RoleCount
		{
			get
			{
				return this._Role.Count;
			}
		}

		public List<bnet.protocol.Role> RoleList
		{
			get
			{
				return this._Role;
			}
		}

		public List<Invitation> SentInvitations
		{
			get
			{
				return this._SentInvitations;
			}
			set
			{
				this._SentInvitations = value;
			}
		}

		public int SentInvitationsCount
		{
			get
			{
				return this._SentInvitations.Count;
			}
		}

		public List<Invitation> SentInvitationsList
		{
			get
			{
				return this._SentInvitations;
			}
		}

		public SubscribeToFriendsResponse()
		{
		}

		public void AddFriends(Friend val)
		{
			this._Friends.Add(val);
		}

		public void AddReceivedInvitations(Invitation val)
		{
			this._ReceivedInvitations.Add(val);
		}

		public void AddRole(bnet.protocol.Role val)
		{
			this._Role.Add(val);
		}

		public void AddSentInvitations(Invitation val)
		{
			this._SentInvitations.Add(val);
		}

		public void ClearFriends()
		{
			this._Friends.Clear();
		}

		public void ClearReceivedInvitations()
		{
			this._ReceivedInvitations.Clear();
		}

		public void ClearRole()
		{
			this._Role.Clear();
		}

		public void ClearSentInvitations()
		{
			this._SentInvitations.Clear();
		}

		public void Deserialize(Stream stream)
		{
			SubscribeToFriendsResponse.Deserialize(stream, this);
		}

		public static SubscribeToFriendsResponse Deserialize(Stream stream, SubscribeToFriendsResponse instance)
		{
			return SubscribeToFriendsResponse.Deserialize(stream, instance, (long)-1);
		}

		public static SubscribeToFriendsResponse Deserialize(Stream stream, SubscribeToFriendsResponse instance, long limit)
		{
			if (instance.Role == null)
			{
				instance.Role = new List<bnet.protocol.Role>();
			}
			if (instance.Friends == null)
			{
				instance.Friends = new List<Friend>();
			}
			if (instance.SentInvitations == null)
			{
				instance.SentInvitations = new List<Invitation>();
			}
			if (instance.ReceivedInvitations == null)
			{
				instance.ReceivedInvitations = new List<Invitation>();
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
					else if (num == 8)
					{
						instance.MaxFriends = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 16)
					{
						instance.MaxReceivedInvitations = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 24)
					{
						instance.MaxSentInvitations = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 34)
					{
						instance.Role.Add(bnet.protocol.Role.DeserializeLengthDelimited(stream));
					}
					else if (num == 42)
					{
						instance.Friends.Add(Friend.DeserializeLengthDelimited(stream));
					}
					else if (num == 50)
					{
						instance.SentInvitations.Add(Invitation.DeserializeLengthDelimited(stream));
					}
					else if (num == 58)
					{
						instance.ReceivedInvitations.Add(Invitation.DeserializeLengthDelimited(stream));
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

		public static SubscribeToFriendsResponse DeserializeLengthDelimited(Stream stream)
		{
			SubscribeToFriendsResponse subscribeToFriendsResponse = new SubscribeToFriendsResponse();
			SubscribeToFriendsResponse.DeserializeLengthDelimited(stream, subscribeToFriendsResponse);
			return subscribeToFriendsResponse;
		}

		public static SubscribeToFriendsResponse DeserializeLengthDelimited(Stream stream, SubscribeToFriendsResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return SubscribeToFriendsResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			SubscribeToFriendsResponse subscribeToFriendsResponse = obj as SubscribeToFriendsResponse;
			if (subscribeToFriendsResponse == null)
			{
				return false;
			}
			if (this.HasMaxFriends != subscribeToFriendsResponse.HasMaxFriends || this.HasMaxFriends && !this.MaxFriends.Equals(subscribeToFriendsResponse.MaxFriends))
			{
				return false;
			}
			if (this.HasMaxReceivedInvitations != subscribeToFriendsResponse.HasMaxReceivedInvitations || this.HasMaxReceivedInvitations && !this.MaxReceivedInvitations.Equals(subscribeToFriendsResponse.MaxReceivedInvitations))
			{
				return false;
			}
			if (this.HasMaxSentInvitations != subscribeToFriendsResponse.HasMaxSentInvitations || this.HasMaxSentInvitations && !this.MaxSentInvitations.Equals(subscribeToFriendsResponse.MaxSentInvitations))
			{
				return false;
			}
			if (this.Role.Count != subscribeToFriendsResponse.Role.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Role.Count; i++)
			{
				if (!this.Role[i].Equals(subscribeToFriendsResponse.Role[i]))
				{
					return false;
				}
			}
			if (this.Friends.Count != subscribeToFriendsResponse.Friends.Count)
			{
				return false;
			}
			for (int j = 0; j < this.Friends.Count; j++)
			{
				if (!this.Friends[j].Equals(subscribeToFriendsResponse.Friends[j]))
				{
					return false;
				}
			}
			if (this.SentInvitations.Count != subscribeToFriendsResponse.SentInvitations.Count)
			{
				return false;
			}
			for (int k = 0; k < this.SentInvitations.Count; k++)
			{
				if (!this.SentInvitations[k].Equals(subscribeToFriendsResponse.SentInvitations[k]))
				{
					return false;
				}
			}
			if (this.ReceivedInvitations.Count != subscribeToFriendsResponse.ReceivedInvitations.Count)
			{
				return false;
			}
			for (int l = 0; l < this.ReceivedInvitations.Count; l++)
			{
				if (!this.ReceivedInvitations[l].Equals(subscribeToFriendsResponse.ReceivedInvitations[l]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasMaxFriends)
			{
				hashCode ^= this.MaxFriends.GetHashCode();
			}
			if (this.HasMaxReceivedInvitations)
			{
				hashCode ^= this.MaxReceivedInvitations.GetHashCode();
			}
			if (this.HasMaxSentInvitations)
			{
				hashCode ^= this.MaxSentInvitations.GetHashCode();
			}
			foreach (bnet.protocol.Role role in this.Role)
			{
				hashCode ^= role.GetHashCode();
			}
			foreach (Friend friend in this.Friends)
			{
				hashCode ^= friend.GetHashCode();
			}
			foreach (Invitation sentInvitation in this.SentInvitations)
			{
				hashCode ^= sentInvitation.GetHashCode();
			}
			foreach (Invitation receivedInvitation in this.ReceivedInvitations)
			{
				hashCode ^= receivedInvitation.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasMaxFriends)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.MaxFriends);
			}
			if (this.HasMaxReceivedInvitations)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.MaxReceivedInvitations);
			}
			if (this.HasMaxSentInvitations)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.MaxSentInvitations);
			}
			if (this.Role.Count > 0)
			{
				foreach (bnet.protocol.Role role in this.Role)
				{
					num++;
					uint serializedSize = role.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			if (this.Friends.Count > 0)
			{
				foreach (Friend friend in this.Friends)
				{
					num++;
					uint serializedSize1 = friend.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			if (this.SentInvitations.Count > 0)
			{
				foreach (Invitation sentInvitation in this.SentInvitations)
				{
					num++;
					uint num1 = sentInvitation.GetSerializedSize();
					num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
				}
			}
			if (this.ReceivedInvitations.Count > 0)
			{
				foreach (Invitation receivedInvitation in this.ReceivedInvitations)
				{
					num++;
					uint serializedSize2 = receivedInvitation.GetSerializedSize();
					num = num + serializedSize2 + ProtocolParser.SizeOfUInt32(serializedSize2);
				}
			}
			return num;
		}

		public static SubscribeToFriendsResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<SubscribeToFriendsResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			SubscribeToFriendsResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, SubscribeToFriendsResponse instance)
		{
			if (instance.HasMaxFriends)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.MaxFriends);
			}
			if (instance.HasMaxReceivedInvitations)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.MaxReceivedInvitations);
			}
			if (instance.HasMaxSentInvitations)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.MaxSentInvitations);
			}
			if (instance.Role.Count > 0)
			{
				foreach (bnet.protocol.Role role in instance.Role)
				{
					stream.WriteByte(34);
					ProtocolParser.WriteUInt32(stream, role.GetSerializedSize());
					bnet.protocol.Role.Serialize(stream, role);
				}
			}
			if (instance.Friends.Count > 0)
			{
				foreach (Friend friend in instance.Friends)
				{
					stream.WriteByte(42);
					ProtocolParser.WriteUInt32(stream, friend.GetSerializedSize());
					Friend.Serialize(stream, friend);
				}
			}
			if (instance.SentInvitations.Count > 0)
			{
				foreach (Invitation sentInvitation in instance.SentInvitations)
				{
					stream.WriteByte(50);
					ProtocolParser.WriteUInt32(stream, sentInvitation.GetSerializedSize());
					Invitation.Serialize(stream, sentInvitation);
				}
			}
			if (instance.ReceivedInvitations.Count > 0)
			{
				foreach (Invitation receivedInvitation in instance.ReceivedInvitations)
				{
					stream.WriteByte(58);
					ProtocolParser.WriteUInt32(stream, receivedInvitation.GetSerializedSize());
					Invitation.Serialize(stream, receivedInvitation);
				}
			}
		}

		public void SetFriends(List<Friend> val)
		{
			this.Friends = val;
		}

		public void SetMaxFriends(uint val)
		{
			this.MaxFriends = val;
		}

		public void SetMaxReceivedInvitations(uint val)
		{
			this.MaxReceivedInvitations = val;
		}

		public void SetMaxSentInvitations(uint val)
		{
			this.MaxSentInvitations = val;
		}

		public void SetReceivedInvitations(List<Invitation> val)
		{
			this.ReceivedInvitations = val;
		}

		public void SetRole(List<bnet.protocol.Role> val)
		{
			this.Role = val;
		}

		public void SetSentInvitations(List<Invitation> val)
		{
			this.SentInvitations = val;
		}
	}
}