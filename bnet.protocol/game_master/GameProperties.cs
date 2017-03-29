using bnet.protocol.attribute;
using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.game_master
{
	public class GameProperties : IProtoBuf
	{
		private List<bnet.protocol.attribute.Attribute> _CreationAttributes = new List<bnet.protocol.attribute.Attribute>();

		public bool HasFilter;

		private AttributeFilter _Filter;

		public bool HasCreate;

		private bool _Create;

		public bool HasOpen;

		private bool _Open;

		public bool HasProgramId;

		private uint _ProgramId;

		public bool Create
		{
			get
			{
				return this._Create;
			}
			set
			{
				this._Create = value;
				this.HasCreate = true;
			}
		}

		public List<bnet.protocol.attribute.Attribute> CreationAttributes
		{
			get
			{
				return this._CreationAttributes;
			}
			set
			{
				this._CreationAttributes = value;
			}
		}

		public int CreationAttributesCount
		{
			get
			{
				return this._CreationAttributes.Count;
			}
		}

		public List<bnet.protocol.attribute.Attribute> CreationAttributesList
		{
			get
			{
				return this._CreationAttributes;
			}
		}

		public AttributeFilter Filter
		{
			get
			{
				return this._Filter;
			}
			set
			{
				this._Filter = value;
				this.HasFilter = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public bool Open
		{
			get
			{
				return this._Open;
			}
			set
			{
				this._Open = value;
				this.HasOpen = true;
			}
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

		public GameProperties()
		{
		}

		public void AddCreationAttributes(bnet.protocol.attribute.Attribute val)
		{
			this._CreationAttributes.Add(val);
		}

		public void ClearCreationAttributes()
		{
			this._CreationAttributes.Clear();
		}

		public void Deserialize(Stream stream)
		{
			GameProperties.Deserialize(stream, this);
		}

		public static GameProperties Deserialize(Stream stream, GameProperties instance)
		{
			return GameProperties.Deserialize(stream, instance, (long)-1);
		}

		public static GameProperties Deserialize(Stream stream, GameProperties instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			if (instance.CreationAttributes == null)
			{
				instance.CreationAttributes = new List<bnet.protocol.attribute.Attribute>();
			}
			instance.Create = false;
			instance.Open = true;
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
							instance.CreationAttributes.Add(bnet.protocol.attribute.Attribute.DeserializeLengthDelimited(stream));
						}
						else if (num1 == 18)
						{
							if (instance.Filter != null)
							{
								AttributeFilter.DeserializeLengthDelimited(stream, instance.Filter);
							}
							else
							{
								instance.Filter = AttributeFilter.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 24)
						{
							instance.Create = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 32)
						{
							instance.Open = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 45)
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

		public static GameProperties DeserializeLengthDelimited(Stream stream)
		{
			GameProperties gameProperty = new GameProperties();
			GameProperties.DeserializeLengthDelimited(stream, gameProperty);
			return gameProperty;
		}

		public static GameProperties DeserializeLengthDelimited(Stream stream, GameProperties instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GameProperties.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameProperties gameProperty = obj as GameProperties;
			if (gameProperty == null)
			{
				return false;
			}
			if (this.CreationAttributes.Count != gameProperty.CreationAttributes.Count)
			{
				return false;
			}
			for (int i = 0; i < this.CreationAttributes.Count; i++)
			{
				if (!this.CreationAttributes[i].Equals(gameProperty.CreationAttributes[i]))
				{
					return false;
				}
			}
			if (this.HasFilter != gameProperty.HasFilter || this.HasFilter && !this.Filter.Equals(gameProperty.Filter))
			{
				return false;
			}
			if (this.HasCreate != gameProperty.HasCreate || this.HasCreate && !this.Create.Equals(gameProperty.Create))
			{
				return false;
			}
			if (this.HasOpen != gameProperty.HasOpen || this.HasOpen && !this.Open.Equals(gameProperty.Open))
			{
				return false;
			}
			if (this.HasProgramId == gameProperty.HasProgramId && (!this.HasProgramId || this.ProgramId.Equals(gameProperty.ProgramId)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (bnet.protocol.attribute.Attribute creationAttribute in this.CreationAttributes)
			{
				hashCode = hashCode ^ creationAttribute.GetHashCode();
			}
			if (this.HasFilter)
			{
				hashCode = hashCode ^ this.Filter.GetHashCode();
			}
			if (this.HasCreate)
			{
				hashCode = hashCode ^ this.Create.GetHashCode();
			}
			if (this.HasOpen)
			{
				hashCode = hashCode ^ this.Open.GetHashCode();
			}
			if (this.HasProgramId)
			{
				hashCode = hashCode ^ this.ProgramId.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.CreationAttributes.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute creationAttribute in this.CreationAttributes)
				{
					num++;
					uint serializedSize = creationAttribute.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			if (this.HasFilter)
			{
				num++;
				uint serializedSize1 = this.Filter.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			if (this.HasCreate)
			{
				num++;
				num++;
			}
			if (this.HasOpen)
			{
				num++;
				num++;
			}
			if (this.HasProgramId)
			{
				num++;
				num = num + 4;
			}
			return num;
		}

		public static GameProperties ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameProperties>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameProperties.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameProperties instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.CreationAttributes.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute creationAttribute in instance.CreationAttributes)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, creationAttribute.GetSerializedSize());
					bnet.protocol.attribute.Attribute.Serialize(stream, creationAttribute);
				}
			}
			if (instance.HasFilter)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.Filter.GetSerializedSize());
				AttributeFilter.Serialize(stream, instance.Filter);
			}
			if (instance.HasCreate)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteBool(stream, instance.Create);
			}
			if (instance.HasOpen)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteBool(stream, instance.Open);
			}
			if (instance.HasProgramId)
			{
				stream.WriteByte(45);
				binaryWriter.Write(instance.ProgramId);
			}
		}

		public void SetCreate(bool val)
		{
			this.Create = val;
		}

		public void SetCreationAttributes(List<bnet.protocol.attribute.Attribute> val)
		{
			this.CreationAttributes = val;
		}

		public void SetFilter(AttributeFilter val)
		{
			this.Filter = val;
		}

		public void SetOpen(bool val)
		{
			this.Open = val;
		}

		public void SetProgramId(uint val)
		{
			this.ProgramId = val;
		}
	}
}