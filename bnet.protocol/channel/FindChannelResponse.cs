using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.channel
{
	public class FindChannelResponse : IProtoBuf
	{
		private List<ChannelDescription> _Channel = new List<ChannelDescription>();

		public List<ChannelDescription> Channel
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

		public List<ChannelDescription> ChannelList
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

		public FindChannelResponse()
		{
		}

		public void AddChannel(ChannelDescription val)
		{
			this._Channel.Add(val);
		}

		public void ClearChannel()
		{
			this._Channel.Clear();
		}

		public void Deserialize(Stream stream)
		{
			FindChannelResponse.Deserialize(stream, this);
		}

		public static FindChannelResponse Deserialize(Stream stream, FindChannelResponse instance)
		{
			return FindChannelResponse.Deserialize(stream, instance, (long)-1);
		}

		public static FindChannelResponse Deserialize(Stream stream, FindChannelResponse instance, long limit)
		{
			if (instance.Channel == null)
			{
				instance.Channel = new List<ChannelDescription>();
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
						instance.Channel.Add(ChannelDescription.DeserializeLengthDelimited(stream));
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

		public static FindChannelResponse DeserializeLengthDelimited(Stream stream)
		{
			FindChannelResponse findChannelResponse = new FindChannelResponse();
			FindChannelResponse.DeserializeLengthDelimited(stream, findChannelResponse);
			return findChannelResponse;
		}

		public static FindChannelResponse DeserializeLengthDelimited(Stream stream, FindChannelResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return FindChannelResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			FindChannelResponse findChannelResponse = obj as FindChannelResponse;
			if (findChannelResponse == null)
			{
				return false;
			}
			if (this.Channel.Count != findChannelResponse.Channel.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Channel.Count; i++)
			{
				if (!this.Channel[i].Equals(findChannelResponse.Channel[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (ChannelDescription channel in this.Channel)
			{
				hashCode = hashCode ^ channel.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Channel.Count > 0)
			{
				foreach (ChannelDescription channel in this.Channel)
				{
					num++;
					uint serializedSize = channel.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			return num;
		}

		public static FindChannelResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<FindChannelResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			FindChannelResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, FindChannelResponse instance)
		{
			if (instance.Channel.Count > 0)
			{
				foreach (ChannelDescription channel in instance.Channel)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, channel.GetSerializedSize());
					ChannelDescription.Serialize(stream, channel);
				}
			}
		}

		public void SetChannel(List<ChannelDescription> val)
		{
			this.Channel = val;
		}
	}
}