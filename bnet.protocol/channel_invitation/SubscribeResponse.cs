using bnet.protocol.invitation;
using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.channel_invitation
{
	public class SubscribeResponse : IProtoBuf
	{
		private List<InvitationCollection> _Collection = new List<InvitationCollection>();

		private List<Invitation> _ReceivedInvitation = new List<Invitation>();

		public List<InvitationCollection> Collection
		{
			get
			{
				return this._Collection;
			}
			set
			{
				this._Collection = value;
			}
		}

		public int CollectionCount
		{
			get
			{
				return this._Collection.Count;
			}
		}

		public List<InvitationCollection> CollectionList
		{
			get
			{
				return this._Collection;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public List<Invitation> ReceivedInvitation
		{
			get
			{
				return this._ReceivedInvitation;
			}
			set
			{
				this._ReceivedInvitation = value;
			}
		}

		public int ReceivedInvitationCount
		{
			get
			{
				return this._ReceivedInvitation.Count;
			}
		}

		public List<Invitation> ReceivedInvitationList
		{
			get
			{
				return this._ReceivedInvitation;
			}
		}

		public SubscribeResponse()
		{
		}

		public void AddCollection(InvitationCollection val)
		{
			this._Collection.Add(val);
		}

		public void AddReceivedInvitation(Invitation val)
		{
			this._ReceivedInvitation.Add(val);
		}

		public void ClearCollection()
		{
			this._Collection.Clear();
		}

		public void ClearReceivedInvitation()
		{
			this._ReceivedInvitation.Clear();
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
			if (instance.Collection == null)
			{
				instance.Collection = new List<InvitationCollection>();
			}
			if (instance.ReceivedInvitation == null)
			{
				instance.ReceivedInvitation = new List<Invitation>();
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
						instance.Collection.Add(InvitationCollection.DeserializeLengthDelimited(stream));
					}
					else if (num == 18)
					{
						instance.ReceivedInvitation.Add(Invitation.DeserializeLengthDelimited(stream));
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
			position += stream.Position;
			return SubscribeResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			SubscribeResponse subscribeResponse = obj as SubscribeResponse;
			if (subscribeResponse == null)
			{
				return false;
			}
			if (this.Collection.Count != subscribeResponse.Collection.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Collection.Count; i++)
			{
				if (!this.Collection[i].Equals(subscribeResponse.Collection[i]))
				{
					return false;
				}
			}
			if (this.ReceivedInvitation.Count != subscribeResponse.ReceivedInvitation.Count)
			{
				return false;
			}
			for (int j = 0; j < this.ReceivedInvitation.Count; j++)
			{
				if (!this.ReceivedInvitation[j].Equals(subscribeResponse.ReceivedInvitation[j]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (InvitationCollection collection in this.Collection)
			{
				hashCode ^= collection.GetHashCode();
			}
			foreach (Invitation receivedInvitation in this.ReceivedInvitation)
			{
				hashCode ^= receivedInvitation.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Collection.Count > 0)
			{
				foreach (InvitationCollection collection in this.Collection)
				{
					num++;
					uint serializedSize = collection.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			if (this.ReceivedInvitation.Count > 0)
			{
				foreach (Invitation receivedInvitation in this.ReceivedInvitation)
				{
					num++;
					uint serializedSize1 = receivedInvitation.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
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
			if (instance.Collection.Count > 0)
			{
				foreach (InvitationCollection collection in instance.Collection)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, collection.GetSerializedSize());
					InvitationCollection.Serialize(stream, collection);
				}
			}
			if (instance.ReceivedInvitation.Count > 0)
			{
				foreach (Invitation receivedInvitation in instance.ReceivedInvitation)
				{
					stream.WriteByte(18);
					ProtocolParser.WriteUInt32(stream, receivedInvitation.GetSerializedSize());
					Invitation.Serialize(stream, receivedInvitation);
				}
			}
		}

		public void SetCollection(List<InvitationCollection> val)
		{
			this.Collection = val;
		}

		public void SetReceivedInvitation(List<Invitation> val)
		{
			this.ReceivedInvitation = val;
		}
	}
}