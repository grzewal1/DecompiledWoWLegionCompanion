using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol
{
	public class ErrorInfo : IProtoBuf
	{
		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public uint MethodId
		{
			get;
			set;
		}

		public bnet.protocol.ObjectAddress ObjectAddress
		{
			get;
			set;
		}

		public uint ServiceHash
		{
			get;
			set;
		}

		public uint Status
		{
			get;
			set;
		}

		public ErrorInfo()
		{
		}

		public void Deserialize(Stream stream)
		{
			ErrorInfo.Deserialize(stream, this);
		}

		public static ErrorInfo Deserialize(Stream stream, ErrorInfo instance)
		{
			return ErrorInfo.Deserialize(stream, instance, (long)-1);
		}

		public static ErrorInfo Deserialize(Stream stream, ErrorInfo instance, long limit)
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
							if (instance.ObjectAddress != null)
							{
								bnet.protocol.ObjectAddress.DeserializeLengthDelimited(stream, instance.ObjectAddress);
							}
							else
							{
								instance.ObjectAddress = bnet.protocol.ObjectAddress.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 16)
						{
							instance.Status = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 24)
						{
							instance.ServiceHash = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 32)
						{
							instance.MethodId = ProtocolParser.ReadUInt32(stream);
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

		public static ErrorInfo DeserializeLengthDelimited(Stream stream)
		{
			ErrorInfo errorInfo = new ErrorInfo();
			ErrorInfo.DeserializeLengthDelimited(stream, errorInfo);
			return errorInfo;
		}

		public static ErrorInfo DeserializeLengthDelimited(Stream stream, ErrorInfo instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ErrorInfo.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ErrorInfo errorInfo = obj as ErrorInfo;
			if (errorInfo == null)
			{
				return false;
			}
			if (!this.ObjectAddress.Equals(errorInfo.ObjectAddress))
			{
				return false;
			}
			if (!this.Status.Equals(errorInfo.Status))
			{
				return false;
			}
			if (!this.ServiceHash.Equals(errorInfo.ServiceHash))
			{
				return false;
			}
			if (!this.MethodId.Equals(errorInfo.MethodId))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.ObjectAddress.GetHashCode();
			hashCode ^= this.Status.GetHashCode();
			hashCode ^= this.ServiceHash.GetHashCode();
			return hashCode ^ this.MethodId.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.ObjectAddress.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			num += ProtocolParser.SizeOfUInt32(this.Status);
			num += ProtocolParser.SizeOfUInt32(this.ServiceHash);
			num += ProtocolParser.SizeOfUInt32(this.MethodId);
			return num + 4;
		}

		public static ErrorInfo ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ErrorInfo>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ErrorInfo.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ErrorInfo instance)
		{
			if (instance.ObjectAddress == null)
			{
				throw new ArgumentNullException("ObjectAddress", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.ObjectAddress.GetSerializedSize());
			bnet.protocol.ObjectAddress.Serialize(stream, instance.ObjectAddress);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt32(stream, instance.Status);
			stream.WriteByte(24);
			ProtocolParser.WriteUInt32(stream, instance.ServiceHash);
			stream.WriteByte(32);
			ProtocolParser.WriteUInt32(stream, instance.MethodId);
		}

		public void SetMethodId(uint val)
		{
			this.MethodId = val;
		}

		public void SetObjectAddress(bnet.protocol.ObjectAddress val)
		{
			this.ObjectAddress = val;
		}

		public void SetServiceHash(uint val)
		{
			this.ServiceHash = val;
		}

		public void SetStatus(uint val)
		{
			this.Status = val;
		}
	}
}