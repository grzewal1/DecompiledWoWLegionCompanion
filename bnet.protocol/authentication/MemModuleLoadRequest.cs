using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.authentication
{
	public class MemModuleLoadRequest : IProtoBuf
	{
		public ContentHandle Handle
		{
			get;
			set;
		}

		public byte[] Input
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

		public byte[] Key
		{
			get;
			set;
		}

		public MemModuleLoadRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			MemModuleLoadRequest.Deserialize(stream, this);
		}

		public static MemModuleLoadRequest Deserialize(Stream stream, MemModuleLoadRequest instance)
		{
			return MemModuleLoadRequest.Deserialize(stream, instance, (long)-1);
		}

		public static MemModuleLoadRequest Deserialize(Stream stream, MemModuleLoadRequest instance, long limit)
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
							if (instance.Handle != null)
							{
								ContentHandle.DeserializeLengthDelimited(stream, instance.Handle);
							}
							else
							{
								instance.Handle = ContentHandle.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 18)
						{
							instance.Key = ProtocolParser.ReadBytes(stream);
						}
						else if (num1 == 26)
						{
							instance.Input = ProtocolParser.ReadBytes(stream);
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

		public static MemModuleLoadRequest DeserializeLengthDelimited(Stream stream)
		{
			MemModuleLoadRequest memModuleLoadRequest = new MemModuleLoadRequest();
			MemModuleLoadRequest.DeserializeLengthDelimited(stream, memModuleLoadRequest);
			return memModuleLoadRequest;
		}

		public static MemModuleLoadRequest DeserializeLengthDelimited(Stream stream, MemModuleLoadRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return MemModuleLoadRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			MemModuleLoadRequest memModuleLoadRequest = obj as MemModuleLoadRequest;
			if (memModuleLoadRequest == null)
			{
				return false;
			}
			if (!this.Handle.Equals(memModuleLoadRequest.Handle))
			{
				return false;
			}
			if (!this.Key.Equals(memModuleLoadRequest.Key))
			{
				return false;
			}
			if (!this.Input.Equals(memModuleLoadRequest.Input))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.Handle.GetHashCode();
			hashCode = hashCode ^ this.Key.GetHashCode();
			return hashCode ^ this.Input.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.Handle.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			num = num + ProtocolParser.SizeOfUInt32((int)this.Key.Length) + (int)this.Key.Length;
			num = num + ProtocolParser.SizeOfUInt32((int)this.Input.Length) + (int)this.Input.Length;
			return num + 3;
		}

		public static MemModuleLoadRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<MemModuleLoadRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			MemModuleLoadRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, MemModuleLoadRequest instance)
		{
			if (instance.Handle == null)
			{
				throw new ArgumentNullException("Handle", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.Handle.GetSerializedSize());
			ContentHandle.Serialize(stream, instance.Handle);
			if (instance.Key == null)
			{
				throw new ArgumentNullException("Key", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteBytes(stream, instance.Key);
			if (instance.Input == null)
			{
				throw new ArgumentNullException("Input", "Required by proto specification.");
			}
			stream.WriteByte(26);
			ProtocolParser.WriteBytes(stream, instance.Input);
		}

		public void SetHandle(ContentHandle val)
		{
			this.Handle = val;
		}

		public void SetInput(byte[] val)
		{
			this.Input = val;
		}

		public void SetKey(byte[] val)
		{
			this.Key = val;
		}
	}
}