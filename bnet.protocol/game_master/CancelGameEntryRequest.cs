using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.game_master
{
	public class CancelGameEntryRequest : IProtoBuf
	{
		public bool HasFactoryId;

		private ulong _FactoryId;

		private List<bnet.protocol.game_master.Player> _Player = new List<bnet.protocol.game_master.Player>();

		public ulong FactoryId
		{
			get
			{
				return this._FactoryId;
			}
			set
			{
				this._FactoryId = value;
				this.HasFactoryId = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public List<bnet.protocol.game_master.Player> Player
		{
			get
			{
				return this._Player;
			}
			set
			{
				this._Player = value;
			}
		}

		public int PlayerCount
		{
			get
			{
				return this._Player.Count;
			}
		}

		public List<bnet.protocol.game_master.Player> PlayerList
		{
			get
			{
				return this._Player;
			}
		}

		public ulong RequestId
		{
			get;
			set;
		}

		public CancelGameEntryRequest()
		{
		}

		public void AddPlayer(bnet.protocol.game_master.Player val)
		{
			this._Player.Add(val);
		}

		public void ClearPlayer()
		{
			this._Player.Clear();
		}

		public void Deserialize(Stream stream)
		{
			CancelGameEntryRequest.Deserialize(stream, this);
		}

		public static CancelGameEntryRequest Deserialize(Stream stream, CancelGameEntryRequest instance)
		{
			return CancelGameEntryRequest.Deserialize(stream, instance, (long)-1);
		}

		public static CancelGameEntryRequest Deserialize(Stream stream, CancelGameEntryRequest instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			if (instance.Player == null)
			{
				instance.Player = new List<bnet.protocol.game_master.Player>();
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
						else if (num1 == 17)
						{
							instance.FactoryId = binaryReader.ReadUInt64();
						}
						else if (num1 == 26)
						{
							instance.Player.Add(bnet.protocol.game_master.Player.DeserializeLengthDelimited(stream));
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

		public static CancelGameEntryRequest DeserializeLengthDelimited(Stream stream)
		{
			CancelGameEntryRequest cancelGameEntryRequest = new CancelGameEntryRequest();
			CancelGameEntryRequest.DeserializeLengthDelimited(stream, cancelGameEntryRequest);
			return cancelGameEntryRequest;
		}

		public static CancelGameEntryRequest DeserializeLengthDelimited(Stream stream, CancelGameEntryRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return CancelGameEntryRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			CancelGameEntryRequest cancelGameEntryRequest = obj as CancelGameEntryRequest;
			if (cancelGameEntryRequest == null)
			{
				return false;
			}
			if (!this.RequestId.Equals(cancelGameEntryRequest.RequestId))
			{
				return false;
			}
			if (this.HasFactoryId != cancelGameEntryRequest.HasFactoryId || this.HasFactoryId && !this.FactoryId.Equals(cancelGameEntryRequest.FactoryId))
			{
				return false;
			}
			if (this.Player.Count != cancelGameEntryRequest.Player.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Player.Count; i++)
			{
				if (!this.Player[i].Equals(cancelGameEntryRequest.Player[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.RequestId.GetHashCode();
			if (this.HasFactoryId)
			{
				hashCode ^= this.FactoryId.GetHashCode();
			}
			foreach (bnet.protocol.game_master.Player player in this.Player)
			{
				hashCode ^= player.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num += 8;
			if (this.HasFactoryId)
			{
				num++;
				num += 8;
			}
			if (this.Player.Count > 0)
			{
				foreach (bnet.protocol.game_master.Player player in this.Player)
				{
					num++;
					uint serializedSize = player.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			num++;
			return num;
		}

		public static CancelGameEntryRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<CancelGameEntryRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			CancelGameEntryRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, CancelGameEntryRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			stream.WriteByte(9);
			binaryWriter.Write(instance.RequestId);
			if (instance.HasFactoryId)
			{
				stream.WriteByte(17);
				binaryWriter.Write(instance.FactoryId);
			}
			if (instance.Player.Count > 0)
			{
				foreach (bnet.protocol.game_master.Player player in instance.Player)
				{
					stream.WriteByte(26);
					ProtocolParser.WriteUInt32(stream, player.GetSerializedSize());
					bnet.protocol.game_master.Player.Serialize(stream, player);
				}
			}
		}

		public void SetFactoryId(ulong val)
		{
			this.FactoryId = val;
		}

		public void SetPlayer(List<bnet.protocol.game_master.Player> val)
		{
			this.Player = val;
		}

		public void SetRequestId(ulong val)
		{
			this.RequestId = val;
		}
	}
}