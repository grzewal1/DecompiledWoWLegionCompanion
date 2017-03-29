using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.account
{
	public class SubscriptionUpdateResponse : IProtoBuf
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

		public SubscriptionUpdateResponse()
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
			SubscriptionUpdateResponse.Deserialize(stream, this);
		}

		public static SubscriptionUpdateResponse Deserialize(Stream stream, SubscriptionUpdateResponse instance)
		{
			return SubscriptionUpdateResponse.Deserialize(stream, instance, (long)-1);
		}

		public static SubscriptionUpdateResponse Deserialize(Stream stream, SubscriptionUpdateResponse instance, long limit)
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
					else if (num == 10)
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

		public static SubscriptionUpdateResponse DeserializeLengthDelimited(Stream stream)
		{
			SubscriptionUpdateResponse subscriptionUpdateResponse = new SubscriptionUpdateResponse();
			SubscriptionUpdateResponse.DeserializeLengthDelimited(stream, subscriptionUpdateResponse);
			return subscriptionUpdateResponse;
		}

		public static SubscriptionUpdateResponse DeserializeLengthDelimited(Stream stream, SubscriptionUpdateResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return SubscriptionUpdateResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			SubscriptionUpdateResponse subscriptionUpdateResponse = obj as SubscriptionUpdateResponse;
			if (subscriptionUpdateResponse == null)
			{
				return false;
			}
			if (this.Ref.Count != subscriptionUpdateResponse.Ref.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Ref.Count; i++)
			{
				if (!this.Ref[i].Equals(subscriptionUpdateResponse.Ref[i]))
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
				hashCode = hashCode ^ @ref.GetHashCode();
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

		public static SubscriptionUpdateResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<SubscriptionUpdateResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			SubscriptionUpdateResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, SubscriptionUpdateResponse instance)
		{
			if (instance.Ref.Count > 0)
			{
				foreach (SubscriberReference @ref in instance.Ref)
				{
					stream.WriteByte(10);
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