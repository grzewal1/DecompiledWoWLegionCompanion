using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.authentication
{
	public class ModuleMessageRequest : IProtoBuf
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

		public int ModuleId
		{
			get;
			set;
		}

		public ModuleMessageRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			ModuleMessageRequest.Deserialize(stream, this);
		}

		public static ModuleMessageRequest Deserialize(Stream stream, ModuleMessageRequest instance)
		{
			return ModuleMessageRequest.Deserialize(stream, instance, (long)-1);
		}

		public static ModuleMessageRequest Deserialize(Stream stream, ModuleMessageRequest instance, long limit)
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
					else if (num == 8)
					{
						instance.ModuleId = (int)ProtocolParser.ReadUInt64(stream);
					}
					else if (num == 18)
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

		public static ModuleMessageRequest DeserializeLengthDelimited(Stream stream)
		{
			ModuleMessageRequest moduleMessageRequest = new ModuleMessageRequest();
			ModuleMessageRequest.DeserializeLengthDelimited(stream, moduleMessageRequest);
			return moduleMessageRequest;
		}

		public static ModuleMessageRequest DeserializeLengthDelimited(Stream stream, ModuleMessageRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ModuleMessageRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ModuleMessageRequest moduleMessageRequest = obj as ModuleMessageRequest;
			if (moduleMessageRequest == null)
			{
				return false;
			}
			if (!this.ModuleId.Equals(moduleMessageRequest.ModuleId))
			{
				return false;
			}
			if (this.HasMessage == moduleMessageRequest.HasMessage && (!this.HasMessage || this.Message.Equals(moduleMessageRequest.Message)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.ModuleId.GetHashCode();
			if (this.HasMessage)
			{
				hashCode ^= this.Message.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num += ProtocolParser.SizeOfUInt64((ulong)this.ModuleId);
			if (this.HasMessage)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32((int)this.Message.Length) + (int)this.Message.Length;
			}
			num++;
			return num;
		}

		public static ModuleMessageRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ModuleMessageRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ModuleMessageRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ModuleMessageRequest instance)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.ModuleId);
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

		public void SetModuleId(int val)
		{
			this.ModuleId = val;
		}
	}
}