using System;
using System.IO;

namespace bnet.protocol.channel
{
	public class SubscribeChannelResponse : IProtoBuf
	{
		public bool HasObjectId;

		private ulong _ObjectId;

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

		public SubscribeChannelResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			SubscribeChannelResponse.Deserialize(stream, this);
		}

		public static SubscribeChannelResponse Deserialize(Stream stream, SubscribeChannelResponse instance)
		{
			return SubscribeChannelResponse.Deserialize(stream, instance, (long)-1);
		}

		public static SubscribeChannelResponse Deserialize(Stream stream, SubscribeChannelResponse instance, long limit)
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
					else if (num == 8)
					{
						instance.ObjectId = ProtocolParser.ReadUInt64(stream);
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

		public static SubscribeChannelResponse DeserializeLengthDelimited(Stream stream)
		{
			SubscribeChannelResponse subscribeChannelResponse = new SubscribeChannelResponse();
			SubscribeChannelResponse.DeserializeLengthDelimited(stream, subscribeChannelResponse);
			return subscribeChannelResponse;
		}

		public static SubscribeChannelResponse DeserializeLengthDelimited(Stream stream, SubscribeChannelResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return SubscribeChannelResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			SubscribeChannelResponse subscribeChannelResponse = obj as SubscribeChannelResponse;
			if (subscribeChannelResponse == null)
			{
				return false;
			}
			if (this.HasObjectId == subscribeChannelResponse.HasObjectId && (!this.HasObjectId || this.ObjectId.Equals(subscribeChannelResponse.ObjectId)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasObjectId)
			{
				hashCode = hashCode ^ this.ObjectId.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasObjectId)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt64(this.ObjectId);
			}
			return num;
		}

		public static SubscribeChannelResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<SubscribeChannelResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			SubscribeChannelResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, SubscribeChannelResponse instance)
		{
			if (instance.HasObjectId)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, instance.ObjectId);
			}
		}

		public void SetObjectId(ulong val)
		{
			this.ObjectId = val;
		}
	}
}