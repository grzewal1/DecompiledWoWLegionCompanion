using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.account
{
	public class GetWalletListResponse : IProtoBuf
	{
		private List<Wallet> _Wallets = new List<Wallet>();

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public List<Wallet> Wallets
		{
			get
			{
				return this._Wallets;
			}
			set
			{
				this._Wallets = value;
			}
		}

		public int WalletsCount
		{
			get
			{
				return this._Wallets.Count;
			}
		}

		public List<Wallet> WalletsList
		{
			get
			{
				return this._Wallets;
			}
		}

		public GetWalletListResponse()
		{
		}

		public void AddWallets(Wallet val)
		{
			this._Wallets.Add(val);
		}

		public void ClearWallets()
		{
			this._Wallets.Clear();
		}

		public void Deserialize(Stream stream)
		{
			GetWalletListResponse.Deserialize(stream, this);
		}

		public static GetWalletListResponse Deserialize(Stream stream, GetWalletListResponse instance)
		{
			return GetWalletListResponse.Deserialize(stream, instance, (long)-1);
		}

		public static GetWalletListResponse Deserialize(Stream stream, GetWalletListResponse instance, long limit)
		{
			if (instance.Wallets == null)
			{
				instance.Wallets = new List<Wallet>();
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
						instance.Wallets.Add(Wallet.DeserializeLengthDelimited(stream));
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

		public static GetWalletListResponse DeserializeLengthDelimited(Stream stream)
		{
			GetWalletListResponse getWalletListResponse = new GetWalletListResponse();
			GetWalletListResponse.DeserializeLengthDelimited(stream, getWalletListResponse);
			return getWalletListResponse;
		}

		public static GetWalletListResponse DeserializeLengthDelimited(Stream stream, GetWalletListResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GetWalletListResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetWalletListResponse getWalletListResponse = obj as GetWalletListResponse;
			if (getWalletListResponse == null)
			{
				return false;
			}
			if (this.Wallets.Count != getWalletListResponse.Wallets.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Wallets.Count; i++)
			{
				if (!this.Wallets[i].Equals(getWalletListResponse.Wallets[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (Wallet wallet in this.Wallets)
			{
				hashCode ^= wallet.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Wallets.Count > 0)
			{
				foreach (Wallet wallet in this.Wallets)
				{
					num++;
					uint serializedSize = wallet.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			return num;
		}

		public static GetWalletListResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetWalletListResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetWalletListResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetWalletListResponse instance)
		{
			if (instance.Wallets.Count > 0)
			{
				foreach (Wallet wallet in instance.Wallets)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, wallet.GetSerializedSize());
					Wallet.Serialize(stream, wallet);
				}
			}
		}

		public void SetWallets(List<Wallet> val)
		{
			this.Wallets = val;
		}
	}
}