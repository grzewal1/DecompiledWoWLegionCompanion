using System;
using System.IO;

namespace bnet.protocol.server_pool
{
	public class ServerState : IProtoBuf
	{
		public bool HasCurrentLoad;

		private float _CurrentLoad;

		public bool HasGameCount;

		private uint _GameCount;

		public bool HasPlayerCount;

		private uint _PlayerCount;

		public float CurrentLoad
		{
			get
			{
				return this._CurrentLoad;
			}
			set
			{
				this._CurrentLoad = value;
				this.HasCurrentLoad = true;
			}
		}

		public uint GameCount
		{
			get
			{
				return this._GameCount;
			}
			set
			{
				this._GameCount = value;
				this.HasGameCount = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public uint PlayerCount
		{
			get
			{
				return this._PlayerCount;
			}
			set
			{
				this._PlayerCount = value;
				this.HasPlayerCount = true;
			}
		}

		public ServerState()
		{
		}

		public void Deserialize(Stream stream)
		{
			ServerState.Deserialize(stream, this);
		}

		public static ServerState Deserialize(Stream stream, ServerState instance)
		{
			return ServerState.Deserialize(stream, instance, (long)-1);
		}

		public static ServerState Deserialize(Stream stream, ServerState instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			instance.CurrentLoad = 1f;
			instance.GameCount = 0;
			instance.PlayerCount = 0;
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						switch (num1)
						{
							case 13:
							{
								instance.CurrentLoad = binaryReader.ReadSingle();
								continue;
							}
							case 16:
							{
								instance.GameCount = ProtocolParser.ReadUInt32(stream);
								continue;
							}
							default:
							{
								if (num1 == 24)
								{
									instance.PlayerCount = ProtocolParser.ReadUInt32(stream);
									continue;
								}
								else
								{
									Key key = ProtocolParser.ReadKey((byte)num, stream);
									if (key.Field == 0)
									{
										throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
									}
									ProtocolParser.SkipKey(stream, key);
									continue;
								}
							}
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

		public static ServerState DeserializeLengthDelimited(Stream stream)
		{
			ServerState serverState = new ServerState();
			ServerState.DeserializeLengthDelimited(stream, serverState);
			return serverState;
		}

		public static ServerState DeserializeLengthDelimited(Stream stream, ServerState instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ServerState.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ServerState serverState = obj as ServerState;
			if (serverState == null)
			{
				return false;
			}
			if (this.HasCurrentLoad != serverState.HasCurrentLoad || this.HasCurrentLoad && !this.CurrentLoad.Equals(serverState.CurrentLoad))
			{
				return false;
			}
			if (this.HasGameCount != serverState.HasGameCount || this.HasGameCount && !this.GameCount.Equals(serverState.GameCount))
			{
				return false;
			}
			if (this.HasPlayerCount == serverState.HasPlayerCount && (!this.HasPlayerCount || this.PlayerCount.Equals(serverState.PlayerCount)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasCurrentLoad)
			{
				hashCode ^= this.CurrentLoad.GetHashCode();
			}
			if (this.HasGameCount)
			{
				hashCode ^= this.GameCount.GetHashCode();
			}
			if (this.HasPlayerCount)
			{
				hashCode ^= this.PlayerCount.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasCurrentLoad)
			{
				num++;
				num += 4;
			}
			if (this.HasGameCount)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.GameCount);
			}
			if (this.HasPlayerCount)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.PlayerCount);
			}
			return num;
		}

		public static ServerState ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ServerState>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ServerState.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ServerState instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasCurrentLoad)
			{
				stream.WriteByte(13);
				binaryWriter.Write(instance.CurrentLoad);
			}
			if (instance.HasGameCount)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.GameCount);
			}
			if (instance.HasPlayerCount)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.PlayerCount);
			}
		}

		public void SetCurrentLoad(float val)
		{
			this.CurrentLoad = val;
		}

		public void SetGameCount(uint val)
		{
			this.GameCount = val;
		}

		public void SetPlayerCount(uint val)
		{
			this.PlayerCount = val;
		}
	}
}