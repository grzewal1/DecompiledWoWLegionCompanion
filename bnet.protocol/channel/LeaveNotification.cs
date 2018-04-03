using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel
{
	public class LeaveNotification : IProtoBuf
	{
		public bool HasAgentId;

		private EntityId _AgentId;

		public bool HasReason;

		private uint _Reason;

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

		public EntityId MemberId
		{
			get;
			set;
		}

		public uint Reason
		{
			get
			{
				return this._Reason;
			}
			set
			{
				this._Reason = value;
				this.HasReason = true;
			}
		}

		public LeaveNotification()
		{
		}

		public void Deserialize(Stream stream)
		{
			LeaveNotification.Deserialize(stream, this);
		}

		public static LeaveNotification Deserialize(Stream stream, LeaveNotification instance)
		{
			return LeaveNotification.Deserialize(stream, instance, (long)-1);
		}

		public static LeaveNotification Deserialize(Stream stream, LeaveNotification instance, long limit)
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
						if (instance.AgentId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.AgentId);
						}
						else
						{
							instance.AgentId = EntityId.DeserializeLengthDelimited(stream);
						}
					}
					else if (num != 18)
					{
						if (num == 24)
						{
							instance.Reason = ProtocolParser.ReadUInt32(stream);
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
					else if (instance.MemberId != null)
					{
						EntityId.DeserializeLengthDelimited(stream, instance.MemberId);
					}
					else
					{
						instance.MemberId = EntityId.DeserializeLengthDelimited(stream);
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

		public static LeaveNotification DeserializeLengthDelimited(Stream stream)
		{
			LeaveNotification leaveNotification = new LeaveNotification();
			LeaveNotification.DeserializeLengthDelimited(stream, leaveNotification);
			return leaveNotification;
		}

		public static LeaveNotification DeserializeLengthDelimited(Stream stream, LeaveNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return LeaveNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			LeaveNotification leaveNotification = obj as LeaveNotification;
			if (leaveNotification == null)
			{
				return false;
			}
			if (this.HasAgentId != leaveNotification.HasAgentId || this.HasAgentId && !this.AgentId.Equals(leaveNotification.AgentId))
			{
				return false;
			}
			if (!this.MemberId.Equals(leaveNotification.MemberId))
			{
				return false;
			}
			if (this.HasReason == leaveNotification.HasReason && (!this.HasReason || this.Reason.Equals(leaveNotification.Reason)))
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
			hashCode ^= this.MemberId.GetHashCode();
			if (this.HasReason)
			{
				hashCode ^= this.Reason.GetHashCode();
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
			uint serializedSize1 = this.MemberId.GetSerializedSize();
			num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			if (this.HasReason)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.Reason);
			}
			num++;
			return num;
		}

		public static LeaveNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<LeaveNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			LeaveNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, LeaveNotification instance)
		{
			if (instance.HasAgentId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentId.GetSerializedSize());
				EntityId.Serialize(stream, instance.AgentId);
			}
			if (instance.MemberId == null)
			{
				throw new ArgumentNullException("MemberId", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.MemberId.GetSerializedSize());
			EntityId.Serialize(stream, instance.MemberId);
			if (instance.HasReason)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.Reason);
			}
		}

		public void SetAgentId(EntityId val)
		{
			this.AgentId = val;
		}

		public void SetMemberId(EntityId val)
		{
			this.MemberId = val;
		}

		public void SetReason(uint val)
		{
			this.Reason = val;
		}
	}
}