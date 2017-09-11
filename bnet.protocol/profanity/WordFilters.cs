using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.profanity
{
	public class WordFilters : IProtoBuf
	{
		private List<WordFilter> _Filters = new List<WordFilter>();

		public List<WordFilter> Filters
		{
			get
			{
				return this._Filters;
			}
			set
			{
				this._Filters = value;
			}
		}

		public int FiltersCount
		{
			get
			{
				return this._Filters.Count;
			}
		}

		public List<WordFilter> FiltersList
		{
			get
			{
				return this._Filters;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public WordFilters()
		{
		}

		public void AddFilters(WordFilter val)
		{
			this._Filters.Add(val);
		}

		public void ClearFilters()
		{
			this._Filters.Clear();
		}

		public void Deserialize(Stream stream)
		{
			WordFilters.Deserialize(stream, this);
		}

		public static WordFilters Deserialize(Stream stream, WordFilters instance)
		{
			return WordFilters.Deserialize(stream, instance, (long)-1);
		}

		public static WordFilters Deserialize(Stream stream, WordFilters instance, long limit)
		{
			if (instance.Filters == null)
			{
				instance.Filters = new List<WordFilter>();
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
						instance.Filters.Add(WordFilter.DeserializeLengthDelimited(stream));
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

		public static WordFilters DeserializeLengthDelimited(Stream stream)
		{
			WordFilters wordFilter = new WordFilters();
			WordFilters.DeserializeLengthDelimited(stream, wordFilter);
			return wordFilter;
		}

		public static WordFilters DeserializeLengthDelimited(Stream stream, WordFilters instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return WordFilters.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			WordFilters wordFilter = obj as WordFilters;
			if (wordFilter == null)
			{
				return false;
			}
			if (this.Filters.Count != wordFilter.Filters.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Filters.Count; i++)
			{
				if (!this.Filters[i].Equals(wordFilter.Filters[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (WordFilter filter in this.Filters)
			{
				hashCode ^= filter.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Filters.Count > 0)
			{
				foreach (WordFilter filter in this.Filters)
				{
					num++;
					uint serializedSize = filter.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			return num;
		}

		public static WordFilters ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<WordFilters>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			WordFilters.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, WordFilters instance)
		{
			if (instance.Filters.Count > 0)
			{
				foreach (WordFilter filter in instance.Filters)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, filter.GetSerializedSize());
					WordFilter.Serialize(stream, filter);
				}
			}
		}

		public void SetFilters(List<WordFilter> val)
		{
			this.Filters = val;
		}
	}
}