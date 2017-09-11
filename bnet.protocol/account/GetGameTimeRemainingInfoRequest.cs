using bnet.protocol;
using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GetGameTimeRemainingInfoRequest : IProtoBuf
	{
		public bool HasGameAccountId;

		private EntityId _GameAccountId;

		public bool HasAccountId;

		private EntityId _AccountId;

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

		public GetGameTimeRemainingInfoRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetGameTimeRemainingInfoRequest.Deserialize(stream, this);
		}

		public static GetGameTimeRemainingInfoRequest Deserialize(Stream stream, GetGameTimeRemainingInfoRequest instance)
		{
			return GetGameTimeRemainingInfoRequest.Deserialize(stream, instance, (long)-1);
		}

		public static GetGameTimeRemainingInfoRequest Deserialize(Stream stream, GetGameTimeRemainingInfoRequest instance, long limit)
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
							if (instance.GameAccountId != null)
							{
								EntityId.DeserializeLengthDelimited(stream, instance.GameAccountId);
							}
							else
							{
								instance.GameAccountId = EntityId.DeserializeLengthDelimited(stream);
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
						else if (instance.AccountId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.AccountId);
						}
						else
						{
							instance.AccountId = EntityId.DeserializeLengthDelimited(stream);
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

		public static GetGameTimeRemainingInfoRequest DeserializeLengthDelimited(Stream stream)
		{
			GetGameTimeRemainingInfoRequest getGameTimeRemainingInfoRequest = new GetGameTimeRemainingInfoRequest();
			GetGameTimeRemainingInfoRequest.DeserializeLengthDelimited(stream, getGameTimeRemainingInfoRequest);
			return getGameTimeRemainingInfoRequest;
		}

		public static GetGameTimeRemainingInfoRequest DeserializeLengthDelimited(Stream stream, GetGameTimeRemainingInfoRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GetGameTimeRemainingInfoRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetGameTimeRemainingInfoRequest getGameTimeRemainingInfoRequest = obj as GetGameTimeRemainingInfoRequest;
			if (getGameTimeRemainingInfoRequest == null)
			{
				return false;
			}
			if (this.HasGameAccountId != getGameTimeRemainingInfoRequest.HasGameAccountId || this.HasGameAccountId && !this.GameAccountId.Equals(getGameTimeRemainingInfoRequest.GameAccountId))
			{
				return false;
			}
			if (this.HasAccountId == getGameTimeRemainingInfoRequest.HasAccountId && (!this.HasAccountId || this.AccountId.Equals(getGameTimeRemainingInfoRequest.AccountId)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasGameAccountId)
			{
				hashCode ^= this.GameAccountId.GetHashCode();
			}
			if (this.HasAccountId)
			{
				hashCode ^= this.AccountId.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasGameAccountId)
			{
				num++;
				uint serializedSize = this.GameAccountId.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasAccountId)
			{
				num++;
				uint serializedSize1 = this.AccountId.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			return num;
		}

		public static GetGameTimeRemainingInfoRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetGameTimeRemainingInfoRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetGameTimeRemainingInfoRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetGameTimeRemainingInfoRequest instance)
		{
			if (instance.HasGameAccountId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.GameAccountId.GetSerializedSize());
				EntityId.Serialize(stream, instance.GameAccountId);
			}
			if (instance.HasAccountId)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.AccountId.GetSerializedSize());
				EntityId.Serialize(stream, instance.AccountId);
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