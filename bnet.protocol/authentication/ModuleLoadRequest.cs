using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.authentication
{
	public class ModuleLoadRequest : IProtoBuf
	{
		public bool HasMessage;

		private byte[] _Message;

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public byte[] Message
		{
			get
			{
				return this._Message;
			}
			set
			{
				this._Message = value;
				this.HasMessage = value != null;
			}
		}

		public ContentHandle ModuleHandle
		{
			get;
			set;
		}

		public ModuleLoadRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			ModuleLoadRequest.Deserialize(stream, this);
		}

		public static ModuleLoadRequest Deserialize(Stream stream, ModuleLoadRequest instance)
		{
			return ModuleLoadRequest.Deserialize(stream, instance, (long)-1);
		}

		public static ModuleLoadRequest Deserialize(Stream stream, ModuleLoadRequest instance, long limit)
		{
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
								instance.Message = ProtocolParser.ReadBytes(stream);
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
						else if (instance.ModuleHandle != null)
						{
							ContentHandle.DeserializeLengthDelimited(stream, instance.ModuleHandle);
						}
						else
						{
							instance.ModuleHandle = ContentHandle.DeserializeLengthDelimited(stream);
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

		public static ModuleLoadRequest DeserializeLengthDelimited(Stream stream)
		{
			ModuleLoadRequest moduleLoadRequest = new ModuleLoadRequest();
			ModuleLoadRequest.DeserializeLengthDelimited(stream, moduleLoadRequest);
			return moduleLoadRequest;
		}

		public static ModuleLoadRequest DeserializeLengthDelimited(Stream stream, ModuleLoadRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ModuleLoadRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ModuleLoadRequest moduleLoadRequest = obj as ModuleLoadRequest;
			if (moduleLoadRequest == null)
			{
				return false;
			}
			if (!this.ModuleHandle.Equals(moduleLoadRequest.ModuleHandle))
			{
				return false;
			}
			if (this.HasMessage == moduleLoadRequest.HasMessage && (!this.HasMessage || this.Message.Equals(moduleLoadRequest.Message)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.ModuleHandle.GetHashCode();
			if (this.HasMessage)
			{
				hashCode ^= this.Message.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.ModuleHandle.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasMessage)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32((int)this.Message.Length) + (int)this.Message.Length;
			}
			num++;
			return num;
		}

		public static ModuleLoadRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ModuleLoadRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ModuleLoadRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ModuleLoadRequest instance)
		{
			if (instance.ModuleHandle == null)
			{
				throw new ArgumentNullException("ModuleHandle", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.ModuleHandle.GetSerializedSize());
			ContentHandle.Serialize(stream, instance.ModuleHandle);
			if (instance.HasMessage)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteBytes(stream, instance.Message);
			}
		}

		public void SetMessage(byte[] val)
		{
			this.Message = val;
		}

		public void SetModuleHandle(ContentHandle val)
		{
			this.ModuleHandle = val;
		}
	}
}