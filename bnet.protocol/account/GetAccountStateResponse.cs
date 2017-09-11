using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GetAccountStateResponse : IProtoBuf
	{
		public bool HasState;

		private AccountState _State;

		public bool HasTags;

		private AccountFieldTags _Tags;

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public AccountState State
		{
			get
			{
				return this._State;
			}
			set
			{
				this._State = value;
				this.HasState = value != null;
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

		public GetAccountStateResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetAccountStateResponse.Deserialize(stream, this);
		}

		public static GetAccountStateResponse Deserialize(Stream stream, GetAccountStateResponse instance)
		{
			return GetAccountStateResponse.Deserialize(stream, instance, (long)-1);
		}

		public static GetAccountStateResponse Deserialize(Stream stream, GetAccountStateResponse instance, long limit)
		{
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 == 10)
						{
							if (instance.State != null)
							{
								AccountState.DeserializeLengthDelimited(stream, instance.State);
							}
							else
							{
								instance.State = AccountState.DeserializeLengthDelimited(stream);
							}
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

		public static GetAccountStateResponse DeserializeLengthDelimited(Stream stream)
		{
			GetAccountStateResponse getAccountStateResponse = new GetAccountStateResponse();
			GetAccountStateResponse.DeserializeLengthDelimited(stream, getAccountStateResponse);
			return getAccountStateResponse;
		}

		public static GetAccountStateResponse DeserializeLengthDelimited(Stream stream, GetAccountStateResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GetAccountStateResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetAccountStateResponse getAccountStateResponse = obj as GetAccountStateResponse;
			if (getAccountStateResponse == null)
			{
				return false;
			}
			if (this.HasState != getAccountStateResponse.HasState || this.HasState && !this.State.Equals(getAccountStateResponse.State))
			{
				return false;
			}
			if (this.HasTags == getAccountStateResponse.HasTags && (!this.HasTags || this.Tags.Equals(getAccountStateResponse.Tags)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasState)
			{
				hashCode ^= this.State.GetHashCode();
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
			if (this.HasState)
			{
				num++;
				uint serializedSize = this.State.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasTags)
			{
				num++;
				uint serializedSize1 = this.Tags.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			return num;
		}

		public static GetAccountStateResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetAccountStateResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetAccountStateResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetAccountStateResponse instance)
		{
			if (instance.HasState)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.State.GetSerializedSize());
				AccountState.Serialize(stream, instance.State);
			}
			if (instance.HasTags)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.Tags.GetSerializedSize());
				AccountFieldTags.Serialize(stream, instance.Tags);
			}
		}

		public void SetState(AccountState val)
		{
			this.State = val;
		}

		public void SetTags(AccountFieldTags val)
		{
			this.Tags = val;
		}
	}
}