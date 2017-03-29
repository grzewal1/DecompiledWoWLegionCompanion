using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.account
{
	public class AccountBlobList : IProtoBuf
	{
		private List<AccountBlob> _Blob = new List<AccountBlob>();

		public List<AccountBlob> Blob
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

		public List<AccountBlob> BlobList
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

		public AccountBlobList()
		{
		}

		public void AddBlob(AccountBlob val)
		{
			this._Blob.Add(val);
		}

		public void ClearBlob()
		{
			this._Blob.Clear();
		}

		public void Deserialize(Stream stream)
		{
			AccountBlobList.Deserialize(stream, this);
		}

		public static AccountBlobList Deserialize(Stream stream, AccountBlobList instance)
		{
			return AccountBlobList.Deserialize(stream, instance, (long)-1);
		}

		public static AccountBlobList Deserialize(Stream stream, AccountBlobList instance, long limit)
		{
			if (instance.Blob == null)
			{
				instance.Blob = new List<AccountBlob>();
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
						instance.Blob.Add(AccountBlob.DeserializeLengthDelimited(stream));
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

		public static AccountBlobList DeserializeLengthDelimited(Stream stream)
		{
			AccountBlobList accountBlobList = new AccountBlobList();
			AccountBlobList.DeserializeLengthDelimited(stream, accountBlobList);
			return accountBlobList;
		}

		public static AccountBlobList DeserializeLengthDelimited(Stream stream, AccountBlobList instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return AccountBlobList.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			AccountBlobList accountBlobList = obj as AccountBlobList;
			if (accountBlobList == null)
			{
				return false;
			}
			if (this.Blob.Count != accountBlobList.Blob.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Blob.Count; i++)
			{
				if (!this.Blob[i].Equals(accountBlobList.Blob[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (AccountBlob blob in this.Blob)
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
				foreach (AccountBlob blob in this.Blob)
				{
					num++;
					uint serializedSize = blob.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			return num;
		}

		public static AccountBlobList ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<AccountBlobList>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			AccountBlobList.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, AccountBlobList instance)
		{
			if (instance.Blob.Count > 0)
			{
				foreach (AccountBlob blob in instance.Blob)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, blob.GetSerializedSize());
					AccountBlob.Serialize(stream, blob);
				}
			}
		}

		public void SetBlob(List<AccountBlob> val)
		{
			this.Blob = val;
		}
	}
}