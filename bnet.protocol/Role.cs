using bnet.protocol.attribute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol
{
	public class Role : IProtoBuf
	{
		private List<string> _Privilege = new List<string>();

		private List<uint> _AssignableRole = new List<uint>();

		public bool HasRequired;

		private bool _Required;

		public bool HasUnique;

		private bool _Unique;

		public bool HasRelegationRole;

		private uint _RelegationRole;

		private List<bnet.protocol.attribute.Attribute> _Attribute = new List<bnet.protocol.attribute.Attribute>();

		public List<uint> AssignableRole
		{
			get
			{
				return this._AssignableRole;
			}
			set
			{
				this._AssignableRole = value;
			}
		}

		public int AssignableRoleCount
		{
			get
			{
				return this._AssignableRole.Count;
			}
		}

		public List<uint> AssignableRoleList
		{
			get
			{
				return this._AssignableRole;
			}
		}

		public List<bnet.protocol.attribute.Attribute> Attribute
		{
			get
			{
				return this._Attribute;
			}
			set
			{
				this._Attribute = value;
			}
		}

		public int AttributeCount
		{
			get
			{
				return this._Attribute.Count;
			}
		}

		public List<bnet.protocol.attribute.Attribute> AttributeList
		{
			get
			{
				return this._Attribute;
			}
		}

		public uint Id
		{
			get;
			set;
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public string Name
		{
			get;
			set;
		}

		public List<string> Privilege
		{
			get
			{
				return this._Privilege;
			}
			set
			{
				this._Privilege = value;
			}
		}

		public int PrivilegeCount
		{
			get
			{
				return this._Privilege.Count;
			}
		}

		public List<string> PrivilegeList
		{
			get
			{
				return this._Privilege;
			}
		}

		public uint RelegationRole
		{
			get
			{
				return this._RelegationRole;
			}
			set
			{
				this._RelegationRole = value;
				this.HasRelegationRole = true;
			}
		}

		public bool Required
		{
			get
			{
				return this._Required;
			}
			set
			{
				this._Required = value;
				this.HasRequired = true;
			}
		}

		public bool Unique
		{
			get
			{
				return this._Unique;
			}
			set
			{
				this._Unique = value;
				this.HasUnique = true;
			}
		}

		public Role()
		{
		}

		public void AddAssignableRole(uint val)
		{
			this._AssignableRole.Add(val);
		}

		public void AddAttribute(bnet.protocol.attribute.Attribute val)
		{
			this._Attribute.Add(val);
		}

		public void AddPrivilege(string val)
		{
			this._Privilege.Add(val);
		}

		public void ClearAssignableRole()
		{
			this._AssignableRole.Clear();
		}

		public void ClearAttribute()
		{
			this._Attribute.Clear();
		}

		public void ClearPrivilege()
		{
			this._Privilege.Clear();
		}

		public void Deserialize(Stream stream)
		{
			Role.Deserialize(stream, this);
		}

		public static Role Deserialize(Stream stream, Role instance)
		{
			return Role.Deserialize(stream, instance, (long)-1);
		}

		public static Role Deserialize(Stream stream, Role instance, long limit)
		{
			if (instance.Privilege == null)
			{
				instance.Privilege = new List<string>();
			}
			if (instance.AssignableRole == null)
			{
				instance.AssignableRole = new List<uint>();
			}
			instance.Required = false;
			instance.Unique = false;
			if (instance.Attribute == null)
			{
				instance.Attribute = new List<bnet.protocol.attribute.Attribute>();
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
						instance.Id = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 18)
					{
						instance.Name = ProtocolParser.ReadString(stream);
					}
					else if (num == 26)
					{
						instance.Privilege.Add(ProtocolParser.ReadString(stream));
					}
					else if (num == 34)
					{
						long position = (long)ProtocolParser.ReadUInt32(stream);
						position += stream.Position;
						while (stream.Position < position)
						{
							instance.AssignableRole.Add(ProtocolParser.ReadUInt32(stream));
						}
						if (stream.Position != position)
						{
							throw new ProtocolBufferException("Read too many bytes in packed data");
						}
					}
					else if (num == 40)
					{
						instance.Required = ProtocolParser.ReadBool(stream);
					}
					else if (num == 48)
					{
						instance.Unique = ProtocolParser.ReadBool(stream);
					}
					else if (num == 56)
					{
						instance.RelegationRole = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 66)
					{
						instance.Attribute.Add(bnet.protocol.attribute.Attribute.DeserializeLengthDelimited(stream));
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

		public static Role DeserializeLengthDelimited(Stream stream)
		{
			Role role = new Role();
			Role.DeserializeLengthDelimited(stream, role);
			return role;
		}

		public static Role DeserializeLengthDelimited(Stream stream, Role instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return Role.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			Role role = obj as Role;
			if (role == null)
			{
				return false;
			}
			if (!this.Id.Equals(role.Id))
			{
				return false;
			}
			if (!this.Name.Equals(role.Name))
			{
				return false;
			}
			if (this.Privilege.Count != role.Privilege.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Privilege.Count; i++)
			{
				if (!this.Privilege[i].Equals(role.Privilege[i]))
				{
					return false;
				}
			}
			if (this.AssignableRole.Count != role.AssignableRole.Count)
			{
				return false;
			}
			for (int j = 0; j < this.AssignableRole.Count; j++)
			{
				if (!this.AssignableRole[j].Equals(role.AssignableRole[j]))
				{
					return false;
				}
			}
			if (this.HasRequired != role.HasRequired || this.HasRequired && !this.Required.Equals(role.Required))
			{
				return false;
			}
			if (this.HasUnique != role.HasUnique || this.HasUnique && !this.Unique.Equals(role.Unique))
			{
				return false;
			}
			if (this.HasRelegationRole != role.HasRelegationRole || this.HasRelegationRole && !this.RelegationRole.Equals(role.RelegationRole))
			{
				return false;
			}
			if (this.Attribute.Count != role.Attribute.Count)
			{
				return false;
			}
			for (int k = 0; k < this.Attribute.Count; k++)
			{
				if (!this.Attribute[k].Equals(role.Attribute[k]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Id.GetHashCode();
			hashCode ^= this.Name.GetHashCode();
			foreach (string privilege in this.Privilege)
			{
				hashCode ^= privilege.GetHashCode();
			}
			foreach (uint assignableRole in this.AssignableRole)
			{
				hashCode ^= assignableRole.GetHashCode();
			}
			if (this.HasRequired)
			{
				hashCode ^= this.Required.GetHashCode();
			}
			if (this.HasUnique)
			{
				hashCode ^= this.Unique.GetHashCode();
			}
			if (this.HasRelegationRole)
			{
				hashCode ^= this.RelegationRole.GetHashCode();
			}
			foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
			{
				hashCode ^= attribute.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num += ProtocolParser.SizeOfUInt32(this.Id);
			uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Name);
			num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			if (this.Privilege.Count > 0)
			{
				foreach (string privilege in this.Privilege)
				{
					num++;
					uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(privilege);
					num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
				}
			}
			if (this.AssignableRole.Count > 0)
			{
				num++;
				uint num1 = num;
				foreach (uint assignableRole in this.AssignableRole)
				{
					num += ProtocolParser.SizeOfUInt32(assignableRole);
				}
				num += ProtocolParser.SizeOfUInt32(num - num1);
			}
			if (this.HasRequired)
			{
				num++;
				num++;
			}
			if (this.HasUnique)
			{
				num++;
				num++;
			}
			if (this.HasRelegationRole)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.RelegationRole);
			}
			if (this.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
				{
					num++;
					uint serializedSize = attribute.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			num += 2;
			return num;
		}

		public static Role ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<Role>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			Role.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, Role instance)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.Id);
			if (instance.Name == null)
			{
				throw new ArgumentNullException("Name", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Name));
			if (instance.Privilege.Count > 0)
			{
				foreach (string privilege in instance.Privilege)
				{
					stream.WriteByte(26);
					ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(privilege));
				}
			}
			if (instance.AssignableRole.Count > 0)
			{
				stream.WriteByte(34);
				uint num = 0;
				foreach (uint assignableRole in instance.AssignableRole)
				{
					num += ProtocolParser.SizeOfUInt32(assignableRole);
				}
				ProtocolParser.WriteUInt32(stream, num);
				foreach (uint assignableRole1 in instance.AssignableRole)
				{
					ProtocolParser.WriteUInt32(stream, assignableRole1);
				}
			}
			if (instance.HasRequired)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteBool(stream, instance.Required);
			}
			if (instance.HasUnique)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteBool(stream, instance.Unique);
			}
			if (instance.HasRelegationRole)
			{
				stream.WriteByte(56);
				ProtocolParser.WriteUInt32(stream, instance.RelegationRole);
			}
			if (instance.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in instance.Attribute)
				{
					stream.WriteByte(66);
					ProtocolParser.WriteUInt32(stream, attribute.GetSerializedSize());
					bnet.protocol.attribute.Attribute.Serialize(stream, attribute);
				}
			}
		}

		public void SetAssignableRole(List<uint> val)
		{
			this.AssignableRole = val;
		}

		public void SetAttribute(List<bnet.protocol.attribute.Attribute> val)
		{
			this.Attribute = val;
		}

		public void SetId(uint val)
		{
			this.Id = val;
		}

		public void SetName(string val)
		{
			this.Name = val;
		}

		public void SetPrivilege(List<string> val)
		{
			this.Privilege = val;
		}

		public void SetRelegationRole(uint val)
		{
			this.RelegationRole = val;
		}

		public void SetRequired(bool val)
		{
			this.Required = val;
		}

		public void SetUnique(bool val)
		{
			this.Unique = val;
		}
	}
}