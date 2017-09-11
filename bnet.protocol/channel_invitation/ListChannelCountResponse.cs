using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.channel_invitation
{
	public class ListChannelCountResponse : IProtoBuf
	{
		private List<bnet.protocol.channel_invitation.ChannelCount> _Channel = new List<bnet.protocol.channel_invitation.ChannelCount>();

		public List<bnet.protocol.channel_invitation.ChannelCount> Channel
		{
			get
			{
				return this._Channel;
			}
			set
			{
				this._Channel = value;
			}
		}

		public int ChannelCount
		{
			get
			{
				return this._Channel.Count;
			}
		}

		public List<bnet.protocol.channel_invitation.ChannelCount> ChannelList
		{
			get
			{
				return this._Channel;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public ListChannelCountResponse()
		{
		}

		public void AddChannel(bnet.protocol.channel_invitation.ChannelCount val)
		{
			this._Channel.Add(val);
		}

		public void ClearChannel()
		{
			this._Channel.Clear();
		}

		public void Deserialize(Stream stream)
		{
			ListChannelCountResponse.Deserialize(stream, this);
		}

		public static ListChannelCountResponse Deserialize(Stream stream, ListChannelCountResponse instance)
		{
			return ListChannelCountResponse.Deserialize(stream, instance, (long)-1);
		}

		public static ListChannelCountResponse Deserialize(Stream stream, ListChannelCountResponse instance, long limit)
		{
			if (instance.Channel == null)
			{
				instance.Channel = new List<bnet.protocol.channel_invitation.ChannelCount>();
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
						instance.Channel.Add(bnet.protocol.channel_invitation.ChannelCount.DeserializeLengthDelimited(stream));
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

		public static ListChannelCountResponse DeserializeLengthDelimited(Stream stream)
		{
			ListChannelCountResponse listChannelCountResponse = new ListChannelCountResponse();
			ListChannelCountResponse.DeserializeLengthDelimited(stream, listChannelCountResponse);
			return listChannelCountResponse;
		}

		public static ListChannelCountResponse DeserializeLengthDelimited(Stream stream, ListChannelCountResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ListChannelCountResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ListChannelCountResponse listChannelCountResponse = obj as ListChannelCountResponse;
			if (listChannelCountResponse == null)
			{
				return false;
			}
			if (this.Channel.Count != listChannelCountResponse.Channel.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Channel.Count; i++)
			{
				if (!this.Channel[i].Equals(listChannelCountResponse.Channel[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (bnet.protocol.channel_invitation.ChannelCount channel in this.Channel)
			{
				hashCode ^= channel.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Channel.Count > 0)
			{
				foreach (bnet.protocol.channel_invitation.ChannelCount channel in this.Channel)
				{
					num++;
					uint serializedSize = channel.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			return num;
		}

		public static ListChannelCountResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ListChannelCountResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ListChannelCountResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ListChannelCountResponse instance)
		{
			if (instance.Channel.Count > 0)
			{
				foreach (bnet.protocol.channel_invitation.ChannelCount channel in instance.Channel)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, channel.GetSerializedSize());
					bnet.protocol.channel_invitation.ChannelCount.Serialize(stream, channel);
				}
			}
		}

		public void SetChannel(List<bnet.protocol.channel_invitation.ChannelCount> val)
		{
			this.Channel = val;
		}
	}
}