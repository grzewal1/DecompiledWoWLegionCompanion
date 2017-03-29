using bnet.protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel_invitation
{
	public class IncrementChannelCountRequest : IProtoBuf
	{
		private List<ChannelCountDescription> _Descriptions = new List<ChannelCountDescription>();

		public EntityId AgentId
		{
			get;
			set;
		}

		public List<ChannelCountDescription> Descriptions
		{
			get
			{
				return this._Descriptions;
			}
			set
			{
				this._Descriptions = value;
			}
		}

		public int DescriptionsCount
		{
			get
			{
				return this._Descriptions.Count;
			}
		}

		public List<ChannelCountDescription> DescriptionsList
		{
			get
			{
				return this._Descriptions;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public IncrementChannelCountRequest()
		{
		}

		public void AddDescriptions(ChannelCountDescription val)
		{
			this._Descriptions.Add(val);
		}

		public void ClearDescriptions()
		{
			this._Descriptions.Clear();
		}

		public void Deserialize(Stream stream)
		{
			IncrementChannelCountRequest.Deserialize(stream, this);
		}

		public static IncrementChannelCountRequest Deserialize(Stream stream, IncrementChannelCountRequest instance)
		{
			return IncrementChannelCountRequest.Deserialize(stream, instance, (long)-1);
		}

		public static IncrementChannelCountRequest Deserialize(Stream stream, IncrementChannelCountRequest instance, long limit)
		{
			if (instance.Descriptions == null)
			{
				instance.Descriptions = new List<ChannelCountDescription>();
			}
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 != 10)
						{
							if (num1 == 18)
							{
								instance.Descriptions.Add(ChannelCountDescription.DeserializeLengthDelimited(stream));
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
						else if (instance.AgentId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.AgentId);
						}
						else
						{
							instance.AgentId = EntityId.DeserializeLengthDelimited(stream);
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

		public static IncrementChannelCountRequest DeserializeLengthDelimited(Stream stream)
		{
			IncrementChannelCountRequest incrementChannelCountRequest = new IncrementChannelCountRequest();
			IncrementChannelCountRequest.DeserializeLengthDelimited(stream, incrementChannelCountRequest);
			return incrementChannelCountRequest;
		}

		public static IncrementChannelCountRequest DeserializeLengthDelimited(Stream stream, IncrementChannelCountRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return IncrementChannelCountRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			IncrementChannelCountRequest incrementChannelCountRequest = obj as IncrementChannelCountRequest;
			if (incrementChannelCountRequest == null)
			{
				return false;
			}
			if (!this.AgentId.Equals(incrementChannelCountRequest.AgentId))
			{
				return false;
			}
			if (this.Descriptions.Count != incrementChannelCountRequest.Descriptions.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Descriptions.Count; i++)
			{
				if (!this.Descriptions[i].Equals(incrementChannelCountRequest.Descriptions[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.AgentId.GetHashCode();
			foreach (ChannelCountDescription description in this.Descriptions)
			{
				hashCode = hashCode ^ description.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.AgentId.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.Descriptions.Count > 0)
			{
				foreach (ChannelCountDescription description in this.Descriptions)
				{
					num++;
					uint serializedSize1 = description.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			num++;
			return num;
		}

		public static IncrementChannelCountRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<IncrementChannelCountRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			IncrementChannelCountRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, IncrementChannelCountRequest instance)
		{
			if (instance.AgentId == null)
			{
				throw new ArgumentNullException("AgentId", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.AgentId.GetSerializedSize());
			EntityId.Serialize(stream, instance.AgentId);
			if (instance.Descriptions.Count > 0)
			{
				foreach (ChannelCountDescription description in instance.Descriptions)
				{
					stream.WriteByte(18);
					ProtocolParser.WriteUInt32(stream, description.GetSerializedSize());
					ChannelCountDescription.Serialize(stream, description);
				}
			}
		}

		public void SetAgentId(EntityId val)
		{
			this.AgentId = val;
		}

		public void SetDescriptions(List<ChannelCountDescription> val)
		{
			this.Descriptions = val;
		}
	}
}