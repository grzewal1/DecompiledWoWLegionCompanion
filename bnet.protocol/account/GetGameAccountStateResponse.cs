using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GetGameAccountStateResponse : IProtoBuf
	{
		public bool HasState;

		private GameAccountState _State;

		public bool HasTags;

		private GameAccountFieldTags _Tags;

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public GameAccountState State
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

		public GameAccountFieldTags Tags
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

		public GetGameAccountStateResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetGameAccountStateResponse.Deserialize(stream, this);
		}

		public static GetGameAccountStateResponse Deserialize(Stream stream, GetGameAccountStateResponse instance)
		{
			return GetGameAccountStateResponse.Deserialize(stream, instance, (long)-1);
		}

		public static GetGameAccountStateResponse Deserialize(Stream stream, GetGameAccountStateResponse instance, long limit)
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
								GameAccountState.DeserializeLengthDelimited(stream, instance.State);
							}
							else
							{
								instance.State = GameAccountState.DeserializeLengthDelimited(stream);
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
							GameAccountFieldTags.DeserializeLengthDelimited(stream, instance.Tags);
						}
						else
						{
							instance.Tags = GameAccountFieldTags.DeserializeLengthDelimited(stream);
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

		public static GetGameAccountStateResponse DeserializeLengthDelimited(Stream stream)
		{
			GetGameAccountStateResponse getGameAccountStateResponse = new GetGameAccountStateResponse();
			GetGameAccountStateResponse.DeserializeLengthDelimited(stream, getGameAccountStateResponse);
			return getGameAccountStateResponse;
		}

		public static GetGameAccountStateResponse DeserializeLengthDelimited(Stream stream, GetGameAccountStateResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GetGameAccountStateResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetGameAccountStateResponse getGameAccountStateResponse = obj as GetGameAccountStateResponse;
			if (getGameAccountStateResponse == null)
			{
				return false;
			}
			if (this.HasState != getGameAccountStateResponse.HasState || this.HasState && !this.State.Equals(getGameAccountStateResponse.State))
			{
				return false;
			}
			if (this.HasTags == getGameAccountStateResponse.HasTags && (!this.HasTags || this.Tags.Equals(getGameAccountStateResponse.Tags)))
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
				hashCode = hashCode ^ this.State.GetHashCode();
			}
			if (this.HasTags)
			{
				hashCode = hashCode ^ this.Tags.GetHashCode();
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

		public static GetGameAccountStateResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetGameAccountStateResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetGameAccountStateResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetGameAccountStateResponse instance)
		{
			if (instance.HasState)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.State.GetSerializedSize());
				GameAccountState.Serialize(stream, instance.State);
			}
			if (instance.HasTags)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.Tags.GetSerializedSize());
				GameAccountFieldTags.Serialize(stream, instance.Tags);
			}
		}

		public void SetState(GameAccountState val)
		{
			this.State = val;
		}

		public void SetTags(GameAccountFieldTags val)
		{
			this.Tags = val;
		}
	}
}