using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.channel_invitation
{
	public class HasRoomForInvitationRequest : IProtoBuf
	{
		public bool HasProgram;

		private uint _Program;

		public bool HasChannelType;

		private string _ChannelType;

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

		public HasRoomForInvitationRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			HasRoomForInvitationRequest.Deserialize(stream, this);
		}

		public static HasRoomForInvitationRequest Deserialize(Stream stream, HasRoomForInvitationRequest instance)
		{
			return HasRoomForInvitationRequest.Deserialize(stream, instance, (long)-1);
		}

		public static HasRoomForInvitationRequest Deserialize(Stream stream, HasRoomForInvitationRequest instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			instance.ChannelType = "default";
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
					else if (num == 8)
					{
						instance.ServiceType = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 21)
					{
						instance.Program = binaryReader.ReadUInt32();
					}
					else if (num == 26)
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

		public static HasRoomForInvitationRequest DeserializeLengthDelimited(Stream stream)
		{
			HasRoomForInvitationRequest hasRoomForInvitationRequest = new HasRoomForInvitationRequest();
			HasRoomForInvitationRequest.DeserializeLengthDelimited(stream, hasRoomForInvitationRequest);
			return hasRoomForInvitationRequest;
		}

		public static HasRoomForInvitationRequest DeserializeLengthDelimited(Stream stream, HasRoomForInvitationRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return HasRoomForInvitationRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			HasRoomForInvitationRequest hasRoomForInvitationRequest = obj as HasRoomForInvitationRequest;
			if (hasRoomForInvitationRequest == null)
			{
				return false;
			}
			if (!this.ServiceType.Equals(hasRoomForInvitationRequest.ServiceType))
			{
				return false;
			}
			if (this.HasProgram != hasRoomForInvitationRequest.HasProgram || this.HasProgram && !this.Program.Equals(hasRoomForInvitationRequest.Program))
			{
				return false;
			}
			if (this.HasChannelType == hasRoomForInvitationRequest.HasChannelType && (!this.HasChannelType || this.ChannelType.Equals(hasRoomForInvitationRequest.ChannelType)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.ServiceType.GetHashCode();
			if (this.HasProgram)
			{
				hashCode ^= this.Program.GetHashCode();
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
			num += ProtocolParser.SizeOfUInt32(this.ServiceType);
			if (this.HasProgram)
			{
				num++;
				num += 4;
			}
			if (this.HasChannelType)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.ChannelType);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			num++;
			return num;
		}

		public static HasRoomForInvitationRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<HasRoomForInvitationRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			HasRoomForInvitationRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, HasRoomForInvitationRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.ServiceType);
			if (instance.HasProgram)
			{
				stream.WriteByte(21);
				binaryWriter.Write(instance.Program);
			}
			if (instance.HasChannelType)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.ChannelType));
			}
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