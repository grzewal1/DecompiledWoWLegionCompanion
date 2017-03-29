using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.channel_invitation
{
	public class ChannelCountDescription : IProtoBuf
	{
		public bool HasChannelType;

		private string _ChannelType;

		public bool HasChannelId;

		private EntityId _ChannelId;

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

		public uint Program
		{
			get;
			set;
		}

		public uint ServiceType
		{
			get;
			set;
		}

		public ChannelCountDescription()
		{
		}

		public void Deserialize(Stream stream)
		{
			ChannelCountDescription.Deserialize(stream, this);
		}

		public static ChannelCountDescription Deserialize(Stream stream, ChannelCountDescription instance)
		{
			return ChannelCountDescription.Deserialize(stream, instance, (long)-1);
		}

		public static ChannelCountDescription Deserialize(Stream stream, ChannelCountDescription instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			instance.ChannelType = "default";
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 == 8)
						{
							instance.ServiceType = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 21)
						{
							instance.Program = binaryReader.ReadUInt32();
						}
						else if (num1 == 26)
						{
							instance.ChannelType = ProtocolParser.ReadString(stream);
						}
						else if (num1 != 34)
						{
							Key key = ProtocolParser.ReadKey((byte)num, stream);
							if (key.Field == 0)
							{
								throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
							}
							ProtocolParser.SkipKey(stream, key);
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

		public static ChannelCountDescription DeserializeLengthDelimited(Stream stream)
		{
			ChannelCountDescription channelCountDescription = new ChannelCountDescription();
			ChannelCountDescription.DeserializeLengthDelimited(stream, channelCountDescription);
			return channelCountDescription;
		}

		public static ChannelCountDescription DeserializeLengthDelimited(Stream stream, ChannelCountDescription instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return ChannelCountDescription.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ChannelCountDescription channelCountDescription = obj as ChannelCountDescription;
			if (channelCountDescription == null)
			{
				return false;
			}
			if (!this.ServiceType.Equals(channelCountDescription.ServiceType))
			{
				return false;
			}
			if (!this.Program.Equals(channelCountDescription.Program))
			{
				return false;
			}
			if (this.HasChannelType != channelCountDescription.HasChannelType || this.HasChannelType && !this.ChannelType.Equals(channelCountDescription.ChannelType))
			{
				return false;
			}
			if (this.HasChannelId == channelCountDescription.HasChannelId && (!this.HasChannelId || this.ChannelId.Equals(channelCountDescription.ChannelId)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.ServiceType.GetHashCode();
			hashCode = hashCode ^ this.Program.GetHashCode();
			if (this.HasChannelType)
			{
				hashCode = hashCode ^ this.ChannelType.GetHashCode();
			}
			if (this.HasChannelId)
			{
				hashCode = hashCode ^ this.ChannelId.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num = num + ProtocolParser.SizeOfUInt32(this.ServiceType);
			num = num + 4;
			if (this.HasChannelType)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.ChannelType);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasChannelId)
			{
				num++;
				uint serializedSize = this.ChannelId.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			num = num + 2;
			return num;
		}

		public static ChannelCountDescription ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ChannelCountDescription>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ChannelCountDescription.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ChannelCountDescription instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.ServiceType);
			stream.WriteByte(21);
			binaryWriter.Write(instance.Program);
			if (instance.HasChannelType)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.ChannelType));
			}
			if (instance.HasChannelId)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteUInt32(stream, instance.ChannelId.GetSerializedSize());
				EntityId.Serialize(stream, instance.ChannelId);
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

		public void SetProgram(uint val)
		{
			this.Program = val;
		}

		public void SetServiceType(uint val)
		{
			this.ServiceType = val;
		}
	}
}