using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace bnet.protocol.friends
{
	public class FriendInvitationParams : IProtoBuf
	{
		public bool HasTargetEmail;

		private string _TargetEmail;

		public bool HasTargetBattleTag;

		private string _TargetBattleTag;

		public bool HasInviterBattleTag;

		private string _InviterBattleTag;

		public bool HasInviterFullName;

		private string _InviterFullName;

		public bool HasInviteeDisplayName;

		private string _InviteeDisplayName;

		private List<uint> _Role = new List<uint>();

		public string InviteeDisplayName
		{
			get
			{
				return this._InviteeDisplayName;
			}
			set
			{
				this._InviteeDisplayName = value;
				this.HasInviteeDisplayName = value != null;
			}
		}

		public string InviterBattleTag
		{
			get
			{
				return this._InviterBattleTag;
			}
			set
			{
				this._InviterBattleTag = value;
				this.HasInviterBattleTag = value != null;
			}
		}

		public string InviterFullName
		{
			get
			{
				return this._InviterFullName;
			}
			set
			{
				this._InviterFullName = value;
				this.HasInviterFullName = value != null;
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

		public string TargetBattleTag
		{
			get
			{
				return this._TargetBattleTag;
			}
			set
			{
				this._TargetBattleTag = value;
				this.HasTargetBattleTag = value != null;
			}
		}

		public string TargetEmail
		{
			get
			{
				return this._TargetEmail;
			}
			set
			{
				this._TargetEmail = value;
				this.HasTargetEmail = value != null;
			}
		}

		public FriendInvitationParams()
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
			FriendInvitationParams.Deserialize(stream, this);
		}

		public static FriendInvitationParams Deserialize(Stream stream, FriendInvitationParams instance)
		{
			return FriendInvitationParams.Deserialize(stream, instance, (long)-1);
		}

		public static FriendInvitationParams Deserialize(Stream stream, FriendInvitationParams instance, long limit)
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
						instance.TargetEmail = ProtocolParser.ReadString(stream);
					}
					else if (num == 18)
					{
						instance.TargetBattleTag = ProtocolParser.ReadString(stream);
					}
					else if (num == 26)
					{
						instance.InviterBattleTag = ProtocolParser.ReadString(stream);
					}
					else if (num == 34)
					{
						instance.InviterFullName = ProtocolParser.ReadString(stream);
					}
					else if (num == 42)
					{
						instance.InviteeDisplayName = ProtocolParser.ReadString(stream);
					}
					else if (num == 50)
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

		public static FriendInvitationParams DeserializeLengthDelimited(Stream stream)
		{
			FriendInvitationParams friendInvitationParam = new FriendInvitationParams();
			FriendInvitationParams.DeserializeLengthDelimited(stream, friendInvitationParam);
			return friendInvitationParam;
		}

		public static FriendInvitationParams DeserializeLengthDelimited(Stream stream, FriendInvitationParams instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return FriendInvitationParams.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			FriendInvitationParams friendInvitationParam = obj as FriendInvitationParams;
			if (friendInvitationParam == null)
			{
				return false;
			}
			if (this.HasTargetEmail != friendInvitationParam.HasTargetEmail || this.HasTargetEmail && !this.TargetEmail.Equals(friendInvitationParam.TargetEmail))
			{
				return false;
			}
			if (this.HasTargetBattleTag != friendInvitationParam.HasTargetBattleTag || this.HasTargetBattleTag && !this.TargetBattleTag.Equals(friendInvitationParam.TargetBattleTag))
			{
				return false;
			}
			if (this.HasInviterBattleTag != friendInvitationParam.HasInviterBattleTag || this.HasInviterBattleTag && !this.InviterBattleTag.Equals(friendInvitationParam.InviterBattleTag))
			{
				return false;
			}
			if (this.HasInviterFullName != friendInvitationParam.HasInviterFullName || this.HasInviterFullName && !this.InviterFullName.Equals(friendInvitationParam.InviterFullName))
			{
				return false;
			}
			if (this.HasInviteeDisplayName != friendInvitationParam.HasInviteeDisplayName || this.HasInviteeDisplayName && !this.InviteeDisplayName.Equals(friendInvitationParam.InviteeDisplayName))
			{
				return false;
			}
			if (this.Role.Count != friendInvitationParam.Role.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Role.Count; i++)
			{
				if (!this.Role[i].Equals(friendInvitationParam.Role[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasTargetEmail)
			{
				hashCode ^= this.TargetEmail.GetHashCode();
			}
			if (this.HasTargetBattleTag)
			{
				hashCode ^= this.TargetBattleTag.GetHashCode();
			}
			if (this.HasInviterBattleTag)
			{
				hashCode ^= this.InviterBattleTag.GetHashCode();
			}
			if (this.HasInviterFullName)
			{
				hashCode ^= this.InviterFullName.GetHashCode();
			}
			if (this.HasInviteeDisplayName)
			{
				hashCode ^= this.InviteeDisplayName.GetHashCode();
			}
			foreach (uint role in this.Role)
			{
				hashCode ^= role.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasTargetEmail)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.TargetEmail);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasTargetBattleTag)
			{
				num++;
				uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.TargetBattleTag);
				num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			}
			if (this.HasInviterBattleTag)
			{
				num++;
				uint num1 = (uint)Encoding.UTF8.GetByteCount(this.InviterBattleTag);
				num = num + ProtocolParser.SizeOfUInt32(num1) + num1;
			}
			if (this.HasInviterFullName)
			{
				num++;
				uint byteCount2 = (uint)Encoding.UTF8.GetByteCount(this.InviterFullName);
				num = num + ProtocolParser.SizeOfUInt32(byteCount2) + byteCount2;
			}
			if (this.HasInviteeDisplayName)
			{
				num++;
				uint num2 = (uint)Encoding.UTF8.GetByteCount(this.InviteeDisplayName);
				num = num + ProtocolParser.SizeOfUInt32(num2) + num2;
			}
			if (this.Role.Count > 0)
			{
				num++;
				uint num3 = num;
				foreach (uint role in this.Role)
				{
					num += ProtocolParser.SizeOfUInt32(role);
				}
				num += ProtocolParser.SizeOfUInt32(num - num3);
			}
			return num;
		}

		public static FriendInvitationParams ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<FriendInvitationParams>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			FriendInvitationParams.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, FriendInvitationParams instance)
		{
			if (instance.HasTargetEmail)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.TargetEmail));
			}
			if (instance.HasTargetBattleTag)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.TargetBattleTag));
			}
			if (instance.HasInviterBattleTag)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.InviterBattleTag));
			}
			if (instance.HasInviterFullName)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.InviterFullName));
			}
			if (instance.HasInviteeDisplayName)
			{
				stream.WriteByte(42);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.InviteeDisplayName));
			}
			if (instance.Role.Count > 0)
			{
				stream.WriteByte(50);
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

		public void SetInviteeDisplayName(string val)
		{
			this.InviteeDisplayName = val;
		}

		public void SetInviterBattleTag(string val)
		{
			this.InviterBattleTag = val;
		}

		public void SetInviterFullName(string val)
		{
			this.InviterFullName = val;
		}

		public void SetRole(List<uint> val)
		{
			this.Role = val;
		}

		public void SetTargetBattleTag(string val)
		{
			this.TargetBattleTag = val;
		}

		public void SetTargetEmail(string val)
		{
			this.TargetEmail = val;
		}
	}
}