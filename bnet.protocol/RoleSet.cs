using bnet.protocol.attribute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol
{
	public class RoleSet : IProtoBuf
	{
		public bool HasSubtype;

		private string _Subtype;

		private List<bnet.protocol.Role> _Role = new List<bnet.protocol.Role>();

		private List<uint> _DefaultRole = new List<uint>();

		public bool HasMaxMembers;

		private int _MaxMembers;

		private List<bnet.protocol.attribute.Attribute> _Attribute = new List<bnet.protocol.attribute.Attribute>();

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

		public List<uint> DefaultRole
		{
			get
			{
				return this._DefaultRole;
			}
			set
			{
				this._DefaultRole = value;
			}
		}

		public int DefaultRoleCount
		{
			get
			{
				return this._DefaultRole.Count;
			}
		}

		public List<uint> DefaultRoleList
		{
			get
			{
				return this._DefaultRole;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public int MaxMembers
		{
			get
			{
				return this._MaxMembers;
			}
			set
			{
				this._MaxMembers = value;
				this.HasMaxMembers = true;
			}
		}

		public string Program
		{
			get;
			set;
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

		public string Service
		{
			get;
			set;
		}

		public string Subtype
		{
			get
			{
				return this._Subtype;
			}
			set
			{
				this._Subtype = value;
				this.HasSubtype = value != null;
			}
		}

		public RoleSet()
		{
		}

		public void AddAttribute(bnet.protocol.attribute.Attribute val)
		{
			this._Attribute.Add(val);
		}

		public void AddDefaultRole(uint val)
		{
			this._DefaultRole.Add(val);
		}

		public void AddRole(bnet.protocol.Role val)
		{
			this._Role.Add(val);
		}

		public void ClearAttribute()
		{
			this._Attribute.Clear();
		}

		public void ClearDefaultRole()
		{
			this._DefaultRole.Clear();
		}

		public void ClearRole()
		{
			this._Role.Clear();
		}

		public void Deserialize(Stream stream)
		{
			RoleSet.Deserialize(stream, this);
		}

		public static RoleSet Deserialize(Stream stream, RoleSet instance)
		{
			return RoleSet.Deserialize(stream, instance, (long)-1);
		}

		public static RoleSet Deserialize(Stream stream, RoleSet instance, long limit)
		{
			instance.Subtype = "default";
			if (instance.Role == null)
			{
				instance.Role = new List<bnet.protocol.Role>();
			}
			if (instance.DefaultRole == null)
			{
				instance.DefaultRole = new List<uint>();
			}
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
					else if (num == 10)
					{
						instance.Program = ProtocolParser.ReadString(stream);
					}
					else if (num == 18)
					{
						instance.Service = ProtocolParser.ReadString(stream);
					}
					else if (num == 26)
					{
						instance.Subtype = ProtocolParser.ReadString(stream);
					}
					else if (num == 34)
					{
						instance.Role.Add(bnet.protocol.Role.DeserializeLengthDelimited(stream));
					}
					else if (num == 42)
					{
						long position = (long)ProtocolParser.ReadUInt32(stream);
						position += stream.Position;
						while (stream.Position < position)
						{
							instance.DefaultRole.Add(ProtocolParser.ReadUInt32(stream));
						}
						if (stream.Position != position)
						{
							throw new ProtocolBufferException("Read too many bytes in packed data");
						}
					}
					else if (num == 48)
					{
						instance.MaxMembers = (int)ProtocolParser.ReadUInt64(stream);
					}
					else if (num == 58)
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

		public static RoleSet DeserializeLengthDelimited(Stream stream)
		{
			RoleSet roleSet = new RoleSet();
			RoleSet.DeserializeLengthDelimited(stream, roleSet);
			return roleSet;
		}

		public static RoleSet DeserializeLengthDelimited(Stream stream, RoleSet instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return RoleSet.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			RoleSet roleSet = obj as RoleSet;
			if (roleSet == null)
			{
				return false;
			}
			if (!this.Program.Equals(roleSet.Program))
			{
				return false;
			}
			if (!this.Service.Equals(roleSet.Service))
			{
				return false;
			}
			if (this.HasSubtype != roleSet.HasSubtype || this.HasSubtype && !this.Subtype.Equals(roleSet.Subtype))
			{
				return false;
			}
			if (this.Role.Count != roleSet.Role.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Role.Count; i++)
			{
				if (!this.Role[i].Equals(roleSet.Role[i]))
				{
					return false;
				}
			}
			if (this.DefaultRole.Count != roleSet.DefaultRole.Count)
			{
				return false;
			}
			for (int j = 0; j < this.DefaultRole.Count; j++)
			{
				if (!this.DefaultRole[j].Equals(roleSet.DefaultRole[j]))
				{
					return false;
				}
			}
			if (this.HasMaxMembers != roleSet.HasMaxMembers || this.HasMaxMembers && !this.MaxMembers.Equals(roleSet.MaxMembers))
			{
				return false;
			}
			if (this.Attribute.Count != roleSet.Attribute.Count)
			{
				return false;
			}
			for (int k = 0; k < this.Attribute.Count; k++)
			{
				if (!this.Attribute[k].Equals(roleSet.Attribute[k]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Program.GetHashCode();
			hashCode ^= this.Service.GetHashCode();
			if (this.HasSubtype)
			{
				hashCode ^= this.Subtype.GetHashCode();
			}
			foreach (bnet.protocol.Role role in this.Role)
			{
				hashCode ^= role.GetHashCode();
			}
			foreach (uint defaultRole in this.DefaultRole)
			{
				hashCode ^= defaultRole.GetHashCode();
			}
			if (this.HasMaxMembers)
			{
				hashCode ^= this.MaxMembers.GetHashCode();
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
			uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Program);
			num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.Service);
			num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			if (this.HasSubtype)
			{
				num++;
				uint num1 = (uint)Encoding.UTF8.GetByteCount(this.Subtype);
				num = num + ProtocolParser.SizeOfUInt32(num1) + num1;
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
			if (this.DefaultRole.Count > 0)
			{
				num++;
				uint num2 = num;
				foreach (uint defaultRole in this.DefaultRole)
				{
					num += ProtocolParser.SizeOfUInt32(defaultRole);
				}
				num += ProtocolParser.SizeOfUInt32(num - num2);
			}
			if (this.HasMaxMembers)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64((ulong)this.MaxMembers);
			}
			if (this.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
				{
					num++;
					uint serializedSize1 = attribute.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			num += 2;
			return num;
		}

		public static RoleSet ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<RoleSet>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			RoleSet.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, RoleSet instance)
		{
			if (instance.Program == null)
			{
				throw new ArgumentNullException("Program", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Program));
			if (instance.Service == null)
			{
				throw new ArgumentNullException("Service", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Service));
			if (instance.HasSubtype)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Subtype));
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
			if (instance.DefaultRole.Count > 0)
			{
				stream.WriteByte(42);
				uint num = 0;
				foreach (uint defaultRole in instance.DefaultRole)
				{
					num += ProtocolParser.SizeOfUInt32(defaultRole);
				}
				ProtocolParser.WriteUInt32(stream, num);
				foreach (uint defaultRole1 in instance.DefaultRole)
				{
					ProtocolParser.WriteUInt32(stream, defaultRole1);
				}
			}
			if (instance.HasMaxMembers)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.MaxMembers);
			}
			if (instance.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in instance.Attribute)
				{
					stream.WriteByte(58);
					ProtocolParser.WriteUInt32(stream, attribute.GetSerializedSize());
					bnet.protocol.attribute.Attribute.Serialize(stream, attribute);
				}
			}
		}

		public void SetAttribute(List<bnet.protocol.attribute.Attribute> val)
		{
			this.Attribute = val;
		}

		public void SetDefaultRole(List<uint> val)
		{
			this.DefaultRole = val;
		}

		public void SetMaxMembers(int val)
		{
			this.MaxMembers = val;
		}

		public void SetProgram(string val)
		{
			this.Program = val;
		}

		public void SetRole(List<bnet.protocol.Role> val)
		{
			this.Role = val;
		}

		public void SetService(string val)
		{
			this.Service = val;
		}

		public void SetSubtype(string val)
		{
			this.Subtype = val;
		}
	}
}