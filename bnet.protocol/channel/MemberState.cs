using bnet.protocol;
using bnet.protocol.attribute;
using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.channel
{
	public class MemberState : IProtoBuf
	{
		private List<bnet.protocol.attribute.Attribute> _Attribute = new List<bnet.protocol.attribute.Attribute>();

		private List<uint> _Role = new List<uint>();

		public bool HasPrivileges;

		private ulong _Privileges;

		public bool HasInfo;

		private AccountInfo _Info;

		public bool HasHidden;

		private bool _Hidden;

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

		public bool Hidden
		{
			get
			{
				return this._Hidden;
			}
			set
			{
				this._Hidden = value;
				this.HasHidden = true;
			}
		}

		public AccountInfo Info
		{
			get
			{
				return this._Info;
			}
			set
			{
				this._Info = value;
				this.HasInfo = value != null;
			}
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

		public MemberState()
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
			MemberState.Deserialize(stream, this);
		}

		public static MemberState Deserialize(Stream stream, MemberState instance)
		{
			return MemberState.Deserialize(stream, instance, (long)-1);
		}

		public static MemberState Deserialize(Stream stream, MemberState instance, long limit)
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
			instance.Hidden = false;
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
							instance.Attribute.Add(bnet.protocol.attribute.Attribute.DeserializeLengthDelimited(stream));
						}
						else if (num1 == 18)
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
						else if (num1 == 24)
						{
							instance.Privileges = ProtocolParser.ReadUInt64(stream);
						}
						else if (num1 != 34)
						{
							if (num1 == 40)
							{
								instance.Hidden = ProtocolParser.ReadBool(stream);
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
						else if (instance.Info != null)
						{
							AccountInfo.DeserializeLengthDelimited(stream, instance.Info);
						}
						else
						{
							instance.Info = AccountInfo.DeserializeLengthDelimited(stream);
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

		public static MemberState DeserializeLengthDelimited(Stream stream)
		{
			MemberState memberState = new MemberState();
			MemberState.DeserializeLengthDelimited(stream, memberState);
			return memberState;
		}

		public static MemberState DeserializeLengthDelimited(Stream stream, MemberState instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return MemberState.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			MemberState memberState = obj as MemberState;
			if (memberState == null)
			{
				return false;
			}
			if (this.Attribute.Count != memberState.Attribute.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Attribute.Count; i++)
			{
				if (!this.Attribute[i].Equals(memberState.Attribute[i]))
				{
					return false;
				}
			}
			if (this.Role.Count != memberState.Role.Count)
			{
				return false;
			}
			for (int j = 0; j < this.Role.Count; j++)
			{
				if (!this.Role[j].Equals(memberState.Role[j]))
				{
					return false;
				}
			}
			if (this.HasPrivileges != memberState.HasPrivileges || this.HasPrivileges && !this.Privileges.Equals(memberState.Privileges))
			{
				return false;
			}
			if (this.HasInfo != memberState.HasInfo || this.HasInfo && !this.Info.Equals(memberState.Info))
			{
				return false;
			}
			if (this.HasHidden == memberState.HasHidden && (!this.HasHidden || this.Hidden.Equals(memberState.Hidden)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
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
			if (this.HasInfo)
			{
				hashCode ^= this.Info.GetHashCode();
			}
			if (this.HasHidden)
			{
				hashCode ^= this.Hidden.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
				{
					num++;
					uint serializedSize = attribute.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
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
			if (this.HasInfo)
			{
				num++;
				uint serializedSize1 = this.Info.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			if (this.HasHidden)
			{
				num++;
				num++;
			}
			return num;
		}

		public static MemberState ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<MemberState>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			MemberState.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, MemberState instance)
		{
			if (instance.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in instance.Attribute)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, attribute.GetSerializedSize());
					bnet.protocol.attribute.Attribute.Serialize(stream, attribute);
				}
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
			if (instance.HasPrivileges)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt64(stream, instance.Privileges);
			}
			if (instance.HasInfo)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteUInt32(stream, instance.Info.GetSerializedSize());
				AccountInfo.Serialize(stream, instance.Info);
			}
			if (instance.HasHidden)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteBool(stream, instance.Hidden);
			}
		}

		public void SetAttribute(List<bnet.protocol.attribute.Attribute> val)
		{
			this.Attribute = val;
		}

		public void SetHidden(bool val)
		{
			this.Hidden = val;
		}

		public void SetInfo(AccountInfo val)
		{
			this.Info = val;
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