using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel_invitation
{
	public class ListChannelCountRequest : IProtoBuf
	{
		public bool HasProgram;

		private uint _Program;

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public EntityId MemberId
		{
			get;
			set;
		}

		public uint Program
		{
			get
			{
				return this._Program;
			}
			set
			{
				this._Program = value;
				this.HasProgram = true;
			}
		}

		public uint ServiceType
		{
			get;
			set;
		}

		public ListChannelCountRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			ListChannelCountRequest.Deserialize(stream, this);
		}

		public static ListChannelCountRequest Deserialize(Stream stream, ListChannelCountRequest instance)
		{
			return ListChannelCountRequest.Deserialize(stream, instance, (long)-1);
		}

		public static ListChannelCountRequest Deserialize(Stream stream, ListChannelCountRequest instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
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
							if (instance.MemberId != null)
							{
								EntityId.DeserializeLengthDelimited(stream, instance.MemberId);
							}
							else
							{
								instance.MemberId = EntityId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 16)
						{
							instance.ServiceType = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 29)
						{
							instance.Program = binaryReader.ReadUInt32();
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

		public static ListChannelCountRequest DeserializeLengthDelimited(Stream stream)
		{
			ListChannelCountRequest listChannelCountRequest = new ListChannelCountRequest();
			ListChannelCountRequest.DeserializeLengthDelimited(stream, listChannelCountRequest);
			return listChannelCountRequest;
		}

		public static ListChannelCountRequest DeserializeLengthDelimited(Stream stream, ListChannelCountRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ListChannelCountRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ListChannelCountRequest listChannelCountRequest = obj as ListChannelCountRequest;
			if (listChannelCountRequest == null)
			{
				return false;
			}
			if (!this.MemberId.Equals(listChannelCountRequest.MemberId))
			{
				return false;
			}
			if (!this.ServiceType.Equals(listChannelCountRequest.ServiceType))
			{
				return false;
			}
			if (this.HasProgram == listChannelCountRequest.HasProgram && (!this.HasProgram || this.Program.Equals(listChannelCountRequest.Program)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.MemberId.GetHashCode();
			hashCode ^= this.ServiceType.GetHashCode();
			if (this.HasProgram)
			{
				hashCode ^= this.Program.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.MemberId.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			num += ProtocolParser.SizeOfUInt32(this.ServiceType);
			if (this.HasProgram)
			{
				num++;
				num += 4;
			}
			num += 2;
			return num;
		}

		public static ListChannelCountRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ListChannelCountRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ListChannelCountRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ListChannelCountRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.MemberId == null)
			{
				throw new ArgumentNullException("MemberId", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.MemberId.GetSerializedSize());
			EntityId.Serialize(stream, instance.MemberId);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt32(stream, instance.ServiceType);
			if (instance.HasProgram)
			{
				stream.WriteByte(29);
				binaryWriter.Write(instance.Program);
			}
		}

		public void SetMemberId(EntityId val)
		{
			this.MemberId = val;
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