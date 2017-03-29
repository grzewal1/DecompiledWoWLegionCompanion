using bnet.protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.invitation
{
	public class GenericRequest : IProtoBuf
	{
		public bool HasAgentId;

		private EntityId _AgentId;

		public bool HasTargetId;

		private EntityId _TargetId;

		public bool HasInviteeName;

		private string _InviteeName;

		public bool HasInviterName;

		private string _InviterName;

		private List<uint> _PreviousRole = new List<uint>();

		private List<uint> _DesiredRole = new List<uint>();

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

		public List<uint> DesiredRole
		{
			get
			{
				return this._DesiredRole;
			}
			set
			{
				this._DesiredRole = value;
			}
		}

		public int DesiredRoleCount
		{
			get
			{
				return this._DesiredRole.Count;
			}
		}

		public List<uint> DesiredRoleList
		{
			get
			{
				return this._DesiredRole;
			}
		}

		public ulong InvitationId
		{
			get;
			set;
		}

		public string InviteeName
		{
			get
			{
				return this._InviteeName;
			}
			set
			{
				this._InviteeName = value;
				this.HasInviteeName = value != null;
			}
		}

		public string InviterName
		{
			get
			{
				return this._InviterName;
			}
			set
			{
				this._InviterName = value;
				this.HasInviterName = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public List<uint> PreviousRole
		{
			get
			{
				return this._PreviousRole;
			}
			set
			{
				this._PreviousRole = value;
			}
		}

		public int PreviousRoleCount
		{
			get
			{
				return this._PreviousRole.Count;
			}
		}

		public List<uint> PreviousRoleList
		{
			get
			{
				return this._PreviousRole;
			}
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

		public EntityId TargetId
		{
			get
			{
				return this._TargetId;
			}
			set
			{
				this._TargetId = value;
				this.HasTargetId = value != null;
			}
		}

		public GenericRequest()
		{
		}

		public void AddDesiredRole(uint val)
		{
			this._DesiredRole.Add(val);
		}

		public void AddPreviousRole(uint val)
		{
			this._PreviousRole.Add(val);
		}

		public void ClearDesiredRole()
		{
			this._DesiredRole.Clear();
		}

		public void ClearPreviousRole()
		{
			this._PreviousRole.Clear();
		}

		public void Deserialize(Stream stream)
		{
			GenericRequest.Deserialize(stream, this);
		}

		public static GenericRequest Deserialize(Stream stream, GenericRequest instance)
		{
			return GenericRequest.Deserialize(stream, instance, (long)-1);
		}

		public static GenericRequest Deserialize(Stream stream, GenericRequest instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			if (instance.PreviousRole == null)
			{
				instance.PreviousRole = new List<uint>();
			}
			if (instance.DesiredRole == null)
			{
				instance.DesiredRole = new List<uint>();
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
							if (instance.TargetId != null)
							{
								EntityId.DeserializeLengthDelimited(stream, instance.TargetId);
							}
							else
							{
								instance.TargetId = EntityId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 25)
						{
							instance.InvitationId = binaryReader.ReadUInt64();
						}
						else if (num1 == 34)
						{
							instance.InviteeName = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 42)
						{
							instance.InviterName = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 50)
						{
							long position = (long)ProtocolParser.ReadUInt32(stream);
							position = position + stream.Position;
							while (stream.Position < position)
							{
								instance.PreviousRole.Add(ProtocolParser.ReadUInt32(stream));
							}
							if (stream.Position != position)
							{
								throw new ProtocolBufferException("Read too many bytes in packed data");
							}
						}
						else if (num1 == 58)
						{
							long position1 = (long)ProtocolParser.ReadUInt32(stream);
							position1 = position1 + stream.Position;
							while (stream.Position < position1)
							{
								instance.DesiredRole.Add(ProtocolParser.ReadUInt32(stream));
							}
							if (stream.Position != position1)
							{
								throw new ProtocolBufferException("Read too many bytes in packed data");
							}
						}
						else if (num1 == 64)
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

		public static GenericRequest DeserializeLengthDelimited(Stream stream)
		{
			GenericRequest genericRequest = new GenericRequest();
			GenericRequest.DeserializeLengthDelimited(stream, genericRequest);
			return genericRequest;
		}

		public static GenericRequest DeserializeLengthDelimited(Stream stream, GenericRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GenericRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GenericRequest genericRequest = obj as GenericRequest;
			if (genericRequest == null)
			{
				return false;
			}
			if (this.HasAgentId != genericRequest.HasAgentId || this.HasAgentId && !this.AgentId.Equals(genericRequest.AgentId))
			{
				return false;
			}
			if (this.HasTargetId != genericRequest.HasTargetId || this.HasTargetId && !this.TargetId.Equals(genericRequest.TargetId))
			{
				return false;
			}
			if (!this.InvitationId.Equals(genericRequest.InvitationId))
			{
				return false;
			}
			if (this.HasInviteeName != genericRequest.HasInviteeName || this.HasInviteeName && !this.InviteeName.Equals(genericRequest.InviteeName))
			{
				return false;
			}
			if (this.HasInviterName != genericRequest.HasInviterName || this.HasInviterName && !this.InviterName.Equals(genericRequest.InviterName))
			{
				return false;
			}
			if (this.PreviousRole.Count != genericRequest.PreviousRole.Count)
			{
				return false;
			}
			for (int i = 0; i < this.PreviousRole.Count; i++)
			{
				if (!this.PreviousRole[i].Equals(genericRequest.PreviousRole[i]))
				{
					return false;
				}
			}
			if (this.DesiredRole.Count != genericRequest.DesiredRole.Count)
			{
				return false;
			}
			for (int j = 0; j < this.DesiredRole.Count; j++)
			{
				if (!this.DesiredRole[j].Equals(genericRequest.DesiredRole[j]))
				{
					return false;
				}
			}
			if (this.HasReason == genericRequest.HasReason && (!this.HasReason || this.Reason.Equals(genericRequest.Reason)))
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
				hashCode = hashCode ^ this.AgentId.GetHashCode();
			}
			if (this.HasTargetId)
			{
				hashCode = hashCode ^ this.TargetId.GetHashCode();
			}
			hashCode = hashCode ^ this.InvitationId.GetHashCode();
			if (this.HasInviteeName)
			{
				hashCode = hashCode ^ this.InviteeName.GetHashCode();
			}
			if (this.HasInviterName)
			{
				hashCode = hashCode ^ this.InviterName.GetHashCode();
			}
			foreach (uint previousRole in this.PreviousRole)
			{
				hashCode = hashCode ^ previousRole.GetHashCode();
			}
			foreach (uint desiredRole in this.DesiredRole)
			{
				hashCode = hashCode ^ desiredRole.GetHashCode();
			}
			if (this.HasReason)
			{
				hashCode = hashCode ^ this.Reason.GetHashCode();
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
			if (this.HasTargetId)
			{
				num++;
				uint serializedSize1 = this.TargetId.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			num = num + 8;
			if (this.HasInviteeName)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.InviteeName);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasInviterName)
			{
				num++;
				uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.InviterName);
				num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			}
			if (this.PreviousRole.Count > 0)
			{
				num++;
				uint num1 = num;
				foreach (uint previousRole in this.PreviousRole)
				{
					num = num + ProtocolParser.SizeOfUInt32(previousRole);
				}
				num = num + ProtocolParser.SizeOfUInt32(num - num1);
			}
			if (this.DesiredRole.Count > 0)
			{
				num++;
				uint num2 = num;
				foreach (uint desiredRole in this.DesiredRole)
				{
					num = num + ProtocolParser.SizeOfUInt32(desiredRole);
				}
				num = num + ProtocolParser.SizeOfUInt32(num - num2);
			}
			if (this.HasReason)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.Reason);
			}
			num++;
			return num;
		}

		public static GenericRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GenericRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GenericRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GenericRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasAgentId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentId.GetSerializedSize());
				EntityId.Serialize(stream, instance.AgentId);
			}
			if (instance.HasTargetId)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.TargetId.GetSerializedSize());
				EntityId.Serialize(stream, instance.TargetId);
			}
			stream.WriteByte(25);
			binaryWriter.Write(instance.InvitationId);
			if (instance.HasInviteeName)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.InviteeName));
			}
			if (instance.HasInviterName)
			{
				stream.WriteByte(42);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.InviterName));
			}
			if (instance.PreviousRole.Count > 0)
			{
				stream.WriteByte(50);
				uint num = 0;
				foreach (uint previousRole in instance.PreviousRole)
				{
					num = num + ProtocolParser.SizeOfUInt32(previousRole);
				}
				ProtocolParser.WriteUInt32(stream, num);
				foreach (uint previousRole1 in instance.PreviousRole)
				{
					ProtocolParser.WriteUInt32(stream, previousRole1);
				}
			}
			if (instance.DesiredRole.Count > 0)
			{
				stream.WriteByte(58);
				uint num1 = 0;
				foreach (uint desiredRole in instance.DesiredRole)
				{
					num1 = num1 + ProtocolParser.SizeOfUInt32(desiredRole);
				}
				ProtocolParser.WriteUInt32(stream, num1);
				foreach (uint desiredRole1 in instance.DesiredRole)
				{
					ProtocolParser.WriteUInt32(stream, desiredRole1);
				}
			}
			if (instance.HasReason)
			{
				stream.WriteByte(64);
				ProtocolParser.WriteUInt32(stream, instance.Reason);
			}
		}

		public void SetAgentId(EntityId val)
		{
			this.AgentId = val;
		}

		public void SetDesiredRole(List<uint> val)
		{
			this.DesiredRole = val;
		}

		public void SetInvitationId(ulong val)
		{
			this.InvitationId = val;
		}

		public void SetInviteeName(string val)
		{
			this.InviteeName = val;
		}

		public void SetInviterName(string val)
		{
			this.InviterName = val;
		}

		public void SetPreviousRole(List<uint> val)
		{
			this.PreviousRole = val;
		}

		public void SetReason(uint val)
		{
			this.Reason = val;
		}

		public void SetTargetId(EntityId val)
		{
			this.TargetId = val;
		}
	}
}