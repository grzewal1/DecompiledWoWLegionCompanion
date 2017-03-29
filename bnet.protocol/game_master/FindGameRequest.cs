using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.game_master
{
	public class FindGameRequest : IProtoBuf
	{
		private List<bnet.protocol.game_master.Player> _Player = new List<bnet.protocol.game_master.Player>();

		public bool HasFactoryId;

		private ulong _FactoryId;

		public bool HasProperties;

		private GameProperties _Properties;

		public bool HasObjectId;

		private ulong _ObjectId;

		public bool HasRequestId;

		private ulong _RequestId;

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

		public ulong ObjectId
		{
			get
			{
				return this._ObjectId;
			}
			set
			{
				this._ObjectId = value;
				this.HasObjectId = true;
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

		public GameProperties Properties
		{
			get
			{
				return this._Properties;
			}
			set
			{
				this._Properties = value;
				this.HasProperties = value != null;
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

		public FindGameRequest()
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
			FindGameRequest.Deserialize(stream, this);
		}

		public static FindGameRequest Deserialize(Stream stream, FindGameRequest instance)
		{
			return FindGameRequest.Deserialize(stream, instance, (long)-1);
		}

		public static FindGameRequest Deserialize(Stream stream, FindGameRequest instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
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
					if (num != -1)
					{
						int num1 = num;
						if (num1 == 10)
						{
							instance.Player.Add(bnet.protocol.game_master.Player.DeserializeLengthDelimited(stream));
						}
						else if (num1 == 17)
						{
							instance.FactoryId = binaryReader.ReadUInt64();
						}
						else if (num1 == 26)
						{
							if (instance.Properties != null)
							{
								GameProperties.DeserializeLengthDelimited(stream, instance.Properties);
							}
							else
							{
								instance.Properties = GameProperties.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 32)
						{
							instance.ObjectId = ProtocolParser.ReadUInt64(stream);
						}
						else if (num1 == 41)
						{
							instance.RequestId = binaryReader.ReadUInt64();
						}
						else if (num1 == 48)
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

		public static FindGameRequest DeserializeLengthDelimited(Stream stream)
		{
			FindGameRequest findGameRequest = new FindGameRequest();
			FindGameRequest.DeserializeLengthDelimited(stream, findGameRequest);
			return findGameRequest;
		}

		public static FindGameRequest DeserializeLengthDelimited(Stream stream, FindGameRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return FindGameRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			FindGameRequest findGameRequest = obj as FindGameRequest;
			if (findGameRequest == null)
			{
				return false;
			}
			if (this.Player.Count != findGameRequest.Player.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Player.Count; i++)
			{
				if (!this.Player[i].Equals(findGameRequest.Player[i]))
				{
					return false;
				}
			}
			if (this.HasFactoryId != findGameRequest.HasFactoryId || this.HasFactoryId && !this.FactoryId.Equals(findGameRequest.FactoryId))
			{
				return false;
			}
			if (this.HasProperties != findGameRequest.HasProperties || this.HasProperties && !this.Properties.Equals(findGameRequest.Properties))
			{
				return false;
			}
			if (this.HasObjectId != findGameRequest.HasObjectId || this.HasObjectId && !this.ObjectId.Equals(findGameRequest.ObjectId))
			{
				return false;
			}
			if (this.HasRequestId != findGameRequest.HasRequestId || this.HasRequestId && !this.RequestId.Equals(findGameRequest.RequestId))
			{
				return false;
			}
			if (this.HasAdvancedNotification == findGameRequest.HasAdvancedNotification && (!this.HasAdvancedNotification || this.AdvancedNotification.Equals(findGameRequest.AdvancedNotification)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (bnet.protocol.game_master.Player player in this.Player)
			{
				hashCode = hashCode ^ player.GetHashCode();
			}
			if (this.HasFactoryId)
			{
				hashCode = hashCode ^ this.FactoryId.GetHashCode();
			}
			if (this.HasProperties)
			{
				hashCode = hashCode ^ this.Properties.GetHashCode();
			}
			if (this.HasObjectId)
			{
				hashCode = hashCode ^ this.ObjectId.GetHashCode();
			}
			if (this.HasRequestId)
			{
				hashCode = hashCode ^ this.RequestId.GetHashCode();
			}
			if (this.HasAdvancedNotification)
			{
				hashCode = hashCode ^ this.AdvancedNotification.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Player.Count > 0)
			{
				foreach (bnet.protocol.game_master.Player player in this.Player)
				{
					num++;
					uint serializedSize = player.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			if (this.HasFactoryId)
			{
				num++;
				num = num + 8;
			}
			if (this.HasProperties)
			{
				num++;
				uint serializedSize1 = this.Properties.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			if (this.HasObjectId)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt64(this.ObjectId);
			}
			if (this.HasRequestId)
			{
				num++;
				num = num + 8;
			}
			if (this.HasAdvancedNotification)
			{
				num++;
				num++;
			}
			return num;
		}

		public static FindGameRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<FindGameRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			FindGameRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, FindGameRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.Player.Count > 0)
			{
				foreach (bnet.protocol.game_master.Player player in instance.Player)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, player.GetSerializedSize());
					bnet.protocol.game_master.Player.Serialize(stream, player);
				}
			}
			if (instance.HasFactoryId)
			{
				stream.WriteByte(17);
				binaryWriter.Write(instance.FactoryId);
			}
			if (instance.HasProperties)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteUInt32(stream, instance.Properties.GetSerializedSize());
				GameProperties.Serialize(stream, instance.Properties);
			}
			if (instance.HasObjectId)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteUInt64(stream, instance.ObjectId);
			}
			if (instance.HasRequestId)
			{
				stream.WriteByte(41);
				binaryWriter.Write(instance.RequestId);
			}
			if (instance.HasAdvancedNotification)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteBool(stream, instance.AdvancedNotification);
			}
		}

		public void SetAdvancedNotification(bool val)
		{
			this.AdvancedNotification = val;
		}

		public void SetFactoryId(ulong val)
		{
			this.FactoryId = val;
		}

		public void SetObjectId(ulong val)
		{
			this.ObjectId = val;
		}

		public void SetPlayer(List<bnet.protocol.game_master.Player> val)
		{
			this.Player = val;
		}

		public void SetProperties(GameProperties val)
		{
			this.Properties = val;
		}

		public void SetRequestId(ulong val)
		{
			this.RequestId = val;
		}
	}
}