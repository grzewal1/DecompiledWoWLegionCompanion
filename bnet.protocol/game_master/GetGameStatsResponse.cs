using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.game_master
{
	public class GetGameStatsResponse : IProtoBuf
	{
		private List<GameStatsBucket> _StatsBucket = new List<GameStatsBucket>();

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

		public GetGameStatsResponse()
		{
		}

		public void AddStatsBucket(GameStatsBucket val)
		{
			this._StatsBucket.Add(val);
		}

		public void ClearStatsBucket()
		{
			this._StatsBucket.Clear();
		}

		public void Deserialize(Stream stream)
		{
			GetGameStatsResponse.Deserialize(stream, this);
		}

		public static GetGameStatsResponse Deserialize(Stream stream, GetGameStatsResponse instance)
		{
			return GetGameStatsResponse.Deserialize(stream, instance, (long)-1);
		}

		public static GetGameStatsResponse Deserialize(Stream stream, GetGameStatsResponse instance, long limit)
		{
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

		public static GetGameStatsResponse DeserializeLengthDelimited(Stream stream)
		{
			GetGameStatsResponse getGameStatsResponse = new GetGameStatsResponse();
			GetGameStatsResponse.DeserializeLengthDelimited(stream, getGameStatsResponse);
			return getGameStatsResponse;
		}

		public static GetGameStatsResponse DeserializeLengthDelimited(Stream stream, GetGameStatsResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GetGameStatsResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetGameStatsResponse getGameStatsResponse = obj as GetGameStatsResponse;
			if (getGameStatsResponse == null)
			{
				return false;
			}
			if (this.StatsBucket.Count != getGameStatsResponse.StatsBucket.Count)
			{
				return false;
			}
			for (int i = 0; i < this.StatsBucket.Count; i++)
			{
				if (!this.StatsBucket[i].Equals(getGameStatsResponse.StatsBucket[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (GameStatsBucket statsBucket in this.StatsBucket)
			{
				hashCode ^= statsBucket.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.StatsBucket.Count > 0)
			{
				foreach (GameStatsBucket statsBucket in this.StatsBucket)
				{
					num++;
					uint serializedSize = statsBucket.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			return num;
		}

		public static GetGameStatsResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetGameStatsResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetGameStatsResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetGameStatsResponse instance)
		{
			if (instance.StatsBucket.Count > 0)
			{
				foreach (GameStatsBucket statsBucket in instance.StatsBucket)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, statsBucket.GetSerializedSize());
					GameStatsBucket.Serialize(stream, statsBucket);
				}
			}
		}

		public void SetStatsBucket(List<GameStatsBucket> val)
		{
			this.StatsBucket = val;
		}
	}
}