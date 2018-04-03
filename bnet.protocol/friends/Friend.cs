using bnet.protocol;
using bnet.protocol.attribute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.friends
{
	public class Friend : IProtoBuf
	{
		private List<bnet.protocol.attribute.Attribute> _Attribute = new List<bnet.protocol.attribute.Attribute>();

		private List<uint> _Role = new List<uint>();

		public bool HasPrivileges;

		private ulong _Privileges;

		public bool HasAttributesEpoch;

		private ulong _AttributesEpoch;

		public bool HasFullName;

		private string _FullName;

		public bool HasBattleTag;

		private string _BattleTag;

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

		public ulong AttributesEpoch
		{
			get
			{
				return this._AttributesEpoch;
			}
			set
			{
				this._AttributesEpoch = value;
				this.HasAttributesEpoch = true;
			}
		}

		public string BattleTag
		{
			get
			{
				return this._BattleTag;
			}
			set
			{
				this._BattleTag = value;
				this.HasBattleTag = value != null;
			}
		}

		public string FullName
		{
			get
			{
				return this._FullName;
			}
			set
			{
				this._FullName = value;
				this.HasFullName = value != null;
			}
		}

		public EntityId Id
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

		public ulong Privileges
		{
			get
			{
				return this._Privileges;
			}
			set
			{
				this._Privileges = value;
				this.HasPrivileges = true;
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

		public Friend()
		{
		}

		public void AddAttribute(bnet.protocol.attribute.Attribute val)
		{
			this._Attribute.Add(val);
		}

		public void AddRole(uint val)
		{
			this._Role.Add(val);
		}

		public void ClearAttribute()
		{
			this._Attribute.Clear();
		}

		public void ClearRole()
		{
			this._Role.Clear();
		}

		public void Deserialize(Stream stream)
		{
			Friend.Deserialize(stream, this);
		}

		public static Friend Deserialize(Stream stream, Friend instance)
		{
			return Friend.Deserialize(stream, instance, (long)-1);
		}

		public static Friend Deserialize(Stream stream, Friend instance, long limit)
		{
			if (instance.Attribute == null)
			{
				instance.Attribute = new List<bnet.protocol.attribute.Attribute>();
			}
			if (instance.Role == null)
			{
				instance.Role = new List<uint>();
			}
			instance.Privileges = (ulong)0;
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
						if (instance.Id != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.Id);
						}
						else
						{
							instance.Id = EntityId.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 18)
					{
						instance.Attribute.Add(bnet.protocol.attribute.Attribute.DeserializeLengthDelimited(stream));
					}
					else if (num == 26)
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
					else if (num == 32)
					{
						instance.Privileges = ProtocolParser.ReadUInt64(stream);
					}
					else if (num == 40)
					{
						instance.AttributesEpoch = ProtocolParser.ReadUInt64(stream);
					}
					else if (num == 50)
					{
						instance.FullName = ProtocolParser.ReadString(stream);
					}
					else if (num == 58)
					{
						instance.BattleTag = ProtocolParser.ReadString(stream);
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

		public static Friend DeserializeLengthDelimited(Stream stream)
		{
			Friend friend = new Friend();
			Friend.DeserializeLengthDelimited(stream, friend);
			return friend;
		}

		public static Friend DeserializeLengthDelimited(Stream stream, Friend instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return Friend.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			Friend friend = obj as Friend;
			if (friend == null)
			{
				return false;
			}
			if (!this.Id.Equals(friend.Id))
			{
				return false;
			}
			if (this.Attribute.Count != friend.Attribute.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Attribute.Count; i++)
			{
				if (!this.Attribute[i].Equals(friend.Attribute[i]))
				{
					return false;
				}
			}
			if (this.Role.Count != friend.Role.Count)
			{
				return false;
			}
			for (int j = 0; j < this.Role.Count; j++)
			{
				if (!this.Role[j].Equals(friend.Role[j]))
				{
					return false;
				}
			}
			if (this.HasPrivileges != friend.HasPrivileges || this.HasPrivileges && !this.Privileges.Equals(friend.Privileges))
			{
				return false;
			}
			if (this.HasAttributesEpoch != friend.HasAttributesEpoch || this.HasAttributesEpoch && !this.AttributesEpoch.Equals(friend.AttributesEpoch))
			{
				return false;
			}
			if (this.HasFullName != friend.HasFullName || this.HasFullName && !this.FullName.Equals(friend.FullName))
			{
				return false;
			}
			if (this.HasBattleTag == friend.HasBattleTag && (!this.HasBattleTag || this.BattleTag.Equals(friend.BattleTag)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Id.GetHashCode();
			foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
			{
				hashCode ^= attribute.GetHashCode();
			}
			foreach (uint role in this.Role)
			{
				hashCode ^= role.GetHashCode();
			}
			if (this.HasPrivileges)
			{
				hashCode ^= this.Privileges.GetHashCode();
			}
			if (this.HasAttributesEpoch)
			{
				hashCode ^= this.AttributesEpoch.GetHashCode();
			}
			if (this.HasFullName)
			{
				hashCode ^= this.FullName.GetHashCode();
			}
			if (this.HasBattleTag)
			{
				hashCode ^= this.BattleTag.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.Id.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
				{
					num++;
					uint serializedSize1 = attribute.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
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
			if (this.HasPrivileges)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.Privileges);
			}
			if (this.HasAttributesEpoch)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.AttributesEpoch);
			}
			if (this.HasFullName)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.FullName);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasBattleTag)
			{
				num++;
				uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.BattleTag);
				num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			}
			num++;
			return num;
		}

		public static Friend ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<Friend>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			Friend.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, Friend instance)
		{
			if (instance.Id == null)
			{
				throw new ArgumentNullException("Id", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.Id.GetSerializedSize());
			EntityId.Serialize(stream, instance.Id);
			if (instance.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in instance.Attribute)
				{
					stream.WriteByte(18);
					ProtocolParser.WriteUInt32(stream, attribute.GetSerializedSize());
					bnet.protocol.attribute.Attribute.Serialize(stream, attribute);
				}
			}
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
			if (instance.HasPrivileges)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteUInt64(stream, instance.Privileges);
			}
			if (instance.HasAttributesEpoch)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteUInt64(stream, instance.AttributesEpoch);
			}
			if (instance.HasFullName)
			{
				stream.WriteByte(50);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.FullName));
			}
			if (instance.HasBattleTag)
			{
				stream.WriteByte(58);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.BattleTag));
			}
		}

		public void SetAttribute(List<bnet.protocol.attribute.Attribute> val)
		{
			this.Attribute = val;
		}

		public void SetAttributesEpoch(ulong val)
		{
			this.AttributesEpoch = val;
		}

		public void SetBattleTag(string val)
		{
			this.BattleTag = val;
		}

		public void SetFullName(string val)
		{
			this.FullName = val;
		}

		public void SetId(EntityId val)
		{
			this.Id = val;
		}

		public void SetPrivileges(ulong val)
		{
			this.Privileges = val;
		}

		public void SetRole(List<uint> val)
		{
			this.Role = val;
		}
	}
}