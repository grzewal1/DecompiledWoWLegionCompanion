using bnet.protocol;
using System;
using System.IO;
using System.Text;

namespace bnet.protocol.channel_invitation
{
	public class ChannelCount : IProtoBuf
	{
		public bool HasChannelId;

		private EntityId _ChannelId;

		public bool HasChannelType;

		private string _ChannelType;

		public EntityId ChannelId
		{
			get
			{
				return this._ChannelId;
			}
			set
			{
				this._ChannelId = value;
				this.HasChannelId = value != null;
			}
		}

		public string ChannelType
		{
			get
			{
				return this._ChannelType;
			}
			set
			{
				this._ChannelType = value;
				this.HasChannelType = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public ChannelCount()
		{
		}

		public void Deserialize(Stream stream)
		{
			ChannelCount.Deserialize(stream, this);
		}

		public static ChannelCount Deserialize(Stream stream, ChannelCount instance)
		{
			return ChannelCount.Deserialize(stream, instance, (long)-1);
		}

		public static ChannelCount Deserialize(Stream stream, ChannelCount instance, long limit)
		{
			instance.ChannelType = "default";
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
								instance.ChannelType = ProtocolParser.ReadString(stream);
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
						else if (instance.ChannelId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.ChannelId);
						}
						else
						{
							instance.ChannelId = EntityId.DeserializeLengthDelimited(stream);
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

		public static ChannelCount DeserializeLengthDelimited(Stream stream)
		{
			ChannelCount channelCount = new ChannelCount();
			ChannelCount.DeserializeLengthDelimited(stream, channelCount);
			return channelCount;
		}

		public static ChannelCount DeserializeLengthDelimited(Stream stream, ChannelCount instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ChannelCount.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ChannelCount channelCount = obj as ChannelCount;
			if (channelCount == null)
			{
				return false;
			}
			if (this.HasChannelId != channelCount.HasChannelId || this.HasChannelId && !this.ChannelId.Equals(channelCount.ChannelId))
			{
				return false;
			}
			if (this.HasChannelType == channelCount.HasChannelType && (!this.HasChannelType || this.ChannelType.Equals(channelCount.ChannelType)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasChannelId)
			{
				hashCode ^= this.ChannelId.GetHashCode();
			}
			if (this.HasChannelType)
			{
				hashCode ^= this.ChannelType.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasChannelId)
			{
				num++;
				uint serializedSize = this.ChannelId.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasChannelType)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.ChannelType);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			return num;
		}

		public static ChannelCount ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ChannelCount>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ChannelCount.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ChannelCount instance)
		{
			if (instance.HasChannelId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.ChannelId.GetSerializedSize());
				EntityId.Serialize(stream, instance.ChannelId);
			}
			if (instance.HasChannelType)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.ChannelType));
			}
		}

		public void SetChannelId(EntityId val)
		{
			this.ChannelId = val;
		}

		public void SetChannelType(string val)
		{
			this.ChannelType = val;
		}
	}
}