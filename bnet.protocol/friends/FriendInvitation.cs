using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.friends
{
	public class FriendInvitation : IProtoBuf
	{
		public bool HasFirstReceived;

		private bool _FirstReceived;

		private List<uint> _Role = new List<uint>();

		public bool FirstReceived
		{
			get
			{
				return this._FirstReceived;
			}
			set
			{
				this._FirstReceived = value;
				this.HasFirstReceived = true;
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

		public FriendInvitation()
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
			FriendInvitation.Deserialize(stream, this);
		}

		public static FriendInvitation Deserialize(Stream stream, FriendInvitation instance)
		{
			return FriendInvitation.Deserialize(stream, instance, (long)-1);
		}

		public static FriendInvitation Deserialize(Stream stream, FriendInvitation instance, long limit)
		{
			instance.FirstReceived = false;
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
						if (num1 == 8)
						{
							instance.FirstReceived = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 18)
						{
							long position = (long)ProtocolParser.ReadUInt32(stream);
							position = position + stream.Position;
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

		public static FriendInvitation DeserializeLengthDelimited(Stream stream)
		{
			FriendInvitation friendInvitation = new FriendInvitation();
			FriendInvitation.DeserializeLengthDelimited(stream, friendInvitation);
			return friendInvitation;
		}

		public static FriendInvitation DeserializeLengthDelimited(Stream stream, FriendInvitation instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return FriendInvitation.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			FriendInvitation friendInvitation = obj as FriendInvitation;
			if (friendInvitation == null)
			{
				return false;
			}
			if (this.HasFirstReceived != friendInvitation.HasFirstReceived || this.HasFirstReceived && !this.FirstReceived.Equals(friendInvitation.FirstReceived))
			{
				return false;
			}
			if (this.Role.Count != friendInvitation.Role.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Role.Count; i++)
			{
				if (!this.Role[i].Equals(friendInvitation.Role[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasFirstReceived)
			{
				hashCode = hashCode ^ this.FirstReceived.GetHashCode();
			}
			foreach (uint role in this.Role)
			{
				hashCode = hashCode ^ role.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasFirstReceived)
			{
				num++;
				num++;
			}
			if (this.Role.Count > 0)
			{
				num++;
				uint num1 = num;
				foreach (uint role in this.Role)
				{
					num = num + ProtocolParser.SizeOfUInt32(role);
				}
				num = num + ProtocolParser.SizeOfUInt32(num - num1);
			}
			return num;
		}

		public static FriendInvitation ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<FriendInvitation>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			FriendInvitation.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, FriendInvitation instance)
		{
			if (instance.HasFirstReceived)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteBool(stream, instance.FirstReceived);
			}
			if (instance.Role.Count > 0)
			{
				stream.WriteByte(18);
				uint num = 0;
				foreach (uint role in instance.Role)
				{
					num = num + ProtocolParser.SizeOfUInt32(role);
				}
				ProtocolParser.WriteUInt32(stream, num);
				foreach (uint role1 in instance.Role)
				{
					ProtocolParser.WriteUInt32(stream, role1);
				}
			}
		}

		public void SetFirstReceived(bool val)
		{
			this.FirstReceived = val;
		}

		public void SetRole(List<uint> val)
		{
			this.Role = val;
		}
	}
}