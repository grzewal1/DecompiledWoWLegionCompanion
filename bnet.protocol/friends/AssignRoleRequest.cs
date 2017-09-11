using bnet.protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.friends
{
	public class AssignRoleRequest : IProtoBuf
	{
		public bool HasAgentId;

		private EntityId _AgentId;

		private List<int> _Role = new List<int>();

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

		public List<int> Role
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

		public List<int> RoleList
		{
			get
			{
				return this._Role;
			}
		}

		public EntityId TargetId
		{
			get;
			set;
		}

		public AssignRoleRequest()
		{
		}

		public void AddRole(int val)
		{
			this._Role.Add(val);
		}

		public void ClearRole()
		{
			this._Role.Clear();
		}

		public void Deserialize(Stream stream)
		{
			AssignRoleRequest.Deserialize(stream, this);
		}

		public static AssignRoleRequest Deserialize(Stream stream, AssignRoleRequest instance)
		{
			return AssignRoleRequest.Deserialize(stream, instance, (long)-1);
		}

		public static AssignRoleRequest Deserialize(Stream stream, AssignRoleRequest instance, long limit)
		{
			if (instance.Role == null)
			{
				instance.Role = new List<int>();
			}
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
							if (instance.AgentId != null)
							{
								EntityId.DeserializeLengthDelimited(stream, instance.AgentId);
							}
							else
							{
								instance.AgentId = EntityId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 != 18)
						{
							if (num1 == 24)
							{
								instance.Role.Add((int)ProtocolParser.ReadUInt64(stream));
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
						else if (instance.TargetId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.TargetId);
						}
						else
						{
							instance.TargetId = EntityId.DeserializeLengthDelimited(stream);
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

		public static AssignRoleRequest DeserializeLengthDelimited(Stream stream)
		{
			AssignRoleRequest assignRoleRequest = new AssignRoleRequest();
			AssignRoleRequest.DeserializeLengthDelimited(stream, assignRoleRequest);
			return assignRoleRequest;
		}

		public static AssignRoleRequest DeserializeLengthDelimited(Stream stream, AssignRoleRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return AssignRoleRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			AssignRoleRequest assignRoleRequest = obj as AssignRoleRequest;
			if (assignRoleRequest == null)
			{
				return false;
			}
			if (this.HasAgentId != assignRoleRequest.HasAgentId || this.HasAgentId && !this.AgentId.Equals(assignRoleRequest.AgentId))
			{
				return false;
			}
			if (!this.TargetId.Equals(assignRoleRequest.TargetId))
			{
				return false;
			}
			if (this.Role.Count != assignRoleRequest.Role.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Role.Count; i++)
			{
				if (!this.Role[i].Equals(assignRoleRequest.Role[i]))
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
			hashCode ^= this.TargetId.GetHashCode();
			foreach (int role in this.Role)
			{
				hashCode ^= role.GetHashCode();
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
			uint serializedSize1 = this.TargetId.GetSerializedSize();
			num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			if (this.Role.Count > 0)
			{
				foreach (int role in this.Role)
				{
					num++;
					num += ProtocolParser.SizeOfUInt64((ulong)role);
				}
			}
			num++;
			return num;
		}

		public static AssignRoleRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<AssignRoleRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			AssignRoleRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, AssignRoleRequest instance)
		{
			if (instance.HasAgentId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentId.GetSerializedSize());
				EntityId.Serialize(stream, instance.AgentId);
			}
			if (instance.TargetId == null)
			{
				throw new ArgumentNullException("TargetId", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.TargetId.GetSerializedSize());
			EntityId.Serialize(stream, instance.TargetId);
			if (instance.Role.Count > 0)
			{
				foreach (int role in instance.Role)
				{
					stream.WriteByte(24);
					ProtocolParser.WriteUInt64(stream, (ulong)role);
				}
			}
		}

		public void SetAgentId(EntityId val)
		{
			this.AgentId = val;
		}

		public void SetRole(List<int> val)
		{
			this.Role = val;
		}

		public void SetTargetId(EntityId val)
		{
			this.TargetId = val;
		}
	}
}