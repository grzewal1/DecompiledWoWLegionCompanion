using bnet.protocol;
using bnet.protocol.attribute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.game_utilities
{
	public class ServerRequest : IProtoBuf
	{
		private List<bnet.protocol.attribute.Attribute> _Attribute = new List<bnet.protocol.attribute.Attribute>();

		public bool HasHost;

		private ProcessId _Host;

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

		public uint Program
		{
			get;
			set;
		}

		public ServerRequest()
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
			ServerRequest.Deserialize(stream, this);
		}

		public static ServerRequest Deserialize(Stream stream, ServerRequest instance)
		{
			return ServerRequest.Deserialize(stream, instance, (long)-1);
		}

		public static ServerRequest Deserialize(Stream stream, ServerRequest instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
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
						else if (num1 == 21)
						{
							instance.Program = binaryReader.ReadUInt32();
						}
						else if (num1 != 26)
						{
							Key key = ProtocolParser.ReadKey((byte)num, stream);
							if (key.Field == 0)
							{
								throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
							}
							ProtocolParser.SkipKey(stream, key);
						}
						else if (instance.Host != null)
						{
							ProcessId.DeserializeLengthDelimited(stream, instance.Host);
						}
						else
						{
							instance.Host = ProcessId.DeserializeLengthDelimited(stream);
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

		public static ServerRequest DeserializeLengthDelimited(Stream stream)
		{
			ServerRequest serverRequest = new ServerRequest();
			ServerRequest.DeserializeLengthDelimited(stream, serverRequest);
			return serverRequest;
		}

		public static ServerRequest DeserializeLengthDelimited(Stream stream, ServerRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ServerRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ServerRequest serverRequest = obj as ServerRequest;
			if (serverRequest == null)
			{
				return false;
			}
			if (this.Attribute.Count != serverRequest.Attribute.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Attribute.Count; i++)
			{
				if (!this.Attribute[i].Equals(serverRequest.Attribute[i]))
				{
					return false;
				}
			}
			if (!this.Program.Equals(serverRequest.Program))
			{
				return false;
			}
			if (this.HasHost == serverRequest.HasHost && (!this.HasHost || this.Host.Equals(serverRequest.Host)))
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
			hashCode ^= this.Program.GetHashCode();
			if (this.HasHost)
			{
				hashCode ^= this.Host.GetHashCode();
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
			num += 4;
			if (this.HasHost)
			{
				num++;
				uint serializedSize1 = this.Host.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			num++;
			return num;
		}

		public static ServerRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ServerRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ServerRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ServerRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in instance.Attribute)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, attribute.GetSerializedSize());
					bnet.protocol.attribute.Attribute.Serialize(stream, attribute);
				}
			}
			stream.WriteByte(21);
			binaryWriter.Write(instance.Program);
			if (instance.HasHost)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteUInt32(stream, instance.Host.GetSerializedSize());
				ProcessId.Serialize(stream, instance.Host);
			}
		}

		public void SetAttribute(List<bnet.protocol.attribute.Attribute> val)
		{
			this.Attribute = val;
		}

		public void SetHost(ProcessId val)
		{
			this.Host = val;
		}

		public void SetProgram(uint val)
		{
			this.Program = val;
		}
	}
}