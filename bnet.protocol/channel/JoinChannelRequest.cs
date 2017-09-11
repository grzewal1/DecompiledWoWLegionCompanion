using bnet.protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel
{
	public class JoinChannelRequest : IProtoBuf
	{
		public bool HasAgentIdentity;

		private Identity _AgentIdentity;

		public bool HasMemberState;

		private bnet.protocol.channel.MemberState _MemberState;

		private List<EntityId> _FriendAccountId = new List<EntityId>();

		public bool HasLocalSubscriber;

		private bool _LocalSubscriber;

		public Identity AgentIdentity
		{
			get
			{
				return this._AgentIdentity;
			}
			set
			{
				this._AgentIdentity = value;
				this.HasAgentIdentity = value != null;
			}
		}

		public EntityId ChannelId
		{
			get;
			set;
		}

		public List<EntityId> FriendAccountId
		{
			get
			{
				return this._FriendAccountId;
			}
			set
			{
				this._FriendAccountId = value;
			}
		}

		public int FriendAccountIdCount
		{
			get
			{
				return this._FriendAccountId.Count;
			}
		}

		public List<EntityId> FriendAccountIdList
		{
			get
			{
				return this._FriendAccountId;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public bool LocalSubscriber
		{
			get
			{
				return this._LocalSubscriber;
			}
			set
			{
				this._LocalSubscriber = value;
				this.HasLocalSubscriber = true;
			}
		}

		public bnet.protocol.channel.MemberState MemberState
		{
			get
			{
				return this._MemberState;
			}
			set
			{
				this._MemberState = value;
				this.HasMemberState = value != null;
			}
		}

		public ulong ObjectId
		{
			get;
			set;
		}

		public JoinChannelRequest()
		{
		}

		public void AddFriendAccountId(EntityId val)
		{
			this._FriendAccountId.Add(val);
		}

		public void ClearFriendAccountId()
		{
			this._FriendAccountId.Clear();
		}

		public void Deserialize(Stream stream)
		{
			JoinChannelRequest.Deserialize(stream, this);
		}

		public static JoinChannelRequest Deserialize(Stream stream, JoinChannelRequest instance)
		{
			return JoinChannelRequest.Deserialize(stream, instance, (long)-1);
		}

		public static JoinChannelRequest Deserialize(Stream stream, JoinChannelRequest instance, long limit)
		{
			if (instance.FriendAccountId == null)
			{
				instance.FriendAccountId = new List<EntityId>();
			}
			instance.LocalSubscriber = true;
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
							if (instance.AgentIdentity != null)
							{
								Identity.DeserializeLengthDelimited(stream, instance.AgentIdentity);
							}
							else
							{
								instance.AgentIdentity = Identity.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 18)
						{
							if (instance.MemberState != null)
							{
								bnet.protocol.channel.MemberState.DeserializeLengthDelimited(stream, instance.MemberState);
							}
							else
							{
								instance.MemberState = bnet.protocol.channel.MemberState.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 26)
						{
							if (instance.ChannelId != null)
							{
								EntityId.DeserializeLengthDelimited(stream, instance.ChannelId);
							}
							else
							{
								instance.ChannelId = EntityId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 32)
						{
							instance.ObjectId = ProtocolParser.ReadUInt64(stream);
						}
						else if (num1 == 42)
						{
							instance.FriendAccountId.Add(EntityId.DeserializeLengthDelimited(stream));
						}
						else if (num1 == 48)
						{
							instance.LocalSubscriber = ProtocolParser.ReadBool(stream);
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

		public static JoinChannelRequest DeserializeLengthDelimited(Stream stream)
		{
			JoinChannelRequest joinChannelRequest = new JoinChannelRequest();
			JoinChannelRequest.DeserializeLengthDelimited(stream, joinChannelRequest);
			return joinChannelRequest;
		}

		public static JoinChannelRequest DeserializeLengthDelimited(Stream stream, JoinChannelRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return JoinChannelRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			JoinChannelRequest joinChannelRequest = obj as JoinChannelRequest;
			if (joinChannelRequest == null)
			{
				return false;
			}
			if (this.HasAgentIdentity != joinChannelRequest.HasAgentIdentity || this.HasAgentIdentity && !this.AgentIdentity.Equals(joinChannelRequest.AgentIdentity))
			{
				return false;
			}
			if (this.HasMemberState != joinChannelRequest.HasMemberState || this.HasMemberState && !this.MemberState.Equals(joinChannelRequest.MemberState))
			{
				return false;
			}
			if (!this.ChannelId.Equals(joinChannelRequest.ChannelId))
			{
				return false;
			}
			if (!this.ObjectId.Equals(joinChannelRequest.ObjectId))
			{
				return false;
			}
			if (this.FriendAccountId.Count != joinChannelRequest.FriendAccountId.Count)
			{
				return false;
			}
			for (int i = 0; i < this.FriendAccountId.Count; i++)
			{
				if (!this.FriendAccountId[i].Equals(joinChannelRequest.FriendAccountId[i]))
				{
					return false;
				}
			}
			if (this.HasLocalSubscriber == joinChannelRequest.HasLocalSubscriber && (!this.HasLocalSubscriber || this.LocalSubscriber.Equals(joinChannelRequest.LocalSubscriber)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasAgentIdentity)
			{
				hashCode ^= this.AgentIdentity.GetHashCode();
			}
			if (this.HasMemberState)
			{
				hashCode ^= this.MemberState.GetHashCode();
			}
			hashCode ^= this.ChannelId.GetHashCode();
			hashCode ^= this.ObjectId.GetHashCode();
			foreach (EntityId friendAccountId in this.FriendAccountId)
			{
				hashCode ^= friendAccountId.GetHashCode();
			}
			if (this.HasLocalSubscriber)
			{
				hashCode ^= this.LocalSubscriber.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasAgentIdentity)
			{
				num++;
				uint serializedSize = this.AgentIdentity.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasMemberState)
			{
				num++;
				uint serializedSize1 = this.MemberState.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			uint num1 = this.ChannelId.GetSerializedSize();
			num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
			num += ProtocolParser.SizeOfUInt64(this.ObjectId);
			if (this.FriendAccountId.Count > 0)
			{
				foreach (EntityId friendAccountId in this.FriendAccountId)
				{
					num++;
					uint serializedSize2 = friendAccountId.GetSerializedSize();
					num = num + serializedSize2 + ProtocolParser.SizeOfUInt32(serializedSize2);
				}
			}
			if (this.HasLocalSubscriber)
			{
				num++;
				num++;
			}
			num += 2;
			return num;
		}

		public static JoinChannelRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<JoinChannelRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			JoinChannelRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, JoinChannelRequest instance)
		{
			if (instance.HasAgentIdentity)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentIdentity.GetSerializedSize());
				Identity.Serialize(stream, instance.AgentIdentity);
			}
			if (instance.HasMemberState)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.MemberState.GetSerializedSize());
				bnet.protocol.channel.MemberState.Serialize(stream, instance.MemberState);
			}
			if (instance.ChannelId == null)
			{
				throw new ArgumentNullException("ChannelId", "Required by proto specification.");
			}
			stream.WriteByte(26);
			ProtocolParser.WriteUInt32(stream, instance.ChannelId.GetSerializedSize());
			EntityId.Serialize(stream, instance.ChannelId);
			stream.WriteByte(32);
			ProtocolParser.WriteUInt64(stream, instance.ObjectId);
			if (instance.FriendAccountId.Count > 0)
			{
				foreach (EntityId friendAccountId in instance.FriendAccountId)
				{
					stream.WriteByte(42);
					ProtocolParser.WriteUInt32(stream, friendAccountId.GetSerializedSize());
					EntityId.Serialize(stream, friendAccountId);
				}
			}
			if (instance.HasLocalSubscriber)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteBool(stream, instance.LocalSubscriber);
			}
		}

		public void SetAgentIdentity(Identity val)
		{
			this.AgentIdentity = val;
		}

		public void SetChannelId(EntityId val)
		{
			this.ChannelId = val;
		}

		public void SetFriendAccountId(List<EntityId> val)
		{
			this.FriendAccountId = val;
		}

		public void SetLocalSubscriber(bool val)
		{
			this.LocalSubscriber = val;
		}

		public void SetMemberState(bnet.protocol.channel.MemberState val)
		{
			this.MemberState = val;
		}

		public void SetObjectId(ulong val)
		{
			this.ObjectId = val;
		}
	}
}