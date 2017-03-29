using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.notification
{
	public class RegisterClientRequest : IProtoBuf
	{
		public bnet.protocol.EntityId EntityId
		{
			get;
			set;
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public RegisterClientRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			RegisterClientRequest.Deserialize(stream, this);
		}

		public static RegisterClientRequest Deserialize(Stream stream, RegisterClientRequest instance)
		{
			return RegisterClientRequest.Deserialize(stream, instance, (long)-1);
		}

		public static RegisterClientRequest Deserialize(Stream stream, RegisterClientRequest instance, long limit)
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
					else if (num != 10)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.EntityId != null)
					{
						bnet.protocol.EntityId.DeserializeLengthDelimited(stream, instance.EntityId);
					}
					else
					{
						instance.EntityId = bnet.protocol.EntityId.DeserializeLengthDelimited(stream);
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

		public static RegisterClientRequest DeserializeLengthDelimited(Stream stream)
		{
			RegisterClientRequest registerClientRequest = new RegisterClientRequest();
			RegisterClientRequest.DeserializeLengthDelimited(stream, registerClientRequest);
			return registerClientRequest;
		}

		public static RegisterClientRequest DeserializeLengthDelimited(Stream stream, RegisterClientRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return RegisterClientRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			RegisterClientRequest registerClientRequest = obj as RegisterClientRequest;
			if (registerClientRequest == null)
			{
				return false;
			}
			if (!this.EntityId.Equals(registerClientRequest.EntityId))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			return hashCode ^ this.EntityId.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.EntityId.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			return num + 1;
		}

		public static RegisterClientRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<RegisterClientRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			RegisterClientRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, RegisterClientRequest instance)
		{
			if (instance.EntityId == null)
			{
				throw new ArgumentNullException("EntityId", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.EntityId.GetSerializedSize());
			bnet.protocol.EntityId.Serialize(stream, instance.EntityId);
		}

		public void SetEntityId(bnet.protocol.EntityId val)
		{
			this.EntityId = val;
		}
	}
}