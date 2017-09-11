using bnet.protocol;
using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.connection
{
	public class ConnectionMeteringContentHandles : IProtoBuf
	{
		private List<bnet.protocol.ContentHandle> _ContentHandle = new List<bnet.protocol.ContentHandle>();

		public List<bnet.protocol.ContentHandle> ContentHandle
		{
			get
			{
				return this._ContentHandle;
			}
			set
			{
				this._ContentHandle = value;
			}
		}

		public int ContentHandleCount
		{
			get
			{
				return this._ContentHandle.Count;
			}
		}

		public List<bnet.protocol.ContentHandle> ContentHandleList
		{
			get
			{
				return this._ContentHandle;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public ConnectionMeteringContentHandles()
		{
		}

		public void AddContentHandle(bnet.protocol.ContentHandle val)
		{
			this._ContentHandle.Add(val);
		}

		public void ClearContentHandle()
		{
			this._ContentHandle.Clear();
		}

		public void Deserialize(Stream stream)
		{
			ConnectionMeteringContentHandles.Deserialize(stream, this);
		}

		public static ConnectionMeteringContentHandles Deserialize(Stream stream, ConnectionMeteringContentHandles instance)
		{
			return ConnectionMeteringContentHandles.Deserialize(stream, instance, (long)-1);
		}

		public static ConnectionMeteringContentHandles Deserialize(Stream stream, ConnectionMeteringContentHandles instance, long limit)
		{
			if (instance.ContentHandle == null)
			{
				instance.ContentHandle = new List<bnet.protocol.ContentHandle>();
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
						instance.ContentHandle.Add(bnet.protocol.ContentHandle.DeserializeLengthDelimited(stream));
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

		public static ConnectionMeteringContentHandles DeserializeLengthDelimited(Stream stream)
		{
			ConnectionMeteringContentHandles connectionMeteringContentHandle = new ConnectionMeteringContentHandles();
			ConnectionMeteringContentHandles.DeserializeLengthDelimited(stream, connectionMeteringContentHandle);
			return connectionMeteringContentHandle;
		}

		public static ConnectionMeteringContentHandles DeserializeLengthDelimited(Stream stream, ConnectionMeteringContentHandles instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ConnectionMeteringContentHandles.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ConnectionMeteringContentHandles connectionMeteringContentHandle = obj as ConnectionMeteringContentHandles;
			if (connectionMeteringContentHandle == null)
			{
				return false;
			}
			if (this.ContentHandle.Count != connectionMeteringContentHandle.ContentHandle.Count)
			{
				return false;
			}
			for (int i = 0; i < this.ContentHandle.Count; i++)
			{
				if (!this.ContentHandle[i].Equals(connectionMeteringContentHandle.ContentHandle[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (bnet.protocol.ContentHandle contentHandle in this.ContentHandle)
			{
				hashCode ^= contentHandle.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.ContentHandle.Count > 0)
			{
				foreach (bnet.protocol.ContentHandle contentHandle in this.ContentHandle)
				{
					num++;
					uint serializedSize = contentHandle.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			return num;
		}

		public static ConnectionMeteringContentHandles ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ConnectionMeteringContentHandles>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ConnectionMeteringContentHandles.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ConnectionMeteringContentHandles instance)
		{
			if (instance.ContentHandle.Count > 0)
			{
				foreach (bnet.protocol.ContentHandle contentHandle in instance.ContentHandle)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, contentHandle.GetSerializedSize());
					bnet.protocol.ContentHandle.Serialize(stream, contentHandle);
				}
			}
		}

		public void SetContentHandle(List<bnet.protocol.ContentHandle> val)
		{
			this.ContentHandle = val;
		}
	}
}