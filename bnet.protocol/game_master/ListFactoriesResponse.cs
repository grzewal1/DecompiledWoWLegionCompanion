using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.game_master
{
	public class ListFactoriesResponse : IProtoBuf
	{
		private List<GameFactoryDescription> _Description = new List<GameFactoryDescription>();

		public bool HasTotalResults;

		private uint _TotalResults;

		public List<GameFactoryDescription> Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				this._Description = value;
			}
		}

		public int DescriptionCount
		{
			get
			{
				return this._Description.Count;
			}
		}

		public List<GameFactoryDescription> DescriptionList
		{
			get
			{
				return this._Description;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public uint TotalResults
		{
			get
			{
				return this._TotalResults;
			}
			set
			{
				this._TotalResults = value;
				this.HasTotalResults = true;
			}
		}

		public ListFactoriesResponse()
		{
		}

		public void AddDescription(GameFactoryDescription val)
		{
			this._Description.Add(val);
		}

		public void ClearDescription()
		{
			this._Description.Clear();
		}

		public void Deserialize(Stream stream)
		{
			ListFactoriesResponse.Deserialize(stream, this);
		}

		public static ListFactoriesResponse Deserialize(Stream stream, ListFactoriesResponse instance)
		{
			return ListFactoriesResponse.Deserialize(stream, instance, (long)-1);
		}

		public static ListFactoriesResponse Deserialize(Stream stream, ListFactoriesResponse instance, long limit)
		{
			if (instance.Description == null)
			{
				instance.Description = new List<GameFactoryDescription>();
			}
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
							instance.Description.Add(GameFactoryDescription.DeserializeLengthDelimited(stream));
						}
						else if (num1 == 16)
						{
							instance.TotalResults = ProtocolParser.ReadUInt32(stream);
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

		public static ListFactoriesResponse DeserializeLengthDelimited(Stream stream)
		{
			ListFactoriesResponse listFactoriesResponse = new ListFactoriesResponse();
			ListFactoriesResponse.DeserializeLengthDelimited(stream, listFactoriesResponse);
			return listFactoriesResponse;
		}

		public static ListFactoriesResponse DeserializeLengthDelimited(Stream stream, ListFactoriesResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return ListFactoriesResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ListFactoriesResponse listFactoriesResponse = obj as ListFactoriesResponse;
			if (listFactoriesResponse == null)
			{
				return false;
			}
			if (this.Description.Count != listFactoriesResponse.Description.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Description.Count; i++)
			{
				if (!this.Description[i].Equals(listFactoriesResponse.Description[i]))
				{
					return false;
				}
			}
			if (this.HasTotalResults == listFactoriesResponse.HasTotalResults && (!this.HasTotalResults || this.TotalResults.Equals(listFactoriesResponse.TotalResults)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (GameFactoryDescription description in this.Description)
			{
				hashCode = hashCode ^ description.GetHashCode();
			}
			if (this.HasTotalResults)
			{
				hashCode = hashCode ^ this.TotalResults.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Description.Count > 0)
			{
				foreach (GameFactoryDescription description in this.Description)
				{
					num++;
					uint serializedSize = description.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			if (this.HasTotalResults)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.TotalResults);
			}
			return num;
		}

		public static ListFactoriesResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ListFactoriesResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ListFactoriesResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ListFactoriesResponse instance)
		{
			if (instance.Description.Count > 0)
			{
				foreach (GameFactoryDescription description in instance.Description)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, description.GetSerializedSize());
					GameFactoryDescription.Serialize(stream, description);
				}
			}
			if (instance.HasTotalResults)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.TotalResults);
			}
		}

		public void SetDescription(List<GameFactoryDescription> val)
		{
			this.Description = val;
		}

		public void SetTotalResults(uint val)
		{
			this.TotalResults = val;
		}
	}
}