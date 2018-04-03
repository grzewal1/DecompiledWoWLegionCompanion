using bnet.protocol.attribute;
using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.game_master
{
	public class GetFactoryInfoResponse : IProtoBuf
	{
		private List<bnet.protocol.attribute.Attribute> _Attribute = new List<bnet.protocol.attribute.Attribute>();

		private List<GameStatsBucket> _StatsBucket = new List<GameStatsBucket>();

		public List<bnet.protocol.attribute.Attribute> Attribute
		{
			get
			{
				return this._Attribute;
			}
			set
			{
				this._Attribute = value;
			}
		}

		public int AttributeCount
		{
			get
			{
				return this._Attribute.Count;
			}
		}

		public List<bnet.protocol.attribute.Attribute> AttributeList
		{
			get
			{
				return this._Attribute;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public List<GameStatsBucket> StatsBucket
		{
			get
			{
				return this._StatsBucket;
			}
			set
			{
				this._StatsBucket = value;
			}
		}

		public int StatsBucketCount
		{
			get
			{
				return this._StatsBucket.Count;
			}
		}

		public List<GameStatsBucket> StatsBucketList
		{
			get
			{
				return this._StatsBucket;
			}
		}

		public GetFactoryInfoResponse()
		{
		}

		public void AddAttribute(bnet.protocol.attribute.Attribute val)
		{
			this._Attribute.Add(val);
		}

		public void AddStatsBucket(GameStatsBucket val)
		{
			this._StatsBucket.Add(val);
		}

		public void ClearAttribute()
		{
			this._Attribute.Clear();
		}

		public void ClearStatsBucket()
		{
			this._StatsBucket.Clear();
		}

		public void Deserialize(Stream stream)
		{
			GetFactoryInfoResponse.Deserialize(stream, this);
		}

		public static GetFactoryInfoResponse Deserialize(Stream stream, GetFactoryInfoResponse instance)
		{
			return GetFactoryInfoResponse.Deserialize(stream, instance, (long)-1);
		}

		public static GetFactoryInfoResponse Deserialize(Stream stream, GetFactoryInfoResponse instance, long limit)
		{
			if (instance.Attribute == null)
			{
				instance.Attribute = new List<bnet.protocol.attribute.Attribute>();
			}
			if (instance.StatsBucket == null)
			{
				instance.StatsBucket = new List<GameStatsBucket>();
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
						instance.Attribute.Add(bnet.protocol.attribute.Attribute.DeserializeLengthDelimited(stream));
					}
					else if (num == 18)
					{
						instance.StatsBucket.Add(GameStatsBucket.DeserializeLengthDelimited(stream));
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

		public static GetFactoryInfoResponse DeserializeLengthDelimited(Stream stream)
		{
			GetFactoryInfoResponse getFactoryInfoResponse = new GetFactoryInfoResponse();
			GetFactoryInfoResponse.DeserializeLengthDelimited(stream, getFactoryInfoResponse);
			return getFactoryInfoResponse;
		}

		public static GetFactoryInfoResponse DeserializeLengthDelimited(Stream stream, GetFactoryInfoResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GetFactoryInfoResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetFactoryInfoResponse getFactoryInfoResponse = obj as GetFactoryInfoResponse;
			if (getFactoryInfoResponse == null)
			{
				return false;
			}
			if (this.Attribute.Count != getFactoryInfoResponse.Attribute.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Attribute.Count; i++)
			{
				if (!this.Attribute[i].Equals(getFactoryInfoResponse.Attribute[i]))
				{
					return false;
				}
			}
			if (this.StatsBucket.Count != getFactoryInfoResponse.StatsBucket.Count)
			{
				return false;
			}
			for (int j = 0; j < this.StatsBucket.Count; j++)
			{
				if (!this.StatsBucket[j].Equals(getFactoryInfoResponse.StatsBucket[j]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
			{
				hashCode ^= attribute.GetHashCode();
			}
			foreach (GameStatsBucket statsBucket in this.StatsBucket)
			{
				hashCode ^= statsBucket.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
				{
					num++;
					uint serializedSize = attribute.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			if (this.StatsBucket.Count > 0)
			{
				foreach (GameStatsBucket statsBucket in this.StatsBucket)
				{
					num++;
					uint serializedSize1 = statsBucket.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			return num;
		}

		public static GetFactoryInfoResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetFactoryInfoResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetFactoryInfoResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetFactoryInfoResponse instance)
		{
			if (instance.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in instance.Attribute)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, attribute.GetSerializedSize());
					bnet.protocol.attribute.Attribute.Serialize(stream, attribute);
				}
			}
			if (instance.StatsBucket.Count > 0)
			{
				foreach (GameStatsBucket statsBucket in instance.StatsBucket)
				{
					stream.WriteByte(18);
					ProtocolParser.WriteUInt32(stream, statsBucket.GetSerializedSize());
					GameStatsBucket.Serialize(stream, statsBucket);
				}
			}
		}

		public void SetAttribute(List<bnet.protocol.attribute.Attribute> val)
		{
			this.Attribute = val;
		}

		public void SetStatsBucket(List<GameStatsBucket> val)
		{
			this.StatsBucket = val;
		}
	}
}