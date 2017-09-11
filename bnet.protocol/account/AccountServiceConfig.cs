using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.account
{
	public class AccountServiceConfig : IProtoBuf
	{
		private List<AccountServiceRegion> _Region = new List<AccountServiceRegion>();

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public List<AccountServiceRegion> Region
		{
			get
			{
				return this._Region;
			}
			set
			{
				this._Region = value;
			}
		}

		public int RegionCount
		{
			get
			{
				return this._Region.Count;
			}
		}

		public List<AccountServiceRegion> RegionList
		{
			get
			{
				return this._Region;
			}
		}

		public AccountServiceConfig()
		{
		}

		public void AddRegion(AccountServiceRegion val)
		{
			this._Region.Add(val);
		}

		public void ClearRegion()
		{
			this._Region.Clear();
		}

		public void Deserialize(Stream stream)
		{
			AccountServiceConfig.Deserialize(stream, this);
		}

		public static AccountServiceConfig Deserialize(Stream stream, AccountServiceConfig instance)
		{
			return AccountServiceConfig.Deserialize(stream, instance, (long)-1);
		}

		public static AccountServiceConfig Deserialize(Stream stream, AccountServiceConfig instance, long limit)
		{
			if (instance.Region == null)
			{
				instance.Region = new List<AccountServiceRegion>();
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
						instance.Region.Add(AccountServiceRegion.DeserializeLengthDelimited(stream));
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

		public static AccountServiceConfig DeserializeLengthDelimited(Stream stream)
		{
			AccountServiceConfig accountServiceConfig = new AccountServiceConfig();
			AccountServiceConfig.DeserializeLengthDelimited(stream, accountServiceConfig);
			return accountServiceConfig;
		}

		public static AccountServiceConfig DeserializeLengthDelimited(Stream stream, AccountServiceConfig instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return AccountServiceConfig.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			AccountServiceConfig accountServiceConfig = obj as AccountServiceConfig;
			if (accountServiceConfig == null)
			{
				return false;
			}
			if (this.Region.Count != accountServiceConfig.Region.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Region.Count; i++)
			{
				if (!this.Region[i].Equals(accountServiceConfig.Region[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (AccountServiceRegion region in this.Region)
			{
				hashCode ^= region.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Region.Count > 0)
			{
				foreach (AccountServiceRegion region in this.Region)
				{
					num++;
					uint serializedSize = region.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			return num;
		}

		public static AccountServiceConfig ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<AccountServiceConfig>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			AccountServiceConfig.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, AccountServiceConfig instance)
		{
			if (instance.Region.Count > 0)
			{
				foreach (AccountServiceRegion region in instance.Region)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, region.GetSerializedSize());
					AccountServiceRegion.Serialize(stream, region);
				}
			}
		}

		public void SetRegion(List<AccountServiceRegion> val)
		{
			this.Region = val;
		}
	}
}