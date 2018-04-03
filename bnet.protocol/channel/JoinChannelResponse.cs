using bnet.protocol;
using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.channel
{
	public class JoinChannelResponse : IProtoBuf
	{
		public bool HasObjectId;

		private ulong _ObjectId;

		public bool HasRequireFriendValidation;

		private bool _RequireFriendValidation;

		private List<EntityId> _PrivilegedAccount = new List<EntityId>();

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

		public List<EntityId> PrivilegedAccount
		{
			get
			{
				return this._PrivilegedAccount;
			}
			set
			{
				this._PrivilegedAccount = value;
			}
		}

		public int PrivilegedAccountCount
		{
			get
			{
				return this._PrivilegedAccount.Count;
			}
		}

		public List<EntityId> PrivilegedAccountList
		{
			get
			{
				return this._PrivilegedAccount;
			}
		}

		public bool RequireFriendValidation
		{
			get
			{
				return this._RequireFriendValidation;
			}
			set
			{
				this._RequireFriendValidation = value;
				this.HasRequireFriendValidation = true;
			}
		}

		public JoinChannelResponse()
		{
		}

		public void AddPrivilegedAccount(EntityId val)
		{
			this._PrivilegedAccount.Add(val);
		}

		public void ClearPrivilegedAccount()
		{
			this._PrivilegedAccount.Clear();
		}

		public void Deserialize(Stream stream)
		{
			JoinChannelResponse.Deserialize(stream, this);
		}

		public static JoinChannelResponse Deserialize(Stream stream, JoinChannelResponse instance)
		{
			return JoinChannelResponse.Deserialize(stream, instance, (long)-1);
		}

		public static JoinChannelResponse Deserialize(Stream stream, JoinChannelResponse instance, long limit)
		{
			instance.RequireFriendValidation = false;
			if (instance.PrivilegedAccount == null)
			{
				instance.PrivilegedAccount = new List<EntityId>();
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
					else if (num == 8)
					{
						instance.ObjectId = ProtocolParser.ReadUInt64(stream);
					}
					else if (num == 16)
					{
						instance.RequireFriendValidation = ProtocolParser.ReadBool(stream);
					}
					else if (num == 26)
					{
						instance.PrivilegedAccount.Add(EntityId.DeserializeLengthDelimited(stream));
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

		public static JoinChannelResponse DeserializeLengthDelimited(Stream stream)
		{
			JoinChannelResponse joinChannelResponse = new JoinChannelResponse();
			JoinChannelResponse.DeserializeLengthDelimited(stream, joinChannelResponse);
			return joinChannelResponse;
		}

		public static JoinChannelResponse DeserializeLengthDelimited(Stream stream, JoinChannelResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return JoinChannelResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			JoinChannelResponse joinChannelResponse = obj as JoinChannelResponse;
			if (joinChannelResponse == null)
			{
				return false;
			}
			if (this.HasObjectId != joinChannelResponse.HasObjectId || this.HasObjectId && !this.ObjectId.Equals(joinChannelResponse.ObjectId))
			{
				return false;
			}
			if (this.HasRequireFriendValidation != joinChannelResponse.HasRequireFriendValidation || this.HasRequireFriendValidation && !this.RequireFriendValidation.Equals(joinChannelResponse.RequireFriendValidation))
			{
				return false;
			}
			if (this.PrivilegedAccount.Count != joinChannelResponse.PrivilegedAccount.Count)
			{
				return false;
			}
			for (int i = 0; i < this.PrivilegedAccount.Count; i++)
			{
				if (!this.PrivilegedAccount[i].Equals(joinChannelResponse.PrivilegedAccount[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasObjectId)
			{
				hashCode ^= this.ObjectId.GetHashCode();
			}
			if (this.HasRequireFriendValidation)
			{
				hashCode ^= this.RequireFriendValidation.GetHashCode();
			}
			foreach (EntityId privilegedAccount in this.PrivilegedAccount)
			{
				hashCode ^= privilegedAccount.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasObjectId)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.ObjectId);
			}
			if (this.HasRequireFriendValidation)
			{
				num++;
				num++;
			}
			if (this.PrivilegedAccount.Count > 0)
			{
				foreach (EntityId privilegedAccount in this.PrivilegedAccount)
				{
					num++;
					uint serializedSize = privilegedAccount.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			return num;
		}

		public static JoinChannelResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<JoinChannelResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			JoinChannelResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, JoinChannelResponse instance)
		{
			if (instance.HasObjectId)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, instance.ObjectId);
			}
			if (instance.HasRequireFriendValidation)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteBool(stream, instance.RequireFriendValidation);
			}
			if (instance.PrivilegedAccount.Count > 0)
			{
				foreach (EntityId privilegedAccount in instance.PrivilegedAccount)
				{
					stream.WriteByte(26);
					ProtocolParser.WriteUInt32(stream, privilegedAccount.GetSerializedSize());
					EntityId.Serialize(stream, privilegedAccount);
				}
			}
		}

		public void SetObjectId(ulong val)
		{
			this.ObjectId = val;
		}

		public void SetPrivilegedAccount(List<EntityId> val)
		{
			this.PrivilegedAccount = val;
		}

		public void SetRequireFriendValidation(bool val)
		{
			this.RequireFriendValidation = val;
		}
	}
}