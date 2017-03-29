using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.presence
{
	public class QueryResponse : IProtoBuf
	{
		private List<bnet.protocol.presence.Field> _Field = new List<bnet.protocol.presence.Field>();

		public List<bnet.protocol.presence.Field> Field
		{
			get
			{
				return this._Field;
			}
			set
			{
				this._Field = value;
			}
		}

		public int FieldCount
		{
			get
			{
				return this._Field.Count;
			}
		}

		public List<bnet.protocol.presence.Field> FieldList
		{
			get
			{
				return this._Field;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public QueryResponse()
		{
		}

		public void AddField(bnet.protocol.presence.Field val)
		{
			this._Field.Add(val);
		}

		public void ClearField()
		{
			this._Field.Clear();
		}

		public void Deserialize(Stream stream)
		{
			QueryResponse.Deserialize(stream, this);
		}

		public static QueryResponse Deserialize(Stream stream, QueryResponse instance)
		{
			return QueryResponse.Deserialize(stream, instance, (long)-1);
		}

		public static QueryResponse Deserialize(Stream stream, QueryResponse instance, long limit)
		{
			if (instance.Field == null)
			{
				instance.Field = new List<bnet.protocol.presence.Field>();
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
					else if (num == 18)
					{
						instance.Field.Add(bnet.protocol.presence.Field.DeserializeLengthDelimited(stream));
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

		public static QueryResponse DeserializeLengthDelimited(Stream stream)
		{
			QueryResponse queryResponse = new QueryResponse();
			QueryResponse.DeserializeLengthDelimited(stream, queryResponse);
			return queryResponse;
		}

		public static QueryResponse DeserializeLengthDelimited(Stream stream, QueryResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return QueryResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			QueryResponse queryResponse = obj as QueryResponse;
			if (queryResponse == null)
			{
				return false;
			}
			if (this.Field.Count != queryResponse.Field.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Field.Count; i++)
			{
				if (!this.Field[i].Equals(queryResponse.Field[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (bnet.protocol.presence.Field field in this.Field)
			{
				hashCode = hashCode ^ field.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Field.Count > 0)
			{
				foreach (bnet.protocol.presence.Field field in this.Field)
				{
					num++;
					uint serializedSize = field.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			return num;
		}

		public static QueryResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<QueryResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			QueryResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, QueryResponse instance)
		{
			if (instance.Field.Count > 0)
			{
				foreach (bnet.protocol.presence.Field field in instance.Field)
				{
					stream.WriteByte(18);
					ProtocolParser.WriteUInt32(stream, field.GetSerializedSize());
					bnet.protocol.presence.Field.Serialize(stream, field);
				}
			}
		}

		public void SetField(List<bnet.protocol.presence.Field> val)
		{
			this.Field = val;
		}
	}
}