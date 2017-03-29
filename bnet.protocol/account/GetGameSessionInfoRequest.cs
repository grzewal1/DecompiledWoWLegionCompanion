using bnet.protocol;
using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GetGameSessionInfoRequest : IProtoBuf
	{
		public bool HasEntityId;

		private bnet.protocol.EntityId _EntityId;

		public bnet.protocol.EntityId EntityId
		{
			get
			{
				return this._EntityId;
			}
			set
			{
				this._EntityId = value;
				this.HasEntityId = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public GetGameSessionInfoRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetGameSessionInfoRequest.Deserialize(stream, this);
		}

		public static GetGameSessionInfoRequest Deserialize(Stream stream, GetGameSessionInfoRequest instance)
		{
			return GetGameSessionInfoRequest.Deserialize(stream, instance, (long)-1);
		}

		public static GetGameSessionInfoRequest Deserialize(Stream stream, GetGameSessionInfoRequest instance, long limit)
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
					else if (num != 10)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.EntityId != null)
					{
						bnet.protocol.EntityId.DeserializeLengthDelimited(stream, instance.EntityId);
					}
					else
					{
						instance.EntityId = bnet.protocol.EntityId.DeserializeLengthDelimited(stream);
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

		public static GetGameSessionInfoRequest DeserializeLengthDelimited(Stream stream)
		{
			GetGameSessionInfoRequest getGameSessionInfoRequest = new GetGameSessionInfoRequest();
			GetGameSessionInfoRequest.DeserializeLengthDelimited(stream, getGameSessionInfoRequest);
			return getGameSessionInfoRequest;
		}

		public static GetGameSessionInfoRequest DeserializeLengthDelimited(Stream stream, GetGameSessionInfoRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GetGameSessionInfoRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetGameSessionInfoRequest getGameSessionInfoRequest = obj as GetGameSessionInfoRequest;
			if (getGameSessionInfoRequest == null)
			{
				return false;
			}
			if (this.HasEntityId == getGameSessionInfoRequest.HasEntityId && (!this.HasEntityId || this.EntityId.Equals(getGameSessionInfoRequest.EntityId)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasEntityId)
			{
				hashCode = hashCode ^ this.EntityId.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasEntityId)
			{
				num++;
				uint serializedSize = this.EntityId.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			return num;
		}

		public static GetGameSessionInfoRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetGameSessionInfoRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetGameSessionInfoRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetGameSessionInfoRequest instance)
		{
			if (instance.HasEntityId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.EntityId.GetSerializedSize());
				bnet.protocol.EntityId.Serialize(stream, instance.EntityId);
			}
		}

		public void SetEntityId(bnet.protocol.EntityId val)
		{
			this.EntityId = val;
		}
	}
}