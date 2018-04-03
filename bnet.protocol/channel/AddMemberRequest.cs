using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel
{
	public class AddMemberRequest : IProtoBuf
	{
		public bool HasAgentId;

		private EntityId _AgentId;

		public bool HasSubscribe;

		private bool _Subscribe;

		public EntityId AgentId
		{
			get
			{
				return this._AgentId;
			}
			set
			{
				this._AgentId = value;
				this.HasAgentId = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public Identity MemberIdentity
		{
			get;
			set;
		}

		public bnet.protocol.channel.MemberState MemberState
		{
			get;
			set;
		}

		public ulong ObjectId
		{
			get;
			set;
		}

		public bool Subscribe
		{
			get
			{
				return this._Subscribe;
			}
			set
			{
				this._Subscribe = value;
				this.HasSubscribe = true;
			}
		}

		public AddMemberRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			AddMemberRequest.Deserialize(stream, this);
		}

		public static AddMemberRequest Deserialize(Stream stream, AddMemberRequest instance)
		{
			return AddMemberRequest.Deserialize(stream, instance, (long)-1);
		}

		public static AddMemberRequest Deserialize(Stream stream, AddMemberRequest instance, long limit)
		{
			instance.Subscribe = true;
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
						if (instance.AgentId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.AgentId);
						}
						else
						{
							instance.AgentId = EntityId.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 18)
					{
						if (instance.MemberIdentity != null)
						{
							Identity.DeserializeLengthDelimited(stream, instance.MemberIdentity);
						}
						else
						{
							instance.MemberIdentity = Identity.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 26)
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
					else if (num == 32)
					{
						instance.ObjectId = ProtocolParser.ReadUInt64(stream);
					}
					else if (num == 40)
					{
						instance.Subscribe = ProtocolParser.ReadBool(stream);
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

		public static AddMemberRequest DeserializeLengthDelimited(Stream stream)
		{
			AddMemberRequest addMemberRequest = new AddMemberRequest();
			AddMemberRequest.DeserializeLengthDelimited(stream, addMemberRequest);
			return addMemberRequest;
		}

		public static AddMemberRequest DeserializeLengthDelimited(Stream stream, AddMemberRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return AddMemberRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			AddMemberRequest addMemberRequest = obj as AddMemberRequest;
			if (addMemberRequest == null)
			{
				return false;
			}
			if (this.HasAgentId != addMemberRequest.HasAgentId || this.HasAgentId && !this.AgentId.Equals(addMemberRequest.AgentId))
			{
				return false;
			}
			if (!this.MemberIdentity.Equals(addMemberRequest.MemberIdentity))
			{
				return false;
			}
			if (!this.MemberState.Equals(addMemberRequest.MemberState))
			{
				return false;
			}
			if (!this.ObjectId.Equals(addMemberRequest.ObjectId))
			{
				return false;
			}
			if (this.HasSubscribe == addMemberRequest.HasSubscribe && (!this.HasSubscribe || this.Subscribe.Equals(addMemberRequest.Subscribe)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasAgentId)
			{
				hashCode ^= this.AgentId.GetHashCode();
			}
			hashCode ^= this.MemberIdentity.GetHashCode();
			hashCode ^= this.MemberState.GetHashCode();
			hashCode ^= this.ObjectId.GetHashCode();
			if (this.HasSubscribe)
			{
				hashCode ^= this.Subscribe.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasAgentId)
			{
				num++;
				uint serializedSize = this.AgentId.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			uint serializedSize1 = this.MemberIdentity.GetSerializedSize();
			num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			uint num1 = this.MemberState.GetSerializedSize();
			num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
			num += ProtocolParser.SizeOfUInt64(this.ObjectId);
			if (this.HasSubscribe)
			{
				num++;
				num++;
			}
			num += 3;
			return num;
		}

		public static AddMemberRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<AddMemberRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			AddMemberRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, AddMemberRequest instance)
		{
			if (instance.HasAgentId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentId.GetSerializedSize());
				EntityId.Serialize(stream, instance.AgentId);
			}
			if (instance.MemberIdentity == null)
			{
				throw new ArgumentNullException("MemberIdentity", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.MemberIdentity.GetSerializedSize());
			Identity.Serialize(stream, instance.MemberIdentity);
			if (instance.MemberState == null)
			{
				throw new ArgumentNullException("MemberState", "Required by proto specification.");
			}
			stream.WriteByte(26);
			ProtocolParser.WriteUInt32(stream, instance.MemberState.GetSerializedSize());
			bnet.protocol.channel.MemberState.Serialize(stream, instance.MemberState);
			stream.WriteByte(32);
			ProtocolParser.WriteUInt64(stream, instance.ObjectId);
			if (instance.HasSubscribe)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteBool(stream, instance.Subscribe);
			}
		}

		public void SetAgentId(EntityId val)
		{
			this.AgentId = val;
		}

		public void SetMemberIdentity(Identity val)
		{
			this.MemberIdentity = val;
		}

		public void SetMemberState(bnet.protocol.channel.MemberState val)
		{
			this.MemberState = val;
		}

		public void SetObjectId(ulong val)
		{
			this.ObjectId = val;
		}

		public void SetSubscribe(bool val)
		{
			this.Subscribe = val;
		}
	}
}