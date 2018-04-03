using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.account
{
	public class GameAccountList : IProtoBuf
	{
		public bool HasRegion;

		private uint _Region;

		private List<GameAccountHandle> _Handle = new List<GameAccountHandle>();

		public List<GameAccountHandle> Handle
		{
			get
			{
				return this._Handle;
			}
			set
			{
				this._Handle = value;
			}
		}

		public int HandleCount
		{
			get
			{
				return this._Handle.Count;
			}
		}

		public List<GameAccountHandle> HandleList
		{
			get
			{
				return this._Handle;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public uint Region
		{
			get
			{
				return this._Region;
			}
			set
			{
				this._Region = value;
				this.HasRegion = true;
			}
		}

		public GameAccountList()
		{
		}

		public void AddHandle(GameAccountHandle val)
		{
			this._Handle.Add(val);
		}

		public void ClearHandle()
		{
			this._Handle.Clear();
		}

		public void Deserialize(Stream stream)
		{
			GameAccountList.Deserialize(stream, this);
		}

		public static GameAccountList Deserialize(Stream stream, GameAccountList instance)
		{
			return GameAccountList.Deserialize(stream, instance, (long)-1);
		}

		public static GameAccountList Deserialize(Stream stream, GameAccountList instance, long limit)
		{
			if (instance.Handle == null)
			{
				instance.Handle = new List<GameAccountHandle>();
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
					else if (num == 24)
					{
						instance.Region = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 34)
					{
						instance.Handle.Add(GameAccountHandle.DeserializeLengthDelimited(stream));
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

		public static GameAccountList DeserializeLengthDelimited(Stream stream)
		{
			GameAccountList gameAccountList = new GameAccountList();
			GameAccountList.DeserializeLengthDelimited(stream, gameAccountList);
			return gameAccountList;
		}

		public static GameAccountList DeserializeLengthDelimited(Stream stream, GameAccountList instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GameAccountList.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameAccountList gameAccountList = obj as GameAccountList;
			if (gameAccountList == null)
			{
				return false;
			}
			if (this.HasRegion != gameAccountList.HasRegion || this.HasRegion && !this.Region.Equals(gameAccountList.Region))
			{
				return false;
			}
			if (this.Handle.Count != gameAccountList.Handle.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Handle.Count; i++)
			{
				if (!this.Handle[i].Equals(gameAccountList.Handle[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasRegion)
			{
				hashCode ^= this.Region.GetHashCode();
			}
			foreach (GameAccountHandle handle in this.Handle)
			{
				hashCode ^= handle.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasRegion)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.Region);
			}
			if (this.Handle.Count > 0)
			{
				foreach (GameAccountHandle handle in this.Handle)
				{
					num++;
					uint serializedSize = handle.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			return num;
		}

		public static GameAccountList ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameAccountList>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameAccountList.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameAccountList instance)
		{
			if (instance.HasRegion)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.Region);
			}
			if (instance.Handle.Count > 0)
			{
				foreach (GameAccountHandle handle in instance.Handle)
				{
					stream.WriteByte(34);
					ProtocolParser.WriteUInt32(stream, handle.GetSerializedSize());
					GameAccountHandle.Serialize(stream, handle);
				}
			}
		}

		public void SetHandle(List<GameAccountHandle> val)
		{
			this.Handle = val;
		}

		public void SetRegion(uint val)
		{
			this.Region = val;
		}
	}
}