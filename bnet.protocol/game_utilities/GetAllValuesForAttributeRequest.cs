using bnet.protocol;
using System;
using System.IO;
using System.Text;

namespace bnet.protocol.game_utilities
{
	public class GetAllValuesForAttributeRequest : IProtoBuf
	{
		public bool HasAttributeKey;

		private string _AttributeKey;

		public bool HasAgentId;

		private EntityId _AgentId;

		public bool HasProgram;

		private uint _Program;

		public EntityId AgentId
		{
			get
			{
				return this._AgentId;
			}
			set
			{
				this._AgentId = value;
				this.HasAgentId = value != null;
			}
		}

		public string AttributeKey
		{
			get
			{
				return this._AttributeKey;
			}
			set
			{
				this._AttributeKey = value;
				this.HasAttributeKey = value != null;
			}
		}

		public uint Program
		{
			get
			{
				return this._Program;
			}
			set
			{
				this._Program = value;
				this.HasProgram = true;
			}
		}

		public GetAllValuesForAttributeRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetAllValuesForAttributeRequest.Deserialize(stream, this);
		}

		public static GetAllValuesForAttributeRequest Deserialize(Stream stream, GetAllValuesForAttributeRequest instance)
		{
			return GetAllValuesForAttributeRequest.Deserialize(stream, instance, (long)-1);
		}

		public static GetAllValuesForAttributeRequest Deserialize(Stream stream, GetAllValuesForAttributeRequest instance, long limit)
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
						if (num1 == 10)
						{
							instance.AttributeKey = ProtocolParser.ReadString(stream);
						}
						else if (num1 != 18)
						{
							if (num1 == 45)
							{
								instance.Program = binaryReader.ReadUInt32();
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
						else if (instance.AgentId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.AgentId);
						}
						else
						{
							instance.AgentId = EntityId.DeserializeLengthDelimited(stream);
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

		public static GetAllValuesForAttributeRequest DeserializeLengthDelimited(Stream stream)
		{
			GetAllValuesForAttributeRequest getAllValuesForAttributeRequest = new GetAllValuesForAttributeRequest();
			GetAllValuesForAttributeRequest.DeserializeLengthDelimited(stream, getAllValuesForAttributeRequest);
			return getAllValuesForAttributeRequest;
		}

		public static GetAllValuesForAttributeRequest DeserializeLengthDelimited(Stream stream, GetAllValuesForAttributeRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GetAllValuesForAttributeRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetAllValuesForAttributeRequest getAllValuesForAttributeRequest = obj as GetAllValuesForAttributeRequest;
			if (getAllValuesForAttributeRequest == null)
			{
				return false;
			}
			if (this.HasAttributeKey != getAllValuesForAttributeRequest.HasAttributeKey || this.HasAttributeKey && !this.AttributeKey.Equals(getAllValuesForAttributeRequest.AttributeKey))
			{
				return false;
			}
			if (this.HasAgentId != getAllValuesForAttributeRequest.HasAgentId || this.HasAgentId && !this.AgentId.Equals(getAllValuesForAttributeRequest.AgentId))
			{
				return false;
			}
			if (this.HasProgram == getAllValuesForAttributeRequest.HasProgram && (!this.HasProgram || this.Program.Equals(getAllValuesForAttributeRequest.Program)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasAttributeKey)
			{
				hashCode = hashCode ^ this.AttributeKey.GetHashCode();
			}
			if (this.HasAgentId)
			{
				hashCode = hashCode ^ this.AgentId.GetHashCode();
			}
			if (this.HasProgram)
			{
				hashCode = hashCode ^ this.Program.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasAttributeKey)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.AttributeKey);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasAgentId)
			{
				num++;
				uint serializedSize = this.AgentId.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasProgram)
			{
				num++;
				num = num + 4;
			}
			return num;
		}

		public void Serialize(Stream stream)
		{
			GetAllValuesForAttributeRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetAllValuesForAttributeRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasAttributeKey)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.AttributeKey));
			}
			if (instance.HasAgentId)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.AgentId.GetSerializedSize());
				EntityId.Serialize(stream, instance.AgentId);
			}
			if (instance.HasProgram)
			{
				stream.WriteByte(45);
				binaryWriter.Write(instance.Program);
			}
		}
	}
}