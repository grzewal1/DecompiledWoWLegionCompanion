using bnet.protocol;
using bnet.protocol.attribute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.notification
{
	public class Notification : IProtoBuf
	{
		public bool HasSenderId;

		private EntityId _SenderId;

		private List<bnet.protocol.attribute.Attribute> _Attribute = new List<bnet.protocol.attribute.Attribute>();

		public bool HasSenderAccountId;

		private EntityId _SenderAccountId;

		public bool HasTargetAccountId;

		private EntityId _TargetAccountId;

		public bool HasSenderBattleTag;

		private string _SenderBattleTag;

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

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public EntityId SenderAccountId
		{
			get
			{
				return this._SenderAccountId;
			}
			set
			{
				this._SenderAccountId = value;
				this.HasSenderAccountId = value != null;
			}
		}

		public string SenderBattleTag
		{
			get
			{
				return this._SenderBattleTag;
			}
			set
			{
				this._SenderBattleTag = value;
				this.HasSenderBattleTag = value != null;
			}
		}

		public EntityId SenderId
		{
			get
			{
				return this._SenderId;
			}
			set
			{
				this._SenderId = value;
				this.HasSenderId = value != null;
			}
		}

		public EntityId TargetAccountId
		{
			get
			{
				return this._TargetAccountId;
			}
			set
			{
				this._TargetAccountId = value;
				this.HasTargetAccountId = value != null;
			}
		}

		public EntityId TargetId
		{
			get;
			set;
		}

		public string Type
		{
			get;
			set;
		}

		public Notification()
		{
		}

		public void AddAttribute(bnet.protocol.attribute.Attribute val)
		{
			this._Attribute.Add(val);
		}

		public void ClearAttribute()
		{
			this._Attribute.Clear();
		}

		public void Deserialize(Stream stream)
		{
			Notification.Deserialize(stream, this);
		}

		public static Notification Deserialize(Stream stream, Notification instance)
		{
			return Notification.Deserialize(stream, instance, (long)-1);
		}

		public static Notification Deserialize(Stream stream, Notification instance, long limit)
		{
			if (instance.Attribute == null)
			{
				instance.Attribute = new List<bnet.protocol.attribute.Attribute>();
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
							if (instance.SenderId != null)
							{
								EntityId.DeserializeLengthDelimited(stream, instance.SenderId);
							}
							else
							{
								instance.SenderId = EntityId.DeserializeLengthDelimited(stream);
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
						else if (num1 == 26)
						{
							instance.Type = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 34)
						{
							instance.Attribute.Add(bnet.protocol.attribute.Attribute.DeserializeLengthDelimited(stream));
						}
						else if (num1 == 42)
						{
							if (instance.SenderAccountId != null)
							{
								EntityId.DeserializeLengthDelimited(stream, instance.SenderAccountId);
							}
							else
							{
								instance.SenderAccountId = EntityId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 != 50)
						{
							if (num1 == 58)
							{
								instance.SenderBattleTag = ProtocolParser.ReadString(stream);
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
						else if (instance.TargetAccountId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.TargetAccountId);
						}
						else
						{
							instance.TargetAccountId = EntityId.DeserializeLengthDelimited(stream);
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

		public static Notification DeserializeLengthDelimited(Stream stream)
		{
			Notification notification = new Notification();
			Notification.DeserializeLengthDelimited(stream, notification);
			return notification;
		}

		public static Notification DeserializeLengthDelimited(Stream stream, Notification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return Notification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			Notification notification = obj as Notification;
			if (notification == null)
			{
				return false;
			}
			if (this.HasSenderId != notification.HasSenderId || this.HasSenderId && !this.SenderId.Equals(notification.SenderId))
			{
				return false;
			}
			if (!this.TargetId.Equals(notification.TargetId))
			{
				return false;
			}
			if (!this.Type.Equals(notification.Type))
			{
				return false;
			}
			if (this.Attribute.Count != notification.Attribute.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Attribute.Count; i++)
			{
				if (!this.Attribute[i].Equals(notification.Attribute[i]))
				{
					return false;
				}
			}
			if (this.HasSenderAccountId != notification.HasSenderAccountId || this.HasSenderAccountId && !this.SenderAccountId.Equals(notification.SenderAccountId))
			{
				return false;
			}
			if (this.HasTargetAccountId != notification.HasTargetAccountId || this.HasTargetAccountId && !this.TargetAccountId.Equals(notification.TargetAccountId))
			{
				return false;
			}
			if (this.HasSenderBattleTag == notification.HasSenderBattleTag && (!this.HasSenderBattleTag || this.SenderBattleTag.Equals(notification.SenderBattleTag)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasSenderId)
			{
				hashCode ^= this.SenderId.GetHashCode();
			}
			hashCode ^= this.TargetId.GetHashCode();
			hashCode ^= this.Type.GetHashCode();
			foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
			{
				hashCode ^= attribute.GetHashCode();
			}
			if (this.HasSenderAccountId)
			{
				hashCode ^= this.SenderAccountId.GetHashCode();
			}
			if (this.HasTargetAccountId)
			{
				hashCode ^= this.TargetAccountId.GetHashCode();
			}
			if (this.HasSenderBattleTag)
			{
				hashCode ^= this.SenderBattleTag.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasSenderId)
			{
				num++;
				uint serializedSize = this.SenderId.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			uint serializedSize1 = this.TargetId.GetSerializedSize();
			num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Type);
			num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			if (this.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
				{
					num++;
					uint num1 = attribute.GetSerializedSize();
					num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
				}
			}
			if (this.HasSenderAccountId)
			{
				num++;
				uint serializedSize2 = this.SenderAccountId.GetSerializedSize();
				num = num + serializedSize2 + ProtocolParser.SizeOfUInt32(serializedSize2);
			}
			if (this.HasTargetAccountId)
			{
				num++;
				uint num2 = this.TargetAccountId.GetSerializedSize();
				num = num + num2 + ProtocolParser.SizeOfUInt32(num2);
			}
			if (this.HasSenderBattleTag)
			{
				num++;
				uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.SenderBattleTag);
				num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			}
			num += 2;
			return num;
		}

		public static Notification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<Notification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			Notification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, Notification instance)
		{
			if (instance.HasSenderId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.SenderId.GetSerializedSize());
				EntityId.Serialize(stream, instance.SenderId);
			}
			if (instance.TargetId == null)
			{
				throw new ArgumentNullException("TargetId", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.TargetId.GetSerializedSize());
			EntityId.Serialize(stream, instance.TargetId);
			if (instance.Type == null)
			{
				throw new ArgumentNullException("Type", "Required by proto specification.");
			}
			stream.WriteByte(26);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Type));
			if (instance.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in instance.Attribute)
				{
					stream.WriteByte(34);
					ProtocolParser.WriteUInt32(stream, attribute.GetSerializedSize());
					bnet.protocol.attribute.Attribute.Serialize(stream, attribute);
				}
			}
			if (instance.HasSenderAccountId)
			{
				stream.WriteByte(42);
				ProtocolParser.WriteUInt32(stream, instance.SenderAccountId.GetSerializedSize());
				EntityId.Serialize(stream, instance.SenderAccountId);
			}
			if (instance.HasTargetAccountId)
			{
				stream.WriteByte(50);
				ProtocolParser.WriteUInt32(stream, instance.TargetAccountId.GetSerializedSize());
				EntityId.Serialize(stream, instance.TargetAccountId);
			}
			if (instance.HasSenderBattleTag)
			{
				stream.WriteByte(58);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.SenderBattleTag));
			}
		}

		public void SetAttribute(List<bnet.protocol.attribute.Attribute> val)
		{
			this.Attribute = val;
		}

		public void SetSenderAccountId(EntityId val)
		{
			this.SenderAccountId = val;
		}

		public void SetSenderBattleTag(string val)
		{
			this.SenderBattleTag = val;
		}

		public void SetSenderId(EntityId val)
		{
			this.SenderId = val;
		}

		public void SetTargetAccountId(EntityId val)
		{
			this.TargetAccountId = val;
		}

		public void SetTargetId(EntityId val)
		{
			this.TargetId = val;
		}

		public void SetType(string val)
		{
			this.Type = val;
		}
	}
}