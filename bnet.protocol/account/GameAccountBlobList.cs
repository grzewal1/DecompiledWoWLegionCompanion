using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.account
{
	public class GameAccountBlobList : IProtoBuf
	{
		private List<GameAccountBlob> _Blob = new List<GameAccountBlob>();

		public List<GameAccountBlob> Blob
		{
			get
			{
				return this._Blob;
			}
			set
			{
				this._Blob = value;
			}
		}

		public int BlobCount
		{
			get
			{
				return this._Blob.Count;
			}
		}

		public List<GameAccountBlob> BlobList
		{
			get
			{
				return this._Blob;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public GameAccountBlobList()
		{
		}

		public void AddBlob(GameAccountBlob val)
		{
			this._Blob.Add(val);
		}

		public void ClearBlob()
		{
			this._Blob.Clear();
		}

		public void Deserialize(Stream stream)
		{
			GameAccountBlobList.Deserialize(stream, this);
		}

		public static GameAccountBlobList Deserialize(Stream stream, GameAccountBlobList instance)
		{
			return GameAccountBlobList.Deserialize(stream, instance, (long)-1);
		}

		public static GameAccountBlobList Deserialize(Stream stream, GameAccountBlobList instance, long limit)
		{
			if (instance.Blob == null)
			{
				instance.Blob = new List<GameAccountBlob>();
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
						instance.Blob.Add(GameAccountBlob.DeserializeLengthDelimited(stream));
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

		public static GameAccountBlobList DeserializeLengthDelimited(Stream stream)
		{
			GameAccountBlobList gameAccountBlobList = new GameAccountBlobList();
			GameAccountBlobList.DeserializeLengthDelimited(stream, gameAccountBlobList);
			return gameAccountBlobList;
		}

		public static GameAccountBlobList DeserializeLengthDelimited(Stream stream, GameAccountBlobList instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GameAccountBlobList.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameAccountBlobList gameAccountBlobList = obj as GameAccountBlobList;
			if (gameAccountBlobList == null)
			{
				return false;
			}
			if (this.Blob.Count != gameAccountBlobList.Blob.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Blob.Count; i++)
			{
				if (!this.Blob[i].Equals(gameAccountBlobList.Blob[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (GameAccountBlob blob in this.Blob)
			{
				hashCode = hashCode ^ blob.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Blob.Count > 0)
			{
				foreach (GameAccountBlob blob in this.Blob)
				{
					num++;
					uint serializedSize = blob.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			return num;
		}

		public static GameAccountBlobList ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameAccountBlobList>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameAccountBlobList.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameAccountBlobList instance)
		{
			if (instance.Blob.Count > 0)
			{
				foreach (GameAccountBlob blob in instance.Blob)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, blob.GetSerializedSize());
					GameAccountBlob.Serialize(stream, blob);
				}
			}
		}

		public void SetBlob(List<GameAccountBlob> val)
		{
			this.Blob = val;
		}
	}
}