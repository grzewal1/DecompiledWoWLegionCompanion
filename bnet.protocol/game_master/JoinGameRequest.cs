using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.game_master
{
	public class JoinGameRequest : IProtoBuf
	{
		private List<bnet.protocol.game_master.Player> _Player = new List<bnet.protocol.game_master.Player>();

		public bool HasAdvancedNotification;

		private bool _AdvancedNotification;

		public bool AdvancedNotification
		{
			get
			{
				return this._AdvancedNotification;
			}
			set
			{
				this._AdvancedNotification = value;
				this.HasAdvancedNotification = true;
			}
		}

		public bnet.protocol.game_master.GameHandle GameHandle
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

		public JoinGameRequest()
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
			JoinGameRequest.Deserialize(stream, this);
		}

		public static JoinGameRequest Deserialize(Stream stream, JoinGameRequest instance)
		{
			return JoinGameRequest.Deserialize(stream, instance, (long)-1);
		}

		public static JoinGameRequest Deserialize(Stream stream, JoinGameRequest instance, long limit)
		{
			if (instance.Player == null)
			{
				instance.Player = new List<bnet.protocol.game_master.Player>();
			}
			instance.AdvancedNotification = false;
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
						if (instance.GameHandle != null)
						{
							bnet.protocol.game_master.GameHandle.DeserializeLengthDelimited(stream, instance.GameHandle);
						}
						else
						{
							instance.GameHandle = bnet.protocol.game_master.GameHandle.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 18)
					{
						instance.Player.Add(bnet.protocol.game_master.Player.DeserializeLengthDelimited(stream));
					}
					else if (num == 24)
					{
						instance.AdvancedNotification = ProtocolParser.ReadBool(stream);
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

		public static JoinGameRequest DeserializeLengthDelimited(Stream stream)
		{
			JoinGameRequest joinGameRequest = new JoinGameRequest();
			JoinGameRequest.DeserializeLengthDelimited(stream, joinGameRequest);
			return joinGameRequest;
		}

		public static JoinGameRequest DeserializeLengthDelimited(Stream stream, JoinGameRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return JoinGameRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			JoinGameRequest joinGameRequest = obj as JoinGameRequest;
			if (joinGameRequest == null)
			{
				return false;
			}
			if (!this.GameHandle.Equals(joinGameRequest.GameHandle))
			{
				return false;
			}
			if (this.Player.Count != joinGameRequest.Player.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Player.Count; i++)
			{
				if (!this.Player[i].Equals(joinGameRequest.Player[i]))
				{
					return false;
				}
			}
			if (this.HasAdvancedNotification == joinGameRequest.HasAdvancedNotification && (!this.HasAdvancedNotification || this.AdvancedNotification.Equals(joinGameRequest.AdvancedNotification)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.GameHandle.GetHashCode();
			foreach (bnet.protocol.game_master.Player player in this.Player)
			{
				hashCode ^= player.GetHashCode();
			}
			if (this.HasAdvancedNotification)
			{
				hashCode ^= this.AdvancedNotification.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.GameHandle.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.Player.Count > 0)
			{
				foreach (bnet.protocol.game_master.Player player in this.Player)
				{
					num++;
					uint serializedSize1 = player.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			if (this.HasAdvancedNotification)
			{
				num++;
				num++;
			}
			num++;
			return num;
		}

		public static JoinGameRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<JoinGameRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			JoinGameRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, JoinGameRequest instance)
		{
			if (instance.GameHandle == null)
			{
				throw new ArgumentNullException("GameHandle", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.GameHandle.GetSerializedSize());
			bnet.protocol.game_master.GameHandle.Serialize(stream, instance.GameHandle);
			if (instance.Player.Count > 0)
			{
				foreach (bnet.protocol.game_master.Player player in instance.Player)
				{
					stream.WriteByte(18);
					ProtocolParser.WriteUInt32(stream, player.GetSerializedSize());
					bnet.protocol.game_master.Player.Serialize(stream, player);
				}
			}
			if (instance.HasAdvancedNotification)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteBool(stream, instance.AdvancedNotification);
			}
		}

		public void SetAdvancedNotification(bool val)
		{
			this.AdvancedNotification = val;
		}

		public void SetGameHandle(bnet.protocol.game_master.GameHandle val)
		{
			this.GameHandle = val;
		}

		public void SetPlayer(List<bnet.protocol.game_master.Player> val)
		{
			this.Player = val;
		}
	}
}