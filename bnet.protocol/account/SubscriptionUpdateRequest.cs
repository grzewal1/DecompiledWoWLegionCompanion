using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.account
{
	public class SubscriptionUpdateRequest : IProtoBuf
	{
		private List<SubscriberReference> _Ref = new List<SubscriberReference>();

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public List<SubscriberReference> Ref
		{
			get
			{
				return this._Ref;
			}
			set
			{
				this._Ref = value;
			}
		}

		public int RefCount
		{
			get
			{
				return this._Ref.Count;
			}
		}

		public List<SubscriberReference> RefList
		{
			get
			{
				return this._Ref;
			}
		}

		public SubscriptionUpdateRequest()
		{
		}

		public void AddRef(SubscriberReference val)
		{
			this._Ref.Add(val);
		}

		public void ClearRef()
		{
			this._Ref.Clear();
		}

		public void Deserialize(Stream stream)
		{
			SubscriptionUpdateRequest.Deserialize(stream, this);
		}

		public static SubscriptionUpdateRequest Deserialize(Stream stream, SubscriptionUpdateRequest instance)
		{
			return SubscriptionUpdateRequest.Deserialize(stream, instance, (long)-1);
		}

		public static SubscriptionUpdateRequest Deserialize(Stream stream, SubscriptionUpdateRequest instance, long limit)
		{
			if (instance.Ref == null)
			{
				instance.Ref = new List<SubscriberReference>();
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
					else if (num == 18)
					{
						instance.Ref.Add(SubscriberReference.DeserializeLengthDelimited(stream));
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

		public static SubscriptionUpdateRequest DeserializeLengthDelimited(Stream stream)
		{
			SubscriptionUpdateRequest subscriptionUpdateRequest = new SubscriptionUpdateRequest();
			SubscriptionUpdateRequest.DeserializeLengthDelimited(stream, subscriptionUpdateRequest);
			return subscriptionUpdateRequest;
		}

		public static SubscriptionUpdateRequest DeserializeLengthDelimited(Stream stream, SubscriptionUpdateRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return SubscriptionUpdateRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			SubscriptionUpdateRequest subscriptionUpdateRequest = obj as SubscriptionUpdateRequest;
			if (subscriptionUpdateRequest == null)
			{
				return false;
			}
			if (this.Ref.Count != subscriptionUpdateRequest.Ref.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Ref.Count; i++)
			{
				if (!this.Ref[i].Equals(subscriptionUpdateRequest.Ref[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (SubscriberReference @ref in this.Ref)
			{
				hashCode ^= @ref.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Ref.Count > 0)
			{
				foreach (SubscriberReference @ref in this.Ref)
				{
					num++;
					uint serializedSize = @ref.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			return num;
		}

		public static SubscriptionUpdateRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<SubscriptionUpdateRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			SubscriptionUpdateRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, SubscriptionUpdateRequest instance)
		{
			if (instance.Ref.Count > 0)
			{
				foreach (SubscriberReference @ref in instance.Ref)
				{
					stream.WriteByte(18);
					ProtocolParser.WriteUInt32(stream, @ref.GetSerializedSize());
					SubscriberReference.Serialize(stream, @ref);
				}
			}
		}

		public void SetRef(List<SubscriberReference> val)
		{
			this.Ref = val;
		}
	}
}