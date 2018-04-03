using bnet.protocol;
using bnet.protocol.attribute;
using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.game_utilities
{
	public class ClientRequest : IProtoBuf
	{
		private List<bnet.protocol.attribute.Attribute> _Attribute = new List<bnet.protocol.attribute.Attribute>();

		public bool HasHost;

		private ProcessId _Host;

		public bool HasBnetAccountId;

		private EntityId _BnetAccountId;

		public bool HasGameAccountId;

		private EntityId _GameAccountId;

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

		public EntityId BnetAccountId
		{
			get
			{
				return this._BnetAccountId;
			}
			set
			{
				this._BnetAccountId = value;
				this.HasBnetAccountId = value != null;
			}
		}

		public EntityId GameAccountId
		{
			get
			{
				return this._GameAccountId;
			}
			set
			{
				this._GameAccountId = value;
				this.HasGameAccountId = value != null;
			}
		}

		public ProcessId Host
		{
			get
			{
				return this._Host;
			}
			set
			{
				this._Host = value;
				this.HasHost = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public ClientRequest()
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
			ClientRequest.Deserialize(stream, this);
		}

		public static ClientRequest Deserialize(Stream stream, ClientRequest instance)
		{
			return ClientRequest.Deserialize(stream, instance, (long)-1);
		}

		public static ClientRequest Deserialize(Stream stream, ClientRequest instance, long limit)
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
						instance.Attribute.Add(bnet.protocol.attribute.Attribute.DeserializeLengthDelimited(stream));
					}
					else if (num == 18)
					{
						if (instance.Host != null)
						{
							ProcessId.DeserializeLengthDelimited(stream, instance.Host);
						}
						else
						{
							instance.Host = ProcessId.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 26)
					{
						if (instance.BnetAccountId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.BnetAccountId);
						}
						else
						{
							instance.BnetAccountId = EntityId.DeserializeLengthDelimited(stream);
						}
					}
					else if (num != 34)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.GameAccountId != null)
					{
						EntityId.DeserializeLengthDelimited(stream, instance.GameAccountId);
					}
					else
					{
						instance.GameAccountId = EntityId.DeserializeLengthDelimited(stream);
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

		public static ClientRequest DeserializeLengthDelimited(Stream stream)
		{
			ClientRequest clientRequest = new ClientRequest();
			ClientRequest.DeserializeLengthDelimited(stream, clientRequest);
			return clientRequest;
		}

		public static ClientRequest DeserializeLengthDelimited(Stream stream, ClientRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ClientRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ClientRequest clientRequest = obj as ClientRequest;
			if (clientRequest == null)
			{
				return false;
			}
			if (this.Attribute.Count != clientRequest.Attribute.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Attribute.Count; i++)
			{
				if (!this.Attribute[i].Equals(clientRequest.Attribute[i]))
				{
					return false;
				}
			}
			if (this.HasHost != clientRequest.HasHost || this.HasHost && !this.Host.Equals(clientRequest.Host))
			{
				return false;
			}
			if (this.HasBnetAccountId != clientRequest.HasBnetAccountId || this.HasBnetAccountId && !this.BnetAccountId.Equals(clientRequest.BnetAccountId))
			{
				return false;
			}
			if (this.HasGameAccountId == clientRequest.HasGameAccountId && (!this.HasGameAccountId || this.GameAccountId.Equals(clientRequest.GameAccountId)))
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
			if (this.HasHost)
			{
				hashCode ^= this.Host.GetHashCode();
			}
			if (this.HasBnetAccountId)
			{
				hashCode ^= this.BnetAccountId.GetHashCode();
			}
			if (this.HasGameAccountId)
			{
				hashCode ^= this.GameAccountId.GetHashCode();
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
			if (this.HasHost)
			{
				num++;
				uint serializedSize1 = this.Host.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			if (this.HasBnetAccountId)
			{
				num++;
				uint num1 = this.BnetAccountId.GetSerializedSize();
				num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
			}
			if (this.HasGameAccountId)
			{
				num++;
				uint serializedSize2 = this.GameAccountId.GetSerializedSize();
				num = num + serializedSize2 + ProtocolParser.SizeOfUInt32(serializedSize2);
			}
			return num;
		}

		public static ClientRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ClientRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ClientRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ClientRequest instance)
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
			if (instance.HasHost)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.Host.GetSerializedSize());
				ProcessId.Serialize(stream, instance.Host);
			}
			if (instance.HasBnetAccountId)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteUInt32(stream, instance.BnetAccountId.GetSerializedSize());
				EntityId.Serialize(stream, instance.BnetAccountId);
			}
			if (instance.HasGameAccountId)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteUInt32(stream, instance.GameAccountId.GetSerializedSize());
				EntityId.Serialize(stream, instance.GameAccountId);
			}
		}

		public void SetAttribute(List<bnet.protocol.attribute.Attribute> val)
		{
			this.Attribute = val;
		}

		public void SetBnetAccountId(EntityId val)
		{
			this.BnetAccountId = val;
		}

		public void SetGameAccountId(EntityId val)
		{
			this.GameAccountId = val;
		}

		public void SetHost(ProcessId val)
		{
			this.Host = val;
		}
	}
}