using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.account
{
	public class GameAccountNotification : IProtoBuf
	{
		private List<GameAccountList> _RegionDelta = new List<GameAccountList>();

		public bool HasSubscriberId;

		private ulong _SubscriberId;

		public bool HasAccountTags;

		private AccountFieldTags _AccountTags;

		public AccountFieldTags AccountTags
		{
			get
			{
				return this._AccountTags;
			}
			set
			{
				this._AccountTags = value;
				this.HasAccountTags = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public List<GameAccountList> RegionDelta
		{
			get
			{
				return this._RegionDelta;
			}
			set
			{
				this._RegionDelta = value;
			}
		}

		public int RegionDeltaCount
		{
			get
			{
				return this._RegionDelta.Count;
			}
		}

		public List<GameAccountList> RegionDeltaList
		{
			get
			{
				return this._RegionDelta;
			}
		}

		public ulong SubscriberId
		{
			get
			{
				return this._SubscriberId;
			}
			set
			{
				this._SubscriberId = value;
				this.HasSubscriberId = true;
			}
		}

		public GameAccountNotification()
		{
		}

		public void AddRegionDelta(GameAccountList val)
		{
			this._RegionDelta.Add(val);
		}

		public void ClearRegionDelta()
		{
			this._RegionDelta.Clear();
		}

		public void Deserialize(Stream stream)
		{
			GameAccountNotification.Deserialize(stream, this);
		}

		public static GameAccountNotification Deserialize(Stream stream, GameAccountNotification instance)
		{
			return GameAccountNotification.Deserialize(stream, instance, (long)-1);
		}

		public static GameAccountNotification Deserialize(Stream stream, GameAccountNotification instance, long limit)
		{
			if (instance.RegionDelta == null)
			{
				instance.RegionDelta = new List<GameAccountList>();
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
							instance.RegionDelta.Add(GameAccountList.DeserializeLengthDelimited(stream));
						}
						else if (num1 == 16)
						{
							instance.SubscriberId = ProtocolParser.ReadUInt64(stream);
						}
						else if (num1 != 26)
						{
							Key key = ProtocolParser.ReadKey((byte)num, stream);
							if (key.Field == 0)
							{
								throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
							}
							ProtocolParser.SkipKey(stream, key);
						}
						else if (instance.AccountTags != null)
						{
							AccountFieldTags.DeserializeLengthDelimited(stream, instance.AccountTags);
						}
						else
						{
							instance.AccountTags = AccountFieldTags.DeserializeLengthDelimited(stream);
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

		public static GameAccountNotification DeserializeLengthDelimited(Stream stream)
		{
			GameAccountNotification gameAccountNotification = new GameAccountNotification();
			GameAccountNotification.DeserializeLengthDelimited(stream, gameAccountNotification);
			return gameAccountNotification;
		}

		public static GameAccountNotification DeserializeLengthDelimited(Stream stream, GameAccountNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GameAccountNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameAccountNotification gameAccountNotification = obj as GameAccountNotification;
			if (gameAccountNotification == null)
			{
				return false;
			}
			if (this.RegionDelta.Count != gameAccountNotification.RegionDelta.Count)
			{
				return false;
			}
			for (int i = 0; i < this.RegionDelta.Count; i++)
			{
				if (!this.RegionDelta[i].Equals(gameAccountNotification.RegionDelta[i]))
				{
					return false;
				}
			}
			if (this.HasSubscriberId != gameAccountNotification.HasSubscriberId || this.HasSubscriberId && !this.SubscriberId.Equals(gameAccountNotification.SubscriberId))
			{
				return false;
			}
			if (this.HasAccountTags == gameAccountNotification.HasAccountTags && (!this.HasAccountTags || this.AccountTags.Equals(gameAccountNotification.AccountTags)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (GameAccountList regionDeltum in this.RegionDelta)
			{
				hashCode ^= regionDeltum.GetHashCode();
			}
			if (this.HasSubscriberId)
			{
				hashCode ^= this.SubscriberId.GetHashCode();
			}
			if (this.HasAccountTags)
			{
				hashCode ^= this.AccountTags.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.RegionDelta.Count > 0)
			{
				foreach (GameAccountList regionDeltum in this.RegionDelta)
				{
					num++;
					uint serializedSize = regionDeltum.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			if (this.HasSubscriberId)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.SubscriberId);
			}
			if (this.HasAccountTags)
			{
				num++;
				uint serializedSize1 = this.AccountTags.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			return num;
		}

		public static GameAccountNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameAccountNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameAccountNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameAccountNotification instance)
		{
			if (instance.RegionDelta.Count > 0)
			{
				foreach (GameAccountList regionDeltum in instance.RegionDelta)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, regionDeltum.GetSerializedSize());
					GameAccountList.Serialize(stream, regionDeltum);
				}
			}
			if (instance.HasSubscriberId)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, instance.SubscriberId);
			}
			if (instance.HasAccountTags)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteUInt32(stream, instance.AccountTags.GetSerializedSize());
				AccountFieldTags.Serialize(stream, instance.AccountTags);
			}
		}

		public void SetAccountTags(AccountFieldTags val)
		{
			this.AccountTags = val;
		}

		public void SetRegionDelta(List<GameAccountList> val)
		{
			this.RegionDelta = val;
		}

		public void SetSubscriberId(ulong val)
		{
			this.SubscriberId = val;
		}
	}
}