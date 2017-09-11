using bnet.protocol;
using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.channel
{
	public class UpdateMemberStateNotification : IProtoBuf
	{
		public bool HasAgentId;

		private EntityId _AgentId;

		private List<Member> _StateChange = new List<Member>();

		private List<uint> _RemovedRole = new List<uint>();

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

		public List<uint> RemovedRole
		{
			get
			{
				return this._RemovedRole;
			}
			set
			{
				this._RemovedRole = value;
			}
		}

		public int RemovedRoleCount
		{
			get
			{
				return this._RemovedRole.Count;
			}
		}

		public List<uint> RemovedRoleList
		{
			get
			{
				return this._RemovedRole;
			}
		}

		public List<Member> StateChange
		{
			get
			{
				return this._StateChange;
			}
			set
			{
				this._StateChange = value;
			}
		}

		public int StateChangeCount
		{
			get
			{
				return this._StateChange.Count;
			}
		}

		public List<Member> StateChangeList
		{
			get
			{
				return this._StateChange;
			}
		}

		public UpdateMemberStateNotification()
		{
		}

		public void AddRemovedRole(uint val)
		{
			this._RemovedRole.Add(val);
		}

		public void AddStateChange(Member val)
		{
			this._StateChange.Add(val);
		}

		public void ClearRemovedRole()
		{
			this._RemovedRole.Clear();
		}

		public void ClearStateChange()
		{
			this._StateChange.Clear();
		}

		public void Deserialize(Stream stream)
		{
			UpdateMemberStateNotification.Deserialize(stream, this);
		}

		public static UpdateMemberStateNotification Deserialize(Stream stream, UpdateMemberStateNotification instance)
		{
			return UpdateMemberStateNotification.Deserialize(stream, instance, (long)-1);
		}

		public static UpdateMemberStateNotification Deserialize(Stream stream, UpdateMemberStateNotification instance, long limit)
		{
			if (instance.StateChange == null)
			{
				instance.StateChange = new List<Member>();
			}
			if (instance.RemovedRole == null)
			{
				instance.RemovedRole = new List<uint>();
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
						else if (num1 == 18)
						{
							instance.StateChange.Add(Member.DeserializeLengthDelimited(stream));
						}
						else if (num1 == 26)
						{
							long position = (long)ProtocolParser.ReadUInt32(stream);
							position += stream.Position;
							while (stream.Position < position)
							{
								instance.RemovedRole.Add(ProtocolParser.ReadUInt32(stream));
							}
							if (stream.Position != position)
							{
								throw new ProtocolBufferException("Read too many bytes in packed data");
							}
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

		public static UpdateMemberStateNotification DeserializeLengthDelimited(Stream stream)
		{
			UpdateMemberStateNotification updateMemberStateNotification = new UpdateMemberStateNotification();
			UpdateMemberStateNotification.DeserializeLengthDelimited(stream, updateMemberStateNotification);
			return updateMemberStateNotification;
		}

		public static UpdateMemberStateNotification DeserializeLengthDelimited(Stream stream, UpdateMemberStateNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return UpdateMemberStateNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			UpdateMemberStateNotification updateMemberStateNotification = obj as UpdateMemberStateNotification;
			if (updateMemberStateNotification == null)
			{
				return false;
			}
			if (this.HasAgentId != updateMemberStateNotification.HasAgentId || this.HasAgentId && !this.AgentId.Equals(updateMemberStateNotification.AgentId))
			{
				return false;
			}
			if (this.StateChange.Count != updateMemberStateNotification.StateChange.Count)
			{
				return false;
			}
			for (int i = 0; i < this.StateChange.Count; i++)
			{
				if (!this.StateChange[i].Equals(updateMemberStateNotification.StateChange[i]))
				{
					return false;
				}
			}
			if (this.RemovedRole.Count != updateMemberStateNotification.RemovedRole.Count)
			{
				return false;
			}
			for (int j = 0; j < this.RemovedRole.Count; j++)
			{
				if (!this.RemovedRole[j].Equals(updateMemberStateNotification.RemovedRole[j]))
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
			foreach (Member stateChange in this.StateChange)
			{
				hashCode ^= stateChange.GetHashCode();
			}
			foreach (uint removedRole in this.RemovedRole)
			{
				hashCode ^= removedRole.GetHashCode();
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
			if (this.StateChange.Count > 0)
			{
				foreach (Member stateChange in this.StateChange)
				{
					num++;
					uint serializedSize1 = stateChange.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			if (this.RemovedRole.Count > 0)
			{
				num++;
				uint num1 = num;
				foreach (uint removedRole in this.RemovedRole)
				{
					num += ProtocolParser.SizeOfUInt32(removedRole);
				}
				num += ProtocolParser.SizeOfUInt32(num - num1);
			}
			return num;
		}

		public static UpdateMemberStateNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<UpdateMemberStateNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			UpdateMemberStateNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, UpdateMemberStateNotification instance)
		{
			if (instance.HasAgentId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentId.GetSerializedSize());
				EntityId.Serialize(stream, instance.AgentId);
			}
			if (instance.StateChange.Count > 0)
			{
				foreach (Member stateChange in instance.StateChange)
				{
					stream.WriteByte(18);
					ProtocolParser.WriteUInt32(stream, stateChange.GetSerializedSize());
					Member.Serialize(stream, stateChange);
				}
			}
			if (instance.RemovedRole.Count > 0)
			{
				stream.WriteByte(26);
				uint num = 0;
				foreach (uint removedRole in instance.RemovedRole)
				{
					num += ProtocolParser.SizeOfUInt32(removedRole);
				}
				ProtocolParser.WriteUInt32(stream, num);
				foreach (uint removedRole1 in instance.RemovedRole)
				{
					ProtocolParser.WriteUInt32(stream, removedRole1);
				}
			}
		}

		public void SetAgentId(EntityId val)
		{
			this.AgentId = val;
		}

		public void SetRemovedRole(List<uint> val)
		{
			this.RemovedRole = val;
		}

		public void SetStateChange(List<Member> val)
		{
			this.StateChange = val;
		}
	}
}