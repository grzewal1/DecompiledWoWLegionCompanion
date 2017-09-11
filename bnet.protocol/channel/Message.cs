using bnet.protocol.attribute;
using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.channel
{
	public class Message : IProtoBuf
	{
		private List<bnet.protocol.attribute.Attribute> _Attribute = new List<bnet.protocol.attribute.Attribute>();

		public bool HasRole;

		private uint _Role;

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

		public uint Role
		{
			get
			{
				return this._Role;
			}
			set
			{
				this._Role = value;
				this.HasRole = true;
			}
		}

		public Message()
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
			Message.Deserialize(stream, this);
		}

		public static Message Deserialize(Stream stream, Message instance)
		{
			return Message.Deserialize(stream, instance, (long)-1);
		}

		public static Message Deserialize(Stream stream, Message instance, long limit)
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
							instance.Attribute.Add(bnet.protocol.attribute.Attribute.DeserializeLengthDelimited(stream));
						}
						else if (num1 == 16)
						{
							instance.Role = ProtocolParser.ReadUInt32(stream);
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

		public static Message DeserializeLengthDelimited(Stream stream)
		{
			Message message = new Message();
			Message.DeserializeLengthDelimited(stream, message);
			return message;
		}

		public static Message DeserializeLengthDelimited(Stream stream, Message instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return Message.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			Message message = obj as Message;
			if (message == null)
			{
				return false;
			}
			if (this.Attribute.Count != message.Attribute.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Attribute.Count; i++)
			{
				if (!this.Attribute[i].Equals(message.Attribute[i]))
				{
					return false;
				}
			}
			if (this.HasRole == message.HasRole && (!this.HasRole || this.Role.Equals(message.Role)))
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
			if (this.HasRole)
			{
				hashCode ^= this.Role.GetHashCode();
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
			if (this.HasRole)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.Role);
			}
			return num;
		}

		public static Message ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<Message>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			Message.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, Message instance)
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
			if (instance.HasRole)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.Role);
			}
		}

		public void SetAttribute(List<bnet.protocol.attribute.Attribute> val)
		{
			this.Attribute = val;
		}

		public void SetRole(uint val)
		{
			this.Role = val;
		}
	}
}