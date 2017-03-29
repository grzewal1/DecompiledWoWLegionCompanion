using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.notification
{
	public class FindClientRequest : IProtoBuf
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

		public FindClientRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			FindClientRequest.Deserialize(stream, this);
		}

		public static FindClientRequest Deserialize(Stream stream, FindClientRequest instance)
		{
			return FindClientRequest.Deserialize(stream, instance, (long)-1);
		}

		public static FindClientRequest Deserialize(Stream stream, FindClientRequest instance, long limit)
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

		public static FindClientRequest DeserializeLengthDelimited(Stream stream)
		{
			FindClientRequest findClientRequest = new FindClientRequest();
			FindClientRequest.DeserializeLengthDelimited(stream, findClientRequest);
			return findClientRequest;
		}

		public static FindClientRequest DeserializeLengthDelimited(Stream stream, FindClientRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return FindClientRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			FindClientRequest findClientRequest = obj as FindClientRequest;
			if (findClientRequest == null)
			{
				return false;
			}
			if (!this.EntityId.Equals(findClientRequest.EntityId))
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

		public static FindClientRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<FindClientRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			FindClientRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, FindClientRequest instance)
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