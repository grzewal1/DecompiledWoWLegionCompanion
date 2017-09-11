using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol
{
	public class RoleSetConfig : IProtoBuf
	{
		private List<bnet.protocol.Privilege> _Privilege = new List<bnet.protocol.Privilege>();

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public List<bnet.protocol.Privilege> Privilege
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

		public List<bnet.protocol.Privilege> PrivilegeList
		{
			get
			{
				return this._Privilege;
			}
		}

		public bnet.protocol.RoleSet RoleSet
		{
			get;
			set;
		}

		public RoleSetConfig()
		{
		}

		public void AddPrivilege(bnet.protocol.Privilege val)
		{
			this._Privilege.Add(val);
		}

		public void ClearPrivilege()
		{
			this._Privilege.Clear();
		}

		public void Deserialize(Stream stream)
		{
			RoleSetConfig.Deserialize(stream, this);
		}

		public static RoleSetConfig Deserialize(Stream stream, RoleSetConfig instance)
		{
			return RoleSetConfig.Deserialize(stream, instance, (long)-1);
		}

		public static RoleSetConfig Deserialize(Stream stream, RoleSetConfig instance, long limit)
		{
			if (instance.Privilege == null)
			{
				instance.Privilege = new List<bnet.protocol.Privilege>();
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
							instance.Privilege.Add(bnet.protocol.Privilege.DeserializeLengthDelimited(stream));
						}
						else if (num1 != 18)
						{
							Key key = ProtocolParser.ReadKey((byte)num, stream);
							if (key.Field == 0)
							{
								throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
							}
							ProtocolParser.SkipKey(stream, key);
						}
						else if (instance.RoleSet != null)
						{
							bnet.protocol.RoleSet.DeserializeLengthDelimited(stream, instance.RoleSet);
						}
						else
						{
							instance.RoleSet = bnet.protocol.RoleSet.DeserializeLengthDelimited(stream);
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

		public static RoleSetConfig DeserializeLengthDelimited(Stream stream)
		{
			RoleSetConfig roleSetConfig = new RoleSetConfig();
			RoleSetConfig.DeserializeLengthDelimited(stream, roleSetConfig);
			return roleSetConfig;
		}

		public static RoleSetConfig DeserializeLengthDelimited(Stream stream, RoleSetConfig instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return RoleSetConfig.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			RoleSetConfig roleSetConfig = obj as RoleSetConfig;
			if (roleSetConfig == null)
			{
				return false;
			}
			if (this.Privilege.Count != roleSetConfig.Privilege.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Privilege.Count; i++)
			{
				if (!this.Privilege[i].Equals(roleSetConfig.Privilege[i]))
				{
					return false;
				}
			}
			if (!this.RoleSet.Equals(roleSetConfig.RoleSet))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (bnet.protocol.Privilege privilege in this.Privilege)
			{
				hashCode ^= privilege.GetHashCode();
			}
			hashCode ^= this.RoleSet.GetHashCode();
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Privilege.Count > 0)
			{
				foreach (bnet.protocol.Privilege privilege in this.Privilege)
				{
					num++;
					uint serializedSize = privilege.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			uint serializedSize1 = this.RoleSet.GetSerializedSize();
			num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			num++;
			return num;
		}

		public static RoleSetConfig ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<RoleSetConfig>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			RoleSetConfig.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, RoleSetConfig instance)
		{
			if (instance.Privilege.Count > 0)
			{
				foreach (bnet.protocol.Privilege privilege in instance.Privilege)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, privilege.GetSerializedSize());
					bnet.protocol.Privilege.Serialize(stream, privilege);
				}
			}
			if (instance.RoleSet == null)
			{
				throw new ArgumentNullException("RoleSet", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.RoleSet.GetSerializedSize());
			bnet.protocol.RoleSet.Serialize(stream, instance.RoleSet);
		}

		public void SetPrivilege(List<bnet.protocol.Privilege> val)
		{
			this.Privilege = val;
		}

		public void SetRoleSet(bnet.protocol.RoleSet val)
		{
			this.RoleSet = val;
		}
	}
}