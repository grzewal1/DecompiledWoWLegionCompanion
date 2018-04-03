using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel
{
	public class ChannelDescription : IProtoBuf
	{
		public bool HasCurrentMembers;

		private uint _CurrentMembers;

		public bool HasState;

		private ChannelState _State;

		public EntityId ChannelId
		{
			get;
			set;
		}

		public uint CurrentMembers
		{
			get
			{
				return this._CurrentMembers;
			}
			set
			{
				this._CurrentMembers = value;
				this.HasCurrentMembers = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public ChannelState State
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

		public ChannelDescription()
		{
		}

		public void Deserialize(Stream stream)
		{
			ChannelDescription.Deserialize(stream, this);
		}

		public static ChannelDescription Deserialize(Stream stream, ChannelDescription instance)
		{
			return ChannelDescription.Deserialize(stream, instance, (long)-1);
		}

		public static ChannelDescription Deserialize(Stream stream, ChannelDescription instance, long limit)
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
						if (instance.ChannelId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.ChannelId);
						}
						else
						{
							instance.ChannelId = EntityId.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 16)
					{
						instance.CurrentMembers = ProtocolParser.ReadUInt32(stream);
					}
					else if (num != 26)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.State != null)
					{
						ChannelState.DeserializeLengthDelimited(stream, instance.State);
					}
					else
					{
						instance.State = ChannelState.DeserializeLengthDelimited(stream);
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

		public static ChannelDescription DeserializeLengthDelimited(Stream stream)
		{
			ChannelDescription channelDescription = new ChannelDescription();
			ChannelDescription.DeserializeLengthDelimited(stream, channelDescription);
			return channelDescription;
		}

		public static ChannelDescription DeserializeLengthDelimited(Stream stream, ChannelDescription instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ChannelDescription.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ChannelDescription channelDescription = obj as ChannelDescription;
			if (channelDescription == null)
			{
				return false;
			}
			if (!this.ChannelId.Equals(channelDescription.ChannelId))
			{
				return false;
			}
			if (this.HasCurrentMembers != channelDescription.HasCurrentMembers || this.HasCurrentMembers && !this.CurrentMembers.Equals(channelDescription.CurrentMembers))
			{
				return false;
			}
			if (this.HasState == channelDescription.HasState && (!this.HasState || this.State.Equals(channelDescription.State)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.ChannelId.GetHashCode();
			if (this.HasCurrentMembers)
			{
				hashCode ^= this.CurrentMembers.GetHashCode();
			}
			if (this.HasState)
			{
				hashCode ^= this.State.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.ChannelId.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasCurrentMembers)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.CurrentMembers);
			}
			if (this.HasState)
			{
				num++;
				uint serializedSize1 = this.State.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			num++;
			return num;
		}

		public static ChannelDescription ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ChannelDescription>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ChannelDescription.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ChannelDescription instance)
		{
			if (instance.ChannelId == null)
			{
				throw new ArgumentNullException("ChannelId", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.ChannelId.GetSerializedSize());
			EntityId.Serialize(stream, instance.ChannelId);
			if (instance.HasCurrentMembers)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.CurrentMembers);
			}
			if (instance.HasState)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteUInt32(stream, instance.State.GetSerializedSize());
				ChannelState.Serialize(stream, instance.State);
			}
		}

		public void SetChannelId(EntityId val)
		{
			this.ChannelId = val;
		}

		public void SetCurrentMembers(uint val)
		{
			this.CurrentMembers = val;
		}

		public void SetState(ChannelState val)
		{
			this.State = val;
		}
	}
}