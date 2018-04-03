using bnet.protocol;
using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.channel
{
	public class SetRolesRequest : IProtoBuf
	{
		public bool HasAgentId;

		private EntityId _AgentId;

		private List<uint> _Role = new List<uint>();

		private List<EntityId> _MemberId = new List<EntityId>();

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

		public List<EntityId> MemberId
		{
			get
			{
				return this._MemberId;
			}
			set
			{
				this._MemberId = value;
			}
		}

		public int MemberIdCount
		{
			get
			{
				return this._MemberId.Count;
			}
		}

		public List<EntityId> MemberIdList
		{
			get
			{
				return this._MemberId;
			}
		}

		public List<uint> Role
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

		public List<uint> RoleList
		{
			get
			{
				return this._Role;
			}
		}

		public SetRolesRequest()
		{
		}

		public void AddMemberId(EntityId val)
		{
			this._MemberId.Add(val);
		}

		public void AddRole(uint val)
		{
			this._Role.Add(val);
		}

		public void ClearMemberId()
		{
			this._MemberId.Clear();
		}

		public void ClearRole()
		{
			this._Role.Clear();
		}

		public void Deserialize(Stream stream)
		{
			SetRolesRequest.Deserialize(stream, this);
		}

		public static SetRolesRequest Deserialize(Stream stream, SetRolesRequest instance)
		{
			return SetRolesRequest.Deserialize(stream, instance, (long)-1);
		}

		public static SetRolesRequest Deserialize(Stream stream, SetRolesRequest instance, long limit)
		{
			if (instance.Role == null)
			{
				instance.Role = new List<uint>();
			}
			if (instance.MemberId == null)
			{
				instance.MemberId = new List<EntityId>();
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
						long position = (long)ProtocolParser.ReadUInt32(stream);
						position += stream.Position;
						while (stream.Position < position)
						{
							instance.Role.Add(ProtocolParser.ReadUInt32(stream));
						}
						if (stream.Position != position)
						{
							throw new ProtocolBufferException("Read too many bytes in packed data");
						}
					}
					else if (num == 26)
					{
						instance.MemberId.Add(EntityId.DeserializeLengthDelimited(stream));
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

		public static SetRolesRequest DeserializeLengthDelimited(Stream stream)
		{
			SetRolesRequest setRolesRequest = new SetRolesRequest();
			SetRolesRequest.DeserializeLengthDelimited(stream, setRolesRequest);
			return setRolesRequest;
		}

		public static SetRolesRequest DeserializeLengthDelimited(Stream stream, SetRolesRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return SetRolesRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			SetRolesRequest setRolesRequest = obj as SetRolesRequest;
			if (setRolesRequest == null)
			{
				return false;
			}
			if (this.HasAgentId != setRolesRequest.HasAgentId || this.HasAgentId && !this.AgentId.Equals(setRolesRequest.AgentId))
			{
				return false;
			}
			if (this.Role.Count != setRolesRequest.Role.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Role.Count; i++)
			{
				if (!this.Role[i].Equals(setRolesRequest.Role[i]))
				{
					return false;
				}
			}
			if (this.MemberId.Count != setRolesRequest.MemberId.Count)
			{
				return false;
			}
			for (int j = 0; j < this.MemberId.Count; j++)
			{
				if (!this.MemberId[j].Equals(setRolesRequest.MemberId[j]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasAgentId)
			{
				hashCode ^= this.AgentId.GetHashCode();
			}
			foreach (uint role in this.Role)
			{
				hashCode ^= role.GetHashCode();
			}
			foreach (EntityId memberId in this.MemberId)
			{
				hashCode ^= memberId.GetHashCode();
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
			if (this.Role.Count > 0)
			{
				num++;
				uint num1 = num;
				foreach (uint role in this.Role)
				{
					num += ProtocolParser.SizeOfUInt32(role);
				}
				num += ProtocolParser.SizeOfUInt32(num - num1);
			}
			if (this.MemberId.Count > 0)
			{
				foreach (EntityId memberId in this.MemberId)
				{
					num++;
					uint serializedSize1 = memberId.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			return num;
		}

		public static SetRolesRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<SetRolesRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			SetRolesRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, SetRolesRequest instance)
		{
			if (instance.HasAgentId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentId.GetSerializedSize());
				EntityId.Serialize(stream, instance.AgentId);
			}
			if (instance.Role.Count > 0)
			{
				stream.WriteByte(18);
				uint num = 0;
				foreach (uint role in instance.Role)
				{
					num += ProtocolParser.SizeOfUInt32(role);
				}
				ProtocolParser.WriteUInt32(stream, num);
				foreach (uint role1 in instance.Role)
				{
					ProtocolParser.WriteUInt32(stream, role1);
				}
			}
			if (instance.MemberId.Count > 0)
			{
				foreach (EntityId memberId in instance.MemberId)
				{
					stream.WriteByte(26);
					ProtocolParser.WriteUInt32(stream, memberId.GetSerializedSize());
					EntityId.Serialize(stream, memberId);
				}
			}
		}

		public void SetAgentId(EntityId val)
		{
			this.AgentId = val;
		}

		public void SetMemberId(List<EntityId> val)
		{
			this.MemberId = val;
		}

		public void SetRole(List<uint> val)
		{
			this.Role = val;
		}
	}
}