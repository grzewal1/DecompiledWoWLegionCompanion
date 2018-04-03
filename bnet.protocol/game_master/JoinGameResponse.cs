using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.game_master
{
	public class JoinGameResponse : IProtoBuf
	{
		public bool HasRequestId;

		private ulong _RequestId;

		public bool HasQueued;

		private bool _Queued;

		private List<bnet.protocol.game_master.ConnectInfo> _ConnectInfo = new List<bnet.protocol.game_master.ConnectInfo>();

		public List<bnet.protocol.game_master.ConnectInfo> ConnectInfo
		{
			get
			{
				return this._ConnectInfo;
			}
			set
			{
				this._ConnectInfo = value;
			}
		}

		public int ConnectInfoCount
		{
			get
			{
				return this._ConnectInfo.Count;
			}
		}

		public List<bnet.protocol.game_master.ConnectInfo> ConnectInfoList
		{
			get
			{
				return this._ConnectInfo;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public bool Queued
		{
			get
			{
				return this._Queued;
			}
			set
			{
				this._Queued = value;
				this.HasQueued = true;
			}
		}

		public ulong RequestId
		{
			get
			{
				return this._RequestId;
			}
			set
			{
				this._RequestId = value;
				this.HasRequestId = true;
			}
		}

		public JoinGameResponse()
		{
		}

		public void AddConnectInfo(bnet.protocol.game_master.ConnectInfo val)
		{
			this._ConnectInfo.Add(val);
		}

		public void ClearConnectInfo()
		{
			this._ConnectInfo.Clear();
		}

		public void Deserialize(Stream stream)
		{
			JoinGameResponse.Deserialize(stream, this);
		}

		public static JoinGameResponse Deserialize(Stream stream, JoinGameResponse instance)
		{
			return JoinGameResponse.Deserialize(stream, instance, (long)-1);
		}

		public static JoinGameResponse Deserialize(Stream stream, JoinGameResponse instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			instance.Queued = false;
			if (instance.ConnectInfo == null)
			{
				instance.ConnectInfo = new List<bnet.protocol.game_master.ConnectInfo>();
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
					else if (num == 9)
					{
						instance.RequestId = binaryReader.ReadUInt64();
					}
					else if (num == 16)
					{
						instance.Queued = ProtocolParser.ReadBool(stream);
					}
					else if (num == 26)
					{
						instance.ConnectInfo.Add(bnet.protocol.game_master.ConnectInfo.DeserializeLengthDelimited(stream));
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

		public static JoinGameResponse DeserializeLengthDelimited(Stream stream)
		{
			JoinGameResponse joinGameResponse = new JoinGameResponse();
			JoinGameResponse.DeserializeLengthDelimited(stream, joinGameResponse);
			return joinGameResponse;
		}

		public static JoinGameResponse DeserializeLengthDelimited(Stream stream, JoinGameResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return JoinGameResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			JoinGameResponse joinGameResponse = obj as JoinGameResponse;
			if (joinGameResponse == null)
			{
				return false;
			}
			if (this.HasRequestId != joinGameResponse.HasRequestId || this.HasRequestId && !this.RequestId.Equals(joinGameResponse.RequestId))
			{
				return false;
			}
			if (this.HasQueued != joinGameResponse.HasQueued || this.HasQueued && !this.Queued.Equals(joinGameResponse.Queued))
			{
				return false;
			}
			if (this.ConnectInfo.Count != joinGameResponse.ConnectInfo.Count)
			{
				return false;
			}
			for (int i = 0; i < this.ConnectInfo.Count; i++)
			{
				if (!this.ConnectInfo[i].Equals(joinGameResponse.ConnectInfo[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasRequestId)
			{
				hashCode ^= this.RequestId.GetHashCode();
			}
			if (this.HasQueued)
			{
				hashCode ^= this.Queued.GetHashCode();
			}
			foreach (bnet.protocol.game_master.ConnectInfo connectInfo in this.ConnectInfo)
			{
				hashCode ^= connectInfo.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasRequestId)
			{
				num++;
				num += 8;
			}
			if (this.HasQueued)
			{
				num++;
				num++;
			}
			if (this.ConnectInfo.Count > 0)
			{
				foreach (bnet.protocol.game_master.ConnectInfo connectInfo in this.ConnectInfo)
				{
					num++;
					uint serializedSize = connectInfo.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			return num;
		}

		public static JoinGameResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<JoinGameResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			JoinGameResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, JoinGameResponse instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasRequestId)
			{
				stream.WriteByte(9);
				binaryWriter.Write(instance.RequestId);
			}
			if (instance.HasQueued)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteBool(stream, instance.Queued);
			}
			if (instance.ConnectInfo.Count > 0)
			{
				foreach (bnet.protocol.game_master.ConnectInfo connectInfo in instance.ConnectInfo)
				{
					stream.WriteByte(26);
					ProtocolParser.WriteUInt32(stream, connectInfo.GetSerializedSize());
					bnet.protocol.game_master.ConnectInfo.Serialize(stream, connectInfo);
				}
			}
		}

		public void SetConnectInfo(List<bnet.protocol.game_master.ConnectInfo> val)
		{
			this.ConnectInfo = val;
		}

		public void SetQueued(bool val)
		{
			this.Queued = val;
		}

		public void SetRequestId(ulong val)
		{
			this.RequestId = val;
		}
	}
}