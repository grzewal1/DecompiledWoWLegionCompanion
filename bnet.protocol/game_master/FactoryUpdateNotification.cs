using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.game_master
{
	public class FactoryUpdateNotification : IProtoBuf
	{
		public bool HasProgramId;

		private uint _ProgramId;

		public GameFactoryDescription Description
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

		public FactoryUpdateNotification.Types.Operation Op
		{
			get;
			set;
		}

		public uint ProgramId
		{
			get
			{
				return this._ProgramId;
			}
			set
			{
				this._ProgramId = value;
				this.HasProgramId = true;
			}
		}

		public FactoryUpdateNotification()
		{
		}

		public void Deserialize(Stream stream)
		{
			FactoryUpdateNotification.Deserialize(stream, this);
		}

		public static FactoryUpdateNotification Deserialize(Stream stream, FactoryUpdateNotification instance)
		{
			return FactoryUpdateNotification.Deserialize(stream, instance, (long)-1);
		}

		public static FactoryUpdateNotification Deserialize(Stream stream, FactoryUpdateNotification instance, long limit)
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
						if (num1 == 8)
						{
							instance.Op = (FactoryUpdateNotification.Types.Operation)((int)ProtocolParser.ReadUInt64(stream));
						}
						else if (num1 != 18)
						{
							if (num1 == 29)
							{
								instance.ProgramId = binaryReader.ReadUInt32();
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
						else if (instance.Description != null)
						{
							GameFactoryDescription.DeserializeLengthDelimited(stream, instance.Description);
						}
						else
						{
							instance.Description = GameFactoryDescription.DeserializeLengthDelimited(stream);
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

		public static FactoryUpdateNotification DeserializeLengthDelimited(Stream stream)
		{
			FactoryUpdateNotification factoryUpdateNotification = new FactoryUpdateNotification();
			FactoryUpdateNotification.DeserializeLengthDelimited(stream, factoryUpdateNotification);
			return factoryUpdateNotification;
		}

		public static FactoryUpdateNotification DeserializeLengthDelimited(Stream stream, FactoryUpdateNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return FactoryUpdateNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			FactoryUpdateNotification factoryUpdateNotification = obj as FactoryUpdateNotification;
			if (factoryUpdateNotification == null)
			{
				return false;
			}
			if (!this.Op.Equals(factoryUpdateNotification.Op))
			{
				return false;
			}
			if (!this.Description.Equals(factoryUpdateNotification.Description))
			{
				return false;
			}
			if (this.HasProgramId == factoryUpdateNotification.HasProgramId && (!this.HasProgramId || this.ProgramId.Equals(factoryUpdateNotification.ProgramId)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.Op.GetHashCode();
			hashCode = hashCode ^ this.Description.GetHashCode();
			if (this.HasProgramId)
			{
				hashCode = hashCode ^ this.ProgramId.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num = num + ProtocolParser.SizeOfUInt64((ulong)this.Op);
			uint serializedSize = this.Description.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasProgramId)
			{
				num++;
				num = num + 4;
			}
			num = num + 2;
			return num;
		}

		public static FactoryUpdateNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<FactoryUpdateNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			FactoryUpdateNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, FactoryUpdateNotification instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.Op);
			if (instance.Description == null)
			{
				throw new ArgumentNullException("Description", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.Description.GetSerializedSize());
			GameFactoryDescription.Serialize(stream, instance.Description);
			if (instance.HasProgramId)
			{
				stream.WriteByte(29);
				binaryWriter.Write(instance.ProgramId);
			}
		}

		public void SetDescription(GameFactoryDescription val)
		{
			this.Description = val;
		}

		public void SetOp(FactoryUpdateNotification.Types.Operation val)
		{
			this.Op = val;
		}

		public void SetProgramId(uint val)
		{
			this.ProgramId = val;
		}

		public static class Types
		{
			public enum Operation
			{
				ADD = 1,
				REMOVE = 2,
				CHANGE = 3
			}
		}
	}
}