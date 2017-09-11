using bnet.protocol;
using bnet.protocol.attribute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.server_pool
{
	public class ServerInfo : IProtoBuf
	{
		public bool HasReplace;

		private bool _Replace;

		public bool HasState;

		private ServerState _State;

		private List<bnet.protocol.attribute.Attribute> _Attribute = new List<bnet.protocol.attribute.Attribute>();

		public bool HasProgramId;

		private uint _ProgramId;

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

		public uint ProgramId
		{
			get
			{
				return this._ProgramId;
			}
			set
			{
				this._ProgramId = value;
				this.HasProgramId = true;
			}
		}

		public bool Replace
		{
			get
			{
				return this._Replace;
			}
			set
			{
				this._Replace = value;
				this.HasReplace = true;
			}
		}

		public ServerState State
		{
			get
			{
				return this._State;
			}
			set
			{
				this._State = value;
				this.HasState = value != null;
			}
		}

		public ServerInfo()
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
			ServerInfo.Deserialize(stream, this);
		}

		public static ServerInfo Deserialize(Stream stream, ServerInfo instance)
		{
			return ServerInfo.Deserialize(stream, instance, (long)-1);
		}

		public static ServerInfo Deserialize(Stream stream, ServerInfo instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			instance.Replace = false;
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
							if (instance.Host != null)
							{
								ProcessId.DeserializeLengthDelimited(stream, instance.Host);
							}
							else
							{
								instance.Host = ProcessId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 16)
						{
							instance.Replace = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 26)
						{
							if (instance.State != null)
							{
								ServerState.DeserializeLengthDelimited(stream, instance.State);
							}
							else
							{
								instance.State = ServerState.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 34)
						{
							instance.Attribute.Add(bnet.protocol.attribute.Attribute.DeserializeLengthDelimited(stream));
						}
						else if (num1 == 45)
						{
							instance.ProgramId = binaryReader.ReadUInt32();
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

		public static ServerInfo DeserializeLengthDelimited(Stream stream)
		{
			ServerInfo serverInfo = new ServerInfo();
			ServerInfo.DeserializeLengthDelimited(stream, serverInfo);
			return serverInfo;
		}

		public static ServerInfo DeserializeLengthDelimited(Stream stream, ServerInfo instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ServerInfo.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ServerInfo serverInfo = obj as ServerInfo;
			if (serverInfo == null)
			{
				return false;
			}
			if (!this.Host.Equals(serverInfo.Host))
			{
				return false;
			}
			if (this.HasReplace != serverInfo.HasReplace || this.HasReplace && !this.Replace.Equals(serverInfo.Replace))
			{
				return false;
			}
			if (this.HasState != serverInfo.HasState || this.HasState && !this.State.Equals(serverInfo.State))
			{
				return false;
			}
			if (this.Attribute.Count != serverInfo.Attribute.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Attribute.Count; i++)
			{
				if (!this.Attribute[i].Equals(serverInfo.Attribute[i]))
				{
					return false;
				}
			}
			if (this.HasProgramId == serverInfo.HasProgramId && (!this.HasProgramId || this.ProgramId.Equals(serverInfo.ProgramId)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Host.GetHashCode();
			if (this.HasReplace)
			{
				hashCode ^= this.Replace.GetHashCode();
			}
			if (this.HasState)
			{
				hashCode ^= this.State.GetHashCode();
			}
			foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
			{
				hashCode ^= attribute.GetHashCode();
			}
			if (this.HasProgramId)
			{
				hashCode ^= this.ProgramId.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.Host.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasReplace)
			{
				num++;
				num++;
			}
			if (this.HasState)
			{
				num++;
				uint serializedSize1 = this.State.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			if (this.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
				{
					num++;
					uint num1 = attribute.GetSerializedSize();
					num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
				}
			}
			if (this.HasProgramId)
			{
				num++;
				num += 4;
			}
			num++;
			return num;
		}

		public static ServerInfo ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ServerInfo>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ServerInfo.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ServerInfo instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.Host == null)
			{
				throw new ArgumentNullException("Host", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.Host.GetSerializedSize());
			ProcessId.Serialize(stream, instance.Host);
			if (instance.HasReplace)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteBool(stream, instance.Replace);
			}
			if (instance.HasState)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteUInt32(stream, instance.State.GetSerializedSize());
				ServerState.Serialize(stream, instance.State);
			}
			if (instance.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in instance.Attribute)
				{
					stream.WriteByte(34);
					ProtocolParser.WriteUInt32(stream, attribute.GetSerializedSize());
					bnet.protocol.attribute.Attribute.Serialize(stream, attribute);
				}
			}
			if (instance.HasProgramId)
			{
				stream.WriteByte(45);
				binaryWriter.Write(instance.ProgramId);
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

		public void SetProgramId(uint val)
		{
			this.ProgramId = val;
		}

		public void SetReplace(bool val)
		{
			this.Replace = val;
		}

		public void SetState(ServerState val)
		{
			this.State = val;
		}
	}
}