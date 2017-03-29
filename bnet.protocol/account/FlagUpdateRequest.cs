using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.account
{
	public class FlagUpdateRequest : IProtoBuf
	{
		public bool HasRegion;

		private uint _Region;

		public AccountId Account
		{
			get;
			set;
		}

		public bool Active
		{
			get;
			set;
		}

		public ulong Flag
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

		public FlagUpdateRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			FlagUpdateRequest.Deserialize(stream, this);
		}

		public static FlagUpdateRequest Deserialize(Stream stream, FlagUpdateRequest instance)
		{
			return FlagUpdateRequest.Deserialize(stream, instance, (long)-1);
		}

		public static FlagUpdateRequest Deserialize(Stream stream, FlagUpdateRequest instance, long limit)
		{
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
							if (instance.Account != null)
							{
								AccountId.DeserializeLengthDelimited(stream, instance.Account);
							}
							else
							{
								instance.Account = AccountId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 16)
						{
							instance.Region = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 24)
						{
							instance.Flag = ProtocolParser.ReadUInt64(stream);
						}
						else if (num1 == 32)
						{
							instance.Active = ProtocolParser.ReadBool(stream);
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

		public static FlagUpdateRequest DeserializeLengthDelimited(Stream stream)
		{
			FlagUpdateRequest flagUpdateRequest = new FlagUpdateRequest();
			FlagUpdateRequest.DeserializeLengthDelimited(stream, flagUpdateRequest);
			return flagUpdateRequest;
		}

		public static FlagUpdateRequest DeserializeLengthDelimited(Stream stream, FlagUpdateRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return FlagUpdateRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			FlagUpdateRequest flagUpdateRequest = obj as FlagUpdateRequest;
			if (flagUpdateRequest == null)
			{
				return false;
			}
			if (!this.Account.Equals(flagUpdateRequest.Account))
			{
				return false;
			}
			if (this.HasRegion != flagUpdateRequest.HasRegion || this.HasRegion && !this.Region.Equals(flagUpdateRequest.Region))
			{
				return false;
			}
			if (!this.Flag.Equals(flagUpdateRequest.Flag))
			{
				return false;
			}
			if (!this.Active.Equals(flagUpdateRequest.Active))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.Account.GetHashCode();
			if (this.HasRegion)
			{
				hashCode = hashCode ^ this.Region.GetHashCode();
			}
			hashCode = hashCode ^ this.Flag.GetHashCode();
			hashCode = hashCode ^ this.Active.GetHashCode();
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.Account.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasRegion)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.Region);
			}
			num = num + ProtocolParser.SizeOfUInt64(this.Flag);
			num++;
			num = num + 3;
			return num;
		}

		public static FlagUpdateRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<FlagUpdateRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			FlagUpdateRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, FlagUpdateRequest instance)
		{
			if (instance.Account == null)
			{
				throw new ArgumentNullException("Account", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.Account.GetSerializedSize());
			AccountId.Serialize(stream, instance.Account);
			if (instance.HasRegion)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.Region);
			}
			stream.WriteByte(24);
			ProtocolParser.WriteUInt64(stream, instance.Flag);
			stream.WriteByte(32);
			ProtocolParser.WriteBool(stream, instance.Active);
		}

		public void SetAccount(AccountId val)
		{
			this.Account = val;
		}

		public void SetActive(bool val)
		{
			this.Active = val;
		}

		public void SetFlag(ulong val)
		{
			this.Flag = val;
		}

		public void SetRegion(uint val)
		{
			this.Region = val;
		}
	}
}