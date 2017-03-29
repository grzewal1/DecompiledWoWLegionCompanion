using System;
using System.IO;

namespace bnet.protocol.game_master
{
	public class SubscribeResponse : IProtoBuf
	{
		public bool HasSubscriptionId;

		private ulong _SubscriptionId;

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public ulong SubscriptionId
		{
			get
			{
				return this._SubscriptionId;
			}
			set
			{
				this._SubscriptionId = value;
				this.HasSubscriptionId = true;
			}
		}

		public SubscribeResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			SubscribeResponse.Deserialize(stream, this);
		}

		public static SubscribeResponse Deserialize(Stream stream, SubscribeResponse instance)
		{
			return SubscribeResponse.Deserialize(stream, instance, (long)-1);
		}

		public static SubscribeResponse Deserialize(Stream stream, SubscribeResponse instance, long limit)
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
						instance.SubscriptionId = ProtocolParser.ReadUInt64(stream);
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

		public static SubscribeResponse DeserializeLengthDelimited(Stream stream)
		{
			SubscribeResponse subscribeResponse = new SubscribeResponse();
			SubscribeResponse.DeserializeLengthDelimited(stream, subscribeResponse);
			return subscribeResponse;
		}

		public static SubscribeResponse DeserializeLengthDelimited(Stream stream, SubscribeResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return SubscribeResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			SubscribeResponse subscribeResponse = obj as SubscribeResponse;
			if (subscribeResponse == null)
			{
				return false;
			}
			if (this.HasSubscriptionId == subscribeResponse.HasSubscriptionId && (!this.HasSubscriptionId || this.SubscriptionId.Equals(subscribeResponse.SubscriptionId)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasSubscriptionId)
			{
				hashCode = hashCode ^ this.SubscriptionId.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasSubscriptionId)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt64(this.SubscriptionId);
			}
			return num;
		}

		public static SubscribeResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<SubscribeResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			SubscribeResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, SubscribeResponse instance)
		{
			if (instance.HasSubscriptionId)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, instance.SubscriptionId);
			}
		}

		public void SetSubscriptionId(ulong val)
		{
			this.SubscriptionId = val;
		}
	}
}