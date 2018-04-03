using bnet.protocol;
using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GetGameAccountStateRequest : IProtoBuf
	{
		public bool HasAccountId;

		private EntityId _AccountId;

		public bool HasGameAccountId;

		private EntityId _GameAccountId;

		public bool HasOptions;

		private GameAccountFieldOptions _Options;

		public bool HasTags;

		private GameAccountFieldTags _Tags;

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

		public GameAccountFieldOptions Options
		{
			get
			{
				return this._Options;
			}
			set
			{
				this._Options = value;
				this.HasOptions = value != null;
			}
		}

		public GameAccountFieldTags Tags
		{
			get
			{
				return this._Tags;
			}
			set
			{
				this._Tags = value;
				this.HasTags = value != null;
			}
		}

		public GetGameAccountStateRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetGameAccountStateRequest.Deserialize(stream, this);
		}

		public static GetGameAccountStateRequest Deserialize(Stream stream, GetGameAccountStateRequest instance)
		{
			return GetGameAccountStateRequest.Deserialize(stream, instance, (long)-1);
		}

		public static GetGameAccountStateRequest Deserialize(Stream stream, GetGameAccountStateRequest instance, long limit)
		{
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
						if (instance.AccountId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.AccountId);
						}
						else
						{
							instance.AccountId = EntityId.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 18)
					{
						if (instance.GameAccountId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.GameAccountId);
						}
						else
						{
							instance.GameAccountId = EntityId.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 82)
					{
						if (instance.Options != null)
						{
							GameAccountFieldOptions.DeserializeLengthDelimited(stream, instance.Options);
						}
						else
						{
							instance.Options = GameAccountFieldOptions.DeserializeLengthDelimited(stream);
						}
					}
					else if (num != 90)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.Tags != null)
					{
						GameAccountFieldTags.DeserializeLengthDelimited(stream, instance.Tags);
					}
					else
					{
						instance.Tags = GameAccountFieldTags.DeserializeLengthDelimited(stream);
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

		public static GetGameAccountStateRequest DeserializeLengthDelimited(Stream stream)
		{
			GetGameAccountStateRequest getGameAccountStateRequest = new GetGameAccountStateRequest();
			GetGameAccountStateRequest.DeserializeLengthDelimited(stream, getGameAccountStateRequest);
			return getGameAccountStateRequest;
		}

		public static GetGameAccountStateRequest DeserializeLengthDelimited(Stream stream, GetGameAccountStateRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GetGameAccountStateRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetGameAccountStateRequest getGameAccountStateRequest = obj as GetGameAccountStateRequest;
			if (getGameAccountStateRequest == null)
			{
				return false;
			}
			if (this.HasAccountId != getGameAccountStateRequest.HasAccountId || this.HasAccountId && !this.AccountId.Equals(getGameAccountStateRequest.AccountId))
			{
				return false;
			}
			if (this.HasGameAccountId != getGameAccountStateRequest.HasGameAccountId || this.HasGameAccountId && !this.GameAccountId.Equals(getGameAccountStateRequest.GameAccountId))
			{
				return false;
			}
			if (this.HasOptions != getGameAccountStateRequest.HasOptions || this.HasOptions && !this.Options.Equals(getGameAccountStateRequest.Options))
			{
				return false;
			}
			if (this.HasTags == getGameAccountStateRequest.HasTags && (!this.HasTags || this.Tags.Equals(getGameAccountStateRequest.Tags)))
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
				hashCode ^= this.AccountId.GetHashCode();
			}
			if (this.HasGameAccountId)
			{
				hashCode ^= this.GameAccountId.GetHashCode();
			}
			if (this.HasOptions)
			{
				hashCode ^= this.Options.GetHashCode();
			}
			if (this.HasTags)
			{
				hashCode ^= this.Tags.GetHashCode();
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
			if (this.HasOptions)
			{
				num++;
				uint num1 = this.Options.GetSerializedSize();
				num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
			}
			if (this.HasTags)
			{
				num++;
				uint serializedSize2 = this.Tags.GetSerializedSize();
				num = num + serializedSize2 + ProtocolParser.SizeOfUInt32(serializedSize2);
			}
			return num;
		}

		public static GetGameAccountStateRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetGameAccountStateRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetGameAccountStateRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetGameAccountStateRequest instance)
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
			if (instance.HasOptions)
			{
				stream.WriteByte(82);
				ProtocolParser.WriteUInt32(stream, instance.Options.GetSerializedSize());
				GameAccountFieldOptions.Serialize(stream, instance.Options);
			}
			if (instance.HasTags)
			{
				stream.WriteByte(90);
				ProtocolParser.WriteUInt32(stream, instance.Tags.GetSerializedSize());
				GameAccountFieldTags.Serialize(stream, instance.Tags);
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

		public void SetOptions(GameAccountFieldOptions val)
		{
			this.Options = val;
		}

		public void SetTags(GameAccountFieldTags val)
		{
			this.Tags = val;
		}
	}
}