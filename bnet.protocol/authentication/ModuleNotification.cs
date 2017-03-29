using System;
using System.IO;

namespace bnet.protocol.authentication
{
	public class ModuleNotification : IProtoBuf
	{
		public bool HasModuleId;

		private int _ModuleId;

		public bool HasResult;

		private uint _Result;

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public int ModuleId
		{
			get
			{
				return this._ModuleId;
			}
			set
			{
				this._ModuleId = value;
				this.HasModuleId = true;
			}
		}

		public uint Result
		{
			get
			{
				return this._Result;
			}
			set
			{
				this._Result = value;
				this.HasResult = true;
			}
		}

		public ModuleNotification()
		{
		}

		public void Deserialize(Stream stream)
		{
			ModuleNotification.Deserialize(stream, this);
		}

		public static ModuleNotification Deserialize(Stream stream, ModuleNotification instance)
		{
			return ModuleNotification.Deserialize(stream, instance, (long)-1);
		}

		public static ModuleNotification Deserialize(Stream stream, ModuleNotification instance, long limit)
		{
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 == 16)
						{
							instance.ModuleId = (int)ProtocolParser.ReadUInt64(stream);
						}
						else if (num1 == 24)
						{
							instance.Result = ProtocolParser.ReadUInt32(stream);
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

		public static ModuleNotification DeserializeLengthDelimited(Stream stream)
		{
			ModuleNotification moduleNotification = new ModuleNotification();
			ModuleNotification.DeserializeLengthDelimited(stream, moduleNotification);
			return moduleNotification;
		}

		public static ModuleNotification DeserializeLengthDelimited(Stream stream, ModuleNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return ModuleNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ModuleNotification moduleNotification = obj as ModuleNotification;
			if (moduleNotification == null)
			{
				return false;
			}
			if (this.HasModuleId != moduleNotification.HasModuleId || this.HasModuleId && !this.ModuleId.Equals(moduleNotification.ModuleId))
			{
				return false;
			}
			if (this.HasResult == moduleNotification.HasResult && (!this.HasResult || this.Result.Equals(moduleNotification.Result)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasModuleId)
			{
				hashCode = hashCode ^ this.ModuleId.GetHashCode();
			}
			if (this.HasResult)
			{
				hashCode = hashCode ^ this.Result.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasModuleId)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt64((ulong)this.ModuleId);
			}
			if (this.HasResult)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.Result);
			}
			return num;
		}

		public static ModuleNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ModuleNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ModuleNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ModuleNotification instance)
		{
			if (instance.HasModuleId)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.ModuleId);
			}
			if (instance.HasResult)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.Result);
			}
		}

		public void SetModuleId(int val)
		{
			this.ModuleId = val;
		}

		public void SetResult(uint val)
		{
			this.Result = val;
		}
	}
}