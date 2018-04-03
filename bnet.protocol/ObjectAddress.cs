using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol
{
	public class ObjectAddress : IProtoBuf
	{
		public bool HasObjectId;

		private ulong _ObjectId;

		public ProcessId Host
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

		public ulong ObjectId
		{
			get
			{
				return this._ObjectId;
			}
			set
			{
				this._ObjectId = value;
				this.HasObjectId = true;
			}
		}

		public ObjectAddress()
		{
		}

		public void Deserialize(Stream stream)
		{
			ObjectAddress.Deserialize(stream, this);
		}

		public static ObjectAddress Deserialize(Stream stream, ObjectAddress instance)
		{
			return ObjectAddress.Deserialize(stream, instance, (long)-1);
		}

		public static ObjectAddress Deserialize(Stream stream, ObjectAddress instance, long limit)
		{
			instance.ObjectId = (ulong)0;
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
						if (num == 16)
						{
							instance.ObjectId = ProtocolParser.ReadUInt64(stream);
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
					else if (instance.Host != null)
					{
						ProcessId.DeserializeLengthDelimited(stream, instance.Host);
					}
					else
					{
						instance.Host = ProcessId.DeserializeLengthDelimited(stream);
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

		public static ObjectAddress DeserializeLengthDelimited(Stream stream)
		{
			ObjectAddress objectAddress = new ObjectAddress();
			ObjectAddress.DeserializeLengthDelimited(stream, objectAddress);
			return objectAddress;
		}

		public static ObjectAddress DeserializeLengthDelimited(Stream stream, ObjectAddress instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ObjectAddress.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ObjectAddress objectAddress = obj as ObjectAddress;
			if (objectAddress == null)
			{
				return false;
			}
			if (!this.Host.Equals(objectAddress.Host))
			{
				return false;
			}
			if (this.HasObjectId == objectAddress.HasObjectId && (!this.HasObjectId || this.ObjectId.Equals(objectAddress.ObjectId)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Host.GetHashCode();
			if (this.HasObjectId)
			{
				hashCode ^= this.ObjectId.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.Host.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasObjectId)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.ObjectId);
			}
			num++;
			return num;
		}

		public static ObjectAddress ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ObjectAddress>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ObjectAddress.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ObjectAddress instance)
		{
			if (instance.Host == null)
			{
				throw new ArgumentNullException("Host", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.Host.GetSerializedSize());
			ProcessId.Serialize(stream, instance.Host);
			if (instance.HasObjectId)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, instance.ObjectId);
			}
		}

		public void SetHost(ProcessId val)
		{
			this.Host = val;
		}

		public void SetObjectId(ulong val)
		{
			this.ObjectId = val;
		}
	}
}