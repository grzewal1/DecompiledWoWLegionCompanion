using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.server_pool
{
	public class PoolStateResponse : IProtoBuf
	{
		private List<ServerInfo> _Info = new List<ServerInfo>();

		public List<ServerInfo> Info
		{
			get
			{
				return this._Info;
			}
			set
			{
				this._Info = value;
			}
		}

		public int InfoCount
		{
			get
			{
				return this._Info.Count;
			}
		}

		public List<ServerInfo> InfoList
		{
			get
			{
				return this._Info;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public PoolStateResponse()
		{
		}

		public void AddInfo(ServerInfo val)
		{
			this._Info.Add(val);
		}

		public void ClearInfo()
		{
			this._Info.Clear();
		}

		public void Deserialize(Stream stream)
		{
			PoolStateResponse.Deserialize(stream, this);
		}

		public static PoolStateResponse Deserialize(Stream stream, PoolStateResponse instance)
		{
			return PoolStateResponse.Deserialize(stream, instance, (long)-1);
		}

		public static PoolStateResponse Deserialize(Stream stream, PoolStateResponse instance, long limit)
		{
			if (instance.Info == null)
			{
				instance.Info = new List<ServerInfo>();
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
						instance.Info.Add(ServerInfo.DeserializeLengthDelimited(stream));
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

		public static PoolStateResponse DeserializeLengthDelimited(Stream stream)
		{
			PoolStateResponse poolStateResponse = new PoolStateResponse();
			PoolStateResponse.DeserializeLengthDelimited(stream, poolStateResponse);
			return poolStateResponse;
		}

		public static PoolStateResponse DeserializeLengthDelimited(Stream stream, PoolStateResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return PoolStateResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			PoolStateResponse poolStateResponse = obj as PoolStateResponse;
			if (poolStateResponse == null)
			{
				return false;
			}
			if (this.Info.Count != poolStateResponse.Info.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Info.Count; i++)
			{
				if (!this.Info[i].Equals(poolStateResponse.Info[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (ServerInfo info in this.Info)
			{
				hashCode = hashCode ^ info.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Info.Count > 0)
			{
				foreach (ServerInfo info in this.Info)
				{
					num++;
					uint serializedSize = info.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			return num;
		}

		public static PoolStateResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<PoolStateResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			PoolStateResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, PoolStateResponse instance)
		{
			if (instance.Info.Count > 0)
			{
				foreach (ServerInfo info in instance.Info)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, info.GetSerializedSize());
					ServerInfo.Serialize(stream, info);
				}
			}
		}

		public void SetInfo(List<ServerInfo> val)
		{
			this.Info = val;
		}
	}
}