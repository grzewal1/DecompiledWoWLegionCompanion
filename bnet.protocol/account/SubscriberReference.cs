using bnet.protocol;
using System;
using System.IO;

namespace bnet.protocol.account
{
	public class SubscriberReference : IProtoBuf
	{
		public bool HasObjectId;

		private ulong _ObjectId;

		public bool HasEntityId;

		private bnet.protocol.EntityId _EntityId;

		public bool HasAccountOptions;

		private AccountFieldOptions _AccountOptions;

		public bool HasAccountTags;

		private AccountFieldTags _AccountTags;

		public bool HasGameAccountOptions;

		private GameAccountFieldOptions _GameAccountOptions;

		public bool HasGameAccountTags;

		private GameAccountFieldTags _GameAccountTags;

		public AccountFieldOptions AccountOptions
		{
			get
			{
				return this._AccountOptions;
			}
			set
			{
				this._AccountOptions = value;
				this.HasAccountOptions = value != null;
			}
		}

		public AccountFieldTags AccountTags
		{
			get
			{
				return this._AccountTags;
			}
			set
			{
				this._AccountTags = value;
				this.HasAccountTags = value != null;
			}
		}

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

		public GameAccountFieldOptions GameAccountOptions
		{
			get
			{
				return this._GameAccountOptions;
			}
			set
			{
				this._GameAccountOptions = value;
				this.HasGameAccountOptions = value != null;
			}
		}

		public GameAccountFieldTags GameAccountTags
		{
			get
			{
				return this._GameAccountTags;
			}
			set
			{
				this._GameAccountTags = value;
				this.HasGameAccountTags = value != null;
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

		public SubscriberReference()
		{
		}

		public void Deserialize(Stream stream)
		{
			SubscriberReference.Deserialize(stream, this);
		}

		public static SubscriberReference Deserialize(Stream stream, SubscriberReference instance)
		{
			return SubscriberReference.Deserialize(stream, instance, (long)-1);
		}

		public static SubscriberReference Deserialize(Stream stream, SubscriberReference instance, long limit)
		{
			instance.ObjectId = (ulong)0;
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
					else if (num == 18)
					{
						if (instance.EntityId != null)
						{
							bnet.protocol.EntityId.DeserializeLengthDelimited(stream, instance.EntityId);
						}
						else
						{
							instance.EntityId = bnet.protocol.EntityId.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 26)
					{
						if (instance.AccountOptions != null)
						{
							AccountFieldOptions.DeserializeLengthDelimited(stream, instance.AccountOptions);
						}
						else
						{
							instance.AccountOptions = AccountFieldOptions.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 34)
					{
						if (instance.AccountTags != null)
						{
							AccountFieldTags.DeserializeLengthDelimited(stream, instance.AccountTags);
						}
						else
						{
							instance.AccountTags = AccountFieldTags.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 42)
					{
						if (instance.GameAccountOptions != null)
						{
							GameAccountFieldOptions.DeserializeLengthDelimited(stream, instance.GameAccountOptions);
						}
						else
						{
							instance.GameAccountOptions = GameAccountFieldOptions.DeserializeLengthDelimited(stream);
						}
					}
					else if (num != 50)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.GameAccountTags != null)
					{
						GameAccountFieldTags.DeserializeLengthDelimited(stream, instance.GameAccountTags);
					}
					else
					{
						instance.GameAccountTags = GameAccountFieldTags.DeserializeLengthDelimited(stream);
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

		public static SubscriberReference DeserializeLengthDelimited(Stream stream)
		{
			SubscriberReference subscriberReference = new SubscriberReference();
			SubscriberReference.DeserializeLengthDelimited(stream, subscriberReference);
			return subscriberReference;
		}

		public static SubscriberReference DeserializeLengthDelimited(Stream stream, SubscriberReference instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return SubscriberReference.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			SubscriberReference subscriberReference = obj as SubscriberReference;
			if (subscriberReference == null)
			{
				return false;
			}
			if (this.HasObjectId != subscriberReference.HasObjectId || this.HasObjectId && !this.ObjectId.Equals(subscriberReference.ObjectId))
			{
				return false;
			}
			if (this.HasEntityId != subscriberReference.HasEntityId || this.HasEntityId && !this.EntityId.Equals(subscriberReference.EntityId))
			{
				return false;
			}
			if (this.HasAccountOptions != subscriberReference.HasAccountOptions || this.HasAccountOptions && !this.AccountOptions.Equals(subscriberReference.AccountOptions))
			{
				return false;
			}
			if (this.HasAccountTags != subscriberReference.HasAccountTags || this.HasAccountTags && !this.AccountTags.Equals(subscriberReference.AccountTags))
			{
				return false;
			}
			if (this.HasGameAccountOptions != subscriberReference.HasGameAccountOptions || this.HasGameAccountOptions && !this.GameAccountOptions.Equals(subscriberReference.GameAccountOptions))
			{
				return false;
			}
			if (this.HasGameAccountTags == subscriberReference.HasGameAccountTags && (!this.HasGameAccountTags || this.GameAccountTags.Equals(subscriberReference.GameAccountTags)))
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
				hashCode ^= this.ObjectId.GetHashCode();
			}
			if (this.HasEntityId)
			{
				hashCode ^= this.EntityId.GetHashCode();
			}
			if (this.HasAccountOptions)
			{
				hashCode ^= this.AccountOptions.GetHashCode();
			}
			if (this.HasAccountTags)
			{
				hashCode ^= this.AccountTags.GetHashCode();
			}
			if (this.HasGameAccountOptions)
			{
				hashCode ^= this.GameAccountOptions.GetHashCode();
			}
			if (this.HasGameAccountTags)
			{
				hashCode ^= this.GameAccountTags.GetHashCode();
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
			if (this.HasEntityId)
			{
				num++;
				uint serializedSize = this.EntityId.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasAccountOptions)
			{
				num++;
				uint serializedSize1 = this.AccountOptions.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			if (this.HasAccountTags)
			{
				num++;
				uint num1 = this.AccountTags.GetSerializedSize();
				num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
			}
			if (this.HasGameAccountOptions)
			{
				num++;
				uint serializedSize2 = this.GameAccountOptions.GetSerializedSize();
				num = num + serializedSize2 + ProtocolParser.SizeOfUInt32(serializedSize2);
			}
			if (this.HasGameAccountTags)
			{
				num++;
				uint num2 = this.GameAccountTags.GetSerializedSize();
				num = num + num2 + ProtocolParser.SizeOfUInt32(num2);
			}
			return num;
		}

		public static SubscriberReference ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<SubscriberReference>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			SubscriberReference.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, SubscriberReference instance)
		{
			if (instance.HasObjectId)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, instance.ObjectId);
			}
			if (instance.HasEntityId)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.EntityId.GetSerializedSize());
				bnet.protocol.EntityId.Serialize(stream, instance.EntityId);
			}
			if (instance.HasAccountOptions)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteUInt32(stream, instance.AccountOptions.GetSerializedSize());
				AccountFieldOptions.Serialize(stream, instance.AccountOptions);
			}
			if (instance.HasAccountTags)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteUInt32(stream, instance.AccountTags.GetSerializedSize());
				AccountFieldTags.Serialize(stream, instance.AccountTags);
			}
			if (instance.HasGameAccountOptions)
			{
				stream.WriteByte(42);
				ProtocolParser.WriteUInt32(stream, instance.GameAccountOptions.GetSerializedSize());
				GameAccountFieldOptions.Serialize(stream, instance.GameAccountOptions);
			}
			if (instance.HasGameAccountTags)
			{
				stream.WriteByte(50);
				ProtocolParser.WriteUInt32(stream, instance.GameAccountTags.GetSerializedSize());
				GameAccountFieldTags.Serialize(stream, instance.GameAccountTags);
			}
		}

		public void SetAccountOptions(AccountFieldOptions val)
		{
			this.AccountOptions = val;
		}

		public void SetAccountTags(AccountFieldTags val)
		{
			this.AccountTags = val;
		}

		public void SetEntityId(bnet.protocol.EntityId val)
		{
			this.EntityId = val;
		}

		public void SetGameAccountOptions(GameAccountFieldOptions val)
		{
			this.GameAccountOptions = val;
		}

		public void SetGameAccountTags(GameAccountFieldTags val)
		{
			this.GameAccountTags = val;
		}

		public void SetObjectId(ulong val)
		{
			this.ObjectId = val;
		}
	}
}