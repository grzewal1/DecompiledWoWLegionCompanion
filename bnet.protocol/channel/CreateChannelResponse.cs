using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel
{
	public class CreateChannelResponse : IProtoBuf
	{
		public bool HasChannelId;

		private EntityId _ChannelId;

		public EntityId ChannelId
		{
			get
			{
				return this._ChannelId;
			}
			set
			{
				this._ChannelId = value;
				this.HasChannelId = value != null;
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
			get;
			set;
		}

		public CreateChannelResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			CreateChannelResponse.Deserialize(stream, this);
		}

		public static CreateChannelResponse Deserialize(Stream stream, CreateChannelResponse instance)
		{
			return CreateChannelResponse.Deserialize(stream, instance, (long)-1);
		}

		public static CreateChannelResponse Deserialize(Stream stream, CreateChannelResponse instance, long limit)
		{
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 == 8)
						{
							instance.ObjectId = ProtocolParser.ReadUInt64(stream);
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
						else if (instance.ChannelId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.ChannelId);
						}
						else
						{
							instance.ChannelId = EntityId.DeserializeLengthDelimited(stream);
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

		public static CreateChannelResponse DeserializeLengthDelimited(Stream stream)
		{
			CreateChannelResponse createChannelResponse = new CreateChannelResponse();
			CreateChannelResponse.DeserializeLengthDelimited(stream, createChannelResponse);
			return createChannelResponse;
		}

		public static CreateChannelResponse DeserializeLengthDelimited(Stream stream, CreateChannelResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return CreateChannelResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			CreateChannelResponse createChannelResponse = obj as CreateChannelResponse;
			if (createChannelResponse == null)
			{
				return false;
			}
			if (!this.ObjectId.Equals(createChannelResponse.ObjectId))
			{
				return false;
			}
			if (this.HasChannelId == createChannelResponse.HasChannelId && (!this.HasChannelId || this.ChannelId.Equals(createChannelResponse.ChannelId)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.ObjectId.GetHashCode();
			if (this.HasChannelId)
			{
				hashCode = hashCode ^ this.ChannelId.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num = num + ProtocolParser.SizeOfUInt64(this.ObjectId);
			if (this.HasChannelId)
			{
				num++;
				uint serializedSize = this.ChannelId.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			num++;
			return num;
		}

		public static CreateChannelResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<CreateChannelResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			CreateChannelResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, CreateChannelResponse instance)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, instance.ObjectId);
			if (instance.HasChannelId)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.ChannelId.GetSerializedSize());
				EntityId.Serialize(stream, instance.ChannelId);
			}
		}

		public void SetChannelId(EntityId val)
		{
			this.ChannelId = val;
		}

		public void SetObjectId(ulong val)
		{
			this.ObjectId = val;
		}
	}
}