using bnet.protocol.attribute;
using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.game_utilities
{
	public class GetAllValuesForAttributeResponse : IProtoBuf
	{
		private List<bnet.protocol.attribute.Variant> _AttributeValue = new List<bnet.protocol.attribute.Variant>();

		public List<bnet.protocol.attribute.Variant> AttributeValue
		{
			get
			{
				return this._AttributeValue;
			}
			set
			{
				this._AttributeValue = value;
			}
		}

		public GetAllValuesForAttributeResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetAllValuesForAttributeResponse.Deserialize(stream, this);
		}

		public static GetAllValuesForAttributeResponse Deserialize(Stream stream, GetAllValuesForAttributeResponse instance)
		{
			return GetAllValuesForAttributeResponse.Deserialize(stream, instance, (long)-1);
		}

		public static GetAllValuesForAttributeResponse Deserialize(Stream stream, GetAllValuesForAttributeResponse instance, long limit)
		{
			if (instance.AttributeValue == null)
			{
				instance.AttributeValue = new List<bnet.protocol.attribute.Variant>();
			}
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
					else if (num == 10)
					{
						instance.AttributeValue.Add(bnet.protocol.attribute.Variant.DeserializeLengthDelimited(stream));
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

		public static GetAllValuesForAttributeResponse DeserializeLengthDelimited(Stream stream)
		{
			GetAllValuesForAttributeResponse getAllValuesForAttributeResponse = new GetAllValuesForAttributeResponse();
			GetAllValuesForAttributeResponse.DeserializeLengthDelimited(stream, getAllValuesForAttributeResponse);
			return getAllValuesForAttributeResponse;
		}

		public static GetAllValuesForAttributeResponse DeserializeLengthDelimited(Stream stream, GetAllValuesForAttributeResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GetAllValuesForAttributeResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetAllValuesForAttributeResponse getAllValuesForAttributeResponse = obj as GetAllValuesForAttributeResponse;
			if (getAllValuesForAttributeResponse == null)
			{
				return false;
			}
			if (this.AttributeValue.Count != getAllValuesForAttributeResponse.AttributeValue.Count)
			{
				return false;
			}
			for (int i = 0; i < this.AttributeValue.Count; i++)
			{
				if (!this.AttributeValue[i].Equals(getAllValuesForAttributeResponse.AttributeValue[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (bnet.protocol.attribute.Variant attributeValue in this.AttributeValue)
			{
				hashCode = hashCode ^ attributeValue.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.AttributeValue.Count > 0)
			{
				foreach (bnet.protocol.attribute.Variant attributeValue in this.AttributeValue)
				{
					num++;
					uint serializedSize = attributeValue.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			return num;
		}

		public void Serialize(Stream stream)
		{
			GetAllValuesForAttributeResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetAllValuesForAttributeResponse instance)
		{
			if (instance.AttributeValue.Count > 0)
			{
				foreach (bnet.protocol.attribute.Variant attributeValue in instance.AttributeValue)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, attributeValue.GetSerializedSize());
					bnet.protocol.attribute.Variant.Serialize(stream, attributeValue);
				}
			}
		}
	}
}