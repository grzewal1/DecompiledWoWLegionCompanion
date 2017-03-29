using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.game_master
{
	public class GameFoundNotification : IProtoBuf
	{
		public bool HasErrorCode;

		private uint _ErrorCode;

		public bool HasGameHandle;

		private bnet.protocol.game_master.GameHandle _GameHandle;

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

		public uint ErrorCode
		{
			get
			{
				return this._ErrorCode;
			}
			set
			{
				this._ErrorCode = value;
				this.HasErrorCode = true;
			}
		}

		public bnet.protocol.game_master.GameHandle GameHandle
		{
			get
			{
				return this._GameHandle;
			}
			set
			{
				this._GameHandle = value;
				this.HasGameHandle = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public ulong RequestId
		{
			get;
			set;
		}

		public GameFoundNotification()
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
			GameFoundNotification.Deserialize(stream, this);
		}

		public static GameFoundNotification Deserialize(Stream stream, GameFoundNotification instance)
		{
			return GameFoundNotification.Deserialize(stream, instance, (long)-1);
		}

		public static GameFoundNotification Deserialize(Stream stream, GameFoundNotification instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			instance.ErrorCode = 0;
			if (instance.ConnectInfo == null)
			{
				instance.ConnectInfo = new List<bnet.protocol.game_master.ConnectInfo>();
			}
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 == 9)
						{
							instance.RequestId = binaryReader.ReadUInt64();
						}
						else if (num1 == 16)
						{
							instance.ErrorCode = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 != 26)
						{
							if (num1 == 34)
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
						else if (instance.GameHandle != null)
						{
							bnet.protocol.game_master.GameHandle.DeserializeLengthDelimited(stream, instance.GameHandle);
						}
						else
						{
							instance.GameHandle = bnet.protocol.game_master.GameHandle.DeserializeLengthDelimited(stream);
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

		public static GameFoundNotification DeserializeLengthDelimited(Stream stream)
		{
			GameFoundNotification gameFoundNotification = new GameFoundNotification();
			GameFoundNotification.DeserializeLengthDelimited(stream, gameFoundNotification);
			return gameFoundNotification;
		}

		public static GameFoundNotification DeserializeLengthDelimited(Stream stream, GameFoundNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GameFoundNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameFoundNotification gameFoundNotification = obj as GameFoundNotification;
			if (gameFoundNotification == null)
			{
				return false;
			}
			if (!this.RequestId.Equals(gameFoundNotification.RequestId))
			{
				return false;
			}
			if (this.HasErrorCode != gameFoundNotification.HasErrorCode || this.HasErrorCode && !this.ErrorCode.Equals(gameFoundNotification.ErrorCode))
			{
				return false;
			}
			if (this.HasGameHandle != gameFoundNotification.HasGameHandle || this.HasGameHandle && !this.GameHandle.Equals(gameFoundNotification.GameHandle))
			{
				return false;
			}
			if (this.ConnectInfo.Count != gameFoundNotification.ConnectInfo.Count)
			{
				return false;
			}
			for (int i = 0; i < this.ConnectInfo.Count; i++)
			{
				if (!this.ConnectInfo[i].Equals(gameFoundNotification.ConnectInfo[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.RequestId.GetHashCode();
			if (this.HasErrorCode)
			{
				hashCode = hashCode ^ this.ErrorCode.GetHashCode();
			}
			if (this.HasGameHandle)
			{
				hashCode = hashCode ^ this.GameHandle.GetHashCode();
			}
			foreach (bnet.protocol.game_master.ConnectInfo connectInfo in this.ConnectInfo)
			{
				hashCode = hashCode ^ connectInfo.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num = num + 8;
			if (this.HasErrorCode)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.ErrorCode);
			}
			if (this.HasGameHandle)
			{
				num++;
				uint serializedSize = this.GameHandle.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.ConnectInfo.Count > 0)
			{
				foreach (bnet.protocol.game_master.ConnectInfo connectInfo in this.ConnectInfo)
				{
					num++;
					uint serializedSize1 = connectInfo.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			num++;
			return num;
		}

		public static GameFoundNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameFoundNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameFoundNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameFoundNotification instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			stream.WriteByte(9);
			binaryWriter.Write(instance.RequestId);
			if (instance.HasErrorCode)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.ErrorCode);
			}
			if (instance.HasGameHandle)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteUInt32(stream, instance.GameHandle.GetSerializedSize());
				bnet.protocol.game_master.GameHandle.Serialize(stream, instance.GameHandle);
			}
			if (instance.ConnectInfo.Count > 0)
			{
				foreach (bnet.protocol.game_master.ConnectInfo connectInfo in instance.ConnectInfo)
				{
					stream.WriteByte(34);
					ProtocolParser.WriteUInt32(stream, connectInfo.GetSerializedSize());
					bnet.protocol.game_master.ConnectInfo.Serialize(stream, connectInfo);
				}
			}
		}

		public void SetConnectInfo(List<bnet.protocol.game_master.ConnectInfo> val)
		{
			this.ConnectInfo = val;
		}

		public void SetErrorCode(uint val)
		{
			this.ErrorCode = val;
		}

		public void SetGameHandle(bnet.protocol.game_master.GameHandle val)
		{
			this.GameHandle = val;
		}

		public void SetRequestId(ulong val)
		{
			this.RequestId = val;
		}
	}
}