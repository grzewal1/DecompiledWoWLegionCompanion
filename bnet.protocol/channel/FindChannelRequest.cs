using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel
{
	public class FindChannelRequest : IProtoBuf
	{
		public bool HasAgentIdentity;

		private Identity _AgentIdentity;

		public Identity AgentIdentity
		{
			get
			{
				return this._AgentIdentity;
			}
			set
			{
				this._AgentIdentity = value;
				this.HasAgentIdentity = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public FindChannelOptions Options
		{
			get;
			set;
		}

		public FindChannelRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			FindChannelRequest.Deserialize(stream, this);
		}

		public static FindChannelRequest Deserialize(Stream stream, FindChannelRequest instance)
		{
			return FindChannelRequest.Deserialize(stream, instance, (long)-1);
		}

		public static FindChannelRequest Deserialize(Stream stream, FindChannelRequest instance, long limit)
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
						if (instance.AgentIdentity != null)
						{
							Identity.DeserializeLengthDelimited(stream, instance.AgentIdentity);
						}
						else
						{
							instance.AgentIdentity = Identity.DeserializeLengthDelimited(stream);
						}
					}
					else if (num != 18)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.Options != null)
					{
						FindChannelOptions.DeserializeLengthDelimited(stream, instance.Options);
					}
					else
					{
						instance.Options = FindChannelOptions.DeserializeLengthDelimited(stream);
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

		public static FindChannelRequest DeserializeLengthDelimited(Stream stream)
		{
			FindChannelRequest findChannelRequest = new FindChannelRequest();
			FindChannelRequest.DeserializeLengthDelimited(stream, findChannelRequest);
			return findChannelRequest;
		}

		public static FindChannelRequest DeserializeLengthDelimited(Stream stream, FindChannelRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return FindChannelRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			FindChannelRequest findChannelRequest = obj as FindChannelRequest;
			if (findChannelRequest == null)
			{
				return false;
			}
			if (this.HasAgentIdentity != findChannelRequest.HasAgentIdentity || this.HasAgentIdentity && !this.AgentIdentity.Equals(findChannelRequest.AgentIdentity))
			{
				return false;
			}
			if (!this.Options.Equals(findChannelRequest.Options))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasAgentIdentity)
			{
				hashCode ^= this.AgentIdentity.GetHashCode();
			}
			hashCode ^= this.Options.GetHashCode();
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasAgentIdentity)
			{
				num++;
				uint serializedSize = this.AgentIdentity.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			uint serializedSize1 = this.Options.GetSerializedSize();
			num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			num++;
			return num;
		}

		public static FindChannelRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<FindChannelRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			FindChannelRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, FindChannelRequest instance)
		{
			if (instance.HasAgentIdentity)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentIdentity.GetSerializedSize());
				Identity.Serialize(stream, instance.AgentIdentity);
			}
			if (instance.Options == null)
			{
				throw new ArgumentNullException("Options", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.Options.GetSerializedSize());
			FindChannelOptions.Serialize(stream, instance.Options);
		}

		public void SetAgentIdentity(Identity val)
		{
			this.AgentIdentity = val;
		}

		public void SetOptions(FindChannelOptions val)
		{
			this.Options = val;
		}
	}
}