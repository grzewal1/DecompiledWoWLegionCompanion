using bnet.protocol.attribute;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.game_master
{
	public class ListFactoriesRequest : IProtoBuf
	{
		public bool HasStartIndex;

		private uint _StartIndex;

		public bool HasMaxResults;

		private uint _MaxResults;

		public AttributeFilter Filter
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

		public uint MaxResults
		{
			get
			{
				return this._MaxResults;
			}
			set
			{
				this._MaxResults = value;
				this.HasMaxResults = true;
			}
		}

		public uint StartIndex
		{
			get
			{
				return this._StartIndex;
			}
			set
			{
				this._StartIndex = value;
				this.HasStartIndex = true;
			}
		}

		public ListFactoriesRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			ListFactoriesRequest.Deserialize(stream, this);
		}

		public static ListFactoriesRequest Deserialize(Stream stream, ListFactoriesRequest instance)
		{
			return ListFactoriesRequest.Deserialize(stream, instance, (long)-1);
		}

		public static ListFactoriesRequest Deserialize(Stream stream, ListFactoriesRequest instance, long limit)
		{
			instance.StartIndex = 0;
			instance.MaxResults = 100;
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
							if (instance.Filter != null)
							{
								AttributeFilter.DeserializeLengthDelimited(stream, instance.Filter);
							}
							else
							{
								instance.Filter = AttributeFilter.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 16)
						{
							instance.StartIndex = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 24)
						{
							instance.MaxResults = ProtocolParser.ReadUInt32(stream);
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

		public static ListFactoriesRequest DeserializeLengthDelimited(Stream stream)
		{
			ListFactoriesRequest listFactoriesRequest = new ListFactoriesRequest();
			ListFactoriesRequest.DeserializeLengthDelimited(stream, listFactoriesRequest);
			return listFactoriesRequest;
		}

		public static ListFactoriesRequest DeserializeLengthDelimited(Stream stream, ListFactoriesRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ListFactoriesRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ListFactoriesRequest listFactoriesRequest = obj as ListFactoriesRequest;
			if (listFactoriesRequest == null)
			{
				return false;
			}
			if (!this.Filter.Equals(listFactoriesRequest.Filter))
			{
				return false;
			}
			if (this.HasStartIndex != listFactoriesRequest.HasStartIndex || this.HasStartIndex && !this.StartIndex.Equals(listFactoriesRequest.StartIndex))
			{
				return false;
			}
			if (this.HasMaxResults == listFactoriesRequest.HasMaxResults && (!this.HasMaxResults || this.MaxResults.Equals(listFactoriesRequest.MaxResults)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Filter.GetHashCode();
			if (this.HasStartIndex)
			{
				hashCode ^= this.StartIndex.GetHashCode();
			}
			if (this.HasMaxResults)
			{
				hashCode ^= this.MaxResults.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.Filter.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasStartIndex)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.StartIndex);
			}
			if (this.HasMaxResults)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.MaxResults);
			}
			num++;
			return num;
		}

		public static ListFactoriesRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ListFactoriesRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ListFactoriesRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ListFactoriesRequest instance)
		{
			if (instance.Filter == null)
			{
				throw new ArgumentNullException("Filter", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.Filter.GetSerializedSize());
			AttributeFilter.Serialize(stream, instance.Filter);
			if (instance.HasStartIndex)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.StartIndex);
			}
			if (instance.HasMaxResults)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.MaxResults);
			}
		}

		public void SetFilter(AttributeFilter val)
		{
			this.Filter = val;
		}

		public void SetMaxResults(uint val)
		{
			this.MaxResults = val;
		}

		public void SetStartIndex(uint val)
		{
			this.StartIndex = val;
		}
	}
}