using System;
using System.IO;

namespace bnet.protocol
{
	public class Identity : IProtoBuf
	{
		public bool HasAccountId;

		private EntityId _AccountId;

		public bool HasGameAccountId;

		private EntityId _GameAccountId;

		public EntityId AccountId
		{
			get
			{
				return this._AccountId;
			}
			set
			{
				this._AccountId = value;
				this.HasAccountId = value != null;
			}
		}

		public EntityId GameAccountId
		{
			get
			{
				return this._GameAccountId;
			}
			set
			{
				this._GameAccountId = value;
				this.HasGameAccountId = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public Identity()
		{
		}

		public void Deserialize(Stream stream)
		{
			Identity.Deserialize(stream, this);
		}

		public static Identity Deserialize(Stream stream, Identity instance)
		{
			return Identity.Deserialize(stream, instance, (long)-1);
		}

		public static Identity Deserialize(Stream stream, Identity instance, long limit)
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
							if (instance.AccountId != null)
							{
								EntityId.DeserializeLengthDelimited(stream, instance.AccountId);
							}
							else
							{
								instance.AccountId = EntityId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 != 18)
						{
							Key key = ProtocolParser.ReadKey((byte)num, stream);
							if (key.Field == 0)
							{
								throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
							}
							ProtocolParser.SkipKey(stream, key);
						}
						else if (instance.GameAccountId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.GameAccountId);
						}
						else
						{
							instance.GameAccountId = EntityId.DeserializeLengthDelimited(stream);
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

		public static Identity DeserializeLengthDelimited(Stream stream)
		{
			Identity identity = new Identity();
			Identity.DeserializeLengthDelimited(stream, identity);
			return identity;
		}

		public static Identity DeserializeLengthDelimited(Stream stream, Identity instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return Identity.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			Identity identity = obj as Identity;
			if (identity == null)
			{
				return false;
			}
			if (this.HasAccountId != identity.HasAccountId || this.HasAccountId && !this.AccountId.Equals(identity.AccountId))
			{
				return false;
			}
			if (this.HasGameAccountId == identity.HasGameAccountId && (!this.HasGameAccountId || this.GameAccountId.Equals(identity.GameAccountId)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasAccountId)
			{
				hashCode = hashCode ^ this.AccountId.GetHashCode();
			}
			if (this.HasGameAccountId)
			{
				hashCode = hashCode ^ this.GameAccountId.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasAccountId)
			{
				num++;
				uint serializedSize = this.AccountId.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasGameAccountId)
			{
				num++;
				uint serializedSize1 = this.GameAccountId.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			return num;
		}

		public static Identity ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<Identity>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			Identity.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, Identity instance)
		{
			if (instance.HasAccountId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AccountId.GetSerializedSize());
				EntityId.Serialize(stream, instance.AccountId);
			}
			if (instance.HasGameAccountId)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.GameAccountId.GetSerializedSize());
				EntityId.Serialize(stream, instance.GameAccountId);
			}
		}

		public void SetAccountId(EntityId val)
		{
			this.AccountId = val;
		}

		public void SetGameAccountId(EntityId val)
		{
			this.GameAccountId = val;
		}
	}
}