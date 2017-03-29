using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel
{
	public class GetChannelInfoRequest : IProtoBuf
	{
		public bool HasAgentId;

		private EntityId _AgentId;

		public bool HasFetchState;

		private bool _FetchState;

		public bool HasFetchMembers;

		private bool _FetchMembers;

		public EntityId AgentId
		{
			get
			{
				return this._AgentId;
			}
			set
			{
				this._AgentId = value;
				this.HasAgentId = value != null;
			}
		}

		public EntityId ChannelId
		{
			get;
			set;
		}

		public bool FetchMembers
		{
			get
			{
				return this._FetchMembers;
			}
			set
			{
				this._FetchMembers = value;
				this.HasFetchMembers = true;
			}
		}

		public bool FetchState
		{
			get
			{
				return this._FetchState;
			}
			set
			{
				this._FetchState = value;
				this.HasFetchState = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public GetChannelInfoRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetChannelInfoRequest.Deserialize(stream, this);
		}

		public static GetChannelInfoRequest Deserialize(Stream stream, GetChannelInfoRequest instance)
		{
			return GetChannelInfoRequest.Deserialize(stream, instance, (long)-1);
		}

		public static GetChannelInfoRequest Deserialize(Stream stream, GetChannelInfoRequest instance, long limit)
		{
			instance.FetchState = false;
			instance.FetchMembers = false;
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
							if (instance.AgentId != null)
							{
								EntityId.DeserializeLengthDelimited(stream, instance.AgentId);
							}
							else
							{
								instance.AgentId = EntityId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 18)
						{
							if (instance.ChannelId != null)
							{
								EntityId.DeserializeLengthDelimited(stream, instance.ChannelId);
							}
							else
							{
								instance.ChannelId = EntityId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 24)
						{
							instance.FetchState = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 32)
						{
							instance.FetchMembers = ProtocolParser.ReadBool(stream);
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

		public static GetChannelInfoRequest DeserializeLengthDelimited(Stream stream)
		{
			GetChannelInfoRequest getChannelInfoRequest = new GetChannelInfoRequest();
			GetChannelInfoRequest.DeserializeLengthDelimited(stream, getChannelInfoRequest);
			return getChannelInfoRequest;
		}

		public static GetChannelInfoRequest DeserializeLengthDelimited(Stream stream, GetChannelInfoRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GetChannelInfoRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetChannelInfoRequest getChannelInfoRequest = obj as GetChannelInfoRequest;
			if (getChannelInfoRequest == null)
			{
				return false;
			}
			if (this.HasAgentId != getChannelInfoRequest.HasAgentId || this.HasAgentId && !this.AgentId.Equals(getChannelInfoRequest.AgentId))
			{
				return false;
			}
			if (!this.ChannelId.Equals(getChannelInfoRequest.ChannelId))
			{
				return false;
			}
			if (this.HasFetchState != getChannelInfoRequest.HasFetchState || this.HasFetchState && !this.FetchState.Equals(getChannelInfoRequest.FetchState))
			{
				return false;
			}
			if (this.HasFetchMembers == getChannelInfoRequest.HasFetchMembers && (!this.HasFetchMembers || this.FetchMembers.Equals(getChannelInfoRequest.FetchMembers)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasAgentId)
			{
				hashCode = hashCode ^ this.AgentId.GetHashCode();
			}
			hashCode = hashCode ^ this.ChannelId.GetHashCode();
			if (this.HasFetchState)
			{
				hashCode = hashCode ^ this.FetchState.GetHashCode();
			}
			if (this.HasFetchMembers)
			{
				hashCode = hashCode ^ this.FetchMembers.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasAgentId)
			{
				num++;
				uint serializedSize = this.AgentId.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			uint serializedSize1 = this.ChannelId.GetSerializedSize();
			num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			if (this.HasFetchState)
			{
				num++;
				num++;
			}
			if (this.HasFetchMembers)
			{
				num++;
				num++;
			}
			num++;
			return num;
		}

		public static GetChannelInfoRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetChannelInfoRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetChannelInfoRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetChannelInfoRequest instance)
		{
			if (instance.HasAgentId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentId.GetSerializedSize());
				EntityId.Serialize(stream, instance.AgentId);
			}
			if (instance.ChannelId == null)
			{
				throw new ArgumentNullException("ChannelId", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.ChannelId.GetSerializedSize());
			EntityId.Serialize(stream, instance.ChannelId);
			if (instance.HasFetchState)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteBool(stream, instance.FetchState);
			}
			if (instance.HasFetchMembers)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteBool(stream, instance.FetchMembers);
			}
		}

		public void SetAgentId(EntityId val)
		{
			this.AgentId = val;
		}

		public void SetChannelId(EntityId val)
		{
			this.ChannelId = val;
		}

		public void SetFetchMembers(bool val)
		{
			this.FetchMembers = val;
		}

		public void SetFetchState(bool val)
		{
			this.FetchState = val;
		}
	}
}