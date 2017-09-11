using bnet.protocol.invitation;
using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.channel_invitation
{
	public class InvitationCollection : IProtoBuf
	{
		public bool HasServiceType;

		private uint _ServiceType;

		public bool HasMaxReceivedInvitations;

		private uint _MaxReceivedInvitations;

		public bool HasObjectId;

		private ulong _ObjectId;

		private List<Invitation> _ReceivedInvitation = new List<Invitation>();

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public uint MaxReceivedInvitations
		{
			get
			{
				return this._MaxReceivedInvitations;
			}
			set
			{
				this._MaxReceivedInvitations = value;
				this.HasMaxReceivedInvitations = true;
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

		public uint ServiceType
		{
			get
			{
				return this._ServiceType;
			}
			set
			{
				this._ServiceType = value;
				this.HasServiceType = true;
			}
		}

		public InvitationCollection()
		{
		}

		public void AddReceivedInvitation(Invitation val)
		{
			this._ReceivedInvitation.Add(val);
		}

		public void ClearReceivedInvitation()
		{
			this._ReceivedInvitation.Clear();
		}

		public void Deserialize(Stream stream)
		{
			InvitationCollection.Deserialize(stream, this);
		}

		public static InvitationCollection Deserialize(Stream stream, InvitationCollection instance)
		{
			return InvitationCollection.Deserialize(stream, instance, (long)-1);
		}

		public static InvitationCollection Deserialize(Stream stream, InvitationCollection instance, long limit)
		{
			if (instance.ReceivedInvitation == null)
			{
				instance.ReceivedInvitation = new List<Invitation>();
			}
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
							instance.ServiceType = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 16)
						{
							instance.MaxReceivedInvitations = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 24)
						{
							instance.ObjectId = ProtocolParser.ReadUInt64(stream);
						}
						else if (num1 == 34)
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

		public static InvitationCollection DeserializeLengthDelimited(Stream stream)
		{
			InvitationCollection invitationCollection = new InvitationCollection();
			InvitationCollection.DeserializeLengthDelimited(stream, invitationCollection);
			return invitationCollection;
		}

		public static InvitationCollection DeserializeLengthDelimited(Stream stream, InvitationCollection instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return InvitationCollection.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			InvitationCollection invitationCollection = obj as InvitationCollection;
			if (invitationCollection == null)
			{
				return false;
			}
			if (this.HasServiceType != invitationCollection.HasServiceType || this.HasServiceType && !this.ServiceType.Equals(invitationCollection.ServiceType))
			{
				return false;
			}
			if (this.HasMaxReceivedInvitations != invitationCollection.HasMaxReceivedInvitations || this.HasMaxReceivedInvitations && !this.MaxReceivedInvitations.Equals(invitationCollection.MaxReceivedInvitations))
			{
				return false;
			}
			if (this.HasObjectId != invitationCollection.HasObjectId || this.HasObjectId && !this.ObjectId.Equals(invitationCollection.ObjectId))
			{
				return false;
			}
			if (this.ReceivedInvitation.Count != invitationCollection.ReceivedInvitation.Count)
			{
				return false;
			}
			for (int i = 0; i < this.ReceivedInvitation.Count; i++)
			{
				if (!this.ReceivedInvitation[i].Equals(invitationCollection.ReceivedInvitation[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasServiceType)
			{
				hashCode ^= this.ServiceType.GetHashCode();
			}
			if (this.HasMaxReceivedInvitations)
			{
				hashCode ^= this.MaxReceivedInvitations.GetHashCode();
			}
			if (this.HasObjectId)
			{
				hashCode ^= this.ObjectId.GetHashCode();
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
			if (this.HasServiceType)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.ServiceType);
			}
			if (this.HasMaxReceivedInvitations)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.MaxReceivedInvitations);
			}
			if (this.HasObjectId)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.ObjectId);
			}
			if (this.ReceivedInvitation.Count > 0)
			{
				foreach (Invitation receivedInvitation in this.ReceivedInvitation)
				{
					num++;
					uint serializedSize = receivedInvitation.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			return num;
		}

		public static InvitationCollection ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<InvitationCollection>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			InvitationCollection.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, InvitationCollection instance)
		{
			if (instance.HasServiceType)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.ServiceType);
			}
			if (instance.HasMaxReceivedInvitations)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.MaxReceivedInvitations);
			}
			if (instance.HasObjectId)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt64(stream, instance.ObjectId);
			}
			if (instance.ReceivedInvitation.Count > 0)
			{
				foreach (Invitation receivedInvitation in instance.ReceivedInvitation)
				{
					stream.WriteByte(34);
					ProtocolParser.WriteUInt32(stream, receivedInvitation.GetSerializedSize());
					Invitation.Serialize(stream, receivedInvitation);
				}
			}
		}

		public void SetMaxReceivedInvitations(uint val)
		{
			this.MaxReceivedInvitations = val;
		}

		public void SetObjectId(ulong val)
		{
			this.ObjectId = val;
		}

		public void SetReceivedInvitation(List<Invitation> val)
		{
			this.ReceivedInvitation = val;
		}

		public void SetServiceType(uint val)
		{
			this.ServiceType = val;
		}
	}
}