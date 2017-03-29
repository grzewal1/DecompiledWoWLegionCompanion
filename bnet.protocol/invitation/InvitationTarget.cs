using bnet.protocol;
using System;
using System.IO;
using System.Text;

namespace bnet.protocol.invitation
{
	public class InvitationTarget : IProtoBuf
	{
		public bool HasIdentity;

		private bnet.protocol.Identity _Identity;

		public bool HasEmail;

		private string _Email;

		public bool HasBattleTag;

		private string _BattleTag;

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

		public string Email
		{
			get
			{
				return this._Email;
			}
			set
			{
				this._Email = value;
				this.HasEmail = value != null;
			}
		}

		public bnet.protocol.Identity Identity
		{
			get
			{
				return this._Identity;
			}
			set
			{
				this._Identity = value;
				this.HasIdentity = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public InvitationTarget()
		{
		}

		public void Deserialize(Stream stream)
		{
			InvitationTarget.Deserialize(stream, this);
		}

		public static InvitationTarget Deserialize(Stream stream, InvitationTarget instance)
		{
			return InvitationTarget.Deserialize(stream, instance, (long)-1);
		}

		public static InvitationTarget Deserialize(Stream stream, InvitationTarget instance, long limit)
		{
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
							if (instance.Identity != null)
							{
								bnet.protocol.Identity.DeserializeLengthDelimited(stream, instance.Identity);
							}
							else
							{
								instance.Identity = bnet.protocol.Identity.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 18)
						{
							instance.Email = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 26)
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

		public static InvitationTarget DeserializeLengthDelimited(Stream stream)
		{
			InvitationTarget invitationTarget = new InvitationTarget();
			InvitationTarget.DeserializeLengthDelimited(stream, invitationTarget);
			return invitationTarget;
		}

		public static InvitationTarget DeserializeLengthDelimited(Stream stream, InvitationTarget instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return InvitationTarget.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			InvitationTarget invitationTarget = obj as InvitationTarget;
			if (invitationTarget == null)
			{
				return false;
			}
			if (this.HasIdentity != invitationTarget.HasIdentity || this.HasIdentity && !this.Identity.Equals(invitationTarget.Identity))
			{
				return false;
			}
			if (this.HasEmail != invitationTarget.HasEmail || this.HasEmail && !this.Email.Equals(invitationTarget.Email))
			{
				return false;
			}
			if (this.HasBattleTag == invitationTarget.HasBattleTag && (!this.HasBattleTag || this.BattleTag.Equals(invitationTarget.BattleTag)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasIdentity)
			{
				hashCode = hashCode ^ this.Identity.GetHashCode();
			}
			if (this.HasEmail)
			{
				hashCode = hashCode ^ this.Email.GetHashCode();
			}
			if (this.HasBattleTag)
			{
				hashCode = hashCode ^ this.BattleTag.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasIdentity)
			{
				num++;
				uint serializedSize = this.Identity.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasEmail)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Email);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasBattleTag)
			{
				num++;
				uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.BattleTag);
				num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			}
			return num;
		}

		public static InvitationTarget ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<InvitationTarget>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			InvitationTarget.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, InvitationTarget instance)
		{
			if (instance.HasIdentity)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.Identity.GetSerializedSize());
				bnet.protocol.Identity.Serialize(stream, instance.Identity);
			}
			if (instance.HasEmail)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Email));
			}
			if (instance.HasBattleTag)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.BattleTag));
			}
		}

		public void SetBattleTag(string val)
		{
			this.BattleTag = val;
		}

		public void SetEmail(string val)
		{
			this.Email = val;
		}

		public void SetIdentity(bnet.protocol.Identity val)
		{
			this.Identity = val;
		}
	}
}