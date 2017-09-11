using bnet.protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.friends
{
	public class ViewFriendsRequest : IProtoBuf
	{
		public bool HasAgentId;

		private EntityId _AgentId;

		private List<uint> _Role = new List<uint>();

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

		public EntityId TargetId
		{
			get;
			set;
		}

		public ViewFriendsRequest()
		{
		}

		public void AddRole(uint val)
		{
			this._Role.Add(val);
		}

		public void ClearRole()
		{
			this._Role.Clear();
		}

		public void Deserialize(Stream stream)
		{
			ViewFriendsRequest.Deserialize(stream, this);
		}

		public static ViewFriendsRequest Deserialize(Stream stream, ViewFriendsRequest instance)
		{
			return ViewFriendsRequest.Deserialize(stream, instance, (long)-1);
		}

		public static ViewFriendsRequest Deserialize(Stream stream, ViewFriendsRequest instance, long limit)
		{
			if (instance.Role == null)
			{
				instance.Role = new List<uint>();
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
							if (num1 == 26)
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

		public static ViewFriendsRequest DeserializeLengthDelimited(Stream stream)
		{
			ViewFriendsRequest viewFriendsRequest = new ViewFriendsRequest();
			ViewFriendsRequest.DeserializeLengthDelimited(stream, viewFriendsRequest);
			return viewFriendsRequest;
		}

		public static ViewFriendsRequest DeserializeLengthDelimited(Stream stream, ViewFriendsRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ViewFriendsRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ViewFriendsRequest viewFriendsRequest = obj as ViewFriendsRequest;
			if (viewFriendsRequest == null)
			{
				return false;
			}
			if (this.HasAgentId != viewFriendsRequest.HasAgentId || this.HasAgentId && !this.AgentId.Equals(viewFriendsRequest.AgentId))
			{
				return false;
			}
			if (!this.TargetId.Equals(viewFriendsRequest.TargetId))
			{
				return false;
			}
			if (this.Role.Count != viewFriendsRequest.Role.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Role.Count; i++)
			{
				if (!this.Role[i].Equals(viewFriendsRequest.Role[i]))
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
			foreach (uint role in this.Role)
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
				num++;
				uint num1 = num;
				foreach (uint role in this.Role)
				{
					num += ProtocolParser.SizeOfUInt32(role);
				}
				num += ProtocolParser.SizeOfUInt32(num - num1);
			}
			num++;
			return num;
		}

		public static ViewFriendsRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ViewFriendsRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ViewFriendsRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ViewFriendsRequest instance)
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
				stream.WriteByte(26);
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
		}

		public void SetAgentId(EntityId val)
		{
			this.AgentId = val;
		}

		public void SetRole(List<uint> val)
		{
			this.Role = val;
		}

		public void SetTargetId(EntityId val)
		{
			this.TargetId = val;
		}
	}
}