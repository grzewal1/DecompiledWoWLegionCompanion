using bnet.protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.presence
{
	public class QueryRequest : IProtoBuf
	{
		private List<FieldKey> _Key = new List<FieldKey>();

		public bnet.protocol.EntityId EntityId
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

		public List<FieldKey> Key
		{
			get
			{
				return this._Key;
			}
			set
			{
				this._Key = value;
			}
		}

		public int KeyCount
		{
			get
			{
				return this._Key.Count;
			}
		}

		public List<FieldKey> KeyList
		{
			get
			{
				return this._Key;
			}
		}

		public QueryRequest()
		{
		}

		public void AddKey(FieldKey val)
		{
			this._Key.Add(val);
		}

		public void ClearKey()
		{
			this._Key.Clear();
		}

		public void Deserialize(Stream stream)
		{
			QueryRequest.Deserialize(stream, this);
		}

		public static QueryRequest Deserialize(Stream stream, QueryRequest instance)
		{
			return QueryRequest.Deserialize(stream, instance, (long)-1);
		}

		public static QueryRequest Deserialize(Stream stream, QueryRequest instance, long limit)
		{
			if (instance.Key == null)
			{
				instance.Key = new List<FieldKey>();
			}
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
								instance.Key.Add(FieldKey.DeserializeLengthDelimited(stream));
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
						else if (instance.EntityId != null)
						{
							bnet.protocol.EntityId.DeserializeLengthDelimited(stream, instance.EntityId);
						}
						else
						{
							instance.EntityId = bnet.protocol.EntityId.DeserializeLengthDelimited(stream);
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

		public static QueryRequest DeserializeLengthDelimited(Stream stream)
		{
			QueryRequest queryRequest = new QueryRequest();
			QueryRequest.DeserializeLengthDelimited(stream, queryRequest);
			return queryRequest;
		}

		public static QueryRequest DeserializeLengthDelimited(Stream stream, QueryRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return QueryRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			QueryRequest queryRequest = obj as QueryRequest;
			if (queryRequest == null)
			{
				return false;
			}
			if (!this.EntityId.Equals(queryRequest.EntityId))
			{
				return false;
			}
			if (this.Key.Count != queryRequest.Key.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Key.Count; i++)
			{
				if (!this.Key[i].Equals(queryRequest.Key[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.EntityId.GetHashCode();
			foreach (FieldKey key in this.Key)
			{
				hashCode ^= key.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.EntityId.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.Key.Count > 0)
			{
				foreach (FieldKey key in this.Key)
				{
					num++;
					uint serializedSize1 = key.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			num++;
			return num;
		}

		public static QueryRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<QueryRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			QueryRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, QueryRequest instance)
		{
			if (instance.EntityId == null)
			{
				throw new ArgumentNullException("EntityId", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.EntityId.GetSerializedSize());
			bnet.protocol.EntityId.Serialize(stream, instance.EntityId);
			if (instance.Key.Count > 0)
			{
				foreach (FieldKey key in instance.Key)
				{
					stream.WriteByte(18);
					ProtocolParser.WriteUInt32(stream, key.GetSerializedSize());
					FieldKey.Serialize(stream, key);
				}
			}
		}

		public void SetEntityId(bnet.protocol.EntityId val)
		{
			this.EntityId = val;
		}

		public void SetKey(List<FieldKey> val)
		{
			this.Key = val;
		}
	}
}