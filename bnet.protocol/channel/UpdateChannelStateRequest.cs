using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel
{
	public class UpdateChannelStateRequest : IProtoBuf
	{
		public bool HasAgentId;

		private EntityId _AgentId;

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

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public ChannelState StateChange
		{
			get;
			set;
		}

		public UpdateChannelStateRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			UpdateChannelStateRequest.Deserialize(stream, this);
		}

		public static UpdateChannelStateRequest Deserialize(Stream stream, UpdateChannelStateRequest instance)
		{
			return UpdateChannelStateRequest.Deserialize(stream, instance, (long)-1);
		}

		public static UpdateChannelStateRequest Deserialize(Stream stream, UpdateChannelStateRequest instance, long limit)
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
							if (instance.AgentId != null)
							{
								EntityId.DeserializeLengthDelimited(stream, instance.AgentId);
							}
							else
							{
								instance.AgentId = EntityId.DeserializeLengthDelimited(stream);
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
						else if (instance.StateChange != null)
						{
							ChannelState.DeserializeLengthDelimited(stream, instance.StateChange);
						}
						else
						{
							instance.StateChange = ChannelState.DeserializeLengthDelimited(stream);
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

		public static UpdateChannelStateRequest DeserializeLengthDelimited(Stream stream)
		{
			UpdateChannelStateRequest updateChannelStateRequest = new UpdateChannelStateRequest();
			UpdateChannelStateRequest.DeserializeLengthDelimited(stream, updateChannelStateRequest);
			return updateChannelStateRequest;
		}

		public static UpdateChannelStateRequest DeserializeLengthDelimited(Stream stream, UpdateChannelStateRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return UpdateChannelStateRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			UpdateChannelStateRequest updateChannelStateRequest = obj as UpdateChannelStateRequest;
			if (updateChannelStateRequest == null)
			{
				return false;
			}
			if (this.HasAgentId != updateChannelStateRequest.HasAgentId || this.HasAgentId && !this.AgentId.Equals(updateChannelStateRequest.AgentId))
			{
				return false;
			}
			if (!this.StateChange.Equals(updateChannelStateRequest.StateChange))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasAgentId)
			{
				hashCode ^= this.AgentId.GetHashCode();
			}
			hashCode ^= this.StateChange.GetHashCode();
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
			uint serializedSize1 = this.StateChange.GetSerializedSize();
			num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			num++;
			return num;
		}

		public static UpdateChannelStateRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<UpdateChannelStateRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			UpdateChannelStateRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, UpdateChannelStateRequest instance)
		{
			if (instance.HasAgentId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentId.GetSerializedSize());
				EntityId.Serialize(stream, instance.AgentId);
			}
			if (instance.StateChange == null)
			{
				throw new ArgumentNullException("StateChange", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.StateChange.GetSerializedSize());
			ChannelState.Serialize(stream, instance.StateChange);
		}

		public void SetAgentId(EntityId val)
		{
			this.AgentId = val;
		}

		public void SetStateChange(ChannelState val)
		{
			this.StateChange = val;
		}
	}
}