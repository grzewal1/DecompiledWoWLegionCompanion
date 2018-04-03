using bnet.protocol;
using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GetAccountStateRequest : IProtoBuf
	{
		public bool HasEntityId;

		private bnet.protocol.EntityId _EntityId;

		public bool HasProgram;

		private uint _Program;

		public bool HasRegion;

		private uint _Region;

		public bool HasOptions;

		private AccountFieldOptions _Options;

		public bool HasTags;

		private AccountFieldTags _Tags;

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

		public AccountFieldOptions Options
		{
			get
			{
				return this._Options;
			}
			set
			{
				this._Options = value;
				this.HasOptions = value != null;
			}
		}

		public uint Program
		{
			get
			{
				return this._Program;
			}
			set
			{
				this._Program = value;
				this.HasProgram = true;
			}
		}

		public uint Region
		{
			get
			{
				return this._Region;
			}
			set
			{
				this._Region = value;
				this.HasRegion = true;
			}
		}

		public AccountFieldTags Tags
		{
			get
			{
				return this._Tags;
			}
			set
			{
				this._Tags = value;
				this.HasTags = value != null;
			}
		}

		public GetAccountStateRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetAccountStateRequest.Deserialize(stream, this);
		}

		public static GetAccountStateRequest Deserialize(Stream stream, GetAccountStateRequest instance)
		{
			return GetAccountStateRequest.Deserialize(stream, instance, (long)-1);
		}

		public static GetAccountStateRequest Deserialize(Stream stream, GetAccountStateRequest instance, long limit)
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
					else if (num == 10)
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
					else if (num == 16)
					{
						instance.Program = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 24)
					{
						instance.Region = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 82)
					{
						if (instance.Options != null)
						{
							AccountFieldOptions.DeserializeLengthDelimited(stream, instance.Options);
						}
						else
						{
							instance.Options = AccountFieldOptions.DeserializeLengthDelimited(stream);
						}
					}
					else if (num != 90)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.Tags != null)
					{
						AccountFieldTags.DeserializeLengthDelimited(stream, instance.Tags);
					}
					else
					{
						instance.Tags = AccountFieldTags.DeserializeLengthDelimited(stream);
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

		public static GetAccountStateRequest DeserializeLengthDelimited(Stream stream)
		{
			GetAccountStateRequest getAccountStateRequest = new GetAccountStateRequest();
			GetAccountStateRequest.DeserializeLengthDelimited(stream, getAccountStateRequest);
			return getAccountStateRequest;
		}

		public static GetAccountStateRequest DeserializeLengthDelimited(Stream stream, GetAccountStateRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GetAccountStateRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetAccountStateRequest getAccountStateRequest = obj as GetAccountStateRequest;
			if (getAccountStateRequest == null)
			{
				return false;
			}
			if (this.HasEntityId != getAccountStateRequest.HasEntityId || this.HasEntityId && !this.EntityId.Equals(getAccountStateRequest.EntityId))
			{
				return false;
			}
			if (this.HasProgram != getAccountStateRequest.HasProgram || this.HasProgram && !this.Program.Equals(getAccountStateRequest.Program))
			{
				return false;
			}
			if (this.HasRegion != getAccountStateRequest.HasRegion || this.HasRegion && !this.Region.Equals(getAccountStateRequest.Region))
			{
				return false;
			}
			if (this.HasOptions != getAccountStateRequest.HasOptions || this.HasOptions && !this.Options.Equals(getAccountStateRequest.Options))
			{
				return false;
			}
			if (this.HasTags == getAccountStateRequest.HasTags && (!this.HasTags || this.Tags.Equals(getAccountStateRequest.Tags)))
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
				hashCode ^= this.EntityId.GetHashCode();
			}
			if (this.HasProgram)
			{
				hashCode ^= this.Program.GetHashCode();
			}
			if (this.HasRegion)
			{
				hashCode ^= this.Region.GetHashCode();
			}
			if (this.HasOptions)
			{
				hashCode ^= this.Options.GetHashCode();
			}
			if (this.HasTags)
			{
				hashCode ^= this.Tags.GetHashCode();
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
			if (this.HasProgram)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.Program);
			}
			if (this.HasRegion)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.Region);
			}
			if (this.HasOptions)
			{
				num++;
				uint serializedSize1 = this.Options.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			if (this.HasTags)
			{
				num++;
				uint num1 = this.Tags.GetSerializedSize();
				num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
			}
			return num;
		}

		public static GetAccountStateRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetAccountStateRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetAccountStateRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetAccountStateRequest instance)
		{
			if (instance.HasEntityId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.EntityId.GetSerializedSize());
				bnet.protocol.EntityId.Serialize(stream, instance.EntityId);
			}
			if (instance.HasProgram)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.Program);
			}
			if (instance.HasRegion)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.Region);
			}
			if (instance.HasOptions)
			{
				stream.WriteByte(82);
				ProtocolParser.WriteUInt32(stream, instance.Options.GetSerializedSize());
				AccountFieldOptions.Serialize(stream, instance.Options);
			}
			if (instance.HasTags)
			{
				stream.WriteByte(90);
				ProtocolParser.WriteUInt32(stream, instance.Tags.GetSerializedSize());
				AccountFieldTags.Serialize(stream, instance.Tags);
			}
		}

		public void SetEntityId(bnet.protocol.EntityId val)
		{
			this.EntityId = val;
		}

		public void SetOptions(AccountFieldOptions val)
		{
			this.Options = val;
		}

		public void SetProgram(uint val)
		{
			this.Program = val;
		}

		public void SetRegion(uint val)
		{
			this.Region = val;
		}

		public void SetTags(AccountFieldTags val)
		{
			this.Tags = val;
		}
	}
}