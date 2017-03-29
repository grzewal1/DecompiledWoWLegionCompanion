using bnet.protocol;
using bnet.protocol.attribute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.game_master
{
	public class ConnectInfo : IProtoBuf
	{
		public bool HasToken;

		private byte[] _Token;

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

		public string Host
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

		public EntityId MemberId
		{
			get;
			set;
		}

		public int Port
		{
			get;
			set;
		}

		public byte[] Token
		{
			get
			{
				return this._Token;
			}
			set
			{
				this._Token = value;
				this.HasToken = value != null;
			}
		}

		public ConnectInfo()
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
			ConnectInfo.Deserialize(stream, this);
		}

		public static ConnectInfo Deserialize(Stream stream, ConnectInfo instance)
		{
			return ConnectInfo.Deserialize(stream, instance, (long)-1);
		}

		public static ConnectInfo Deserialize(Stream stream, ConnectInfo instance, long limit)
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
							if (instance.MemberId != null)
							{
								EntityId.DeserializeLengthDelimited(stream, instance.MemberId);
							}
							else
							{
								instance.MemberId = EntityId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 18)
						{
							instance.Host = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 24)
						{
							instance.Port = (int)ProtocolParser.ReadUInt64(stream);
						}
						else if (num1 == 34)
						{
							instance.Token = ProtocolParser.ReadBytes(stream);
						}
						else if (num1 == 42)
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

		public static ConnectInfo DeserializeLengthDelimited(Stream stream)
		{
			ConnectInfo connectInfo = new ConnectInfo();
			ConnectInfo.DeserializeLengthDelimited(stream, connectInfo);
			return connectInfo;
		}

		public static ConnectInfo DeserializeLengthDelimited(Stream stream, ConnectInfo instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return ConnectInfo.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ConnectInfo connectInfo = obj as ConnectInfo;
			if (connectInfo == null)
			{
				return false;
			}
			if (!this.MemberId.Equals(connectInfo.MemberId))
			{
				return false;
			}
			if (!this.Host.Equals(connectInfo.Host))
			{
				return false;
			}
			if (!this.Port.Equals(connectInfo.Port))
			{
				return false;
			}
			if (this.HasToken != connectInfo.HasToken || this.HasToken && !this.Token.Equals(connectInfo.Token))
			{
				return false;
			}
			if (this.Attribute.Count != connectInfo.Attribute.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Attribute.Count; i++)
			{
				if (!this.Attribute[i].Equals(connectInfo.Attribute[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.MemberId.GetHashCode();
			hashCode = hashCode ^ this.Host.GetHashCode();
			hashCode = hashCode ^ this.Port.GetHashCode();
			if (this.HasToken)
			{
				hashCode = hashCode ^ this.Token.GetHashCode();
			}
			foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
			{
				hashCode = hashCode ^ attribute.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.MemberId.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Host);
			num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			num = num + ProtocolParser.SizeOfUInt64((ulong)this.Port);
			if (this.HasToken)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32((int)this.Token.Length) + (int)this.Token.Length;
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
			num = num + 3;
			return num;
		}

		public static ConnectInfo ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ConnectInfo>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ConnectInfo.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ConnectInfo instance)
		{
			if (instance.MemberId == null)
			{
				throw new ArgumentNullException("MemberId", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.MemberId.GetSerializedSize());
			EntityId.Serialize(stream, instance.MemberId);
			if (instance.Host == null)
			{
				throw new ArgumentNullException("Host", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Host));
			stream.WriteByte(24);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.Port);
			if (instance.HasToken)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteBytes(stream, instance.Token);
			}
			if (instance.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in instance.Attribute)
				{
					stream.WriteByte(42);
					ProtocolParser.WriteUInt32(stream, attribute.GetSerializedSize());
					bnet.protocol.attribute.Attribute.Serialize(stream, attribute);
				}
			}
		}

		public void SetAttribute(List<bnet.protocol.attribute.Attribute> val)
		{
			this.Attribute = val;
		}

		public void SetHost(string val)
		{
			this.Host = val;
		}

		public void SetMemberId(EntityId val)
		{
			this.MemberId = val;
		}

		public void SetPort(int val)
		{
			this.Port = val;
		}

		public void SetToken(byte[] val)
		{
			this.Token = val;
		}
	}
}